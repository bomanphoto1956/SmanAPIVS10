using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NexusDB.ADOProvider;
using System.Data;


namespace SManApi.Gasket
{
    public class CWorkingCost
    {
        CDB cdb = null;
        public CWorkingCost()
        {
            cdb = new CDB();
        }


        /// <summary>
        /// Get working costs (and cuttingMargin)
        /// Return always one row. If nothing is stored then 
        /// the return value in in ErrCode will be -100. As this
        /// error is expected the first time something is stored this
        /// has to be taken care of in the calling code.
        /// All other errors shall be shown to the user
        /// </summary>
        /// <param name="ident"></param>
        /// <returns></returns>
        /// 2018-08-17 KJBO
        public gWorkingCostCL getWorkingCosts(string ident)
        {
            gWorkingCostCL wc = new gWorkingCostCL();
            CReparator cr = new CReparator();
            int identOK = cr.checkIdent(ident);
            if (identOK == -1)
            {
                wc.ErrCode = -10;
                wc.ErrMessage = "Ogiltigt login";
                return wc;
            }

            string sSql = " select workingCostId, cuttingHourNet, cuttingHourSales, handlingHourNet, handlingHourSales, cuttingMargin "
                        + " from gWorkingCost ";

            string errText = "";
            DataTable dt = cdb.getData(sSql, ref errText);

            int errCode = -100;

            if (errText != "")
            {
                if (errText.Length > 2000)
                    errText = errText.Substring(1, 2000);
                wc.ErrCode = errCode;
                wc.ErrMessage = errText;
                return wc;
            }

            if (dt.Rows.Count == 0)
            {
                wc.ErrCode = -100;
                wc.ErrMessage = "Det finns ingen registrerad arbetskostnad";
                return wc;
            }

            DataRow dr = dt.Rows[0];

            wc.workingCostId = Convert.ToInt32(dr["workingCostId"]);
            wc.cuttingHourNet = Convert.ToDecimal(dr["cuttingHourNet"]);
            wc.cuttingHourSales = Convert.ToDecimal(dr["cuttingHourSales"]);
            wc.handlingHourNet = Convert.ToDecimal(dr["handlingHourNet"]);
            wc.handlingHourSales = Convert.ToDecimal(dr["handlingHourSales"]);
            wc.cuttingMargin = Convert.ToDecimal(dr["cuttingMargin"]);
            wc.ErrCode = 0;
            wc.ErrMessage = "";

            return wc;
        }


        private string getInsertSql()
        {
            return " insert into gWorkingCost (cuttingHourNet, cuttingHourSales, handlingHourNet, handlingHourSales, cuttingMargin, reg, regdate) "
                + " values(:cuttingHourNet, :cuttingHourSales, :handlingHourNet, :handlingHourSales, :cuttingMargin, :reg, :regdate) ";
        }

        private string getUpdateSql()
        {
            string sSql = " update gWorkingCost "
                        + " set cuttingHourNet = :cuttingHourNet "
                        + " , cuttingHourSales = :cuttingHourSales "
                        + " , handlingHourNet = :handlingHourNet "
                        + " , handlingHourSales = :handlingHourSales "
                        + " , cuttingMargin = :cuttingMargin "
                        + " , updat = :updat "
                        + " , updatDat = :updatDat "
                        + " where workingCostId = :workingCostId ";
            return sSql;
        }

        private void setParameters(NxParameterCollection np, gWorkingCostCL wc, ReparatorCL rep)
        {
            DateTime now = DateTime.Now;
            np.Add("workingCostId", wc.workingCostId);
            np.Add("cuttingHourNet", wc.cuttingHourNet);
            np.Add("cuttingHourSales", wc.cuttingHourSales);
            np.Add("handlingHourNet", wc.handlingHourNet);
            np.Add("handlingHourSales", wc.handlingHourSales);
            np.Add("cuttingMargin", wc.cuttingMargin);
            np.Add("reg", rep.AnvID);
            np.Add("regdate", now);
            np.Add("updat", rep.AnvID);
            np.Add("updatDat", now);
        }

        private int getLastId()
        {
            string sSql = "select coalesce(max(workingCostId),0) maxId "
                        + " from gWorkingCost ";
            string dummy = "";
            DataTable dt = cdb.getData(sSql, ref dummy);
            return Convert.ToInt32(dt.Rows[0]["maxId"]);
        }

        /// <summary>
        /// Saves a working cost.
        /// Will return the saved item with the
        /// new value in workingCostId field (if this is a new item)
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="wc"></param>
        /// <returns></returns>
        /// 2018-08-17 KJBO
        public gWorkingCostCL saveWorkingCost(string ident, gWorkingCostCL wc)
        {
            gWorkingCostCL wcReturn = new gWorkingCostCL();
            wcReturn.ErrCode = 0;
            wcReturn.ErrMessage = "";

            CReparator cr = new CReparator();
            int identOK = cr.checkIdent(ident);

            if (identOK == -1)
            {
                wcReturn.ErrCode = -10;
                wcReturn.ErrMessage = "Ogiltigt login";
                return wcReturn;
            }

            if (wc.cuttingHourNet <= 0 || wc.cuttingHourNet > 100000)
            {
                wcReturn.ErrCode = -1;
                wcReturn.ErrMessage = "Timdebitering skärtid netto utanför marginalen";
                return wcReturn;;
            }

            if (wc.cuttingHourSales <= 0 || wc.cuttingHourSales > 100000)
            {
                wcReturn.ErrCode = -1;
                wcReturn.ErrMessage = "Timdebitering skärtid brutto utanför marginalen";
                return wcReturn; ;
            }

            if (wc.cuttingHourNet > wc.cuttingHourSales)
            {
                wcReturn.ErrCode = -1;
                wcReturn.ErrMessage = "Timdebitering skärtid nettopris är lägre än bruttopris";
                return wcReturn; ;
            }

            if (wc.handlingHourNet < 0 || wc.handlingHourNet > 100000)
            {
                wcReturn.ErrCode = -1;
                wcReturn.ErrMessage = "Timpris hantering netto utanför marginalen";
                return wcReturn; ;
            }
            if (wc.handlingHourSales < 0 || wc.handlingHourSales > 100000)
            {
                wcReturn.ErrCode = -1;
                wcReturn.ErrMessage = "Timpris hantering brutto utanför marginalen";
                return wcReturn; ;
            }

            if (wc.handlingHourNet > wc.handlingHourSales)
            {
                wcReturn.ErrCode = -1;
                wcReturn.ErrMessage = "Timpris plocktid netto är högre än brutto";
                return wcReturn; ;
            }

            if (wc.cuttingMargin < 0 || wc.cuttingMargin > 1000)
            {
                wcReturn.ErrCode = -1;
                wcReturn.ErrMessage = "Skärmarginal utanför gränsvärdet";
                return wcReturn; ;
            }

            string sSql = "";
            if (wc.workingCostId == 0)
                sSql = getInsertSql();
            else
                sSql = getUpdateSql();
            NxParameterCollection np = new NxParameterCollection();
            ReparatorCL rep = cr.getReparator(ident);
            setParameters(np, wc, rep);
            string errTxt = "";
            int iRc = cdb.updateData(sSql, ref errTxt, np);
            if (errTxt != "")
            {
                if (errTxt.Length > 2000)
                    errTxt = errTxt.Substring(1, 2000);
                wcReturn.ErrCode = -100;
                wcReturn.ErrMessage = errTxt;
                return wcReturn;;
            }

            return getWorkingCosts(ident);
        }

    }
}
