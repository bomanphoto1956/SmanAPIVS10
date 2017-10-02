using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using NexusDB.ADOProvider;
using System.Runtime.InteropServices;

namespace SManApi
{    

    public class CTidRed
    {
        CDB cdb = null;

 /*
        [DllImport("PTimeRep2.dll",
        CallingConvention = CallingConvention.StdCall,
        CharSet = CharSet.Ansi)]

        public static extern int
            createTimeRep2();

        */


        public CTidRed()
        {
            cdb = new CDB(); 
        }

        /// <summary>
        /// Returns a list of valid dates for
        /// registry of time for one ServiceRow
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="SrAltKey">Alternate key</param>
        /// <returns>List of dates or an error message</returns>
        public List<OpenDateCL> getOpenDates(string ident, string SrAltKey)
        {
            return getOpenDates(ident, SrAltKey, true, 0);
        }


        /// <summary>
        /// Returns a list of valid dates for
        /// registry of time for one ServiceOrder
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="vartOrdernr"></param>
        /// <returns></returns>
        /// 2017-03-15 KJBO
        public List<OpenDateCL> getOpenDatesSH(string ident, string vartOrdernr)
        {
            return getOpenDates(ident, "", true, 2, vartOrdernr);            
        }



        /// <summary>
        /// New function for timeRegVersion 2
        /// called by getOpenDates(string ident, string SrAltKey, bool bValidate, int timeRegVersion = 0)
        /// which has a different signature
        /// </summary>
        /// <param name="vartOrdernr"></param>
        /// <returns></returns>
        private List<OpenDateCL> getOpenDates(string vartOrdernr)
        {

            List<OpenDateCL> dateList = new List<OpenDateCL>();
            // Select clause
            string sSql = " SELECT fromDate, toDate "
                        + " FROM timeReport2 "
                        + " where vart_ordernr = :vart_ordernr ";
            // Add parameter
            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("vart_ordernr", vartOrdernr);

            // Init error variable and perform SQL
            string err = "";
            DataTable dt = cdb.getData(sSql, ref err, pc);

            int errCode = -100;

            // If no error from database but no row exists
            if (err == "" && dt.Rows.Count == 0)
            {
                err = "Det finns inga upplagda datum för tidregistrering ";
                errCode = 0;
            }

            // If error from database
            if (err != "")
            {
                OpenDateCL d = new OpenDateCL();
                if (err.Length > 2000)
                    err = err.Substring(1, 2000);
                d.ErrCode = errCode;
                d.ErrMessage = err;
                dateList.Add(d);
                return dateList;
            }

            // If we are here then everything is fine and we can concentrate
            // on building a datelist
            DateTime dat = Convert.ToDateTime(dt.Rows[0]["fromDate"]);
            DateTime toDate = Convert.ToDateTime(dt.Rows[0]["toDate"]);
            while (dat <= toDate)
            {
                OpenDateCL d = new OpenDateCL();
                d.Datum = dat;
                d.ErrCode = 0;
                d.ErrMessage = "";
                dateList.Add(d);
                dat = dat.AddDays(1);
            }
            // Return the list
            return dateList;

        }



        /// <summary>
        /// Returns a list of valid dates for
        /// registry of time for one ServiceRow or ServiceOrder
        /// 2017-03-14 KJBO
        /// Added functionality to handle timeRegistryVersion 2
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="SrAltKey">Alternate key</param>
        /// <returns>List of dates or an error message</returns>
        private List<OpenDateCL> getOpenDates(string ident, string SrAltKey, bool bValidate, int timeRegVersion = 0, string vart_ordernr = "")
        {
            // Create a list of open dates for use in return clause
            List<OpenDateCL> dateList = new List<OpenDateCL>();

            // If we need to validate user...
            if (bValidate)
            {
                CReparator cr = new CReparator();
                int identOK = cr.checkIdent(ident);
                if (identOK == -1)
                {
                    OpenDateCL d = new OpenDateCL();
                    d.ErrCode = -10;
                    d.ErrMessage = "Ogiltigt login";
                    dateList.Add(d);
                    return dateList;
                }
            }


            if (vart_ordernr == "")
            {
                // We need to retrieve vart_ordernr and radnr
                // from alternate key
                CServRad crv = new CServRad();
                DataTable dt = crv.validateServRad(SrAltKey);
                // If error then return
                if (dt.Rows.Count == 0)
                {
                    OpenDateCL d = new OpenDateCL();
                    d.ErrCode = -10;
                    d.ErrMessage = "Ogiltig ServiceRad";
                    dateList.Add(d);
                    return dateList;
                }

                // Store local variables
                vart_ordernr = dt.Rows[0]["vart_ordernr"].ToString();
                int radnr = Convert.ToInt32(dt.Rows[0]["radnr"]);
            }

            // timeRegVersion 0 means that this function shall
            // determine the right timeRegVersion
            if (timeRegVersion == 0)
            {
               CMisc cm = new CMisc();
               timeRegVersion = cm.getTimeRegVersion(ident, vart_ordernr);
            }

            // If we need validation then check if the current order
            // is openForApp, godkand or (in case of timeRegVersion 2)
            // also check if the timeRegistry is approved
            if (bValidate)
            {
                CServiceHuvud ch = new CServiceHuvud();
                DataTable dt = ch.validateOrderOpenGodkand(vart_ordernr);
                bool bOpenForApp = Convert.ToBoolean(dt.Rows[0]["OpenForApp"]);
                bool bGodkand = Convert.ToBoolean(dt.Rows[0]["Godkand"]);
                // Return if not open for app
                if (!bOpenForApp)
                {
                    OpenDateCL d = new OpenDateCL();
                    d.ErrCode = -10;
                    d.ErrMessage = "Aktuell order är stängd för inmatning";
                    dateList.Add(d);
                    return dateList;
                }
                // return if godkand
                if (bGodkand)
                {
                    OpenDateCL d = new OpenDateCL();
                    d.ErrCode = -10;
                    d.ErrMessage = "Aktuell order är godkänd. Ändring ej tillåten ";
                    dateList.Add(d);
                    return dateList;
                }

                // Return if timeRegVersion = 2 and the time registry is approved
                // and thus closed 
                if (timeRegVersion == 2)
                {
                    if (isTime2Approved(vart_ordernr))
                    {
                        OpenDateCL d = new OpenDateCL();
                        d.ErrCode = -10;
                        d.ErrMessage = "Aktuell tidredovisning är stängd och kan inte ändras ";
                        dateList.Add(d);
                        return dateList;

                    }                    
                }               
            }


            // Create date list for timeRegVersion 1
            if (timeRegVersion == 1)
            {
                string sSql = "";
                for (int i = 1; i <= 7; i++)
                {
                    if (i > 1)
                        sSql += " union ";
                    sSql += " select distinct datum" + i.ToString() + " ";
                    if (i == 1)
                        sSql += " datum ";
                    sSql += " from tidrapp "
                          + " where vart_ordernr = :vart_ordernr ";
                }
                sSql += " order by 1 ";

                NxParameterCollection pc = new NxParameterCollection();
                pc.Add("vart_ordernr", vart_ordernr);

                string err = "";
                DataTable dt = cdb.getData(sSql, ref err, pc);

                int errCode = -100;

                if (err == "" && dt.Rows.Count == 0)
                {
                    err = "Det finns inga upplagda datum för tidregistrering ";
                    errCode = 0;
                }


                if (err != "")
                {
                    OpenDateCL d = new OpenDateCL();
                    if (err.Length > 2000)
                        err = err.Substring(1, 2000);
                    d.ErrCode = errCode;
                    d.ErrMessage = err;
                    dateList.Add(d);
                    return dateList;
                }

                foreach (DataRow dr in dt.Rows)
                {
                    OpenDateCL d = new OpenDateCL();
                    d.Datum = Convert.ToDateTime(dr["datum"]);
                    d.ErrCode = 0;
                    d.ErrMessage = "";
                    dateList.Add(d);
                }

                return dateList;
            }
            // Call function for timeRegVersion 2
            return getOpenDates(vart_ordernr);
        }

        

