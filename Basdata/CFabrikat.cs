using NexusDB.ADOProvider;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace SManApi.Basdata
{
    public class CFabrikat
    {

        /// <summary>
        /// Validates and saves fabrikat
        /// </summary>
        CDB cdb = null;
        public CFabrikat()
        {
            cdb = new CDB();
        }

        private string getInsertSQL()
        {
            string sSql = "insert into fabrikat (fabrikat, reg, regdat) "
                        + " values(:fabrikat, :reg, :regdat) ";
            return sSql;
        }



        private string fabrikatExists(string fabrikat)
        {
            string sSql = " select count(*) antal "
                        + " from fabrikat "
                        + " where fabrikat = :fabrikat ";
            string errTxt = "";
            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("fabrikat", fabrikat);
            DataTable dt = cdb.getData(sSql, ref errTxt, pc);
            if (errTxt != "")
                return errTxt;
            if (dt.Rows.Count == 1)
                return dt.Rows[0]["antal"].ToString();
            return "";
        }



        /// <summary>
        /// Save a new fabrikat to the database
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        /// 2019-04-03 KJBO
        public FabrikatCL saveFabrikat(string ident, FabrikatCL f)
        {
            FabrikatCL resp = new FabrikatCL();
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
            if (f.Fabrikat == "")
            {
                resp.ErrCode = -1;
                resp.ErrMessage = "Fabrikat måste anges";
                return resp;
            }

            string exists = fabrikatExists(f.Fabrikat);
            if (exists != "0" && exists != "1")
            {
                resp.ErrCode = -100;
                resp.ErrMessage = "Fel vid kontroll av fabrikat. Felmeddelande : " + exists;
                if (resp.ErrMessage.Length > 2000)
                    resp.ErrMessage = resp.ErrMessage.Substring(1, 2000);
                return resp;
            }

            if (exists == "1")
            {
                resp.ErrCode = 100;
                resp.ErrMessage = "Fabrikat finns redan registrerat";
                return resp;
            }

            string sSql = getInsertSQL();
            NxParameterCollection pc = new NxParameterCollection();            
            pc.Add("fabrikat", f.Fabrikat);
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
            List<FabrikatCL> respList = cbv.getFabrikat("", f.Fabrikat);
            if (respList.Count == 1)
                return respList[0];
            return resp;

        }



    }
}