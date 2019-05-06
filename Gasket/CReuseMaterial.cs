using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NexusDB.ADOProvider;
using System.Data;

namespace SManApi.Gasket
{
    public class CReuseMaterial
    {
        CDB cdb = null;
        public CReuseMaterial()
        {
            cdb = new CDB();
        }


        /// <summary>
        /// Get a list of all reuse material sizes
        /// Set reuseMatId to 0 to get all objects
        /// Otherwise return only the object matching the 
        /// Id number

        /// </summary>
        /// <param name="ident"></param>
        /// <param name="reuseMatId"></param>
        /// <returns></returns>
        /// 2018-08-31 KJBO 
        public List<gReuseMatCL> getReuseMaterial(string ident, int reuseMatId)
        {
            List<gReuseMatCL> rmList = new List<gReuseMatCL>();
            CReparator cr = new CReparator();
            int identOK = cr.checkIdent(ident);
            if (identOK == -1)
            {
                gReuseMatCL gm = new gReuseMatCL();
                gm.ErrCode = -10;
                gm.ErrMessage = "Ogiltigt login";
                rmList.Add(gm);                
                return rmList;
            }



            NxParameterCollection pc = new NxParameterCollection();
            string sSql = " SELECT gm.reuseMatId, gm.minDiam, gm.reusePercentage, coalesce(min(gmNext.minDiam), 1500) gmMaxDiam "
                        + " FROM gReuseMat gm "
                        + " left outer join gReuseMat gmNext on gm.minDiam < gmNext.minDiam ";
            if (reuseMatId > 0)
            {
                sSql += "where gm.reuseMatId = :reuseMatId ";
                pc.Add("reuseMatId", reuseMatId);
            }
            sSql += " group by gm.reuseMatId, gm.minDiam, gm.reusePercentage ";
            string errText = "";
            DataTable dt = cdb.getData(sSql, ref errText, pc);

            int errCode = -100;
            if (errText != "")
            {
                gReuseMatCL gm = new gReuseMatCL();
                if (errText.Length > 2000)
                    errText = errText.Substring(1, 2000);
                gm.ErrCode = errCode;
                gm.ErrMessage = errText;
                rmList.Add(gm);
                return rmList;
            }
            foreach (DataRow dr in dt.Rows)
            {
                gReuseMatCL gm = new gReuseMatCL();
                gm.ErrCode = 0;
                gm.ErrMessage = "";
                gm.reuseMatId = Convert.ToInt32(dr["reuseMatId"]);
                gm.minDiam = Convert.ToDecimal(dr["minDiam"]);
                gm.maxDiam = Convert.ToDecimal(dr["gmMaxDiam"]);                
                gm.reusePercentage = Convert.ToDecimal(dr["reusePercentage"]);
                rmList.Add(gm);
            }
            return rmList;
        }

        private string getInsertSQL()
        {

            return " insert into gReuseMat ( minDiam, reusePercentage, reg, regdate) "
                + " values(:minDiam, :reusePercentage, :reg, :regdate) ";
        }

        private string getUpdateSQL()
        {
            string sSql = " update gReuseMat "
                        + " set minDiam = :minDiam "
                        + " , reusePercentage = :reusePercentage "
                        + " , updat = :reg "
                        + " , updatDat = :regdate "
                        + " where reuseMatId = :reuseMatId ";
            return sSql;
        }

        private NxParameterCollection setParameters( gReuseMatCL mat, ReparatorCL rep)
        {
            NxParameterCollection pc = new NxParameterCollection();            
            pc.Add("reuseMatId", mat.reuseMatId);
            pc.Add("minDiam",mat.minDiam);
            pc.Add("reusePercentage",mat.reusePercentage);
            pc.Add("reg",rep.AnvID);
            pc.Add("regdate",DateTime.Now);
            return pc;
        }


        private int getLastID()
        {
            string sSql = " select max(reuseMatId) maxId "
                        + " from gReuseMat ";
            string e = "";
            DataTable dt = cdb.getData(sSql, ref e);
            return Convert.ToInt32(dt.Rows[0]["maxId"]);
        }

