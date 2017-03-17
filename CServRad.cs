using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NexusDB.ADOProvider;
using System.Data;

namespace SManApi
{

    /// <summary>
    /// Handles servicerader
    /// </summary>
    /// 2016-02-03 KJBO Pergas AB
    public class CServRad
    {
        // Variable for database class
        CDB cdb = null;
        public CServRad()
        {
            cdb = new CDB();
        }

        /// <summary>
        /// Returns the SQL code for insert new servicerad
        /// </summary>
        /// <returns>SQL code</returns>
        // 2016-02-08 KJBO  Pergas AB
        private string getServradInsertSQL()
        {

            string sSql = " insert into servicerad ( ventil_id, vart_ordernr, radnr, kontroll, arbete "
                         + " , anmarkning, reservdelar, stalldon_kontroll, stalldon_arbete, stalldon_delar "
                         + " , lagesstall_kontroll, lagesstall_arbete, lagesstall_delar, antal_boxpack, boxpackning "
                         + " , boxpack_material, antal_brostpack, brostpackning, brostpack_material, ovr_komment, alternatekey, arbetsordernr "
                         + ", hos_kund, pa_verkstad, servradKlar, attention "
                         + " )  "
                         + "  values ( :pventil_id, :pvart_ordernr, :pradnr, :pkontroll, :parbete "
                         + " , :panmarkning, :preservdelar, :pstalldon_kontroll, :pstalldon_arbete, :pstalldon_delar "
                         + " , :plagesstall_kontroll, :plagesstall_arbete, :plagesstall_delar, :pantal_boxpack, :pboxpackning "
                         + " , :pboxpack_material, :pantal_brostpack, :pbrostpackning, :pbrostpack_material, :povr_komment, :palternatekey, :parbetsordernr "
                         + " , :phos_kund, :ppa_verkstad, :pservradKlar, :pAttention "
                         + "  )";  
            return sSql;
        }

        /// <summary>
        /// Creates SQL code for update all fiels for
        /// the servicerad table. With parameters
        /// </summary>
        /// <returns>SQL code</returns>
        // 2016-02-08 KJBO  Pergas AB
        private string getServradUpdateSQL()
        {

            string sSql = " update servicerad "
                         + " set ventil_id = :pventil_id "
                         + ", kontroll = :pkontroll "
                         + ", arbete = :parbete "
                         + ", anmarkning = :panmarkning "
                         + ", reservdelar = :preservdelar "
                         + ", stalldon_kontroll = :pstalldon_kontroll "
                         + ", stalldon_arbete = :pstalldon_arbete "
                         + ", stalldon_delar = :pstalldon_delar "
                         + ", lagesstall_kontroll = :plagesstall_kontroll "
                         + ", lagesstall_arbete = :plagesstall_arbete "
                         + ", lagesstall_delar = :plagesstall_delar "
                         + ", antal_boxpack = :pantal_boxpack "
                         + ", boxpackning = :pboxpackning "
                         + ", boxpack_material = :pboxpack_material "
                         + ", antal_brostpack = :pantal_brostpack "
                         + ", brostpackning = :pbrostpackning "
                         + ", brostpack_material = :pbrostpack_material "
                         + ", ovr_komment = :povr_komment "                         
                         + ", alternatekey = :palternatekey "
                         + ", arbetsordernr = :parbetsordernr "          
                         + ", hos_kund = :phos_kund "
                         + ", pa_verkstad = :ppa_verkstad "
                         + ", servradKlar = :pservradKlar "
                         + ", attention = :pAttention "
                         + " where vart_ordernr =  :pvart_ordernr"
                         + " and radnr = :pradnr ";

            return sSql;

        }


        /// <summary>
        /// Get the delimiter for update statements
        /// 
        /// </summary>
        /// <param name="bFirst">Swith to signal if this is the first call</param>
        /// <returns>delimiter as string</returns>
        // 2016-02-08 KJBO  Pergas AB
        private string getDelimiter(ref bool bFirst)
        {
            if (bFirst)
            {
                bFirst = false;
                return " set ";
            }
            return " , ";

        }


