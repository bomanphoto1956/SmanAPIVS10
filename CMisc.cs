using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using NexusDB.ADOProvider;

namespace SManApi
{
    public class CMisc
    {

        private const string apiVersion = "1.02";

        /*
         Version 0.96:
         2016-06-14 Logging enabled. Bug fixes.
         
         */

        /// <summary>
        /// Returns database and API versions
        /// </summary>
        /// <param name="ident"></param>
        /// <returns></returns>
        public VersionCL getVersion(string ident)
        {
            CReparator cr = new CReparator();
            int identOK = cr.checkIdent(ident);

            VersionCL v = new VersionCL();
            if (identOK == -1)
            {
                v.ErrCode = -10;
                v.ErrMessage = "Ogiltigt login";
                v.dbVersion = 0;
                v.APIVersion = "";
                return v;
            }

            string sSql = " SELECT db_version "
                        + " FROM \"Version\" ";
            string errSt = "";
            CDB cdb = new CDB();
            DataTable dt = cdb.getData(sSql, ref errSt);

            if (errSt != "")
            {
                if (errSt.Length > 2000)
                    errSt = errSt.Substring(1, 2000);
                v.dbVersion = 0;
                v.APIVersion = "";
                v.ErrCode = -100;
                v.ErrMessage = "Databasfel : " + errSt;
                return v;
            }

            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                v.dbVersion = Convert.ToInt32(dr["db_version"]);
                v.APIVersion = apiVersion;
                v.ErrCode = 0;
                v.ErrMessage = "";
                return v;
            }

            v.dbVersion = 0;
            v.APIVersion = "";
            v.ErrCode = 0;
            v.ErrMessage = "Ingen information tillgänglig";

            return v;
            
        }
        
        /// <summary>
        /// Get the current timeRegVersion
        /// can be either 1 or 2
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="vartOrdernr"></param>
        /// <returns>Version or -1 for invalid ident or -2 for database error 
        /// (no more error description is available for this function</returns>
        public int getTimeRegVersion(string ident, string vartOrdernr)
        {
            // Check identity
            CReparator cr = new CReparator();
            int identOK = cr.checkIdent(ident);

            if (identOK == -1)
                return -1;

            // Check if any either tidrapp (version1) or timeReport2 (version 2) that is available
            string sSql = " select 1 version "
                        + " from tidrapp "
                        + " where vart_ordernr = :vart_ordernr "
                        + " union "
                        + " SELECT 2 "
                        + " FROM timeReport2 "
                        + " where vart_ordernr = :vart_ordernr ";
            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("vart_ordernr", vartOrdernr);            
            CDB cdb = new CDB();
            string errSt = "";

            // Get the result (or error)
            DataTable dt = cdb.getData(sSql, ref errSt,pc);

            // If any error then return -2
            if (errSt != "")
            {
                return -2;
            }

            // count result rows (shall be either 1 or 0)
            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                return Convert.ToInt16(dr["version"]);
            }

            // If no rows was found above then check the timeRegVersion setting in the version table
            sSql = sSql = " SELECT timeRegVersion "
                        + " FROM \"Version\" ";

            errSt = "";
            dt = cdb.getData(sSql, ref errSt);

            // Error handling
            if (errSt != "")
            {
                return -2;
            }

            // If row is found....
            if (dt.Rows.Count > 0)
                return Convert.ToInt16(dt.Rows[0]["timeRegVersion"]);

            // If nothing is found then we are on version 1
            return 1;



        }


    }
}