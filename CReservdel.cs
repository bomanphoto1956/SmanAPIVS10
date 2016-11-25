using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using NexusDB.ADOProvider;

namespace SManApi
{
    

    /// <summary>
    /// Reservdel and artikel
    /// </summary>
    public class CReservdel
    {

        // Database class
        CDB cdb = null;

        public CReservdel()
        {
            cdb = new CDB();
        }




        /// <summary>
        /// Get a list of artikel for display purposes
        /// </summary>
        /// <param name="ident">Identity</param>
        /// <param name="ArtnrFilter">Artnr or part of..</param>
        /// <param name="ArtnamnFilter">ArtNamn or part of..</param>
        /// <returns></returns>
        // 2016-02-09 KJBO  
        public List<ArtikelCL> getArtList(string ident, string ArtnrFilter, string ArtnamnFilter)
        {
            // Create article list
            List<ArtikelCL> artlist = new List<ArtikelCL>();

            // Get reparator
            CReparator cr = new CReparator();
            int identOK = cr.checkIdent(ident);

            if (identOK == -1)
            {
                ArtikelCL art = new ArtikelCL();
                art.ErrCode = -10;
                art.ErrMessage = "Ogiltigt login";
                artlist.Add(art);
                return artlist;
            }

            // SQL string
            string sSql = " SELECT a.artnr, a.artnamn, a.lev_id, l.levnamn, a.anm1, a.anm2 "
                        + " FROM artikel a "
                        + " left outer join leverantor l on a.lev_id = l.lev_id "
                        + " where upper(a.artnr) like upper(:artnr) "
                        + " and upper(a.artnamn) like upper(:artnamn) "
                        + " and a.visas = true ";

            // Add parameter list
            NxParameterCollection np = new NxParameterCollection();
            np.Add("artnr", CCommonFunc.addWildCard(ArtnrFilter));
            np.Add("artnamn", CCommonFunc.addWildCard(ArtnamnFilter));

            // Init variable
            string errText = "";

            DataTable dt = cdb.getData(sSql, ref errText, np);

            // Init varible
            int errCode = -100;

            // No rows found.....
            if (errText == "" && dt.Rows.Count == 0)
            {
                errText = "Det finns inga artiklar i aktuellt urval ";
                errCode = 0;
            }

            // No rows found or error when retrieving
            if (errText != "")
            {
                ArtikelCL a = new ArtikelCL();
                if (errText.Length > 2000)
                    errText = errText.Substring(1, 2000);
                a.ErrCode = errCode;
                a.ErrMessage = errText;
                artlist.Add(a);
                return artlist;
            }

            // Loop rows.....
            foreach (DataRow dr in dt.Rows)
            {
                ArtikelCL a = new ArtikelCL();
                a.Artnr = dr["artnr"].ToString();
                a.Artnamn = dr["artnamn"].ToString();
                a.LevID = dr["lev_id"].ToString();
                a.LevNamn = dr["levnamn"].ToString();
                a.Anm1 = dr["anm1"].ToString();
                a.Anm2 = dr["anm2"].ToString();
                artlist.Add(a);
            }

            // .. and return list
            return artlist;
        }



        /// <summary>
        /// Return one artikel
        /// </summary>
        /// <param name="ident">Ident</param>
        /// <param name="Artnr">Artnr</param>
        /// <returns></returns>
        // 2016-02-10 KJBO
        public ArtikelCL getArtikel(string ident, string Artnr)
        {
            ArtikelCL art = new ArtikelCL();

            CReparator cr = new CReparator();
            int identOK = cr.checkIdent(ident);

            if (identOK == -1)
            {                
                art.ErrCode = -10;
                art.ErrMessage = "Ogiltigt login";
                return art;
            }

            string sSql = " SELECT a.artnr, a.artnamn, a.lev_id, l.levnamn, a.anm1, a.anm2 "
                        + " FROM artikel a "
                        + " left outer join leverantor l on a.lev_id = l.lev_id "
                        + " where a.artnr = :artnr ";

            NxParameterCollection np = new NxParameterCollection();
            np.Add("artnr", Artnr);            

            string errText = "";

            DataTable dt = cdb.getData(sSql, ref errText, np);

            int errCode = -100;

            if (errText == "" && dt.Rows.Count == 0)
            {
                errText = "Artikeln finns ej ";
                errCode = 0;
            }

            if (errText != "")
            {                
                if (errText.Length > 2000)
                    errText = errText.Substring(1, 2000);
                art.ErrCode = errCode;
                art.ErrMessage = errText;                        
            }

            if (dt.Rows.Count == 1)
            {
                DataRow dr = dt.Rows[0];
                art.Artnr = dr["artnr"].ToString();
                art.Artnamn = dr["artnamn"].ToString();
                art.LevID = dr["lev_id"].ToString();
                art.LevNamn = dr["levnamn"].ToString();
                art.Anm1 = dr["anm1"].ToString();
                art.Anm2 = dr["anm2"].ToString();

            }


            return art;
        }



