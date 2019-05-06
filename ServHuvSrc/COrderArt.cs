using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NexusDB.ADOProvider;
using System.Data;

namespace SManApi.ServHuvSrc
{
    public class COrderArt
    {
        CDB cdb = null;
        public COrderArt()
        {
            cdb = new CDB();
        }

        /// <summary>
        /// Returns a list of ordered arts for a given order
        /// </summary>
        /// <param name="orderArtId"></param>
        /// <param name="vartOrdernr"></param>
        /// <param name="ident"></param>
        /// <returns></returns>
        public List<OrderArtCL> getOrderArt(int orderArtId, string vartOrdernr, string ident)
        {
            return getOrderArt(orderArtId, vartOrdernr, ident, true);
        }

        /// <summary>
        /// Returns a list of ordered arts for a given order
        /// </summary>
        /// <param name="orderArtId"></param>
        /// <param name="vartOrdernr"></param>
        /// <param name="ident"></param>
        /// <returns></returns>
        public List<OrderArtCL> getOrderArt(int orderArtId, string vartOrdernr, string ident, bool getStock)
        {
            CReparator cr = new CReparator();
            List<OrderArtCL> orderArts = new List<OrderArtCL>();
            int identOK = cr.checkIdent(ident);
            if (identOK == -1)
            {
                OrderArtCL oa = new OrderArtCL();
                oa.ErrCode = -10;
                oa.ErrMessage = "Ogiltigt login";
                orderArts.Add(oa);
                return orderArts;
            }
            return getOrderArt(orderArtId, vartOrdernr, getStock);
        }


        /// <summary>
        /// Returns a list of ordered arts for a given order
        /// </summary>
        /// <param name="orderArtId"></param>
        /// <param name="vartOrdernr"></param>
        /// <param name="ident"></param>
        /// <returns></returns>
        private List<OrderArtCL> getOrderArt(int orderArtId, string vartOrdernr)
        {
            return getOrderArt(orderArtId, vartOrdernr, true);
        }

        // 2018-08-30 KJBO
        public List<OrderArtCL> getOrderArt(string vartOrdernr)
        {
            return getOrderArt(0, vartOrdernr, false);
        }



        /// <summary>
        /// Returns a list of ordered arts for a given order
        /// </summary>
        /// <param name="orderArtId"></param>
        /// <param name="vartOrdernr"></param>
        /// <param name="ident"></param>
        /// <returns></returns>
        private List<OrderArtCL> getOrderArt(int orderArtId, string vartOrdernr, bool getStock)
        {
            List<OrderArtCL> orderArts = new List<OrderArtCL>();
            if (orderArtId == 0 && vartOrdernr == "")
            {
                OrderArtCL oa = new OrderArtCL();
                oa.ErrCode = -101;
                oa.ErrMessage = "Felaktigt argument";
                orderArts.Add(oa);
                return orderArts;

            }



            string sSql = "SELECT oa.orderArtId, oa.vart_ordernr, oa.artnr, oa.coAntal, oa.ciAntal, sum(coalesce(r.antal,0)) ordAntal, a.artnamn, a.artikelkod, coalesce(oa.tempCiAntal,0) tempCiAntal "
                        + " FROM orderArt oa "
                        + " join artikel a on oa.artnr = a.artnr "
                       + " left outer join reservdel r on (oa.vart_ordernr = r.vart_ordernr and oa.artnr = r.artnr) ";
            if (orderArtId > 0)
                sSql += " where oa.orderArtId = :orderArtId ";
            else
                sSql += " where oa.vart_ordernr = :vart_ordernr ";
            sSql += " group by oa.orderArtId, oa.vart_ordernr, oa.artnr, oa.coAntal, oa.ciAntal, a.artnamn, a.artikelkod, oa.tempCiAntal ";
            NxParameterCollection np = new NxParameterCollection();
            if (orderArtId > 0)
                np.Add("orderArtId", orderArtId);
            else
                np.Add("vart_ordernr", vartOrdernr);
            string errText = "";
            DataTable dt = cdb.getData(sSql, ref errText, np);

            if (errText != "")
            {
                OrderArtCL oa = new OrderArtCL();
                if (errText.Length > 2000)
                    errText = errText.Substring(1, 2000);
                oa.ErrCode = -100;
                oa.ErrMessage = errText;
                orderArts.Add(oa);
                return orderArts;
            }
            // 2018-05-22 KJBO
            PyramidServ.ServiceSoapClient client = new PyramidServ.ServiceSoapClient();
            foreach (DataRow dr in dt.Rows)
            {
                OrderArtCL oa = new OrderArtCL();
                oa.OrderArtId = Convert.ToInt32(dr["orderArtId"]);
                oa.VartOrdernr = dr["vart_ordernr"].ToString();
                oa.Artnr = dr["artnr"].ToString();
                oa.ArtNamn = dr["artnamn"].ToString();
                oa.Artikelkod = dr["artikelkod"].ToString();
                oa.CoAntal = Convert.ToDecimal(dr["coAntal"]);
                oa.CiAntal = Convert.ToDecimal(dr["ciAntal"]);
                oa.OrdAntal = Convert.ToDecimal(dr["ordAntal"]);
                // 2018-05-02 KJBO
                oa.TempCiAntal = Convert.ToDecimal(dr["tempCiAntal"]);
                oa.Stock = 0;
                if (getStock)
                    oa.Stock = getPyramidArtStock(oa.Artikelkod, client);
                orderArts.Add(oa);
            }
            client.Close();
            return orderArts;

        }