        /// <summary>
        /// Creates an update SQL that only creates
        /// update statement if any change has been
        /// made for the current column
        /// </summary>
        /// <param name="sr">New ServiceradCL, updated by the client program</param>
        /// <param name="orig">Original ServiceradCL with values as the where when the client "checked out"</param>
        /// <returns>SQL statement</returns>
        // 2016-02-09 KJBO  Pergas AB
        private string getServradUpdateSQL(ServiceRadCL sr, ServiceRadCL orig, ref bool valveChanged)
        {
            bool bFirst = false;
            valveChanged = false;
            string sSql = " update servicerad ";
            sSql += " set vart_ordernr = :pvart_ordernr ";
            if (sr.VentilID != orig.VentilID)
            {
                sSql += getDelimiter(ref bFirst) + " ventil_id = :pventil_id ";
                valveChanged = true;
            }
            if (sr.Kontroll != orig.Kontroll)
                sSql += getDelimiter(ref bFirst) + " kontroll = :pkontroll ";
            if (sr.Arbete != orig.Arbete)
                sSql += getDelimiter(ref bFirst) + " arbete = :parbete ";
            if (sr.Anmarkning != orig.Anmarkning)
                sSql += getDelimiter(ref bFirst) + " anmarkning = :panmarkning ";
            if (sr.Reservdelar != orig.Reservdelar)
                sSql += getDelimiter(ref bFirst) + " reservdelar = :preservdelar ";
            if (sr.StalldonKontroll != orig.StalldonKontroll)
                sSql += getDelimiter(ref bFirst) + " stalldon_kontroll = :pstalldon_kontroll ";
            if (sr.StalldonArbete != orig.StalldonArbete)
                sSql += getDelimiter(ref bFirst) + " stalldon_arbete = :pstalldon_arbete ";
            if (sr.StalldonDelar != orig.StalldonDelar)
                sSql += getDelimiter(ref bFirst) + " stalldon_delar = :pstalldon_delar ";
            if (sr.LagesstallKontroll != orig.LagesstallKontroll)
                sSql += getDelimiter(ref bFirst) + " lagesstall_kontroll = :plagesstall_kontroll ";
            if (sr.LagesstallArbete != orig.LagesstallArbete)
                sSql += getDelimiter(ref bFirst) + " lagesstall_arbete = :plagesstall_arbete ";
            if (sr.LagesstallDelar != orig.LagesstallDelar)
                sSql += getDelimiter(ref bFirst) + " lagesstall_delar = :plagesstall_delar ";
            if (sr.AntalBoxpack != orig.AntalBoxpack)
                sSql += getDelimiter(ref bFirst) + " antal_boxpack = :pantal_boxpack ";
            if (sr.Boxpackning != orig.Boxpackning)
                sSql += getDelimiter(ref bFirst) + " boxpackning = :pboxpackning ";
            if (sr.BrostpackMaterial!= orig.BoxpackMaterial)
                sSql += getDelimiter(ref bFirst) + " boxpack_material = :pboxpack_material ";
            if (sr.AntalBrostpack != orig.AntalBrostpack)
                sSql += getDelimiter(ref bFirst) + " antal_brostpack = :pantal_brostpack ";
            if (sr.Brostpackning != orig.Brostpackning)
                sSql += getDelimiter(ref bFirst) + " brostpackning = :pbrostpackning ";
            if (sr.BrostpackMaterial != orig.BrostpackMaterial)
                sSql += getDelimiter(ref bFirst) + " brostpack_material = :pbrostpack_material ";
            if (sr.OvrKomment != orig.OvrKomment)
                sSql += getDelimiter(ref bFirst) + " ovr_komment = :povr_komment ";
            if (sr.AlternateKey != orig.AlternateKey)
                sSql += getDelimiter(ref bFirst) + " alternatekey = :palternatekey ";
            if (sr.Arbetsordernr != orig.Arbetsordernr)
                sSql += getDelimiter(ref bFirst) + " arbetsordernr = :parbetsordernr ";
            if (sr.hosKund != orig.hosKund)
                sSql += getDelimiter(ref bFirst) + " hos_kund = :phos_kund ";
            if (sr.paVerkstad != orig.paVerkstad)
                sSql += getDelimiter(ref bFirst) + " pa_verkstad = :ppa_verkstad ";
            if (sr.klar != orig.klar)
                sSql += getDelimiter(ref bFirst) + " servradKlar = :pservradKlar ";
            if (sr.Attention != orig.Attention)
                sSql += getDelimiter(ref bFirst) + " attention = :pAttention ";
            sSql += " where vart_ordernr =  :pvart_ordernr"
                  + " and radnr = :pradnr ";
            return sSql;
        }


        /// <summary>
        /// Convert from servicerad object to SQL parameters
        /// </summary>
        /// <param name="np">Parameter collection (being updated in the function)</param>
        /// <param name="sr">ServiceradCl</param>
        // 2016-02-09 KJBO  Pergas AB
        private void setParameters(NxParameterCollection np, ServiceRadCL sr)
        {


            string sVar = sr.VentilID;
            np.Add("pventil_id", sVar);
            sVar = sr.VartOrdernr;
            np.Add("pvart_ordernr", sVar);                     
            np.Add("pradnr", sr.Radnr);
            sVar = sr.Kontroll;          
            np.Add("pkontroll", sVar);
            sVar = sr.Arbete;
            np.Add("parbete", sVar);
            sVar = sr.Anmarkning;            
            np.Add("panmarkning", sVar);
            sVar = sr.Reservdelar;            
            np.Add("preservdelar", sVar);
            sVar = sr.StalldonKontroll;
            np.Add("pstalldon_kontroll", sVar);
            sVar = sr.StalldonArbete;      
            np.Add("pstalldon_arbete", sVar);
            sVar = sr.StalldonDelar;          
            np.Add("pstalldon_delar", sVar);
            sVar = sr.LagesstallKontroll;            
            np.Add("plagesstall_kontroll", sVar);
            sVar = sr.LagesstallArbete;            
            np.Add("plagesstall_arbete", sVar);
            sVar = sr.LagesstallDelar;            
            np.Add("plagesstall_delar", sVar);           
            if (sr.AntalBoxpack == 0)
                np.Add("pantal_boxpack", System.DBNull.Value);
            else
                np.Add("pantal_boxpack", sr.AntalBoxpack);
            sVar = sr.Boxpackning;
            np.Add("pboxpackning", sVar);
            sVar = sr.BoxpackMaterial;
            np.Add("pboxpack_material", sVar);            
            if (sr.AntalBrostpack == 0)
                np.Add("pantal_brostpack", System.DBNull.Value);
            else
                np.Add("pantal_brostpack", sr.AntalBrostpack);
            sVar = sr.Brostpackning;
            np.Add("pbrostpackning", sVar);
            sVar = sr.BrostpackMaterial;
            np.Add("pbrostpack_material", sVar);
            sVar = sr.OvrKomment;            
            np.Add("povr_komment", sVar);
            np.Add("palternatekey", sr.AlternateKey);
            sVar = sr.Arbetsordernr;
            np.Add("parbetsordernr", sVar);
            if (sr.hosKund == 0)
                np.Add("phos_kund", false);
            else
                np.Add("phos_kund", true);
            if (sr.paVerkstad == 0)
                np.Add("ppa_verkstad", false);
            else
                np.Add("ppa_verkstad", true);
            if (sr.klar == 0)
                np.Add("pservradKlar", false);
            else
                np.Add("pservradKlar", true);
            // 2016-11-22 KJBO
            if (sr.Attention == 0)
                np.Add("pAttention", false);
            else
                np.Add("pAttention", true);
        }



        /// <summary>
        /// Validates ordernr exists in db
        /// </summary>
        /// <param name="vartOrdernr">Ordernr</param>
        /// <param name="err">Reference to error</param>
        /// <returns>1-Exists 0-missing</returns>
        // 2016-02-09 KJBO  Pergas AB
        private int validateVartOrdernr(string vartOrdernr, ref string err)
        {
            CServiceHuvud cs = new CServiceHuvud();
            return cs.validateVartOrdernr(vartOrdernr, ref err);
        }

