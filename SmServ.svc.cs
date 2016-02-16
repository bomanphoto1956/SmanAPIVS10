using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

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

        public List<VentilCL> getVentilsForCust(string ident, string KundID)
        {
            CVentil cv = new CVentil();
            return cv.getVentilsForCust(ident, KundID);
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
        /// Returns all registered time (all rows)
        /// for a specific service row (identified by srAltKey)
        /// and a specific user (identifiec by ident)
        /// </summary>
        /// <param name="ident">Identity</param>
        /// <param name="srAltKey">AlternateKey for servicerad</param>
        /// <returns>List of registered time or one row with error message</returns>
        // 2016-02-15 Pergas AB KJBO
        public List<ServRadRepTidCL> getServRadRepTidForServiceRad(string ident, string srAltKey)
        {
            CTidRed ct = new CTidRed();

            return ct.getServRadRepTidForServiceRad(ident, srAltKey);
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

    
    


    }
}