        private Decimal getPyramidArtStock(string artikelkod, PyramidServ.ServiceSoapClient client)
        {
            Decimal result = 0;
            try
            {
                string stock = client.ArtGetStock(ref artikelkod).Trim();
                stock = stock.Replace(".", ",");
                if (stock != "")
                {
                    Decimal.TryParse(stock, out result);
                }

            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }
            return result;
        }

        /// <summary>
        /// Get all articles that are handled by
        /// pyramid as stock articles
        /// </summary>
        /// <param name="ident"></param>
        /// <returns></returns>
        public List<OrderArtCL> getCheckoutableArticles(string ident, string artnr)
        {
            CReparator cr = new CReparator();
            List<OrderArtCL> orderArts = new List<OrderArtCL>();
            int identOK = cr.checkIdent(ident);
            if (identOK == -1)
            {
                OrderArtCL oa = new OrderArtCL();
                oa.ErrCode = -10;
                oa.ErrMessage = "Ogiltigt login";
                orderArts.Add(oa);
                return orderArts;
            }
            return getCheckoutableArticles(artnr, 0);
        }



        /// <summary>
        /// Gets one article defined by artnr
        /// Also returns stock for this article
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="artnr"></param>
        /// <returns></returns>
        public OrderArtCL getCheckoutableArticle(string ident, string artnr)
        {
            CReparator cr = new CReparator();
            List<OrderArtCL> orderArts = new List<OrderArtCL>();
            int identOK = cr.checkIdent(ident);
            if (identOK == -1)
            {
                OrderArtCL oa = new OrderArtCL();
                oa.ErrCode = -10;
                oa.ErrMessage = "Ogiltigt login";
                return oa;
            }
            orderArts = getCheckoutableArticles(artnr, 1);
            return orderArts[0];
        }

        /// <summary>
        /// Get all articles that are handled by
        /// pyramid as stock articles
        /// </summary>
        /// <param name="ident"></param>
        /// <returns></returns>
        private List<OrderArtCL> getCheckoutableArticles(string artnr, int getStock)
        {

            List<OrderArtCL> orderArts = new List<OrderArtCL>();
            string sSql = "SELECT a.artnr,  a.artnamn, a.artikelkod "
                        + " from artikel a "
                        + " where a.artikelkod is not null "
                        + " and a.kategori = 1 ";
            if (artnr != "")
                sSql += " and a.artnr = :artnr ";
            // sSql += " order by a.artnamn ";

            NxParameterCollection pc = new NxParameterCollection();
            if (artnr != "")
                pc.Add("artnr", artnr);
            string errText = "";
            DataTable dt = cdb.getData(sSql, ref errText, pc);

            if (errText != "")
            {
                OrderArtCL oa = new OrderArtCL();
                if (errText.Length > 2000)
                    errText = errText.Substring(1, 2000);
                oa.ErrCode = -100;
                oa.ErrMessage = errText;
                orderArts.Add(oa);
                return orderArts;
            }
            // 2018-05-22 KJBO
            PyramidServ.ServiceSoapClient client = new PyramidServ.ServiceSoapClient();
            foreach (DataRow dr in dt.Rows)
            {
                OrderArtCL oa = new OrderArtCL();
                oa.Artnr = dr["artnr"].ToString();
                oa.ArtNamn = dr["artnamn"].ToString();
                oa.Artikelkod = dr["artikelkod"].ToString();
                // 2018-05-22 KJBO
                if (getStock == 1)
                    oa.Stock = getPyramidArtStock(dr["artikelkod"].ToString(), client);
                orderArts.Add(oa);
            }
            client.Close();
            return orderArts;

        }

        private string getOaInsertSql()
        {
            return "insert into orderArt (vart_ordernr, artnr, coAntal, reg, regdat) "
                + " values( :vart_ordernr, :artnr, :coAntal, :reg, :regdat) ";
        }

        private string getOaUpdateSql()
        {
            return " update orderArt "
                + " set coAntal = :coAntal "
                + " , uppdaterat = :uppdaterat "
                + " , uppdat_dat = :uppdat_dat "
                + "  where orderArtId = :orderArtId ";
        }