        /// <summary>
        /// Validate that the ventilID exists in the database
        /// </summary>
        /// <param name="ventilID">VentilID</param>
        /// <param name="err">Reference to SQL error</param>
        /// <returns>1-Exists 0-Missing</returns>
        private int validateVentilID( string ventilID, ref string err)
        {
            CVentil cv = new CVentil();
            return cv.validateVentilID(ventilID, ref err);

        }

        /// <summary>
        /// Huv for calling validation functions
        /// </summary>
        /// <param name="sr">ServiceradCL to be validate</param>
        /// <param name="err">Reference to error</param>
        /// <returns>1-OK, -1 Ordernr doesnt exist, -2 VentilID doesnt exist</returns>
        // 2016-02-09 KJBO  Pergas AB
        private int validateServRad(ServiceRadCL sr, ref string err)
        {
            if (validateVartOrdernr(sr.VartOrdernr, ref err) == 0 || err != "") 
                return -1;
            if (validateVentilID(sr.VentilID, ref err) == 0 || err != "")
                return -2;
            return 1;
        }


        /// <summary>
        /// Calculates the next "free" radnr
        /// </summary>
        /// <param name="vartOrdernr">Ordernr</param>
        /// <returns>The first available radnr</returns>
        // 2016-02-09 KJBO  Pergas AB
        private int getNewRadnr( string vartOrdernr)
        {

            string sSql = " SELECT coalesce(max(radnr),0) as maxRow "
                        + " FROM ServiceRad "
                        + " where vart_ordernr = :pVartOrdernr ";

            NxParameterCollection np = new NxParameterCollection();
            np.Add("pVartOrdernr", vartOrdernr);

            string err = "";
            DataTable dt = cdb.getData(sSql, ref err, np);

            int maxrow = Convert.ToInt32(dt.Rows[0][0]);
            maxrow++;
            return maxrow;

        }



        /// <summary>
        /// Saves a servicerad
        /// </summary>
        /// <param name="sr">New or changed ServiceRadCL</param>
        /// <param name="ident">Identity of caller</param>
        /// <returns>An instance of the inserted/updated row or one row with error description</returns>
        // 2016-02-09 KJBO  Pergas AB
        public ServiceRadCL saveServRad(ServiceRadCL sr, string ident)
        {
            bool bNew = false;
            bool bValveChanged = false;
            ServiceRadCL lSr = new ServiceRadCL();
            CReparator cr = new CReparator();

            // Check login
            int identOK = cr.checkIdent(ident);
            if (identOK == -1)
            {
                lSr.ErrCode = -10;
                lSr.ErrMessage = "Ogiltigt login";
                return lSr;
            }

            CServiceHuvud ch = new CServiceHuvud();
            string sOpen = ch.isOpen(ident, sr.VartOrdernr);
            if (sOpen != "1")
            {
                {
                    lSr.ErrCode = -10;
                    if (sOpen == "-1")
                        lSr.ErrMessage = "Order är stängd för inmatning";
                    else
                        lSr.ErrMessage = sOpen;
                    return lSr;
                }
            }

            // Get reparator identified by ident
            ReparatorCL reparator = cr.getReparator(ident);

            // init error code
            string err = "";

            // Do a validation
            int validate = validateServRad(sr, ref err);

            // Check for DB errors
            if (err != "")
            {
                err = "DbError when validating servRad : " + err;
                lSr.ErrCode = -100;
                lSr.ErrMessage = err;
                return lSr;
            }

            // Is ordernr correct?
            if (validate == -1)
            {
                lSr.ErrCode = -1;
                lSr.ErrMessage = "Felaktigt ordernr (" + sr.VartOrdernr + ") ";
                return lSr;
            }

            // Is ventilID correct?
            if (validate == -2)
            {
                lSr.ErrCode = -1;
                lSr.ErrMessage = "Felaktigt ventilID (" + sr.VentilID + ") ";
                return lSr;
            }


            // Start building SQL clause
            String sSql = "";

            // Indicates new row
            if (sr.Radnr == 0)
            {
                // Get new radnr
                sr.Radnr = getNewRadnr(sr.VartOrdernr);
                // Create new alternate key
                sr.AlternateKey = Guid.NewGuid().ToString();
                // Get SQL for insert
                sSql = getServradInsertSQL();                
                bNew = true;
            }
            else
            {
                // Get the original version of the ServiceradCL (the version checked out by AnvID
                ServiceRadCL orig = getServRad(ident, sr.VartOrdernr, sr.Radnr, reparator.AnvID);

                // Validate the result. If anything goes wrong (probably original row not found)
                // then return the "default" SQL update which updates all fields
                if (orig.ErrMessage == "" )
                    // This is the smart update that only updates changed fields
                    sSql = getServradUpdateSQL(sr, orig, ref bValveChanged);
                else
                    // This is the default update that will update all fields
                    sSql = getServradUpdateSQL();                    
            }

            // Create a collection of parameters
            NxParameterCollection np = new NxParameterCollection();
            // Get all parameters from servicerad
            setParameters(np, sr);

            // Init errortext
            string errText = "";
            // Do the update or insert
            int iRc = cdb.updateData(sSql, ref errText, np);

            // If anything goes wrong
            if (errText != "")
            {

                if (errText.Length > 2000)
                    errText = errText.Substring(1, 2000);
                lSr.ErrCode = -100;
                lSr.ErrMessage = errText;
                return lSr;
            }

            // Get a reparator from ident
            ReparatorCL rep = cr.getReparator(ident);
            // Stores reparator information to servicerad
            storeReparator(rep, sr.VartOrdernr, sr.Radnr);
            // Stores reparator information to the servradrep table (this is the new way!)
            storeReparator2(rep, sr.AlternateKey);

            // If this is a new row
            if (bNew || bValveChanged)
            {
                // Store default values
                // Removed 2016-09-12 because all values shall be handled by app
                //storeDefaults(sr.AlternateKey);
                // Get values from ventil (duplicate some values for history reasons)
                getValuesFromVentil(sr.AlternateKey);
            }

            if (sr.OvrKomment != "")
            {
                CVentil cv = new CVentil();
                cv.updateForraComment(sr.VentilID, sr.OvrKomment);
            }
 


            // Now return the row to the caller with reparator values and default stored
            return getServRad(ident, sr.VartOrdernr, sr.Radnr);            
        }



