﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

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
        ServiceRadCL saveServRad(string ident, ServiceRadCL sr );

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
        //  2016-02-14
        [OperationContract]
        List<OpenDateCL> getOpenDates(string ident, string SrAltKey);

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
        //  2016-02-18 KJBO Pergas AB
        [OperationContract]
        List<ReparatorCL> getReparatorsForServiceHuvud(string ident, string vartOrdernr);




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
        public int ErrCode
        { get; set; }

        [DataMember]
        public string ErrMessage
        { get; set; }


    }


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
        public string Kontroll
        { get; set; }

        [DataMember]
        public string Arbete
        { get; set; }

        [DataMember]
        public string Anmarkning
        { get; set; }

        [DataMember]
        public string Reservdelar
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
        public string StalldonKontroll
        { get; set; }

        [DataMember]
        public string StalldonArbete
        { get; set; }

        [DataMember]
        public string StalldonDelar
        { get; set; }

        [DataMember]
        public string LagesstallKontroll
        { get; set; }

        [DataMember]
        public string LagesstallArbete
        { get; set; }

        [DataMember]
        public string LagesstallDelar
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
        public string OvrKomment
        { get; set; }

        [DataMember]
        public string VentilID //40 (GUID)
        { get; set; }

        [DataMember]
        public string AlternateKey //40 (GUID)
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
        public string Fabrikat // 20
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
    /// 
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
            public int ErrCode
            { get; set; }

            [DataMember]
            public string ErrMessage
            { get; set; }
        }


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


}