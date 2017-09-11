using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NexusDB.ADOProvider;
using System.Data;

namespace SManApi
{
    
    public class CVentil
    {

        CDB cdb = null;
        public CVentil()
        {
            cdb = new CDB();
        }

        public VentilCL getVentil(string ident, string ventilID)
        {
            return getVentil(ident, ventilID, true);
        }


        /// <summary>
        /// Get all ventils for one customer
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="KundID"></param>
        /// <returns></returns>
        public List<VentilCL> getVentilsForCust(string ident, string KundID, string position, string IDnr, string ventiltyp, string fabrikat, string anlaggningsnr)
        {
            List<VentilCL> vl = new List<VentilCL>();

            CReparator cr = new CReparator();

            int identOK = cr.checkIdent(ident);

            if (identOK == -1)
            {
                VentilCL v = new VentilCL();
                v.ErrCode = -10;
                v.ErrMessage = "Ogiltigt login";
                vl.Add(v);
                return vl;
            }


            string sSql = " SELECT v.ventil_id, v.ventilkategori, v.kund_id, v.\"position\", v.fabrikat, v.ventiltyp, v.id_nr, v.pn, v.pn2, v.dn, v.dn2, "
                        + " v.oppningstryck, v.stalldonstyp, v.stalldon_id_nr, v.stalldon_fabrikat, v.stalldon_artnr, v.lagesstallartyp, "
                        + " v.lagesstall_id_nr, v.lagesstall_fabrikat, v.avdelning, v.anlaggningsnr,  "
                        + "  v.forra_comment, vk.ventilkategori as ventilkategori_namn "
                        + ", v.plan, v.rum "
                        + " FROM ventil v "
                        + " join ventilkategori vk on v.ventilkategori = vk.ventilkat_id "
                        + " where v.kund_id = :pKundID "
                        + " and upper(coalesce(v.\"position\",'')) like upper(:position) "
                        + " and upper(coalesce(v.id_nr,'')) like upper(:id_nr) "
                        + " and upper(coalesce(v.ventiltyp,'')) like upper(:ventiltyp) "
                        + " and upper(coalesce(v.fabrikat,'')) like upper(:fabrikat) "
                        + " and upper(coalesce(v.anlaggningsnr,'')) like upper(:anlaggningsnr) ";

            NxParameterCollection np = new NxParameterCollection();
            np.Add("pKundID", KundID);
            np.Add("position", CCommonFunc.addWildCard(position));
            np.Add("ID_nr", CCommonFunc.addWildCard(IDnr));
            np.Add("ventiltyp", CCommonFunc.addWildCard(ventiltyp));
            np.Add("fabrikat", CCommonFunc.addWildCard(fabrikat));
            np.Add("anlaggningsnr", CCommonFunc.addWildCard(anlaggningsnr));

            string errText = "";

            DataTable dt = cdb.getData(sSql, ref errText, np);

            int errCode = -100;

            if (errText == "" && dt.Rows.Count == 0)
            {
                errText = "Det finns inga ventiler för aktuell kund";
                errCode = 0;
            }

            if (errText != "")
            {
                VentilCL v = new VentilCL();
                if (errText.Length > 2000)
                    errText = errText.Substring(1, 2000);
                v.ErrCode = errCode;
                v.ErrMessage = errText;
                vl.Add(v);
                return vl;
            }

            foreach (DataRow dr in dt.Rows)
            {
                VentilCL vr = new VentilCL();
                vr.VentilID = dr["ventil_id"].ToString();
                vr.VentilkatID = Convert.ToInt16(dr["ventilkategori"]);
                vr.Ventilkategori = dr["ventilkategori_namn"].ToString();
                vr.KundID = dr["kund_id"].ToString();
                vr.Position = dr["position"].ToString();
                vr.Fabrikat = dr["fabrikat"].ToString();
                vr.Ventiltyp = dr["ventiltyp"].ToString();
                vr.IdNr = dr["id_nr"].ToString();
                vr.Pn = dr["pn"].ToString();
                vr.Pn2 = dr["pn2"].ToString();
                vr.Dn = dr["dn"].ToString();
                vr.Dn2 = dr["dn2"].ToString();
                if (dr["oppningstryck"] == DBNull.Value)
                    vr.Oppningstryck = 0;
                else
                    vr.Oppningstryck = Convert.ToDecimal(dr["oppningstryck"]);
                vr.Stalldonstyp = dr["stalldonstyp"].ToString();
                vr.StalldonIDNr = dr["stalldon_id_nr"].ToString();
                vr.StalldonFabrikat = dr["stalldon_fabrikat"].ToString();
                vr.StalldonArtnr = dr["stalldon_artnr"].ToString();
                vr.Lagesstallartyp = dr["lagesstallartyp"].ToString();
                vr.LagesstallIDNr = dr["lagesstall_id_nr"].ToString();
                vr.LagesstallFabrikat = dr["lagesstall_fabrikat"].ToString();
                vr.Avdelning = dr["avdelning"].ToString();
                vr.Anlaggningsnr = dr["anlaggningsnr"].ToString();
                vr.Plan = dr["plan"].ToString();
                vr.Rum = dr["rum"].ToString();
                vr.ErrCode = 0;
                vr.ErrMessage = "";

                vl.Add(vr);


            }

            return vl;


        }