        /// <summary>
        /// Get values from ventil that shall be stored in servicerad
        /// </summary>
        /// <param name="alternatekey">alternat key to servicerad</param>
        private void getValuesFromVentil(string alternatekey)
        {
            // Get the ventil ID from servicerad
            string sSql = " select ventil_id "
                        + " from servicerad "
                        + " where alternatekey = :alternatekey ";

            // Create parameters
            NxParameterCollection np = new NxParameterCollection();
            np.Add("alternatekey", alternatekey);

            string err = ""; // Not used.....
            DataTable dt = cdb.getData(sSql, ref err, np);

            // get the ventilID into local parameter
            string ventil_id = "";
            if (dt.Rows.Count == 1)
            {
                ventil_id = dt.Rows[0][0].ToString();
            }

            // Didnt get any ventil (this shall never happen......)
            if (ventil_id == "")
                return;

            // SQL clause to get the desired values from ventil
            sSql = " select v.\"position\", v.avdelning, v.anlaggningsnr, v.oppningstryck "
                + " , v.stalldonstyp, v.stalldon_id_nr, v.stalldon_fabrikat, v.stalldon_artnr "
                + " , v.lagesstallartyp, v.lagesstall_id_nr, v.lagesstall_fabrikat, v.forra_comment "
                + " from ventil v "
                + " where v.ventil_id = :ventil_id ";
            // Add parameter
            np.Add("ventil_id", ventil_id);

            dt = cdb.getData(sSql, ref err, np);

            // If ventil is found....
            if (dt.Rows.Count == 1)
            {
                // Create parameters from each field in the Ventil SQL clause
                DataRow dr = dt.Rows[0];
                string sVar = dr["lagesstallartyp"].ToString();
                np.Add("plagesstallartyp", sVar);
                sVar = dr["lagesstall_id_nr"].ToString();
                np.Add("plagesstall_id_nr", sVar);
                sVar = dr["lagesstall_fabrikat"].ToString();
                np.Add("plagesstall_fabrikat", sVar);
                sVar = dr["stalldonstyp"].ToString();
                np.Add("pstalldonstyp", sVar);
                sVar = dr["stalldon_id_nr"].ToString();
                np.Add("pstalldon_id_nr", sVar);
                sVar = dr["stalldon_fabrikat"].ToString();
                np.Add("pstalldon_fabrikat", sVar);
                sVar = dr["stalldon_artnr"].ToString();
                np.Add("pstalldon_artnr", sVar);
                if (dr["oppningstryck"] == DBNull.Value)
                    np.Add("poppningstryck", System.DBNull.Value);
                else
                    np.Add("poppningstryck", Convert.ToDecimal(dr["oppningstryck"]));
                sVar = dr["anlaggningsnr"].ToString();
                np.Add("panlaggningsnr", sVar);
                sVar = dr["avdelning"].ToString();
                np.Add("pavdelning", sVar);
                sVar = dr["position"].ToString();
                np.Add("pkundens_pos", sVar);
                sVar = dr["forra_comment"].ToString();
                np.Add("povr_komment", sVar);
            }

            // Update clause with parameters for the current servicerad (parameters created above)
            string sSqlUpdate = " update servicerad "
            + " set lagesstallartyp = :plagesstallartyp "
             + ", lagesstall_id_nr = :plagesstall_id_nr "
             + ", lagesstall_fabrikat = :plagesstall_fabrikat "
             + ", stalldonstyp = :pstalldonstyp "
             + ", stalldon_id_nr = :pstalldon_id_nr "
             + ", stalldon_fabrikat = :pstalldon_fabrikat "
             + ", stalldon_artnr = :pstalldon_artnr "
             + ", oppningstryck = :poppningstryck "
             + ", anlaggningsnr = :panlaggningsnr "
             + ", avdelning = :pavdelning "
             + ", kundens_pos = :pkundens_pos "
             + ", ovr_komment = :povr_komment "
             + " where alternatekey = :alternatekey ";

            err = "";
            // And update
            cdb.updateData(sSqlUpdate, ref err, np);
            return;
        }


        /// <summary>
        /// Store default values in servicerad
        /// </summary>
        /// <param name="alternatekey"></param>
        
        private void storeDefaults(string alternatekey)
        {
            string sSql = " update servicerad "
                        + " set hos_kund = true "
                        + " , pa_verkstad = false "
                        + " where alternatekey = :alternatekey ";

            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("alternatekey", alternatekey);

            string err = "";
            cdb.updateData(sSql, ref err, pc);
        }


