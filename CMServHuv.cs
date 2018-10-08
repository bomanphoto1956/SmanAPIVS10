using NexusDB.ADOProvider;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using SManApi.ExportToPyramid;

namespace SManApi
{
    public class CMServHuv
    {
        CDB cdb = null;
        CLog log = null;
        public CMServHuv()
        {
            cdb = new CDB();
            log = new CLog();
        }

        private DataTable getWeekPeriod(string vart_ordernr, ref string ErrText)
        {
            string sSql = "select fromDate, toDate "
                        + " from timeReport2 "
                        + " where vart_ordernr = :vart_ordernr ";
            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("vart_ordernr", vart_ordernr);
            ErrText = "";
            DataTable dt = cdb.getData(sSql, ref ErrText, pc);
            return dt;
        }

        /// <summary>
        /// Identifies a servicehuvud by sh.vart_ordernr
        /// Retrieves the matching order and return
        /// </summary>
        /// <param name="sh"></param>
        /// <param name="ident"></param>
        /// <returns>ServHuvCL</returns>
        public ServHuvCL getServiceHuvud(ServHuvCL sh, string ident)
        {
            CReparator cr = new CReparator();
            int identOK = cr.checkIdent(ident);
            if (identOK == -1)
            {
                ServHuvCL sh2 = new ServHuvCL();
                sh2.ErrCode = -10;
                sh2.ErrMessage = "Ogiltigt login";
                return sh2;
            }

            return getServiceHuvud(sh);
        }



        /// <summary>
        /// Identifies a servicehuvud by sh.vart_ordernr
        /// Retrieves the matching order and return
        /// </summary>
        /// <param name="sh"></param>
        /// <param name="ident"></param>
        /// <returns>ServHuvCL</returns>
        private ServHuvCL getServiceHuvud(ServHuvCL sh)
        {

            string sSql = " SELECT sh.vart_ordernr, sh.ert_ordernr, sh.kund, sh.datum, sh.orderAdmin, "
                        + " sh.allrep, sh.godkand, sh.godkand_dat, sh.openForApp, sh.momskod, sh.generalNote, sh.orderLabel "
                        + " FROM ServiceHuvud sh "
                        + " where sh.vart_ordernr = :pVartOrdernr";

            NxParameterCollection np = new NxParameterCollection();
            np.Add("pVartOrdernr", sh.Vart_ordernr);
            string errText = "";
            DataTable dt = cdb.getData(sSql, ref errText, np);

            int errCode = -100;
            if (errText == "" && dt.Rows.Count == 0)
            {
                errText = "Felaktigt Ordernr";
                errCode = 0;
            }


            if (errText != "")
            {

                if (errText.Length > 2000)
                    errText = errText.Substring(1, 2000);
                ServHuvCL sh2 = new ServHuvCL();
                sh2.ErrCode = errCode;
                sh2.ErrMessage = errText;
                return sh2;
            }

            DataTable dtWeeks = getWeekPeriod(sh.Vart_ordernr, ref errText);

            if (errText != "")
            {
                if (errText.Length > 2000)
                    errText = errText.Substring(1, 2000);
                ServHuvCL sh2 = new ServHuvCL();
                sh2.ErrCode = errCode;
                sh2.ErrMessage = errText;
                return sh2;
            }


            DataRow dr = dt.Rows[0];
            ServHuvCL shc = new ServHuvCL();
            shc.Vart_ordernr = dr["vart_ordernr"].ToString();
            shc.OrderDate = Convert.ToDateTime(dr["datum"]);
            shc.OrderAdmin = dr.IsNull("orderAdmin") ? "" : dr["orderAdmin"].ToString();
            shc.OpenForApp = dr.IsNull("openForApp") ? false : Convert.ToBoolean(dr["openForApp"]);
            shc.Kund = dr["kund"].ToString();
            shc.Godkand_dat = dr.IsNull("godkand_dat") ? Convert.ToDateTime("1900-01-01") : Convert.ToDateTime(dr["godkand_dat"]);
            shc.Godkand = dr.IsNull("godkand") ? false : Convert.ToBoolean(dr["godkand"]);
            shc.Ert_ordernr = dr["ert_ordernr"].ToString();
            shc.AllRep = dr.IsNull("allrep") ? false : Convert.ToBoolean(dr["allrep"]);
            shc.momskod = dr.IsNull("momskod") ? 1 : Convert.ToInt16(dr["momskod"]);
            shc.generalNote = dr["generalNote"].ToString();
            shc.orderLabel = dr["orderLabel"].ToString();
            shc.ErrCode = 0;
            shc.ErrMessage = "";
            if (dtWeeks == null || dtWeeks.Rows.Count == 0)
            {
                shc.FromDate = Convert.ToDateTime("1900-01-01");
                shc.ToDate = Convert.ToDateTime("1900-01-01");
            }
            else
            {
                DataRow drWeeks = dtWeeks.Rows[0];
                shc.FromDate = Convert.ToDateTime(drWeeks["fromDate"]);
                shc.ToDate = Convert.ToDateTime(drWeeks["toDate"]);
            }
            return shc;
        }

