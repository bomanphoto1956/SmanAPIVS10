using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NexusDB.ADOProvider;
using System.Data;


namespace SManApi.Gasket
{
    public class CGasket
    {
        CDB cdb = null;
        public CGasket()
        {
            cdb = new CDB();
        }


        /// <summary>
        /// Returns all gaskets or just one
        /// If gasketId = 0 all registered gasket will be returned
        /// if gasketId > 0 then the gasket with that primary key
        /// will be returned.
        /// If a gasket id is provided and this gasket don't exists
        /// then the errorCode will return -100 and the calling program
        /// needs to take the correct action.
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="gasketId"></param>
        /// <returns></returns>
        /// 2018-08-17 kjbo
        public List<gGasketCL> getGasket(string ident, int gasketId)
        {
            List<gGasketCL> gasketList = new List<gGasketCL>();
            
            CReparator cr = new CReparator();
            int identOK = cr.checkIdent(ident);
            if (identOK == -1)
            {
                gGasketCL gasket = new gGasketCL();                
                gasket.ErrCode = -10;
                gasket.ErrMessage = "Ogiltigt login";
                gasketList.Add(gasket);
                return gasketList;
            }            


            string sSql = " select g.gasketId, g.gasketTypeId, g.materialThicknId, g.outerDiam, g.innerDiam, g.reusableMaterial, g.cuttingMargin, "
                        + " g.standardPriceProduct, g.handlingTime, g.price, g.note, g.description "
                        + ", coalesce(g.Type2SecHoleCount,0) Type2SecHoleCount, coalesce(g.Type2SecHoleDiam,0) Type2SecHoleDiam "
                        + " , gt.\"description\" materialThicknName "
                        + " , gs.\"description\" materialSizeName "
                        + " , gm.material materialName "
                        + " , gtp.gasketType gasketTypeName "
                        + ",  gt.buyPrice / ((gs.materialLength * 1000) * (gs.materialWidth * 1000)) materialCostMm2 "
                        + ",  gt.SellPrice / gt.buyPrice materialMarginPercent "
                        + ", (gt.cuttingTime * 1000) / 60 cuttingSpeedMmSek "
                        + " from gGasket g "
                        + " join gMaterialThickn gt on g.materialThicknId = gt.materialThicknId "
                        + " join gMaterialSize gs on gt.materialSizeId = gs.materialSizeId "
                        + " join gMaterial gm on gs.materialId = gm.materialId "
                        + " join gGasketType gtp on g.gasketTypeId = gtp.gasketTypeId ";
            if (gasketId > 0)
                sSql += " where gasketId = :gasketId ";
            NxParameterCollection pc = new NxParameterCollection();
            if (gasketId > 0)
                pc.Add("gasketId", gasketId);
            string errText = "";
            DataTable dt = cdb.getData(sSql, ref errText, pc);

            int errCode = -100;

            if (errText != "")
            {
                gGasketCL gasket = new gGasketCL();
                if (errText.Length > 2000)
                    errText = errText.Substring(1, 2000);
                gasket.ErrCode = errCode;
                gasket.ErrMessage = errText;
                gasketList.Add(gasket);
                return gasketList;
            }

            if (gasketId > 0 && dt.Rows.Count == 0)
            {
                gGasketCL gasket = new gGasketCL();
                gasket.ErrCode = -100;
                gasket.ErrMessage = "Det finns ingen packning med id " + gasketId.ToString();
                gasketList.Add(gasket);
                return gasketList;
            }

            foreach (DataRow dr in dt.Rows)
            {
                gGasketCL gasket = new gGasketCL();
                gasket.gasketId = Convert.ToInt32(dr["gasketId"]);
                gasket.gasketTypeId = Convert.ToInt32(dr["gasketTypeId"]);
                gasket.materialThicknId = Convert.ToInt32(dr["materialThicknId"]);
                gasket.outerDiam = Convert.ToDecimal(dr["outerDiam"]);
                gasket.innerDiam = Convert.ToDecimal(dr["innerDiam"]);
                gasket.reusableMaterial = Convert.ToDecimal(dr["reusableMaterial"]);
                gasket.cuttingMargin = Convert.ToDecimal(dr["cuttingMargin"]);
                gasket.standardPriceProduct = Convert.ToBoolean(dr["standardPriceProduct"]);
                gasket.handlingTime = Convert.ToDecimal(dr["handlingTime"]);
                gasket.Type2SecHoleCount = Convert.ToInt32(dr["Type2SecHoleCount"]);
                gasket.Type2SecHoleDiam = Convert.ToDecimal(dr["Type2SecHoleDiam"]);                
                gasket.price = Convert.ToDecimal(dr["price"]);
                gasket.note = dr["note"].ToString();
                gasket.description = dr["description"].ToString();
                gasket.materialName = dr["materialName"].ToString();
                gasket.materialSizeName = dr["materialSizeName"].ToString();
                gasket.materialThicknName = dr["materialThicknName"].ToString();
                gasket.gasketTypeName = dr["gasketTypeName"].ToString();
                gasket.materialCostMm2 = Convert.ToDouble(dr["materialCostMm2"]);
                gasket.materialMarginPercent = Convert.ToDouble(dr["materialMarginPercent"]);
                gasket.cuttingSpeedMmSek = Convert.ToDouble(dr["cuttingSpeedMmSek"]);
                if (gasket.gasketTypeId == 1 || gasket.gasketTypeId == 2)
                {                    
                    gasket.cuttingLengthOuterMm = Convert.ToDouble(gasket.outerDiam) * Math.PI;
                    gasket.cuttingLengthInnerMm = Convert.ToDouble(gasket.innerDiam) * Math.PI;
                    Double dArea = Convert.ToDouble((gasket.outerDiam + (gasket.cuttingMargin * 2)) * (gasket.outerDiam + (gasket.cuttingMargin * 2)));
                    Double reusable = Convert.ToDouble(1 - (gasket.reusableMaterial / 100));
                    gasket.materialArea = dArea * reusable;
                }
                // 2018-08-30
                // Add time to cut the exra holes when typeId = 2
                if (gasket.gasketTypeId == 2)
                {
                    gasket.cuttingLengthOuterMm += (Convert.ToDouble(gasket.Type2SecHoleDiam) * Math.PI) * gasket.Type2SecHoleCount;
                }
                gasket.ErrCode = 0;
                gasket.ErrMessage = ""; 
                gasketList.Add(gasket);
            }
            return gasketList;
        }