        /// <summary>
        /// Store reparator (if this is one of the three first
        /// reparators for this servicerad)
        /// </summary>
        /// <param name="r"></param>
        /// <param name="vart_ordernr"></param>
        /// <param name="radnr"></param>
        // 2016-02-09 KJBO  Pergas AB
        private void storeReparator(ReparatorCL r, string vart_ordernr, int radnr)
        {
            string sSql = " select reparator, reparator2, reparator3, alternatekey "
                        + " from servicerad "
                        + " where vart_ordernr = :pVartOrdernr "
                        + " and radnr = :radnr ";

            NxParameterCollection np = new NxParameterCollection();
            np.Add("pVartOrdernr", vart_ordernr);
            np.Add("radnr", radnr);

            string err = "";
            DataTable dt = cdb.getData(sSql, ref err, np);

            if (dt.Rows.Count == 1)
            {
                // First check if the current reparator
                // is in one of the three reparator fields
                // on this servicerad.....
                bool bExists = false;
                DataRow dr = dt.Rows[0];
                if (dr["reparator"] != DBNull.Value)
                {
                    if (dr["reparator"].ToString() == r.Reparator)
                        bExists = true;
                }
                if (dr["reparator2"] != DBNull.Value)
                {
                    if (dr["reparator2"].ToString() == r.Reparator)
                        bExists = true;
                }
                if (dr["reparator3"] != DBNull.Value)
                {
                    if (dr["reparator3"].ToString() == r.Reparator)
                        bExists = true;
                }

                // ... If not then
                if (!bExists)
                {
                    bool bRepReady = false;
                    np.Add("pReparator", r.Reparator);
                    if (dr["reparator"] == DBNull.Value)
                    {
                        sSql = " update servicerad "
                            + " set reparator = :pReparator "
                            + " where vart_ordernr = :pVartOrdernr "
                            + " and radnr = :radnr ";
                        bRepReady = true;
                    }
                    if (!bRepReady && dr["reparator2"] == DBNull.Value)
                    {
                        sSql = " update servicerad "
                            + " set reparator2 = :pReparator "
                            + " where vart_ordernr = :pVartOrdernr "
                            + " and radnr = :radnr ";
                        bRepReady = true;
                    }
                    if (!bRepReady && dr["reparator3"] == DBNull.Value)
                    {
                        sSql = " update servicerad "
                            + " set reparator3 = :pReparator "
                            + " , hos_kund = true "
                            + " where vart_ordernr = :pVartOrdernr "
                            + " and radnr = :radnr ";
                        bRepReady = true;
                    }
                    // If any of the three reparator fields
                    // was empty then this reparator is entered there
                    if (bRepReady)
                    {
                        err = "";
                        cdb.updateData(sSql, ref err, np);
                    }

                }


            }
        }


        /// <summary>
        /// Add the current reaparator to the servradrep table
        /// </summary>
        /// <param name="r"></param>
        /// <param name="alternatekey"></param>
        private void storeReparator2(ReparatorCL r, string alternatekey)
        {
            // First check if the current reparator already
            // exists in the table for this servicerad
            string sSql = " select count(*) as antal "
                        + " from servradrep "
                        + " where srAltKey = :pSrAltKey "
                        + " and AnvID = :pAnvID ";
            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("pSrAltKey", alternatekey);
            pc.Add("pAnvID", r.AnvID);

            string err = "";
            DataTable dt = cdb.getData(sSql, ref err, pc);

            // Shall always be 1
            if (dt.Rows.Count == 1)
            {
                DataRow dr = dt.Rows[0];
                if (Convert.ToInt16(dr[0]) == 0)
                {
                    sSql = " insert into servradrep (srAltKey, AnvID) "
                        + " values (:pSrAltKey, :pAnvID) ";                    
                    cdb.updateData(sSql, ref err, pc);
                }
            }
        }

        /// <summary>
        /// Store one servicerad
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="vartOrdernr"></param>
        /// <param name="radnr"></param>
        /// <returns>The new created or updated row</returns>
        // 2016-02-09 KJBO  Pergas AB
        public ServiceRadCL getServRad(string ident, string vartOrdernr, int radnr)
        {
            return getServRad(ident, vartOrdernr, radnr, "");
        }