        private void setParameters(NxParameterCollection np, ServHuvCL sh, string anvId)
        {
            np.Add("vart_ordernr", sh.Vart_ordernr);
            np.Add("ert_ordernr", sh.Ert_ordernr);
            np.Add("kund", sh.Kund);
            np.Add("datum", sh.OrderDate);
            np.Add("momskod", sh.momskod);
            np.Add("generalNote", sh.generalNote);
            np.Add("orderLabel", sh.orderLabel);
            if (sh.IsNew)
            {
                np.Add("reg", anvId);
                np.Add("regdat", System.DateTime.Now);
            }
            else
            {
                np.Add("Uppdaterat", anvId);
                np.Add("Uppdat_dat", System.DateTime.Now);
                np.Add("godkand", sh.Godkand);
                if (sh.Godkand)
                    np.Add("godkand_dat", System.DateTime.Today);
                else
                    np.Add("godkand_dat", System.DBNull.Value);
            }
            np.Add("OrderAdmin", sh.OrderAdmin);
        }


        private string getInsertSQL()
        {
            return " insert into  ServiceHuvud ( vart_ordernr, ert_ordernr, kund, datum, reg, regdat, godkand, utskriven, posttyp, OrderAdmin, OpenForApp, AllRep, SentToPyramid, momskod, generalNote, orderLabel) "
                        + " values( :vart_ordernr, :ert_ordernr, :kund, :datum, :reg, :regdat, false, 0, 1, :OrderAdmin, true, false, false, :momskod, :generalNote, :orderLabel) ";
        }

        private string getUpdateSQL()
        {
            return " update ServiceHuvud "
            + " set ert_ordernr = :ert_ordernr "
            + " , kund = :kund "
            + " , datum = :datum "
            + " , Uppdaterat = :Uppdaterat "
            + " , Uppdat_dat = :Uppdat_dat "
            + " , OrderAdmin = :OrderAdmin "
            + " , godkand = :godkand "
            + " , godkand_dat = :godkand_dat "
            + " , momskod = :momskod "
            + " , generalNote = :generalNote "
            + " , orderLabel = :orderLabel "
            + " where vart_ordernr = :vart_ordernr ";
        }

        private int validateVartOrdernr(string vart_ordernr, ref string ErrText)
        {
            string sSql = " select count(*) antal "
                 + " from ServiceHuvud "
                 + " where vart_ordernr = :vart_ordernr ";

            NxParameterCollection np = new NxParameterCollection();
            np.Add("vart_ordernr", vart_ordernr);

            DataTable dt = cdb.getData(sSql, ref ErrText, np);
            if (ErrText != "")
            {
                ErrText = "Error while checking vårt ordernr. Message " + ErrText;
                return -100;
            }

            int antal = Convert.ToInt32(dt.Rows[0]["antal"]);
            return antal == 0 ? 1 : -1;
        }