        /// <summary>
        /// Get a specific TidRed record identified by ID (PK)
        /// </summary>
        /// <param name="ident">Identity</param>
        /// <param name="ID">ID of the ServRadRepTid</param>
        /// <returns>One instance of the ServRadRepTidCL class or one row with an error</returns>
        // 2016-02-15 KJBO Pergas AB
        public ServRadRepTidCL getServRadRepTid(string ident, int ID)
        {
            List<ServRadRepTidCL> srrList = getServRadRepTidForServiceRad(ident, "", ID, "");
            return srrList[0];
        }

        /// <summary>
        /// Returns all registered time(all rows)
        /// for a specific service row (identified by srAltKey)
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="srAltKey"></param>
        /// <returns></returns>
        //  2016-11-18 Pergas AB KJBO
        public List<ServRadRepTidCL> getServRadRepTidForSR(string ident, string srAltKey)
        {
            return getServRadRepTidForServiceRad(ident, srAltKey, 0, "");
        }


        /// <summary>
        /// Returns all registered time(all rows)
        /// for a specific service row (identified by srAltKey)
        /// and for a specific user (identified by AnvID)
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="AnvID"></param>
        /// <param name="srAltKey"></param>
        /// <returns></returns>
        //  2016-02-15 Pergas AB KJBO
        public List<ServRadRepTidCL> getServRadRepTidForServiceRad(string ident, string AnvID, string srAltKey)
        {
            return getServRadRepTidForServiceRad(ident, srAltKey, 0, AnvID);
        }

        
        /// <summary>
        /// General function for retrieving one or 
        /// several ServRadTidRep rows
        /// Either ID or srAltKey must be provided
        /// </summary>
        /// <param name="ident">Identity</param>
        /// <param name="srAltKey">Alternate key to servRad (can be empty string)</param>
        /// <param name="ID">Primary key</param>
        /// <returns>List of ServRadTidRepCL or one row with error message</returns>
        //  2016-02-15 KJBO Pergas AB
        private List<ServRadRepTidCL> getServRadRepTidForServiceRad(string ident, string srAltKey, int ID, string AnvID)
        {
            List<ServRadRepTidCL> srrRows = new List<ServRadRepTidCL>();

            CReparator cr = new CReparator();
            int identOK = cr.checkIdent(ident);

            if (identOK == -1)
            {
                ServRadRepTidCL srr = new ServRadRepTidCL();
                srr.ErrCode = -10;
                srr.ErrMessage = "Ogiltigt login";
                srrRows.Add(srr);
                return srrRows;
            }

            NxParameterCollection pc = new NxParameterCollection();
            string sSql = " SELECT srrt.ID, srrt.srAltKey, srrt.anvID, srrt.tid, srrt.datum, srrt.TimeTypeID, srrt.SalartID, srrt.rep_kat_id "
                        + " FROM ServradRepTid srrt ";
            if (ID != 0)
            {
                sSql += " where srrt.ID = :ID ";
                pc.Add("ID", ID);
            }
            else
            {
                if (srAltKey != "")
                {
                    sSql += " where srrt.srAltKey = :srAltKey ";
                    pc.Add("srAltKey", srAltKey);
                }
                if (AnvID != "")
                {
                    sSql += " and srrt.anvID = :anvID ";
                    pc.Add("anvID", AnvID);
                }

            }
            
            

            string errText = "";

            DataTable dt = cdb.getData(sSql, ref errText, pc);

            int errCode = -100;

            if (errText == "" && dt.Rows.Count == 0)
            {
                if (ID != 0)
                    errText = "Felaktigt ID";
                else
                    errText = "Inga tidsinmatning registrerade för aktuell användare och servicerad";
                errCode = 0;
            }


            if (errText != "")
            {
                ServRadRepTidCL srr = new ServRadRepTidCL();
                if (errText.Length > 2000)
                    errText = errText.Substring(1, 2000);
                srr.ErrCode = errCode;
                srr.ErrMessage = errText;
                srrRows.Add(srr);
                return srrRows;
            }

            foreach (DataRow dr in dt.Rows)
            {
                ServRadRepTidCL srr = new ServRadRepTidCL();
                srr.ID = Convert.ToInt32(dr["ID"]);
                srr.SrAltKey = dr["srAltKey"].ToString();
                srr.AnvID = dr["anvID"].ToString();
                srr.Datum = Convert.ToDateTime(dr["datum"]);
                srr.Tid = Convert.ToDecimal(dr["tid"]);
                srr.timeTypeID = Convert.ToInt32(dr["TimeTypeID"]);
                if (dr["SalartID"] == DBNull.Value)
                    srr.SalartID = 0;
                else
                    srr.SalartID = Convert.ToInt32(dr["SalartID"]);
                srr.RepKatID = dr["rep_kat_id"].ToString();
                srr.ErrCode = 0;
                srr.ErrMessage = "";
                srrRows.Add(srr);
            }
            return srrRows;
        }



        private string getInsertSQL()
        {
            string sSql = " insert into ServradRepTid (  anvID, datum, regdat, srAltKey "
                         + " , tid, attesterad, TimeTypeID, SalartID, rep_kat_id)  "
                        + "  values ( :anvID, :datum, :regdat, :srAltKey "
                        + " , :tid, false, :TimeTypeID, :SalartID, :rep_kat_id )";

            return sSql;
        }



        private string getUpdateSQL()
        {
            string sSql = " update ServradRepTid "                                                 
                        + " set anvID = :anvID "
                        + ", datum = :datum "                        
                        + ", srAltKey = :srAltKey "
                        + ", tid = :tid "
                        + ", uppdat_dat = :uppdat_dat "
                        + ", TimeTypeID = :TimeTypeID "
                        + ", SalartID = :SalartID "
                        + ", rep_kat_id = :rep_kat_id "
                        + " where ID = :ID ";
            return sSql;

        }

        private string getDeleteSQL()
        {
            string sSql = " delete from ServradRepTid "
                        + " where ID = :ID ";
            return sSql;
        }


        private void setParameters( NxParameterCollection np, ServRadRepTidCL sr)
        {
            if (sr.ID != 0)            
                np.Add("ID", sr.ID);            
            np.Add("anvID", sr.AnvID);                        
            np.Add("datum", sr.Datum);                       
            np.Add("regdat", System.DateTime.Now);            
            np.Add("srAltKey", sr.SrAltKey);                        
            np.Add("tid", sr.Tid);
            np.Add("uppdat_dat", System.DateTime.Now);
            np.Add("TimeTypeID", sr.timeTypeID);
            if (sr.SalartID == 0)
                np.Add("SalartID", System.DBNull.Value);
            else
                np.Add("SalartID", sr.SalartID);
            np.Add("rep_kat_id", sr.RepKatID);

        }

        private int validateAlternateKey(string alternateKey)
        {
            CServRad crs = new CServRad();

            DataTable dt = crs.validateServRad(alternateKey);
            return dt.Rows.Count;
        }


        private int validateAnvID( string anvID)
        {
            CReparator cr = new CReparator();

            return cr.getName(anvID).Length;
        }