        public VentilCL getVentil(string ident, string ventilID, bool validateUser)
        {
            VentilCL vr = new VentilCL();

            if (validateUser)
            {
                CReparator cr = new CReparator();

                int identOK = cr.checkIdent(ident);

                if (identOK == -1)
                {
                    vr.ErrCode = -10;
                    vr.ErrMessage = "Ogiltigt login";
                    return vr;
                }
            }

            string sSql = " SELECT v.ventil_id, v.ventilkategori, v.kund_id, v.\"position\", v.fabrikat, v.ventiltyp, v.id_nr, v.pn, v.pn2, v.dn, v.dn2, "
                        + " v.oppningstryck, v.stalldonstyp, v.stalldon_id_nr, v.stalldon_fabrikat, v.stalldon_artnr, v.lagesstallartyp, "
                        + " v.lagesstall_id_nr, v.lagesstall_fabrikat, v.avdelning,  v.anlaggningsnr,  "
                        + " v.forra_comment, vk.ventilkategori as ventilkategori_namn "
                        + " , plan, rum "
                        + " FROM ventil v "
                        + " join ventilkategori vk on v.ventilkategori = vk.ventilkat_id "
                        + " where ventil_id = :pVentilID ";

            NxParameterCollection np = new NxParameterCollection();
            np.Add("pVentilID", ventilID);

            string errText = "";

            DataTable dt = cdb.getData(sSql, ref errText, np);

            int errCode = -100;

            if (errText == "" && dt.Rows.Count == 0)
            {
                errText = "Felaktigt ventilID";
                errCode = 0;
            }


            if (errText != "")
            {

                if (errText.Length > 2000)
                    errText = errText.Substring(1, 2000);
                vr.ErrCode = errCode;
                vr.ErrMessage = errText;
                return vr;
            }


            DataRow dr = dt.Rows[0];

            vr.VentilID = dr["ventil_id"].ToString();
            vr.VentilkatID = Convert.ToInt16(dr["ventilkategori"]);
            vr.Ventilkategori = dr["ventilkategori_namn"].ToString();
            vr.KundID = dr["kund_id"].ToString();
            vr.Position = dr["position"].ToString();
            vr.Fabrikat = dr["fabrikat"].ToString();
            vr.Ventiltyp = dr["ventiltyp"].ToString();
            vr.IdNr = dr["id_nr"].ToString();
            vr.Pn = dr["pn"].ToString();
            vr.Pn2 = dr["pn2"].ToString();
            vr.Dn = dr["dn"].ToString();
            vr.Dn2 = dr["dn2"].ToString();
            if (dr["oppningstryck"] == DBNull.Value)
                vr.Oppningstryck = 0;
            else
                vr.Oppningstryck = Convert.ToDecimal(dr["oppningstryck"]);
            vr.Stalldonstyp = dr["stalldonstyp"].ToString();
            vr.StalldonIDNr = dr["stalldon_id_nr"].ToString();
            vr.StalldonFabrikat = dr["stalldon_fabrikat"].ToString();
            vr.StalldonArtnr = dr["stalldon_artnr"].ToString();
            vr.Lagesstallartyp = dr["lagesstallartyp"].ToString();
            vr.LagesstallIDNr = dr["lagesstall_id_nr"].ToString();
            vr.LagesstallFabrikat = dr["lagesstall_fabrikat"].ToString();
            vr.Avdelning = dr["avdelning"].ToString();
            vr.Plan = dr["plan"].ToString();
            vr.Rum = dr["rum"].ToString();
            vr.Anlaggningsnr = dr["anlaggningsnr"].ToString();                
            vr.ErrCode = 0;
            vr.ErrMessage = "";

            return vr;
        }



