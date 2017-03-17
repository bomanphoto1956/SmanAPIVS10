using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using NexusDB.ADOProvider;

namespace SManApi
{
    public class CServiceHuvud
    {

        CDB cdb = null;
        public CServiceHuvud()
        {
            cdb = new CDB();
        }



        /// <summary>
        /// Get one servicehuvud and return
        /// </summary>
        /// <param name="ident">identity</param>
        /// <param name="vartOrdernr">vart_ordernr</param>
        /// <returns></returns>
        public ServiceHuvudCL getServHuv(string ident, string vartOrdernr)
        {
            CReparator cr = new CReparator();

            int identOK = cr.checkIdent(ident);

            if (identOK == -1)
            {
                ServiceHuvudCL sh2 = new ServiceHuvudCL();
                sh2.ErrCode = -10;
                sh2.ErrMessage = "Ogiltigt login";
                return sh2;                
            }

            string sSql = " SELECT sh.vart_ordernr, sh.ert_ordernr, sh.kund, c.kund as kundnamn, sh.datum, sh.orderAdmin, "
                        + " r.reparator, sh.allrep "
                        + " FROM ServiceHuvud sh "
                        + " join kund c on sh.kund = c.kund_id "
                        + " left outer join reparator r on sh.orderAdmin = r.AnvID "
                        + " where godkand = false "
                        + " and posttyp = 1 "
                        + " and OpenForApp = true "
                        + " and sh.vart_ordernr = :pVartOrdernr";

            NxParameterCollection np = new NxParameterCollection();

            np.Add("pVartOrdernr", vartOrdernr);


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
                ServiceHuvudCL sh2 = new ServiceHuvudCL();
                sh2.ErrCode = errCode;
                sh2.ErrMessage = errText;
                return sh2;
            }

            DataRow dr = dt.Rows[0];
            
            ServiceHuvudCL sh = new ServiceHuvudCL();
            sh.VartOrdernr = dr["vart_ordernr"].ToString();
            sh.ErtOrdernr = dr["ert_ordernr"].ToString();
            sh.Kund = dr["kund"].ToString();
            sh.KundNamn = dr["kundnamn"].ToString();
            sh.OrderDatum = Convert.ToDateTime(dr["datum"]);
            sh.OrderAdminID = dr["orderAdmin"].ToString();
            sh.OrderAdminNamn = dr["reparator"].ToString();
            sh.ErrCode = 0;
            sh.ErrMessage = "";
            return sh;
            
            
        }



