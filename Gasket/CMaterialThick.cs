using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NexusDB.ADOProvider;
using System.Data;


namespace SManApi.Gasket
{
    public class CMaterialThick
    {
        CDB cdb = null;
        public CMaterialThick()
        {
            cdb = new CDB();
        }


        /// <summary>
        /// Returns a list of material thickness
        /// if parameter materialThicknId = 0 then all registered material thickness will be returned
        /// if the parameter is > 0 then only one (or error) will be returned
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="materialThicknId"></param>
        /// <returns></returns>
        /// 2018-08-16 KJBO
        public List<gMaterialThicknCL> getMaterialThickn(string ident, int materialThicknId)
        {
            List<gMaterialThicknCL> gmList = new List<gMaterialThicknCL>();

            CReparator cr = new CReparator();
            int identOK = cr.checkIdent(ident);
            if (identOK == -1)
            {
                gMaterialThicknCL gm = new gMaterialThicknCL();
                gm.ErrCode = -10;
                gm.ErrMessage = "Ogiltigt login";
                gmList.Add(gm);
                return gmList;
            }

            string sSql = " SELECT mt.materialThicknId, mt.materialSizeId, mt.\"description\", mt.thicknShort, mt.thickness, mt.buyPrice, mt.sellPrice, mt.cuttingTime "
                        + " , ms.\"description\" materialSize, m.material "
                        + " FROM gMaterialThickn mt "
                        + " join gMaterialSize ms on mt.materialSizeId = ms.materialSizeId "
                        + " join gMaterial m on ms.materialId = m.materialId ";
            if (materialThicknId > 0)
                sSql += " where mt.materialThicknId = :materialThicknId ";

            NxParameterCollection pc = new NxParameterCollection();
            if (materialThicknId != 0)
                pc.Add("materialThicknId", materialThicknId);
            string errText = "";
            DataTable dt = cdb.getData(sSql, ref errText, pc);

            int errCode = -100;

            if (errText != "")
            {
                gMaterialThicknCL gm = new gMaterialThicknCL();
                if (errText.Length > 2000)
                    errText = errText.Substring(1, 2000);
                gm.ErrCode = errCode;
                gm.ErrMessage = errText;
                gmList.Add(gm);
                return gmList;
            }

            if (materialThicknId > 0 && dt.Rows.Count == 0)
            {
                gMaterialThicknCL gm = new gMaterialThicknCL();
                gm.ErrCode = -1;
                gm.ErrMessage = "Det finns ingen registrerad med id " + materialThicknId.ToString();
                gmList.Add(gm);
                return gmList;
            }

            foreach (DataRow dr in dt.Rows)
            {
                gMaterialThicknCL gm = new gMaterialThicknCL();
                gm.ErrCode = 0;
                gm.ErrMessage = "";
                gm.materialThicknId = Convert.ToInt32(dr["materialThicknId"]);
                gm.materialSizeId = Convert.ToInt32(dr["materialSizeId"]);
                gm.description = dr["description"].ToString();
                gm.thicknShort = dr["thicknShort"].ToString();
                gm.thickness = Convert.ToDecimal(dr["thickness"]);
                gm.buyPrice = Convert.ToDecimal(dr["buyPrice"]);
                gm.sellPrice = Convert.ToDecimal(dr["sellPrice"]);
                gm.cuttingTime = Convert.ToDecimal(dr["cuttingTime"]);
                gm.materialSize = dr["materialSize"].ToString();
                gm.materialName = dr["material"].ToString();
                gmList.Add(gm);
            }
            return gmList;
        }

        private string getInsertSql()
        {
            return " insert into gMaterialThickn (materialSizeId, \"description\", thicknShort, thickness, buyPrice, sellPrice, cuttingTime, reg, regdate) "
                    + " values( :materialSizeId, :description, :thicknShort, :thickness, :buyPrice, :sellPrice, :cuttingTime, :reg, :regdate) "; 
        }

        private string getUpdateSql()
        {
            string sSql = " update gMaterialThickn "
                        + " set materialSizeId = :materialSizeId "
                        + ", \"description\" = :description "
                        + ", thicknShort = :thicknShort "
                        + ", thickness = :thickness "
                        + ", buyPrice = :buyPrice "
                        + ", sellPrice = :sellPrice "
                        + ", cuttingTime = :cuttingTime "
                        + ", updat = :updat"
                        + ", updatDat = :updatDat "
                        + " where materialThicknId = :materialThicknId ";
            return sSql;
        }

        private void setParameters( NxParameterCollection np, gMaterialThicknCL mt, ReparatorCL rep)
        {
            DateTime now = DateTime.Now;
            np.Add("materialThicknId",mt.materialThicknId);
            np.Add("materialSizeId",mt.materialSizeId);
            np.Add("description",mt.description);
            np.Add("thicknShort",mt.thicknShort);
            np.Add("thickness",mt.thickness);
            np.Add("buyPrice",mt.buyPrice);
            np.Add("sellPrice",mt.sellPrice);
            np.Add("cuttingTime",mt.cuttingTime);
            np.Add("reg",rep.AnvID);
            np.Add("regdate",now);
            np.Add("updat",rep.AnvID);
            np.Add("updatDat",now);
        }

