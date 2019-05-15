using NexusDB.ADOProvider;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

namespace SManApi.Drawing
{
    public class CDrawing
    {

        CDB cdb = null;
        CPicture cpict = null;
        public CDrawing()
        {
            cdb = new CDB();
            cpict = new CPicture();
        }

        private string saveDrawingToFile(MemoryStream m, ref string error, ref long fileSize)
        {
            string path = "";
            string fileName = "";
            try
            {
                // Get the file name from GUID
                fileName = Guid.NewGuid().ToString() + ".pdf";
                // Get the upload directory (both for upload and download)                
                path = cpict.getUploadDirectory(fileName);
                using (FileStream file = new FileStream(path, FileMode.Create, FileAccess.Write))
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
                error = "Error when temporary storing drawing to a new file in the upload directory (" + path + "). Error message : " + ex.Message;
                fileName = "-1 " + ex.Message;
            }
            return fileName;
        }



        /// <summary>
        /// Upload a drawing from client to server
        /// </summary>
        /// <param name="sPict"></param>
        /// <returns></returns>
        /// 2019-05-06 KJBO
        public string uploadDrawing(Stream sPict)
        {
            string fileName = Guid.NewGuid().ToString() + ".pdf";
            try
            {                
                string path = cpict.getUploadDirectory(fileName);

                const int bufferSize = 2048;
                byte[] buffer = new byte[bufferSize];
                using (FileStream outputStream = new FileStream(path, FileMode.Create, FileAccess.Write))
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
        /// Save a previous uploaded drawing to database
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public DrawingCL saveDrawing(string ident, DrawingCL d)
        {
            
            CReparator cr = new CReparator();
            int identOK = cr.checkIdent(ident);

            // Creates a class to return an error
            DrawingCL cdRet = new DrawingCL();
            if (identOK == -1)
            {

                cpict.deletePict(d.DrawingIdent);
                cdRet.ErrCode = -10;
                cdRet.ErrMessage = "Ogiltigt login";
                return cdRet;
            }

            // Init variable
            string err = "";
            int valid = validateDrawing(d, false, false, ref err);         
            if (valid == -1 || valid == -4)
            {
                cpict.deletePict(d.DrawingIdent);
                cdRet.ErrCode = -1;
                cdRet.ErrMessage = "VentilId saknas eller felaktigt";
                return cdRet;
            }

            if (valid == -5)
            {
                cdRet.ErrCode = -1;
                cdRet.ErrMessage = "Ritning saknas i uppladdningbiblioteket";
                return cdRet;
            }




            string sSql = "";

            if (d.DrawingNo == 0)
            {
                d.DrawingNo = getNextDrawinNo(d);
                sSql = getInsertSql();
            }
            else
                sSql = getUpdateSQL(true);
            NxParameterCollection np = new NxParameterCollection();
            setParameters(np, d, true);

            string errText = "";

            int iRc = cdb.updateData(sSql, ref errText, np);

            if (errText != "")
            {
                if (errText.Length > 2000)
                    errText = errText.Substring(1, 2000);
                cdRet.ErrCode = -100;
                cdRet.ErrMessage = errText;
                return cdRet;
            }
            cpict.deletePict(d.DrawingIdent);
            return d;


        }


        private bool drawingExists(string ventil_id, int drawingNo)
        {
            string sSql = " select count(*) countDrawing "
                        + " from valveDrawing "
                        + " where ventil_id = :ventil_id "
                        + " and drawingNo = :drawingNo ";
            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("ventil_id", ventil_id);
            pc.Add("drawingNo", drawingNo);
            string err = "";
            DataTable dt = cdb.getData(sSql, ref err, pc);
            if (dt.Rows.Count == 0)
                return false;
            return Convert.ToInt32(dt.Rows[0]["countDrawing"]) > 0;

        }

        private int validateDrawing(DrawingCL d, bool forDelete, bool forUpdateMeta, ref string err)
        {
            
            if (string.IsNullOrEmpty(d.ventil_id))
                return -1;
            if (forDelete)
            {
                if (d.DrawingNo == 0)
                    return -2;
            }
            if (forDelete || d.DrawingNo > 0)
            {
                if (!drawingExists(d.ventil_id, d.DrawingNo))
                    return -3;
            }
            else if (d.DrawingNo == 0)
            {
                CVentil v = new CVentil();
                string dummy = "";
                if (v.validateVentilID(d.ventil_id, ref dummy) == 0)
                    return -4;
            }
            if (!forDelete)
            {
                if (!cpict.validatePictIdent(d.DrawingIdent))
                    return -5;
            }

            return 1;
        }

        private int getNextDrawinNo(DrawingCL d)
        {

            string sSql = " select coalesce(max(DrawingNo),0) as maxDrawingNo "
                        + " from valveDrawing "
                        + " where ventil_id = :ventil_id ";

            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("ventil_id", d.ventil_id);            
            string err = "";
            DataTable dt = cdb.getData(sSql, ref err, pc);
            return Convert.ToInt32(dt.Rows[0]["maxDrawingNo"]) + 1;            
        }


        private string getInsertSql ()
        {
            return " insert into valveDrawing (ventil_id, drawingNo, drawing, drawingDescr, drawingSize, fileType) "
                + " values ( :ventil_id, :drawingNo, :drawing, :drawingDescr, :drawingSize, :fileType) ";

        }


        private string getUpdateSQL(bool imageUpdate)
        {
            string sSql = " update valveDrawing "                        
                        + ", drawingDescr = :drawingDescr "
                        + ", fileType = :fileType ";
            if (imageUpdate)
            {
                sSql += ", drawing = :drawing "
                    + ", drawingSize = :drawingSize ";
            }
            sSql = " where ventil_id = :ventil_id "
                + " and drawingNo = :drawingNo ";
            return sSql;
        }


        /// <summary>
        /// Set parameters function
        /// </summary>
        /// <param name="np"></param>
        /// <param name="d"></param>
        /// <param name="retrieveDrawing"></param>
        private void setParameters(NxParameterCollection np, DrawingCL d, Boolean retrieveDrawing)
        {
            long fileSize = 0;
            np.Add("ventil_id", d.ventil_id);
            np.Add("drawingNo", d.DrawingNo);
            np.Add("drawingDescr", d.Description);
            np.Add("fileType", d.FileType);
            if (retrieveDrawing)
            {
                np.Add("drawing", getDrawing(d.DrawingIdent, ref fileSize));
                np.Add("drawingSize", fileSize);
                d.DrawingSize = fileSize;
            }
        }

        private byte[] getDrawing(string drawingIdent, ref long fileSize)
        {
            string filePath = cpict.getUploadDirectory(drawingIdent);
            FileInfo f = new FileInfo(filePath);
            fileSize = f.Length;
            FileStream stream = new FileStream(
                filePath, FileMode.Open, FileAccess.Read);
            BinaryReader reader = new BinaryReader(stream);

            byte[] drawing = reader.ReadBytes((int)stream.Length);

            reader.Close();
            stream.Close();

            return drawing;
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
        public DrawingCL getDrawing(string ident, string ventilId, int ritningNo)
        {

            DrawingCL d = new DrawingCL();            
            CReparator cr = new CReparator();

            int identOK = cr.checkIdent(ident);

            if (identOK == -1)
            {
                d.ErrCode = -10;
                d.ErrMessage = "Ogiltigt login";
                return d;
            }

            string sSql = "SELECT ventil_id, drawingNo, drawing, drawingDescr, drawingSize, fileType "
                        + "  FROM valveDrawing "
                        + " where ventil_id = :ventil_id "
                        + " and drawingNo = :drawingNo ";


            NxParameterCollection np = new NxParameterCollection();
            np.Add("ventil_id", ventilId);
            np.Add("drawingNo", ritningNo);

            string errText = "";

            DataTable dt = cdb.getData(sSql, ref errText, np);

            int errCode = -100;

            if (errText == "" && dt.Rows.Count == 0)
            {
                errText = "Felaktig ritningsidentitet";
                errCode = 0;
            }


            if (errText != "")
            {

                if (errText.Length > 2000)
                    errText = errText.Substring(1, 2000);
                d.ErrCode = errCode;
                d.ErrMessage = errText;
                return d;
            }


            DataRow dr = dt.Rows[0];
            d.ErrCode = 0;
            d.ErrMessage = "";
            string error = "";
            long fileSize = 0;
            if (dr["drawing"] != DBNull.Value)
            {
                byte[] data = (byte[])dr["drawing"];
                MemoryStream ms = new MemoryStream(data);
                d.DrawingIdent = saveDrawingToFile(ms, ref error, ref fileSize);
            }
            if (error != "")
            {
                d.ErrCode = -1;
                d.ErrMessage = error;
                return d;
            }
            d.Description = dr["drawingDescr"].ToString();
            d.DrawingNo = Convert.ToInt32(dr["drawingNo"]);
            d.DrawingSize = Convert.ToInt64(dr["drawingSize"]);
            d.FileType = dr["fileType"].ToString();
            d.ventil_id = dr["ventil_id"].ToString();
            return d;

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
        public Stream downLoadDrawing(string drawingIdent, ref string error)
        {
            return cpict.downLoadPict(drawingIdent, ref error);
        }








    }
}