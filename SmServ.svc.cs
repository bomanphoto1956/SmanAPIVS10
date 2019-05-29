using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.IO;
using SManApi.Drawing;

namespace SManApi
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "SmServ" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select SmServ.svc or SmServ.svc.cs at the Solution Explorer and start debugging.
    public class SmServ : ISmServ
    {


        public string login(string AnvID, string pwd)
        {
            CReparator cr = new CReparator();
            return cr.login(AnvID, pwd);
        }

        public ReparatorCL getReparator(string ident)
        {
            CReparator cr = new CReparator();

            return cr.getReparator(ident);

        }

        public List<ReparatorCL> getReparators(string ident)
        {
            CReparator cr = new CReparator();

            return cr.getReparators(ident);

        }


        public List<ServiceHuvudCL> getServHuvForUser(string ident)
        {
            CServiceHuvud cs = new CServiceHuvud();

            return cs.getServHuvForUser(ident);
        }

        public ServiceHuvudCL getServHuv(string ident, string vartOrdernr)
        {
            CServiceHuvud cs = new CServiceHuvud();

            return cs.getServHuv(ident, vartOrdernr);

        }

        public List<ServiceRadListCL> getAllServRad(string ident, string vartOrdernr)
        {
            CServRad cr = new CServRad();
            return cr.getAllServRad(ident, vartOrdernr);
        }

        public ServiceRadCL getServRad(string ident, string vartOrdernr, int radnr)
        {
            CServRad cr = new CServRad();
            return cr.getServRad(ident, vartOrdernr, radnr);
        }

        public ServiceRadCL saveServRad(string ident, ServiceRadCL sr)
        {
            CServRad cr = new CServRad();

            return cr.saveServRad(sr, ident);
        }


        public VentilCL getVentil(string ident, string ventilID)
        {
            CVentil cv = new CVentil();
            return cv.getVentil(ident, ventilID);
        }

        public List<VentilCL> getVentilsForCust(string ident, string KundID, string position, string IDnr, string ventiltyp, string fabrikat, string anlaggningsnr)
        {
            CVentil cv = new CVentil();
            return cv.getVentilsForCust(ident, KundID, position, IDnr, ventiltyp, fabrikat, anlaggningsnr);
        }

        public VentilCL saveVentil(string ident, VentilCL v)
        {
            CVentil cv = new CVentil();

            return cv.saveVentil(ident, v);
        }


        public List<VentilKategoriCL> getVentilKategorier(string ident)
        {
            CVentil cv = new CVentil();

            return cv.getVentilKategoriers(ident);
        }

        public List<FabrikatCL> getFabrikat(string ident)
        {
            CComboValues cc = new CComboValues();
            return cc.getFabrikat(ident);

        }

        public List<DnCL> getDn(string ident)
        {
            CComboValues cc = new CComboValues();
            return cc.getDn(ident);
        }

        public List<PnCL> getPn(string ident)
        {
            CComboValues cc = new CComboValues();
            return cc.getPn(ident);

        }


        /// <summary>
        /// Get a list of artikel for displaying purposes
        /// </summary>
        /// <param name="ident">Identity</param>
        /// <param name="ArtnrFilter">Artnr or part of..</param>
        /// <param name="ArtnamnFilter">ArtNamn or part of..</param>
        /// <returns></returns>
        // 2016-02-09 KJBO  
        public List<ArtikelCL> getArtList(string ident, string ArtnrFilter, string ArtnamnFilter)
        {
            CReservdel cr = new CReservdel();

            return cr.getArtList(ident, ArtnrFilter, ArtnamnFilter);
        }


        /// <summary>
        /// Return one artikel
        /// </summary>
        /// <param name="ident">Ident</param>
        /// <param name="Artnr">Artnr</param>
        /// <returns></returns>
        // 2016-02-10 KJBO
        public ArtikelCL getArtikel(string ident, string Artnr)
        {
            CReservdel cr = new CReservdel();

            return cr.getArtikel(ident, Artnr);
        }

        /// <summary>
        /// Return a list of reservdel for one servicerad
        /// </summary>
        /// <param name="ident">Ident</param>
        /// <param name="VartOrdernr">VartOrdernr</param>
        /// <param name="RadNr">Radnr</param>
        /// <returns>List of reservdel or one row with error</returns>
        // 2016-02-10 KJBO Pergas AB
        public List<ReservdelCL> getReservdelsForServiceRad(string ident, string VartOrdernr, int RadNr)
        {
            CReservdel cr = new CReservdel();

            return cr.getReservdelsForServiceRad(ident, VartOrdernr, RadNr);
        }

        /// <summary>
        /// Get one reservdel identified by primary key
        /// </summary>
        /// <param name="ident">identity</param>
        /// <param name="VartOrdernr"></param>
        /// <param name="RadNr"></param>
        /// <param name="ReservNr"></param>
        /// <returns>The reservdel or an error</returns>
        //  2016-02-10 KJBO Pergas AB
        public ReservdelCL getReservdel(string ident, string VartOrdernr, int RadNr, int ReservNr)
        {
            CReservdel cr = new CReservdel();

            return cr.getReservdel(ident, VartOrdernr, RadNr, ReservNr);
        }


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
        public ReservdelCL saveReservdel(string ident, ReservdelCL reservdel)
        {
            CReservdel cr = new CReservdel();
            return cr.saveReservdel(ident, reservdel);
        }

        /// <summary>
        /// Returns a list of valid dates for
        /// registry of time for one ServiceRow
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="SrAltKey">Alternate key</param>
        /// <returns>List of dates or an error message</returns>
        public List<OpenDateCL> getOpenDates(string ident, string SrAltKey)
        {
            CTidRed ct = new CTidRed();
            return ct.getOpenDates(ident, SrAltKey);
        }


        /// <summary>
        /// Returns a list of valid dates for
        /// registry of time for one ServiceOrder
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="vartOrdernr"></param>
        /// <returns></returns>
        /// 2017-03-15 KJBO
        public List<OpenDateCL> getOpenDatesSH(string ident, string vartOrdernr)
        {
            CTidRed ct = new CTidRed();
            return ct.getOpenDatesSH(ident, vartOrdernr);
        }




        /// <summary>
        /// Get a specific TidRed record identified by ID (PK)
        /// </summary>
        /// <param name="ident">Identity</param>
        /// <param name="ID">ID of the ServRadRepTid</param>
        /// <returns>One instance of the ServRadRepTidCL class or one row with an error</returns>
        // 2016-02-15 KJBO Pergas AB
        public ServRadRepTidCL getServRadRepTid(string ident, int ID)
        {
            CTidRed ct = new CTidRed();
            return ct.getServRadRepTid(ident, ID);
        }

        /// <summary>
        /// Returns all registered time(all rows)
        /// for a specific service row (identified by srAltKey)
        /// and for a specific user (identified by AnvID)
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="AnvID"></param>
        /// <param name="srAltKey"></param>
        /// <returns></returns>
        //  2016-02-15 Pergas AB KJBO
        public List<ServRadRepTidCL> getServRadRepTidForServiceRad(string ident, string AnvID, string srAltKey)
        {
            CTidRed ct = new CTidRed();

            return ct.getServRadRepTidForServiceRad(ident, AnvID, srAltKey);
        }

        /// <summary>
        /// Returns all registered time(all rows)
        /// for a specific service row (identified by srAltKey)
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="srAltKey"></param>
        /// <returns></returns>
        //  2016-11-18 Pergas AB KJBO
        public List<ServRadRepTidCL> getServRadRepTidForSR(string ident, string srAltKey)
        {
            CTidRed ct = new CTidRed();
            return ct.getServRadRepTidForSR(ident, srAltKey);
        }


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
        public ServRadRepTidCL saveServRadRepTid(string ident, ServRadRepTidCL srt)
        {
            CTidRed ct = new CTidRed();

            return ct.saveServRadRepTid(ident, srt);
        }


        /// <summary>
        /// Check if an order is open for editing
        /// The return value is a string, 1 = open, -1 = closed or an error message
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="VartOrdernr"></param>
        /// <returns>1 - Open -1 - Closed or an error message</returns>
        public string isOpen(string ident, string VartOrdernr)
        {
            CServiceHuvud cs = new CServiceHuvud();

            return cs.isOpen(ident, VartOrdernr);
        }

        /// <summary>
        /// Get a list of all reparators assigned to one 
        /// servicehuvud identified by vartOrdernr
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="vartOrdernr"></param>
        /// <returns>A list of reparators or error</returns>
        /// 2017-03-14 Added functionality
        /// RepKatID is now current for this ordernr        
        public List<ReparatorCL> getReparatorsForServiceHuvud(string ident, string vartOrdernr)
        {
            CReparator cr = new CReparator();
            return cr.getReparatorsForServiceHuvud(ident, vartOrdernr);
        }

        /// <summary>
        /// Returns the alternate key for a serviceRow
        /// </summary>
        /// <param name="vartOrdernr"></param>
        /// <param name="radnr"></param>
        /// <returns>If ident is invalid the function returns "-10"</returns>
        /// <returns>If a database error occurs then the return is "-1" followed by the database error description"</returns>        
        /// <returns>If no row is found by the provided primary key then the result is an empty string</returns>
        /// <returns>In the normal case (identity is OK and the primary key exists) the function return the alternate key</returns>
        //  2016-02-29 Pergas AB KJBO
        public string getAlternateKey(string ident, string vartOrdernr, int radnr)
        {
            CServRad cr = new CServRad();

            return cr.getAlternateKey(ident, vartOrdernr, radnr);
        }



        /// <summary>
        /// Stores a picture or an image to the
        /// local directory "UpLoads"
        /// The name of the picture is a GUID
        /// which is returned to the caller on success
        /// </summary>
        /// <param name="sPict">Picture as stream</param>
        /// <returns>The name of the picture (that has to be referred in
        /// future calls when this picture shall be stored with metadata
        /// If an error occurs then the return string is -1 followed by an
        /// error message</returns>
        //  2016-03-03 KJBO
        public string uploadPict(Stream sPict)
        {
            CPicture cp = new CPicture();

            return cp.uploadPict(sPict);
        }


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
        public PictureCL savePicture(string ident, PictureCL p)
        {
            CPicture cp = new CPicture();
            return cp.savePicture(ident, p);
        }




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
        public PictureCL getPicture(string ident, string vartOrdernr, int radnr, int bildNr)
        {
            CPicture cp = new CPicture();
            return cp.getPicture(ident, vartOrdernr, radnr, bildNr);
        }


        /// <summary>
        /// The downLoadPict method accept a pictIdent parameter as well
        /// as a reference to an error string
        /// The method returns a memorystream to the caller. If an
        /// error occurs then the stream is nukll and en error is 
        /// message is written to the error parameter
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
        public Stream downLoadPict(string pictIdent)
        {
            string error = "";
            CPicture cp = new CPicture();
            return cp.downLoadPict(pictIdent, ref error);
        }

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
        public PictureCL deletePicture(string ident, PictureCL p)
        {
            CPicture cp = new CPicture();
            return cp.deletePicture(ident, p);
        }

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
        public List<PictureCL> getPicturesForServiceRad(string ident, string vartOrdernr, int radnr)
        {
            CPicture cp = new CPicture();
            return cp.getPicturesForServiceRad(ident, vartOrdernr, radnr);
        }

        /// <summary>
        /// Returns all standardtext
        /// </summary>
        /// <param name="ident"></param>
        /// <returns></returns>
        public List<StandardTextCL> getAllSttText(string ident)
        {
            CStdText cs = new CStdText();
            return cs.getAllSttText(ident);
        }


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
        /// 
        public List<PictCatCL> getPictCategories(string ident, int Step)
        {
            CPicture cp = new CPicture();

            return cp.getPictCategories(ident, Step);
        }


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
        public PictureCL updatePictMetadata(string ident, PictureCL p)
        {
            CPicture cp = new CPicture();

            return cp.updatePictMetadata(ident, p);
        }


        /// <summary>
        /// Returns database and API versions
        /// </summary>
        /// <param name="ident"></param>
        /// <returns></returns>
        public VersionCL getVersion(string ident)
        {
            CMisc cm = new CMisc();
            return cm.getVersion(ident);
        }

        /// <summary>
        /// Sum all registered hours for one servicerad
        /// </summary>
        /// <param name="ident">ident</param>
        /// <param name="srAltKey">alternate key for servicerad</param>
        /// <param name="AnvID">UserID or empty string for all users</param>
        /// <returns>Number of hours or -1 if error occurs</returns>
        /// 2016-06-17 KJBO
        public Decimal SumHoursForServRad(string ident, string srAltKey, string AnvID)
        {
            CTidRed ct = new CTidRed();
            return ct.SumHoursForServRad(ident, srAltKey, AnvID);
        }

        /// <summary>
        /// Deletes a reservdel identified by primary key
        /// </summary>
        /// <param name="ident">identity string</param>
        /// <param name="reservdel">One valid reservdel</param>
        /// <returns>Empty string if OK otherwise error message</returns>
        /// 2016-09-30 KJBO
        public string deleteReservdel(string ident, ReservdelCL reservdel)
        {
            CReservdel cr = new CReservdel();
            return cr.deleteReservdel(ident, reservdel);
        }

        /// <summary>
        /// Returns all time registry for a given order
        /// </summary>
        /// <param name="ident">Identity</param>
        /// <param name="vartOrdernr">Order number</param>
        /// <returns>List of RepTidListCL</returns>
        public List<RepTidListCL> getAllTimeForOrder(string ident, string vartOrdernr)
        {
            CTidRed ct = new CTidRed();

            return ct.getAllTimeForOrder(ident, vartOrdernr);
        }

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
        public List<TimeTypeCL> getTimeTypes(string ident, bool hosKund, bool paVerkstad)
        {
            CTidRed ct = new CTidRed();

            return ct.getTimeTypes(ident, hosKund, paVerkstad);
        }

        /// <summary>
        /// Get the current timeRegVersion
        /// can be either 1 or 2
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="vartOrdernr"></param>
        /// <returns>Version or -1 for invalid ident or -2 for database error 
        /// (no more error description is available for this function</returns>
        public int getTimeRegVersion(string ident, string vartOrdernr)
        {
            CMisc cm = new CMisc();
            return cm.getTimeRegVersion(ident, vartOrdernr);
        }


        /// <summary>
        /// Return all salarts either for servicedetalj or serviceorder 
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="forServiceDetalj"></param>
        /// <returns>List of salart or error code</returns>
        public List<SalartCL> getSalart(string ident, bool forServiceDetalj)
        {
            CComboValues cv = new CComboValues();

            return cv.getSalart(ident, forServiceDetalj);
        }


        /// <summary>
        /// Get valid time types for one order
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="vartOrdernr"></param>
        /// <returns>Valid time types</returns>
        // 2017-03-14 KJBO
        public List<TimeTypeCL> getTimeTypesForOrder(string ident, string vartOrdernr)
        {
            CTidRed cr = new CTidRed();

            return cr.getTimeTypesForOrder(ident, vartOrdernr);
        }


        /// <summary>
        /// Get all available repKat for 
        /// timeregistration version 2
        /// </summary>
        /// <param name="ident"></param>
        /// <returns></returns>
        /// 2017-03-14 KJBO
        public List<RepKatCL> getRepKat(string ident)
        {
            CReparator cr = new CReparator();
            return cr.getRepKat(ident);
        }

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
        public ServHuvRepTidCL saveServHuvRepTid(string ident, ServHuvRepTidCL sht)
        {
            CTidRed ct = new CTidRed();
            return ct.saveServHuvRepTid(ident, sht);
        }

        /// <summary>
        /// Get one row of ServHuvRepTid identified by PK
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        /// 2017-03-15 KJBO
        public ServHuvRepTidCL getServHuvRepTid(string ident, int ID)
        {
            CTidRed ct = new CTidRed();
            return ct.getServHuvRepTid(ident, ID);
        }

        /// <summary>
        /// Get all ServHuvRepTid for one order
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="vartOrdernr"></param>
        /// <returns></returns>
        /// 2017-03-15 KJBO
        public List<ServHuvRepTidCL> getServHuvRepTidForSH(string ident, string vartOrdernr)
        {
            CTidRed ct = new CTidRed();
            return ct.getServHuvRepTidForSH(ident, vartOrdernr);
        }

        /// <summary>
        /// Get all ServHuvRepTid for one order and 
        /// one user ( = anv)
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="vartOrdernr"></param>
        /// <param name="anvID"></param>
        /// <returns></returns>
        /// 2017-03-15 KJBO
        public List<ServHuvRepTidCL> getServHuvRepTidForShAnv(string ident, string vartOrdernr, string anvID)
        {
            CTidRed ct = new CTidRed();
            return ct.getServHuvRepTidForShAnv(ident, vartOrdernr, anvID);
        }

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
        public RepKatCL getDefaultRepKat(string ident, string AnvID, string VartOrdernr)
        {
            CReparator cr = new CReparator();

            return cr.getDefaultRepKat(ident, AnvID, VartOrdernr);
        }

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
        public TimeRep2ProcessCL generateTimeReg2Report(string ident, TimeRep2ProcessCL p, bool bOverrideExisting, bool approve, List<int> timeRep2WeekIds, List<KundEmailCL> kundEmails)
        {
            CTidRed ct = new CTidRed();
            return ct.generateTimeReg2Report(ident, p, bOverrideExisting, approve, timeRep2WeekIds, kundEmails);
        }

        /// <summary>
        /// After calling generateTimeReg2Report() there is a possibility
        /// to check the status of the report generation process.
        /// Call this function and you get a status report back
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="VartOrdernr"></param>
        /// <returns>Instance of TimeRep2ProcessCL</returns>
        /// 2017-03-21 KJBO
        public TimeRep2ProcessCL getTimeRep2ReportStatus(string ident, string VartOrdernr)
        {
            CTidRed ct = new CTidRed();
            return ct.getTimeRep2ReportStatus(ident, VartOrdernr);
        }


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
        // 2017-10-23 KJBO
        public List<TimeRep2WeekCL> getTimeRep2Weeks(string ident, string VartOrdernr)
        {
            CTidRed ct = new CTidRed();
            return ct.getTimeRep2Weeks(ident, VartOrdernr);
        }


        /// <summary>
        /// This function shall be called when a contact list for sending timereports is requierd
        /// It gives a list of earlier contacts and also if the current contact
        /// (email address) was selected for having a timeReport in history
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="VartOrdernr"></param>
        /// <returns>List of KundEmailCL or error</returns>
        /// 2017-10-24 KJBO
        public List<KundEmailCL> getTimeRecordContactList(string ident, string VartOrdernr)
        {
            CTidRed ct = new CTidRed();
            return ct.getTimeRecordContactList(ident, VartOrdernr);
        }

        /// <summary>
        /// Get all reparators 
        /// or just the reparator with AnvID
        /// </summary>
        /// <returns>List of reparators</returns>
        /// 2018-08-21 KJBO
        public List<ReparatorCL> gGetReparators(string ident, string AnvID)
        {
            CReparator rep = new CReparator();
            return rep.getReparators(ident, AnvID);
        }

        /// <summary>
        /// Returns the selectable gasket levels
        /// </summary>
        /// <param name="ident"></param>
        /// <returns></returns>
        /// 2018-08-21 KJBO
        public List<KeyValuePair<int, string>> gGetGasketLevels(string ident)
        {
            CReparator rep = new CReparator();
            return rep.gGetGasketLevels(ident);
        }

        /// <summary>
        /// Save access level for gasket handling
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="reparator"></param>
        /// <returns></returns>
        /// 2018-08-21 KJBO
        public ReparatorCL saveGasketLevel(string ident, ReparatorCL reparator)
        {
            CReparator rep = new CReparator();
            return rep.saveGasketLevel(ident, reparator);
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
            Basdata.CFabrikat fabr = new Basdata.CFabrikat();
            return fabr.saveFabrikat(ident, f);
        }


        /// <summary>
        /// Saves a new DN value to the database
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public DnCL saveDn(string ident, DnCL d)
        {
            Basdata.CDn dn = new Basdata.CDn();
            return dn.saveDn(ident, d);
        }

        /// <summary>
        /// Saves a new PN to the database
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        /// 2019-04-04 KJBO
        public PnCL savePn(string ident, PnCL p)
        {
            Basdata.CPn pn = new Basdata.CPn();
            return pn.savePn(ident, p);
        }

        /// <summary>
        /// Upload a drawing from client to server
        /// </summary>
        /// <param name="sPict"></param>
        /// <returns></returns>
        /// 2019-05-06 KJBO
        public string uploadDrawing(Stream sPict)
        {
            CDrawing cd = new CDrawing();
            return cd.uploadDrawing(sPict);
        }

        /// <summary>
        /// Save a previous uploaded drawing to database
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public DrawingCL saveDrawing(string ident, DrawingCL d)
        {
            CDrawing cd = new CDrawing();
            return cd.saveDrawing(ident, d);
        }

        /// <summary>
        /// Get a drawing from the database identified by
        /// primary key (ventil_id, drawingNo)
        /// Returns a DrawingCL object with the drawingIdent
        /// field with a file name to the file being extracted
        /// by the server.
        /// If the fileName is empty or begins with -1 then
        /// there is an error while extracting the picture from
        /// the database to the temporary storage
        /// 
        /// After this function is called there has to be a call
        /// to downloadDrawing with the drawingIdent as parameter
        /// This function returns the drawing to the caller as
        /// a memoryStream
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="ventilId"></param>
        /// <param name="ritningNo"></param>
        /// <returns></returns>
        /// 2019-05-08 KJBO
        public DrawingCL getDrawing(string ident, string ventilId, int ritningNo)
        {
            CDrawing cd = new CDrawing();
            return cd.getDrawing(ident, ventilId, ritningNo);
        }


        /// The downLoadDrawing method accept a drawingIdent parameter as well
        /// as a reference to an error string
        /// The method calls downLoadPict on CPicture class and return the stream
        /// If and error occurs then the stream is null and an error
        /// message is writtent to the error parameter
        /// 
        /// This method shall be called after a call to getDrawing. When getDrawing
        /// is called it will store a copy of the picture on the server and also return
        /// a drawingCL object with the drawingIdent. This identity is used when this 
        /// method is called.
        public Stream downLoadDrawing(string drawingIdent)
        {
            string error = "";
            CDrawing cd = new CDrawing();
            return cd.downLoadDrawing(drawingIdent, ref error);            
        }

        /// <summary>
        /// Deletes a drawing from the database. The drawing is
        /// identified by ventil_id and drawingNo (PK).
        /// Return value is a DrawingCl with errCode = 0
        /// and errMessage as an empty string. On error
        /// the errCode is not 0 and the errMessage tells 
        /// what was going wrong
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        /// 2019-05-15 KJBO
        public DrawingCL deleteDrawing(string ident, DrawingCL d)
        {
            CDrawing cd = new CDrawing();
            return cd.deleteDrawing(ident, d);
        }


        /// <summary>
        /// This method returns all drawings for one ventil
        /// Note that you dont get the actual drawing nor the
        /// drawingIdent. Instead you use this method for getting a
        /// list of available drawings (and also gets the drawing
        /// description).
        /// After that you have to call GetDrawing and DownloadDrawing
        /// in turn in order to get each individual drawing.
        /// The reason for this is performance. This method gives
        /// a fast list of available drawings only.
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="vartOrdernr"></param>
        /// <param name="radnr"></param>
        /// <returns></returns>
        /// 2016-03-11 Pergas AB kjbo           
        public List<DrawingCL> getDrawingsForVentil(string ident, string ventilId)
        {
            CDrawing cd = new CDrawing();
            return cd.getDrawingsForVentil(ident, ventilId);
        }

        /// <summary>
        /// Updates the drawing metadata.
        /// Note that the drawing must exist, identified
        /// by the following properties in the drawing class:
        /// ventil_id, DrawingNo.
        /// For performance reason this method does not evaluate
        /// the drawing size.
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        /// 2019-05-27 KJBO
        public DrawingCL updateDrawingMetaData(string ident, DrawingCL d)
        {
            CDrawing cd = new CDrawing();
            return cd.updateDrawingMetaData(ident, d);
        }




        }
    }