        /// <summary>
        /// Return a list of reservdel for one servicerad
        /// </summary>
        /// <param name="ident">Ident</param>
        /// <param name="VartOrdernr">VartOrdernr</param>
        /// <param name="RadNr">Radnr</param>
        /// <returns>List of reservdel or one row with error</returns>
        // 2016-02-10 KJBO
        public List<ReservdelCL> getReservdelsForServiceRad(string ident, string VartOrdernr, int RadNr)
        {
            return getReservdelGeneral(ident, VartOrdernr, RadNr, -1);
        }



        /// <summary>
        /// Get one reservdel identified by primary key
        /// </summary>
        /// <param name="ident">identity</param>
        /// <param name="VartOrdernr"></param>
        /// <param name="RadNr"></param>
        /// <param name="ReservNr"></param>
        /// <returns>The reservdel or an error</returns>
        //  2016-02-10 KJBO Pergas AB
        public ReservdelCL getReservdel(string ident, string VartOrdernr, int RadNr, int ReservNr)
        {
            List<ReservdelCL> reslist = getReservdelGeneral(ident, VartOrdernr, RadNr, ReservNr);
            return reslist[0];
        }



        /// <summary>
        /// General hub for retrieving reservdel
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="VartOrdernr"></param>
        /// <param name="RadNr"></param>
        /// <param name="ReservNr"></param>
        /// <returns>A list of reservdel or error</returns>
        private List<ReservdelCL> getReservdelGeneral(string ident, string VartOrdernr, int RadNr, int ReservNr)
        {
            List<ReservdelCL> reslist = new List<ReservdelCL>();


            CReparator cr = new CReparator();
            int identOK = cr.checkIdent(ident);

            if (identOK == -1)
            {
                ReservdelCL res = new ReservdelCL();                
                res.ErrCode = -10;
                res.ErrMessage = "Ogiltigt login";
                reslist.Add(res);
                return reslist;
            }

            // Build sql string depending on parameters. -1 as ReservNr means return all reservdel for one
            // ServiceRad. 
            string sSql = " select vart_ordernr, radnr, reserv_nr, antal, artnr, artnamn, faktureras, lev_id, enhet "
                        + " , reg, regdat, uppdaterat, uppdat_dat, skriv_nu "
                        + " from reservdel "
                        + " where vart_ordernr = :vart_ordernr "
                        + " and radnr = :radnr ";
            if (ReservNr > -1)
                sSql += " and reserv_nr = :reserv_nr ";

            // Create parameters
            NxParameterCollection np = new NxParameterCollection();
            np.Add("vart_ordernr", VartOrdernr);
            np.Add("radnr", RadNr);
            if (ReservNr > -1)
                np.Add("reserv_nr", ReservNr);

            string errText = "";

            DataTable dt = cdb.getData(sSql, ref errText, np);

            int errCode = -100;

            if (errText == "" && dt.Rows.Count == 0)
            {
                if (ReservNr == -1)
                    errText = "Det finns inga reservdelar i aktuellt urval ";
                else
                    errText = "Reservdel med ID VartOrdernr : " + VartOrdernr + ", RadNr : " + RadNr.ToString() + ", ReservNr : " + ReservNr.ToString() + " finns ej.";
                errCode = 0;
            }

            if (errText != "")
            {
                ReservdelCL res = new ReservdelCL();
                if (errText.Length > 2000)
                    errText = errText.Substring(1, 2000);
                res.ErrCode = errCode;
                res.ErrMessage = errText;
                reslist.Add(res);
                return reslist;
            }

            foreach (DataRow dr in dt.Rows)
            {
                ReservdelCL res = new ReservdelCL();
                res.Antal = 1;
                res.Faktureras = true;
                res.VartOrdernr = dr["vart_ordernr"].ToString();
                res.Radnr = Convert.ToInt32(dr["radnr"]);
                res.ReservNr = Convert.ToInt32(dr["reserv_nr"]);
                if (dr["antal"] != DBNull.Value)
                    res.Antal = Convert.ToDecimal(dr["antal"]);
                res.Artnr = dr["artnr"].ToString();
                res.ArtNamn = dr["artnamn"].ToString();
                if (dr["faktureras"] != DBNull.Value)
                    res.Faktureras = Convert.ToBoolean(dr["faktureras"]);
                res.LevID = dr["lev_id"].ToString();
                res.Enhet = dr["enhet"].ToString();
                reslist.Add(res);
            }

            return reslist;

        }

