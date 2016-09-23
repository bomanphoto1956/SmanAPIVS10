using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace SManApi
{
    public class CMisc
    {

        private const string apiVersion = "1.01";

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


    }
}