        /// <summary>
        /// Saves reusable material
        /// Returns the new created or changed material object
        /// If an error occurs than that error will be returned
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="reuseMat"></param>
        /// <returns></returns>
        /// 2018-08-31 KJBO 
        public gReuseMatCL saveReuseMaterial(string ident, gReuseMatCL reuseMat)
        {
            gReuseMatCL reuseMatRet = new gReuseMatCL();

            CReparator cr = new CReparator();
            int identOK = cr.checkIdent(ident);

            if (identOK == -1)
            {
                reuseMatRet.ErrCode = -10;
                reuseMatRet.ErrMessage = "Ogiltigt login";
                return reuseMatRet;
            }

            if (reuseMat.minDiam <= 0 || reuseMat.minDiam > 1500)
            {
                reuseMatRet.ErrCode = -1;
                reuseMatRet.ErrMessage = "Minsta diameter måste vara större än 0 och mindre än 1500 mm.";
                return reuseMatRet;
            }
            if (reuseMat.reusePercentage < 0 || reuseMat.reusePercentage >= 100)
            {
                reuseMatRet.ErrCode = -1;
                reuseMatRet.ErrMessage = "Återanvändbar procent måste vara större än 0 och mindre än 100";
                return reuseMatRet;
            }

            string sSql = "";
            if (reuseMat.reuseMatId == 0)
                sSql = getInsertSQL();
            else
                sSql = getUpdateSQL();
            ReparatorCL rep = cr.getReparator(ident);
            NxParameterCollection pc = setParameters(reuseMat, rep);
            string errTxt = "";
            int iRc = cdb.updateData(sSql, ref errTxt, pc);

            if (errTxt != "")
            {
                if (errTxt.Length > 2000)
                    errTxt = errTxt.Substring(1, 2000);
                reuseMatRet.ErrCode = -100;
                reuseMatRet.ErrMessage = errTxt;
                return reuseMatRet;
            }
            
            if (reuseMat.reuseMatId == 0)            
                reuseMat.reuseMatId = getLastID();

            List<gReuseMatCL> list = getReuseMaterial(ident, reuseMat.reuseMatId);
            if (list.Count == 0)
            {
                reuseMatRet.ErrCode = -101;
                reuseMatRet.ErrMessage = "Can not retrieve the current reuseMaterial";
                return reuseMatRet;
            }

            return list[0];
        }



        /// <summary>
        /// Returns the deault reusable percent depending on the innerSize of a gasket
        /// Please check errCode before using the result
        /// The only field that shall be accessed is the reusePercentage
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="innerSize"></param>
        /// <returns></returns>
        /// 2018-08-31 KJBO
        public gReuseMatCL getReusablePercentage( string ident, Decimal innerSize)
        {
            gReuseMatCL reuseMatRet = new gReuseMatCL();
            reuseMatRet.ErrCode = 0;
            reuseMatRet.ErrMessage = "";


            CReparator cr = new CReparator();
            int identOK = cr.checkIdent(ident);

            if (identOK == -1)
            {
                reuseMatRet.ErrCode = -10;
                reuseMatRet.ErrMessage = "Ogiltigt login";
                return reuseMatRet;
            }

            string sSql = "select coalesce(reusePercentage,0) reusePercentage "
                        + " from gReuseMat "
                        + " where minDiam = (select max(minDiam) "
                        + " from gReuseMat "
                        + " where minDiam <= :minDiam ) ";
            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("minDiam", innerSize);
            string errTxt = "";
            DataTable dt = cdb.getData(sSql, ref errTxt, pc);
            if (errTxt != "")
            {
                if (errTxt.Length > 2000)
                    errTxt = errTxt.Substring(1, 2000);
                reuseMatRet.ErrCode = -100;
                reuseMatRet.ErrMessage = errTxt;
                return reuseMatRet;
            }

            if (dt.Rows.Count == 0)
            {
                reuseMatRet.reusePercentage = 0;
                return reuseMatRet;
            }

            DataRow dr = dt.Rows[0];
            reuseMatRet.reusePercentage = Convert.ToDecimal(dr["reusePercentage"]);
            return reuseMatRet;
        }
    }
}