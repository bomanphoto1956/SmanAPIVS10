using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NexusDB.ADOProvider;
using System.Data;


namespace SManApi.Gasket
{
    public class CMaterialSize
    {

        CDB cdb = null;
        public CMaterialSize()
        {
            cdb = new CDB();
        }

        /// <summary>
        /// Returns registered material sizes
        /// if materialSizeId is 0 then all registered material sizes will be returned
        /// else the material size that matches the materiallSizeId will be returned
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="materialSizeId"></param>
        /// <returns></returns>
        public List<gMaterialSizeCL> getMaterialSize(string ident, int materialSizeId)
        {
            List<gMaterialSizeCL> gmList = new List<gMaterialSizeCL>();

            CReparator cr = new CReparator();
            int identOK = cr.checkIdent(ident);
            if (identOK == -1)
            {
                gMaterialSizeCL gm = new gMaterialSizeCL();
                gm.ErrCode = -10;
                gm.ErrMessage = "Ogiltigt login";
                gmList.Add(gm);
                return gmList;
            }

            string sSql = " SELECT ms.materialSizeId, ms.materialId, ms.\"description\", ms.sizeShort, ms.materialLength, ms.materialWidth, ms.defaultVal, m.material materialName "
                        + " FROM gMaterialSize ms "
                        + " join gMaterial m on ms.materialId = m.materialId ";
            if (materialSizeId != 0)
                sSql += " where materialSizeId = :materialSizeId ";

            NxParameterCollection pc = new NxParameterCollection();
            if (materialSizeId != 0)
                pc.Add("materialSizeId", materialSizeId);
            string errText = "";
            DataTable dt = cdb.getData(sSql, ref errText, pc);

            int errCode = -100;

            if (errText != "")
            {
                gMaterialSizeCL gm = new gMaterialSizeCL();
                if (errText.Length > 2000)
                    errText = errText.Substring(1, 2000);
                gm.ErrCode = errCode;
                gm.ErrMessage = errText;
                gmList.Add(gm);
                return gmList;
            }

            if (materialSizeId > 0 && dt.Rows.Count == 0)
            {
                gMaterialSizeCL gm = new gMaterialSizeCL();
                gm.ErrCode = -1;
                gm.ErrMessage = "Det finns inget registrerat material med id " + materialSizeId.ToString();
                gmList.Add(gm);
                return gmList;

            }

            foreach (DataRow dr in dt.Rows)
            {
                gMaterialSizeCL gm = new gMaterialSizeCL();
                gm.ErrCode = 0;
                gm.ErrMessage = "";
                gm.materialSizeId = Convert.ToInt32(dr["materialSizeId"]);
                gm.materialId = Convert.ToInt32(dr["materialId"]);
                gm.description = dr["description"].ToString();
                gm.sizeShort = dr["sizeShort"].ToString();
                gm.materialLength = Convert.ToDecimal(dr["materialLength"]);
                gm.materialWidth = Convert.ToDecimal(dr["materialWidth"]);
                gm.defaultVal = Convert.ToBoolean(dr["defaultVal"]);
                gm.materialName = dr["materialName"].ToString();
                gmList.Add(gm);
            }

            return gmList;


        }


        private string getInsertSql()
        {
            return " insert into gMaterialSize (  materialId, \"description\", sizeShort, materialLength, materialWidth, defaultVal, reg, regdate ) "
            + " values( :materialId, :description, :sizeShort, :materialLength, :materialWidth, :defaultVal, :reg, :regdate) ";
        }


        private string getUpdateSql()
        {
            string sSql = "update gMaterialSize "                        
                        + " set materialId = :materialId "
                        + " , \"description\" = :description "
                        + ", sizeShort = :sizeShort "
                        + ", materialLength = :materialLength "
                        + ", materialWidth = :materialWidth "
                        + ", defaultVal = :defaultVal "
                        + ", updat = :updat "
                        + ", updatDat = :updatDat "
                        + " where materialSizeId = :materialSizeId ";
            return sSql;
        }


        private void setParameters(NxParameterCollection pc, gMaterialSizeCL mat, ReparatorCL rep)
        {
            DateTime now = DateTime.Now;
            pc.Add("materialSizeId", mat.materialSizeId);
            pc.Add("materialId",mat.materialId);
            pc.Add("description",mat.description);
            pc.Add("sizeShort",mat.sizeShort);
            pc.Add("materialLength",mat.materialLength);
            pc.Add("materialWidth",mat.materialWidth);
            pc.Add("defaultVal",mat.defaultVal);
            pc.Add("reg",rep.AnvID);
            pc.Add("regdate",now);
            pc.Add("updat",rep.AnvID);
            pc.Add("updatDat", now);
            return;
        }