        private void setOaParameters(NxParameterCollection pc, OrderArtCL oa, Boolean forInsert, String AnvId)
        {
            pc.Add("coAntal", oa.CoAntal);
            if (forInsert)
            {
                pc.Add("vart_ordernr", oa.VartOrdernr);
                pc.Add("artnr", oa.Artnr);
                pc.Add("reg", AnvId);
                pc.Add("regdat", System.DateTime.Now);
            }
            else
            {
                pc.Add("orderArtId", oa.OrderArtId);
                pc.Add("uppdaterat", AnvId);
                pc.Add("uppdat_dat", System.DateTime.Now);
            }
        }

        private int validateOa(OrderArtCL oa, ref string errTxt)
        {
            List<OrderArtCL> oaList = getCheckoutableArticles(oa.Artnr, 0);
            if (oaList.Count == 0)
                return -1;
            int liRc = servHuvExistsAndOpen(oa.VartOrdernr, ref errTxt);
            if (liRc == -1)
                return -2;
            if (liRc == -2)
                return -3;
            return 1;
        }

        private int servHuvExistsAndOpen(string vartOrdernr, ref string errText)
        {
            string sSql = "select vart_ordernr, godkand "
                        + " from servicehuvud "
                        + "where vart_ordernr = :vart_ordernr ";
            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("vart_ordernr", vartOrdernr);
            errText = "";
            DataTable dt = cdb.getData(sSql, ref errText, pc);
            if (dt.Rows.Count == 0)
                return -1;
            if (Convert.ToBoolean(dt.Rows[0]["godkand"]) == true)
                return -2;
            return 1;

        }


        private string getArtnrForOrderArt(int orderArtId)
        {
            string sSql = " select artnr "
                        + " from orderArt "
                        + " where orderArtId = " + orderArtId.ToString() + " ";
            string dummy = "";
            DataTable dt = cdb.getData(sSql, ref dummy);
            return dt.Rows[0][0].ToString();

        }

        public OrderArtCL checkoutOrderArt(string ident, OrderArtCL oa)
        {
            CReparator cr = new CReparator();
            int identOK = cr.checkIdent(ident);
            if (identOK == -1)
            {
                OrderArtCL oaRet = new OrderArtCL();
                oaRet.ErrCode = -10;
                oaRet.ErrMessage = "Ogiltigt login";
                return oaRet;
            }
            return checkoutOrderArt(oa, ident, false);
        }




        public OrderArtCL checkoutOrderArt(OrderArtCL oa, string id, bool acceptGodkand)
        {
            ExportToPyramid.CExportToPyramid pyExport = new ExportToPyramid.CExportToPyramid();
            string exportErr = pyExport.checkPyramidAPIAvailable();
            if (exportErr != "")
            {
                OrderArtCL oacl = new OrderArtCL();
                oacl.ErrCode = -18250;
                oacl.ErrMessage = exportErr;
                return oacl;
            }

            ReparatorCL rep = null;
            if (id == "")
            {
                rep = new ReparatorCL();
                rep.AnvID = "Pyramid";
            }
            else
            {
                CReparator cr = new CReparator();
                rep = cr.getReparator(id);
            }
            // 2018-04-23 KJBO
            // Special handling due to gui limitations
            //if (oa.OrderArtId != 0 && oa.Artnr == null)
            //{
            //    oa.Artnr = getArtnrForOrderArt(oa.OrderArtId);
            //}


            if (oa.VartOrdernr == "")
            {
                OrderArtCL oaRet = new OrderArtCL();
                oaRet.ErrCode = -103;
                oaRet.ErrMessage = "Ordernummer måste anges";
                return oaRet;
            }

            if (oa.Artnr == "")
            {
                OrderArtCL oaRet = new OrderArtCL();
                oaRet.ErrCode = -104;
                oaRet.ErrMessage = "Artikelnummer måste anges";
                return oaRet;
            }


            string errTxt = "";
            int rc = validateOa(oa, ref errTxt);
            if (rc == -1)
            {
                OrderArtCL oaRet = new OrderArtCL();
                oaRet.ErrCode = -106;
                oaRet.ErrMessage = "Felaktigt ordernr";
                return oaRet;

            }

            if (rc == -2)
            {
                OrderArtCL oaRet = new OrderArtCL();
                oaRet.ErrCode = -107;
                if (errTxt == "")
                    oaRet.ErrMessage = "Felaktigt ordernr";
                else
                    oaRet.ErrMessage = errTxt;
                return oaRet;

            }

            if (!acceptGodkand)
            {
                if (rc == -3)
                {
                    OrderArtCL oaRet = new OrderArtCL();
                    oaRet.ErrCode = -108;
                    if (errTxt == "")
                        oaRet.ErrMessage = "Ordern är godkänd. Ändringar ej tillåtna";
                    else
                        oaRet.ErrMessage = errTxt;
                    return oaRet;

                }
            }

            string sSql = "";
            // 2018-04-30 KJBO
            if (oa.OrderArtId == 0)
            {
                decimal toBeAdded = 0;
                oa.OrderArtId = reuseOrderArtId(oa, ref toBeAdded);
                if (oa.OrderArtId != 0)
                    oa.CoAntal += toBeAdded;
            }
            if (oa.OrderArtId == 0)
                sSql = getOaInsertSql();
            else
                sSql = getOaUpdateSql();
            NxParameterCollection pc = new NxParameterCollection();
            setOaParameters(pc, oa, oa.OrderArtId == 0, rep.AnvID);

            int li_rc = cdb.updateData(sSql, ref errTxt, pc);

            if (errTxt != "")
            {
                OrderArtCL oaRet = new OrderArtCL();
                if (errTxt.Length > 2000)
                    errTxt = errTxt.Substring(1, 2000);
                oaRet.ErrCode = -100;
                oaRet.ErrMessage = errTxt;
                return oaRet;
            }

            if (oa.OrderArtId == 0)
            {
                sSql = "SELECT MAX(orderArtId) orderArtId FROM orderArt ";
                DataTable dt = cdb.getData(sSql, ref errTxt);


                if (errTxt != "")
                {
                    OrderArtCL oaRet = new OrderArtCL();
                    if (errTxt.Length > 2000)
                        errTxt = errTxt.Substring(1, 2000);
                    oaRet.ErrCode = -100;
                    oaRet.ErrMessage = errTxt;
                    return oaRet;

                }
                oa.OrderArtId = Convert.ToInt32(dt.Rows[0][0]);
            }



            List<OrderArtCL> oaList = getOrderArt(oa.OrderArtId, "");

            if (oaList.Count != 1)
            {
                OrderArtCL oaRet = new OrderArtCL();
                if (errTxt.Length > 2000)
                    errTxt = errTxt.Substring(1, 2000);
                oaRet.ErrCode = -110;
                oaRet.ErrMessage = "Error when retrieving orderArt";
                return oaRet;
            }
            
            ErrorCL errCl = pyExport.reserveArticle(oaList[0].OrderArtId);
            //pyExport.retryAllOrders();
            return oaList[0];
        }

