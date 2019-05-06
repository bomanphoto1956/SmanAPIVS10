using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace SManApi
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ISmManager2" in both code and config file together.
    [ServiceContract]
    public interface ISmManager
    {
        [OperationContract]
        void DoWork();

        /// <summary>
        /// This is the login function for SmManager service
        /// The reparator has to be in the category of AL_ST
        /// in order to be accepted
        /// </summary>
        /// <param name=Login parameters></param>
        /// <returns>Login parameters with reparator name and ident added</returns>
        /// 2018-01-18 KJBO VodVision 
        [OperationContract]
        LoginAdm loginAdmin(LoginAdm login);

        [OperationContract]
        List<ListServHuvCL> getServHuv(string ident, int selType);

        /// <summary>
        /// Identifies a servicehuvud by sh.vart_ordernr
        /// Retrieves the matching order and return
        /// </summary>
        /// <param name="sh"></param>
        /// <param name="ident"></param>
        /// <returns></returns>
        [OperationContract]
        ServHuvCL getServiceHuvud(ServHuvCL sh, string ident);

        /// <summary>
        /// Creates a list of all active reparatör
        /// in alphabetic order. To be used by dropdown controls
        /// or elsewhere when a key-value collection is needed
        /// </summary>
        /// <param name="ident"></param>
        /// <returns></returns>
        /// 2018-01-28 KJBO
        [OperationContract]
        List<ListRepCL> getReparatorList(string ident);

        /// <summary>
        /// Returns a list of customers in a key-value pair
        /// To be used by dropdowns or similar
        /// </summary>
        /// <param name="ident"></param>
        /// <returns></returns>
        /// 2018-01-28 KJBO
        [OperationContract]
        List<ListKundCL> getCustList(string ident);

        /// <summary>
        /// Validate, insert or update one servicehuvud
        /// </summary>
        /// <param name="sh"></param>
        /// <param name="ident"></param>
        /// <returns>Newly created servicehuvud</returns>
        // 2018-01-29 KJBO
        [OperationContract]
        ServHuvCL saveServHuv(ServHuvCL sh, string ident);

        /// <summary>
        /// Toggle the approve flag for one report week
        /// </summary>
        /// <param name="id"></param>
        /// <param name="approve"></param>
        /// <param name="ident"></param>
        /// <returns></returns>
        // 2018-02-01 KJBO
        [OperationContract]
        ErrorCL toggleApprove(int id, Boolean approve, string ident);

        /// <summary>
        /// Get a list of all reparators connected to an order
        /// </summary>
        /// <param name="VartOrdernr"></param>
        /// <param name="ident"></param>
        /// <returns></returns>
        /// 2018-02-02 KJBO
        [OperationContract]
        List<ShReparatorCL> getShReparatorList(string VartOrdernr, string ident);

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
        [OperationContract]
        ErrorCL updateShrep(string ident, string VartOrdernr, string AnvID, Boolean addToOrder);


        /// <summary>
        /// Count reparators for a certain order
        /// Returns number of reparators or -1 for error
        /// </summary>
        /// <param name="vart_ordernr"></param>
        /// <param name="ident"></param>
        /// <returns></returns>
        /// 2018-02-07 KJBO
        [OperationContract]
        int countReparator(string vart_ordernr, string ident);

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
        [OperationContract]
        ServHuvCL updateAllRep(string VartOrdernr, bool allRep, string ident);

        /// <summary>
        /// Returns a list of ordered arts/products for a given order
        /// </summary>
        /// <param name="vartOrdernr"></param>
        /// <param name="ident"></param>
        /// <returns></returns>
        [OperationContract]
        List<OrderArtCL> getOrderArt(int orderArtId, string vartOrdernr, string ident);

        [OperationContract]
        OrderArtCL checkoutOrderArt(string ident, OrderArtCL oa);

        /// <summary>
        /// Get all articles that are handled by
        /// pyramid as stock articles
        /// </summary>
        /// <param name="ident"></param>
        /// <returns></returns>
        /// 2018-04-20 kjbo
        /// 2018-05-22 Added artnr as an extra argument
        [OperationContract]
        List<OrderArtCL> getCheckoutableArticles(string ident, string artnr);

        /// <summary>
        /// Generate file for lagerautomat
        /// </summary>
        /// <param name="vartOrdernr"></param>
        /// <returns></returns>
        /// 2018-04-24 Indentive AB (kjbo)
        [OperationContract]
        ErrorCL genCompStoreData(string ident, string vartOrdernr);

        /// <summary>
        /// Count how many articles that are du to
        /// be sent to CompactStore
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="vartOrdernr"></param>
        /// <returns>No of items or -1 if error (ident fails)</returns>
        /// 2018-04-22 Indentive AB (kjbo)
        [OperationContract]
        Decimal countArtForStorageAut(string ident, string vartOrdernr);

        /// <summary>
        /// Calculate the next available order number is this is numeric
        /// Otherwise returns an empty string
        /// </summary>
        /// <returns></returns>
        /// 2018-04-26 KJBO Indentive AB
        [OperationContract]
        string getNextOrderNumber();

        /// <summary>
        /// Returns a list of momskoder
        /// This is used by Pyramid
        /// </summary>
        /// <param name="ident"></param>
        /// <returns></returns>
        /// 2018-04-27 KJBO Indentive AB
        [OperationContract]
        List<MomskodCL> getMomskoder(string ident);



        /// <summary>
        /// Check if there are any orderArt rows
        /// Use to determine if checkin shall be available
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="vartOrdernr"></param>
        /// <returns>Number of orderArt articles for the current ordernr or -1 if error occurs</returns>
        /// 2018-05-02 KJBO Indentive AB
        [OperationContract]
        int countOrderArtRows(string ident, string vartOrdernr);

        /// <summary>
        /// Calculate how many artikel that shall remain
        /// on a service order and suggest the checkin
        /// value by calling setTemCiAntal
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="vartOrdernr"></param>
        /// <returns>Error class</returns>
        /// 2018-05-02 KJBO Indentive AB
        [OperationContract]
        ErrorCL calculateCiOrderArt(string ident, string vartOrdernr);


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
        [OperationContract]
        ErrorCL setTempCiAntal(string ident, int orderArtId, decimal newValue);

        /// <summary>
        /// Commits the temporary temCiAntal to
        /// the real ciAntal in the orderArt table
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="vartOrdernr"></param>
        /// <returns></returns>
        /// 2018-05-02 KJBO Indentive AB
        [OperationContract]
        ErrorCL commitTempCiAntal(string ident, string vartOrdernr);

        /// <summary>
        /// Exports all timerecords to Pyramid by
        /// aggregating on konto (=artikel)
        /// </summary>
        /// <param name="vartOrdernr"></param>
        /// <returns></returns>
        /// 2018-05-03 KJBO Indentive AB
        [OperationContract]
        ErrorCL exportTime(string vartOrdernr);


        /// <summary>
        /// Gets one article defined by artnr
        /// Also returns stock for this article
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="artnr"></param>
        /// <returns></returns>
        /// 2018-05-22 KJBO Indentive AB
        [OperationContract]
        OrderArtCL getCheckoutableArticle(string ident, string artnr);

        /// <summary>
        /// Returns a list of available levels for the outchecked articles list
        /// </summary>
        /// <param name="ident"></param>
        /// 2018-05-28
        /// <returns></returns>
        [OperationContract]
        List<KeyValuePair<int, string>> getUcArtListSelection(string ident);


        /// <summary>
        /// Returns a list of all outchecked objects
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="listType"></param>
        /// <returns></returns>
        /// 2018-05-28 KJBO Indentive AB
        [OperationContract]
        List<OrderArtListCL> getOutcheckedList(string ident, int listType);

        /// <summary>
        /// Retuns a key-value list of available selection
        /// for the below getExportFullList
        /// </summary>
        /// <param name="ident"></param>
        /// <returns></returns>
        /// 2018-05-31 KJBO
        [OperationContract]
        List<KeyValuePair<int, string>> getExportListSelection(string ident);


        /// <summary>
        /// Get a full export list for one order. This list can reflect all items that will be exported
        /// or all items that is already exported by setting the exported boolean flag.
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="vartOrdernr"></param>
        /// <param name="exported"></param>
        /// <returns>A list of orderArtCL type</returns>
        /// 2018-05-30 KJBO Indentive AB        
        [OperationContract]
        List<OrderArtCL> getExportFullList(string ident, string vartOrdernr, int listType);


        [OperationContract]
        void logMessage(int logLevel, string message, string messageDescr = "");


        ////////////////////////////////////////////////////////////////////////////// Gasket Manager functions //////////////////////////////////////////////

        /// <summary>
        /// Login for the GaskMan application
        /// Will check tha gasketLevel (will be 5 for a user or 10 for an administrator)
        /// If the user is MaSa (Mattias Samuelsson) then the gasketLevel will be 10 without
        /// checking.
        /// </summary>
        /// <param name="login"></param>
        /// <returns>LoginAdm class. Check for errors</returns>
        /// 2018-08-14 kjbo
        [OperationContract]
        LoginAdm GLogin(LoginAdm login);


        /// <summary>
        /// Returns a list of all registered materials
        /// </summary>
        /// <param name="ident"></param>
        /// <returns></returns>
        /// 2018-08-15 KJBO
        [OperationContract]
        List<gMaterialCL> getAllMaterial(string ident);


        /// <summary>
        /// Returns a list of all registered materials
        /// If materialId <> 0 then only the material ID that
        /// matches the current ID will be returned.
        /// In that case this will be 0 or 1 instance of gMaterialCL
        /// </summary>
        /// <param name="ident"></param>
        /// <returns></returns>
        /// 2018-08-15 KJBO
        [OperationContract]
        List<gMaterialCL> getMaterial(string ident, int materialId);




        /// <summary>
        /// Saves material to database
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="mat"></param>
        /// <returns>The saved material</returns>
        /// 2018-08-15 KJBO
        [OperationContract]
        gMaterialCL saveMaterial(string ident, gMaterialCL mat);




        /// <summary>
        /// Returns registered material sizes
        /// if materialSizeId is 0 then all registered material sizes will be returned
        /// else the material size that matches the materiallSizeId will be returned
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="materialSizeId"></param>
        /// <returns></returns>
        /// 2018-08-16 KJBO 
        [OperationContract]
        List<gMaterialSizeCL> getMaterialSize(string ident, int materialSizeId);


        /// <summary>
        /// Validates and saves material size to the database
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="matSize"></param>
        /// <returns></returns>
        /// 2018-08-16 KJBO
        [OperationContract]
        gMaterialSizeCL saveMaterialSize(string ident, gMaterialSizeCL matSize);


        /// <summary>
        /// Returns a list of material thickness
        /// if parameter materialThicknId = 0 then all registered material thickness will be returned
        /// if the parameter is > 0 then only one (or error) will be returned
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="materialThicknId"></param>
        /// <returns></returns>
        /// 2018-08-16 KJBO
        [OperationContract]
        List<gMaterialThicknCL> getMaterialThickn(string ident, int materialThicknId);

        /// <summary>
        /// Saves a material thickness to the database.
        /// If an error occurs then the return object (of type gMaterialThicknCL)
        /// will have error information
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="thickn"></param>
        /// <returns></returns>
        /// 2018-08-16 KJBO 
        [OperationContract]
        gMaterialThicknCL saveMaterialThickness(string ident, gMaterialThicknCL thickn);


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
        [OperationContract]
        gWorkingCostCL getWorkingCosts(string ident);

        /// <summary>
        /// Saves a working cost.
        /// Will return the saved item with the
        /// new value in workingCostId field (if this is a new item)
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="wc"></param>
        /// <returns></returns>
        /// 2018-08-17 KJBO
        [OperationContract]
        gWorkingCostCL saveWorkingCost(string ident, gWorkingCostCL wc);


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
        [OperationContract]
        List<gGasketCL> getGasket(string ident, int gasketId);



        /// <summary>
        /// Validates and saves a gasket to the database
        /// Always check ErrCode and ErrMessage for errors
        /// Returns the new created gasket
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="gasket"></param>
        /// <returns></returns>
        /// 2018-08-17 KJBO
        [OperationContract]
        gGasketCL saveGasket(string ident, gGasketCL gasket);


        /// <summary>
        /// Delete a registered gasket
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="gasketId"></param>
        /// <returns>Error class.</returns>
        /// 2018-08-21 KJBO Indentive AB
        [OperationContract]
        ErrorCL deleteGasket(string ident, int gasketId);

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
        [OperationContract]
        List<KeyValuePair<int, string>> getGasketType(string ident);


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
        [OperationContract]
        List<gReuseMatCL> getReuseMaterial(string ident, int reuseMatId);

        /// <summary>
        /// Saves reusable material
        /// Returns the new created or changed material object
        /// If an error occurs than that error will be returned
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="reuseMat"></param>
        /// <returns></returns>
        /// 2018-08-31 KJBO
        [OperationContract]
        gReuseMatCL saveReuseMaterial(string ident, gReuseMatCL reuseMat);

        /// <summary>
        /// Returns the deault reusable percent depending on the innerSize of a gasket
        /// Please check errCode before using the result
        /// The only field that shall be accessed is the reusePercentage
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="innerSize"></param>
        /// <returns></returns>
        /// 2018-08-31 KJBO
        [OperationContract]
        gReuseMatCL getReusablePercentage(string ident, Decimal innerSize);


        /// <summary>
        /// Reset export all Pyramid export 
        /// settings for an order
        /// </summary>
        /// <param name="aVart_ordernr"></param>
        /// <param name="ident"></param>
        /// <returns>ErrorCL</returns>
        /// 2018-11-02 KJBO
        [OperationContract]
        ErrorCL resetExport(string aVart_ordernr, string ident);

        /// <summary>
        /// When recreating reservation for "products in stock" = orderArt
        /// then we need to send "some at the time" because of timouts in
        /// the communication with Pyramid
        /// This means that this function is repetedly called until
        /// the return value total_handled equals to total_count then
        /// all orderArt rows for one order is resent
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="aVart_ordernr"></param>
        /// <param name="noToResend"></param>
        /// <returns></returns>
        /// 2018-11-07 KJBO
        [OperationContract]
        resendOrderArtCL resendxOrderArt(string aVart_ordernr, int noToResend, string ident);

        /// <summary>
        /// Returns a list of order changes for the current ordernr
        /// Returns open/close/reset events for the order
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="vartOrdernr"></param>
        /// <returns></returns>
        /// 2018-11-09
        [OperationContract]
        List<pyramidChangeCL> getPyramidChange(string ident, string vartOrdernr);







    }


    ////////////////////////////////////////////////////////////////////////////// Gasket Manager DataContract //////////////////////////////////////////////

    [DataContract]
    public class gMaterialCL
    {
        [DataMember]
        public int materialId // Unique identity
        { get; set; }

        [DataMember]
        public string material // Materialnamn
        { get; set; }

        [DataMember]
        public string materialShort // Kortnamn för material
        { get; set; }


        [DataMember]
        public int ErrCode
        { get; set; }

        [DataMember]
        public string ErrMessage
        { get; set; }

    }

    [DataContract]
    public class gMaterialSizeCL
    {
        [DataMember]
        public int materialSizeId // Primary key
        { get; set; }

        [DataMember]
        public int materialId // Foreign key to material
        { get; set; }


        [DataMember]
        public string description //Human readable description of material and size
        { get; set; }

        [DataMember]
        public string sizeShort // Short to material size (in order to build article ID)
        { get; set; }

        [DataMember]
        public Decimal materialLength //Length of material (in meters)
        { get; set; }

        [DataMember]
        public Decimal materialWidth // Width of material (in meters)
        { get; set; }


        [DataMember]
        public bool defaultVal //If this is a default value (shall be in top of dropdown)
        { get; set; }

        [DataMember]
        public string materialName //Readonly from material table
        { get; set; }


        [DataMember]
        public int ErrCode
        { get; set; }

        [DataMember]
        public string ErrMessage
        { get; set; }

    }

    [DataContract]
    public class gMaterialThicknCL // Thickness of material
    {

        [DataMember]
        public int materialThicknId // Primary key
        { get; set; }

        [DataMember]
        public int materialSizeId // Foreign key to material size
        { get; set; }


        [DataMember]
        public string description //Human readable description of material and size
        { get; set; }

        [DataMember]
        public string thicknShort // Short to material size (in order to build article ID)
        { get; set; }

        [DataMember]
        public Decimal thickness //Thickness of material in mm
        { get; set; }

        [DataMember]
        public Decimal buyPrice // Inprice
        { get; set; }

        [DataMember]
        public Decimal sellPrice // Sales price
        { get; set; }

        [DataMember]
        public Decimal cuttingTime // In meter/sek
        { get; set; }

        [DataMember]
        public string materialName //Readonly from material table
        { get; set; }

        [DataMember]
        public string materialSize //Readonly from materialSize table
        { get; set; }

        [DataMember]
        public int ErrCode
        { get; set; }

        [DataMember]
        public string ErrMessage
        { get; set; }

    }


    [DataContract]
    public class gWorkingCostCL // Thickness of material
    {
        [DataMember]
        public int workingCostId // Primary key
        { get; set; }

        [DataMember]
        public Decimal cuttingHourNet // Net cost for cutting
        { get; set; }

        [DataMember]
        public Decimal cuttingHourSales // Sales price for cutting
        { get; set; }

        [DataMember]
        public Decimal handlingHourNet // Net cost for handling
        { get; set; }

        [DataMember]
        public Decimal handlingHourSales // Sales cost for handling
        { get; set; }

        [DataMember]
        public Decimal cuttingMargin // cutting margin (mm)
        { get; set; }

        [DataMember]
        public int ErrCode
        { get; set; }

        [DataMember]
        public string ErrMessage
        { get; set; }

    }

    [DataContract]
    public class gGasketCL // Gasket
    {
        [DataMember]
        public int gasketId // Primary key
        { get; set; }

        [DataMember]
        public int gasketTypeId // Foreign key to gasketType
        { get; set; }

        [DataMember]
        public int materialThicknId // Foreign key to material thickness -> materialSize -> material
        { get; set; }

        [DataMember]
        public Decimal outerDiam // Outer diameter in mm
        { get; set; }

        [DataMember]
        public Decimal innerDiam // Inner diameter in mm
        { get; set; }

        [DataMember]
        public Decimal reusableMaterial // Reusable material in percent
        { get; set; }

        [DataMember]
        public Decimal cuttingMargin // Cutting margin in mm
        { get; set; }

        [DataMember]
        public bool standardPriceProduct // Whether this is a standard price or not
        { get; set; }

        [DataMember]
        public Decimal handlingTime // Handling time in seconds
        { get; set; }

        [DataMember]
        public Decimal price // Selling price
        { get; set; }

        [DataMember]
        public string note
        { get; set; }

        [DataMember]
        public int Type2SecHoleCount // Selling price
        { get; set; }

        [DataMember]
        public Decimal Type2SecHoleDiam // Selling price
        { get; set; }

        [DataMember]
        public string description // beskrivning
        { get; set; }


        [DataMember]
        public string materialName //Readonly from material table
        { get; set; }

        [DataMember]
        public string materialSizeName //Readonly from materialSize table
        { get; set; }

        [DataMember]
        public string materialThicknName //Readonly from materialthickness table
        { get; set; }

        [DataMember]
        public string gasketTypeName //Readonly from materialthickness table
        { get; set; }

        [DataMember]
        public Double materialCostMm2 //Material cost
        { get; set; }

        [DataMember]
        public Double materialMarginPercent
        { get; set; }

        [DataMember]
        public Double cuttingLengthOuterMm //Cutting length
        { get; set; }

        [DataMember]
        public Double cuttingLengthInnerMm //Cutting length
        { get; set; }

        [DataMember]
        public Double cuttingSpeedMmSek //Cutting speed in m/sek
        { get; set; }

        [DataMember]
        public Double materialArea
        { get; set; }

        [DataMember]
        public int ErrCode
        { get; set; }

        [DataMember]
        public string ErrMessage
        { get; set; }

    }


    [DataContract]
    public class gReuseMatCL
    {
        [DataMember]
        public int reuseMatId // Primary key
        { get; set; }

        [DataMember]
        public Decimal minDiam // Min diameter
        { get; set; }

        [DataMember]
        public Decimal maxDiam // Max diameter calculated
        { get; set; }


        [DataMember]
        public Decimal reusePercentage // Reusable percentage
        { get; set; }



        [DataMember]
        public int ErrCode
        { get; set; }

        [DataMember]
        public string ErrMessage
        { get; set; }


    }

    [DataContract]
    public class LoginAdm
    {
        [DataMember]
        public string AnvID // Unique identity
        { get; set; }

        [DataMember]
        public string pwd // Password
        { get; set; }

        [DataMember]
        public string reparator // Name of the reparator
        { get; set; }

        [DataMember]
        public string ident // Ident that is used in later calls
        { get; set; }       // To be stored in the client

        [DataMember]
        public int gasketLevel // Used by GaskMan to store access level
        { get; set; }

        [DataMember]
        public bool canResetPyramid
        { get; set; }


        [DataMember]
        public int ErrCode
        { get; set; }

        [DataMember]
        public string ErrMessage
        { get; set; }



    }

    [DataContract]
    public class ServHuvCL
    {
        [DataMember]
        public string Vart_ordernr
        { get; set; }

        [DataMember]
        public string Ert_ordernr
        { get; set; }

        [DataMember]
        public DateTime OrderDate
        { get; set; }

        [DataMember]
        public string Kund
        { get; set; }

        [DataMember]
        public bool Godkand
        { get; set; }

        [DataMember]
        public DateTime Godkand_dat
        { get; set; }

        [DataMember]
        public bool OpenForApp
        { get; set; }

        [DataMember]
        public string OrderAdmin
        { get; set; }
        [DataMember]
        public bool IsNew
        { get; set; }
        [DataMember]
        public bool AllRep
        { get; set; }
        [DataMember]
        public DateTime FromDate
        { get; set; }
        [DataMember]
        public DateTime ToDate
        { get; set; }
        [DataMember]
        public int momskod
        { get; set; }
        [DataMember]
        public string generalNote
        { get; set; }

        [DataMember]
        public string orderLabel // Ordermärkning 2018-08-23
        { get; set; }

        [DataMember]
        public int ErrCode
        { get; set; }

        [DataMember]
        public string ErrMessage
        { get; set; }
    }



    [DataContract]
    public class ListServHuvCL
    {
        [DataMember]
        public string vart_ordernr
        { get; set; }

        [DataMember]
        public string ert_ordernr
        { get; set; }

        [DataMember]
        public string orderDate
        { get; set; }

        [DataMember]
        public string kund
        { get; set; }

        [DataMember]
        public string reparator_msg
        { get; set; }

        [DataMember]
        public string week_msg
        { get; set; }

        [DataMember]
        public bool sentToPyramid
        { get; set; }

        [DataMember]
        public string pyramidError
        { get; set; }

        [DataMember]
        public string orderLabel
        { get; set; }

        [DataMember]
        public int ErrCode
        { get; set; }

        [DataMember]
        public string ErrMessage
        { get; set; }

    }

    [DataContract]
    public class ListKundCL
    {
        [DataMember]
        public string kund_id
        { get; set; }

        [DataMember]
        public string kund
        { get; set; }

        [DataMember]
        public int ErrCode
        { get; set; }

        [DataMember]
        public string ErrMessage
        { get; set; }

    }


    [DataContract]
    public class ListRepCL
    {
        [DataMember]
        public string AnvId
        { get; set; }

        [DataMember]
        public string Reparator
        { get; set; }

        [DataMember]
        public int ErrCode
        { get; set; }

        [DataMember]
        public string ErrMessage
        { get; set; }

    }

    [DataContract]
    public class ShReparatorCL
    {
        [DataMember]
        public int Id
        { get; set; }

        [DataMember]
        public string VartOrdernr
        { get; set; }

        [DataMember]
        public string AnvID
        { get; set; }
        [DataMember]
        public int ErrCode
        { get; set; }

        [DataMember]
        public string ErrMessage
        { get; set; }

    }

    [DataContract]
    public class OrderArtListCL
    {
        [DataMember]
        public string vartOrdernr
        { get; set; }
        [DataMember]
        public string ansvarig
        { get; set; }
        [DataMember]
        public string artnr
        { get; set; }
        [DataMember]
        public string artnamn
        { get; set; }
        [DataMember]
        public int antalCo
        { get; set; }
        [DataMember]
        public int antalOrder
        { get; set; }
        [DataMember]
        public int ErrCode
        { get; set; }

        [DataMember]
        public string ErrMessage
        { get; set; }
    }



    [DataContract]
    public class OrderArtCL
    {

        [DataMember]
        public int OrderArtId // Unique identity
        { get; set; }

        [DataMember]
        public string VartOrdernr // Vårt ordernr
        { get; set; }


        [DataMember]
        public string Artnr // Artikelnr
        { get; set; }
        [DataMember]
        public string ArtNamn // Artikelbenämning
        { get; set; }
        [DataMember]
        public string Artikelkod // Artikelkod i Pyramid
        { get; set; }

        [DataMember]
        public Decimal CoAntal // Checkout antal
        { get; set; }

        [DataMember]
        public Decimal OrdAntal // Använt på aktuell order
        { get; set; }

        [DataMember]
        public Decimal CiAntal // Checkin antal
        { get; set; }

        [DataMember]
        public Decimal Stock // Antal i lager (Pyramid)
        { get; set; }


        [DataMember]
        public Decimal TempCiAntal // Temporär utcheckning
        { get; set; }

        [DataMember]
        public string note
        { get; set; }


        [DataMember]
        public int ErrCode
        { get; set; }

        [DataMember]
        public string ErrMessage
        { get; set; }

    }




    [DataContract]
    public class ErrorCL
    {

        [DataMember]
        public int ErrCode
        { get; set; }

        [DataMember]
        public string ErrMessage
        { get; set; }

    }

    [DataContract]
    public class MomskodCL
    {
        [DataMember]
        public int MomskodId // Unique identity
        { get; set; }

        [DataMember]
        public string Momskod // Description of moms
        { get; set; }

        [DataMember]
        public int ErrCode
        { get; set; }

        [DataMember]
        public string ErrMessage
        { get; set; }
    }


        [DataContract]
        public class resendOrderArtCL
        {
            [DataMember]
            public string vartOrdernr 
            { get; set; }

            [DataMember]
            public int total_count //The total number of orderArt belonging to this order
            { get; set; }
            [DataMember]
            public int total_handled //The total number of orderArt handled for this order
            { get; set; }


            [DataMember]
            public int ErrCode
            { get; set; }

            [DataMember]
            public string ErrMessage
            { get; set; }

        }

    [DataContract]
    public class pyramidChangeCL
    {
        [DataMember]
        public string vartOrdernr
        { get; set; }

        [DataMember]
        public int ChangeTypeId // Orderstatus change number
        { get; set; }

        [DataMember]
        public string ChangeTypeName //Name of the order status
        { get; set; }

        [DataMember]
        public string reg
        { get; set; }

        [DataMember]
        public string regName
        { get; set; }

        [DataMember]
        public DateTime regdate
        { get; set; }

        [DataMember]
        public int ErrCode
        { get; set; }

        [DataMember]
        public string ErrMessage
        { get; set; }

    }


}