        /// <summary>
        /// Private function to get one servicerad either from the
        /// servicerad table (as called from the public getServRad above)
        /// or from the anvServRad table which will return the version
        /// of servrad that was current when the user (AnvID) last retrieved
        /// the row.
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="vartOrdernr"></param>
        /// <param name="radnr"></param>
        /// <param name="AnvID"></param>
        /// <returns>ServiceRadCL (or one error row)</returns>
        private ServiceRadCL getServRad(string ident, string vartOrdernr, int radnr, string AnvID)
        {
            ServiceRadCL sr = new ServiceRadCL();
            
            // Check login
            CReparator cr = new CReparator();
            if (AnvID == "")
            {
                int identOK = cr.checkIdent(ident);

                if (identOK == -1)
                {
                    sr.ErrCode = -10;
                    sr.ErrMessage = "Ogiltigt login";
                    return sr;
                }
            }

            // Get reparator
            ReparatorCL r = cr.getReparator(ident);

            // SQL select clause that is build depending on if there is an AnvID or not as parameter
            string sSql = " SELECT sr.vart_ordernr, sr.radnr, sr.kontroll, sr.arbete, sr.anmarkning, sr.reservdelar, "
                        + " sr.reparator, sr.reparator2, sr.reparator3, sr.stalldon_kontroll, sr.stalldon_arbete, sr.stalldon_delar "
                        + " , sr.lagesstall_kontroll, sr.lagesstall_arbete, sr.lagesstall_delar, sr.antal_boxpack, sr.boxpackning "
                        + " , sr.boxpack_material, sr.antal_brostpack, sr.brostpackning, sr.brostpack_material, sr.ovr_komment, "
                        + " sr.ventil_id, sr.alternatekey, sr.arbetsordernr, sr.hos_kund, sr.pa_verkstad, coalesce(sr.servradKlar,false) servradklar, attention ";
            if (AnvID == "")
                sSql += " from servicerad sr ";
            else
                sSql += " from anvServiceRad sr ";
            sSql += " where sr.vart_ordernr = :pVartOrdernr "
                 + " and sr.radnr = :pRadnr ";
            if (AnvID != "")
                sSql += " and sr.AnvID = :pAnvID ";


            // Create parameter list
            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("pVartOrdernr", vartOrdernr);
            pc.Add("pRadnr", radnr);
            if (AnvID != "")
                pc.Add("pAnvID", AnvID);


            string errText = "";

            DataTable dt = cdb.getData(sSql, ref errText, pc);

            int errCode = -100;

            // No row is found
            if (errText == "" && dt.Rows.Count == 0)
            {
                errText = "Felaktigt Order eller radnr";
                errCode = 0;
            }

            // Now store a copy of of servicerad into the anvServicerad table
            // for future use when updating
            if (AnvID == "")
            {
                // Store a copy of the current servrad marked with the current
                // AnvID
                errText = checkoutServiceRad(r.AnvID, vartOrdernr, radnr);

                // Error handling
                if (errText != "")
                {
                    if (errText.Length > 2000)
                        errText = errText.Substring(1, 2000);
                    sr.ErrCode = errCode;
                    sr.ErrMessage = errText;
                    return sr;
                }
            }


            // Now its time to fill the ServiceRadCL
            DataRow dr = dt.Rows[0];

            sr.VartOrdernr = dr["vart_ordernr"].ToString();
            sr.Radnr = Convert.ToInt32(dr["radnr"]);
            sr.Kontroll = dr["kontroll"].ToString();
            sr.Arbete = dr["arbete"].ToString();
            sr.Anmarkning = dr["anmarkning"].ToString();
            sr.Reservdelar = dr["reservdelar"].ToString();
            sr.Reparator = dr["reparator"].ToString();
            sr.Reparator2 = dr["reparator2"].ToString();
            sr.Reparator3 = dr["reparator3"].ToString();
            sr.StalldonKontroll = dr["stalldon_kontroll"].ToString();
            sr.StalldonArbete = dr["stalldon_arbete"].ToString();
            sr.StalldonDelar = dr["stalldon_delar"].ToString();
            sr.LagesstallKontroll = dr["lagesstall_kontroll"].ToString();
            sr.LagesstallArbete = dr["lagesstall_arbete"].ToString();
            sr.LagesstallDelar = dr["lagesstall_delar"].ToString();
            if (dr["antal_boxpack"] == DBNull.Value)
                sr.AntalBoxpack = 0;
            else
                sr.AntalBoxpack = Convert.ToInt32(dr["antal_boxpack"]);
            sr.Boxpackning = dr["boxpackning"].ToString();
            sr.BoxpackMaterial = dr["boxpack_material"].ToString();
            if (dr["antal_brostpack"] == DBNull.Value)
                sr.AntalBrostpack = 0;
            else
                sr.AntalBrostpack = Convert.ToInt32(dr["antal_brostpack"]);
            sr.Brostpackning = dr["brostpackning"].ToString();
            sr.BrostpackMaterial = dr["brostpack_material"].ToString();
            sr.OvrKomment = dr["ovr_komment"].ToString();
            sr.VentilID = dr["ventil_id"].ToString();
            sr.AlternateKey = dr["alternatekey"].ToString();
            sr.Arbetsordernr = dr["arbetsordernr"].ToString();
            if (Convert.ToBoolean(dr["hos_kund"]) == true)
                sr.hosKund = 1;
            else
                sr.hosKund = 0;
            if (Convert.ToBoolean(dr["pa_verkstad"]) == true)
                sr.paVerkstad = 1;
            else
                sr.paVerkstad = 0;
            if (Convert.ToBoolean(dr["servradKlar"]) == true)
                sr.klar = 1;
            else
                sr.klar = 0;
            if (Convert.ToBoolean(dr["attention"]) == true)
                sr.Attention = 1;
            else
                sr.Attention = 0;
            sr.ErrCode = 0;
            sr.ErrMessage = "";

            // Return servicerad
            return sr;
        }



        /// <summary>
        /// Given a vartOrdernr this function return all
        /// belonging serverrad details.
        /// The ServiceRadList properties is a copy of
        /// how the user sees the service rows in ServiceManager10
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="vartOrdernr"></param>
        /// <returns></returns>
        /// 2016-02-03 KJBO Pergas AB
        public List<ServiceRadListCL> getAllServRad(string ident, string vartOrdernr)
        {

            List<ServiceRadListCL> srl = new List<ServiceRadListCL>();
    
            CReparator cr = new CReparator();

            int identOK = cr.checkIdent(ident);

            if (identOK == -1)
            {
                ServiceRadListCL sr = new ServiceRadListCL();
                sr.ErrCode = -10;
                sr.ErrMessage = "Ogiltigt login";
                srl.Add(sr);
                return srl;
            }
            


            string sSql = " SELECT s.vart_ordernr, s.radnr, s.anlaggningsnr, s.kundens_pos, vk.ventilkategori, v.ventiltyp, v.fabrikat, v.id_nr, s.kontroll, "
                        + " s.arbete, s.anmarkning, v.dn, v.dn2, v.pn, v.pn2, s.reparator, s.reparator2, s.reparator3, s.avdelning, s.arbetsordernr "
                        + ", s.hos_kund, s.pa_verkstad, coalesce(s.servradKlar,false) servradklar "
                        + " FROM servicerad s join ventil v on s.ventil_id = v.ventil_id join ventilkategori vk on v.ventilkategori = vk.ventilkat_id "
                        + " where s.vart_ordernr = :pVartOrdernr ";

            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("pVartOrdernr", vartOrdernr);

            string errText = "";

            DataTable dt = cdb.getData(sSql, ref errText, pc);

            int errCode = -100;

            // If not error and no rows exists.......
            if (errText == "" && dt.Rows.Count == 0)
            {
                CServiceHuvud cs = new CServiceHuvud();
                int countOrder = cs.validateVartOrdernr(vartOrdernr, ref errText);
                // .... return empty recordset if ordernr exists.....
                if (errText == "" && countOrder == 1)
                    return srl;
                
            }

            //... otherwise return felaktigt ordernr
            if (errText == "" && dt.Rows.Count == 0)
            {                                
                errText = "Felaktigt Ordernr";
                errCode = 0;
            }


            if (errText != "")
            {

                if (errText.Length > 2000)
                    errText = errText.Substring(1, 2000);
                ServiceRadListCL sr = new ServiceRadListCL();                
                sr.ErrCode = errCode;
                sr.ErrMessage = errText;
                srl.Add(sr);
                return srl;
            }

            ReparatorCL r = cr.getReparator(ident);

            // Loop and add to the ServiceRadList collection
            foreach (DataRow dr in dt.Rows)
            {
                ServiceRadListCL sr = new ServiceRadListCL();
                sr.VartOrdernr = dr["vart_ordernr"].ToString();
                sr.RadNr = Convert.ToInt32(dr["radnr"]);
                sr.Anlaggningsnr = dr["anlaggningsnr"].ToString();
                sr.IdNr = dr["id_nr"].ToString();
                sr.Avdelning = dr["avdelning"].ToString();
                sr.KundensPosNr = dr["kundens_pos"].ToString();
                sr.VentilKategori = dr["ventilkategori"].ToString();
                sr.Ventiltyp = dr["ventiltyp"].ToString();
                sr.Fabrikat = dr["fabrikat"].ToString();
                sr.Dn = dr["dn"].ToString();
                sr.Pn = dr["pn"].ToString();
                sr.Arbetsordernr = dr["arbetsordernr"].ToString();
                if (Convert.ToBoolean(dr["hos_kund"]) == true)
                    sr.hosKund = 1;
                else
                    sr.hosKund = 0;
                if (Convert.ToBoolean(dr["pa_verkstad"]) == true)
                    sr.paVerkstad = 1;
                else
                    sr.paVerkstad = 0;
                if (Convert.ToBoolean(dr["servradKlar"]) == true)
                    sr.klar = 1;
                else
                    sr.klar = 0;
                sr.ErrCode = 0;
                sr.ErrMessage = "";
                srl.Add(sr);                
            }

            return srl;
            
        }