        private int reuseOrderArtId(OrderArtCL oa, ref decimal coAntal)
        {
            String sSql = " SELECT orderArtId, coAntal "
                        + " FROM orderArt "
                        + " where vart_ordernr = :vart_ordernr "
                        + " and artnr = :artnr ";
            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("vart_ordernr", oa.VartOrdernr);
            pc.Add("artnr", oa.Artnr);
            string error = "";
            DataTable dt = cdb.getData(sSql, ref error, pc);
            if (dt.Rows.Count == 0)
                return 0;
            DataRow dr = dt.Rows[0];
            coAntal = Convert.ToDecimal(dr["coAntal"]);
            return Convert.ToInt32(dr["orderArtId"]);
        }


        /// <summary>
        /// Check if there are any orderArt rows
        /// Use to determine if checkin shall be available
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="vartOrdernr"></param>
        /// <returns>Number of orderArt articles for the current ordernr or -1 if error occurs</returns>
        /// 
        public int countOrderArtRows(string ident, string vartOrdernr)
        {

            CReparator cr = new CReparator();
            int identOK = cr.checkIdent(ident);
            if (identOK == -1)
            {
                return -1;
            }

            string sSql = " select count(*) count_rows "
                        + " from orderArt "
                        + " where vart_ordernr = :vart_ordernr ";
            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("vart_ordernr", vartOrdernr);
            string dummy = "";
            DataTable dt = cdb.getData(sSql, ref dummy, pc);
            return Convert.ToInt32(dt.Rows[0]["count_rows"]);
        }



        /// <summary>
        /// Calculate how many artikel that shall remain
        /// on a service order and suggest the checkin
        /// value by calling setTemCiAntal
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="vartOrdernr"></param>
        /// <returns>Error class</returns>
        /// 2018-05-02 KJBO Indentive AB
        public ErrorCL calculateCiOrderArt(string ident, string vartOrdernr)
        {
            ErrorCL errCl = new ErrorCL();
            string sSql = " SELECT artnr, coAntal - ciAntal netto "
                        + " FROM orderArt "
                        + " where vart_ordernr = :vart_ordernr ";
            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("vart_ordernr", vartOrdernr);

            string sSqlReservdel = " select coalesce(sum(antal),0) sum_antal "
                                + " from reservdel "
                                + " where vart_ordernr = :vart_ordernr "
                                + " and artnr = :artnr ";
            NxParameterCollection pcReserv = new NxParameterCollection();
            NxCommand cmReserv = new NxCommand(sSqlReservdel, cdb.getConn());
            NxDataAdapter daReserv = new NxDataAdapter(cmReserv);
            cmReserv.Parameters.Add("vart_ordernr", vartOrdernr);
            cmReserv.Parameters.Add("artnr", "");
            DataTable dtReserv = new DataTable();


            string errText = "";
            DataTable dt = cdb.getData(sSql, ref errText, pc);
            if (errText != "")
            {
                errText = "Error when retrieving data from orderArt table. Message : " + errText;
                if (errText.Length > 2000)
                    errText = errText.Substring(1, 2000);
                errCl.ErrCode = -100;
                errCl.ErrMessage = errText;
                return errCl;
            }

            foreach (DataRow dr in dt.Rows)
            {
                string artnr = dr["artnr"].ToString();
                Decimal ciNetto = Convert.ToDecimal(dr["netto"]);
                Decimal countReserv = 0;
                try
                {
                    cmReserv.Parameters["artnr"].Value = artnr;
                    dtReserv.Rows.Clear();
                    daReserv.Fill(dtReserv);
                    if (dtReserv.Rows.Count > 0)
                    {
                        countReserv = Convert.ToDecimal(dtReserv.Rows[0]["sum_antal"]);
                    }
                }
                catch (Exception ex)
                {

                    errCl.ErrCode = -100;
                    errText = "Error when retrieving data from reservdel table. Message : " + ex.Message;
                    if (errText.Length > 2000)
                        errText = errText.Substring(1, 2000);
                    errCl.ErrMessage = errText;
                    return errCl;
                }
                ciNetto -= countReserv;
                ciNetto = Math.Round(ciNetto * 100) / 100;
                errCl = setTempCiAntal(0, vartOrdernr, artnr, ciNetto);
                if (errCl.ErrCode != 0)
                    return errCl;



            }
            errCl.ErrCode = 0;
            errCl.ErrMessage = "";
            return errCl;
        }