        public List<VentilKategoriCL> getVentilKategoriers(string ident)
        {
            List<VentilKategoriCL> vc = new List<VentilKategoriCL>();
            CReparator cr = new CReparator();

            int identOK = cr.checkIdent(ident);

            if (identOK == -1)
            {
                VentilKategoriCL v = new VentilKategoriCL();
                v.ErrCode = -10;
                v.ErrMessage = "Ogiltigt login";
                vc.Add(v);
                return vc;
            }


            string sSql = " SELECT ventilkat_id, ventilkategori "
                        + " FROM ventilkategori "
                        + " order by ventilkategori ";



            string errText = "";

            DataTable dt = cdb.getData(sSql, ref errText);

            int errCode = -100;

            if (errText == "" && dt.Rows.Count == 0)
            {
                errText = "Inga ventilkategorier finns tillgängliga";
                errCode = 0;
            }


            if (errText != "")
            {

                if (errText.Length > 2000)
                    errText = errText.Substring(1, 2000);

                VentilKategoriCL v = new VentilKategoriCL();
                v.ErrCode = errCode;
                v.ErrMessage = errText;
                vc.Add(v);
                return vc;
            }


            foreach (DataRow dr in dt.Rows)
            {
                VentilKategoriCL v = new VentilKategoriCL();
                v.VentilkatID = Convert.ToInt16(dr["ventilkat_id"]);
                v.Ventilkategori = dr["ventilkategori"].ToString();
                v.ErrCode = 0;
                v.ErrMessage = "";

                vc.Add(v);
            }
            return vc;
        }

        private int validateVentilKategori(int ventKatID)
        {
            string sSql = " Select count(*) as antal "
                        + " from ventilkategori "
                        + " where ventilkat_id = :pVentKatID ";

            NxParameterCollection np = new NxParameterCollection();
            np.Add("pVentKatID", ventKatID);

            string errText = "";

            DataTable dt = cdb.getData(sSql, ref errText, np);

            return Convert.ToInt16(dt.Rows[0][0]); 
        }

        private int duplicateExists(VentilCL v)
        {

            string sSql = " select count(*) as antal "
                        + " from ventil "
                        + " where kund_id = :kund_id "
                        + " and \"position\" = :position ";
            if (v.VentilID != null)
                sSql += " and ventil_id <> :ventil_id ";

            NxParameterCollection np = new NxParameterCollection();
            np.Add("kund_id", v.KundID);
            np.Add("position", v.Position);
            if (v.VentilID != null)
                np.Add("ventil_id", v.VentilID);

            string err = "";
            DataTable dt = cdb.getData(sSql, ref err, np);

            if (dt.Rows.Count == 0)
                return 0;

            return Convert.ToInt32(dt.Rows[0]["antal"]);

        }




        private int validateVentil(VentilCL v)
        {
            if (validateVentilKategori(v.VentilkatID) == 0)
                return -1;

            CKund cKund = new CKund();
            if (cKund.validateKund(v.KundID) == 0)
                return -2;
            if (v.forceSave == false)
            {
                if (duplicateExists(v) > 0)
                    return -3;
            }
            if (v.insideDiameter != 0 && v.outsideDiameter != 0)
            {
                if (v.insideDiameter > v.outsideDiameter)
                    return -4;
            }
            return 1;                       
        }

