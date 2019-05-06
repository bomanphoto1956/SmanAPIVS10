using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NexusDB.ADOProvider;
using System.Data;

namespace SManApi.ExportToPyramid
{
    public class CResetExport
    {
        CDB cdb = null;
        CLog log = null;
        string vart_ordernr = "";
        CExportToPyramid pyrExp = null;
        public CResetExport()
        {
            cdb = new CDB();
            log = new CLog();
            pyrExp = new CExportToPyramid();
        }


        /// <summary>
        /// Reset export all Pyramid export 
        /// settings for an order
        /// </summary>
        /// <param name="aVart_ordernr"></param>
        /// <param name="ident"></param>
        /// <returns>ErrorCL</returns>
        /// 2018-11-02 KJBO
        public ErrorCL resetExport(string aVart_ordernr, string ident)
        {
            ErrorCL err = new ErrorCL();
            
            CReparator cr = new CReparator();
            int identOK = cr.checkIdent(ident);
            if (identOK == -1)
            {                
                err.ErrCode = -10;
                err.ErrMessage = "Ogiltigt login";
                return err;
            }
            ReparatorCL reparator = cr.getReparator(ident);
            vart_ordernr = aVart_ordernr;
            
            err = resendServHuv(ident);
            if (err.ErrCode != 0)            
                return err;            
                                       
            err.ErrMessage = resetOrderArt();
            if (err.ErrMessage != "")
            {
                err.ErrCode = -36102;
                return err;
            }
            

            err.ErrMessage = resetReservdel();
            if (err.ErrMessage != "")
            {
                err.ErrCode = -36103;
                return err;
            }
            

            err.ErrMessage = resetServHuvRepTid();
            if (err.ErrMessage != "")
            {
                err.ErrCode = -36104;
                return err;
            }

            

            err.ErrMessage = resetServRadRepTid();
            if (err.ErrMessage != "")
            {
                err.ErrCode = -36105;
                return err;
            }


            // 2018-12-03 KJBO
            err.ErrMessage = pyrExp.resetPyramidExport(vart_ordernr);
            if (err.ErrMessage != "")
            {
                err.ErrCode = -36106;
                return err;
            }


            pyrExp.addToPyramidChange(vart_ordernr, reparator.AnvID, 3);
            
            return err;
        }


        private string resetOrderArt()
        {
            string result = "";

            string sSql = " select orderArtId, coAntal - ciAntal coAntal "
                        + " from orderArt "
                        + " where vart_ordernr = :vart_ordernr ";
            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("vart_ordernr", vart_ordernr);
            DataTable dt = cdb.getData(sSql, ref result, pc);

            if (result != "")
                return result;

            string sSqlDelete = " delete from oaPyramid "
                            + " where orderArtId = :orderArtId ";
            NxParameterCollection pcDelete = new NxParameterCollection();
            pcDelete.Add("orderArtId", 1);

            foreach (DataRow dr in dt.Rows)
            {
                int orderArtId = Convert.ToInt32(dr["orderArtId"]);
                pcDelete["orderArtId"].Value = orderArtId;
                cdb.updateData(sSqlDelete, ref result, pcDelete);
                if (result != "")
                    return result;
            }

            // Return here. The last code is replaced with resendxOrderArt
            return result;
        }



        /// <summary>
        /// When recreating reservation for "products in stock" = orderArt
        /// then we need to send "some at the time" because of timouts in
        /// the communication with Pyramid
        /// This means that this function is repetedly called until
        /// the return value total_handled equals to total_count then
        /// all orderArt rows for one order is resent
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="aVart_ordernr"></param>
        /// <param name="noToResend"></param>
        /// <returns></returns>
        public resendOrderArtCL resendxOrderArt( string aVart_ordernr, int noToResend, string ident)
        {
            resendOrderArtCL resend = new resendOrderArtCL();
            resend.ErrCode = 0;
            resend.ErrMessage = "";
            resend.total_count = 0;
            resend.total_handled = 0;
            resend.vartOrdernr = aVart_ordernr;

            CReparator cr = new CReparator();
            int identOK = cr.checkIdent(ident);
            if (identOK == -1)
            {
                resend.ErrCode = -10;
                resend.ErrMessage = "Ogiltigt login";
                return resend;
            }

            vart_ordernr = aVart_ordernr;


            string sSql = " select orderArtId, coAntal - ciAntal coAntal "
                        + " from orderArt oa "
                        + " where vart_ordernr = :vart_ordernr "
                        + " and not exists(select 'x' "
                        + " from oaPyramid oap "
                        + " where oa.orderArtId = oap.orderArtId "
                        + " and coAntal <> 0) ";
            string error = "";
            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("vart_ordernr", vart_ordernr);
            DataTable dt = cdb.getData(sSql, ref error, pc);
            int numberoOfRowsToSend = dt.Rows.Count;
            if (error != "")
            {
                resend.ErrCode = -36310;
                resend.ErrMessage = "Error when retrieving orderArt to resend. Error message : " + error;
                return resend;
            }

            int countRows = 0;
            foreach (DataRow dr in dt.Rows)
            {
                int orderArtId = Convert.ToInt32(dr["orderArtId"]);
                int antal = Convert.ToInt32(dr["coAntal"]);
                error = pyrExp.createReservation(orderArtId, antal);

                // 2019-04-30
                if (error == "Article does not exist." && !CConfig.validateReservation)
                    error = "";


                if (error != "")
                {
                    resend.ErrCode = -36314;
                    resend.ErrMessage = "Error when creating reservation in Pyramid. Error message : " + error;
                    return resend;
                }
                countRows++;                
                if (countRows == noToResend)
                    break;
            }

            string sSqlCount = " select count(*) countRows "
                             + " from orderArt "
                             + " where vart_ordernr = :vart_ordernr ";
            error = "";
            DataTable dtCount = cdb.getData(sSqlCount, ref error, pc);
            if (error != "")
            {
                resend.ErrCode = -36312;
                resend.ErrMessage = "Error when counting total orderArt for order. Error message : " + error;
                return resend;
            }

            resend.total_count = Convert.ToInt32(dtCount.Rows[0]["countRows"]);
            resend.total_handled = resend.total_count - (numberoOfRowsToSend - countRows);
            return resend;
        }




