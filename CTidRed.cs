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
            
            List<OpenDateCL> dateList = new List<OpenDateCL>();

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
            // First validate the srAltKey exists

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
            List<ServRadRepTidCL> srrList = getServRadRepTidForServiceRad(ident, "", ID);
            return srrList[0];
        }


        /// <summary>
        /// Returns all registered time (all rows)
        /// for a specific service row (identified by srAltKey)
        /// and a specific user (identifiec by ident)
        /// </summary>
        /// <param name="ident">Identity</param>
        /// <param name="srAltKey">AlternateKey for servicerad</param>
        /// <returns>List of registered time or one row with error message</returns>
        // 2016-02-15 Pergas AB KJBO
        public List<ServRadRepTidCL> getServRadRepTidForServiceRad(string ident, string srAltKey)
        {
            return getServRadRepTidForServiceRad(ident, srAltKey, 0);
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
        private List<ServRadRepTidCL> getServRadRepTidForServiceRad(string ident, string srAltKey, int ID)
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

            string sSql = " SELECT srrt.ID, srrt.srAltKey, srrt.anvID, srrt.tid, srrt.datum "
                        + " FROM ServradRepTid srrt "
                        + " join Authenticate a on srrt.anvID = a.anvID ";
            if (ID != 0)
                sSql += " where srrt.ID = :ID ";
            else
                sSql += " where srrt.srAltKey = :srAltKey "
                     + " and a.Ident = :Ident ";


            NxParameterCollection pc = new NxParameterCollection();
            if (ID != 0)
                pc.Add("ID", ID);
            else
            {
                pc.Add("srAltKey", srAltKey);
                pc.Add("Ident", ident);
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
                srr.ErrCode = 0;
                srr.ErrMessage = "";
                srrRows.Add(srr);
            }
            return srrRows;
        }



        private string getInsertSQL()
        {
            string sSql = " insert into ServradRepTid (  anvID, datum, regdat, srAltKey "
                         + " , tid)  "
                        + "  values ( :anvID, :datum, :regdat, :srAltKey "
                        + " , :tid )";

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
            List<OpenDateCL> dateList = getOpenDates(ident, srt.SrAltKey);

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
                        + " and datum = :datum ";
            if (srt.ID != 0)
                sSql += " and ID <> :ID ";

            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("srAltKey", srt.SrAltKey);
            pc.Add("anvID", srt.AnvID);
            pc.Add("datum", srt.Datum);
            if (srt.ID != 0)
                pc.Add("ID", srt.ID);

            string er = "";

            DataTable dt = cdb.getData(sSql, ref er, pc);
            return Convert.ToInt32(dt.Rows[0]["antal"]);
        }

        private int validateTid( Decimal tid, ref string err)
        {
            if (tid <= 0)
            {
                err = "Registrerad tid måste vara större än 0";
                return 0;
            }
            if (tid > 24)
            {
                err = "Registrerad tid får inte överstiga 24 timmar";
                return 0;
            }
            return 1;
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
        /// If an error occurs then an error is returne
        /// in the ServRadTidRep return row
        /// </summary>
        /// <param name="ident">Identity</param>
        /// <param name="srt">ServRadTidRepCL</param>
        /// <returns>The saved row or an error</returns>
        //  2016-02-15 KJBO Pergas AB
        public ServRadRepTidCL saveServRadRepTid(string ident, ServRadRepTidCL srt)
        {
            bool bNew = false;
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

            if (valid == -5)
            {
                retSrt.ErrCode = -1;
                retSrt.ErrMessage = "Det finns redan tid redovisat för aktuell dag och ventil";
                return retSrt;
            }

            string sSql = "";

            // This is a new ventil
            if (srt.ID == 0)
            {
                sSql = getInsertSQL();
                bNew = true;
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
                srt.ID = getLastInserted(srt.AnvID);

            return getServRadRepTid(ident, srt.ID);

        }
        

    }
}