        private string getVentilInsertSQL()
        {

            string sSql = " insert into ventil ( ventil_id, ventilkategori, kund_id, \"position\", fabrikat "
                     + " , ventiltyp, id_nr, pn, pn2, dn "
                    + " , dn2, oppningstryck, stalldonstyp, stalldon_id_nr, stalldon_fabrikat "
                    + " , stalldon_artnr, lagesstallartyp, lagesstall_id_nr, lagesstall_fabrikat, avdelning "
                    + " , anlaggningsnr, reg, regdat "
                    + " , changed, plan, rum, insideDiameter, outsideDiameter )  "
                    + "  values ( :pventil_id, :pventilkategori, :pkund_id, :pposition, :pfabrikat "
                    + " , :pventiltyp, :pid_nr, :ppn, :ppn2, :pdn "
                    + " , :pdn2, :poppningstryck, :pstalldonstyp, :pstalldon_id_nr, :pstalldon_fabrikat "
                    + " , :pstalldon_artnr, :plagesstallartyp, :plagesstall_id_nr, :plagesstall_fabrikat, :pavdelning "
                    + " , :panlaggningsnr, :preg, :pregdat "
                    + " , :pchanged, :pplan, :prum, :insideDiameter, :outsideDiameter  )";  




            return sSql;

        }

        private string getVentilUpdateSQL()
        {
            string sSql = " update ventil "
                         + " set ventilkategori = :pventilkategori "                         
                         + ", kund_id = :pkund_id "
                         + ", \"position\" = :pposition "
                         + ", fabrikat = :pfabrikat "
                         + ", ventiltyp = :pventiltyp "
                         + ", id_nr = :pid_nr "
                         + ", pn = :ppn "
                         + ", pn2 = :ppn2 "
                         + ", dn = :pdn "
                         + ", dn2 = :pdn2 "
                         + ", oppningstryck = :poppningstryck "
                         + ", stalldonstyp = :pstalldonstyp "
                         + ", stalldon_id_nr = :pstalldon_id_nr "
                         + ", stalldon_fabrikat = :pstalldon_fabrikat "
                         + ", stalldon_artnr = :pstalldon_artnr "
                         + ", lagesstallartyp = :plagesstallartyp "
                         + ", lagesstall_id_nr = :plagesstall_id_nr "
                         + ", lagesstall_fabrikat = :plagesstall_fabrikat "
                         + ", avdelning = :pavdelning "
                         + ", anlaggningsnr = :panlaggningsnr "
                         + ", Uppdaterat = :pUppdaterat "
                         + ", Uppdat_dat = :pUppdat_dat "
                         + ", changed = :pchanged "
                         + ", plan = :pplan "
                         + ", rum = :prum "
                         + ", insideDiameter = :insideDiameter "
                         + ", outsideDiameter = :outsideDiameter "
                         + " where ventil_id = :pventil_id ";
            return sSql;

        }


        private void setParameters (NxParameterCollection np, VentilCL v, bool bNew, string sAnvID  )
        {
             
            string sVar = "";
            if (bNew)
            {
                DateTime lDt = System.DateTime.Now;
                np.Add("pregdat", lDt);
                sVar = sAnvID;
                if (sVar.Length == 0)
                    np.Add("preg", System.DBNull.Value);
                else
                    np.Add("preg", sVar);
            }
            sVar = v.Anlaggningsnr;
                np.Add("panlaggningsnr", sVar);
            sVar = v.Avdelning;
                np.Add("pavdelning", sVar);
            sVar = v.LagesstallFabrikat;
                np.Add("plagesstall_fabrikat", sVar);
            sVar = v.LagesstallIDNr;
                np.Add("plagesstall_id_nr", sVar);
            sVar = v.Lagesstallartyp;
                np.Add("plagesstallartyp", sVar);
            sVar = v.StalldonArtnr;
                np.Add("pstalldon_artnr", sVar);
            sVar = v.StalldonFabrikat;
                np.Add("pstalldon_fabrikat", sVar);
            sVar = v.StalldonIDNr;
                np.Add("pstalldon_id_nr", sVar);
            sVar = v.Stalldonstyp;
                np.Add("pstalldonstyp", sVar);
            Decimal d = v.Oppningstryck;
            if (d == 0)
                np.Add("poppningstryck", System.DBNull.Value);
            else
                np.Add("poppningstryck", d);
            sVar = v.Dn2;
                np.Add("pdn2", sVar);
            sVar = v.Dn;
                np.Add("pdn", sVar);
            sVar = v.Pn2;
                np.Add("ppn2", sVar);
            sVar = v.Pn;
                np.Add("ppn", sVar);
            sVar = v.IdNr;
                np.Add("pid_nr", sVar);
            sVar = v.Ventiltyp;
                np.Add("pventiltyp", sVar);
            sVar = v.Fabrikat;
                np.Add("pfabrikat", sVar);
            sVar = v.Position;
                np.Add("pposition", sVar);
            sVar = v.KundID;
                np.Add("pkund_id", sVar);            
            np.Add("pventilkategori", v.VentilkatID);
            np.Add("pventil_id", v.VentilID);
            if (!bNew)
            {
                sVar = sAnvID;
                    np.Add("pUppdaterat", sVar);
                DateTime ldtNow = System.DateTime.Now;
                np.Add("pUppdat_dat", ldtNow);
            }            
            np.Add("pchanged", System.DateTime.Now);
            np.Add("pplan", v.Plan);
            np.Add("prum", v.Rum);
            // 2017-06-21 KJBO
            if (v.insideDiameter == 0)
                np.Add("insideDiameter", System.DBNull.Value);
            else
                np.Add("insideDiameter", v.insideDiameter);
            if (v.outsideDiameter == 0)
                np.Add("outsideDiameter", System.DBNull.Value);
            else
                np.Add("outsideDiameter", v.outsideDiameter);
            
        }


