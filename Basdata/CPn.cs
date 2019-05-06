using NexusDB.ADOProvider;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace SManApi.Basdata
{
    public class CPn
    {

        CDB cdb = null;
        public CPn()
        {
            cdb = new CDB();
        }


        private string getInsertSql()
        {
            string sSql = "insert into pn(pn, reg, regdat) "
                        + " values(:pn, :reg, :regdat) ";
            return sSql;
        }


        private string pnExists(string pn)
        {
            string sSql = " select count(*) antal "
                        + " from pn "
                        + " where pn = :pn ";
            string errTxt = "";
            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("pn", pn);
            DataTable dt = cdb.getData(sSql, ref errTxt, pc);
            if (errTxt != "")
                return errTxt;
            if (dt.Rows.Count == 1)
                return dt.Rows[0]["antal"].ToString();
            return "";
        }


        /// <summary>
        /// Saves a new PN to the database
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        /// 2019-04-04 KJBO
        public PnCL savePn(string ident, PnCL p)
        {
            PnCL resp = new PnCL();
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
            if (p.Pn == "")
            {
                resp.ErrCode = -1;
                resp.ErrMessage = "Pn måste anges";
                return resp;
            }

            string exists = pnExists(p.Pn);
            if (exists != "0" && exists != "1")
            {
                resp.ErrCode = -100;
                resp.ErrMessage = "Fel vid kontroll av PN. Felmeddelande : " + exists;
                if (resp.ErrMessage.Length > 2000)
                    resp.ErrMessage = resp.ErrMessage.Substring(1, 2000);
                return resp;
            }

            if (exists == "1")
            {
                resp.ErrCode = 100;
                resp.ErrMessage = "Pn finns redan registrerat";
                return resp;
            }

            string sSql = getInsertSql();
            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("pn", p.Pn);
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
            List<PnCL> respList = cbv.getPn("", p.Pn);
            if (respList.Count == 1)
                return respList[0];
            return resp;
        }

    }
}