        /// <summary>
        /// Store a copy of a servicerad marked with AnvID
        /// This row is intended for comparison on save to see if
        /// the AnvID has been changed any fields
        /// </summary>
        /// <param name="AnvID"></param>
        /// <param name="vart_ordernr"></param>
        /// <param name="radnr"></param>
        /// <returns>Error string or hopefully empty string</returns>
        private string checkoutServiceRad(string AnvID, string vart_ordernr, int radnr)
        {

            // Create table if not exists
            if (!cdb.bCheckIfTableExist("anvServicerad"))
                cdb.createAnvServRadTable();


            // Remove all histor for the current AnvID.
            // Only one row at the time exists for all users
            string sSql = " delete from anvServicerad "
                        + " where AnvID = :AnvID";

            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("AnvID", AnvID);

            string ErrTxt = "";
            cdb.updateData(sSql, ref ErrTxt, pc);

            // Insert statement
            sSql = "insert into anvServicerad "
                + " select :AnvID, sr.* "
                + " from servicerad sr "
                + " where sr.vart_ordernr = :vart_ordernr "
                + " and sr.radnr = :radnr ";

            pc.Add("vart_ordernr", vart_ordernr);
            pc.Add("radnr", radnr);

            cdb.updateData(sSql, ref ErrTxt, pc);
            // Error handling
            if (ErrTxt != "")
                ErrTxt = "Fel vid sparande av serviceradversion för användaren " + AnvID + ". " + ErrTxt;

            return ErrTxt;


        }


        /// <summary>
        /// Update all rows containing one ventil. Only update if the
        /// servicehuvud is open for app and not godkand
        /// Function is called from save of ventil
        /// </summary>
        /// <param name="ventilID"></param>
        public void updateFromVentil2(string ventilID)
        {
            // Check if anything we are interested in has been changed.
            // Also check that the order is open
            string sSql = " select sh.vart_ordernr "
                        + " from servicehuvud sh "
                        + " join servicerad sr on sh.vart_ordernr = sr.vart_ordernr "
                        + " join ventil v on sr.ventil_id = v.ventil_id "
                        + " where v.ventil_id = :ventil_id "
                        + " and ((coalesce(v.\"position\",'') <> coalesce(sr.kundens_pos,'')) or "
                        + " (coalesce(v.avdelning,'') <> coalesce(sr.avdelning,'')) or "
                        + " (coalesce(v.anlaggningsnr,'') <> coalesce(sr.anlaggningsnr)) or "
                        + " (coalesce(v.oppningstryck,0) <> coalesce(sr.oppningstryck,0)) or "
                        + " (coalesce(v.stalldonstyp) <> coalesce(sr.stalldonstyp)) or "
                        + " (coalesce(v.stalldon_id_nr,'') <> coalesce(sr.stalldon_id_nr)) or "
                        + " (coalesce(v.stalldon_fabrikat,'') <> coalesce(sr.stalldon_fabrikat,'')) or "
                        + " (coalesce(v.stalldon_artnr,'') <> coalesce(sr.stalldon_artnr,'')) or "
                        + " (coalesce(v.lagesstallartyp,'') <> coalesce(sr.lagesstallartyp,'')) or "
                        + " (coalesce(v.lagesstall_id_nr,'') <> coalesce(sr.lagesstall_id_nr,'')) or "
                        + " (coalesce(v.lagesstall_fabrikat,'') <> coalesce(sr.lagesstall_fabrikat,'')) or "
                        + " (coalesce(v.forra_comment,'') <> coalesce(sr.ovr_komment,''))) and "
                        + " sh.godkand = false and sh.openForApp = true ";
            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("ventil_id", ventilID);

            string err = "";
            DataTable dt = cdb.getData(sSql, ref err, pc);

            // No servicerows for this ventil. Just return
            if (dt.Rows.Count == 0)
                return;
            foreach (DataRow dr in dt.Rows)
            {

                // Retrieve the alternate key for all servicerows
                // in one serviceorder that has this ventil
                // Normally this shall be only one row....
                sSql = "select alternatekey "
                    + " from servicerad "
                    + " where vart_ordernr = :vart_ordernr "
                    + " and ventil_id = :ventil_id ";

                pc.Add("vart_ordernr", dr["vart_ordernr"].ToString());

                dt = cdb.getData(sSql, ref err, pc);

                // .... as said above, normally only one row
                foreach (DataRow dr2 in dt.Rows)
                {
                    getValuesFromVentil(dr2["alternatekey"].ToString());
                }


            }




        }