        private string getInsertSql()
        {
            return " insert into gGasket ( gasketTypeId, materialThicknId, outerDiam, innerDiam, reusableMaterial, cuttingMargin "
                + " , standardPriceProduct, handlingTime, Type2SecHoleCount, Type2SecHoleDiam, price, note, description, reg, regdate) "
                + " values( :gasketTypeId, :materialThicknId, :outerDiam, :innerDiam, :reusableMaterial, :cuttingMargin "
                + " , :standardPriceProduct, :handlingTime, :Type2SecHoleCount, :Type2SecHoleDiam, :price, :note, :description, :reg, :regdate) ";
        }

        
        private string getUpdateSql()
        {
            string sSql = " update gGasket "
                        + " set gasketTypeId = :gasketTypeId "
                        + ", materialThicknId = :materialThicknId "
                        + ", outerDiam = :outerDiam "
                        + ", innerDiam = :innerDiam "
                        + ", reusableMaterial = :reusableMaterial "
                        + ", cuttingMargin = :cuttingMargin "
                        + ", standardPriceProduct = :standardPriceProduct "
                        + ", handlingTime = :handlingTime "
                        + ", Type2SecHoleCount = :Type2SecHoleCount "
                        + ", Type2SecHoleDiam = :Type2SecHoleDiam "
                        + ", price = :price "
                        + ", note = :note "
                        + ", description = :description "
                        + ", updat = :reg "
                        + ", updatDat = :regdate "
                        + " where gasketId = :gasketId ";
            return sSql;
        }
        
        private void setParameters(NxParameterCollection np, gGasketCL g, ReparatorCL rep)
        {
            DateTime now = DateTime.Now;
            np.Add("gasketId", g.gasketId);
            np.Add("gasketTypeId",g.gasketTypeId);
            np.Add("materialThicknId", g.materialThicknId);
            np.Add("outerDiam", g.outerDiam);
            np.Add("innerDiam", g.innerDiam);
            np.Add("reusableMaterial", g.reusableMaterial);
            np.Add("cuttingMargin", g.cuttingMargin);
            np.Add("standardPriceProduct", g.standardPriceProduct);
            np.Add("handlingTime", g.handlingTime);
            np.Add("Type2SecHoleCount", g.Type2SecHoleCount);
            np.Add("Type2SecHoleDiam", g.Type2SecHoleDiam);
            np.Add("price", g.price);
            np.Add("note", g.note);
            np.Add("description", g.description);
            np.Add("reg", rep.AnvID);
            np.Add("regdate",now);
        }


        private int getLastId()
        {
            string sSql = "select coalesce(max(gasketId),0) maxId "
                        + " from gGasket ";
            string dummy = "";
            DataTable dt = cdb.getData(sSql, ref dummy);
            return Convert.ToInt32(dt.Rows[0]["maxId"]);
        }


