using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NexusDB.ADOProvider;
using System.Data;
using SManApi.ArticleCommit;

namespace SManApi.ExportToPyramid
{
    public class CExportToPyramid
    {
        CDB cdb = null;
        CLog log = null;
        public CExportToPyramid()
        {
            cdb = new CDB();
            log = new CLog();
        }

        public ErrorCL exportOrder(ServHuvCL sh)
        {
            String errSt = "";
            ErrorCL err = new ErrorCL();
            err.ErrCode = 0;
            err.ErrMessage = "";
            CMServHuv servHuv = new CMServHuv();
            log.log("Före getDeliveryAddress. Kund ID = " + sh.Kund, "0");
            DataRow addr = servHuv.getDeliveryAddress(sh.Kund, ref errSt);
            if (addr == null)
                log.log("Adressen returnerar null", "0");
            log.log("Efter getDeliveryAddress ", "0");
            if (errSt != "")
            {
                err.ErrCode = -1201;
                err.ErrMessage = errSt;
                log.log("DeliveryAddress returnerar " + err.ErrMessage, "0");
                return err;
            }
            log.log("Före moms ", "0");
            String moms = "";
            if (sh.momskod == 2)
                moms = "T";
            log.log("moms = " + sh.momskod, "0");
            log.log("Före GetSoapClient", "0");
            PyramidServ.ServiceSoapClient pyramid = null;
            try
            {
                pyramid = new PyramidServ.ServiceSoapClient();
            }
            catch (Exception ex)
            {
                err.ErrCode = -14102;
                err.ErrMessage = ex.Message;
                log.log("Fel vid soapClient. Meddelande : " + err.ErrMessage, "0");
                return err;
            }
            log.log("Efter GetSoapClient", "0");
            PyramidServ.MyOrderRows myOrderRows = new PyramidServ.MyOrderRows();
            string vartOrdernr = sh.Vart_ordernr;
            log.log("Vårt ordernr : " + vartOrdernr, "0");
            try
            {
                log.log("Före pyCreateOrder ", "0");

                pyramid.PyCreateOrder(addr["foretagskod"].ToString(), ref vartOrdernr, "SM", sh.orderLabel ,"", sh.Ert_ordernr, "", moms, addr["kund"].ToString(), addr["lev_adress1"].ToString(), addr["lev_adress3"].ToString(), addr["lev_adress2"].ToString(), addr["land"].ToString(), "", myOrderRows);                
                log.log("Efter pyCreateOrder ", "0");
                pyramid.Close();
            }
            catch (Exception ex)
            {
                err.ErrCode = -1205;
                err.ErrMessage = ex.Message;
                log.log("Error message i pyCreateOrder " + ex.Message, ex.HResult.ToString());
                return err;
            }
            log.log("Före updatePyramidStatus", "0");
            updatePyramidStatus(sh.Vart_ordernr, err.ErrMessage);
            log.log("updatePyramidStatus returnerar " + err.ErrMessage, "0");
            log.log("Efter updatePyramidStatus", "0");

            return err;
        }


        private void updatePyramidStatus(string VartOrdernr, string Error)
        {

            String sSql = " update serviceHuvud "
                        + " set SentToPyramid = :SentToPyramid ";
            if (Error != "")
                sSql += ", PyramidError = :PyramidError ";
            sSql += " where vart_ordernr = :vart_ordernr ";

            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("SentToPyramid", Error == "");
            pc.Add("vart_ordernr", VartOrdernr);
            if (Error != "")
                pc.Add("PyramidError", Error.Length <= 254 ? Error : Error.Substring(0, 254));
            string errStr = "";
            cdb.updateData(sSql, ref errStr, pc);
        }

        private string exportReservdelSql(bool exported, bool bAll = false)
        {
            string sSql = " SELECT r.artnr, a.artikelkod, r.artnamn, a.artnamn origArtnamn, 0 dotProduct, sum(r.antal) sum_antal "
                         + " FROM reservdelPyr r "
                         + " join artikel a on r.artnr = a.artnr "
                         + " where r.vart_ordernr = :vart_ordernr "
                         + " and a.kategori <> 1 ";
            if (!bAll)
            {
                if (exported)
                    sSql += " and r.pyramidExport is not null ";
                else
                    sSql += " and r.pyramidExport is null ";
            }
            sSql += " group by r.artnr, a.artikelkod, a.artnamn, r.artnamn "
            + " union all "
            + " select r.artnr, r.artnr, r.artnamn, '', 1, sum(r.antal) sum_antal "
            + " from reservdelPyr r "
            + " where r.vart_ordernr = :vart_ordernr ";
            if (!bAll)
            {
                if (exported)
                    sSql += " and r.pyramidExport is not null ";
                else
                    sSql += " and r.pyramidExport is null ";
            }
            sSql += " and char_length(trim(r.artnr)) > 0 "
            + " and substring(artnr from char_length(artnr)) = '.' "
            + " group by r.artnr, r.artnamn ";
            return sSql;
        }