        private bool checkUniqueThicknShort(gMaterialThicknCL mt, ref string errTxt)
        {

            string sSql = " select count(*) countThicknShort "
                        + " from gMaterialThickn "
                        + " where thicknShort = :thicknShort "
                        + " and materialSizeId = :materialSizeId ";
            if (mt.materialThicknId > 0)
                sSql += "  and materialThicknId <> :materialThicknId ";
            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("thicknShort",mt.thicknShort);
            pc.Add("materialSizeId",mt.materialSizeId);
            if (mt.materialThicknId > 0)
                pc.Add("materialThicknId", mt.materialThicknId);            
            DataTable dt = cdb.getData(sSql, ref errTxt, pc);
            if (errTxt != "")
                return false;
            if (dt.Rows.Count == 0)
                return false;
            DataRow dr = dt.Rows[0];
            return Convert.ToInt32(dr["countThicknShort"]) == 0;
        }

        private int validateData(gMaterialThicknCL m, ref string errTxt)
        {
            if (!checkUniqueThicknShort(m, ref errTxt))
                return -1;
            return 1;
        }

        private int getLastId()
        {
            string sSql = "select coalesce(max(materialThicknId),0) maxId "
                        + " from gMaterialThickn ";
            string dummy = "";
            DataTable dt = cdb.getData(sSql, ref dummy);
            return Convert.ToInt32(dt.Rows[0]["maxId"]);
        }


        /// <summary>
        /// Saves a material thickness to the database.
        /// If an error occurs then the return object (of type gMaterialThicknCL)
        /// will have error information
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="thickn"></param>
        /// <returns></returns>
        /// 2018-08-16 KJBO 
        public gMaterialThicknCL saveMaterialThickness(string ident, gMaterialThicknCL thickn)
        {
            gMaterialThicknCL thicknRet = new gMaterialThicknCL();

            CReparator cr = new CReparator();
            int identOK = cr.checkIdent(ident);

            if (identOK == -1)
            {
                thicknRet.ErrCode = -10;
                thicknRet.ErrMessage = "Ogiltigt login";
                return thicknRet;
            }

            if (thickn.materialSizeId <= 0)
            {
                thicknRet.ErrCode = -1;
                thicknRet.ErrMessage = "Materialstorlek måste väljas";
                return thicknRet;
            }

            if (thickn.description == "")
            {
                thicknRet.ErrCode = -1;
                thicknRet.ErrMessage = "Beskrivning måste anges";
                return thicknRet;
            }

            if (thickn.thicknShort == "")
            {
                thicknRet.ErrCode = -1;
                thicknRet.ErrMessage = "Kortbeskrivning måste anges";
                return thicknRet;
            }

            if (thickn.thickness <= 0 || thickn.thickness > 100)
            {
                thicknRet.ErrCode = -1;
                thicknRet.ErrMessage = "Tjocklek måste vara större än 0 och mindre än 100 mm ";
                return thicknRet;
            }

            if (thickn.buyPrice <= 0 || thickn.buyPrice > 100000)
            {
                thicknRet.ErrCode = -1;
                thicknRet.ErrMessage = "Inköpspris felaktigt angivet";
                return thicknRet;

            }

            if (thickn.sellPrice <= 0 || thickn.sellPrice > 1000000)
            {
                thicknRet.ErrCode = -1;
                thicknRet.ErrMessage = "Försäljningspris felaktigt angivet";
                return thicknRet;
            }

            if (thickn.sellPrice < thickn.buyPrice)
            {
                thicknRet.ErrCode = -1;
                thicknRet.ErrMessage = "Försäljningspris bör vara större än inköpspris";
                return thicknRet;
            }

            if (thickn.cuttingTime <= 0 || thickn.cuttingTime > 100)
            {
                thicknRet.ErrCode = -1;
                thicknRet.ErrMessage = "Skärtid bör vara större än 0 men mindre än 100 m/minut";
                return thicknRet;
            }
            string errTxt = "";
            int rc = validateData(thickn, ref errTxt);

            // Check if database error occurs when checkin unique value
            if (errTxt != "")
            {
                thicknRet.ErrCode = -100;
                if (errTxt.Length > 2000)
                    errTxt = errTxt.Substring(1, 2000);
                thicknRet.ErrMessage = "Fel vid kontroll av data. Felbeskrivning : " + errTxt;
                return thicknRet;
            }

            string sSql = "";
            if (thickn.materialThicknId == 0)
                sSql = getInsertSql();
            else
                sSql = getUpdateSql();
            NxParameterCollection np = new NxParameterCollection();
            ReparatorCL rep = cr.getReparator(ident);
            setParameters(np, thickn, rep);
            errTxt = "";
            int iRc = cdb.updateData(sSql, ref errTxt, np);
            if (errTxt != "")
            {
                if (errTxt.Length > 2000)
                    errTxt = errTxt.Substring(1, 2000);
                thicknRet.ErrCode = -100;
                thicknRet.ErrMessage = errTxt;
                return thicknRet;
            }

            if (thickn.materialThicknId == 0)
                thickn.materialThicknId = getLastId();
            List<gMaterialThicknCL> thicknList = getMaterialThickn(ident, thickn.materialThicknId);
            return thicknList[0];
        }                  


    }
}