        /// <summary>
        /// If there is only one gasket type then the id
        /// of this will be returned.
        /// Else return -1
        /// </summary>
        /// <returns></returns>
        private int getGasketTypeId()
        {
            string sSql = " select gasketTypeId "
                        + " from gGasketType ";
            string dummy = "";
            DataTable dt = cdb.getData(sSql, ref dummy);
            if (dt.Rows.Count == 1)
                return Convert.ToInt32(dt.Rows[0]["gasketTypeId"]);
            return -1;

        }


        private bool validateGasketAgainstMaterialSize(gGasketCL g)
        {
            string sSql = " SELECT ms.materialLength * 1000 materialLengthMm, ms.materialWidth * 1000 materialWidthMm "
                        + " FROM gMaterialThickn mt "
                        + " join gMaterialSize ms on mt.materialSizeId = ms.materialSizeId "
                        + " where mt.materialThicknId = :materialThicknId ";
            string dummy = "";
            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("materialThicknId", g.materialThicknId);
            DataTable dt = cdb.getData(sSql, ref dummy, pc);
            if (dt.Rows.Count > 0)
            {
                Decimal materialLength = Convert.ToDecimal(dt.Rows[0]["materialLengthMm"]);
                Decimal materialWidth = Convert.ToDecimal(dt.Rows[0]["materialWidthMm"]);
                if (g.outerDiam > materialLength || g.innerDiam > materialWidth)
                {
                    return false;
                }
            }
            else
                return false;
            return true;
        }



        /// <summary>
        /// Validates and saves a gasket to the database
        /// Always check ErrCode and ErrMessage for errors
        /// Returns the new created gasket
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="gasket"></param>
        /// <returns></returns>
        /// 2018-08-17 KJBO
        public gGasketCL saveGasket(string ident, gGasketCL gasket)
        {
            gGasketCL gasketRet = new gGasketCL();
            CReparator cr = new CReparator();
            int identOK = cr.checkIdent(ident);

            if (identOK == -1)
            {
                gasketRet.ErrCode = -10;
                gasketRet.ErrMessage = "Ogiltigt login";
                return gasketRet;
            }

            // If there is only one type of gasket
            // then this value will be set to the 
            // gasket type field.
            int gasketTypeId = getGasketTypeId();
            if (gasketTypeId > 0)
                gasket.gasketTypeId = 1;

            if (gasket.gasketTypeId <= 0)
            {
                gasketRet.ErrCode = -1;
                gasketRet.ErrMessage = "Packningstyp måste väljas";
                return gasketRet;
            }

            if (gasket.materialThicknId <= 0)
            {
                gasketRet.ErrCode = -1;
                gasketRet.ErrMessage = "Packningsmaterial måste väljas";
                return gasketRet;
            }

            if (!validateGasketAgainstMaterialSize(gasket))
            {
                gasketRet.ErrCode = -1;
                gasketRet.ErrMessage = "Packningens ytterdiameter är större än valt material";
                return gasketRet;
            }

            if (gasket.outerDiam <= 0 || gasket.outerDiam > 100000)
            {
                gasketRet.ErrCode = -1;
                gasketRet.ErrMessage = "Ytterdiameter måste vara större än 0 (och mindre än 100000)";
                return gasketRet;
            }

            if (gasket.innerDiam <= 0 || gasket.innerDiam > 100000)
            {
                gasketRet.ErrCode = -1;
                gasketRet.ErrMessage = "Innerdiameter måste vara större än 0 (och mindre än 100000)";
                return gasketRet;
            }

            if (gasket.innerDiam > gasket.outerDiam)
            {
                gasketRet.ErrCode = -1;
                gasketRet.ErrMessage = "Ytterdiameter måste vara större än innerdiameter";
                return gasketRet;
            }

            if (gasket.reusableMaterial < 0 || gasket.reusableMaterial >= 100 )
            {
                gasketRet.ErrCode = -1;
                gasketRet.ErrMessage = "Återanvändbart material har felaktig procentsats";
                return gasketRet;
            }

            if (gasket.cuttingMargin < 0 || gasket.cuttingMargin > 100)
            {
                gasketRet.ErrCode = -1;
                gasketRet.ErrMessage = "Felaktig skärmarginal";
                return gasketRet;
            }

            if (gasket.handlingTime < 0 || gasket.handlingTime > 10000)
            {
                gasketRet.ErrCode = -1;
                gasketRet.ErrMessage = "Felaktig hanteringstid";
                return gasketRet;
            }

            if (gasket.price < 0 || gasket.price > 1000000)
            {
                gasketRet.ErrCode = -1;
                gasketRet.ErrMessage = "Felaktigt pris";
                return gasketRet;
            }

            if (gasket.gasketTypeId == 2)
            {
                if (gasket.Type2SecHoleCount < 1)
                {
                    gasketRet.ErrCode = -1;
                    gasketRet.ErrMessage = "Antal yttre hål felaktigt angivet";
                    return gasketRet;
                }

                Decimal materialLeft = gasket.outerDiam - gasket.innerDiam;
                materialLeft = materialLeft / 2;
                //materialLeft -= gasket.cuttingMargin * 2;
                if (gasket.Type2SecHoleDiam > materialLeft)
                {
                    gasketRet.ErrCode = -1;
                    gasketRet.ErrMessage = "Diameter på yttre hål måste vara mindre än " + materialLeft.ToString() + " mm."; 
                    return gasketRet;
                }


                if (gasket.Type2SecHoleDiam <= 0 || gasket.Type2SecHoleDiam > 100000 )
                {
                    gasketRet.ErrCode = -1;
                    gasketRet.ErrMessage = "Diameter på yttre hål måste vara större än 0 (och mindre än 100000)";
                    return gasketRet;
                }

            }

            string errTxt = "";

            string sSql = "";
            if (gasket.gasketId == 0)
                sSql = getInsertSql();
            else
                sSql = getUpdateSql();
            NxParameterCollection np = new NxParameterCollection();
            ReparatorCL rep = cr.getReparator(ident);
            setParameters(np, gasket, rep);
            errTxt = "";
            int iRc = cdb.updateData(sSql, ref errTxt, np);
            if (errTxt != "")
            {
                if (errTxt.Length > 2000)
                    errTxt = errTxt.Substring(1, 2000);
                gasketRet.ErrCode = -100;
                gasketRet.ErrMessage = errTxt;
                return gasketRet;
            }

            if (gasket.gasketId == 0)
                gasket.gasketId = getLastId();
            List<gGasketCL> gasketList = getGasket(ident, gasket.gasketId);            
            return gasketList[0];
        }