        /// <summary>
        /// Set a new value to column tempCiAntal on table orderArt
        /// This is a temporary value (to be committed later) for
        /// how many items that will be inserted into CompactStore
        /// after a serviceorder session
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="orderArtId"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        /// 2018-05-02 KJBO Indentive AB
        public ErrorCL setTempCiAntal(string ident, int orderArtId, decimal newValue)
        {
            CReparator cr = new CReparator();
            int identOK = cr.checkIdent(ident);
            if (identOK == -1)
            {
                ErrorCL err = new ErrorCL();
                err.ErrCode = -10;
                err.ErrMessage = "Ogiltigt login";
                return err;
            }
            return setTempCiAntal(orderArtId, "", "", newValue);
        }

        private ErrorCL setTempCiAntal(int orderArtId, string vartOrdernr, string artnr, decimal tempCiAntal)
        {
            ErrorCL errCl = new ErrorCL();
            errCl.ErrCode = 0;
            errCl.ErrMessage = "";
            string sSql = " update orderArt "
                        + " set tempCiAntal = :tempCiAntal ";
            if (orderArtId != 0)
                sSql += " where orderArtId = :orderArtId ";
            else
                sSql += " where vart_ordernr = :vart_ordernr "
                        + " and artnr = :artnr ";

            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("tempCiAntal", tempCiAntal);
            if (orderArtId != 0)
                pc.Add("orderArtId", orderArtId);
            else
            {
                pc.Add("vart_ordernr", vartOrdernr);
                pc.Add("artnr", artnr);
            }
            string errText = "";
            cdb.updateData(sSql, ref errText, pc);
            if (errText != "")
            {
                errCl.ErrCode = -100;
                errText = "Error when updating tempCiAntal on orderArt table. Message : " + errText;
                if (errText.Length > 2000)
                    errText = errText.Substring(1, 2000);
                errCl.ErrMessage = errText;
                return errCl;
            }
            return errCl;
        }



        /// <summary>
        /// Commits the temporary temCiAntal to
        /// the real ciAntal in the orderArt table
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="vartOrdernr"></param>
        /// <returns></returns>
        /// 2018-05-02 KJBO Indentive AB
        public ErrorCL commitTempCiAntal(string ident, string vartOrdernr)
        {
            ErrorCL err = new ErrorCL();
            err.ErrCode = 0;
            err.ErrMessage = "";

            CReparator cr = new CReparator();
            int identOK = cr.checkIdent(ident);
            if (identOK == -1)
            {
                err.ErrCode = -10;
                err.ErrMessage = "Ogiltigt login";
                return err;
            }


            string sSql = " update orderArt "
                        + " set ciAntal = ciAntal + coalesce(tempCiAntal, 0) "
                        + " where vart_ordernr = :vart_ordernr ";
            string error = "";
            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("vart_ordernr", vartOrdernr);
            cdb.updateData(sSql, ref error, pc);
            if (error != "")
            {
                err.ErrCode = -100;
                if (error.Length > 2000)
                    error = error.Substring(1, 2000);
                err.ErrMessage = error;
                return err;
            }
            return err;
        }

        public decimal countOutcheckedArt(string vartOrdernr, string artnr)
        {
            string sSql = " select coalesce(sum(coAntal),0) - coalesce(sum(ciAntal),0) sum_outchecked "
                        + " from orderart oa "
                        + " where vart_ordernr = :vart_ordernr "
                        + " and artnr = :artnr ";
            NxParameterCollection pc = new NxParameterCollection();
            decimal countOutchecked = 0;
            pc.Add("vart_ordernr", vartOrdernr);
            pc.Add("artnr", artnr);
            string err = "";
            DataTable dt = cdb.getData(sSql, ref err, pc);
            if (dt.Rows.Count == 1)
                countOutchecked = Convert.ToDecimal(dt.Rows[0]["sum_outchecked"]);
            return countOutchecked;

        }