        private string resetReservdel()
        {
            string result = "";
            string sSqlDelete = " delete from reservdelPyr where vart_ordernr = :vart_ordernr ";
            NxParameterCollection pDelete = new NxParameterCollection();
            pDelete.Add("vart_ordernr", vart_ordernr);

            cdb.updateData(sSqlDelete, ref result, pDelete);
            if (result != "")
                return result;            
            string sSqlSelect = " select r.vart_ordernr, r.radnr, r.reserv_nr, coalesce(r.antal,0) antal "
                                + " from reservdel r "
                                + " where r.vart_ordernr = :vart_ordernr ";
            NxParameterCollection pcSelect = new NxParameterCollection();
            pcSelect.Add("vart_ordernr", vart_ordernr);

            DataTable dt = cdb.getData(sSqlSelect, ref result, pcSelect);
            if (result != "")
                return result;

            
            CReservdel cres = new CReservdel();
            foreach (DataRow dr in dt.Rows)
            {
                if (Convert.ToInt32(dr["antal"]) == 0)
                    continue;
                string vart_ordernr = dr["vart_ordernr"].ToString();
                int radnr = Convert.ToInt32(dr["radnr"]);
                int reserv_nr = Convert.ToInt32(dr["reserv_nr"]);
                ReservdelCL res = new ReservdelCL();
                res.VartOrdernr = vart_ordernr;
                res.Radnr = radnr;
                res.ReservNr = reserv_nr;
                result = cres.AddReservdelPyr(res);
                if (result != "")
                    return result;

            }
            return result;
        }


        private string resetServHuvRepTid()
        {
            string result = "";
            string sSqlUpdate = " update servHuvRepTid "
                            + " set pyramidExport = null "
                            + " where vart_ordernr = :vart_ordernr "
                            + " and pyramidExport is not null ";
            NxParameterCollection pcUpdate = new NxParameterCollection();
            pcUpdate.Add("vart_ordernr", vart_ordernr);
            cdb.updateData(sSqlUpdate, ref result, pcUpdate);            
            return result;
        }


        private string resetServRadRepTid()
        {
            string result = "";

            string sSqlSelect = " select alternateKey ak "
                            + " from servicerad "
                            + " where vart_ordernr = :vart_ordernr ";
            NxParameterCollection pcSelect = new NxParameterCollection();
            pcSelect.Add("vart_ordernr", vart_ordernr);
            DataTable dt = cdb.getData(sSqlSelect, ref result, pcSelect);
            if (result != "")
                return result;

            string sSqlUpdate = "update servradreptid "
                            + " set pyramidExport = null "
                            + " where srAltKey = :srAltKey "
                            + " and pyramidExport is not null ";

            NxParameterCollection pcUpdate = new NxParameterCollection();
            pcUpdate.Add("srAltKey", "xx");

            foreach (DataRow dr in dt.Rows)
            {
                string ak = dr["ak"].ToString();
                pcUpdate["srAltKey"].Value = ak;
                cdb.updateData(sSqlUpdate, ref result, pcUpdate);
                if (result != "")
                    return result;
            }
            return result;
        }


        private ErrorCL resendServHuv(string ident)
        {
            ErrorCL err = new ErrorCL();
            err.ErrCode = 0;
            err.ErrMessage = "";

            ServHuvCL sh = new ServHuvCL();
            CMServHuv csh = new CMServHuv();
            sh.Vart_ordernr = vart_ordernr;
            sh = csh.getServiceHuvud(sh, ident);
            if (sh.ErrCode != 0)
            {
                err.ErrCode = sh.ErrCode;
                err.ErrMessage = sh.ErrMessage;
                return err;
            }
            err = pyrExp.exportOrder(sh);
            return err;
        }




    }
}