        private int validateDatum( ServRadRepTidCL srt, string ident, int timeRegVersion)
        {
            List<OpenDateCL> dateList = getOpenDates(ident, srt.SrAltKey, false, timeRegVersion);

            OpenDateCL dateToCheck = new OpenDateCL();

            dateToCheck.Datum = srt.Datum;
            dateToCheck.ErrCode = 0;
            dateToCheck.ErrMessage = "";  
          
            foreach (OpenDateCL od in dateList)
            {                
                if (od.Datum == srt.Datum)                                    
                    return 1;                
            }

            return 0;
        }


        private int validateDuplicate(ServRadRepTidCL srt, int timeRegVersion)
        {
            string sSql = " SELECT count(*) as antal "
                        + " from ServRadRepTid "
                        + " where srAltKey = :srAltKey "
                        + " and anvID = :anvID "
                        + " and datum = :datum "
                        + " and TimeTypeID = :TimeTypeID ";
            if (timeRegVersion == 2)
            {
                sSql += " and SalartID = :SalartID "
                        + " and rep_kat_id = :rep_kat_id ";
            }
            if (srt.ID != 0)
                sSql += " and ID <> :ID ";

            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("srAltKey", srt.SrAltKey);
            pc.Add("anvID", srt.AnvID);
            pc.Add("datum", srt.Datum);
            pc.Add("TimeTypeID", srt.timeTypeID);
            if (srt.ID != 0)
                pc.Add("ID", srt.ID);
            if (timeRegVersion == 2)
            {
                pc.Add("SalartID", srt.SalartID);
                pc.Add("rep_kat_id", srt.RepKatID);
            }

            string er = "";

            DataTable dt = cdb.getData(sSql, ref er, pc);
            return Convert.ToInt32(dt.Rows[0]["antal"]);
        }

        private int validateTid( Decimal tid, int salartId, ref string err)
        {

            if (tid < 0)
            {
                err = "Registrerad tid får inte vara negativ";
                return 0;
            }
            CSalart csa = new CSalart();
            string unit = csa.getSalartUnit(salartId);
            if (tid > 24 && unit.ToUpper() == "TIM")
            {
                err = "Registrerad tid får inte överstiga 24 timmar";
                return 0;
            }
            return 1;
        }



        private int validateTimeType( int timeTypeID)
        {
            string sSql = " select count(*) antal "
                        + " from TimeType "
                        + " where TimeTypeID = :TimeTypeID ";

            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("TimeTypeID", timeTypeID);

            string er = "";
            DataTable dt = cdb.getData(sSql, ref er, pc);
            if (Convert.ToInt32(dt.Rows[0]["antal"]) == 1)
                return 1;
            return 0;

        }

        private int validateAttested(ServRadRepTidCL srt)
        {
            if (srt.ID == 0)
                return 0;
            string sSql = "SELECT attesterad "
                        + "FROM ServRadRepTid "
                        + "where ID = :ID ";
            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("ID", srt.ID);

            string er = "";
            DataTable dt = cdb.getData(sSql, ref er, pc);

            if (dt.Rows.Count == 0)
                return 0;

            Boolean bAttest = Convert.ToBoolean(dt.Rows[0]["attesterad"]);

            if (bAttest)
                return 1;
            return 0;
        }


        private int validateServRadRepTid(ServRadRepTidCL srt, string ident, ref string err, int timeRegVersion)
        {
            err = "";
            if (validateAlternateKey(srt.SrAltKey) == 0)            
                return -1;            
            if (validateAnvID(srt.AnvID) == 0 )
                return -2;
            if (validateDatum(srt, ident, timeRegVersion) == 0)
                return -3;
            if (validateTid(srt.Tid, srt.SalartID, ref err) == 0)
                return -4;
            if (validateDuplicate(srt, timeRegVersion) > 0)
                return -5;
            if (validateTimeType(srt.timeTypeID) == 0)
                return -6;
            if (validateAttested(srt) == 1)
                return -7;
            return 1;
        }

        private int validateSalart(int SalartID, bool forServiceDetalj, ref int salartCatID)
        {
            CSalart csa = new CSalart();
            int valid = csa.validateSalart(SalartID, forServiceDetalj, ref salartCatID);
            if (valid == 1)
                return 1;
            return -1;            
        }

        private int validateRepKat(string repKatID)
        {
            CReparator cr = new CReparator();
            int valid = cr.validateRepKat(repKatID);
            if (valid == 1)
                return 1;
            return -1;
        }

        private int getLastInserted( string AnvID)
        {
            string sSql = " SELECT coalesce(max(ID),0) as MaxID "
                        + " FROM ServradRepTid " 
                        + " where AnvID = :AnvID ";
            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("AnvID", AnvID);

            string er = "";
            DataTable dt = cdb.getData(sSql, ref er, pc);
            return Convert.ToInt32(dt.Rows[0]["MaxID"]);
        }

        
        /// <summary>
        /// Validates one ServRadRepTid
        /// If the ID is 0 then this method
        /// assumes that this is a new row
        /// Returns the validated and stored
        /// row with the new ID (if its a new row)
        /// If an error occurs then an error is returned
        /// in the ServRadTidRep return row
        /// </summary>
        /// <param name="ident">Identity</param>
        /// <param name="srt">ServRadTidRepCL</param>
        /// <returns>The saved row or an error</returns>
        ///  2016-02-15 KJBO Pergas AB
        ///  Added handling for generation 2 of timeregistry
        ///  2017-03-13 KJBO Pergas AB
        public ServRadRepTidCL saveServRadRepTid(string ident, ServRadRepTidCL srt)
        {
            bool bNew = false;
            // 2016-06-17
            bool bDeleted = false;
            CReparator cr = new CReparator();
            int identOK = cr.checkIdent(ident);

            ServRadRepTidCL retSrt = new ServRadRepTidCL();
            if (identOK == -1)
            {
                retSrt.ErrCode = -10;
                retSrt.ErrMessage = "Ogiltigt login";
                return retSrt;
            }

            CServRad crs = new CServRad();
            DataTable dtsr = crs.validateServRad(srt.SrAltKey);

            string sVartOrdernr = dtsr.Rows[0]["vart_ordernr"].ToString();

            CServiceHuvud ch = new CServiceHuvud();
            string sOpen = ch.isOpen(ident, sVartOrdernr);
            if (sOpen != "1")
            {                
                retSrt.ErrCode = -10;
                if (sOpen == "-1")
                    retSrt.ErrMessage = "Order är stängd för inmatning";
                else
                    retSrt.ErrMessage = sOpen;
                return retSrt;                
            }


            if (isTime2Approved(sVartOrdernr))
            {
                retSrt.ErrCode = -10;
                retSrt.ErrMessage = "Aktuell tidredovisning är stängd och kan inte ändras ";
                return retSrt;                
            }

            // Added 2017-03-13 for generation 2 handling
            CMisc cm = new CMisc();
            int timeRegVersion = cm.getTimeRegVersion(ident, sVartOrdernr);


            string err = "";
            int valid = validateServRadRepTid(srt, ident, ref err, timeRegVersion);

            if (valid == -1)
            {
                retSrt.ErrCode = -1;
                retSrt.ErrMessage = "Felaktig servicerad";
                return retSrt;
            }

            if (valid == -2)
            {
                retSrt.ErrCode = -1;
                retSrt.ErrMessage = "Felaktigt användarID";
                return retSrt;
            }

            if (valid == -3)
            {
                retSrt.ErrCode = -1;
                retSrt.ErrMessage = "Felaktigt datum";
                return retSrt;
            }

            if (valid == -4)
            {
                retSrt.ErrCode = -1;
                retSrt.ErrMessage = err;
                return retSrt;
            }

            // 2016-06-17 added clause
            if (valid == -5)
            {
                retSrt.ErrCode = -1;
                if (timeRegVersion == 1)
                    retSrt.ErrMessage = "Det finns redan tid redovisat för aktuell dag och ventil ";
                else
                    retSrt.ErrMessage = "Det finns redan tid redovisat för aktuell användare, dag, ventil och löneart ";
                return retSrt;
            }

            // 2016-11-01 KJBO
            if (valid == -6)
            {
                retSrt.ErrCode = -1;
                retSrt.ErrMessage = "Felaktigt TimeTypeID";
                return retSrt;
            }

            if (valid == -7)
            {
                retSrt.ErrCode = -1;
                retSrt.ErrMessage = "Tidregistreringen är attesterad";
                return retSrt;

            }



            if (timeRegVersion == 2)
            {                
                if (srt.SalartID == 0)
                {
                    retSrt.ErrCode = -1;
                    retSrt.ErrMessage = "Löneart saknas";
                    return retSrt;
                }
                if (srt.RepKatID == "")
                {
                    retSrt.ErrCode = -1;
                    retSrt.ErrMessage = "Reparatörskategori saknas";
                    return retSrt;
                }

                int dontNeed = 0;
                if (validateSalart(srt.SalartID, true, ref dontNeed) == -1)
                {
                    retSrt.ErrCode = -1;
                    retSrt.ErrMessage = "Felaktig löneart";
                    return retSrt;
                }
                if (validateRepKat(srt.RepKatID) == -1)
                {
                    retSrt.ErrCode = -1;
                    retSrt.ErrMessage = "Felaktig reparatörskategor";
                    return retSrt;
                }

            }

            string sSql = "";

            // Nothing to save... 2016-06-17
            if (srt.ID == 0 && srt.Tid == 0)
                return srt;


            // This is a new ventil
            if (srt.ID == 0)
            {
                sSql = getInsertSQL();
                bNew = true;
            }
            else if (srt.Tid == 0)
            {
                sSql = getDeleteSQL();
                bDeleted = true;
            }
            else
                sSql = getUpdateSQL();
           
            NxParameterCollection np = new NxParameterCollection();
            setParameters(np, srt);
            
            string errText = "";

            int iRc = cdb.updateData(sSql, ref errText, np);

            if (errText != "")
            {
                if (errText.Length > 2000)
                    errText = errText.Substring(1, 2000);
                retSrt.ErrCode = -100;
                retSrt.ErrMessage = errText;
                return retSrt;
            }

            if (bNew)
            {
                srt.ID = getLastInserted(srt.AnvID);

                // 2016-04-04 KJBO
                crs.ensureReparatorExists(ident, srt.SrAltKey, srt.AnvID);
            }
            if (bDeleted)
                return srt;
            return getServRadRepTid(ident, srt.ID);
        }