        /// <summary>
        /// Returns a list of available levels for the outchecked articles list
        /// </summary>
        /// <param name="ident"></param>
        /// 2018-05-28
        /// <returns></returns>
        public List<KeyValuePair<int, string>> getUcArtListSelection(string ident)
        {
            List<KeyValuePair<int, string>> list = new List<KeyValuePair<int, string>>();

            CReparator cr = new CReparator();
            int identOK = cr.checkIdent(ident);
            if (identOK == -1)
            {
                list.Add(new KeyValuePair<int, string>(-10, "Ogiltigt login"));
                return list;
            }
            list.Add(new KeyValuePair<int, string>(1, "Artikel"));
            list.Add(new KeyValuePair<int, string>(2, "Ansvarig"));
            list.Add(new KeyValuePair<int, string>(3, "Order"));
            return list;
        }


        /// <summary>
        /// Returns a list of all outchecked objects
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="listType"></param>
        /// <returns></returns>
        /// 2018-05-28 KJBO Indentive AB
        public List<OrderArtListCL> getOutcheckedList(string ident, int listType)
        {
            List<OrderArtListCL> oaList = new List<OrderArtListCL>();

            CReparator cr = new CReparator();
            int identOK = cr.checkIdent(ident);
            if (identOK == -1)
            {
                OrderArtListCL oa = new OrderArtListCL();
                oa.ErrCode = -10;
                oa.ErrMessage = "Ogiltigt login";
                oaList.Add(oa);
                return oaList;
            }

            DateTime startDate = CConfig.startDateForCoList;


                                              
            string sSql = " select oa.artnr, a.artnamn, rep.reparator, oa.vart_ordernr, sum(oa.coAntal) - sum(oa.ciAntal) antalCo, coalesce(sum(r.antal),0) antalOrder "
                    + " from orderart oa "
                    + " join artikel a on oa.artnr = a.artnr "
                    + " join servicehuvud sh on oa.vart_ordernr = sh.vart_ordernr "
                    + " join reparator rep on sh.OrderAdmin = rep.AnvId "
                    + " left outer join "
                    + " (select r2.artnr, r2.vart_ordernr, sum(r2.antal) antal "
                    + " from reservdel r2 "
                    + " group by r2.artnr, r2.vart_ordernr) r on(oa.artnr = r.artnr and oa.vart_ordernr = r.vart_ordernr) "
                    + " where sh.sentToPyramid = true "
                    + " and sh.datum >= :startDate "
                    + " group by oa.artnr, a.artnamn, rep.reparator, oa.vart_ordernr "
                    + " having sum(oa.coAntal) - sum(oa.ciAntal) - coalesce(sum(r.antal), 0) > 0 ";



            string err = "";
            // 2018-06-18 KJBO
            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("startDate", startDate);
            DataTable dt = cdb.getData(sSql, ref err, pc);

            if (err != "")
            {
                OrderArtListCL oa = new OrderArtListCL();
                oa.ErrCode = -100;
                if (err.Length > 2000)
                    err = err.Substring(1, 2000);
                oa.ErrMessage = err;
                oaList.Add(oa);
                return oaList;
            }

            foreach (DataRow dr in dt.Rows)
            {
                OrderArtListCL oa = new OrderArtListCL();
                oa.antalCo = Convert.ToInt32(dr["antalCo"]);
                oa.antalOrder = Convert.ToInt32(dr["antalOrder"]);
                oa.artnamn = dr["artnamn"].ToString();
                oa.artnr = dr["artnr"].ToString();
                oa.ErrCode = 0;
                oa.ErrMessage = "";
                oa.ansvarig = "";
                if (listType >= 2)                
                    oa.ansvarig = dr["reparator"].ToString();
                if (listType == 3)
                    oa.vartOrdernr = dr["vart_ordernr"].ToString();                
                oaList.Add(oa);
            }

            if (listType == 1)
            {
                List<OrderArtListCL> oaList2 = new List<OrderArtListCL>();
                foreach (OrderArtListCL oa in oaList)
                {
                    OrderArtListCL oa2 = oaList2.Find(x => x.artnr == oa.artnr);
                    if (oa2 == null)
                    {
                        oa2 = new OrderArtListCL();
                        oa2.artnr = oa.artnr;                        
                        oa2.antalCo = oa.antalCo;
                        oa2.antalOrder = oa.antalOrder;
                        oa2.artnamn = oa.artnamn;                        
                        oaList2.Add(oa2);
                    }
                    else
                    {
                        oa2.antalCo += oa.antalCo;
                        oa2.antalOrder += oa.antalOrder;
                    }
                }
                return oaList2;

            }


            if (listType == 2)
            {
                List<OrderArtListCL> oaList2 = new List<OrderArtListCL>();
                foreach (OrderArtListCL oa in oaList)
                {
                    OrderArtListCL oa2 = oaList2.Find(x => x.artnr == oa.artnr && x.ansvarig == oa.ansvarig);
                    if (oa2 == null)
                    {
                        oa2 = new OrderArtListCL();
                        oa2.artnr = oa.artnr;
                        oa2.ansvarig = oa.ansvarig;
                        oa2.antalCo = oa.antalCo;
                        oa2.antalOrder = oa.antalOrder;
                        oa2.artnamn = oa.artnamn;
                        oa2.artnr = oa.artnr;
                        oaList2.Add(oa2);
                    }
                    else
                    {
                        oa2.antalCo += oa.antalCo;
                        oa2.antalOrder += oa.antalOrder;
                    }
                }
                return oaList2;

            }


            return oaList;
        }

