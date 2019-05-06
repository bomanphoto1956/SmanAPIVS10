using NexusDB.ADOProvider;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace SManApi.Basdata
{
    public class CDn
    {

        /// <summary>
        /// Validates and saves DN values
        /// </summary>
        CDB cdb = null;
        public CDn()
        {
            cdb = new CDB();
        }


        private string getInsertSql()
        {
            string sSql = "insert into dn(dn, reg, regdat) "
                        + " values(:dn, :reg, :regdat) ";
            return sSql;
        }

        private string dnExists(string dn)
        {
            string sSql = " select count(*) antal "
                        + " from dn "
                        + " where dn = :dn ";
            string errTxt = "";
            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("dn", dn);
            DataTable dt = cdb.getData(sSql, ref errTxt, pc);
            if (errTxt != "")
                return errTxt;
            if (dt.Rows.Count == 1)
                return dt.Rows[0]["antal"].ToString();
            return "";
        }

        /// <summary>
        /// Saves a new DN value to the database
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public DnCL saveDn(string ident, DnCL d)
        {
            DnCL resp = new DnCL();
            resp.ErrCode = 0;
            resp.ErrMessage = "";
            CReparator cr = new CReparator();
            int identOK = cr.checkIdent(ident);

            if (identOK == -1)
            {
                resp.ErrCode = -10;
                resp.ErrMessage = "Ogiltigt login";
                return resp;
            }

            if (d.Dn == "")
            {
                resp.ErrCode = -1;
                resp.ErrMessage = "Dn måste anges";
                return resp;
            }

            string exists = dnExists(d.Dn);
            if (exists != "0" && exists != "1")
            {
                resp.ErrCode = -100;
                resp.ErrMessage = "Fel vid kontroll av DN. Felmeddelande : " + exists;
                if (resp.ErrMessage.Length > 2000)
                    resp.ErrMessage = resp.ErrMessage.Substring(1, 2000);
                return resp;
            }

            if (exists == "1")
            {
                resp.ErrCode = 100;
                resp.ErrMessage = "Dn finns redan registrerat";
                return resp;
            }

            string sSql = getInsertSql();
            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("dn", d.Dn);
            pc.Add("reg", "System");
            pc.Add("regdat", System.DateTime.Now);
            string errText = "";
            int iRc = cdb.updateData(sSql, ref errText, pc);
            if (errText != "")
            {

                if (errText.Length > 2000)
                    errText = errText.Substring(1, 2000);

                resp.ErrCode = -100;
                resp.ErrMessage = errText;
                return resp;
            }
            CComboValues cbv = new CComboValues();
            List<DnCL> respList = cbv.getDn("", d.Dn);
            if (respList.Count == 1)
                return respList[0];
            return resp;

        }



    }
}