        /// <summary>
        /// Sum all registered hours for one servicerad
        /// </summary>
        /// <param name="ident">ident</param>
        /// <param name="srAltKey">alternate key for servicerad</param>
        /// <param name="AnvID">UserID or empty string for all users</param>
        /// <returns></returns>
        public Decimal SumHoursForServRad(string ident, string srAltKey, string AnvID)
        {            

            CReparator cr = new CReparator();
            int identOK = cr.checkIdent(ident);

            if (identOK == -1)                            
                return -1;            

            string sSql = " SELECT coalesce(sum(tid),0) as sum_hours "
                        + " FROM ServradRepTid "
                        + " where srAltKey = :srAltKey ";
            if (AnvID != "")
               sSql += " and anvID = :anvID ";


            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("srAltKey", srAltKey);
            if (AnvID != "")
                pc.Add("anvID", AnvID);


            string errText = "";

            DataTable dt = cdb.getData(sSql, ref errText, pc);


            if (errText == "" && dt.Rows.Count == 0)            
                return 0;            


            if (errText != "")
            {
                return -2;
            }

            DataRow dr = dt.Rows[0];

            return Convert.ToDecimal(dr["sum_hours"]);
        }



        /// <summary>
        /// Sum all working hours for one servicerad 
        /// </summary>
        /// <param name="ident">identity</param>
        /// <param name="srAltKey">alternate key for servicerad</param>
        /// <returns>Number of working hours for one servicerad</returns>
        /// 2016-06-17 KJBO
        public Decimal SumHoursForServRad(string ident, string srAltKey)
        {
            return SumHoursForServRad(ident, srAltKey, "");
        }



        /// <summary>
        /// Returns all time registry for a given order
        /// </summary>
        /// <param name="ident">Identity</param>
        /// <param name="vartOrdernr">Order number</param>
        /// <returns>List of RepTidListCL</returns>
        public List<RepTidListCL> getAllTimeForOrder(string ident, string vartOrdernr)
        {

            List<RepTidListCL> rtls = new List<RepTidListCL>();

            CReparator cr = new CReparator();
            int identOK = cr.checkIdent(ident);

            if (identOK == -1)
            {
                RepTidListCL rtl = new RepTidListCL();                
                rtl.ErrCode = -10;
                rtl.ErrMessage = "Ogiltigt login";
                rtls.Add(rtl);
                return rtls;
            }


            string sSql = " SELECT sr.vart_ordernr, sr.radnr, sr.AlternateKey, srt.id servRadRepTidID, tt.TimeTypeID, tt.TimeType, srt.AnvID, r.reparator, "
                        + " ' Position : ' + sr.kundens_pos + ' (' + coalesce(v.fabrikat,'') + ' ' + vk.ventilkategori + ' aonr: ' + coalesce(sr.arbetsordernr,'') + ')' rowDescription "
                        + " , srt.datum, srt.tid, sr.kundens_pos "
                        + " FROM servicerad sr "
                        + " join ventil v on sr.ventil_id = v.ventil_id "
                        + " join ventilKategori vk on v.ventilkategori = vk.ventilkat_id "
                        + " join servRadRepTid srt on sr.AlternateKey = srt.srAltKey "
                        + " join TimeType tt on srt.TimeTypeID = tt.TimeTypeID "
                        + " join reparator r on srt.AnvID = r.AnvID "
                        + " where sr.vart_ordernr = :vart_ordernr ";


            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("vart_ordernr",vartOrdernr);
            

            string errText = "";

            DataTable dt = cdb.getData(sSql, ref errText, pc);

            int errCode = -100;

            if (errText == "" && dt.Rows.Count == 0)
            {
                errText = "Inga tidsinmatning registrerade för aktuell order";
                errCode = 0;
            }


            if (errText != "")
            {
                RepTidListCL rtl = new RepTidListCL();                                
                if (errText.Length > 2000)
                    errText = errText.Substring(1, 2000);
                rtl.ErrCode = errCode;
                rtl.ErrMessage = errText;
                rtls.Add(rtl);
                return rtls;
            }

            foreach (DataRow dr in dt.Rows)
            {
                RepTidListCL rtl = new RepTidListCL();
                rtl.vartOrdernr = dr["vart_ordernr"].ToString();
                rtl.radnr = Convert.ToInt32(dr["radnr"]);
                rtl.srAltKey = dr["AlternateKey"].ToString();
                rtl.ServRadRepTidId = Convert.ToInt32(dr["servRadRepTidID"]);
                rtl.timeTypeID = Convert.ToInt32(dr["TimeTypeID"]);
                rtl.timeType = dr["TimeType"].ToString();
                rtl.anvID = dr["AnvID"].ToString();
                rtl.reparator = dr["reparator"].ToString();
                rtl.rowDescription = dr["rowDescription"].ToString();
                rtl.datum = Convert.ToDateTime(dr["datum"]);
                rtl.tid = Convert.ToDecimal(dr["tid"]);
                rtl.position = dr["kundens_pos"].ToString();
                rtl.ErrCode = 0;
                rtl.ErrMessage = "";
                rtls.Add(rtl);
            }
            return rtls;
        }