        public string updateForraComment( string ventilID, string ovr_komment)
        {
            string sSql = "update ventil "
                        + " set forra_comment = '" + ovr_komment + "' "
                        + " where ventil_id = '" + ventilID + "' ";
            string err = "";
            cdb.updateData(sSql, ref err);
            return err;
        }





        public VentilCL saveVentil(string ident, VentilCL v)
        {
            VentilCL vc = new VentilCL();
            CReparator cr = new CReparator();
            int identOK = cr.checkIdent(ident);

            if (identOK == -1)
            {                
                vc.ErrCode = -10;
                vc.ErrMessage = "Ogiltigt login";
                return vc;
            }

            int iRes = validateVentil(v);
            if (iRes == -1)
            {
                vc.ErrCode = -1;
                vc.ErrMessage = "Felaktig ventilkategori";
                return vc;
            }
            if (iRes == -2)
            {
                vc.ErrCode = -1;
                vc.ErrMessage = "Felaktigt kundID";
                return vc;
            }

            if (iRes == -3)
            {
                vc.ErrCode = 101;
                vc.ErrMessage = "Det finns redan en ventil med detta positionsnr";
                return vc;
            }

            if (iRes == -4)
            {
                vc.ErrCode = -1;
                vc.ErrMessage = "Ytterdiameter måste vara större än innerdiameter";
                return vc;
            }

            ReparatorCL r = cr.getReparator(ident);

            

            string sSql = "";
            bool bNew = false;

            // This is a new ventil
            if (v.VentilID == null)
            {
                string ventilID = Guid.NewGuid().ToString();
                v.VentilID = ventilID;
                sSql = getVentilInsertSQL();
                bNew = true;
            }

            else
                sSql = getVentilUpdateSQL();


            NxParameterCollection np = new NxParameterCollection();
            setParameters(np, v, bNew, r.AnvID);

            string errText = "";

            int iRc = cdb.updateData(sSql, ref errText, np);

            if (errText != "")
            {

                if (errText.Length > 2000)
                    errText = errText.Substring(1, 2000);

                v.ErrCode = -100;
                v.ErrMessage = errText;
                return v;
            }

            // 2016-10-17 KJBO
            CServRad csr = new CServRad();
            csr.updateFromVentil2(v.VentilID);



            return getVentil(ident, v.VentilID);
        }

        public int validateVentilID(string ventilID, ref string err)
        {
            string sSql = " select count(*) as antal "
                        + " from ventil v "
                        + " where v.ventil_id = :pVentilID";

            NxParameterCollection np = new NxParameterCollection();
            np.Add("pVentilID", ventilID);

            DataTable dt = cdb.getData(sSql, ref err, np);
            return Convert.ToInt16(dt.Rows[0][0]);

        }


    }
}