        public ErrorCL exportReservdel(string vartOrdernr)
        {
            ErrorCL errCl = new ErrorCL();
            errCl.ErrCode = 0;
            errCl.ErrMessage = "";
            string sSql = exportReservdelSql(false);
            /*
            string sSql = " SELECT r.artnr, a.artikelkod, r.artnamn, 0 dotProduct, sum(r.antal) sum_antal "
                         + " FROM reservdel r "
                         + " join artikel a on r.artnr = a.artnr "
                         + " where r.vart_ordernr = :vart_ordernr "
                         + " and a.kategori <> 1 "
                         + " and r.pyramidExport is null "
                         + " group by r.artnr, a.artikelkod, a.artnamn, r.artnamn "
                         + " union all "
                         + " select r.artnr, r.artnr, r.artnamn, 1, sum(r.antal) sum_antal "
                         + " from reservdel r "
                         + " where r.vart_ordernr = :vart_ordernr "
                         + " and r.pyramidExport is null "
                         + " and char_length(r.artnr) > 0 "
                         + " and substring(artnr from char_length(artnr)) = '.' "
                         + " group by r.artnr, r.artnamn ";

            */

            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("vart_ordernr", vartOrdernr);
            string errTxt = "";
            DataTable dt = cdb.getData(sSql, ref errTxt, pc);
            if (errTxt != "")
            {
                if (errTxt.Length > 2000)
                    errTxt = errTxt.Substring(1, 2000);
                errCl.ErrCode = -100;
                errCl.ErrMessage = errTxt;
                return errCl;
            }
            if (dt.Rows.Count > 0)
            {
                PyramidServ.ServiceSoapClient pyramid = new PyramidServ.ServiceSoapClient();
                foreach (DataRow dr in dt.Rows)
                {
                    string message = "";
                    string artnr = dr["artikelkod"].ToString();
                    string sum_antal = dr["sum_antal"].ToString();
                    PyramidServ.MyOrderRow orderRow = new PyramidServ.MyOrderRow();
                    orderRow.MyArtCode = artnr;
                    orderRow.MyQty = sum_antal;
                    orderRow.MyArtName = "";
                    string artnamn = dr["artnamn"].ToString();
                    string origArtnamn = dr["origArtnamn"].ToString();
                    int dotProduct = Convert.ToInt16(dr["dotProduct"]);
                    if (dotProduct == 0)
                    {
                        if (origArtnamn.Substring(origArtnamn.Length - 1) == "$")
                            orderRow.MyArtName = artnamn;
                    }
                    else
                        orderRow.MyArtName = artnamn;
                    
                    if (Convert.ToDecimal(sum_antal) >= 0.001M || Convert.ToDecimal(sum_antal) <= -0.001M)
                        message = pyOrderRow(vartOrdernr, orderRow);                    
                    errCl = updateReservdel(vartOrdernr, artnr, message);
                    addToPyramidOrder(vartOrdernr, artnr, Convert.ToDecimal(sum_antal), orderRow.MyArtName);
                }
            }
            return errCl;
        }


        private ErrorCL updateReservdel(string vartOrdernr, string artnr, string error)
        {
            ErrorCL errCl = new ErrorCL();
            errCl.ErrCode = 0;
            errCl.ErrMessage = "";
            string sSql = " select r.id "
                        + " FROM reservdelPyr r "
                        + " join artikel a on r.artnr = a.artnr "
                        + " where r.vart_ordernr = :vart_ordernr "
                        + " and a.kategori <> 1 "
                        + " and r.pyramidExport is null "
                        + " and r.artnr = :artnr "
                        + " union all "
                        + " select r.id "
                        + " from reservdelPyr r "
                        + " where r.vart_ordernr = :vart_ordernr "
                        + " and r.pyramidExport is null "
                        + " and char_length(r.artnr) > 0 "
                        + " and substring(artnr from char_length(artnr)) = '.' "
                        + " and r.artnr = :artnr ";
            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("vart_ordernr", vartOrdernr);
            pc.Add("artnr", artnr);
            string errTxt = "";
            DataTable dt = cdb.getData(sSql, ref errTxt, pc);
            if (errTxt != "")
            {
                errTxt = "Error while selecting reservdel after pyramidExport. Message : " + errTxt;
                if (errTxt.Length > 2000)
                    errTxt = errTxt.Substring(1, 2000);
                errCl.ErrCode = -100;
                errCl.ErrMessage = errTxt;
                return errCl;
            }

            if (dt.Rows.Count == 0)
                return errCl;
            string sSqlUpdate = " update reservdelPyr "
                            + " set pyramidExport = :pyramidExport "
                            + ", pyramidError = :pyramidError "
                            + " where id = :id ";
            NxConnection cn = cdb.getConn();
            NxCommand cmUpdate = new NxCommand(sSqlUpdate, cn);
            cmUpdate.Parameters.Add("pyramidExport", DateTime.Now);
            if (error != "")
                cmUpdate.Parameters["pyramidExport"].Value = null;
            cmUpdate.Parameters.Add("id", DbType.Int32);
            cmUpdate.Parameters.Add("pyramidError", DbType.String);
            foreach (DataRow dr in dt.Rows)
            {
                int id = Convert.ToInt32(dr["id"]);
                try
                {
                    cmUpdate.Parameters["id"].Value = id;                    
                    if (error == "")
                    {
                        cmUpdate.Parameters["pyramidExport"].Value = DateTime.Now;
                        cmUpdate.Parameters["pyramidError"].Value = DBNull.Value;
                    }
                    else
                    {
                        cmUpdate.Parameters["pyramidExport"].Value = null;
                        if (error.Length > 50)
                            error = error.Substring(0, 50);
                        cmUpdate.Parameters["pyramidError"].Value = error;
                    }

                    cn.Open();
                    cmUpdate.ExecuteNonQuery();
                    cn.Close();
                }
                catch (Exception ex)
                {
                    errTxt = "Error while updating reservdel after pyramidExport. Message : " + ex.Message;
                    if (errTxt.Length > 2000)
                        errTxt = errTxt.Substring(1, 2000);
                    errCl.ErrCode = -100;
                    errCl.ErrMessage = errTxt;
                    return errCl;
                }
            }
            return errCl;
        }




        /// <summary>
        /// Exports all timerecords to Pyramid by
        /// aggregating on konto (=artikel)
        /// </summary>
        /// <param name="vartOrdernr"></param>
        /// <returns></returns>
        /// 2018-05-03 KJBO Indentive AB
        public ErrorCL exportTime(string vartOrdernr)
        {
            ErrorCL errCl = exportTimeSH(vartOrdernr);
            if (errCl.ErrCode != 0)
                return errCl;
            errCl = exportTimeSR(vartOrdernr);
            return errCl;
        }