        /// <summary>
        /// Get valid time types for one order
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="vartOrdernr"></param>
        /// <returns>Valid time types</returns>
        // 2017-03-14 KJBO
        public List<TimeTypeCL> getTimeTypesForOrder(string ident, string vartOrdernr)
        {
            string sSql = "SELECT count(*) hos_kund "
                        + "FROM servicerad "
                        + "where vart_ordernr = :vart_ordernr "
                        + "and hos_kund ";

            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("vart_ordernr", vartOrdernr);

            string errText = "";

            DataTable dt = cdb.getData(sSql, ref errText, pc);

            bool bHosKund = false;

            if (dt.Rows.Count > 0)
            {
                bHosKund = Convert.ToInt32(dt.Rows[0]["hos_kund"]) > 0;
            }


            sSql = "SELECT count(*) pa_verkstad "
                + "FROM servicerad "
                + "where vart_ordernr = :vart_ordernr "
                + "and pa_verkstad ";

            pc = new NxParameterCollection();
            pc.Add("vart_ordernr", vartOrdernr);
            errText = "";

            dt = cdb.getData(sSql, ref errText, pc);

            bool bPaVerkstad = false;

            if (dt.Rows.Count > 0)
            {
                bPaVerkstad = Convert.ToInt32(dt.Rows[0]["pa_verkstad"]) > 0;
            }
            return getTimeTypes(ident, bHosKund, bPaVerkstad);
        }




        /// <summary>
        /// Return a list of valid timeTypes
        /// The list varies depending on the hosKund and paVerkstad parameters
        /// Normaly you check the corresponding Servicerad and the hosKund and paVerkstad
        /// and send those variables to this function thus getting the right list.
        /// To override (and get alla timeTypes) you just set both parameters to true
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="hosKund"></param>
        /// <param name="paVerkstad"></param>
        /// <param name="all"></param>
        /// <returns></returns>
        public List<TimeTypeCL> getTimeTypes( string ident, bool hosKund, bool paVerkstad)
        {
            List<TimeTypeCL> tts = new List<TimeTypeCL>();

            CReparator cr = new CReparator();
            int identOK = cr.checkIdent(ident);

            if (identOK == -1)
            {
                TimeTypeCL tt = new TimeTypeCL();
                tt.ErrCode = -10;
                tt.ErrMessage = "Ogiltigt login";
                tts.Add(tt);
                return tts;
            }


            string sSql = " select tt.timeTypeID, tt.TimeType "
                        + " from TimeType tt "                        
                        + " where 1 = 1 ";
            if (hosKund && !paVerkstad)
                sSql += " and tt.TimeTypeBaseID = 1 ";
            if (!hosKund && paVerkstad)
                sSql += " and tt.TimeTypeBaseID = 2 ";
            if (!hosKund && !paVerkstad)
                sSql += " and tt.TimeTypeBaseID = 3 ";

            string errText = "";

            DataTable dt = cdb.getData(sSql, ref errText, null);

            if (errText != "")
            {
                TimeTypeCL tt = new TimeTypeCL();
                if (errText.Length > 2000)
                    errText = errText.Substring(1, 2000);
                tt.ErrCode = -100;
                tt.ErrMessage = errText;
                tts.Add(tt);
                return tts;
            }

            foreach (DataRow dr in dt.Rows)
            {
                TimeTypeCL tt = new TimeTypeCL();
                tt.TimeTypeID = Convert.ToInt32(dr["timeTypeID"]);
                tt.TimeType = dr["TimeType"].ToString();
                tt.ErrCode = 0;
                tt.ErrMessage = "";
                tts.Add(tt);            
            }
            return tts;

        }


        public bool isTime2Approved (string vartOrdernr)
        {


            string sSql = " SELECT approved "
                        + " FROM timeReport2 "
                        + " where vart_ordernr = :vartOrdernr ";
            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("vartOrdernr", vartOrdernr);

            string errText = "";
            DataTable dt = cdb.getData(sSql, ref errText, pc);
            
            if (errText == "" && dt.Rows.Count == 0)
            {
                return false;
            }

            if (dt.Rows.Count == 0)
                return false;

            return Convert.ToBoolean(dt.Rows[0]["approved"]);


        }


        private int validateDatum(ServHuvRepTidCL sht)
        {
            List<OpenDateCL> dateList = getOpenDates(sht.VartOrdernr);

            OpenDateCL dateToCheck = new OpenDateCL();

            dateToCheck.Datum = sht.Datum;
            dateToCheck.ErrCode = 0;
            dateToCheck.ErrMessage = "";

            foreach (OpenDateCL od in dateList)
            {
                if (od.Datum == sht.Datum)
                    return 1;
            }

            return 0;
        }

        private int validateDuplicate(ServHuvRepTidCL sht, int salartTypeCategory)
        {

            string sSql = "SELECT count(*) as antal "
                        + "from ServHuvRepTid "
                        + "where vart_ordernr = :vart_ordernr "
                        + " and datum = :datum "
                        + " and TimeTypeID = :TimeTypeID "
                        + " and SalartID = :SalartID ";
            if (salartTypeCategory == 1)
                  sSql += " and anvID = :anvID "
                        + " and rep_kat_id = :rep_kat_id ";
            if (sht.ID != 0)
                sSql += " and ID <> :ID ";

            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("vart_ordernr", sht.VartOrdernr);            
            pc.Add("datum", sht.Datum);
            pc.Add("TimeTypeID", sht.TimeTypeID);
            pc.Add("SalartID", sht.SalartID);
            if (salartTypeCategory == 1)
            {
                pc.Add("anvID", sht.AnvId);
                pc.Add("rep_kat_id", sht.RepKatID);
            }
            if (sht.ID != 0)
                pc.Add("ID", sht.ID);           
            string er = "";

            DataTable dt = cdb.getData(sSql, ref er, pc);
            return Convert.ToInt32(dt.Rows[0]["antal"]);
        }


        private int validateAttested(ServHuvRepTidCL srt)
        {
            if (srt.ID == 0)
                return 0;
            string sSql = "SELECT attesterad "
                        + "FROM ServHuvRepTid "
                        + "where ID = :ID ";
            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("ID", srt.ID);

            string er = "";
            DataTable dt = cdb.getData(sSql, ref er, pc);

            if (dt.Rows.Count == 0)
                return 0;

            Boolean bAttest = Convert.ToBoolean(dt.Rows[0]["attesterad"]);

            if (bAttest)
                return 1;
            return 0;
        }

        



        private int validateServHuvRepTid(ServHuvRepTidCL sht, string ident, ref string err, int SalartCatID)
        {
            err = "";
            if (SalartCatID == 1)
            {
                if (validateAnvID(sht.AnvId) == 0)
                    return -2;
            }
            if (validateDatum(sht) == 0)
                return -3;
            if (validateTid(sht.Tid, sht.SalartID, ref err) == 0)
                return -4;
            if (validateDuplicate(sht, SalartCatID) > 0)
                return -5;
            if (validateTimeType(sht.TimeTypeID) == 0)
                return -6;
            if (validateAttested(sht) == 1)
                return -7;
            return 1;
        }


        private string getShInsertSQL()
        {
            string sSql = " insert into ServHuvRepTid ( vart_ordernr, timeTypeID, salartID, anvID, rep_kat_id, tid, datum, regdat, attesterad) "
                        + " values ( :vart_ordernr, :timeTypeID, :salartID, :anvID, :rep_kat_id, :tid, :datum, :regdat, false) ";
            return sSql;
        }

        private string getShDeleteSQL()
        {
            string sSql = " delete from ServHuvRepTid "
                        + " where ID = :ID ";
            return sSql;
        }


