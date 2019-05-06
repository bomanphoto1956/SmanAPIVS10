using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NexusDB.ADOProvider;
using System.Data;


namespace SManApi.Gasket
{
    public class CMaterial
    {

        CDB cdb = null;
        public CMaterial()
        {
            cdb = new CDB();
        }



        /// <summary>
        /// Returns a list of all registered materials
        /// </summary>
        /// <param name="ident"></param>
        /// <returns></returns>
        /// 2018-08-15 KJBO
        public List<gMaterialCL> getAllMaterial(string ident)
        {
            return getMaterial(ident, 0);
        }

        /// <summary>
        /// Returns a list of all registered materials
        /// If materialId <> 0 then only the material ID that
        /// matches the current ID will be returned.
        /// In that case this will be 0 or 1 instance of gMaterialCL
        /// </summary>
        /// <param name="ident"></param>
        /// <returns></returns>
        /// 2018-08-15 KJBO
        public List<gMaterialCL> getMaterial(string ident, int materialId)
        {
            List<gMaterialCL> gmList = new List<gMaterialCL>();

            CReparator cr = new CReparator();
            int identOK = cr.checkIdent(ident);
            if (identOK == -1)
            {
                gMaterialCL gm = new gMaterialCL();
                gm.ErrCode = -10;
                gm.ErrMessage = "Ogiltigt login";
                gmList.Add(gm);
                return gmList;
            }


            string sSql = " SELECT materialId, material, materialShort "
                       + " FROM gMaterial";
            if (materialId != 0)
                sSql += " where materialId = :materialId";

            NxParameterCollection pc = new NxParameterCollection();
            if (materialId != 0)
                pc.Add("materialId", materialId);
            string errText = "";
            DataTable dt = cdb.getData(sSql, ref errText, pc);

            int errCode = -100;

            if (errText != "")
            {
                gMaterialCL gm = new gMaterialCL();
                if (errText.Length > 2000)
                    errText = errText.Substring(1, 2000);
                gm.ErrCode = errCode;
                gm.ErrMessage = errText;
                gmList.Add(gm);
                return gmList;
            }

            foreach (DataRow dr in dt.Rows)
            {
                gMaterialCL gm = new gMaterialCL();
                gm.ErrCode = 0;
                gm.ErrMessage = "";
                gm.material = dr["material"].ToString();
                gm.materialId = Convert.ToInt32(dr["materialId"]);
                gm.materialShort = dr["materialShort"].ToString();
                gmList.Add(gm);
            }

            return gmList;

        }


        private string getInsertMatrSQL()
        {
            return " insert into gMaterial (material, materialShort, reg, regdate) "
                    + " values(:material, :materialShort, :reg, :regdate) ";
        }

        private string getUpdateMatrSQL()
        {
            string sSql = " update gMaterial "
                        + " set material = :material "
                        + " , materialShort = :materialShort "
                        + " , updat = :updat "
                        + " , updatDat = :updatDat "
                        + " where materialId = :materialId";
            return sSql;

        }

        private void setMatrParameters(NxParameterCollection pc, gMaterialCL mat, ReparatorCL rep)
        {
            DateTime now = DateTime.Now;
            pc.Add("materialId", mat.materialId);
            pc.Add("material", mat.material);
            pc.Add("materialShort", mat.materialShort);
            pc.Add("reg", rep.AnvID);
            pc.Add("updat", rep.AnvID);
            pc.Add("regdate", now);
            pc.Add("updatDat", now);
            return;
        }



        private int checkUniqueMaterialShort(gMaterialCL m, ref string errTxt)
        {
            string sSql = " SELECT count(*) countRows "
                        + " FROM gMaterial "
                        + " where materialShort = :materialShort ";
            if (m.materialId != 0)
                sSql += " and materialId <> :materialId";
            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("materialShort", m.materialShort);
            if (m.materialId != 0)
                pc.Add("materialId", m.materialId);
            errTxt = "";
            DataTable dt = cdb.getData(sSql, ref errTxt, pc);
            if (errTxt != "")
                return -1;
            if (dt.Rows.Count == 0)
                return -1;
            DataRow dr = dt.Rows[0];
            return Convert.ToInt32(dr["countRows"]);
        }

        private int validateMaterial(gMaterialCL m, ref string errTxt)
        {
            int res = checkUniqueMaterialShort(m, ref errTxt);
            if (res > 0)
                return -1;
            if (res < 0)
                return -2;
            return 1;
        }

        private int getLastMatrID()
        {
            string sSql = " select max(materialId) maxId "
                        + " from gMaterial ";
            string e = "";
            DataTable dt = cdb.getData(sSql, ref e);
            return Convert.ToInt32(dt.Rows[0]["maxId"]);
        }


        /// <summary>
        /// Saves material to database
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="mat"></param>
        /// <returns>The saved material</returns>
        public gMaterialCL saveMaterial(string ident, gMaterialCL mat)
        {
            gMaterialCL matRet = new gMaterialCL();

            CReparator cr = new CReparator();
            int identOK = cr.checkIdent(ident);

            if (identOK == -1)
            {
                matRet.ErrCode = -10;
                matRet.ErrMessage = "Ogiltigt login";
                return matRet;
            }

            if (mat.material == "")
            {
                matRet.ErrCode = -1;
                matRet.ErrMessage = "Materialnamn måste anges";
                return matRet;
            }

            if (mat.materialShort == "")
            {
                matRet.ErrCode = -1;
                matRet.ErrMessage = "Kortnamn måste anges";
                return matRet;
            }

            string errTxt = "";
            int iRes = validateMaterial(mat, ref errTxt);

            if (errTxt != "")
            {
                matRet.ErrCode = -1;
                matRet.ErrMessage = "Fel vid kontroll av unikt kortnamn. Felmeddelande : " + errTxt;
                return matRet;
            }

            if (iRes == -2)
            {
                matRet.ErrCode = -1;
                matRet.ErrMessage = "Fel vid kontroll av unikt kortnamn. Felmeddelande saknas.";
                return matRet;
            }

            if (iRes == -1)
            {
                matRet.ErrCode = -1;
                matRet.ErrMessage = "Kortnamn måste vara unikt";
                return matRet;
            }

            string sSql = "";

            if (mat.materialId == 0)
                sSql = getInsertMatrSQL();
            else
                sSql = getUpdateMatrSQL();

            NxParameterCollection np = new NxParameterCollection();
            ReparatorCL rep = cr.getReparator(ident);
            setMatrParameters(np, mat, rep);
            errTxt = "";
            int iRc = cdb.updateData(sSql, ref errTxt, np);
            if (errTxt != "")
            {
                if (errTxt.Length > 2000)
                    errTxt = errTxt.Substring(1, 2000);
                matRet.ErrCode = -100;
                matRet.ErrMessage = errTxt;
                return matRet;
            }

            if (mat.materialId == 0)
                mat.materialId = getLastMatrID();
            List<gMaterialCL> matList = getMaterial(ident, mat.materialId);
            return matList[0];
        }


    }
}