        /// <summary>
        /// Returns a list of all outchecked objects
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="listType"></param>
        /// <returns></returns>
        /// 2018-05-28 KJBO Indentive AB
        public List<OrderArtListCL> getOutcheckedListOldNotUsed(string ident, int listType)
        {
            List<OrderArtListCL> oaList = new List<OrderArtListCL>();

            CReparator cr = new CReparator();
            int identOK = cr.checkIdent(ident);
            if (identOK == -1)
            {
                OrderArtListCL oa = new OrderArtListCL();
                oa.ErrCode = -10;
                oa.ErrMessage = "Ogiltigt login";
                oaList.Add(oa);
                return oaList;
            }


            string sSql = "";
            string sSqlOtherOrders = "";
            if (listType == 1)
            {
                sSql = " select oa.artnr, a.artnamn, sum(oa.coAntal) - sum(oa.ciAntal) antalCo, coalesce(sum(r.antal),0) antalOrder "
                    + " from orderart oa "
                    + " join artikel a on oa.artnr = a.artnr "
                    + " left outer join "
                    + " (select artnr, sum(antal) antal "
                    + " from reservdel r "
                    + " join servicehuvud s on r.vart_ordernr = s.vart_ordernr "
                    + " where s.sentToPyramid = true "
                    + " group by artnr) r on(r.artnr = oa.artnr) "
                    + " group by oa.artnr, a.artnamn "
                    + " having sum(oa.coAntal) - sum(oa.ciAntal) - coalesce(sum(r.antal), 0) > 0 ";
            }

            /*
            if (listType == 2)
            {
                sSql = " select oa.artnr, a.artnamn, rep.reparator, sum(oa.coAntal), sum(oa.ciAntal), coalesce(sum(r.antal),0) orderAntal, sum(oa.coAntal) - sum(oa.ciAntal) - coalesce(sum(r.antal),0) netto "
                    + " from orderart oa "
                    + " join artikel a on oa.artnr = a.artnr "
                    + " join servicehuvud sh on oa.vart_ordernr = sh.vart_ordernr "
                    + " join reparator rep on sh.OrderAdmin = rep.AnvId "
                    + " left outer join "
                    + " (select r2.artnr, rep2.AnvId, sum(r2.antal) antal "
                    + " from reservdel r2 "
                    + " join servicehuvud sh2 on r2.vart_ordernr = sh2.vart_ordernr "
                    + " join reparator rep2 on sh2.orderAdmin = rep2.AnvId "
                    + " group by r2.artnr, rep2.AnvId) r on (oa.artnr = r.artnr and rep.AnvId = r.AnvId) "
                    + " group by oa.artnr, a.artnamn, rep.reparator "
                    + " having sum(oa.coAntal) - sum(oa.ciAntal) - coalesce(sum(r.antal), 0) > 0 ";
            }
            */

            if (listType >= 2)
                sSql = " select oa.artnr, a.artnamn, rep.reparator, oa.vart_ordernr, sum(oa.coAntal) - sum(oa.ciAntal) antalCo, coalesce(sum(r.antal),0) antalOrder "
                    + " from orderart oa "
                    + " join artikel a on oa.artnr = a.artnr "
                    + " join servicehuvud sh on oa.vart_ordernr = sh.vart_ordernr "
                    + " join reparator rep on sh.OrderAdmin = rep.AnvId "
                    + " left outer join "
                    + " (select r2.artnr, r2.vart_ordernr, sum(r2.antal) antal "
                    + " from reservdel r2 "
                    + " group by r2.artnr, r2.vart_ordernr) r on(oa.artnr = r.artnr and oa.vart_ordernr = r.vart_ordernr) "
                    + " group by oa.artnr, a.artnamn, rep.reparator, oa.vart_ordernr "
                    + " having sum(oa.coAntal) - sum(oa.ciAntal) - coalesce(sum(r.antal), 0) > 0 ";





            string err = "";
            DataTable dt = cdb.getData(sSql, ref err);

            if (err != "")
            {
                OrderArtListCL oa = new OrderArtListCL();
                oa.ErrCode = -100;
                if (err.Length > 2000)
                    err = err.Substring(1, 2000);
                oa.ErrMessage = err;
                oaList.Add(oa);
                return oaList;
            }

            foreach (DataRow dr in dt.Rows)
            {
                OrderArtListCL oa = new OrderArtListCL();
                oa.antalCo = Convert.ToInt32(dr["antalCo"]);
                oa.antalOrder = Convert.ToInt32(dr["antalOrder"]);
                oa.artnamn = dr["artnamn"].ToString();
                oa.artnr = dr["artnr"].ToString();
                oa.ErrCode = 0;
                oa.ErrMessage = "";
                oa.ansvarig = "";
                if (listType >= 2)
                {
                    oa.ansvarig = dr["reparator"].ToString();
                    oa.vartOrdernr = dr["vart_ordernr"].ToString();
                }
                oaList.Add(oa);
            }

            if (listType >= 2)
            {
                string orderString = "";
                string artString = "";
                foreach (DataRow dr in dt.Rows)
                {
                    if (orderString.Length > 0)
                        orderString += ", ";
                    orderString += "'" + dr["vart_ordernr"].ToString() + "' ";
                    if (artString.Length > 0)
                        artString += ", ";
                    artString += "'" + dr["artnr"].ToString() + "' ";
                }

                if (orderString.Length > 0 && artString.Length > 0)
                {
                    sSqlOtherOrders = " select r.vart_ordernr, s.orderAdmin, rep.reparator, r.artnr, a.artnamn,sum(r.antal) antalOrder "
                                        + " from serviceHuvud s "
                                        + " join reservdel r on s.vart_ordernr = r.vart_ordernr "
                                        + " join reparator rep on s.orderAdmin = rep.AnvId "
                                        + " join artikel a on r.artnr = a.artnr "
                                        + " where s.sentToPyramid = true ";
                    sSqlOtherOrders += " and r.vart_ordernr not in ( " + orderString + ") ";
                    sSqlOtherOrders += " and r.artnr in ( " + artString + ") ";
                    sSqlOtherOrders += " group by r.artnr, a.artnamn, s.orderAdmin, rep.reparator, r.vart_ordernr ";

                    DataTable dtOthers = cdb.getData(sSqlOtherOrders, ref err);
                    if (err != "")
                    {
                        OrderArtListCL oa = new OrderArtListCL();
                        oaList.Clear();
                        oa.ErrCode = -100;
                        err = "Error when retrieving data for other orders. Error message : " + err;
                        if (err.Length > 2000)
                            err = err.Substring(1, 2000);
                        oa.ErrMessage = err;
                        oaList.Add(oa);
                        return oaList;
                    }

                    foreach (DataRow dr in dtOthers.Rows)
                    {
                        OrderArtListCL oa = new OrderArtListCL();
                        oa.ansvarig = dr["reparator"].ToString();
                        oa.antalCo = 0;
                        oa.artnamn = dr["artnamn"].ToString();
                        oa.antalOrder = Convert.ToInt32(dr["antalOrder"]);
                        oa.artnr = dr["artnr"].ToString();
                        oa.vartOrdernr = dr["vart_ordernr"].ToString();
                        oaList.Add(oa);
                    }


                }

                if (listType == 2)
                {
                    List<OrderArtListCL> oaList2 = new List<OrderArtListCL>();
                    foreach (OrderArtListCL oa in oaList)
                    {
                        OrderArtListCL oa2 = oaList2.Find(x => x.artnr == oa.artnr && x.ansvarig == oa.ansvarig);
                        if (oa2 == null)
                        {
                            oa2 = new OrderArtListCL();
                            oa2.artnr = oa.artnr;
                            oa2.ansvarig = oa.ansvarig;
                            oa2.antalCo = oa.antalCo;
                            oa2.antalOrder = oa.antalOrder;
                            oa2.artnamn = oa.artnamn;
                            oa2.artnr = oa.artnr;
                            oaList2.Add(oa2);
                        }
                        else
                        {
                            oa2.antalCo += oa.antalCo;
                            oa2.antalOrder += oa.antalOrder;
                        }
                    }
                    return oaList2;
                }


            }

            return oaList;
        }


        // 2018-08-30 KJBO
        public ErrorCL updateCiAntal(int orderArtId, Decimal antal)
        {
            ErrorCL err = new ErrorCL();
            err.ErrCode = 0;
            err.ErrMessage = "";

            string sSql = " update orderArt "
                        + " set ciAntal = :ciAntal "
                        + " where orderArtId = :orderArtId ";
            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("ciAntal", antal);
            pc.Add("orderArtId", orderArtId);
            string errText = "";
            cdb.updateData(sSql, ref errText, pc);

            if (errText != "")
            {
                if (errText.Length > 2000)
                    errText = errText.Substring(1, 2000);
                err.ErrCode = -100;
                err.ErrMessage = errText;                                
            }
            return err;
        }


    }
}