        private string getShUpdateSQL()
        {
            string sSql = " update ServHuvRepTid "
                        + " set anvID = :anvID "
                        + ", datum = :datum "
                        + ", vart_ordernr = :vart_ordernr "
                        + ", tid = :tid "
                        + ", uppdat_dat = :uppdat_dat "
                        + ", TimeTypeID = :TimeTypeID "
                        + ", SalartID = :SalartID "
                        + ", rep_kat_id = :rep_kat_id "
                        + " where ID = :ID ";
            return sSql;

        }

        private int getShLastInserted()
        {
            string sSql = " SELECT coalesce(max(ID),0) as MaxID "
                        + " FROM ServHuvRepTid ";                        

            string er = "";
            DataTable dt = cdb.getData(sSql, ref er);
            return Convert.ToInt32(dt.Rows[0]["MaxID"]);
        }



        private void setParameters(NxParameterCollection np, ServHuvRepTidCL sr, int salartCatID)
        {
            if (sr.ID != 0)
            {
                np.Add("ID", sr.ID);
                np.Add("uppdat_dat", System.DateTime.Now);
            }
            else
                np.Add("regdat", System.DateTime.Now);                            
            np.Add("datum", sr.Datum);
            np.Add("vart_ordernr", sr.VartOrdernr);
            np.Add("tid", sr.Tid);            
            np.Add("TimeTypeID", sr.TimeTypeID);
            np.Add("SalartID", sr.SalartID);
            if (salartCatID == 1)
            {
                np.Add("anvID", sr.AnvId);    
                np.Add("rep_kat_id", sr.RepKatID);
            }
            else
            {
                np.Add("anvID", System.DBNull.Value);
                np.Add("rep_kat_id", System.DBNull.Value);
            }                           
        }





        
        /// <summary>
        /// Validates one ServHuvRepTid
        /// If the ID is 0 the this method
        /// assumes that this is a new row
        /// Returns the validated and stored
        /// row with the new ID (if its a new row)
        /// If an error occurs then an error is returned
        /// in the ServHuvTidRep return row
        /// </summary>
        /// <param name="ident">Identity</param>
        /// <param name="sht">ServHuvRepTid</param>
        /// <returns></returns>
        public ServHuvRepTidCL saveServHuvRepTid(string ident, ServHuvRepTidCL sht)
        {
            // Init variables
            bool bNew = false;            
            bool bDeleted = false;

            CReparator cr = new CReparator();
            int identOK = cr.checkIdent(ident);

            ServHuvRepTidCL retSrt = new ServHuvRepTidCL();
            if (identOK == -1)
            {
                retSrt.ErrCode = -10;
                retSrt.ErrMessage = "Ogiltigt login";
                return retSrt;
            }


            if (sht.VartOrdernr == "")
            {
                retSrt.ErrCode = -1;
                retSrt.ErrMessage = "Ordernummer saknas";
                return retSrt;
            }
            

            CServiceHuvud ch = new CServiceHuvud();
            string sOpen = ch.isOpen(ident, sht.VartOrdernr);
            if (sOpen != "1")
            {
                retSrt.ErrCode = -10;
                if (sOpen == "-1")
                    retSrt.ErrMessage = "Order är stängd för inmatning";
                else
                    retSrt.ErrMessage = sOpen;
                return retSrt;
            }


            if (isTime2Approved(sht.VartOrdernr))
            {
                retSrt.ErrCode = -10;
                retSrt.ErrMessage = "Aktuell tidredovisning är stängd och kan inte ändras ";
                return retSrt;
            }

            if (sht.SalartID == 0)
            {
                retSrt.ErrCode = -1;
                retSrt.ErrMessage = "Löneart saknas";
                return retSrt;
            }


            int salartCatID = 0;
            if (validateSalart(sht.SalartID, false, ref salartCatID) == -1)
            {
                retSrt.ErrCode = -1;
                retSrt.ErrMessage = "Felaktig löneart";
                return retSrt;
            }



            string err = "";
            int valid = validateServHuvRepTid(sht, ident, ref err, salartCatID);


            if (valid == -2)
            {
                retSrt.ErrCode = -1;
                retSrt.ErrMessage = "Felaktigt användarID";
                return retSrt;
            }

            if (valid == -3)
            {
                retSrt.ErrCode = -1;
                retSrt.ErrMessage = "Felaktigt datum";
                return retSrt;
            }

            if (valid == -4)
            {
                retSrt.ErrCode = -1;
                retSrt.ErrMessage = err;
                return retSrt;
            }

            // 2016-06-17 added clause
            if (valid == -5)
            {
                retSrt.ErrCode = -1;
                if (salartCatID == 1)
                    retSrt.ErrMessage = "Det finns redan tid redovisat för aktuell reparatör, dag och löneart ";
                else
                    retSrt.ErrMessage = "Det finns redan tid redovisat för aktuell dag och löneart ";
                return retSrt;
            }

            // 2016-11-01 KJBO
            if (valid == -6)
            {
                retSrt.ErrCode = -1;
                retSrt.ErrMessage = "Felaktigt TimeTypeID";
                return retSrt;
            }

            if (valid == -7)
            {
                retSrt.ErrCode = -1;
                retSrt.ErrMessage = "Tidregistreringen är attesterad";
                return retSrt;

            }



            if (salartCatID == 1)
            {
                if (sht.RepKatID == "")
                {
                    retSrt.ErrCode = -1;
                    retSrt.ErrMessage = "Reparatörskategori saknas";
                    return retSrt;
                }

                if (validateRepKat(sht.RepKatID) == -1)
                {
                    retSrt.ErrCode = -1;
                    retSrt.ErrMessage = "Felaktig reparatörskategori";
                    return retSrt;
                }
            }

            

            string sSql = "";

            // Nothing to save... 2016-06-17
            if (sht.ID == 0 && sht.Tid == 0)
                return sht;


            // This is a new ventil
            if (sht.ID == 0)
            {
                sSql = getShInsertSQL();
                bNew = true;
            }
            else if (sht.Tid == 0)
            {
                sSql = getShDeleteSQL();
                bDeleted = true;
            }
            else
                sSql = getShUpdateSQL();

            NxParameterCollection np = new NxParameterCollection();
            setParameters(np, sht, salartCatID);

            string errText = "";

            int iRc = cdb.updateData(sSql, ref errText, np);

            if (errText != "")
            {
                if (errText.Length > 2000)
                    errText = errText.Substring(1, 2000);
                retSrt.ErrCode = -100;
                retSrt.ErrMessage = errText;
                return retSrt;
            }

            if (bNew)
            {
                sht.ID = getShLastInserted();
                CServRad crs = new CServRad();
                // 2016-04-04 KJBO
                crs.ensureReparatorExists(ident, "", "", sht.VartOrdernr);
            }
            if (bDeleted)
                return sht;
            return getServHuvRepTid(ident, sht.ID);
        }



        /// <summary>
        /// Get one row of ServHuvRepTid identified by PK
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        /// 2017-03-15 KJBO
        public ServHuvRepTidCL getServHuvRepTid(string ident, int ID)
        {
            List<ServHuvRepTidCL> srrList = getServHuvRepTidForOrder(ident, "", ID, "");
            return srrList[0];
        }




        /// <summary>
        /// Get all ServHuvRepTid for one order
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="vartOrdernr"></param>
        /// <returns></returns>
        /// 2017-03-15 KJBO
        public List<ServHuvRepTidCL> getServHuvRepTidForSH(string ident, string vartOrdernr)
        {
            return getServHuvRepTidForOrder(ident, vartOrdernr, 0, "");
        }