        private int validateServHuv(ServHuvCL sh, ref string ErrText)
        {
            if (sh.IsNew)
            {
                int rc = validateVartOrdernr(sh.Vart_ordernr, ref ErrText);
                if (rc < 1)
                    return rc;
            }
            return 1;
        }

        private string getWeekInsertSQL()
        {
            return "insert into timeReport2 (vart_ordernr, fromDate, toDate, approved, Reg, Regdat) "
                    + " values(:vart_ordernr, :fromDate, :toDate, false, :Reg, :Regdat) ";
        }

        private string getWeekUpdateSQL()
        {
            return "update timeReport2 "
                + " set fromDate = :fromDate "
                + " , toDate = :toDate "
                + " , Uppdaterat = :Uppdaterat "
                + " , uppdat_dat = :uppdat_dat "
                + " where vart_ordernr = :vart_ordernr ";
        }

        private void setWeekParameters(NxParameterCollection np, ServHuvCL sh, string RegBy, bool isNew)
        {
            np.Add("vart_ordernr", sh.Vart_ordernr);
            np.Add("fromDate", sh.FromDate);
            np.Add("toDate", sh.ToDate);

            if (isNew)
            {
                np.Add("Reg", RegBy);
                np.Add("Regdat", DateTime.Now);

            }
            else
            {
                np.Add("Uppdaterat", RegBy);
                np.Add("uppdat_dat", DateTime.Now);
            }
        }

        private int countTr2Weeks(string VartOrdernr, ref string errTxt)
        {
            string sSql = " select count(*) countRows "
                        + " from timeReport2 "
                        + " where vart_ordernr = '" + VartOrdernr + "' ";
            DataTable dt = cdb.getData(sSql, ref errTxt);
            int countRows = 0;
            if (dt.Rows.Count > 0)
                countRows = Convert.ToInt32(dt.Rows[0]["countRows"]);
            return countRows;
        }


        public int saveWeeks(ServHuvCL sh, string RegBy, ref string ErrText)
        {
            string sSql = "";
            int countRows = countTr2Weeks(sh.Vart_ordernr, ref ErrText);

            if (ErrText == "")
            {
                if (countRows == 0)
                    sSql = getWeekInsertSQL();
                else
                    sSql = getWeekUpdateSQL();
                NxParameterCollection np = new NxParameterCollection();
                setWeekParameters(np, sh, RegBy, countRows == 0);

                ErrText = "";
                int iRc = cdb.updateData(sSql, ref ErrText, np);
            }

            if (ErrText != "")
            {
                ErrText = "Error while creating date period " + ErrText;
                if (ErrText.Length > 2000)
                    ErrText = ErrText.Substring(1, 2000);
                return -100;
            }

            CTidRed ct = new CTidRed();
            ct.createTr2Weeks(sh.Vart_ordernr, sh.FromDate, sh.ToDate, ref ErrText);
            if (ErrText != "")
            {
                ErrText = "Error while creating weeks for period " + ErrText;
                if (ErrText.Length > 2000)
                    ErrText = ErrText.Substring(1, 2000);
                return -100;
            }

            return 1;

        }



