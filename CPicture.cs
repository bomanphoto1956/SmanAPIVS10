using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NexusDB.ADOProvider;
using System.Drawing;
using System.IO;
using System.Data;
using NexusDB.Tools;

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

        private string getUplDir()
        {
            return HttpContext.Current.Server.MapPath("~/Uploads/");
        }


        private string getUploadDirectory(string fileName)
        {
            return Path.Combine(getUplDir(), fileName);

        }


        /// <summary>
        /// Validates that a file with the name as the parameter pictIdent
        /// exists in the Uploads path
        /// </summary>
        /// <param name="pictIdent"></param>
        /// <returns></returns>
        private bool validatePictIdent(string pictIdent)
        {
            string fileAndPath = getUploadDirectory(pictIdent);
            if (File.Exists(fileAndPath))
                return true;
            return false;

        }


        private bool validatePictCategory(PictureCL p)
        {
            if (p.PictCatID == 0)
                return false;

            string sSql = " select count(*) antal "
                        + " from PictCategory "
                        + " where pictCatID = :pictCatID ";
            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("pictCatID", p.PictCatID);
            

            string err = "";

            DataTable dt = cdb.getData(sSql, ref err, pc);

            if (dt.Rows.Count == 0)
                return false;

            return (Convert.ToInt32(dt.Rows[0]["antal"]) == 1);               
        }


        /// <summary>
        /// Private function to validate entered data
        /// </summary>
        /// <param name="p"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        private int validatePicture(PictureCL p, bool forDelete, ref string err)
        {
            CServRad cs = new CServRad();
            if (p.VartOrdernr == "")
                return -1;
            if (p.Radnr == 0)
                return -2;
            if (forDelete)
            {
                if (p.BildNr == 0)
                    return -3;
            }
            if (forDelete || p.BildNr > 0 )
            {
                int antal = cs.validteServRadBild(p.VartOrdernr, p.Radnr, p.BildNr);
                if (antal == 0)
                    return -4;
            }
            else if (p.BildNr == 0)
            {
                int antal = cs.validateServRad(p.VartOrdernr, p.Radnr);
                if (antal == 0)
                    return -5;
            }
            if (!forDelete)
            {
                if (!validatePictIdent(p.PictIdent))
                    return -6;
            }
            if (!forDelete)
            {
                if (!validatePictCategory(p))
                    return -7;
            }
            return 1;
        }


        /// <summary>
        /// Private function that returns the insert SQL clause
        /// </summary>
        /// <returns></returns>
        private string getInsertSQL()
        {
            string sSql = " insert into servrad_bild ( bild, bild_nr, radnr, vart_ordernr,pictDescript, pictSize, pictType, pictCatID )  "
                         + "  values ( :bild, :bild_nr, :radnr, :vart_ordernr, :pictDescript, :pictSize, :pictType, :pictCatID )";  

            return sSql;
        }


        private string getUpdateSQL()
        {
            string sSql = " update servrad_bild "
                        + " set  pictType = :pictType "                                                 
                         + ", pictSize = :pictSize "                         
                         + ", bild = :bild "
                         + ", PictDescript = :PictDescript "
                         + ", pictCatID = :pictCatID "
                         + " where vart_ordernr = :vart_ordernr "
                         + " and radnr = :radnr "
                         + " and bild_nr = :bild_nr ";
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


        private byte[] getPhoto(string pictIdent, ref long fileSize)
        {
            string filePath = getUploadDirectory(pictIdent);
            FileInfo f = new FileInfo(filePath);
            fileSize = f.Length;
            FileStream stream = new FileStream(
                filePath, FileMode.Open, FileAccess.Read);
            BinaryReader reader = new BinaryReader(stream);

            byte[] photo = reader.ReadBytes((int)stream.Length);

            reader.Close();
            stream.Close();

            return photo;
        }

        /// <summary>
        /// Saves the pictory temporary
        /// (when the picture is retrieved from the
        /// database and converted to a memoryStream)
        /// The picture is saved temporary in order for
        /// the client to retrieve the picture.
        /// Finally the created file is deleted
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        private string savePictToToFile(MemoryStream m, ref string error, ref long fileSize)
        {
            string path = "";
            string fileName = "";
            try
            {
                // Get the file name from GUID
                fileName = Guid.NewGuid().ToString() + ".jpg";
                // Get the upload directory (both for upload and download)
                path = getUploadDirectory(fileName);

                using (FileStream file = new FileStream(path, FileMode.Create, System.IO.FileAccess.Write))
                {
                    byte[] bytes = new byte[m.Length];
                    m.Read(bytes, 0, (int)m.Length);
                    file.Write(bytes, 0, bytes.Length);
                    m.Close();
                }
                FileInfo f = new FileInfo(path);
                fileSize = f.Length;
            }
            catch (Exception ex)
            {
                error = "Error when temporary storing picture to a new file in the upload directory (" + path + "). Error message : " + ex.Message;
                fileName = "-1 " + ex.Message;
            }
            return fileName;
        }






        /// <summary>
        /// Sets the parameters to the SQL clause
        /// </summary>
        /// <param name="np"></param>
        /// <param name="p"></param>
        private void setParameters(NxParameterCollection np, PictureCL p, Boolean retrievePhoto)
        {
            long fileSize = 0;
            if (retrievePhoto)
                np.Add("bild", getPhoto(p.PictIdent, ref fileSize));             
             np.Add("bild_nr", p.BildNr);                             
             np.Add("radnr", p.Radnr);               
             np.Add("vart_ordernr", p.VartOrdernr);
             np.Add("pictDescript", p.Description);
             np.Add("pictSize", fileSize);
             np.Add("pictType", "jpg");
             p.pictSize = fileSize;
             p.pictType = "jpg";
             np.Add("pictCatID", p.PictCatID);
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



        private void deleteOldPics()
        {
            DateTime ldtOffset = System.DateTime.Now.AddDays(-1);
            string dir = getUplDir();

            string[] sFiles = Directory.GetFiles(dir);

            foreach (string filePath in sFiles)
            {
                DateTime modification = File.GetLastWriteTime(filePath);
                if (modification < ldtOffset)
                    File.Delete(filePath);
            }
        }


        private void deletePict(string pictIdent)
        {
            string path = getUploadDirectory(pictIdent);

            File.Delete(path);
            deleteOldPics();


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

            PictureCL p = new PictureCL();

            CReparator cr = new CReparator();

            int identOK = cr.checkIdent(ident);

            if (identOK == -1)
            {
                p.ErrCode = -10;
                p.ErrMessage = "Ogiltigt login";
                return p;
            }

            
            string sSql = " SELECT vart_ordernr, radnr, bild_nr, bild, pictDescript, pictSize, pictType, pictCatID "
                         + " FROM servrad_bild "
                         + " where vart_ordernr = :vart_ordernr "
                         + " and radnr = :radnr "
                         + " and bild_nr = :bild_nr ";

            NxParameterCollection np = new NxParameterCollection();
            np.Add("vart_ordernr", vartOrdernr);
            np.Add("radnr", radnr);
            np.Add("bild_nr", bildNr);            

            string errText = "";

            DataTable dt = cdb.getData(sSql, ref errText, np);

            int errCode = -100;

            if (errText == "" && dt.Rows.Count == 0)
            {
                errText = "Felaktig bildidentitet";
                errCode = 0;
            }


            if (errText != "")
            {

                if (errText.Length > 2000)
                    errText = errText.Substring(1, 2000);
                p.ErrCode = errCode;
                p.ErrMessage = errText;
                return p;
            }


            DataRow dr = dt.Rows[0];
            p.ErrCode = 0;
            p.ErrMessage = "";
            string error = "";
            long fileSize = 0;
            if (dr["bild"] != DBNull.Value)
            {
                byte[] data = (byte[])dr["bild"];
                MemoryStream ms = new MemoryStream(data);
                p.PictIdent = savePictToToFile(ms, ref error, ref fileSize);
            } 
            if (error != "")
            {                
                p.ErrCode = -1;
                p.ErrMessage = error;
                return p;
            }
            p.VartOrdernr = dr["vart_ordernr"].ToString();
            p.Radnr = Convert.ToInt32(dr["radnr"]);
            p.BildNr = Convert.ToInt32(dr["bild_nr"]);
            p.Description = dr["pictDescript"].ToString();
            p.pictSize = Convert.ToInt64(dr["pictSize"]);
            p.pictType = dr["pictType"].ToString();
            p.PictCatID = 0;
            if (dr["pictCatID"] != DBNull.Value)
                p.PictCatID = Convert.ToInt32(dr["pictCatID"]);
            return p;

        }















        /// <summary>
        /// Saves a picture to the database
        /// This method shall be called directory after
        /// a call to UploadPict
        /// The UploadPict gives you (upon success)
        /// an identity (=filename) to the upoaded file
        /// This identity is provided to this function
        /// in the PictureCL class
        /// If PictureCL.bildnr = 0 indicates new picture
        /// Otherwise providing picture number indicates update
        /// 
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        //  2016-03-07 KJBO
        public PictureCL savePicture(string ident, PictureCL p)
        {
            CReparator cr = new CReparator();
            int identOK = cr.checkIdent(ident);

            // Creates a class to return an error
            PictureCL pN = new PictureCL(); 
            if (identOK == -1)
            {
                deletePict(p.PictIdent);
                pN.ErrCode = -10;
                pN.ErrMessage = "Ogiltigt login";
                return pN;
            }

            // Init variable
            string err = "";
            int valid = validatePicture(p, false, ref err);
            if (valid == -1 || valid == -2 || valid == -5)
            {
                deletePict(p.PictIdent);
                pN.ErrCode = -1;
                pN.ErrMessage = "Kan ej hitta order";
                return pN;
            }
            if (valid == -4)
            {
                deletePict(p.PictIdent);
                pN.ErrCode = -1;
                pN.ErrMessage = "Bildnummer saknas för aktuell servicerad";
                return pN;
            }

            if (valid == -6)
            {                
                pN.ErrCode = -1;
                pN.ErrMessage = "Bild saknas i uppladdningbiblioteket";
                return pN;
            }

            
            if (valid == -7)
            {
                pN.ErrCode = -1;
                pN.ErrMessage = "Felaktig bildkategori (PictCatID) ";
                return pN;
            }

            CServiceHuvud ch = new CServiceHuvud();
            string sOpen = ch.isOpen(ident, p.VartOrdernr);
            if (sOpen != "1")
            {
                {
                    deletePict(p.PictIdent);
                    pN.ErrCode = -10;
                    if (sOpen == "-1")
                        pN.ErrMessage = "Order är stängd för inmatning";
                    else
                        pN.ErrMessage = sOpen;
                    return pN;
                }
            }

            string sSql = "";

            if (p.BildNr == 0)
            {
                // This is a new bild
                p.BildNr = getNextBildNr(p);

                sSql = getInsertSQL();
            }
            else
                sSql = getUpdateSQL();
            NxParameterCollection np = new NxParameterCollection();
            setParameters(np, p, true);
            
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
            deletePict(p.PictIdent);
            return p;
                      
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
            int valid = validatePicture(p, true, ref err);
            if (valid == -4 || valid == -3)
            {                
                pN.ErrCode = -1;
                pN.ErrMessage = "Det finns ingen bild lagrad för vårt ordernr : " + p.VartOrdernr + ", radnr : " + p.Radnr.ToString() + " bild nr : " + p.BildNr.ToString();
                return pN;
            }

            CServiceHuvud ch = new CServiceHuvud();
            string sOpen = ch.isOpen(ident, p.VartOrdernr);
            if (sOpen != "1")
            {
                {
                    deletePict(p.PictIdent);
                    pN.ErrCode = -10;
                    if (sOpen == "-1")
                        pN.ErrMessage = "Order är stängd för inmatning";
                    else
                        pN.ErrMessage = sOpen;
                    return pN;
                }
            }


            string sSql = "";


            sSql = " delete from servrad_bild "
                + " where vart_ordernr = :vart_ordernr "
                + " and radnr = :radnr "
                + " and bild_nr = :bild_nr ";

            NxParameterCollection np = new NxParameterCollection();
            setParameters(np, p, false);

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

            p.VartOrdernr = "";
            p.Radnr = 0;
            p.BildNr = 0;
            p.PictIdent = "";
            p.ErrCode = 0;
            p.ErrMessage = "";
            p.Description = "";
            p.PictCatID = 0;
            return p;                    

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
        public Stream downLoadPict( string pictIdent, ref string error )
        {
            try
            {
                string path = getUploadDirectory(pictIdent);
                if (!File.Exists(path))
                {
                    error = "Filen " + pictIdent + " finns ej på servern ";
                    return null;
                }
                Stream s = File.OpenRead(path);
                s.Position = 0;                
                return s;                     
            }
            catch (Exception ex)
            {
                error = "Error when downLoadPict is called. Error message : " + ex.Message;                
            }
            return null;

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
            string fileName = Guid.NewGuid().ToString() + ".jpg";
            try
            {
                string path = getUploadDirectory(fileName);                    

                const int bufferSize = 2048;
                byte[] buffer = new byte[bufferSize];
                using (FileStream outputStream = new FileStream(path,FileMode.Create, FileAccess.Write))
                {
                    int bytesRead = sPict.Read(buffer, 0, bufferSize);
                    while (bytesRead > 0)
                    {
                        outputStream.Write(buffer, 0, bytesRead);
                        bytesRead = sPict.Read(buffer, 0, bufferSize);
                    }
                    outputStream.Close();
                }

            }
            catch (Exception ex)
            {
                return "-1 " + ex.Message;
            }
            return fileName;
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
            List<PictureCL> pList = new List<PictureCL>();

            CReparator cr = new CReparator();

            int identOK = cr.checkIdent(ident);

            if (identOK == -1)
            {
                PictureCL p = new PictureCL();
                p.ErrCode = -10;
                p.ErrMessage = "Ogiltigt login";
                pList.Add(p);
                return pList;
            }


            string sSql = " SELECT vart_ordernr, radnr, bild_nr, bild, pictDescript, pictSize, pictType, pictCatID "
                         + " FROM servrad_bild "
                         + " where vart_ordernr = :vart_ordernr "
                         + " and radnr = :radnr ";           
        

            NxParameterCollection np = new NxParameterCollection();
            np.Add("vart_ordernr", vartOrdernr);
            np.Add("radnr", radnr);                        

            string errText = "";

            DataTable dt = cdb.getData(sSql, ref errText, np);

            int errCode = -100;

            if (errText == "" && dt.Rows.Count == 0)
            {            
                errText = "Det finns inga bilder för aktuell servicerad";
                errCode = 0;
            }


            if (errText != "")
            {
                PictureCL p = new PictureCL();
                if (errText.Length > 2000)
                    errText = errText.Substring(1, 2000);
                p.ErrCode = errCode;
                p.ErrMessage = errText;
                pList.Add(p);
                return pList;
            }


            foreach (DataRow dr in dt.Rows)
            {
                PictureCL p = new PictureCL();
                p.ErrCode = 0;
                p.ErrMessage = "";
                p.VartOrdernr = dr["vart_ordernr"].ToString();
                p.Radnr = Convert.ToInt32(dr["radnr"]);
                p.BildNr = Convert.ToInt32(dr["bild_nr"]);
                p.Description = dr["pictDescript"].ToString();
                p.pictSize = Convert.ToInt64(dr["pictSize"]);
                p.pictType = dr["pictType"].ToString();
                p.PictCatID = 0;
                if (dr["pictCatID"] != DBNull.Value)
                p.PictCatID = Convert.ToInt32(dr["pictCatID"]);
                pList.Add(p);
            }

            return pList;

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
        public List<PictCatCL> getPictCategories(string ident, int Step)
        {
            List<PictCatCL> pcl = new List<PictCatCL>();
            CReparator cr = new CReparator();
            int identOK = cr.checkIdent(ident);

            if (identOK == -1)
            {
                PictCatCL p = new PictCatCL();                
                p.ErrCode = -10;
                p.ErrMessage = "Ogiltigt login";
                pcl.Add(p);
                return pcl;
            }

            NxParameterCollection pc = new NxParameterCollection();

            // if argument Step = 0 then return all rows
            // otherwise return all rows matching the current step
            // and all steps with value of 0
            string sSql = " SELECT PictCatID, PictCatName, Step "
                        + " FROM PictCategory ";
            if (Step > 0)
            {
                sSql += " where step = :step "
                    + " or step = 0 ";

                
                pc.Add("step", Step);

            }


            string errText = "";

            DataTable dt = cdb.getData(sSql, ref errText, pc);

            if (errText != "")
            {
                PictCatCL p = new PictCatCL();                                
                if (errText.Length > 2000)
                    errText = errText.Substring(1, 2000);
                p.ErrCode = -100;
                p.ErrMessage = errText;
                pcl.Add(p);
                return pcl;
            }

            foreach (DataRow dr in dt.Rows)
            {
                PictCatCL p = new PictCatCL();
                p.PictCatID = Convert.ToInt32(dr["PictCatID"]);
                p.PictCatName = dr["PictCatName"].ToString();
                p.Step = Convert.ToInt16(dr["Step"]);
                p.ErrCode = 0;
                p.ErrMessage = "";
                pcl.Add(p);
            }

            return pcl;

        }

          
           




        
    }
}