        private int validateServiceRow(ReservdelCL r)
        {
            string sSql = " select count(*) as antal "
                        + " from servicerad "
                        + " where vart_ordernr = :vart_ordernr "
                        + " and radnr = :radnr ";

            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("vart_ordernr", r.VartOrdernr);
            pc.Add("radnr", r.Radnr);

            string errText = "";

            DataTable dt = cdb.getData(sSql, ref errText, pc);

            return Convert.ToInt16(dt.Rows[0][0]); 

        }

        private int validateReservdelExists(ReservdelCL r)
        {
            string sSql = " select count(*) as antal "
                        + " from reservdel "
                        + " where vart_ordernr = :vart_ordernr "
                        + " and radnr = :radnr "
                        + " and reserv_nr = :reserv_nr ";

            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("vart_ordernr", r.VartOrdernr);
            pc.Add("radnr", r.Radnr);
            pc.Add("reserv_nr", r.ReservNr);

            string errText = "";

            DataTable dt = cdb.getData(sSql, ref errText, pc);

            return Convert.ToInt16(dt.Rows[0][0]); 
        }


        private int validateReservdel(ReservdelCL r)
        {
            if (validateServiceRow(r) == 0)
                return -1;
            return 1;
        }


        private int getNextReservNr(ReservdelCL r)
        {
            string sSql = " SELECT coalesce(max(reserv_nr),0) as maxRow "
                        + " from reservdel "
                        + " where vart_ordernr = :vart_ordernr "
                        + " and radnr = :radnr ";

            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("vart_ordernr", r.VartOrdernr);
            pc.Add("radnr", r.Radnr);

            string err = "";
            DataTable dt = cdb.getData(sSql, ref err, pc);

            int maxrow = Convert.ToInt32(dt.Rows[0][0]);
            maxrow++;
            return maxrow;
        }


        private string getInsertSQL()
        {
            return " insert into reservdel ( antal, artnamn, artnr, enhet, faktureras "
             + " , lev_id, radnr, reg, regdat, reserv_nr "
             + " , skriv_nu, vart_ordernr)  "
             + "  values ( :antal, :artnamn, :artnr, :enhet, :faktureras "
             + " , :lev_id, :radnr, :reg, :regdat, :reserv_nr "
             + " , :skriv_nu, :vart_ordernr )";           
        }


        private string getUpdateSQL()
        {
            return " update reservdel "
             + " set antal = :antal "
             + ", artnamn = :artnamn "
             + ", artnr = :artnr "
             + ", enhet = :enhet "
             + ", faktureras = :faktureras "
             + ", lev_id = :lev_id "
             
             + ", skriv_nu = :skriv_nu "
             + ", uppdat_dat = :uppdat_dat "
             + ", uppdaterat = :uppdaterat "             
             + " where vart_ordernr = :vart_ordernr "
             + " and radnr = :radnr "
             + " and reserv_nr = :reserv_nr ";                     
        }

        private string getDeleteSQL()
        {
            return "delete from reservdel "
             + " where vart_ordernr = :vart_ordernr "
             + " and radnr = :radnr "
             + " and reserv_nr = :reserv_nr ";                     

        }

        
        private void setParameters(NxParameterCollection np, ReservdelCL r, string AnvID)
        {
            string sVar = "";            
            np.Add("antal", r.Antal);
            sVar = r.ArtNamn;
            np.Add("artnamn", sVar);
            sVar = r.Artnr;
            np.Add("artnr", sVar);
            sVar = r.Enhet;
            np.Add("enhet", sVar);
            np.Add("faktureras", r.Faktureras);
            sVar = r.LevID;
            np.Add("lev_id", sVar);
            sVar = AnvID;
            np.Add("reg", sVar);
            np.Add("regdat", System.DateTime.Now);
            np.Add("skriv_nu", true);
            np.Add("uppdat_dat", System.DateTime.Now);
            np.Add("uppdaterat", AnvID);
            sVar = r.VartOrdernr;
            np.Add("vart_ordernr", sVar);
            np.Add("radnr", r.Radnr);
             np.Add("reserv_nr", r.ReservNr);
        }