        /// <summary>
        /// Validate, insert or update one servicehuvud
        /// </summary>
        /// <param name="sh"></param>
        /// <param name="ident"></param>
        /// <returns>Newly created servicehuvud</returns>
        // 2018-01-29 KJBO
        public ServHuvCL saveServHuv(ServHuvCL sh, string ident)
        {

            log.log("saveServHuv startas", "0");

            ServHuvCL shc = new ServHuvCL();
            CReparator cr = new CReparator();
            int identOK = cr.checkIdent(ident);

            if (identOK == -1)
            {
                shc.ErrCode = -10;
                shc.ErrMessage = "Ogiltigt login";
                return shc;
            }
            log.log("Efter koll av identitet", "0");

            ReparatorCL repIdent = cr.getReparator(ident);

            if (sh.OrderAdmin.Length == 0)
            {
                shc.ErrCode = -6;
                shc.ErrMessage = "Orderadministratör måste väljas";
                log.log("Orderadministratör måste väljas", "0");
                return shc;
            }

            ReparatorCL rep = cr.getReparatorFromID(sh.OrderAdmin);
            if (rep == null)
            {
                shc.ErrCode = -2;
                shc.ErrMessage = "Felaktig orderadmin";
                log.log("Felaktig admin", "0");
                return shc;
            }

            if (sh.Vart_ordernr.Length == 0)
            {
                shc.ErrCode = -3;
                shc.ErrMessage = "Vårt ordernummer måste anges";
                return shc;
            }

            if (sh.Ert_ordernr.Length == 0)
            {
                shc.ErrCode = -4;
                shc.ErrMessage = "Ert ordernummer måste anges";
                return shc;
            }

            if (sh.Kund == "")
            {
                shc.ErrCode = -5;
                shc.ErrMessage = "Kund måste väljas";
                return shc;
            }

            string ErrTxt = "";
            int rc = validateServHuv(sh, ref ErrTxt);
            log.log("ValidateServHuv returnerar : " + rc.ToString(), "");
            if (ErrTxt != "")
            {
                shc.ErrCode = -101;
                shc.ErrMessage = ErrTxt;
                log.log("Feltext från validateServHuv " + ErrTxt, "0");
                return shc;
            }
            if (rc == -1)
            {
                shc.ErrCode = -7;
                shc.ErrMessage = "Vårt ordernummer används redan i systemet";
                return shc;
            }

            if (sh.FromDate > sh.ToDate)
            {
                shc.ErrCode = -8;
                shc.ErrMessage = "Felaktigt datumintervall";
                return shc;
            }

            CExportToPyramid expPyr = new CExportToPyramid();
            bool shallThisOrderBeSentToPyr = (CConfig.sendToPyramid == 1) && shallSendToPyramid(sh.Vart_ordernr);
            if (shallThisOrderBeSentToPyr)
            {
                shc.ErrMessage = expPyr.checkPyramidAPIAvailable();
                if (shc.ErrMessage != "")
                {
                    shc.ErrCode = -1305;
                    if (shc.ErrMessage.Length > 2000)
                        shc.ErrMessage = shc.ErrMessage.Substring(1, 2000);
                    return shc;
                }
            }
            string sSql = "";
            if (sh.IsNew)
                sSql = getInsertSQL();
            else
                sSql = getUpdateSQL();
            NxParameterCollection np = new NxParameterCollection();
            setParameters(np, sh, repIdent.AnvID);
            log.log("Efter set parameters ", "0");
            string errText = "";
            int iRc = cdb.updateData(sSql, ref errText, np);
            log.log("Feltext från updateData " + errText, "0");
            if (errText != "")
            {

                if (errText.Length > 2000)
                    errText = errText.Substring(1, 2000);
                shc.ErrCode = -100;
                shc.ErrMessage = errText;
                return shc;
            }

            iRc = saveWeeks(sh, repIdent.AnvID, ref errText);
            log.log("SaveWeeks returnerar " + iRc.ToString(), "0");
            if (iRc != 1)
            {
                shc.ErrCode = -100;
                shc.ErrMessage = errText;
                return shc;
            }
            string godkand = "N";
            if (sh.Godkand)
                godkand = "J";
            log.log("Godkänd är " + godkand, "0");
            if (sh.Godkand)
            {
                CTidRed ctr = new CTidRed();
                ctr.closeAllWeeksForOrder(sh.Vart_ordernr);
            }
            log.log("Före sendToPyramid", "0");
            log.log("sentToPyramid returnerar " + CConfig.sendToPyramid.ToString(), "0");

            if (shallThisOrderBeSentToPyr)
            {
                ErrorCL errCl = null;
                log.log("Före  exportToPyramid", "0");
                errCl = expPyr.exportOrder(sh);
                log.log("Efter  exportToPyramid", "0");


                if (errCl.ErrCode != 0)
                {
                    shc.ErrCode = errCl.ErrCode;
                    shc.ErrMessage = errCl.ErrMessage;
                    return shc;
                }


                // 2018-05-17
                if (sh.Godkand && isPyramidOrder(sh.Vart_ordernr))
                {

                    errCl = expPyr.exportTime(sh.Vart_ordernr);
                    if (errCl.ErrCode != 0)
                    {
                        shc.ErrCode = errCl.ErrCode;
                        shc.ErrMessage = errCl.ErrMessage;
                        return shc;
                    }


                    errCl = expPyr.exportReservdel(sh.Vart_ordernr);
                    if (errCl.ErrCode != 0)
                    {
                        shc.ErrCode = errCl.ErrCode;
                        shc.ErrMessage = errCl.ErrMessage;
                        return shc;
                    }

                    errCl = expPyr.exportReservdelKat1(sh.Vart_ordernr);
                    if (errCl.ErrCode != 0)
                    {
                        shc.ErrCode = errCl.ErrCode;
                        shc.ErrMessage = errCl.ErrMessage;
                        return shc;
                    }

                }

                expPyr.setOrderStatus(sh.Vart_ordernr);
                resendToPyramid();
            }


            if (sh.IsNew)
            {
                CServRad csr = new CServRad();

                string err = csr.createFirstRow(sh.Vart_ordernr, sh.Kund);
                if (err != "")
                {
                    shc.ErrCode = -1201;
                    shc.ErrMessage = err;
                    return shc;
                }
            }


            ServHuvCL shRet = getServiceHuvud(sh, ident);
            log.log("getServiceHuvud har meddelande " + shRet.ErrMessage, "0");
            return shRet;

        }




