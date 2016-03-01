using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NexusDB.ADOProvider;
using System.Drawing;
using System.IO;
using System.Data;

namespace SManApi
{
    /// <summary>
    /// Class to handle pictures
    /// </summary>
    public class CPicture
    {
        CDB cdb = null;
        public CPicture()
        {
            cdb = new CDB();
        }


        /// <summary>
        /// Private function to validate entered data
        /// </summary>
        /// <param name="p"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        private int validatePicture(PictureCL p, ref string err)
        {
            CServRad cs = new CServRad();
            if (p.VartOrdernr == "")
                return -1;
            if (p.Radnr == 0)
                return -1;
            int antal = cs.validateServRad(p.VartOrdernr, p.Radnr);
            if (antal == 0)
                return -1;
            if (p.Bild.Length == 0)
                return -2;
            return 1;
        }


        /// <summary>
        /// Private function that returns the insert SQL clause
        /// </summary>
        /// <returns></returns>
        private string getInsertSQL()
        {
            string sSql = " insert into servrad_bild ( bild, bild_nr, radnr, vart_ordernr)  "
                         + "  values ( :bild, :bild_nr, :radnr, :vart_ordernr )";  

            return sSql;
        }


        /// <summary>
        /// Conversion function that takes a byte array
        /// and returns an image
        /// </summary>
        /// <param name="byteArrayIn">Picture as array of byte</param>
        /// <returns></returns>
        private Image byteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }



        /// <summary>
        /// Sets the parameters to the SQL clause
        /// </summary>
        /// <param name="np"></param>
        /// <param name="p"></param>
        private void setParameters(NxParameterCollection np, PictureCL p)
        {
             np.Add("bild",byteArrayToImage(p.Bild));
             np.Add("bild_nr", p.BildNr);                             
             np.Add("radnr", p.Radnr);               
             np.Add("vart_ordernr", p.VartOrdernr);
        }


        /// <summary>
        /// Calculates the next "free" picture number
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private int getNextBildNr(PictureCL p)
        {
            string sSql = " select coalesce(max(bild_nr),0) as bild_nr "
                        + " from servrad_bild "
                        + " where vart_ordernr = :vart_ordernr "
                        + " and radnr = :radnr ";

            NxParameterCollection pc = new NxParameterCollection();

            pc.Add("vart_ordernr", p.VartOrdernr);
            pc.Add("radnr", p.Radnr);

            string err = "";

            DataTable dt = cdb.getData(sSql, ref err, pc);           
 
            return Convert.ToInt32(dt.Rows[0]["bild_nr"]) + 1;
        }


        /// <summary>
        /// Saves a picture to the database
        /// </summary>
        /// <param name="ident">Identity</param>
        /// <param name="p">PictueCL class</param>
        /// <returns>The stored picture or an error message</returns>
        //  2016-02-29 Pergas AB KJBO
        public PictureCL savePicture(string ident, PictureCL p)
        {
            CReparator cr = new CReparator();
            int identOK = cr.checkIdent(ident);

            // Creates a class to return an error
            PictureCL pN = new PictureCL(); 
            if (identOK == -1)
            {
                pN.ErrCode = -10;
                pN.ErrMessage = "Ogiltigt login";
                return pN;
            }

            // Init variable
            string err = "";
            int valid = validatePicture(p, ref err);
            if (valid == -1)
            {
                pN.ErrCode = -1;
                pN.ErrMessage = "Felaktig servicerad";
                return pN;
            }
            if (valid == -2)
            {
                pN.ErrCode = -1;
                pN.ErrMessage = "Bild saknas";
                return pN;
            }

            CServiceHuvud ch = new CServiceHuvud();
            string sOpen = ch.isOpen(ident, p.VartOrdernr);
            if (sOpen != "1")
            {
                {
                    pN.ErrCode = -10;
                    if (sOpen == "-1")
                        pN.ErrMessage = "Order är stängd för inmatning";
                    else
                        pN.ErrMessage = sOpen;
                    return pN;
                }
            }

            string sSql = "";
            
            // This is a new bild
            p.BildNr = getNextBildNr(p);

            sSql = getInsertSQL();
                
            NxParameterCollection np = new NxParameterCollection();
            setParameters(np, p);
            
            string errText = "";

            int iRc = cdb.updateData(sSql, ref errText, np);

            if (errText != "")
            {
                if (errText.Length > 2000)
                    errText = errText.Substring(1, 2000);
                pN.ErrCode = -100;
                pN.ErrMessage = errText;
                return pN;
            }

            return p;
                      
        }



                    





          
           




        
    }
}