        public void updateFromVentil(string ventilID)
        {
            // Only servicerad that are open for app and not godkand can be updated
            // Now retrieve the latest servicerad for this ventil
            /*            string sSql = "select vart_ordernr, openforapp,godkand "
                        + " from servicehuvud "
                        + " where regdat = (SELECT max(regdat) "
                        + " FROM servicehuvud sh "
                        + " join servicerad sr on sh.vart_ordernr = sr.vart_ordernr "                        
                        + " where sr.ventil_id = :ventil_id) "; */

            string sSql = "select sh.vart_ordernr, sh.openforapp, sh.godkand "
                     + " from servicehuvud sh "
                     + " join servicerad sr on sh.vart_ordernr = sr.vart_ordernr "
                     + " where sr.ventil_id = :ventil_id "
                     + " and sh.openForApp = true and sh.godkand = false ";

            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("ventil_id", ventilID);

            string err = "";
            DataTable dt = cdb.getData(sSql, ref err, pc);
            
            // No servicerows for this ventil. Just return
            if (dt.Rows.Count == 0)
                return;
            DataRow dr = dt.Rows[0];

            // Check that the current row is valid for update
            if (Convert.ToBoolean(dr["openforapp"]) && !Convert.ToBoolean(dr["godkand"]))
            {
                // Retrieve the alternate key for all servicerows
                // in one serviceorder that has this ventil
                // Normally this shall be only one row....
                sSql = "select alternatekey "
                    + " from servicerad "
                    + " where vart_ordernr = :vart_ordernr "
                    + " and ventil_id = :ventil_id ";

                pc.Add("vart_ordernr", dr["vart_ordernr"].ToString());

                dt = cdb.getData(sSql, ref err, pc);

                // .... as said above, normally only one row
                foreach (DataRow dr2 in dt.Rows)
                {
                    getValuesFromVentil(dr2["alternatekey"].ToString());
                }


            }


        }

        public DataTable validateServRad(string srAltKey)
        {
            string sSql = " select vart_ordernr, radnr "
                        + " from servicerad "
                        + " where alternateKey = :srAltKey ";

            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("srAltKey", srAltKey);

            string err = "";

            return cdb.getData(sSql, ref err, pc);


        }

        public int validateServRad(string VartOrdernr, int radnr)
        {
            string sSql = " select count(*) as antal "
                        + " from servicerad "
                        + " where vart_ordernr = :vart_ordernr "
                        + " and radnr = :radnr";

            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("vart_ordernr", VartOrdernr);
            pc.Add("radnr", radnr);

            string err = "";

            DataTable dt = cdb.getData(sSql, ref err, pc);

            if (dt.Rows.Count == 0)
                return 0;
            return Convert.ToInt32(dt.Rows[0]["antal"]);
        }


        public int validteServRadBild(string VartOrdernr, int radnr, int bildNr)
        {

            string sSql = " select count(*) as antal "
            + " from servrad_bild "
            + " where vart_ordernr = :vart_ordernr "
            + " and radnr = :radnr "
            + " and bild_nr = :bild_nr ";
            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("vart_ordernr", VartOrdernr);
            pc.Add("radnr", radnr);
            pc.Add("bild_nr", bildNr);

            string err = "";

            DataTable dt = cdb.getData(sSql, ref err, pc);

            if (dt.Rows.Count == 0)
                return 0;
            return Convert.ToInt32(dt.Rows[0]["antal"]);

        }





        /// <summary>
        /// Returns the alternate key for a serviceRow
        /// </summary>
        /// <param name="vartOrdernr"></param>
        /// <param name="radnr"></param>
        /// <returns>If ident is invalid the function returns "-10"</returns>
        /// <returns>If a database error occurs then the return is "-1" followed by the database error description"</returns>        
        /// <returns>If no row is found by the provided primary key then the result is an empty string</returns>
        /// <returns>In the normal case (identity is OK and the primary key exists) the function return the alternate key</returns>
        //  2016-02-29 Pergas AB KJBO
        public string getAlternateKey( string ident, string vartOrdernr, int radnr)
        {

            CReparator cr = new CReparator();

            int identOK = cr.checkIdent(ident);

            if (identOK == -1)            
                return "-10";            

            string sSql = " select alternateKey "            
                        + " from servicerad "
                        + " where vart_ordernr = :vart_ordernr "
                        + " and radnr = :radnr ";

            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("vart_ordernr", vartOrdernr);
            pc.Add("radnr", radnr);            

            string err = "";

            DataTable dt = cdb.getData(sSql, ref err, pc);

            if (err != "")
                return "-100 " + err;

            if (dt.Rows.Count == 0)
                return "";
            return dt.Rows[0]["alternateKey"].ToString();

        }

        public int getOrdernrRadnrFromAltKey(string alternateKey, ref string vartOrdernr, ref int radnr)
        {
            string sSql = " select vart_ordernr, radnr "
                        + " from servicerad "
                        + " where alternatekey = :alternatekey ";

            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("alternatekey", alternateKey);

            string err = "";

            DataTable dt = cdb.getData(sSql, ref err, pc);

            if (err != "")
                return -1;

            if (dt.Rows.Count == 0)
                return 0;
            vartOrdernr = dt.Rows[0]["vart_ordernr"].ToString();
            radnr = Convert.ToInt32(dt.Rows[0]["radnr"]);
            return 1;
           
        }



        /// <summary>
        /// This method is called from CTidRed class to ensure that
        /// a reparator that register time on an order row also is registered
        /// as a reparator on that row.
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="srAltKey"></param>
        /// <param name="anvID"></param>
        public void ensureReparatorExists(string ident, string srAltKey, string anvID, string vartOrdernr = "")
        {            
            int radnr = 0;
            if (vartOrdernr == "")
                getOrdernrRadnrFromAltKey(srAltKey, ref vartOrdernr, ref radnr);
            if (vartOrdernr != "")
            {
                // Get a reparator from anvID
                CReparator cr = new CReparator();
                ReparatorCL rep = cr.getReparatorFromID(anvID);
                storeReparator(rep, vartOrdernr, radnr);
                storeReparator2(rep, srAltKey);
            }
        }



    }
}