        private void resendToPyramid()
        {
            String sSql = " select vart_ordernr "
                        + " from serviceHuvud "
                        + " where SentToPyramid = false ";
            String errStr = "";
            DataTable dt = cdb.getData(sSql, ref errStr);
            ExportToPyramid.CExportToPyramid expPyr = new ExportToPyramid.CExportToPyramid();
            foreach (DataRow dr in dt.Rows)
            {
                ServHuvCL sh = new ServHuvCL();
                sh.Vart_ordernr = dr["vart_ordernr"].ToString();
                sh = getServiceHuvud(sh);
                ErrorCL errCl = expPyr.exportOrder(sh);
            }

        }

        public DataRow getDeliveryAddress(string kund_id, ref string err)
        {
            String sSql = "SELECT coalesce(lev_adress1,'') lev_adress1 , "
                         + " coalesce(lev_adress2, '') lev_adress2, "
                         + " coalesce(lev_postnr, '') + ' ' + coalesce(lev_stad, '') lev_adress3 "
                         + " , foretagskod "
                         + " , kund "
                         + " , coalesce(lev_postnr, '') + ' ' + coalesce(lev_stad, '') lev_adress4 "
                         + " , coalesce(land, '') land "
                         + " FROM kund "
                         + " where kund_id = :kund_id ";

            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("kund_id", kund_id);
            err = "";
            DataTable dt = cdb.getData(sSql, ref err, pc);

            if (dt.Rows.Count == 0)
            {
                err = "Adressuppgifter saknas för kund : " + kund_id;
                log.log("Adressuppgifter saknas för kund : " + kund_id, "0");
                return null;
            }

            return dt.Rows[0];
        }




        /// <summary>
        /// Updates the AllRep flag on an order
        /// (This flag indicates that all reparators
        /// can log in. The normal process is that
        /// there is a list (in the shReparator table) with
        /// the reparators that can log in on a certain order).
        /// </summary>
        /// <param name="VartOrdernr"></param>
        /// <param name="allRep"></param>
        /// <param name="ident"></param>
        /// <returns></returns>
        /// 2018-02-08 KJBO
        public ServHuvCL updateAllRep(string VartOrdernr, bool allRep, string ident)
        {
            ServHuvCL shc = new ServHuvCL();
            CReparator cr = new CReparator();
            ReparatorCL repIdent = cr.getReparator(ident);


            string sSql = " update servicehuvud "
                        + " set AllRep = :AllRep "
                        + " where vart_ordernr = :vart_ordernr ";
            NxParameterCollection np = new NxParameterCollection();
            np.Add("AllRep", allRep);
            np.Add("vart_ordernr", VartOrdernr);

            string errText = "";
            int iRc = cdb.updateData(sSql, ref errText, np);

            if (errText != "")
            {
                if (errText.Length > 2000)
                    errText = errText.Substring(1, 2000);
                shc.ErrCode = -100;
                shc.ErrMessage = errText;
                return shc;
            }
            shc.Vart_ordernr = VartOrdernr;
            return getServiceHuvud(shc, ident);

        }