        /// <summary>
        /// Delete a registered gasket
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="gasketId"></param>
        /// <returns>Error class.</returns>
        /// 2018-08-21 KJBO Indentive AB
        public ErrorCL deleteGasket(string ident, int gasketId)
        {
            ErrorCL err = new ErrorCL();
            err.ErrCode = 0;
            err.ErrMessage = "";

            CReparator cr = new CReparator();
            int identOK = cr.checkIdent(ident);

            if (identOK == -1)
            {
                err.ErrCode = -10;
                err.ErrMessage = "Ogiltigt login";
                return err;
            }

            string sSql = " delete from gGasket where gasketId = :gasketId ";
            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("gasketId", gasketId);
            string errTxt = "";
            int rc = cdb.updateData(sSql, ref errTxt, pc);

            if (errTxt != "")
            {
                if (errTxt.Length > 2000)
                    errTxt = errTxt.Substring(1, 2000);
                err.ErrCode = -100;
                err.ErrMessage = errTxt;
                return err;
            }

            return err;
        }


        /// <summary>
        ///  Return a keyValue list of available gasket types
        ///  If the list exists of only one row then the
        ///  calling problem shall investigate if the first (int)
        ///  value is negative. This indicates an error and the
        ///  error message is returned in the string part
        /// </summary>
        /// <param name="ident"></param>
        /// <returns></returns>
        /// 2018-08-29 KJBO
        public List<KeyValuePair<int, string>> getGasketType(string ident)
        {
            var list = new List<KeyValuePair<int, string>>();
            CReparator cr = new CReparator();
            int identOK = cr.checkIdent(ident);

            if (identOK == -1)
            {
                list.Add(new KeyValuePair<int, string>(-1, "Ogiltigt login"));
                return list;
            }

            string sSql = " SELECT gasketTypeId, gasketType "
                        + " FROM gGasketType ";
            string errTxt = "";
            DataTable dt = cdb.getData(sSql, ref errTxt);
            if (errTxt != "")
            {
                if (errTxt.Length > 2000)
                    errTxt = errTxt.Substring(1, 2000);
                list.Add(new KeyValuePair<int, string>(-100, errTxt));
                return list;
            }

            foreach (DataRow dr in dt.Rows)
            {
                list.Add(new KeyValuePair<int, string>(Convert.ToInt32(dr["gasketTypeId"]), dr["gasketType"].ToString()));
            }
            return list;
        }

    }
}