        /// <summary>
        /// Get all ServHuvRepTid for one order and 
        /// one user ( = anv)
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="vartOrdernr"></param>
        /// <param name="anvID"></param>
        /// <returns></returns>
        /// 2017-03-15 KJBO
        public List<ServHuvRepTidCL> getServHuvRepTidForShAnv(string ident, string vartOrdernr, string anvID)
        {
            return getServHuvRepTidForOrder(ident, vartOrdernr, 0, anvID);
        }




        /// <summary>
        /// Lists ServRadRepTid rows depending on parameters
        /// Note that this is a private function used by several
        /// other public functions
        /// 
        /// If an ID is given (and is not 0) this function returns only zero or one row
        /// because this is the primary key
        /// 
        /// If ID is 0 and vartOrdernr is not an empty string then this function returns 
        /// a list of all ServRadRepTid rows for the current order.
        /// 
        /// If ID is 0 and both vartOrdernr and AnvId is provided this function returns
        /// a list of all ServRadRepTid rows for the current order and user (anvID)
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="vartOrdernr"></param>
        /// <param name="ID"></param>
        /// <param name="AnvID"></param>
        /// <returns></returns>
        private List<ServHuvRepTidCL> getServHuvRepTidForOrder(string ident, string vartOrdernr, int ID, string AnvID)
        {
            // Object to use when return from function
            List<ServHuvRepTidCL> shrRows = new List<ServHuvRepTidCL>();

            // Check ident
            CReparator cr = new CReparator();
            int identOK = cr.checkIdent(ident);

            if (identOK == -1)
            {
                ServHuvRepTidCL srr = new ServHuvRepTidCL();
                srr.ErrCode = -10;
                srr.ErrMessage = "Ogiltigt login";
                shrRows.Add(srr);
                return shrRows;
            }

            // Create an empty parameter collection
            NxParameterCollection pc = new NxParameterCollection();
            string sSql = " select ID, vart_ordernr, anvID, tid, datum, TimeTypeID, SalartID, rep_kat_id "
                        + " from servHuvRepTid ";
            // Now construct the SQL clause and add parameters...
            if (ID != 0)
            {
                sSql += " where ID = :ID ";
                pc.Add("ID", ID);
            }
            else
            {
                if (vartOrdernr != "")
                {
                    sSql += " where vart_ordernr = :vart_ordernr ";
                    pc.Add("vart_ordernr", vartOrdernr);
                }
                if (AnvID != "")
                {
                    sSql += " and anvID = :anvID ";
                    pc.Add("anvID", AnvID);
                }

            }

            string errText = "";

            // Get data
            DataTable dt = cdb.getData(sSql, ref errText, pc);

            int errCode = -100;

            // Check if any result
            if (errText == "" && dt.Rows.Count == 0)
            {
                if (ID != 0)
                    errText = "Felaktigt ID";
                else
                    errText = "Inga tidsinmatning registrerade för aktuell serviceorder/användare";
                errCode = 0;
            }


            // Check for database errors
            if (errText != "")
            {
                ServHuvRepTidCL srr = new ServHuvRepTidCL();
                if (errText.Length > 2000)
                    errText = errText.Substring(1, 2000);
                srr.ErrCode = errCode;
                srr.ErrMessage = errText;
                shrRows.Add(srr);
                return shrRows;
            }

            // Build result list
            foreach (DataRow dr in dt.Rows)
            {
                ServHuvRepTidCL shr = new ServHuvRepTidCL();
                shr.ID = Convert.ToInt32(dr["ID"]);
                shr.VartOrdernr = dr["vart_ordernr"].ToString();
                if (dr["anvID"] != DBNull.Value)
                    shr.AnvId = dr["anvID"].ToString();
                shr.Datum = Convert.ToDateTime(dr["datum"]);
                shr.Tid = Convert.ToDecimal(dr["tid"]);
                shr.TimeTypeID = Convert.ToInt32(dr["TimeTypeID"]);
                shr.SalartID = Convert.ToInt32(dr["SalartID"]);
                if (dr["rep_kat_id"] != DBNull.Value)
                    shr.RepKatID = dr["rep_kat_id"].ToString();
                shr.ErrCode = 0;
                shr.ErrMessage = "";
                shrRows.Add(shr);
            }
            // Return result
            return shrRows;
        }

        private string getTimeReg2ReportInsertSQL()
        {
            string sSql = "insert into TimeRep2Process (vart_ordernr, email, reportType, ordered, reportStatus, errCode, errMess, linkURL, linkAdded, emailCreated, dropBoxFilename, dropBoxArchiveName, dropBoxArchiveCreated, approve) "
                        + " values (:vart_ordernr, :email, :reportType, :ordered, 0, 0,'' ,:linkURL ,:linkAdded ,:emailCreated, :dropBoxFilename, :dropBoxArchiveName, :dropBoxArchiveCreated, :approve ) ";
            return sSql;
        }

        private string getTimeReg2ReportUpdateSQL()
        {
            string sSql = " update TimeRep2Process "
                        + " set email = :email "
                        + ", ordered = :ordered "
                        + ", reportStatus = 0 "
                        + ", errCode = 0 "
                        + ", errMess = ''"
                        + ", linkURL = :linkURL "
                        + ", linkAdded = :linkAdded "
                        + ", emailCreated = :emailCreated "
                        + ", reportType = :reportType "
                        + ", dropBoxFilename = :dropBoxFilename "
                        + ", dropBoxArchiveName = :dropBoxArchiveName "
                        + ", dropBoxArchiveCreated = :dropBoxArchiveCreated "
                        + ", approve = :approve "
                        + " where vart_ordernr = :vart_ordernr ";
            return sSql;
        }










        /// <summary>
        /// Initiate creation of timeRep2Report
        /// parameter p must have at least a VartOrdernr and an email (for the returning mail)
        /// The return value is a filled instance of TimeRep2ProcessCL with init values.
        /// To check the status of the report generation, call getTimeRep2ReportStatus(string VartOrdernr)
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="p"></param>
        /// <param name="bOverrideExisting"></param>
        /// <returns>A filled TimeRep2ProcessCL</returns>
        /// 2017-03-21  KJBO
        /// 2017-09-10 KJBO Added detailed parameter
        public TimeRep2ProcessCL generateTimeReg2Report(string ident, TimeRep2ProcessCL p, bool bOverrideExisting, bool approve)        
        {
            
            CReparator cr = new CReparator();           

            int identOK = cr.checkIdent(ident);

            if (identOK == -1)
            {
                p.ErrCode = -10;
                p.ErrMessage = "Ogiltigt login";
                return p;
            }
                
            // Check how many rows matiching the current ordernr
            string sSql = "SELECT count(*) count_rows "
                        + " FROM TimeRep2Process "
                        + " where vart_ordernr = :vart_ordernr ";

            // Create an empty parameter collection
            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("vart_ordernr", p.VartOrdernr);

            string errText = "";

            // Get data
            DataTable dt = cdb.getData(sSql, ref errText, pc);
            

            // Check if any result
            if (errText == "" && dt.Rows.Count == 0)
            {
                p.ErrCode = 0;
                p.ErrMessage = "Kan inte räkna rader i TimeRep2Process tabellen";
                return p;
            }
            int errCode = -100;



            // Check for database errors
            if (errText != "")
            {
                if (errText.Length > 2000)
                    errText = errText.Substring(1, 2000);
                p.ErrCode = errCode;
                p.ErrMessage = errText;
                return p;
            }

            // Init variable
            int antal = 0;

            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                antal = Convert.ToInt32(dr["count_rows"]);
            }

            // If row exists and allow overwrite is disallowed then repor error
            if (antal == 1 && !bOverrideExisting)
            {
                p.ErrCode = 100;
                p.ErrMessage = "Det finns redan en rapportbeställning för denna order";
                return p;
            }