        /// <summary>
        /// Calculate the next available order number is this is numeric
        /// Otherwise returns an empty string
        /// </summary>
        /// <returns></returns>
        /// 2018-04-26 KJBO Indentive AB
        public string getNextOrderNumber()
        {
            String sSql = "SELECT max(vart_ordernr) maxOrder "
                        + " FROM ServiceHuvud "
                        + " where vart_ordernr <> '3' "
                        + " and vart_ordernr<> '20151011' ";

            string err = "";
            DataTable dt = cdb.getData(sSql, ref err);
            string maxOrder = "";
            if (dt.Rows.Count == 0)
                return maxOrder;
            string orderNum = dt.Rows[0]["maxOrder"].ToString();
            int n;
            if (int.TryParse(orderNum, out n))
            {
                n++;
                // Check configuration...
                if (n < CConfig.orderStartNumber)
                    n = CConfig.orderStartNumber;
                maxOrder = n.ToString();
            }
            return maxOrder;
        }


        /// <summary>
        /// Returns a list of momskoder
        /// This is used by Pyramid
        /// </summary>
        /// <param name="ident"></param>
        /// <returns></returns>
        /// 2018-04-27 KJBO Indentive AB
        public List<MomskodCL> getMomskoder(string ident)
        {
            List<MomskodCL> momsList = new List<MomskodCL>();
            CReparator cr = new CReparator();
            int identOK = cr.checkIdent(ident);

            if (identOK == -1)
            {
                MomskodCL m = new MomskodCL();
                m.ErrCode = -10;
                m.ErrMessage = "Ogiltigt login";
                momsList.Add(m);
                return momsList;
            }

            momsList.Add(new MomskodCL { Momskod = "Normal moms", MomskodId = 1, ErrCode = 0, ErrMessage = "" });
            momsList.Add(new MomskodCL { Momskod = "Omvänd skattskyldighet", MomskodId = 2, ErrCode = 0, ErrMessage = "" });
            return momsList;
        }



        /// <summary>
        /// Check if the current order has been sent to Pyrmid
        /// </summary>
        /// <param name="vartOrdernr"></param>
        /// <returns></returns>
        public bool isPyramidOrder(string vartOrdernr)
        {
            string sSql = " SELECT coalesce(SentToPyramid,false) SentToPyramid "
                        + " FROM ServiceHuvud sh "
                        + " where vart_ordernr = :vart_ordernr ";
            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("vart_ordernr", vartOrdernr);
            string err = "";
            DataTable dt = cdb.getData(sSql, ref err, pc);
            if (dt.Rows.Count == 0)
                return false;
            bool value = Convert.ToBoolean(dt.Rows[0]["SentToPyramid"]);
            return Convert.ToBoolean(dt.Rows[0]["SentToPyramid"]);
        }



        /// <summary>
        /// This function returns true for all new orders where SentToPyramid
        /// is either true or false.
        /// All old orders returns false
        /// </summary>
        /// <param name="vartOrdernr"></param>
        /// <returns></returns>
        private bool shallSendToPyramid(string vartOrdernr)
        {
            string sSql = " SELECT count(*) antal "
                        + " FROM ServiceHuvud sh "
                        + " where vart_ordernr = :vart_ordernr "
                        + " and SentToPyramid is not null ";
            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("vart_ordernr", vartOrdernr);
            string err = "";
            DataTable dt = cdb.getData(sSql, ref err, pc);
            return Convert.ToInt16(dt.Rows[0]["antal"]) > 0;

        }


    }
}