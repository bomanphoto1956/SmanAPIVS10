using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using NexusDB.ADOProvider;

namespace SManApi
{
    public class CTidRed
    {
        CDB cdb = null;
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
            return getOpenDates(ident, SrAltKey, true);
        }



        /// <summary>
        /// Returns a list of valid dates for
        /// registry of time for one ServiceRow
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="SrAltKey">Alternate key</param>
        /// <returns>List of dates or an error message</returns>
        private List<OpenDateCL> getOpenDates(string ident, string SrAltKey, bool bValidate)
        {
            
            List<OpenDateCL> dateList = new List<OpenDateCL>();

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

            CServRad crv = new CServRad();
            DataTable dt = crv.validateServRad(SrAltKey);

            if (dt.Rows.Count == 0)
            {
                OpenDateCL d = new OpenDateCL();
                d.ErrCode = -10;
                d.ErrMessage = "Ogiltig ServiceRad";
                dateList.Add(d);
                return dateList;
            }

            string vart_ordernr = dt.Rows[0]["vart_ordernr"].ToString();
            int radnr = Convert.ToInt32(dt.Rows[0]["radnr"]);

            if (bValidate)
            {

                CServiceHuvud ch = new CServiceHuvud();

                dt = ch.validateOrderOpenGodkand(vart_ordernr);

                bool bOpenForApp = Convert.ToBoolean(dt.Rows[0]["OpenForApp"]);
                bool bGodkand = Convert.ToBoolean(dt.Rows[0]["Godkand"]);

                if (!bOpenForApp)
                {
                    OpenDateCL d = new OpenDateCL();
                    d.ErrCode = -10;
                    d.ErrMessage = "Aktuell order är stängd för inmatning";
                    dateList.Add(d);
                    return dateList;
                }


                if (bGodkand)
                {
                    OpenDateCL d = new OpenDateCL();
                    d.ErrCode = -10;
                    d.ErrMessage = "Aktuell order är godkänd. Ändring ej tillåten ";
                    dateList.Add(d);
                    return dateList;
                }
            }
            

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
            dt = cdb.getData(sSql, ref err, pc);

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
            string sSql = " SELECT srrt.ID, srrt.srAltKey, srrt.anvID, srrt.tid, srrt.datum, srrt.TimeTypeID "
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
                srr.ErrCode = 0;
                srr.ErrMessage = "";
                srrRows.Add(srr);
            }
            return srrRows;
        }



        private string getInsertSQL()
        {
            string sSql = " insert into ServradRepTid (  anvID, datum, regdat, srAltKey "
                         + " , tid, attesterad, TimeTypeID)  "
                        + "  values ( :anvID, :datum, :regdat, :srAltKey "
                        + " , :tid, false, :TimeTypeID )";

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

        private int validateDatum( ServRadRepTidCL srt, string ident)
        {
            List<OpenDateCL> dateList = getOpenDates(ident, srt.SrAltKey, false);

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


        private int validateDuplicate(ServRadRepTidCL srt)
        {
            string sSql = " SELECT count(*) as antal "
                        + " from ServRadRepTid "
                        + " where srAltKey = :srAltKey "
                        + " and anvID = :anvID "
                        + " and datum = :datum "
                        + " and TimeTypeID = :TimeTypeID ";
            if (srt.ID != 0)
                sSql += " and ID <> :ID ";

            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("srAltKey", srt.SrAltKey);
            pc.Add("anvID", srt.AnvID);
            pc.Add("datum", srt.Datum);
            pc.Add("TimeTypeID", srt.timeTypeID);
            if (srt.ID != 0)
                pc.Add("ID", srt.ID);

            string er = "";

            DataTable dt = cdb.getData(sSql, ref er, pc);
            return Convert.ToInt32(dt.Rows[0]["antal"]);
        }

        private int validateTid( Decimal tid, ref string err)
        {
            if (tid < 0)
            {
                err = "Registrerad tid får inte vara negativ";
                return 0;
            }
            if (tid > 24)
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


        private int validateServRadRepTid(ServRadRepTidCL srt, string ident, ref string err)
        {
            err = "";
            if (validateAlternateKey(srt.SrAltKey) == 0)            
                return -1;            
            if (validateAnvID(srt.AnvID) == 0 )
                return -2;
            if (validateDatum(srt, ident) == 0)
                return -3;
            if (validateTid(srt.Tid, ref err) == 0)
                return -4;
            if (validateDuplicate(srt) > 0)
                return -5;
            if (validateTimeType(srt.timeTypeID) == 0)
                return -6;
            return 1;

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
        //  2016-02-15 KJBO Pergas AB
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



            string err = "";
            int valid = validateServRadRepTid(srt, ident, ref err);

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
                retSrt.ErrMessage = "Det finns redan tid redovisat för aktuell dag och ventil";
                return retSrt;
            }

            // 2016-11-01 KJBO
            if (valid == -6)
            {
                retSrt.ErrCode = -1;
                retSrt.ErrMessage = "Felaktigt TimeTypeID";
                return retSrt;
            }


            CServRad crs = new CServRad();

            DataTable dtsr = crs.validateServRad(srt.SrAltKey);

            string sVartOrdernr = dtsr.Rows[0]["vart_ordernr"].ToString();

            CServiceHuvud ch = new CServiceHuvud();
            string sOpen = ch.isOpen(ident, sVartOrdernr);
            if (sOpen != "1")
            {
                {
                    retSrt.ErrCode = -10;
                    if (sOpen == "-1")
                        retSrt.ErrMessage = "Order är stängd för inmatning";
                    else
                        retSrt.ErrMessage = sOpen;
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

    }


    
}