            // 2017-07-03
            if (approve)
            {
                int rc = attestAllTime2(p.VartOrdernr, false);
                errText = "";
                switch (rc)
                {
                    case -1: errText = "Fel vid attestering av ServHuvRepTid";
                        break;
                    case -2: errText = "Fel vid kontroll av ServRadRepTid";
                        break;
                    case -3: errText = "Fel vid uppdatering av ServHuvRepTid";
                        break;
                }

                if (errText != "")
                {
                    p.ErrCode = -101;
                    p.ErrMessage = errText;
                    return p;
                }

                if (approveTimeRep2(p.VartOrdernr, ref errText) != 0)
                {
                    p.ErrCode = -101;
                    p.ErrMessage = errText;
                    return p;
                }
            }
            else
                attestAllTime2(p.VartOrdernr, true);

            if (antal == 0)
                sSql = getTimeReg2ReportInsertSQL();
            else
                sSql = getTimeReg2ReportUpdateSQL();



            DateTime dtNow = System.DateTime.Now;
            pc = new NxParameterCollection();
            pc.Add("vart_ordernr", p.VartOrdernr);
            pc.Add("email", p.Email);
            pc.Add("reportType", p.ReportType);
            pc.Add("ordered", dtNow);
            pc.Add("linkURL", DBNull.Value);
            pc.Add("linkAdded", DBNull.Value);
            pc.Add("emailCreated", DBNull.Value);
            pc.Add("dropBoxFilename", DBNull.Value);
            pc.Add("dropBoxArchiveName", DBNull.Value);
            pc.Add("dropBoxArchiveCreated", DBNull.Value);
            pc.Add("approve", approve);



            errText = "";

            cdb.updateData(sSql, ref errText, pc);

            if (errText != "")
            {

                if (errText.Length > 2000)
                    errText = errText.Substring(1, 2000);
                p.ErrCode = -100;
                p.ErrMessage = errText;
                return p;                
            }

            //int li_rc = createTimeRep2();   

            return getTimeRep2ReportStatus(p.VartOrdernr);

        }


        /// <summary>
        /// Get status of timeRep2Report
        /// </summary>
        /// <param name="VartOrdernr"></param>
        /// <returns></returns>
        private TimeRep2ProcessCL getTimeRep2ReportStatus(string VartOrdernr)
        {
            TimeRep2ProcessCL p = new TimeRep2ProcessCL();
            string sSql = "select vart_ordernr, email, reportType, ordered, reportStatus, errCode, errMess, linkURL, linkAdded, emailCreated "
                        + " from TimeRep2Process "
                        + " where vart_ordernr = :vart_ordernr ";

            // Create an empty parameter collection
            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("vart_ordernr", VartOrdernr);

            string errText = "";

            // Get data
            DataTable dt = cdb.getData(sSql, ref errText, pc);


            // Check if any result
            if (errText == "" && dt.Rows.Count == 0)
            {
                p.ErrCode = 0;
                p.ErrMessage = "Det finns ingen beställning på tidrapport för denna order";
                return p;
            }

            int errCode = -100;

            // Check for database errors
            if (errText != "")
            {
                if (errText.Length > 2000)
                    errText = errText.Substring(1, 2000);
                p.ErrCode = errCode;
                p.ErrMessage = errText;
                return p;
            }

            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                p.VartOrdernr = dr["vart_ordernr"].ToString();
                p.Email = dr["email"].ToString();
                p.ReportType = Convert.ToInt32(dr["reportType"]);
                p.Ordered = Convert.ToDateTime(dr["ordered"]);
                p.ReportStatus = Convert.ToInt32(dr["reportStatus"]);
                p.ErrCode = Convert.ToInt32(dr["errCode"]);
                p.ErrMessage = dr["errMess"].ToString();
                p.LinkURL = dr["linkURL"].ToString();
                if (dr["linkAdded"] != DBNull.Value)
                    p.LinkAdded = Convert.ToDateTime(dr["linkAdded"]);
                if (dr["emailCreated"] != DBNull.Value)
                    p.EmailCreated = Convert.ToDateTime(dr["emailCreated"]);
                return p;
            }

            return null;

        }


        /// <summary>
        /// After calling generateTimeReg2Report() there is a possibility
        /// to check the status of the report generation process.
        /// Call this function and you get a status report back
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="VartOrdernr"></param>
        /// <returns>Instance of TimeRep2ProcessCL</returns>
        /// 2017-03-21 KJBO
        public TimeRep2ProcessCL getTimeRep2ReportStatus(string ident, string VartOrdernr)
        {
            TimeRep2ProcessCL p = new TimeRep2ProcessCL();
            CReparator cr = new CReparator();

            int identOK = cr.checkIdent(ident);

            if (identOK == -1)
            {
                p.ErrCode = -10;
                p.ErrMessage = "Ogiltigt login";
                return p;
            }   
                 
            return getTimeRep2ReportStatus(VartOrdernr);

        }



        /// <summary>
        /// Attest all unattested time registry
        /// </summary>
        /// <param name="VartOrdernr"></param>
        /// <returns></returns>
        private int attestAllTime2( string VartOrdernr, bool unattest)
        {
            string sSql = "";
            if (unattest)
            {
                sSql = "update servHuvRepTid "
                        + "set attesterad = false "
                        + ", attestDat = null "
                        + "where vart_ordernr = :vart_ordernr "
                        + "and attesterad = true ";
            }
            else
            {
                sSql = "update servHuvRepTid "
                            + "set attesterad = true "
                            + ", attestDat = :attestDat "
                            + "where vart_ordernr = :vart_ordernr "
                            + "and attesterad = false ";
            }

            DateTime ldtNow = DateTime.Now;
            NxParameterCollection np = new NxParameterCollection();
            np.Add("vart_ordernr", VartOrdernr);
            np.Add("attestDat", ldtNow);
 

            string errText = "";

            int iRc = cdb.updateData(sSql, ref errText, np);

            if (errText != "")            
                return -1;

            // Get a list of all alternate keys (rows) belonging to
            // this order
            sSql = "select alternateKey "
                + " from servicerad "
                + " where vart_ordernr = :vart_ordernr ";
            
            errText = "";

            DataTable dt = cdb.getData(sSql, ref errText, np);
            

            if (errText != "")            
                return -2;
            string sSqlUpdate = "";

            if (unattest)
            {
                sSqlUpdate = " update servRadRepTid "
                                + " set attesterad = false "
                                + ", attestDat = null "
                                + " where srAltKey = :srAltKey "
                                + " and attesterad = true ";
            }
            else
            {
                sSqlUpdate = " update servRadRepTid "
                                + " set attesterad = true "
                                + ", attestDat = :attestDat "
                                + " where srAltKey = :srAltKey "
                                + " and attesterad = false ";
            }
            np.Add("srAltKey", DbType.String);

            // Loop through and attest all
            foreach (DataRow dr in dt.Rows)
            {
                string alternateKey = dr["alternateKey"].ToString();
                np["srAltKey"].Value = alternateKey;
                iRc = cdb.updateData(sSqlUpdate, ref errText, np);

                if (errText != "")
                    return -3;
            }




            return 1;
        }

        private int approveTimeRep2(string vartOrdernr, ref string errText)
        {
            string sSql = "update timeReport2 "
                        + " set approved = true "
                        + " where vart_ordernr = :vartOrdernr ";

            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("vartOrdernr", vartOrdernr);

            errText = "";
            cdb.updateData(sSql, ref errText, pc);

            int liRc = 0;
            if (errText != "")
            {
                liRc = -1;                
            }


            return liRc;

        }


    }


    
}