        private string exportTimeSHSql(bool exported, bool approved, bool all = false)
        {
            string sSql = " SELECT a.accountNo, coalesce(ar.artnamn,'') artnamn, sum(srt.tid) sumTid "
                        + " FROM ServHuvRepTid srt "
                        + " join salart s on srt.salartID = s.salartID "
                        + " join account a on s.salartID = a.salartID and coalesce(srt.rep_kat_id,'AL_ST') = a.rep_kat_id "
                        + " join timeRep2Week tr2w on srt.timeRep2WeekId = tr2w.id "
                        + " left outer join artikel ar on a.accountNo = ar.artnr "
                        + " where srt.vart_ordernr = :vart_ordernr ";
            if (!all)
            {
                if (exported)
                    sSql += " and srt.pyramidExport is not null ";
                else
                    sSql += " and srt.pyramidExport is null ";
                if (approved)
                {
                    sSql += " and tr2w.approved = true "
                        + " and srt.attesterad = true ";
                }
            }
            sSql += " and srt.timeTypeID = 1 "
            + " group by a.accountNo, ar.artnamn ";
            return sSql;
        }

        private ErrorCL exportTimeSH(string vartOrdernr)
        {
            ErrorCL errCl = new ErrorCL();
            errCl.ErrCode = 0;
            errCl.ErrMessage = "";
            string sSql = exportTimeSHSql(false, true);
            /*
            string sSql = " SELECT a.accountNo, coalesce(ar.artnamn,'') artnamn, sum(srt.tid) sumTid "
                        + " FROM ServHuvRepTid srt "
                        + " join salart s on srt.salartID = s.salartID "
                        + " join account a on s.salartID = a.salartID and coalesce(srt.rep_kat_id,'AL_ST') = a.rep_kat_id "
                        + " join timeRep2Week tr2w on srt.timeRep2WeekId = tr2w.id "
                        + " left outer join artikel ar on a.accountNo = ar.artnr "
                        + " where srt.vart_ordernr = :vart_ordernr "
                        + " and srt.pyramidExport is null "
                        + " and tr2w.approved = true "
                        + " and srt.timeTypeID = 1 "
                        + " group by a.accountNo, ar.artnamn ";
                        */
            string errTxt = "";
            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("vart_ordernr", vartOrdernr);
            DataTable dt = cdb.getData(sSql, ref errTxt, pc);

            if (errTxt != "")
            {
                if (errTxt.Length > 2000)
                    errTxt = errTxt.Substring(1, 2000);
                errCl.ErrCode = -100;
                errCl.ErrMessage = errTxt;
                return errCl;
            }
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    string message = "";
                    string artnr = dr["accountNo"].ToString();
                    Decimal sumTid = Convert.ToDecimal(dr["sumTid"]);

                    PyramidServ.MyOrderRow orderRow = new PyramidServ.MyOrderRow();
                    orderRow.MyArtCode = artnr;
                    orderRow.MyQty = dr["sumTid"].ToString();
                    // 2018-05-30 KJBO
                    //orderRow.MyArtName = dr["artnamn"].ToString();
                    orderRow.MyArtName = "";
                    message = pyOrderRow(vartOrdernr, orderRow);
                    errCl = markShExported(vartOrdernr, artnr, message);
                    addToPyramidOrder(vartOrdernr, artnr, Convert.ToDecimal(sumTid), orderRow.MyArtName);
                }
            }
            return errCl;
        }


        private ErrorCL markShExported(string vartOrdernr, string accountNo, string error)
        {
            ErrorCL errCl = new ErrorCL();
            errCl.ErrCode = 0;
            errCl.ErrMessage = "";
            string sSql = "SELECT srt.ID "
                        + " FROM ServHuvRepTid srt "
                        + " join salart s on srt.salartID = s.salartID "
                        + " join account a on s.salartID = a.salartID and coalesce(srt.rep_kat_id,'AL_ST') = a.rep_kat_id "
                        + " join timeRep2Week tr2w on srt.timeRep2WeekId = tr2w.id "
                        + " where srt.vart_ordernr = :vart_ordernr "
                        + " and srt.pyramidExport is null "
                        + " and tr2w.approved = true "
                        + " and srt.attesterad = true "
                        + " and srt.timeTypeID = 1 "
                        + " and a.accountNo = :accountNo ";
            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("vart_ordernr", vartOrdernr);
            pc.Add("accountNo", accountNo);

            string sSqlUpdate = " update ServHuvRepTid "
                              + " set pyramidExport = :pyramidExport "
                              + " , pyramidError = :pyramidError "
                              + " where ID = :ID ";
            NxConnection cn = cdb.getConn();
            NxCommand cmUpdate = new NxCommand(sSqlUpdate, cn);
            DateTime dtNow = DateTime.Now;
            cmUpdate.Parameters.Add("pyramidExport", dtNow);
            cmUpdate.Parameters.Add("ID", DbType.Int32);
            cmUpdate.Parameters.Add("pyramidError", DbType.String);
            string errTxt = "";
            DataTable dt = cdb.getData(sSql, ref errTxt, pc);

            if (errTxt != "")
            {
                if (errTxt.Length > 2000)
                    errTxt = errTxt.Substring(1, 2000);
                errCl.ErrCode = -100;
                errCl.ErrMessage = errTxt;
                return errCl;
            }

            foreach (DataRow dr in dt.Rows)
            {
                int id = Convert.ToInt32(dr["ID"]);
                try
                {
                    cmUpdate.Parameters["ID"].Value = id;
                    if (error == "")
                    {
                        cmUpdate.Parameters["pyramidError"].Value = DBNull.Value;
                        cmUpdate.Parameters["pyramidExport"].Value = dtNow;
                    }
                    else
                    {
                        if (error.Length > 50)
                            error = error.Substring(0, 50);
                        cmUpdate.Parameters["pyramidError"].Value = error;
                        cmUpdate.Parameters["pyramidExport"].Value = DBNull.Value;
                    }
                    cn.Open();
                    cmUpdate.ExecuteNonQuery();
                    cn.Close();
                }
                catch (Exception ex)
                {
                    errCl.ErrCode = -101;
                    errTxt = "Error while updating PyramidExport on ServHuvRepTid. Message : " + ex.Message;
                    if (errTxt.Length > 2000)
                        errTxt = errTxt.Substring(1, 2000);
                    errCl.ErrCode = -100;
                    errCl.ErrMessage = errTxt;
                    return errCl;
                }
            }
            return errCl;
        }

        private string exportTimeSRSql(bool exported, bool approved, bool all = false)
        {
            string sSql = " SELECT a.accountNo, coalesce(ar.artnamn,'') artnamn, sum(srt.tid) sumTid "
                        + " FROM ServRadRepTid srt "
                        + " join Servicerad sr on srt.srAltKey = sr.alternateKey "
                        + " join salart s on srt.salartID = s.salartID "
                        + " join account a on s.salartID = a.salartID and srt.rep_kat_id = a.rep_kat_id "
                        + " join timeRep2Week tr2w on srt.timeRep2WeekId = tr2w.id "
                        + " left outer join artikel ar on a.accountNo = ar.artnr "
                        + " where sr.vart_ordernr = :vart_ordernr ";
            if (!all)
            {
                if (exported)
                    sSql += " and srt.pyramidExport is not null ";
                else
                    sSql += " and srt.pyramidExport is null ";
                if (approved)
                {
                    sSql += " and tr2w.approved = true "
                        + " and srt.attesterad = true ";
                }
            }
            sSql += " and srt.timeTypeID = 1 "
            + " group by a.accountNo, ar.artnamn ";
            return sSql;
        }

        private ErrorCL exportTimeSR(string vartOrdernr)
        {
            ErrorCL errCl = new ErrorCL();
            errCl.ErrCode = 0;
            errCl.ErrMessage = "";
            string sSql = exportTimeSRSql(false, true);
            /*
            string sSql = " SELECT a.accountNo, coalesce(ar.artnamn,'') artnamn, sum(srt.tid) sumTid "
                        + " FROM ServRadRepTid srt "
                        + " join Servicerad sr on srt.srAltKey = sr.alternateKey "
                        + " join salart s on srt.salartID = s.salartID "
                        + " join account a on s.salartID = a.salartID and srt.rep_kat_id = a.rep_kat_id "
                        + " join timeRep2Week tr2w on srt.timeRep2WeekId = tr2w.id "
                        + " left outer join artikel ar on a.accountNo = ar.artnr "
                        + " where sr.vart_ordernr = :vart_ordernr "
                        + " and srt.pyramidExport is null "
                        + " and tr2w.approved = true "
                        + " and srt.timeTypeID = 1 "
                        + " group by a.accountNo, ar.artnamn ";
                        */
            string errTxt = "";
            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("vart_ordernr", vartOrdernr);
            DataTable dt = cdb.getData(sSql, ref errTxt, pc);

            if (errTxt != "")
            {
                if (errTxt.Length > 2000)
                    errTxt = errTxt.Substring(1, 2000);
                errCl.ErrCode = -100;
                errCl.ErrMessage = errTxt;
                return errCl;
            }
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    string message = "";
                    string artnr = dr["accountNo"].ToString();
                    Decimal sumTid = Convert.ToDecimal(dr["sumTid"]);
                    PyramidServ.MyOrderRow orderRow = new PyramidServ.MyOrderRow();
                    orderRow.MyArtCode = artnr;
                    orderRow.MyQty = dr["sumTid"].ToString();
                    // 2018-05-30 KJBO
                    orderRow.MyArtName = "";
                    message = pyOrderRow(vartOrdernr, orderRow);
                    errCl = markSrExported(vartOrdernr, artnr, message);
                    addToPyramidOrder(vartOrdernr, artnr, Convert.ToDecimal(sumTid), orderRow.MyArtName);
                }
            }
            return errCl;
        }

        private ErrorCL markSrExported(string vartOrdernr, string accountNo, string error)
        {
            ErrorCL errCl = new ErrorCL();
            errCl.ErrCode = 0;
            errCl.ErrMessage = "";

            string sSql = "SELECT srt.ID "
                         + " FROM ServRadRepTid srt "
                         + " join Servicerad sr on srt.srAltKey = sr.alternateKey "
                         + " join salart s on srt.salartID = s.salartID "
                         + " join account a on s.salartID = a.salartID and srt.rep_kat_id = a.rep_kat_id "
                         + " join timeRep2Week tr2w on srt.timeRep2WeekId = tr2w.id "
                         + " where sr.vart_ordernr = :vart_ordernr "
                         + " and srt.pyramidExport is null "
                         + " and tr2w.approved = true "
                         + " and srt.attesterad = true "
                         + " and srt.timeTypeID = 1 "
                         + " and a.accountNo = :accountNo ";

            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("vart_ordernr", vartOrdernr);
            pc.Add("accountNo", accountNo);

            string sSqlUpdate = " update ServRadRepTid "
                              + " set pyramidExport = :pyramidExport "
                              + " , pyramidError = :pyramidError "
                              + " where ID = :ID ";
            NxConnection cn = cdb.getConn();
            NxCommand cmUpdate = new NxCommand(sSqlUpdate, cn);
            DateTime dtNow = DateTime.Now;
            cmUpdate.Parameters.Add("pyramidExport", dtNow);
            cmUpdate.Parameters.Add("pyramidError", DbType.String);
            cmUpdate.Parameters.Add("ID", DbType.Int32);
            string errTxt = "";
            DataTable dt = cdb.getData(sSql, ref errTxt, pc);

            if (errTxt != "")
            {
                if (errTxt.Length > 2000)
                    errTxt = errTxt.Substring(1, 2000);
                errCl.ErrCode = -100;
                errCl.ErrMessage = errTxt;
                return errCl;
            }

            foreach (DataRow dr in dt.Rows)
            {
                int id = Convert.ToInt32(dr["ID"]);
                try
                {
                    cmUpdate.Parameters["ID"].Value = id;
                    if (error == "")
                    {
                        cmUpdate.Parameters["pyramidExport"].Value = dtNow;
                        cmUpdate.Parameters["pyramidError"].Value = DBNull.Value;
                    }
                    else
                    {
                        cmUpdate.Parameters["pyramidExport"].Value = DBNull.Value;
                        if (error.Length > 50)
                            error = error.Substring(0, 50);
                        cmUpdate.Parameters["pyramidError"].Value = error;

                    }
                    cn.Open();
                    cmUpdate.ExecuteNonQuery();
                    cn.Close();
                }
                catch (Exception ex)
                {
                    errCl.ErrCode = -101;
                    errTxt = "Error while updating PyramidExport on ServRadRepTid. Message : " + ex.Message;
                    if (errTxt.Length > 2000)
                        errTxt = errTxt.Substring(1, 2000);
                    errCl.ErrCode = -100;
                    errCl.ErrMessage = errTxt;
                    return errCl;
                }
            }
            return errCl;
        }

        private int getNextPyramidRow(DataTable dt)
        {
            int row = 0;
            foreach (DataRow dr in dt.Rows)
                if (Convert.ToInt32(dr["radnr"]) > row)
                    row = Convert.ToInt32(dr["radnr"]);
            row++;
            return row;
        }

        private void updatePyramidRow(string vartOrdernr, int radnr, string artnr, Decimal antal)
        {
            string sSql = " update PyramidOrderPris "
                        + " set antal = antal + " + antal.ToString() + " "
                        + " where vart_ordernr = '" + vartOrdernr + "' "
                        + " and radnr = " + radnr.ToString() + " "
                        + " and artnr = '" + artnr + "' ";
            string dummy = "";
            cdb.updateData(sSql, ref dummy);
        }

        private void addPyramidRow(string vartOrdernr, int radnr, string artnr, Decimal antal, string artnamn)
        {
            string sSql = " insert into PyramidOrderPris (vart_ordernr, radnr, artnr, antal, artnamn) "
                        + " values (:vart_ordernr, :radnr, :artnr, :antal, :artnamn) ";
            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("vart_ordernr", vartOrdernr);
            pc.Add("radnr", radnr);
            pc.Add("artnr", artnr);
            pc.Add("antal", antal);
            pc.Add("artnamn", artnamn);
            string dummy = "";
            cdb.updateData(sSql, ref dummy, pc);
        }


        private void addToPyramidOrder(string vartOrdernr, string artnr, Decimal antal, string artnamn)
        {
            string sSql = " SELECT vart_ordernr, radnr, artnr, antal "
                        + " FROM PyramidOrderPris "
                        + " where vart_ordernr = '" + vartOrdernr + "' ";
            string dummy = "";
            int row = 0;
            DataTable dt = cdb.getData(sSql, ref dummy);
            DataRow[] drs = dt.Select("artnr = '" + artnr + "'");
            if (drs != null && drs.Length > 0)
            {
                DataRow dr = drs[0];
                row = Convert.ToInt32(dr["radnr"]);
            }
            else
                row = getNextPyramidRow(dt);
            if (drs.Length > 0 && artnamn == "")
                updatePyramidRow(vartOrdernr, row, artnr, antal);
            else
                addPyramidRow(vartOrdernr, row, artnr, antal, artnamn);
        }



        /// <summary>
        /// Reserverar artikel i pyramid.
        /// Artikeln hämtas genom att kontrollera orderArt tabellen
        /// med medskickat ID.
        /// Slutligen "prickas raden av" så att vi vet vilka som skickats till
        /// Pyramid.
        /// </summary>
        /// <param name="orderArtId"></param>
        /// <returns></returns>
        public ErrorCL reserveArticle(int orderArtId)
        {
            ErrorCL errCl = new ErrorCL();
            errCl.ErrCode = 0;
            errCl.ErrMessage = "";
            string sSql = " select coalesce(sum(oa.coAntal),0) sum_antal "
                        + " from orderArt oa "
                        + " where oa.orderArtId = :orderArtId ";
            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("orderArtId", orderArtId);
            string errTxt = "";
            DataTable dt = cdb.getData(sSql, ref errTxt, pc);

            if (errTxt != "")
            {
                if (errTxt.Length > 2000)
                    errTxt = errTxt.Substring(1, 2000);
                errCl.ErrCode = -100;
                errCl.ErrMessage = errTxt;
                return errCl;

            }
            if (dt.Rows.Count > 0)
            {
                Decimal sum_antal = Convert.ToDecimal(dt.Rows[0]["sum_antal"]);
                Decimal sum_handled = alreadyHandled(orderArtId);
                if (sum_antal - sum_handled != 0)
                {
                    createReservation(orderArtId, sum_antal - sum_handled, ref errTxt);
                    if (errTxt != "")
                    {
                        errCl.ErrMessage = errTxt;
                        errCl.ErrCode = -14364;
                    }
                }
            }
            return errCl;


        }

        private Decimal alreadyHandled(int orderArtId)
        {
            string sSql = " select coalesce(sum(oap.antal),0) sum_antal "
                        + " from oaPyramid oap "
                        + " where oap.orderArtId = :orderArtId ";
            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("orderArtId", orderArtId);
            string dummy = "";
            DataTable dt = cdb.getData(sSql, ref dummy, pc);
            if (dt.Rows.Count > 0)
                return Convert.ToDecimal(dt.Rows[0]["sum_antal"]);
            return 0;
        }

        private void createReservation(int orderArtId, decimal antal, ref string errTxt)
        {
            string vartOrdernr = "";
            DataTable dtArtikelRows = getArtikelFromOA(orderArtId, ref errTxt);
            if (errTxt == "")
            {

                if (dtArtikelRows.Rows.Count == 0)
                {
                    errTxt = "Kan ej hitta artikel för att uppdatera Pyramid";
                    return;
                }
            }
            if (errTxt == "")
            {
                DataRow dr = dtArtikelRows.Rows[0];
                vartOrdernr = dr["vart_ordernr"].ToString();
                PyramidServ.MyOrderRow or = new PyramidServ.MyOrderRow();
                or.MyArtCode = dr["artikelkod"].ToString();
                or.MyQty = antal.ToString();
                // 2018-05-29 KJBO
                //or.MyArtName = dr["artnamn"].ToString();
                or.MyArtName = "";
                errTxt = pyOrderRow(vartOrdernr, or);
                markOrderArtExported(orderArtId, antal, ref errTxt);
                addToPyramidOrder(vartOrdernr, or.MyArtCode, antal, or.MyArtName);
            }
            return;
        }

        public void retryAllOrders()
        {
            string sSql = " SELECT orderArtId "
                        + " FROM orderArt ";
            string dummy = "";
            NxParameterCollection pc = new NxParameterCollection();
            DataTable dt = cdb.getData(sSql, ref dummy, pc);
            foreach (DataRow dr in dt.Rows)
                reserveArticle(Convert.ToInt32(dr["orderArtId"]));

        }

        public void retryOrders(string vartOrdernr)
        {
            string sSql = " SELECT orderArtId "
                        + " FROM orderArt "
                        + " where vart_ordernr = :vart_ordernr ";
            string dummy = "";
            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("vart_ordernr", vartOrdernr);
            DataTable dt = cdb.getData(sSql, ref dummy, pc);
            foreach (DataRow dr in dt.Rows)
                reserveArticle(Convert.ToInt32(dr["orderArtId"]));
        }

        private DataTable getArtikelFromOA(int orderArtId, ref string errTxt)
        {
            string sSql = " SELECT oa.vart_ordernr, a.artnr, a.artikelkod, a.artnamn "
                        + " FROM orderArt oa "
                        + " join artikel a on oa.artnr = a.artnr "
                        + " where oa.orderArtId = :orderArtId ";
            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("orderArtId", orderArtId);
            return cdb.getData(sSql, ref errTxt, pc);
        }

        private void markOrderArtExported(int orderArtId, Decimal antal, ref string error)
        {
            ErrorCL errCl = new ErrorCL();
            errCl.ErrCode = 0;
            errCl.ErrMessage = "";
            string sSql = " insert into oaPyramid (orderArtId, antal, regdat, result, resultDescr) "
                        + " values(:orderArtId, :antal, :regdat, :result, :resultDescr) ";
            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("orderArtId", orderArtId);
            pc.Add("antal", DbType.Decimal);
            pc.Add("result", DbType.Int16);
            if (error == "")
            {
                pc["antal"].Value = antal;
                pc["result"].Value = 1;
            }
            else
            {
                pc["antal"].Value = 0;
                pc["result"].Value = 0;
            }
            if (error.Length > 50)
                error = error.Substring(0, 50);
            pc.Add("resultDescr", error);
            DateTime now = DateTime.Now;
            pc.Add("regdat", now);
            string errTxt = "";
            cdb.updateData(sSql, ref errTxt, pc);
        }

        public void setOrderStatus(string vartOrdernr)
        {
            string sSql = " select godkand "
                        + " from servicehuvud "
                        + " where vart_ordernr = :vart_ordernr ";
            string error = "";
            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("vart_ordernr", vartOrdernr);
            DataTable dt = cdb.getData(sSql, ref error, pc);
            if (dt.Rows.Count == 1)
            {
                string sGodk = "";
                bool godkand = Convert.ToBoolean(dt.Rows[0]["godkand"]);
                if (godkand)
                    sGodk = "G";

                PyramidServ.ServiceSoapClient pyramidCl = new PyramidServ.ServiceSoapClient();
                try
                {
                    string result = pyramidCl.PyOrderSetStatus(vartOrdernr, ref sGodk);
                    pyramidCl.Close();
                }
                catch (Exception ex)
                {
                    string st = ex.Message;
                    //if (message == "" && ex.HResult != -2146233087)
                    //    message = ex.Message;
                }
            }
        }

        public ErrorCL exportReservdelKat1(string vartOrdernr)
        {
            ServHuvSrc.COrderArt coa = new ServHuvSrc.COrderArt();
            ErrorCL errCl = new ErrorCL();
            errCl.ErrCode = 0;
            errCl.ErrMessage = "";

            string sSql = " SELECT r.artnr, a.artikelkod, a.artnamn, coalesce(sum(r.antal),0) sum_antal "
                       + "  FROM reservdel r "
                       + "  join artikel a on r.artnr = a.artnr "
                       + "  where r.vart_ordernr = :vart_ordernr "
                       + "  and a.kategori = 1 "
                       + " group by r.artnr, a.artikelkod, a.artnamn ";
            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("vart_ordernr", vartOrdernr);
            string errTxt = "";
            DataTable dt = cdb.getData(sSql, ref errTxt, pc);
            if (errTxt != "")
            {
                if (errTxt.Length > 2000)
                    errTxt = errTxt.Substring(1, 2000);
                errCl.ErrCode = -100;
                errCl.ErrMessage = errTxt;
                return errCl;
            }
            List<CArticleCommitData> acDataList = new List<CArticleCommitData>();
            foreach (DataRow dr in dt.Rows)
            {
                decimal countOutchecked = coa.countOutcheckedArt(vartOrdernr, dr["artnr"].ToString());
                decimal sumAntal = Convert.ToDecimal(dr["sum_antal"]);                
                if (sumAntal > countOutchecked)
                {

                    OrderArtCL oa = new OrderArtCL();
                    oa.VartOrdernr = vartOrdernr;
                    oa.Artnr = dr["artnr"].ToString();
                    oa.CoAntal = sumAntal - countOutchecked;
                    oa = coa.checkoutOrderArt(oa, "", true);

                    // 2018-08-28 KJBO
                    CArticleCommitData acData = new CArticleCommitData();
                    acData.articleNumber = oa.Artnr;
                    acData.orderArtID = oa.OrderArtId;
                    acData.orderNumber = oa.VartOrdernr;
                    acData.quantity = oa.CoAntal;
                    acDataList.Add(acData);
                    
                }
            }
            if (acDataList.Count > 0)
            {
                CArticleCommit ac = new CArticleCommit();
                errCl = ac.generateFile(acDataList,"2");
                if (errCl.ErrCode != 0)
                    return errCl;
            }
            // Now check if any articles are checkedout but not checkedin
            ServHuvSrc.COrderArt oArt = new ServHuvSrc.COrderArt();
            // Get a list of all checked out items
            List<OrderArtCL> oaList = oArt.getOrderArt(vartOrdernr);
            acDataList.Clear();
            foreach (OrderArtCL orderArt in oaList)
            {
                // Init variable
                Decimal countOnOrder = 0;
                // Now check if the outchecked item exists on order...
                DataRow[] orderSums = dt.Select("artnr = '" + orderArt.Artnr + "' ");
                // .. get the number or order
                if (orderSums != null && orderSums.Length > 0)                
                    countOnOrder = Convert.ToDecimal(orderSums[0]["sum_antal"]); 
                // Now calculate how many items that shall be inchecked
                Decimal toCheckIn = orderArt.CoAntal - orderArt.CiAntal - countOnOrder;
                if (toCheckIn > 0)
                {
                    // Do checkin
                    errCl = oArt.updateCiAntal(orderArt.OrderArtId, toCheckIn + orderArt.CiAntal);
                    if (errCl.ErrCode != 0)
                        return errCl;


                    // Now commit the outchecked items
                    CArticleCommitData acData = new CArticleCommitData();
                    acData.articleNumber = orderArt.Artikelkod;
                    acData.orderArtID = orderArt.OrderArtId;
                    acData.orderNumber = vartOrdernr;
                    // Note that the number has to be negative
                    acData.quantity = -toCheckIn;
                    acDataList.Add(acData);
                }
            }
            if (acDataList.Count > 0)
            {
                CArticleCommit ac = new CArticleCommit();
                errCl = ac.generateFile(acDataList,"3");
                if (errCl.ErrCode != 0)
                    return errCl;
            }
            return errCl;
        }

        private string pyOrderRow(string vartOrdernr, PyramidServ.MyOrderRow orderRow)
        {
            string message = "";
            PyramidServ.ServiceSoapClient pyramid = new PyramidServ.ServiceSoapClient();
            try
            {
                PyramidServ.MyOrderRows orderRows = new PyramidServ.MyOrderRows();                
                orderRow.MyQty = orderRow.MyQty.Replace(",", ".");
                orderRows.Add(orderRow);
                string pyResult = pyramid.PyOrderRow(vartOrdernr, orderRows, out message);                                      
                //string pyResult = pyramid.PyOrderRowEx(vartOrdernr, orderRows, out message);
                if (pyResult == "true")
                    message = "";
                if (message == null)
                    message = "";
                pyramid.Close();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return message;
        }

        //private DataTable getExportTimeSH(string vartOrdernr, bool exported, bool approved, ref string error)
        private DataTable getExportTimeSH(string vartOrdernr, int listType, ref string error)
        {
            bool exported = false;            
            bool bAll = false;
            if (listType == 2)            
                exported = true;
            if (listType == 3)
                bAll = true;
            string sSql = exportTimeSHSql(exported, false, bAll);
            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("vart_ordernr", vartOrdernr);
            error = "";
            DataTable dt = cdb.getData(sSql, ref error, pc);
            return dt;
        }

        //private DataTable getExportTimeSR(string vartOrdernr, bool exported, bool approved, ref string error)
        private DataTable getExportTimeSR(string vartOrdernr,int listType, ref string error)
        {
            bool exported = false;
            bool bAll = false;
            if (listType == 2)
                exported = true;
            if (listType == 3)
                bAll = true;
            string sSql = exportTimeSRSql(exported, false, bAll);
            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("vart_ordernr", vartOrdernr);
            error = "";
            DataTable dt = cdb.getData(sSql, ref error, pc);
            return dt;
        }

        private DataTable getExportReservdel (string vartOrdernr, int listType, ref string error)
        {
            bool exported = false;
            bool bAll = false;
            if (listType == 2)
                exported = true;
            if (listType == 3)
                bAll = true;
            string sSql = exportReservdelSql(exported, bAll);
            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("vart_ordernr", vartOrdernr);
            error = "";
            DataTable dt = cdb.getData(sSql, ref error, pc);
            return dt;
        }

        /// <summary>
        /// Retuns a key-value list of available selection
        /// for the below getExportFullList
        /// </summary>
        /// <param name="ident"></param>
        /// <returns></returns>
        /// 2018-05-31 KJBO
        public List<KeyValuePair<int, string>> getExportListSelection(string ident)
        {
            List<KeyValuePair<int, string>> list = new List<KeyValuePair<int, string>>();

            CReparator cr = new CReparator();
            int identOK = cr.checkIdent(ident);
            if (identOK == -1)
            {
                list.Add(new KeyValuePair<int, string>(-10, "Ogiltigt login"));
                return list;
            }
            list.Add(new KeyValuePair<int, string>(1, "Ej Exporterad"));
            list.Add(new KeyValuePair<int, string>(2, "Exporterade"));
            list.Add(new KeyValuePair<int, string>(3, "Alla"));
            return list;
        }




        /// <summary>
        /// Get a full export list for one order. This list can reflect all items that will be exported
        /// or all items that is already exported by setting the exported boolean flag.
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="vartOrdernr"></param>
        /// <param name="exported"></param>
        /// <returns>A list of orderArtCL type</returns>
        /// 2018-05-30 KJBO Indentive AB
        public List<OrderArtCL> getExportFullList(string ident, string vartOrdernr, int listType)
        {
            List<OrderArtCL> returnOaList = new List<OrderArtCL>();
            if (listType >= 2)
            {
                ServHuvSrc.COrderArt oart = new ServHuvSrc.COrderArt();
                List<OrderArtCL> orderArtList = oart.getOrderArt(0, vartOrdernr, ident, false);
                foreach (OrderArtCL oa in orderArtList)
                {
                    OrderArtCL returnOa = new OrderArtCL();
                    returnOa.Artnr = oa.Artnr;
                    returnOa.Artikelkod = oa.Artnr;
                    returnOa.ArtNamn = oa.ArtNamn;
                    returnOa.CoAntal = oa.CoAntal - oa.CiAntal;
                    returnOa.ErrCode = oa.ErrCode;
                    returnOa.ErrMessage = oa.ErrMessage;
                    if (returnOa.CoAntal > 0.001M)
                        returnOaList.Add(returnOa);
                    if (oa.ErrCode != 0)
                        return returnOaList;
                }
            }
            string error = "";
            DataTable dt = getExportTimeSH(vartOrdernr, listType, ref error);
            // Error handling
            if (error != "")
            {
                error = "Error on getExportTimeSH " + error;
                returnOaList.Clear();
                OrderArtCL returnOa = new OrderArtCL();
                returnOa.ErrCode = -100;
                if (error.Length > 2000)
                    error = error.Substring(1, 2000);
                returnOa.ErrMessage = error;
                returnOaList.Add(returnOa);
                return returnOaList;
            }
            // End of error handling
            foreach (DataRow dr in dt.Rows)
            {
                OrderArtCL returnOa = new OrderArtCL();
                returnOa.Artnr = dr["accountNo"].ToString();
                returnOa.Artikelkod = dr["accountNo"].ToString();
                returnOa.ArtNamn = dr["artnamn"].ToString();
                returnOa.CoAntal = Convert.ToDecimal(dr["sumTid"]);
                if (listType != 2)
                    returnOa.note = "Attesterade och ej attesterade";
                if (returnOa.CoAntal > 0.001M)
                    returnOaList.Add(returnOa);
            }
            error = "";
            dt = getExportTimeSR(vartOrdernr, listType, ref error);
            // Error handling
            if (error != "")
            {
                error = "Error on getExportTimeSR " + error;
                returnOaList.Clear();
                OrderArtCL returnOa = new OrderArtCL();
                returnOa.ErrCode = -100;
                if (error.Length > 2000)
                    error = error.Substring(1, 2000);
                returnOa.ErrMessage = error;
                returnOaList.Add(returnOa);
                return returnOaList;
            }
            // End of error handling
            foreach (DataRow dr in dt.Rows)
            {
                OrderArtCL returnOa = new OrderArtCL();
                returnOa.Artnr = dr["accountNo"].ToString();
                returnOa.Artikelkod = dr["accountNo"].ToString();
                returnOa.ArtNamn = dr["artnamn"].ToString();
                returnOa.CoAntal = Convert.ToDecimal(dr["sumTid"]);
                if (listType != 2)
                    returnOa.note = "Attesterade och ej attesterade";
                returnOaList.Add(returnOa);
            }
            error = "";
            dt = getExportReservdel(vartOrdernr, listType, ref error);
            // Error handling
            if (error != "")
            {
                error = "Error on getExportReservdel " + error;
                returnOaList.Clear();
                OrderArtCL returnOa = new OrderArtCL();
                returnOa.ErrCode = -100;
                if (error.Length > 2000)
                    error = error.Substring(1, 2000);
                returnOa.ErrMessage = error;                
                returnOaList.Add(returnOa);
                return returnOaList;
            }
            // End of error handling
            foreach (DataRow dr in dt.Rows)
            {
                OrderArtCL returnOa = new OrderArtCL();
                returnOa.Artnr = dr["artnr"].ToString();
                returnOa.Artikelkod = dr["artikelkod"].ToString();
                returnOa.ArtNamn = dr["artnamn"].ToString();
                returnOa.CoAntal = Convert.ToDecimal(dr["sum_antal"]);
                if (returnOa.CoAntal > 0.001M)
                    returnOaList.Add(returnOa);
            }
            return returnOaList;
            
        }


        /// <summary>
        /// Check if pyramid API is available
        /// </summary>
        /// <returns></returns>
        public string checkPyramidAPIAvailable()
        {
            string rc = "";
            string status = "";
            try
            { 
                PyramidServ.ServiceSoapClient client = new PyramidServ.ServiceSoapClient();
                status = client.PyStatus("S");
            }
            catch (Exception ex)
            {
                rc = "Fel vid kommunikation med Pyramid API. Felmeddelande : " + ex.Message;
            }
            return rc;
        }

    }
}









