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

            string sSql = " SELECT a.artnr, a.artnamn, a.lev_id, l.levnamn, a.anm1, a.anm2, kategori "
                        + " FROM artikel a "
                        + " left outer join leverantor l on a.lev_id = l.lev_id "
                        + " where a.artnr = :artnr "
                        + " and a.visas = true ";

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
                art.kategori = Convert.ToInt32(dr["kategori"]);
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
                        + " , reg, regdat, uppdaterat, uppdat_dat, skriv_nu, getFromCS "
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
                res.getFromCS = false;
                res.VartOrdernr = dr["vart_ordernr"].ToString();
                res.Radnr = Convert.ToInt32(dr["radnr"]);
                res.ReservNr = Convert.ToInt32(dr["reserv_nr"]);
                if (dr["antal"] != DBNull.Value)
                    res.Antal = Convert.ToDecimal(dr["antal"]);
                res.Artnr = dr["artnr"].ToString();
                res.ArtNamn = dr["artnamn"].ToString();
                if (dr["faktureras"] != DBNull.Value)
                    res.Faktureras = Convert.ToBoolean(dr["faktureras"]);
                if (dr["getFromCS"] != DBNull.Value)
                    res.getFromCS = Convert.ToBoolean(dr["getFromCS"]);
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


        private int validateArtikelExists(ReservdelCL r)
        {
            String sSql = " SELECT count(artnr) count_artikel "
                        + " FROM artikel "
                        + " where artnr = :artnr "
                        + " and visas = true ";
            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("artnr", r.Artnr);
            string err = "";
            DataTable dt = cdb.getData(sSql, ref err, pc);
            if (err != "")
                return -1;
            if (dt.Rows.Count == 0)
                return 0;
            return Convert.ToInt16(dt.Rows[0]["count_artikel"]);
        }




        private int validateReservdel(ReservdelCL r)
        {
            if (validateServiceRow(r) == 0)
                return -1;

            // If this is supposed to point to an existing 
            // reservdel then check that this reservdel exists
            // 2018-08-27 KJBO
            if (r.ReservNr != 0)
            {
                if (validateReservdelExists(r) == 0 ) 
                    return -4;
            }
            if (isDotArticle(r.Artnr))
            {
                if (r.ArtNamn == null || r.ArtNamn == "")
                    return -3;
                return 1;
            }
            int rc = validateArtikelExists(r);
            if (rc == -1)
                return -10;
            if (rc == 0)
                return -2;
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
             + " , skriv_nu, vart_ordernr, getFromCS, pyramidExport)  "
             + "  values ( :antal, :artnamn, :artnr, :enhet, :faktureras "
             + " , :lev_id, :radnr, :reg, :regdat, :reserv_nr "
             + " , :skriv_nu, :vart_ordernr, :getFromCS, :pyramidExport )";
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
             + ", getFromCS = :getFromCS "
             + ", pyramidExport = :pyramidExport "
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



        /// <summary>
        /// Standardize the unit to mixedcase
        /// </summary>
        /// <param name="aUnit"></param>
        /// <returns></returns>
        /// 2017-09-06 KJBO
        private string standardizeUnit(string aUnit)
        {
            if (aUnit == "st")
                return "St";
            if (aUnit == "m")
                return "M";
            if (aUnit == "kg")
                return "Kg";
            return aUnit;
        }


        private void setParameters(NxParameterCollection np, ReservdelCL r, string AnvID)
        {
            DateTime dtAncient = Convert.ToDateTime("2000-01-01");
            string sVar = "";
            np.Add("antal", r.Antal);
            sVar = r.ArtNamn;
            np.Add("artnamn", sVar);
            sVar = r.Artnr;
            np.Add("artnr", sVar);
            sVar = standardizeUnit(r.Enhet);
            np.Add("enhet", sVar);
            np.Add("faktureras", r.Faktureras);
            np.Add("getFromCS", r.getFromCS);
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
            np.Add("pyramidExport", dtAncient);
            
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

            AddOrDeleteReservdelPyr(reservdel, true, ref errText);

            if (errText == "")
                cdb.updateData(sSql, ref errText, np);

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
        public ReservdelCL saveReservdel(string ident, ReservdelCL reservdel)
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
            if (valid == -10)
            {
                retRes.ErrCode = -1;
                retRes.ErrMessage = "Fel vid kontroll av reservdel";
                return retRes;
            }

            if (valid == -2)
            {
                retRes.ErrCode = -1;
                retRes.ErrMessage = "Reservdel finns inte";
                return retRes;

            }

            if (valid == -3)
            {
                retRes.ErrCode = -1;
                retRes.ErrMessage = "Egen reservdel måste ha en benämning";
                return retRes;
            }

            if (valid == -4)
            {
                retRes.ErrCode = -1;
                retRes.ErrMessage = "Det finns ingen reservdel på ordernr : " + reservdel.VartOrdernr + "rad : " + reservdel.Radnr.ToString() + " reservdelsrad : " + reservdel.ReservNr.ToString();
                return retRes;
            }

            string sSql = "";
            string errText = "";
            int errCode = 0;
            
            // This is a new reservdel
            if (reservdel.ReservNr == 0)
            {
                reservdel.ReservNr = getNextReservNr(reservdel);
                sSql = getInsertSQL();
            }
            else            
                sSql = getUpdateSQL();


            AddOrDeleteReservdelPyr(reservdel, true, ref errText);            
            ReparatorCL rep = cr.getReparator(ident);
            NxParameterCollection np = new NxParameterCollection();
            setParameters(np, reservdel, rep.AnvID);

            int iRc = cdb.updateData(sSql, ref errText, np);


            if (errText == "")
            {
                AddOrDeleteReservdelPyr(reservdel, false, ref errText);
                if (errText != "")
                {
                    errText = "Fel vid anrop till addToReservdelPyr. Felmeddelande : " + errText;
                    errCode = -1303;
                }
            }
            if (errText != "")
            {

                if (errText.Length > 2000)
                    errText = errText.Substring(1, 2000);
                if (errCode == 0)
                    retRes.ErrCode = -100;
                else
                    retRes.ErrCode = errCode;
                retRes.ErrMessage = errText;
                return retRes;
            }

            // 2018-05-17 KJBO Check if this is a pyramidOrder
            CMServHuv shuv = new CMServHuv();
            if (shuv.isPyramidOrder(reservdel.VartOrdernr))
            {


                ErrorCL errCl = checkOutIfNeeded(ident, reservdel);
                if (errCl.ErrMessage != "")
                {
                    if (errText.Length > 2000)
                        errText = errText.Substring(1, 2000);

                    retRes.ErrCode = errCl.ErrCode;
                    retRes.ErrMessage = errCl.ErrMessage;
                    return retRes;

                }



                CompactStore.CCompactStore store = new CompactStore.CCompactStore();
                errCl = store.genCompStoreData(ident, reservdel.VartOrdernr);
                if (errCl.ErrMessage != "" && errCl.ErrCode != 1)
                {
                    if (errText.Length > 2000)
                        errText = errText.Substring(1, 2000);

                    retRes.ErrCode = errCl.ErrCode;
                    retRes.ErrMessage = errCl.ErrMessage;
                    return retRes;

                }

            }



            return getReservdel(ident, reservdel.VartOrdernr, reservdel.Radnr, reservdel.ReservNr);

        }

        private void AddOrDeleteReservdelPyr(ReservdelCL r, bool delete, ref string error)
        {
            string sSql = " select artnr, coalesce(artnamn,'') artnamn, antal "
                        + " from reservdel "
                        + " where vart_ordernr = :vart_ordernr "
                        + " and radnr = :radnr "
                        + " and reserv_nr = :reserv_nr ";
            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("vart_ordernr",r.VartOrdernr);
            pc.Add("radnr",r.Radnr);
            pc.Add("reserv_nr",r.ReservNr);
            error = "";
            DataTable dt = cdb.getData(sSql, ref error, pc);
            if (error != "")
                return;
            if (dt.Rows.Count == 1)
            {
                DataRow dr = dt.Rows[0];
                ReservdelCL res = new ReservdelCL();
                res.VartOrdernr = r.VartOrdernr;
                res.Radnr = r.Radnr;
                res.ReservNr = r.ReservNr;
                res.Artnr = dr["artnr"].ToString();
                res.ArtNamn = dr["artnamn"].ToString();                
                res.Antal = Convert.ToDecimal(dr["antal"]);
                if (delete)
                    res.Antal = -res.Antal;
                if (res.Antal > 0.001M || res.Antal < -0.001M)
                    addToReservdelPyr(res, ref error);
            }
        }

        private Decimal countReservdelNotUsed(ReservdelCL r , ref string error)
        {
            string sSql = " select coalesce(sum(antal),0) sumAntal "
                        + " from reservdel "
                        + " where vart_ordernr = :vart_ordernr "                        
                        + " and artnr = :artnr ";
            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("vart_ordernr",r.VartOrdernr);
            pc.Add("artnr", r.Artnr);            
            error = "";
            DataTable dt = cdb.getData(sSql, ref error, pc);
            if (error != "")
                return 0;
            return Convert.ToDecimal(dt.Rows[0]["sumAntal"]);
        }

        private Decimal getNotExportedNotUsedAnymore(ReservdelCL r, ref string error)
        {
            string sSql = " SELECT coalesce(sum(antal),0) sumAntal "
                        + " FROM reservdelPyr "
                        + " where vart_ordernr = :vart_ordernr "
                        + " and radnr = :radnr "
                        + " and reserv_nr = :reserv_nr ";
                        
            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("vart_ordernr",r.VartOrdernr);
            pc.Add("radnr",r.Radnr);
            pc.Add("reserv_nr",r.ReservNr);
            error = "";
            DataTable dt = cdb.getData(sSql, ref error, pc);
            if (error != "")
                return 0;
            return Convert.ToDecimal(dt.Rows[0]["sumAntal"]);

        }

        private void addToReservdelPyr(ReservdelCL r, ref string error)
        {
            string sSql = " update reservdelPyr "
                            + " set antal = antal + :antal "
                            + " where vart_ordernr = :vart_ordernr "
                            + " and artnr = :artnr "
                            + " and artnamn = :artnamn "
                            + " and PyramidExport is null ";

            error = "";
            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("vart_ordernr",r.VartOrdernr);
            pc.Add("artnr",r.Artnr);
            pc.Add("antal",r.Antal);
            pc.Add("artnamn", r.ArtNamn);
            int updated = cdb.updateData(sSql, ref error, pc);
            if (error != "")
                return;
            if (updated < 1)
            {
                sSql = " insert into reservdelPyr(vart_ordernr, artnr, artnamn, antal) "
                    + " values (:vart_ordernr, :artnr, :artnamn, :antal) ";
                cdb.updateData(sSql, ref error, pc);
            }
            return;
        }

        private OrderArtCL checkoutOrderArt(string ident, ReservdelCL res, Decimal coNumber)
        {
            ServHuvSrc.COrderArt coa = new ServHuvSrc.COrderArt();
            OrderArtCL oa = new OrderArtCL();
            oa.Artnr = res.Artnr;
            oa.VartOrdernr = res.VartOrdernr;
            oa.CoAntal = coNumber;
            oa = coa.checkoutOrderArt(ident, oa);
            return oa;
        }

        private ErrorCL checkOutIfNeeded(string ident, ReservdelCL res)
        {
            ErrorCL err = new ErrorCL();
            err.ErrCode = 0;
            err.ErrMessage = "";
            Decimal onOrder = 0;
            Decimal outChecked = 0;

            if (isDotArticle(res.Artnr))            
                return err;
            string errStr = "";
            int kategori = getArtKat(res.Artnr, ref errStr);
            if (kategori != 1)
                return err;
            if (errStr == "")            
                onOrder = countArtOnOrder(res, ref errStr);
            if (errStr == "")
                outChecked = countOutchecked(res, ref errStr);
            if (errStr != "")
            {
                err.ErrMessage = errStr;
                if (err.ErrMessage != "")
                    err.ErrCode = 13101;
                return err;
            }
            if (onOrder > outChecked)
            {
                OrderArtCL oaCL = checkoutOrderArt(ident, res, onOrder - outChecked);
                if (oaCL.ErrCode != 0)
                {
                    err.ErrMessage = oaCL.ErrMessage;
                    err.ErrCode = oaCL.ErrCode;
                    return err;
                }


                // Added 2018-08-27. Reservdel shall now be confirmed in Pyramid
                if (!res.getFromCS)
                {
                    List<ArticleCommit.CArticleCommitData> acList = new List<ArticleCommit.CArticleCommitData>();
                    ArticleCommit.CArticleCommitData ac = new ArticleCommit.CArticleCommitData();
                    ac.articleNumber = res.Artnr;
                    ac.orderNumber = res.VartOrdernr;
                    ac.quantity = onOrder - outChecked;
                    ac.orderArtID = oaCL.OrderArtId;
                    acList.Add(ac);
                    ArticleCommit.CArticleCommit acCommit = new ArticleCommit.CArticleCommit();
                    err = acCommit.generateFile(acList,"1");
                    //ac.sav
                }

                // Removed 2018-08-27 and replaced by the code above.

                //if (!res.getFromCS)
                //{
                //    CompactStore.updateOAStorageData data = new CompactStore.updateOAStorageData();
                //    data.orderArtId = oaCL.OrderArtId;
                //    //data.stockToSend = Convert.ToInt32(oaCL.OrdAntal);
                //    data.stockToSend = res.Antal;
                //    data.error = "";
                //    CompactStore.CCompactStore cs = new CompactStore.CCompactStore();
                //    string result = cs.updateDbWithoutSend(data, "x");
                //}
            }
            return err;
        }

        


        /// <summary>
        /// Count the number of this article on this order
        /// </summary>
        /// <param name="r"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        //  2018-04-30 Indentive AB Kjbo
        private Decimal countArtOnOrder(ReservdelCL r, ref String err)
        {
            String sSql = "select coalesce(sum(antal),0) sum_antal "
                        + " from reservdel "
                        + " where vart_ordernr = :vart_ordernr "
                        + " and artnr = :artnr ";
            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("vart_ordernr",r.VartOrdernr);
            pc.Add("artnr",r.Artnr);
            err = "";
            DataTable dt = cdb.getData(sSql, ref err, pc);

            if (err != "")
                return -1;
            return Convert.ToDecimal(dt.Rows[0]["sum_antal"]);
        }


        /// <summary>
        /// Count the number of outchecked articles
        /// </summary>
        /// <param name="r"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        // 2018-04-30 Indentive AB Kjbo
        private Decimal countOutchecked(ReservdelCL r, ref String err)
        {
            String sSql = " select coalesce(sum(coAntal),0) - coalesce(sum(ciAntal),0) sum_outchecked "
                        + " from orderArt "
                        + " where vart_ordernr = :vart_ordernr "
                        + " and artnr = :artnr ";
            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("vart_ordernr", r.VartOrdernr);
            pc.Add("artnr", r.Artnr);
            err = "";
            DataTable dt = cdb.getData(sSql, ref err, pc);

            if (err != "")
                return -1;
            return Convert.ToDecimal(dt.Rows[0]["sum_outchecked"]);
        }



        /// <summary>
        /// Check if this is a dot article (owner defined)
        /// Must have at least 2 characters and the last one
        /// shall be a dot.
        /// </summary>
        /// <param name="artnr"></param>
        /// <returns></returns>
        private bool isDotArticle(string artnr)
        {
            if (artnr.Length < 2)
                return false;
            if (artnr.Substring(artnr.Length - 1) == ".")
                return true;
            return false;
        }

        private int getArtKat(string artnr, ref string err)
        {
            string sSql = " SELECT coalesce(kategori,0) kategori "
                        + " FROM artikel "
                        + " where artnr = :artnr "
                        + " and visas = true ";
            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("artnr",artnr);
            err = "";
            DataTable dt = cdb.getData(sSql, ref err, pc);
            if (err != "")
            {
                return 0;
            }
            if (dt.Rows.Count == 0)
            {
                err = "Artikelnr : " + artnr + " finns ej/är ej aktiv";
                return 0;
            }
            return Convert.ToInt32(dt.Rows[0]["kategori"]);

        }



    }
}

            