        private bool checkUniqueSizeShort(gMaterialSizeCL m, ref string errTxt)
        {
            string sSql = " select count(*) countSizeShort "
                        + " from gMaterialSize "
                        + " where sizeShort = :sizeShort "
                        + " and materialId = :materialId ";
            if (m.materialSizeId > 0)
                sSql += " and materialSizeId <> :materialSizeId ";
            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("sizeShort", m.sizeShort);
            pc.Add("materialId",m.materialId);
            if (m.materialSizeId != 0)
                pc.Add("materialSizeId", m.materialSizeId);
            errTxt = "";
            DataTable dt = cdb.getData(sSql, ref errTxt, pc);
            if (errTxt != "")
                return false;
            if (dt.Rows.Count == 0)
                return false;
            DataRow dr = dt.Rows[0];
            return Convert.ToInt32(dr["countSizeShort"]) == 0;
        }

        private int validateData(gMaterialSizeCL m, ref string errTxt)
        {
            if (!checkUniqueSizeShort(m, ref errTxt))
                return -1;
            return 1;
        }

        private int getLastId()
        {
            string sSql = "select coalesce(max(materialSizeId),0) maxId "
                        + " from gMaterialSize ";
            string dummy = "";
            DataTable dt = cdb.getData(sSql, ref dummy);
            return Convert.ToInt32(dt.Rows[0]["maxId"]);
        }



        /// <summary>
        /// Validates and saves material size to the database
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="matSize"></param>
        /// <returns></returns>
        /// 2018-08-16 KJBO
        public gMaterialSizeCL saveMaterialSize(string ident, gMaterialSizeCL matSize)
        {
            gMaterialSizeCL matSizeRet = new gMaterialSizeCL();

            CReparator cr = new CReparator();
            int identOK = cr.checkIdent(ident);

            if (identOK == -1)
            {
                matSizeRet.ErrCode = -10;
                matSizeRet.ErrMessage = "Ogiltigt login";
                return matSizeRet;
            }

            if (matSize.description == "")
            {
                matSizeRet.ErrCode = -1;
                matSizeRet.ErrMessage = "Beskrivning måste anges";
                return matSizeRet;
            }

            if (matSize.materialId <= 0)
            {
                matSizeRet.ErrCode = -1;
                matSizeRet.ErrMessage = "Välj material";
                return matSizeRet;
            }

            if (matSize.sizeShort == "")
            {
                matSizeRet.ErrCode = -1;
                matSizeRet.ErrMessage = "Kortbeskrivning måste anges";
                return matSizeRet;
            }

            if (matSize.materialLength <= 0)
            {
                matSizeRet.ErrCode = -1;
                matSizeRet.ErrMessage = "Materiallängd måste vara större an 0";
                return matSizeRet;
            }

            if (matSize.materialWidth <= 0)
            {
                matSizeRet.ErrCode = -1;
                matSizeRet.ErrMessage = "Materialbredd måste vara större an 0";
                return matSizeRet;
            }

            string errTxt = "";
            int rc = validateData(matSize, ref errTxt);

            // Check if database error occurs when checkin unique value
            if (errTxt != "")
            {
                matSizeRet.ErrCode = -100;
                if (errTxt.Length > 2000)
                    errTxt = errTxt.Substring(1, 2000);
                matSizeRet.ErrMessage = "Fel vid kontroll av data. Felbeskrivning : " + errTxt;
                return matSizeRet;

            }

            // If the current short description already exists
            if (rc == -1)
            {
                matSizeRet.ErrCode = -1;
                matSizeRet.ErrMessage = "Kortbeskrivning måste vara unik";
                return matSizeRet;

            }

            string sSql = "";

            if (matSize.materialSizeId == 0)
                sSql = getInsertSql();
            else
                sSql = getUpdateSql();

            NxParameterCollection np = new NxParameterCollection();
            ReparatorCL rep = cr.getReparator(ident);
            setParameters(np, matSize, rep);
            errTxt = "";
            int iRc = cdb.updateData(sSql, ref errTxt, np);
            if (errTxt != "")
            {
                if (errTxt.Length > 2000)
                    errTxt = errTxt.Substring(1, 2000);
                matSizeRet.ErrCode = -100;
                matSizeRet.ErrMessage = errTxt;
                return matSizeRet;
            }

            if (matSize.materialSizeId == 0)
                matSize.materialSizeId = getLastId();
            List<gMaterialSizeCL> matList = getMaterialSize(ident, matSize.materialSizeId);                
            return matList[0];
        }


    }
}