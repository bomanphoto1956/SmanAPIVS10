using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.IO;
using System.ServiceModel.Web;

namespace SManApi
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ISmServ" in both code and config file together.
    [ServiceContract]
    public interface ISmServ
    {

        /// <summary>
        /// Login, creates and return a valid ident
        /// Ident is valid 24 hours after a valid login
        /// </summary>
        /// <param name="AnvID">UserID</param>
        /// <param name="pwd">Password</param>
        /// <returns>identity or empty string</returns>
        /// 2016-02-03 KJBO Pergas AB
        [OperationContract]
        string login(string AnvID, string pwd);


        /// <summary>
        /// Given a valid ident this will
        /// return all reparator information        
        /// </summary>
        /// <param name="ident">Identity</param>
        /// <returns></returns>
        /// 2016-02-03 KJBO Pergas AB
        [OperationContract]
        ReparatorCL getReparator(string ident);

        /// <summary>
        /// Given a valid identity this
        /// will return all active reparaors
        /// </summary>
        /// <param name="ident">Identity</param>
        /// <returns>List of reparators</returns>
        [OperationContract]
        List<ReparatorCL> getReparators(string ident);


        /// <summary>
        /// Get all reparators 
        /// or just the reparator with AnvID
        /// </summary>
        /// <returns>List of reparators</returns>
        /// 2018-08-21 KJBO
        [OperationContract]
        List<ReparatorCL> gGetReparators(string ident, string AnvID);


        /// <summary>
        /// Given a valid identity this function will
        /// return all ServHuv rows that this reparator 
        /// has access to.
        /// Either by being the administrator of this Service
        /// or that all reparators will have acess to all
        /// service open for App or that the reparator
        /// is in the list of reparators that have access to
        /// this Service.
        /// </summary>
        /// <param name="ident">Identity</param>
        /// <returns>List of servicehuvud</returns>
        /// 2016-02-03 KJBO Pergas AB
        [OperationContract]
        List<ServiceHuvudCL> getServHuvForUser(string ident);

        /// <summary>
        /// Given a valid identity this function return
        /// one serviceHuvud identified by vartOrdernr
        /// </summary>
        /// <param name="ident"></param>
        /// <returns>one ServiceHuvud</returns>
        /// 2016-02-03 KJBO Pergas AB
        [OperationContract]
        ServiceHuvudCL getServHuv(string ident, string vartOrdernr);



        /// <summary>
        /// Returns all servicerad for one ordernr
        /// Intended for displaying and selecting row to
        /// edit. When editing one row use the ServRadCL
        /// object and getServRad function.
        /// </summary>
        /// <param name="ident">Identity</param>
        /// <param name="vartOrdernr"></param>
        /// <returns>List of servicerader</returns>
        /// 2016-02-03 KJBO Pergas AB
        [OperationContract]
        List<ServiceRadListCL> getAllServRad(string ident, string vartOrdernr);



        /// <summary>
        /// Use this function to retieve one row in order to
        /// updat the fields
        /// </summary>
        /// <param name="ident">Identity</param>
        /// <param name="vartOrdernr"></param>
        /// <param name="radnr"></param>
        /// <returns>One servicerad</returns>
        [OperationContract]
        ServiceRadCL getServRad(string ident, string vartOrdernr, int radnr);


        /// <summary>
        /// Save a new or changed sericerow to the databas
        /// </summary>
        /// <param name="sr">ServiceRadCL to be saved</param>
        /// <param name="ident">Identity</param>
        /// <returns>The updated servicerow or an error on the first row</returns>
        // 2016-02-08 KJBO Pergas AB
        [OperationContract]
        ServiceRadCL saveServRad(string ident, ServiceRadCL sr);

        /// <summary>
        /// Use this function to retrieve one ventil
        /// in order to read or update the information
        /// The primary key is a GUID and the foreign
        /// key field is found in ServiceRadCL class
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="ventilID"></param>
        /// <returns></returns>
        /// 2016-02-03 KJBO
        [OperationContract]
        VentilCL getVentil(string ident, string ventilID);


        /// <summary>
        /// Return a list of ventil for one customer
        /// </summary>
        /// <param name="ident">Ident</param>
        /// <param name="KundID">KundID</param>
        /// <returns>List of ventiCL</returns>
        //  2016-02-08 KJBO Pergas AB
        [OperationContract]
        List<VentilCL> getVentilsForCust(string ident, string KundID, string position, string IDnr, string ventiltyp, string fabrikat, string anlaggningsnr);



        /// <summary>
        /// Save a ventil. If the ventil_id is empty
        /// then the API assumes a new ventil
        /// Returns the new saved (inserted or updated)
        /// ventil or one row with error and error message
        /// if a validation fails
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        [OperationContract]
        VentilCL saveVentil(string ident, VentilCL v);




        /// <summary>
        /// For use in comboboxes
        /// Returns an ID/value combination in
        /// order to see the value and store the ID
        /// </summary>        
        /// 2016-02-04 KJBO
        [OperationContract]
        List<VentilKategoriCL> getVentilKategorier(string ident);


        /// <summary>
        /// Get a list of fabrikat to be used for ComboBoxes
        /// for ventil, stalldon and lagesstallare 
        /// </summary>
        /// <param name="ident"></param>
        /// <returns></returns>
        /// 2016-02-04 KJBO
        [OperationContract]
        List<FabrikatCL> getFabrikat(string ident);


        /// <summary>
        /// Get a list of dn to be used for Comboboxes
        /// for ventil when selecting dn/dn2
        /// </summary>
        /// <param name="ident"></param>
        /// <returns></returns>
        /// 2016-02-04 KJBO
        [OperationContract]
        List<DnCL> getDn(string ident);

        /// <summary>
        /// Get a list of pn to be used for Comboboxes
        /// for ventil when selecting pn/pn2
        /// </summary>
        /// <param name="ident"></param>
        /// <returns></returns>
        /// 2016-02-04 KJBO
        [OperationContract]
        List<PnCL> getPn(string ident);

        /// <summary>
        /// Get a list of artikel for displaying purposes
        /// </summary>
        /// <param name="ident">Identity</param>
        /// <param name="ArtnrFilter">Artnr or part of..</param>
        /// <param name="ArtnamnFilter">ArtNamn or part of..</param>
        /// <returns></returns>
        // 2016-02-09 KJBO 
        [OperationContract]
        List<ArtikelCL> getArtList(string ident, string ArtnrFilter, string ArtnamnFilter);


        /// <summary>
        /// Return one artikel
        /// </summary>
        /// <param name="ident">Ident</param>
        /// <param name="Artnr">Artnr</param>
        /// <returns></returns>
        // 2016-02-10 KJBO
        [OperationContract]
        ArtikelCL getArtikel(string ident, string Artnr);

        /// <summary>
        /// Return a list of reservdel for one servicerad
        /// </summary>
        /// <param name="ident">Ident</param>
        /// <param name="VartOrdernr">VartOrdernr</param>
        /// <param name="RadNr">Radnr</param>
        /// <returns>List of reservdel or one row with error</returns>
        // 2016-02-10 KJBO
        [OperationContract]
        List<ReservdelCL> getReservdelsForServiceRad(string ident, string VartOrdernr, int RadNr);

        /// <summary>
        /// Get one reservdel identified by primary key
        /// </summary>
        /// <param name="ident">identity</param>
        /// <param name="VartOrdernr"></param>
        /// <param name="RadNr"></param>
        /// <param name="ReservNr"></param>
        /// <returns>The reservdel or an error</returns>
        //  2016-02-10 KJBO Pergas AB
        [OperationContract]
        ReservdelCL getReservdel(string ident, string VartOrdernr, int RadNr, int ReservNr);

        /// <summary>
        /// Saves a reservdel to database.
        /// If ReservNr = 0 then the method
        /// assumes that this is a new row to be added
        /// Otherwise an update is issued
        /// </summary>
        /// <param name="ident">Identity</param>
        /// <param name="reservdel">ReservdelCL</param>
        /// <returns>The new created or updated reservdel</returns>
        //  2016-02-10 KJBO
        [OperationContract]
        ReservdelCL saveReservdel(string ident, ReservdelCL reservdel);


        /// <summary>
        /// Returns a list of valid dates for
        /// registry of time for one ServiceRow
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="SrAltKey">Alternate key</param>
        /// <returns>List of dates or an error message</returns>
        [OperationContract]
        List<OpenDateCL> getOpenDates(string ident, string SrAltKey);

        /// <summary>
        /// Returns a list of valid dates for
        /// registry of time for one ServiceOrder
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="vartOrdernr"></param>
        /// <returns></returns>
        /// 2017-03-15 KJBO
        [OperationContract]
        List<OpenDateCL> getOpenDatesSH(string ident, string vartOrdernr);



        /// <summary>
        /// Get a specific TidRed record identified by ID (PK)
        /// </summary>
        /// <param name="ident">Identity</param>
        /// <param name="ID">ID of the ServRadRepTid</param>
        /// <returns>One instance of the ServRadRepTidCL class or one row with an error</returns>
        // 2016-02-15 KJBO Pergas AB
        [OperationContract]
        ServRadRepTidCL getServRadRepTid(string ident, int ID);


        /// <summary>
        /// Returns all registered time(all rows)
        /// for a specific service row (identified by srAltKey)
        /// and for a specific user (identified by AnvID)
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="AnvID"></param>
        /// <param name="srAltKey"></param>
        /// <returns></returns>
        //  2016-02-18 Pergas AB KJBO
        [OperationContract]
        List<ServRadRepTidCL> getServRadRepTidForServiceRad(string ident, string AnvID, string srAltKey);


        /// <summary>
        /// Returns all registered time(all rows)
        /// for a specific service row (identified by srAltKey)
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="srAltKey"></param>
        /// <returns></returns>
        //  2016-11-18 Pergas AB KJBO
        [OperationContract]
        List<ServRadRepTidCL> getServRadRepTidForSR(string ident, string srAltKey);



        /// <summary>
        /// Validates one ServRadRepTid
        /// If the ID is 0 then this method
        /// assumes that this is a new row
        /// Returns the validated and stored
        /// row with the new ID (if its a new row)
        /// If an error occurs then an error is returne
        /// in the ServRadTidRep return row
        /// </summary>
        /// <param name="ident">Identity</param>
        /// <param name="srt">ServRadTidRepCL</param>
        /// <returns>The saved row or an error</returns>
        //  2016-02-15 KJBO Pergas AB
        [OperationContract]
        ServRadRepTidCL saveServRadRepTid(string ident, ServRadRepTidCL srt);


        /// <summary>
        /// Check if an order is open for editing
        /// The return value is a string, 1 = open, -1 = closed or an error message
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="VartOrdernr"></param>
        /// <returns>1-Open -1-Closed or an error message</returns>
        //  2016-02-15 KJBO Pergas AB
        [OperationContract]
        string isOpen(string ident, string VartOrdernr);


        /// <summary>
        /// Get a list of all reparators assigned to one 
        /// servicehuvud identified by vartOrdernr
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="vartOrdernr"></param>
        /// <returns>A list of reparators or error</returns>
        /// 2017-03-14 Added functionality
        /// RepKatID is now current for this ordernr        
        [OperationContract]
        List<ReparatorCL> getReparatorsForServiceHuvud(string ident, string vartOrdernr);


        /// <summary>
        /// Returns the alternate key for a serviceRad
        /// </summary>
        /// <param name="vartOrdernr"></param>
        /// <param name="radnr"></param>
        /// <returns>If ident is invalid the function returns "-10"</returns>
        /// <returns>If a database error occurs then the return is "-1" followed by the database error description"</returns>        
        /// <returns>If no row is found by the provided primary key then the result is an empty string</returns>
        /// <returns>In the normal case (identity is OK and the primary key exists) the function return the alternate key</returns>
        //  2016-02-29 Pergas AB KJBO
        [OperationContract]
        string getAlternateKey(string ident, string vartOrdernr, int radnr);



        // Picture handling.
        // The first step is to upload the picture 
        // in a stream (see UploadPict) below
        // If the upload operation is successful then
        // it returns an identity of the uploaded file
        // (otherwise it returns an error string starting with -1
        // Immidiatelly after a successful upload the method 
        // commitPicture shall be called. This method stores
        // the picture to the database together with 
        // the medadata provided in PictureCL class
        // If an error occurs during upload of the picture

        /// <summary>
        /// Stores a picture or an image to the
        /// local directory "UpLoads"
        /// The name of the picture is a GUID
        /// which is returned to the caller on success
        /// Note that this function doesn't have any 
        /// identity provided. This is because of limitation
        /// of this Rest API where a method that receives a stream
        /// as parameter can not have any other parameters.
        /// </summary>
        /// <param name="sPict">Picture as stream</param>
        /// <returns>The name of the picture (that has to be referred in
        /// future calls when this picture shall be stored with metadata
        /// If an error occurs then the return string is -1 followed by an
        /// error message</returns>
        //  2016-03-03 KJBO
        [OperationContract]
        string uploadPict(Stream sPict);


        /// <summary>
        /// Saves a picture to the database
        /// This method shall be called directory after
        /// a call to UploadPict
        /// The UploadPict gives you (upon success)
        /// an identity (=filename) to the upoaded file
        /// This identity is provided to this function
        /// in the PictureCL class
        /// Note that the BildNr field in the PictureClass
        /// shall always be 0 indicating that this is a
        /// new picture to be stored. There is no way
        /// to update a picture. In that case you need to delete
        /// the picture and, after that, add a new one
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        //  2016-03-07 KJBO
        [OperationContract]
        PictureCL savePicture(string ident, PictureCL p);




        /// <summary>
        /// Get a picture from the database identified by
        /// primary key (vartOrdernr, radnr, bildNr)
        /// Returns a PictCL object with the pictIdent
        /// field with a file name to the file being extracted
        /// by the server.
        /// If the fileName is empty or begins with -1 then
        /// there is an error while extracting the picture from
        /// the database to the temporary storage
        /// 
        /// After this function is called there has to be a call
        /// to downloadPicture with the pictIdent as parameter
        /// This function returns the picture to the caller as
        /// a memoryStream
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="vartOrdernr"></param>
        /// <param name="radnr"></param>
        /// <param name="bildNr"></param>
        /// <returns></returns>
        /// 2016-03-09 KJBO
        [OperationContract]
        PictureCL getPicture(string ident, string vartOrdernr, int radnr, int bildNr);



        /// <summary>
        /// The downLoadPict method accept a pictIdent parameter as well
        /// as a reference to an error string
        /// The method returns a memorystream to the caller. If an
        /// error occurs then the stream is null        
        /// 
        /// This method shall be called after a call to getPicture. When getPicture
        /// is called it will store a copy of the picture on the server and also return
        /// a pictureCL object with the pictIdent. This identity is used when this 
        /// method is called.
        /// </summary>
        /// <param name="pictIdent"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        /// 2016-03-10 KJBO Pergas AB
        [OperationContract]
        Stream downLoadPict(string pictIdent);

        /// <summary>
        /// Delete a picture from the database identified by
        /// values provided in the PictureCL parameter
        /// vartOrdernr
        /// radnr
        /// bildNr
        /// 
        /// Note that the pictIdent parameter doesnt need to
        /// be filled in this case.
        ///         
        /// The method returns an empty picture class if 
        /// everything is OK
        /// If anything goes wrong the errCode and the errMessage
        /// will give further information. 
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        /// 2016-03-11 Pergas AB KJBO
        [OperationContract]
        PictureCL deletePicture(string ident, PictureCL p);



        /// <summary>
        /// This method returns all pictures for one servicerad
        /// Note that you dont get the actual picture nor the
        /// pictIdent. Instead you use this method for getting a
        /// list of available pictures (and also gets the picture
        /// description).
        /// After that you have to call GetPicture and download picture
        /// in turn in order to get each individual picture.
        /// The reason for this is performance. This method gives
        /// a fast list of available pictures only.
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="vartOrdernr"></param>
        /// <param name="radnr"></param>
        /// <returns></returns>
        /// 2016-03-11 Pergas AB kjbo
        [OperationContract]
        List<PictureCL> getPicturesForServiceRad(string ident, string vartOrdernr, int radnr);



        /// <summary>
        /// Returns all standardtext
        /// See details in the StandardTextCL class below 
        /// </summary>
        /// <param name="ident"></param>
        /// <returns></returns>
        [OperationContract]
        List<StandardTextCL> getAllSttText(string ident);


        /// <summary>
        /// Function to get a list of picture categories
        /// The step parameter indicates the step in the
        /// documentation where the different categories are available
        /// as follows
        /// 1 : Check before service
        /// 2 : Service job done
        /// 3 : Other remarks
        /// 4 : Spare parts
        /// 0 : Anywhere (no matter where the picture is taken i the process)
        /// 
        /// If Step is set to 0 you get all possible categories in return
        /// Otherwise you get the categories for the current step as well as
        /// category all categories with step 0.
        /// Note that it is required with 2 pictures (of differend categories)
        /// for step no 1 in the process
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="Step"></param>
        /// <returns></returns>
        [OperationContract]
        List<PictCatCL> getPictCategories(string ident, int Step);


        /// <summary>
        /// Updates the picture metadata.
        /// Note that the picture must exist, identified
        /// by the following properties in the picture class:
        /// VartOrdernr, Radnr, BidlNr.
        /// For performance reason this method does not evaluate
        /// the picture size.
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        [OperationContract]
        PictureCL updatePictMetadata(string ident, PictureCL p);

        /// <summary>
        /// Returns database and API versions
        /// </summary>
        /// <param name="ident"></param>
        /// <returns></returns>
        [OperationContract]
        VersionCL getVersion(string ident);


        /// <summary>
        /// Sum all registered hours for one servicerad
        /// </summary>
        /// <param name="ident">ident</param>
        /// <param name="srAltKey">alternate key for servicerad</param>
        /// <param name="AnvID">UserID or empty string for all users</param>
        /// <returns>Number of hours or -1 if error occurs</returns>
        /// 2016-06-17 KJBO
        [OperationContract]
        Decimal SumHoursForServRad(string ident, string srAltKey, string AnvID);


        /// <summary>
        /// Deletes a reservdel identified by primary key
        /// </summary>
        /// <param name="ident">identity string</param>
        /// <param name="reservdel">One valid reservdel</param>
        /// <returns>Empty string if OK otherwise error message</returns>
        /// 2016-09-30 KJBO
        [OperationContract]
        string deleteReservdel(string ident, ReservdelCL reservdel);


        /// <summary>
        /// Returns all time registry for a given order
        /// </summary>
        /// <param name="ident">Identity</param>
        /// <param name="vartOrdernr">Order number</param>
        /// <returns>List of RepTidListCL</returns>
        /// 2016-11-01 KJBO
        [OperationContract]
        List<RepTidListCL> getAllTimeForOrder(string ident, string vartOrdernr);



        /// <summary>
        /// Return a list of valid timeTypes
        /// The list varies depending on the hosKund and paVerkstad parameters
        /// Normaly you check the corresponding Servicerad and the hosKund and paVerkstad
        /// and send those variables to this function thus getting the right list.
        /// To override (and get alla timeTypes) you just set both parameters to true
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="hosKund"></param>
        /// <param name="paVerkstad"></param>
        /// <param name="all"></param>
        /// <returns></returns>
        /// 2016-11-01 KJBO
        [OperationContract]
        List<TimeTypeCL> getTimeTypes(string ident, bool hosKund, bool paVerkstad);


        /// <summary>
        /// Get valid time types for one order
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="vartOrdernr"></param>
        /// <returns>Valid time types</returns>
        // 2017-03-14 KJBO
        [OperationContract]
        List<TimeTypeCL> getTimeTypesForOrder(string ident, string vartOrdernr);



        /// <summary>
        /// Get the current timeRegVersion
        /// can be either 1 or 2
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="vartOrdernr"></param>
        /// <returns>Version or -1 for invalid ident or -2 for database error 
        /// (no more error description is available for this function</returns>
        /// 2017-03-10 KJBO
        [OperationContract]
        int getTimeRegVersion(string ident, string vartOrdernr);


        /// <summary>
        /// Return all salarts either for servicedetalj or serviceorder 
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="forServiceDetalj"></param>
        /// <returns>List of salart or error code</returns>
        /// 2017-03-13 KJBO
        [OperationContract]
        List<SalartCL> getSalart(string ident, bool forServiceDetalj);


        /// <summary>
        /// Get all available repKat for 
        /// timeregistration version 2
        /// </summary>
        /// <param name="ident"></param>
        /// <returns></returns>
        /// 2017-03-14 KJBO
        [OperationContract]
        List<RepKatCL> getRepKat(string ident);

        /// <summary>
        /// Validates one ServHuvRepTid
        /// If the ID is 0 the this method
        /// assumes that this is a new row
        /// Returns the validated and stored
        /// row with the new ID (if its a new row)
        /// If an error occurs then an error is returned
        /// in the ServHuvTidRep return row
        /// </summary>
        /// <param name="ident">Identity</param>
        /// <param name="sht">ServHuvRepTid</param>
        /// <returns></returns>
        /// 2017-03-15 KJBO
        [OperationContract]
        ServHuvRepTidCL saveServHuvRepTid(string ident, ServHuvRepTidCL sht);


        /// <summary>
        /// Get one row of ServHuvRepTid identified by PK
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        /// 2017-03-15 KJBO
        [OperationContract]
        ServHuvRepTidCL getServHuvRepTid(string ident, int ID);

        /// <summary>
        /// Get all ServHuvRepTid for one order
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="vartOrdernr"></param>
        /// <returns></returns>
        /// 2017-03-15 KJBO
        [OperationContract]
        List<ServHuvRepTidCL> getServHuvRepTidForSH(string ident, string vartOrdernr);


        /// <summary>
        /// Get all ServHuvRepTid for one order and 
        /// one user ( = anv)
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="vartOrdernr"></param>
        /// <param name="anvID"></param>
        /// <returns></returns>
        /// 2017-03-15 KJBO
        [OperationContract]
        List<ServHuvRepTidCL> getServHuvRepTidForShAnv(string ident, string vartOrdernr, string anvID);

        /// <summary>
        /// Determines if the AnvID is administrator for the current order
        /// If so the function will return the administrator RepCat.
        /// Otherwise it will return the default RepKat.
        /// repKat
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="AnvID"></param>
        /// <param name="VartOrdernr"></param>
        /// <returns></returns>
        /// 2017-03-21 KJBO
        [OperationContract]
        RepKatCL getDefaultRepKat(string ident, string AnvID, string VartOrdernr);


        /// <summary>
        /// Initiate creation of timeRep2Report
        /// parameter p must have at least a VartOrdernr and an email (for the returning mail)
        /// The return value is a filled instance of TimeRep2ProcessCL with init values.
        /// To check the status of the report generation, call getTimeRep2ReportStatus(string VartOrdernr)
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="p"></param>
        /// <param name="bOverrideExisting"></param>
        /// <returns>A filled TimeRep2ProcessCL</returns>
        /// 2017-03-21  KJBO
        /// 2017-05-19 KJBO Added approve parameter
        /// 2017-09-10 KJBO Added detailed parameter
        /// 2017-09-20 KJBO Removed detailed parameter. Using p.ReportType to indicate detail level
        /// where 4 = standard report and 5 = detailed report
        /// 2017-10-24 KJBO Added timeRep2WeekIds which is a list of integer
        /// representing primary key of timeRep2Week. This is the weeks that shall be reported this time
        /// 2017-10-25 KJBO Added kundEmails list to indicate which emails that shall receive the attested
        /// timereport.
        [OperationContract]
        TimeRep2ProcessCL generateTimeReg2Report(string ident, TimeRep2ProcessCL p, bool bOverrideExisting, bool approve, List<int> timeRep2WeekIds, List<KundEmailCL> kundEmails);

        /// <summary>
        /// After calling generateTimeReg2Report() there is a possibility
        /// to check the status of the report generation process.
        /// Call this function and you get a status report back
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="VartOrdernr"></param>
        /// <returns>Instance of TimeRep2ProcessCL</returns>
        /// 2017-03-21 KJBO
        [OperationContract]
        TimeRep2ProcessCL getTimeRep2ReportStatus(string ident, string VartOrdernr);


        /// <summary>
        /// This method shall be called in order to display
        /// a list of weeks to be included int the report
        /// Note that there can be weeks that are approved
        /// and those weeks shall not be selectable (only displayed)
        /// 
        /// </summary>
        /// <param name="ident">Needs no explanation</param>
        /// <param name="VartOrdernr">The current order</param>
        /// <returns>List of TimeRep2WeekCL</returns>
        //  2017-10-23 KJBO
        [OperationContract]
        List<TimeRep2WeekCL> getTimeRep2Weeks(string ident, string VartOrdernr);



        /// <summary>
        /// This function shall be called when a contact list for sending timereports is requierd
        /// It gives a list of earlier contacts and also if the current contact
        /// (email address) was selected for having a timeReport in history
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="VartOrdernr"></param>
        /// <returns>List of KundEmailCL or error</returns>
        /// 2017-10-25 KJBO
        [OperationContract]
        List<KundEmailCL> getTimeRecordContactList(string ident, string VartOrdernr);


        /// <summary>
        /// Returns the selectable gasket levels
        /// </summary>
        /// <param name="ident"></param>
        /// <returns></returns>
        /// 2018-08-21 KJBO
        [OperationContract]
        List<KeyValuePair<int, string>> gGetGasketLevels(string ident);

        /// <summary>
        /// Save access level for gasket handling
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="reparator"></param>
        /// <returns></returns>
        /// 2018-08-21 KJBO
        [OperationContract]
        ReparatorCL saveGasketLevel(string ident, ReparatorCL reparator);


        /// <summary>
        /// Save a new fabrikat to the database
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        /// 2019-04-03 KJBO
        [OperationContract]
        FabrikatCL saveFabrikat(string ident, FabrikatCL f);

        /// <summary>
        /// Saves a new DN value to the database
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        /// 2019-04-04 KJBO
        [OperationContract]
        DnCL saveDn(string ident, DnCL d);

        /// <summary>
        /// Saves a new PN to the database
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        /// 2019-04-04 KJBO
        [OperationContract]
        PnCL savePn(string ident, PnCL p);





        }


    // Use a data contract as illustrated in the sample below to add composite types to service operations.


    // Class ReparatorCL is only for reading and listing. 
    // No changes are allowed in this version of the API
    [DataContract]
    public class ReparatorCL
    {
        [DataMember]
        public string AnvID
        { get; set; }

        [DataMember]
        public string Reparator
        { get; set; }

        [DataMember]
        public string RepKatID
        { get; set; }

        [DataMember]
        public int gasketLevel // Access level for gasket calculation. 0 (or null) = no access, 5 = user, 10 = administrator
        { get; set; }



        [DataMember]
        public int ErrCode
        { get; set; }

        [DataMember]
        public string ErrMessage
        { get; set; }


    }

    // Class ServiceHuvudCL is only for reading and listing. 
    // No changes are allowed in this version of the API
    [DataContract]
    public class ServiceHuvudCL
    {
        [DataMember]
        public string VartOrdernr
        { get; set; }

        [DataMember]
        public string ErtOrdernr
        { get; set; }

        [DataMember]
        public string Kund
        { get; set; }

        [DataMember]
        public string KundNamn
        { get; set; }

        [DataMember]
        public DateTime OrderDatum
        { get; set; }

        [DataMember]
        public string OrderAdminID
        { get; set; }

        [DataMember]
        public string OrderAdminNamn
        { get; set; }

        [DataMember]
        public string OrderLabel
        { get; set; }


        [DataMember]
        public int ErrCode
        { get; set; }

        [DataMember]
        public string ErrMessage
        { get; set; }


    }

    /// 2016-06-29 KJBO Added Arbetsordernr according to meeting with Ventilteknik
    /// 2016-09-12 KJBO Added hosKund, paVerkstad and klar.
    [DataContract]
    public class ServiceRadListCL
    {
        [DataMember]
        public string VartOrdernr
        { get; set; }

        [DataMember]
        public int RadNr
        { get; set; }

        [DataMember]
        public string Anlaggningsnr
        { get; set; }

        [DataMember]
        public string IdNr
        { get; set; }

        [DataMember]
        public string Avdelning
        { get; set; }

        [DataMember]
        public string KundensPosNr
        { get; set; }

        [DataMember]
        public string VentilKategori
        { get; set; }

        [DataMember]
        public string Ventiltyp
        { get; set; }

        [DataMember]
        public string Fabrikat
        { get; set; }

        [DataMember]
        public string Dn
        { get; set; }

        [DataMember]
        public string Pn
        { get; set; }

        [DataMember]
        public string Arbetsordernr //50
        { get; set; }

        [DataMember]
        public int hosKund
        { get; set; }

        [DataMember]
        public int paVerkstad
        { get; set; }

        [DataMember]
        public int klar
        { get; set; }



        [DataMember]
        public int ErrCode
        { get; set; }

        [DataMember]
        public string ErrMessage
        { get; set; }


    }

    /// <summary>
    /// For updating row
    /// These are the properties that the
    /// reparator shall update.
    /// String length are defined after each
    /// If no lenght is written then there is a blob field storing
    /// the data
    /// property
    /// </summary>
    /// 2016-02-03 KJBO Pergas AB
    /// 2016-05-27 KJBO Added Arbetsordernr according to meeting with Ventilteknik
    /// 2016-09-12 KJBO Added hosKund, paVerkstad and klar.
    /// 2017-02-27 KJBO Addel max length to several string fields
    [DataContract]
    public class ServiceRadCL
    {
        [DataMember]
        public string VartOrdernr // 10
        { get; set; }

        [DataMember]
        public int Radnr // Om radnr sätts till 0 indikerar detta att det är en ny rad
        { get; set; }

        [DataMember]
        public string Kontroll // 2016-02-27 200 tkn
        { get; set; }

        [DataMember]
        public string Arbete // 2016-02-27 400 tkn
        { get; set; }

        [DataMember]
        public string Anmarkning // 2016-02-27 200 tkn
        { get; set; }

        [DataMember]
        public string Reservdelar // 2016-02-27 200 tkn
        { get; set; }


        [DataMember]
        public string Reparator //10 do not update
        { get; set; }

        [DataMember]
        public string Reparator2 //10 do not update
        { get; set; }

        [DataMember]
        public string Reparator3 // 10 do not update
        { get; set; }

        [DataMember]
        public string StalldonKontroll // 2016-02-27 200 tkn
        { get; set; }

        [DataMember]
        public string StalldonArbete // 2016-02-27 400 tkn
        { get; set; }

        [DataMember]
        public string StalldonDelar // 2016-02-27 200 tkn
        { get; set; }

        [DataMember]
        public string LagesstallKontroll // 2016-02-27 200 tkn
        { get; set; }

        [DataMember]
        public string LagesstallArbete // 2016-02-27 400 tkn
        { get; set; }

        [DataMember]
        public string LagesstallDelar // 2016-02-27 200 tkn
        { get; set; }

        [DataMember]
        public int AntalBoxpack
        { get; set; }

        [DataMember]
        public string Boxpackning //25
        { get; set; }

        [DataMember]
        public string BoxpackMaterial //40
        { get; set; }

        [DataMember]
        public int AntalBrostpack
        { get; set; }

        [DataMember]
        public string Brostpackning //25
        { get; set; }

        [DataMember]
        public string BrostpackMaterial //40
        { get; set; }

        [DataMember]
        public string OvrKomment // 2016-02-27 200 tkn
        { get; set; }

        [DataMember]
        public string VentilID //40 (GUID)
        { get; set; }

        [DataMember]
        public string AlternateKey //40 (GUID)
        { get; set; }

        [DataMember]
        public string Arbetsordernr //50
        { get; set; }

        [DataMember]
        public int hosKund // 1 eller 0
        { get; set; }

        [DataMember]
        public int paVerkstad // 1 eller 0
        { get; set; }

        [DataMember]
        public int klar // 1 eller 0
        { get; set; }

        [DataMember]
        public int Attention // (1 eller 0) Aktuell om en servicerad har notering i OvrKomment. Om denna sätts till 1 kommer denna servicerad att följas upp i efterhand
        { get; set; }

        [DataMember]
        public int valveOpen // Om ventil är öppen eller stängd före demontering. Kan ha värde 0 eller 1. Obligatoriskt val. Användaren måste välja före spara. Om användaren
                             // ej valt ska värdet vara -1 vilket indikerar null och kommer att generera ett felmeddelande.
        { get; set; }



        [DataMember]
        public int ErrCode
        { get; set; }

        [DataMember]
        public string ErrMessage
        { get; set; }

    }


    /// <summary>
    /// This class is for comboboxes
    /// where the user selects the suitable
    /// ventilkategori. 
    /// </summary>
    [DataContract]
    public class VentilKategoriCL
    {

        [DataMember]
        public int VentilkatID // PK
        { get; set; }

        [DataMember]
        public string Ventilkategori
        { get; set; }

        [DataMember]
        public int ErrCode
        { get; set; }

        [DataMember]
        public string ErrMessage
        { get; set; }


    }




    [DataContract]
    public class VentilCL
    {
        [DataMember]
        public string VentilID // 40
        { get; set; }

        [DataMember]
        public int VentilkatID // Foreign key to ventilkategoriCL
        { get; set; }

        [DataMember]
        public string Ventilkategori // Only for display purposes (value taken from ventilkategori table)
        { get; set; }


        [DataMember]
        public string KundID //20

        { get; set; }
        [DataMember]
        public string Position //50
        { get; set; }

        [DataMember]
        public string Fabrikat // 20
        { get; set; }

        [DataMember]
        public string Ventiltyp // 40
        { get; set; }

        [DataMember]
        public string IdNr // 20
        { get; set; }

        [DataMember]
        public string Pn //10
        { get; set; }

        [DataMember]
        public string Pn2 // 10
        { get; set; }

        [DataMember]
        public string Dn // 10
        { get; set; }

        [DataMember]
        public string Dn2 // 10
        { get; set; }

        [DataMember]
        public decimal Oppningstryck // 10.3
        { get; set; }

        [DataMember]
        public string Stalldonstyp // 20
        { get; set; }

        [DataMember]
        public string StalldonIDNr // 20
        { get; set; }

        [DataMember]
        public string StalldonFabrikat // 20
        { get; set; }

        [DataMember]
        public string StalldonArtnr // 20
        { get; set; }

        [DataMember]
        public string Lagesstallartyp // 20
        { get; set; }

        [DataMember]
        public string LagesstallIDNr // 20
        { get; set; }

        [DataMember]
        public string LagesstallFabrikat // 20
        { get; set; }

        [DataMember]
        public string Avdelning // 20 
        { get; set; }

        [DataMember]
        public string Anlaggningsnr // 20 
        { get; set; }

        [DataMember]
        public string Plan // 40 
        { get; set; }

        [DataMember]
        public string Rum // 20 
        { get; set; }

        [DataMember]
        public bool forceSave // Boolean value that indicates to ignore duplicate position for a customer
        { get; set; }         // If you try to save a ventil with the same position as another ventil
                              // for the same customer. Normally you set this value to false and then get
                              // a warning by ErrCode 101 and a warning text. If you still want to save the
                              // ventil with the same position then you just try again with forceSave to true.
                              // If a customer has changed a ventil at a certain position it's perfectly normal
                              // to have duplicate position. But if there is a typing error the appUser has to
                              // change position and try to save again with forceSave to false 2016-03-21 KJBO 

        [DataMember]
        public decimal insideDiameter // Inside and outside diameter is values used in online pressure measurement
        { get; set; }                 // This value is not mandatory. It is expressed in mm (or parts of)  

        [DataMember]
        public decimal outsideDiameter
        { get; set; }

        [DataMember]
        public int ErrCode
        { get; set; }

        [DataMember]
        public string ErrMessage
        { get; set; }
    }


    /// <summary>
    /// Class to feed comboboxes when selecting fabrikat
    /// on ventil, stalldon and lagesstallare.
    /// The single Fabrikat property acts as both
    /// ID and value and can be seen as a way to
    /// prevent the user from entering different values
    /// for the same fabrikat
    /// </summary>
    /// 2016-02-04 KJBO Pergas AB
    [DataContract]
    public class FabrikatCL
    {
        [DataMember]
        public string Fabrikat 
        { get; set; }

        [DataMember]
        public int ErrCode
        { get; set; }

        [DataMember]
        public string ErrMessage
        { get; set; }
    }

    /// Class to feed comboboxes when selecting dn/dn2
    /// on ventil.
    /// The single dn property acts as both
    /// ID and value and can be seen as a way to
    /// prevent the user from entering different values
    /// for the same dn.
    /// 2016-02-04 KJBO Pergas AB
    [DataContract]
    public class DnCL
    {
        [DataMember]
        public string Dn // 10
        { get; set; }

        [DataMember]
        public int ErrCode
        { get; set; }

        [DataMember]
        public string ErrMessage
        { get; set; }

    }

    /// Class to feed comboboxes when selecting pn/pn2
    /// on ventil.
    /// The single dn property acts as both
    /// ID and value and can be seen as a way to
    /// prevent the user from entering different values
    /// for the same pn.
    /// 2016-02-04 KJBO Pergas AB
    [DataContract]
    public class PnCL
    {
        [DataMember]
        public string Pn // 10
        { get; set; }

        [DataMember]
        public int ErrCode
        { get; set; }

        [DataMember]
        public string ErrMessage
        { get; set; }
    }


    /// <summary>
    /// Artikel is for display only
    /// When the reparator will add reservdelar
    /// to the serviceorder he/she will select
    /// from a list of artikel
    /// 2018-05-28 Added kategori
    /// </summary>
    [DataContract]
    public class ArtikelCL
    {
        [DataMember]
        public string Artnr  // 16
        { get; set; }

        [DataMember]
        public string Artnamn  // 40
        { get; set; }

        [DataMember]
        public string LevID  // 16
        { get; set; }

        [DataMember]
        public string LevNamn  // 50
        { get; set; }

        [DataMember]
        public string Anm1  // 60
        { get; set; }

        [DataMember]
        public string Anm2  // 60
        { get; set; }
        [DataMember]
        public int kategori  // Artikelkategor. 1 = lagerförda artiklar, Övriga nummer saknar betydelse för SM
        { get; set; }

        [DataMember]
        public int ErrCode
        { get; set; }

        [DataMember]
        public string ErrMessage
        { get; set; }
    }


    /// <summary>
    /// Class reservdelCL is connected to a ServiceOrder
    /// by VartOrdernr and radnr
    /// The primary key is VartOrdernr, Radnr, and ReservNr
    /// If a new ReservdelCL is sent to the API the ReservNr shall be 0
    /// and VartOdernr and Radnr must exist.
    /// The user can enter anything in artnr and artnamn, but if a artnr
    /// exists in the ArtikelCL class the Artnamn shall default to the
    /// Artnamn in artikel and also the levID from artikel. 
    /// The reservdelCL is not in any way (other then described above)
    /// related to Artikel.
    /// 2018-05-28 Added getFromCL value
    /// </summary>
    [DataContract]
    public class ReservdelCL
    {
        [DataMember]
        public string VartOrdernr // 10
        { get; set; }

        [DataMember]
        public int Radnr
        { get; set; }

        [DataMember]
        public int ReservNr
        { get; set; }

        [DataMember]
        public decimal Antal
        { get; set; }

        [DataMember]
        public string Artnr // 20
        { get; set; }

        [DataMember]
        public string ArtNamn // 40
        { get; set; }

        [DataMember]
        public bool Faktureras
        { get; set; }

        [DataMember]
        public string LevID // 16
        { get; set; }

        [DataMember]
        public string Enhet // 5 (usualy set to st but can also be m for Meter or whatever the user wants)
        { get; set; }
        [DataMember]
        public bool getFromCS // Boolean value telling if the current item shall be fetched from CompactStore
        { get; set; }


        [DataMember]
        public int ErrCode
        { get; set; }

        [DataMember]
        public string ErrMessage
        { get; set; }
    }




    /// <summary>
    /// List of registered hours for a whole order (vartOrdernr)
    /// This class is used for returning all time registrations for the order
    /// Note the new functionality for timeTypeID which categorizes all the time registrations
    /// All of the time earlier time registrations is coded as "kund"
    /// Excuse for the swenglish. Most of the new functionality in the system is
    /// coded in english and the old parts are in Swedish.
    /// </summary>
    /// SalartName added 2017-11-13 KJBO
    [DataContract]
    public class RepTidListCL
    {

        [DataMember]
        public string vartOrdernr // Primary key to the whole order
        { get; set; }

        [DataMember]
        public int radnr // Row number in table servicerad
        { get; set; }

        [DataMember]
        public string srAltKey // Alternate key to servicerad (primary key is vartOrdernr and radnr in combination)
        { get; set; }


        [DataMember]
        public int ServRadRepTidId // Should normally not be needed in this context. Primary key for ServRadRepTid table
        { get; set; }

        [DataMember]
        public int timeTypeID // Primary key for TimeType table normally not needed
        { get; set; }

        [DataMember]
        public string timeType // Could be "kund", "verkstad" or "maskintid"
        { get; set; }

        [DataMember]
        public string anvID // Key to user and in this context key to reparator (user and reparator is the same table)
        { get; set; }

        [DataMember]
        public string reparator // Name of the reparator corresponding to anvID
        { get; set; }

        [DataMember]
        public string rowDescription // position, ventiltyp, fabrikat, arbetsordernr in combination 
        { get; set; }

        [DataMember]
        public DateTime datum // Date of the time registration
        { get; set; }

        [DataMember]
        public Decimal tid // Registered hours
        { get; set; }

        [DataMember]
        public string position // Position, Also included in rowDescription
        { get; set; }

        [DataMember]
        public string salartName // Name of salart (Added 2017-11-13 KJBO)
        { get; set; }

        [DataMember]
        public int ErrCode
        { get; set; }

        [DataMember]
        public string ErrMessage
        { get; set; }

    }







    /// <summary>
    /// Datacontract for RepTid version 2
    /// Added SalartId and RepKatID which is required for version 2
    /// 2017-03-13
    /// </summary>
    [DataContract]
    public class ServRadRepTidCL
    {
        [DataMember]
        public int ID
        { get; set; }

        [DataMember]
        public string SrAltKey // 40 (Alternativ unik nyckel till servicerad)
        { get; set; }

        [DataMember]
        public string AnvID // 10
        { get; set; }

        [DataMember]
        public DateTime Datum // 
        { get; set; }

        [DataMember]
        public Decimal Tid  // Arbetad tid i timmar 
        { get; set; }

        [DataMember]
        public int timeTypeID  // The new timeTypeID
        { get; set; }

        [DataMember]
        public int SalartID
        { get; set; }

        [DataMember]
        public string RepKatID
        { get; set; }


        [DataMember]
        public int ErrCode
        { get; set; }

        [DataMember]
        public string ErrMessage
        { get; set; }

    }

    /// <summary>
    /// Class for returning available
    /// time registration dates for a
    /// ServiceOrderRow
    /// </summary>
    [DataContract]
    public class OpenDateCL
    {
        [DataMember]
        public DateTime Datum
        { get; set; }

        [DataMember]
        public int ErrCode
        { get; set; }

        [DataMember]
        public string ErrMessage
        { get; set; }

    }

    /// <summary>
    /// Class for returning available
    /// time registration dates for a
    /// ServiceOrderRow
    /// </summary>
    [DataContract]
    public class PictureCL
    {
        [DataMember]
        public string VartOrdernr // 10
        { get; set; }

        [DataMember]
        public int Radnr // Int
        { get; set; }

        [DataMember]
        public int BildNr // int 
        { get; set; }

        [DataMember]
        public string PictIdent // 60
        { get; set; }

        [DataMember]
        public string Description // 100
        { get; set; }

        [DataMember]
        public long pictSize // long
        { get; set; }

        [DataMember]
        public string pictType // 10
        { get; set; }


        [DataMember]
        public int PictCatID // int 
        { get; set; }


        [DataMember]
        public int ErrCode
        { get; set; }

        [DataMember]
        public string ErrMessage
        { get; set; }

    }

    [DataContract]
    public class PictCatCL
    {
        [DataMember]
        public int PictCatID // PK
        { get; set; }

        [DataMember]
        public string PictCatName // 40
        { get; set; }

        [DataMember]
        public int Step // 40
        { get; set; }

        [DataMember]
        public int ErrCode
        { get; set; }

        [DataMember]
        public string ErrMessage
        { get; set; }
    }


    [DataContract]
    public class StandardTextCL
    {
        [DataMember]
        public string StdTextID // 10
        { get; set; }

        [DataMember]
        public string Text // This is the text that shall be duplicated in the textbox
        { get; set; }

        [DataMember]
        public int Kategori // This value is used to filter out only the currently availabe texts
        { get; set; }       // for each stage in the entering of values. The easiest way to see this
                            // is to try the Sman10 application and see what happens. For the moment
                            // only kategories 2, 3 and 5 are used
        [DataMember]
        public string KategoriBeskr // Description of the above categories
        { get; set; }

        [DataMember]
        public int ventilkatID // For filtering only the current ventilkategori. 
        { get; set; }          // 0 represents all ventilkategories. Normaly you show all
                               // of category 0 and also the rows matching the current category of the ventil 
        [DataMember]
        public int ErrCode
        { get; set; }

        [DataMember]
        public string ErrMessage
        { get; set; }
    }


    [DataContract]
    public class VersionCL
    {
        [DataMember]
        public int dbVersion // 10
        { get; set; }

        [DataMember]
        public string APIVersion // 10
        { get; set; }

        [DataMember]
        public int ErrCode
        { get; set; }

        [DataMember]
        public string ErrMessage
        { get; set; }



    }

    /// <summary>
    /// Class for time types..
    /// Consists if ID and name for TimeTypes
    /// </summary>
    [DataContract]
    public class TimeTypeCL
    {
        [DataMember]
        public int TimeTypeID
        { get; set; }

        [DataMember]
        public string TimeType
        { get; set; }

        [DataMember]
        public int ErrCode
        { get; set; }

        [DataMember]
        public string ErrMessage
        { get; set; }
    }


    /// <summary>
    /// Salart has to be provided
    /// when timeRegVersion is 2 (the newer version)
    /// This datacontract is used both when time is registered
    /// for each servicerow and for the whole order
    /// SalartTypeID determines where and how to
    /// use the salart. 
    /// SalartTypeID 1 : Valid when entering time at servicerow level
    ///                  Requires both reparator and RepKatID
    ///                  (Examples of this category is normaltid, övertid vardag
    ///                  
    /// SalartTypeID 2 : Valid when entering time at serviceorder level
    ///                  Requires both reparator and RepKatID
    ///                  (Examples of this category is sovtid, veckovila, traktamente)
    ///                  
    /// SalartTypeID 3 : Valid when entering time at serviceorder level
    ///                  when the registered value is not connected to individual reparator
    ///                  (Examples of this category is milersättning, tryckmätning maskinhyra
    ///                  
    /// 2017-03-13 KJBO
    /// </summary>
    [DataContract]
    public class SalartCL
    {
        [DataMember]
        public int SalartID // Primary key
        { get; set; }

        [DataMember]
        public int SalartTypeID // See above
        { get; set; }


        [DataMember]
        public string SalartName // Name of the salart
        { get; set; }

        [DataMember]
        public string Unit // Measuring unit (timmar, st, dagar)
        { get; set; }


        [DataMember]
        public int ErrCode
        { get; set; }

        [DataMember]
        public string ErrMessage
        { get; set; }
    }


    /// <summary>
    /// Reparatörskategori
    /// Used for selection of reparatörskategori
    /// in combobox or similar
    /// 2017-03-14 KJBO
    /// </summary>

    [DataContract]
    public class RepKatCL
    {
        [DataMember]
        public string RepKatID // Primary key max 10 tkn
        { get; set; }

        [DataMember]
        public string RepKat // Max 40 tkn
        { get; set; }

        [DataMember]
        public int ErrCode
        { get; set; }

        [DataMember]
        public string ErrMessage
        { get; set; }
    }



    /// <summary>
    /// Add time to Serviceorder
    /// Used in TimeReport version 2
    /// 2017-03-14 KJBO
    /// </summary>
    [DataContract]
    public class ServHuvRepTidCL
    {

        [DataMember]
        public int ID // Primary key
        { get; set; }

        [DataMember]
        public string VartOrdernr // Ordernumber max 10 char
        { get; set; }

        [DataMember]
        public int TimeTypeID // kund, verkstad eller maskintid (foreign key  to TimeType)
        { get; set; }

        [DataMember]
        public int SalartID // Löneart/tidart foreign key to salart (=löneart)
        { get; set; }

        [DataMember]
        public string AnvId // Reparatör ID foreign key to reparator (max 10 tkn)
        { get; set; }

        [DataMember]
        public string RepKatID // Reparatörskategori foreign key to repKat
        { get; set; }

        [DataMember]
        public decimal Tid // Time or any other unit (can be st or day )
        { get; set; }

        [DataMember]
        public DateTime Datum // Day
        { get; set; }


        [DataMember]
        public int ErrCode
        { get; set; }

        [DataMember]
        public string ErrMessage
        { get; set; }
    }


    [DataContract]
    public class TimeRep2ProcessCL
    {

        [DataMember]
        public string VartOrdernr // Ordernumber max 10 char Primary key
        { get; set; }

        [DataMember]
        public string Email // Email address to receive notification about
        { get; set; }       // report created 


        [DataMember]
        public int ReportType // Not used yet. Today always 2
        { get; set; }       // In the future the customer can decide which level of details in the report (1-3)
                            // 2017-09-20 KJBO
                            // ReportType 4 = standard report, 5 = detailed report
                            // From now on don't use 2 anymore

        [DataMember]
        public DateTime Ordered // Datetime when a report order has been created
        { get; set; }

        [DataMember]
        public int ReportStatus // Interesting information
        { get; set; }       // 0 = initial, 1 = in process, 2 = report created on disc, 3 = report sent via dropbox

        [DataMember]
        public string LinkURL // Link to the URL in DropBox (available when reportStatus = 3)
        { get; set; }

        [DataMember]
        public DateTime LinkAdded // Date and time when report was copied to DropBox and link was created
        { get; set; }

        [DataMember]
        public DateTime EmailCreated // Date and time when email was created and sent to Email address above
        { get; set; }


        [DataMember]
        public int ErrCode
        { get; set; }

        [DataMember]
        public string ErrMessage
        { get; set; }

    }


    [DataContract]
    public class TimeRep2WeekCL
    {

        [DataMember]
        public int ID // Unique identity
        { get; set; }

        [DataMember]
        public string VartOrdernr // Current ordernumber (probably not needed in the communication
        { get; set; }           // between App and API)


        [DataMember]
        public string YearWeek // yyyy-ww
        { get; set; }


        [DataMember]
        public bool Approved // If approved is true than the current week is closed and should not
        { get; set; }       // be selected in the App but displayed as readonly


        [DataMember]
        public int ErrCode
        { get; set; }

        [DataMember]
        public string ErrMessage
        { get; set; }

    }

    [DataContract]
    public class KundEmailCL
    {

        [DataMember]
        public int ID // Unique identity
        { get; set; }

        [DataMember]
        public string Kontaktperson // Name of the contact person
        { get; set; }


        [DataMember]
        public string Email // Email address
        { get; set; }


        [DataMember]
        public bool SelectedForTR // Toggle this setting for selected or not
        { get; set; }


        [DataMember]
        public int ErrCode
        { get; set; }

        [DataMember]
        public string ErrMessage
        { get; set; }

    }


    [DataContract]
    public class DrawingCL
    {
        [DataMember]
        public string ventil_id
        { get; set; }

        [DataMember]
        public int DrawingNo // int 
        { get; set; }

        [DataMember]
        public string DrawingIdent // 60
        { get; set; }

        [DataMember]
        public string Description // 100
        { get; set; }

        [DataMember]
        public long DrawingSize // long
        { get; set; }

        [DataMember]
        public string FileType // 10
        { get; set; }

        [DataMember]
        public int ErrCode
        { get; set; }

        [DataMember]
        public string ErrMessage
        { get; set; }

    }



}

