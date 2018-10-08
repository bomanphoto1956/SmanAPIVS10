using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using SManApi.Gasket;


namespace SManApi
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "SmManager2" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select SmManager2.svc or SmManager2.svc.cs at the Solution Explorer and start debugging.
    public class SmManager : ISmManager
    {
        public void DoWork()
        {
        }

        /// <summary>
        /// This is the login function for SmManager service
        /// The reparator has to be in the category of AL_ST
        /// in order to be accepted
        /// </summary>
        /// <param name>Login parameters</param>
        /// <returns>Login parameters with reparator name and ident added</returns>
        public LoginAdm loginAdmin(LoginAdm login)
        {
            CReparator cr = new CReparator();
            return cr.loginAdmin(login);
        }

        public List<ListServHuvCL> getServHuv(string ident, int selType)
        {
            CServiceHuvud csh = new CServiceHuvud();
            return csh.getServHuv(ident, selType);
        }

        /// <summary>
        /// Identifies a servicehuvud by sh.vart_ordernr
        /// Retrieves the matching order and return
        /// </summary>
        /// <param name="sh"></param>
        /// <param name="ident"></param>
        /// <returns></returns>

        public ServHuvCL getServiceHuvud(ServHuvCL sh, string ident)
        {
            CMServHuv cms = new CMServHuv();
            return cms.getServiceHuvud(sh, ident);
        }

        /// <summary>
        /// Creates a list of all active reparatör
        /// in alphabetic order. To be used by dropdown controls
        /// or elsewhere when a key-value collection is needed
        /// </summary>
        /// <param name="ident"></param>
        /// <returns></returns>
        public List<ListRepCL> getReparatorList(string ident)
        {
            CMReparator cmr = new CMReparator();
            return cmr.getReparatorList(ident);
        }

        /// <summary>
        /// Returns a list of customers in a key-value pair
        /// To be used by dropdowns or similar
        /// </summary>
        /// <param name="ident"></param>
        /// <returns></returns>
        public List<ListKundCL> getCustList(string ident)
        {
            CMKund cmk = new CMKund();
            return cmk.getCustList(ident);
        }

        /// <summary>
        /// Validate, insert or update one servicehuvud
        /// </summary>
        /// <param name="sh"></param>
        /// <param name="ident"></param>
        /// <returns>Newly created servicehuvud</returns>
        // 2018-01-29 KJBO
        public ServHuvCL saveServHuv(ServHuvCL sh, string ident)
        {
            CMServHuv cms = new CMServHuv();
            return cms.saveServHuv(sh, ident);
        }

        /// <summary>
        /// Toggle the approve flag for one report week
        /// </summary>
        /// <param name="id"></param>
        /// <param name="approve"></param>
        /// <param name="ident"></param>
        /// <returns></returns>
        public ErrorCL toggleApprove(int id, Boolean approve, string ident)
        {
            CTidRed crt = new CTidRed();
            return crt.toggleApprove(id, approve, ident);
        }


        /// <summary>
        /// Get a list of all reparators connected to an order
        /// </summary>
        /// <param name="VartOrdernr"></param>
        /// <param name="ident"></param>
        /// <returns></returns>
        /// 2018-02-02 KJBO
        public List<ShReparatorCL> getShReparatorList(string VartOrdernr, string ident)
        {
            CMReparator cmr = new CMReparator();
            return cmr.getShReparatorList(VartOrdernr, ident);

        }

        /// <summary>
        /// Updates the table shReparator.
        /// If the parameter addTo order then a new row will be added
        /// with the current ordernr and AnvId
        /// if false then the row is deleted
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="VartOrdernr"></param>
        /// <param name="AnvID"></param>
        /// <param name="addToOrder"></param>
        /// <returns></returns>
        /// 2018-02-05  KJBO    
        public ErrorCL updateShrep(string ident, string VartOrdernr, string AnvID, Boolean addToOrder)
        {
            CMReparator cmr = new CMReparator();
            return cmr.updateShrep(ident, VartOrdernr, AnvID, addToOrder);
        }

        /// <summary>
        /// Count reparators for a certain order
        /// Returns number of reparators or -1 for error
        /// </summary>
        /// <param name="vart_ordernr"></param>
        /// <param name="ident"></param>
        /// <returns></returns>
        public int countReparator(string vart_ordernr, string ident)
        {
            CServiceHuvud ch = new CServiceHuvud();
            return ch.countReparator(vart_ordernr, ident);
        }


        /// <summary>
        /// Updates the AllRep flag on an order
        /// (This flag indicates that all reparators
        /// can log in. The normal process is that
        /// there is a list (in the shReparator table) with
        /// the reparators that can log in on a certain order).
        /// </summary>
        /// <param name="VartOrdernr"></param>
        /// <param name="allRep"></param>
        /// <param name="ident"></param>
        /// <returns></returns>
        /// 2018-02-08 KJBO
        public ServHuvCL updateAllRep(string VartOrdernr, bool allRep, string ident)
        {
            CMServHuv cmsh = new CMServHuv();
            return cmsh.updateAllRep(VartOrdernr, allRep, ident);
        }

        /// <summary>
        /// Returns a list of ordered arts for a given order
        /// Or only one orderArt defined by orderArtId
        /// </summary>
        /// <param name="vartOrdernr"></param>
        /// <param name="ident"></param>
        /// <returns></returns>
        public List<OrderArtCL> getOrderArt(int orderArtId, string vartOrdernr, string ident)
        {
            ServHuvSrc.COrderArt orderArt = new ServHuvSrc.COrderArt();
            return orderArt.getOrderArt(orderArtId, vartOrdernr, ident);
        }

        /// <summary>
        /// Get all articles that are handled by
        /// pyramid as stock articles
        /// </summary>
        /// <param name="ident"></param>
        /// <returns></returns>
        public List<OrderArtCL> getCheckoutableArticles(string ident, string artnr)
        {
            ServHuvSrc.COrderArt oa = new ServHuvSrc.COrderArt();
            return oa.getCheckoutableArticles(ident, artnr);
        }

        public OrderArtCL checkoutOrderArt(string ident, OrderArtCL oa)
        {
            ServHuvSrc.COrderArt coa = new ServHuvSrc.COrderArt();
            return coa.checkoutOrderArt(ident, oa);
        }

        /// <summary>
        /// Generate file for lagerautomat
        /// </summary>
        /// <param name="vartOrdernr"></param>
        /// <returns></returns>
        /// 2018-04-24 Indentive AB
        public ErrorCL genCompStoreData(string ident, string vartOrdernr)
        {
            CompactStore.CCompactStore cs = new CompactStore.CCompactStore();
            return cs.genCompStoreData(ident, vartOrdernr);
        }

        /// <summary>
        /// Count how many articles that are du to
        /// be sent to CompactStore
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="vartOrdernr"></param>
        /// <returns>No of items or -1 if error (ident fails)</returns>
        /// 2018-04-22
        public Decimal countArtForStorageAut(string ident, string vartOrdernr)
        {
            CompactStore.CCompactStore cs = new CompactStore.CCompactStore();
            return cs.countArtForStorageAut(ident, vartOrdernr);
        }

        /// <summary>
        /// Calculate the next available order number is this is numeric
        /// Otherwise returns an empty string
        /// </summary>
        /// <returns></returns>
        /// 2018-04-26 KJBO Indentive AB
        public string getNextOrderNumber()
        {
            CMServHuv sh = new CMServHuv();
            return sh.getNextOrderNumber();
        }

        /// <summary>
        /// Returns a list of momskoder
        /// This is used by Pyramid
        /// </summary>
        /// <param name="ident"></param>
        /// <returns></returns>
        /// 2018-04-27 KJBO Indentive AB
        public List<MomskodCL> getMomskoder(string ident)
        {
            CMServHuv sh = new CMServHuv();
            return sh.getMomskoder(ident);
        }

        /// <summary>
        /// Check if there are any orderArt rows
        /// Use to determine if checkin shall be available
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="vartOrdernr"></param>
        /// <returns>Number of orderArt articles for the current ordernr or -1 if error occurs</returns>
        /// 
        public int countOrderArtRows(string ident, string vartOrdernr)
        {
            ServHuvSrc.COrderArt oa = new ServHuvSrc.COrderArt();
            return oa.countOrderArtRows(ident, vartOrdernr);
        }

        /// <summary>
        /// Calculate how many artikel that shall remain
        /// on a service order and suggest the checkin
        /// value by calling setTemCiAntal
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="vartOrdernr"></param>
        /// <returns>Error class</returns>
        /// 2018-05-02 KJBO Indentive AB
        public ErrorCL calculateCiOrderArt(string ident, string vartOrdernr)
        {
            ServHuvSrc.COrderArt oa = new ServHuvSrc.COrderArt();
            return oa.calculateCiOrderArt(ident, vartOrdernr);
        }

        /// <summary>
        /// Set a new value to column tempCiAntal on table orderArt
        /// This is a temporary value (to be committed later) for
        /// how many items that will be inserted into CompactStore
        /// after a serviceorder session
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="orderArtId"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        /// 2018-05-02 KJBO Indentive AB
        public ErrorCL setTempCiAntal(string ident, int orderArtId, decimal newValue)
        {
            ServHuvSrc.COrderArt oa = new ServHuvSrc.COrderArt();
            return oa.setTempCiAntal(ident, orderArtId, newValue);
        }

        /// <summary>
        /// Commits the temporary temCiAntal to
        /// the real ciAntal in the orderArt table
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="vartOrdernr"></param>
        /// <returns></returns>
        /// 2018-05-02 KJBO Indentive AB
        public ErrorCL commitTempCiAntal(string ident, string vartOrdernr)
        {
            ServHuvSrc.COrderArt oa = new ServHuvSrc.COrderArt();
            return oa.commitTempCiAntal(ident, vartOrdernr);
        }

        /// <summary>
        /// Exports all timerecords to Pyramid by
        /// aggregating on konto (=artikel)
        /// </summary>
        /// <param name="vartOrdernr"></param>
        /// <returns></returns>
        /// 2018-05-03 KJBO Indentive AB
        public ErrorCL exportTime(string vartOrdernr)
        {
            ExportToPyramid.CExportToPyramid cexp = new ExportToPyramid.CExportToPyramid();
            return cexp.exportTime(vartOrdernr);
        }

        /// <summary>
        /// Gets one article defined by artnr
        /// Also returns stock for this article
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="artnr"></param>
        /// <returns></returns>
        /// 2018-05-22 KJBO Indentive AB
        public OrderArtCL getCheckoutableArticle(string ident, string artnr)
        {
            ServHuvSrc.COrderArt oa = new ServHuvSrc.COrderArt();
            return oa.getCheckoutableArticle(ident, artnr);
        }

        /// <summary>
        /// Returns a list of available levels for the outchecked articles list
        /// </summary>
        /// <param name="ident"></param>
        /// 2018-05-28
        /// <returns></returns>
        public List<KeyValuePair<int, string>> getUcArtListSelection(string ident)
        {
            ServHuvSrc.COrderArt oa = new ServHuvSrc.COrderArt();
            return oa.getUcArtListSelection(ident);
        }


        /// <summary>
        /// Returns a list of all outchecked objects
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="listType"></param>
        /// <returns></returns>
        /// 2018-05-28 KJBO Indentive AB
        public List<OrderArtListCL> getOutcheckedList(string ident, int listType)
        {
            ServHuvSrc.COrderArt oa = new ServHuvSrc.COrderArt();
            return oa.getOutcheckedList(ident, listType);
        }

        /// <summary>
        /// Retuns a key-value list of available selection
        /// for the below getExportFullList
        /// </summary>
        /// <param name="ident"></param>
        /// <returns></returns>
        /// 2018-05-31 KJBO
        public List<KeyValuePair<int, string>> getExportListSelection(string ident)
        {
            ExportToPyramid.CExportToPyramid pExp = new ExportToPyramid.CExportToPyramid();
            return pExp.getExportListSelection(ident);
        }


        /// <summary>
        /// Get a full export list for one order. This list can reflect all items that will be exported
        /// or all items that is already exported by setting the exported boolean flag.
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="vartOrdernr"></param>
        /// <param name="exported"></param>
        /// <returns>A list of orderArtCL type</returns>
        /// 2018-05-30 KJBO Indentive AB
        public List<OrderArtCL> getExportFullList(string ident, string vartOrdernr, int listType)
        {
            ExportToPyramid.CExportToPyramid pExp = new ExportToPyramid.CExportToPyramid();
            return pExp.getExportFullList(ident, vartOrdernr, listType);
        }

        public void logMessage(int logLevel, string message, string messageDescr = "")
        {
            CDevLog cd = new CDevLog();
            cd.logMessage(logLevel, message, messageDescr);
        }

        /// <summary>
        /// Login for the GaskMan application
        /// Will check tha gasketLevel (will be 5 for a user or 10 for an administrator)
        /// If the user is MaSa (Mattias Samuelsson) then the gasketLevel will be 10 without
        /// checking.
        /// </summary>
        /// <param name="login"></param>
        /// <returns>LoginAdm class. Check for errors</returns>
        /// 2018-08-14 kjbo
        public LoginAdm GLogin(LoginAdm login)
        {
            CReparator cr = new CReparator();
            return cr.GLogin(login);
        }

        /// <summary>
        /// Returns a list of all registered materials
        /// </summary>
        /// <param name="ident"></param>
        /// <returns></returns>
        /// 2018-08-15 KJBO
        public List<gMaterialCL> getAllMaterial(string ident)
        {
            CMaterial cm = new CMaterial();
            return cm.getAllMaterial(ident);
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
            CMaterial cm = new CMaterial();
            return cm.getMaterial(ident, materialId);
        }

        /// <summary>
        /// Saves material to database
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="mat"></param>
        /// <returns>The saved material</returns>
        /// 2018-08-15 KJBO
        public gMaterialCL saveMaterial(string ident, gMaterialCL mat)
        {
            CMaterial cm = new CMaterial();
            return cm.saveMaterial(ident, mat);
        }


        /// <summary>
        /// Returns registered material sizes
        /// if materialSizeId is 0 then all registered material sizes will be returned
        /// else the material size that matches the materiallSizeId will be returned
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="materialSizeId"></param>
        /// <returns></returns>
        /// 2018-08-16 KJBO 
        public List<gMaterialSizeCL> getMaterialSize(string ident, int materialSizeId)
        {
            CMaterialSize cm = new CMaterialSize();
            return cm.getMaterialSize(ident, materialSizeId);
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
            CMaterialSize cm = new CMaterialSize();
            return cm.saveMaterialSize(ident, matSize);
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
            CMaterialThick mt = new CMaterialThick();
            return mt.getMaterialThickn(ident, materialThicknId);
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
            CMaterialThick mt = new CMaterialThick();
            return mt.saveMaterialThickness(ident, thickn);
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
            CWorkingCost cw = new CWorkingCost();
            return cw.getWorkingCosts(ident);
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
            CWorkingCost cw = new CWorkingCost();
            return cw.saveWorkingCost(ident, wc);
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
            CGasket g = new CGasket();
            return g.getGasket(ident, gasketId);
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
            CGasket g = new CGasket();
            return g.saveGasket(ident, gasket);
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
            CGasket g = new CGasket();
            return g.deleteGasket(ident, gasketId);
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
            CGasket g = new CGasket();
            return g.getGasketType(ident);
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
            CReuseMaterial cr = new CReuseMaterial();
            return cr.getReuseMaterial(ident, reuseMatId);
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
            CReuseMaterial cr = new CReuseMaterial();
            return cr.saveReuseMaterial(ident, reuseMat);
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
        public gReuseMatCL getReusablePercentage(string ident, Decimal innerSize)
        {
            CReuseMaterial cr = new CReuseMaterial();
            return cr.getReusablePercentage(ident, innerSize);
        }



    }
}
