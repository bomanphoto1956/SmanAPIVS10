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
    
    


    }
}
