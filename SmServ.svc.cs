using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.IO;

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

            return cr.saveServRad(sr,ident);
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
        public ReservdelCL saveReservdel( string ident, ReservdelCL reservdel)
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
        public PictureCL getPicture( string ident, string vartOrdernr, int radnr, int bildNr)
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
        public Stream downLoadPict( string pictIdent )
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







    }
}