        // Get all open ServiceHuvud for a user
        public List<ServiceHuvudCL> getServHuvForUser(string ident)
        {
            List<ServiceHuvudCL> shList = new List<ServiceHuvudCL>();

            CReparator cr = new CReparator();

            int identOK = cr.checkIdent(ident);

            if (identOK == -1)
            {
                ServiceHuvudCL sh = new ServiceHuvudCL();
                sh.ErrCode = -10;
                sh.ErrMessage = "Ogiltigt login";
                shList.Add(sh);
                return shList;
            }



            string sSql = " SELECT sh.vart_ordernr, sh.ert_ordernr, sh.kund, c.kund as kundnamn, sh.datum, sh.orderAdmin, "
                        + " r.reparator, sh.allrep "
                        + " FROM ServiceHuvud sh "
                        + " join kund c on sh.kund = c.kund_id "
                        + " left outer join reparator r on sh.orderAdmin = r.AnvID "
                        + " where godkand = false "
                        + " and posttyp = 1 "
                        + " and OpenForApp = true "
                        + " and (allRep = true "
                        + " or orderAdmin = ( select min(a.AnvID) "
                                            + " from Authenticate a "
                                            + " where a.ident = :pIdent) "
                        + " or exists ( select 'x' "
                                    + " from shReparator shr "
                                    + " where shr.vart_ordernr = sh.vart_ordernr "
                                    + " and shr.AnvID = ( select min(a.AnvID) "
                                    + " from Authenticate a "
                                    + " where a.ident = :pIdent))) ";

            NxParameterCollection np = new NxParameterCollection();

            np.Add("pIdent", ident);


            string errText = "";
            DataTable dt = cdb.getData(sSql, ref errText, np);

            int errCode = -100;
            if (errText == "" && dt.Rows.Count == 0)
            {
                errText = "Det finns inga tillgängliga order för aktuell användare";
                errCode = -10;
            }

            if (errText != "")
            {
                if (errText.Length > 2000)
                    errText = errText.Substring(1, 2000);
                ServiceHuvudCL sh = new ServiceHuvudCL();
                sh.ErrCode = errCode;
                sh.ErrMessage = errText;
                shList.Add(sh);
                return shList;
            }
            
            foreach (DataRow dr in dt.Rows)
            {
                ServiceHuvudCL sh = new ServiceHuvudCL();
                sh.VartOrdernr = dr["vart_ordernr"].ToString();
                sh.ErtOrdernr = dr["ert_ordernr"].ToString();
                sh.Kund = dr["kund"].ToString();
                sh.KundNamn = dr["kundnamn"].ToString();
                sh.OrderDatum = Convert.ToDateTime(dr["datum"]);
                sh.OrderAdminID = dr["orderAdmin"].ToString();
                sh.OrderAdminNamn = dr["reparator"].ToString();
                sh.ErrCode = 0;
                sh.ErrMessage = "";

                shList.Add(sh);
            }

            return shList;
        }


        public int validateVartOrdernr(string vartOrdernr, ref string err)
        {
            string sSql = " SELECT count(*) as antal "
                        + " FROM ServiceHuvud "
                        + " where vart_ordernr = :pVartOrdernr ";

            NxParameterCollection np = new NxParameterCollection();
            np.Add("pVartOrdernr", vartOrdernr);
            
            DataTable dt = cdb.getData(sSql, ref err, np);

            return Convert.ToInt16(dt.Rows[0][0]);           
        }

        public DataTable validateOrderOpenGodkand(string vart_ordernr)
        {
            string sSql = " select OpenForApp, Godkand "
                        + " from ServiceHuvud "
                        + " where vart_ordernr = :vart_ordernr ";

            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("vart_ordernr", vart_ordernr);

            string err = "";

            return cdb.getData(sSql, ref err, pc);

        }
        


        /// <summary>
        /// Check if an order is open for editing
        /// The return value is a string, 1 = open, -1 = closed or an error message
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="VartOrdernr"></param>
        /// <returns>1 - Open -1 - Closed or an error message</returns>
        public string isOpen(string ident, string VartOrdernr)
        {

            CReparator cr = new CReparator();

            int identOK = cr.checkIdent(ident);

            if (identOK == -1)            
                return "Ogiltigt login";            


            string sSql = " select Coalesce(OpenForApp, false) as OpenForApp, "
                        + " Coalesce(Godkand, true) as Godkand "
                        + " from servicehuvud "
                        + " where vart_ordernr = :vart_ordernr ";

            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("vart_ordernr", VartOrdernr);

            string errText = "";
            DataTable dt = cdb.getData(sSql, ref errText, pc);

            int errCode = -100;
            if (errText == "" && dt.Rows.Count == 0)
            {
                errText = "Det finns ingen order med aktuellt ordernr";
                errCode = -10;
                return errText;
            }

            if (errText != "")
            {
                if (errText.Length > 2000)
                    errText = errText.Substring(1, 2000);
                ServiceHuvudCL sh = new ServiceHuvudCL();
                sh.ErrCode = errCode;
                sh.ErrMessage = errText;                
            }


            DataRow dr = dt.Rows[0];
            Boolean bOpenForApp = Convert.ToBoolean(dr["OpenForApp"]);
            Boolean bGodkand = Convert.ToBoolean(dr["Godkand"]);
            if (bOpenForApp && !bGodkand)
                return "1";
            return "-1";
        }



        



    }
}