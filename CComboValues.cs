using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace SManApi
{

    
    public class CComboValues
    {

        CDB cdb = null;

        public CComboValues()
        {
            cdb = new CDB();
        }

        public List<FabrikatCL> getFabrikat(string ident)
        {
            List<FabrikatCL> lf = new List<FabrikatCL>();

            CReparator cr = new CReparator();
            int identOK = cr.checkIdent(ident);

            if (identOK == -1)
            {
                FabrikatCL f = new FabrikatCL();                
                f.ErrCode = -10;
                f.ErrMessage = "Ogiltigt login";
                lf.Add(f);
                return lf;
            }

            string sSql = " select fabrikat "
                        + " from fabrikat "
                        + " order by fabrikat ";

                        string errText = "";

            DataTable dt = cdb.getData(sSql, ref errText);

            int errCode = -100;

            if (errText == "" && dt.Rows.Count == 0)
            {
                errText = "Inga fabrikat finns tillgängliga";
                errCode = 0;
            }

            if (errText != "")
            {

                if (errText.Length > 2000)
                    errText = errText.Substring(1, 2000);
                FabrikatCL f = new FabrikatCL();                
                f.ErrCode = errCode;
                f.ErrMessage = errText;
                lf.Add(f);
                return lf;
            }


            foreach (DataRow dr in dt.Rows)
            {
                FabrikatCL f = new FabrikatCL();
                f.Fabrikat = dr["fabrikat"].ToString();
                f.ErrCode = 0;
                f.ErrMessage = "";
                lf.Add(f);
            }
            return lf;
        }


        public List<DnCL> getDn(string ident)
        {
            List<DnCL> ld = new List<DnCL>();
            CReparator cr = new CReparator();
            int identOK = cr.checkIdent(ident);

            if (identOK == -1)
            {
                DnCL d = new DnCL();
                
                d.ErrCode = -10;
                d.ErrMessage = "Ogiltigt login";
                ld.Add(d);                
                return ld;
            }

            string sSql = " select dn "
                        + " from dn "
                        + " order by dn ";
                        string errText = "";

            DataTable dt = cdb.getData(sSql, ref errText);
            
            int errCode = -100;

            if (errText == "" && dt.Rows.Count == 0)
            {
                errText = "Inga dn finns tillgängliga";
                errCode = 0;
            }

            if (errText != "")
            {

                if (errText.Length > 2000)
                    errText = errText.Substring(1, 2000);                
                DnCL d = new DnCL();
                d.ErrCode = errCode;
                d.ErrMessage = errText;
                ld.Add(d);
                return ld;
            }

            foreach (DataRow dr in dt.Rows)
            {
                DnCL d = new DnCL();
                d.Dn = dr["dn"].ToString();
                d.ErrCode = 0;
                d.ErrMessage = "";
                ld.Add(d);
            }
            return ld;
        }


         public List<PnCL> getPn(string ident)
        {
            List<PnCL> lp = new List<PnCL>();
            CReparator cr = new CReparator();
            int identOK = cr.checkIdent(ident);

            if (identOK == -1)
            {                
                PnCL p = new PnCL();
                p.ErrCode = -10;
                p.ErrMessage = "Ogiltigt login";
                lp.Add(p);                
                return lp;
            }

            string sSql = " select pn "
                        + " from pn "
                        + " order by pn ";
                        
            string errText = "";

            DataTable dt = cdb.getData(sSql, ref errText);
            
            int errCode = -100;

            if (errText == "" && dt.Rows.Count == 0)
            {
                errText = "Inga pn finns tillgängliga";
                errCode = 0;
            }

            if (errText != "")
            {

                if (errText.Length > 2000)
                    errText = errText.Substring(1, 2000);
                PnCL p = new PnCL();
                p.ErrCode = errCode;
                p.ErrMessage = errText;
                lp.Add(p);
                return lp;
            }


            foreach (DataRow dr in dt.Rows)
            {                
                PnCL p = new PnCL();
                p.Pn = dr["pn"].ToString();
                p.ErrCode = 0;
                p.ErrMessage = "";
                lp.Add(p);
            }
            return lp;
        }

        /// <summary>
        /// Return all salarts either for servicedetalj or serviceorder 
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="forServiceDetalj"></param>
        /// <returns>List of salart or error code</returns>
        public List<SalartCL> getSalart(string ident, bool forServiceDetalj)
        {
            List<SalartCL> ls = new List<SalartCL>();
            CReparator cr = new CReparator();
            int identOK = cr.checkIdent(ident);

            if (identOK == -1)
            {
                SalartCL s = new SalartCL();
                s.ErrCode = -10;
                s.ErrMessage = "Ogiltigt login";
                ls.Add(s);
                return ls;
            }
            

            string sSql = " SELECT SalartID, SalartTypeID, SalartName, enhet "
                        + " FROM Salart ";
            if (forServiceDetalj)
                sSql += "where SalartTypeID = 1";
            else
                sSql += "where SalartTypeID > 1";
                        

            string errText = "";

            DataTable dt = cdb.getData(sSql, ref errText);

            int errCode = -100;

            if (errText == "" && dt.Rows.Count == 0)
            {
                errText = "Lönearter finns tillgängliga";
                errCode = 0;
            }

            if (errText != "")
            {

                if (errText.Length > 2000)
                    errText = errText.Substring(1, 2000);
                SalartCL s = new SalartCL();
                s.ErrCode = errCode;
                s.ErrMessage = errText;
                ls.Add(s);
                return ls;
            }


            foreach (DataRow dr in dt.Rows)
            {
                SalartCL s = new SalartCL();
                s.SalartID = Convert.ToInt32(dr["SalartID"]);
                s.SalartTypeID = Convert.ToInt32(dr["SalartTypeID"]);
                s.SalartName = dr["SalartName"].ToString();
                s.Unit = dr["enhet"].ToString();
                s.ErrCode = 0;
                s.ErrMessage = "";
                ls.Add(s);
            }
            return ls;

        }



    }
}