        /// <summary>
        /// Deletes a reservdel identified by primary key
        /// </summary>
        /// <param name="ident">identity string</param>
        /// <param name="reservdel">One valid reservdel</param>
        /// <returns>Empty string if OK otherwise error message</returns>
        public string deleteReservdel(string ident, ReservdelCL reservdel)
        {

            CReparator cr = new CReparator();
            int identOK = cr.checkIdent(ident);

            ReservdelCL retRes = new ReservdelCL();
            if (identOK == -1)
            {
                return "Ogiltigt login";
            }

            // Validate that order is open for editing
            CServiceHuvud ch = new CServiceHuvud();
            string sOpen = ch.isOpen(ident, reservdel.VartOrdernr);
            if (sOpen != "1")
            {
                {
                    retRes.ErrCode = -10;
                    if (sOpen == "-1")
                        return "Order är stängd för inmatning";                        
                    else
                        return sOpen;                    
                }
            }


            int exists = validateReservdelExists(reservdel);
            
            if (exists == 0)
            {
                return "Reservdel finns ej";
            }

            string sSql = "";

            sSql = getDeleteSQL();

            NxParameterCollection np = new NxParameterCollection();
            setParameters(np, reservdel, "");

            string errText = "";

            int iRc = cdb.updateData(sSql, ref errText, np);

            if (errText != "")
            {

                if (errText.Length > 2000)
                    errText = errText.Substring(1, 2000);

                return errText;
            }

            return "";

        }

    

        
        /// <summary>
        /// Saves a reservdel to database.
        /// If ReservNr = 0 then the method
        /// assumes that this is a new row to be added
        /// Otherwise an update is issued
        /// </summary>
        /// <param name="ident">Identity</param>
        /// <param name="reservdel">ReservdelCL</param>
        /// <returns>The new created or updated reservdel</returns>
        //  2016-02-10 KJBO
        public ReservdelCL saveReservdel( string ident, ReservdelCL reservdel)
        {
            
            CReparator cr = new CReparator();
            int identOK = cr.checkIdent(ident);

            ReservdelCL retRes = new ReservdelCL();
            if (identOK == -1)
            {                
                retRes.ErrCode = -10;
                retRes.ErrMessage = "Ogiltigt login";
                return retRes;
            }

            // Validate that order is open for editing
            CServiceHuvud ch = new CServiceHuvud();
            string sOpen = ch.isOpen(ident, reservdel.VartOrdernr);
            if (sOpen != "1")
            {
                {
                    retRes.ErrCode = -10;
                    if (sOpen == "-1")
                        retRes.ErrMessage = "Order är stängd för inmatning";
                    else
                        retRes.ErrMessage = sOpen;
                    return retRes;
                }
            }


            int valid = validateReservdel(reservdel);
            if (valid == -1)
            {
                retRes.ErrCode = -1;
                retRes.ErrMessage = "Felaktig serviceorder";
                return retRes;
            }

            string sSql = "";            

            // This is a new ventil
            if (reservdel.ReservNr == 0)
            {
                reservdel.ReservNr = getNextReservNr(reservdel);
                sSql = getInsertSQL();                
            }

            else
                sSql = getUpdateSQL();

            ReparatorCL rep = cr.getReparator(ident);
            NxParameterCollection np = new NxParameterCollection();
            setParameters(np, reservdel, rep.AnvID);

            string errText = "";

            int iRc = cdb.updateData(sSql, ref errText, np);

            if (errText != "")
            {

                if (errText.Length > 2000)
                    errText = errText.Substring(1, 2000);

                retRes.ErrCode = -100;
                retRes.ErrMessage = errText;
                return retRes;
            }

            return getReservdel(ident, reservdel.VartOrdernr, reservdel.Radnr, reservdel.ReservNr);            
           
        }


    }
}

            



