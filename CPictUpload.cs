using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Hosting;

namespace SManApi
{
    public class CPictUpload
    {


        //public Stream DownloadFile(string fileName, string fileExtension)
        //{
        //    string downloadFilePath = Path.Combine(HostingEnvironment.MapPath("~/FileServer/Extracts"), fileName + "." + fileExtension);

        //    //Write logic to create the file
        //    File.Create(downloadFilePath);

        //    String headerInfo = "attachment; filename=" + fileName + "." + fileExtension;
        //    WebOperationContext.Current.OutgoingResponse.Headers["Content-Disposition"] = headerInfo;

        //    WebOperationContext.Current.OutgoingResponse.ContentType = "application/octet-stream";

        //    return File.OpenRead(downloadFilePath);
        //}

        //public void UploadFile(string fileName, Stream stream)
        public void UploadFile(Stream stream)
        {
            string FilePath = Path.Combine(HostingEnvironment.MapPath("~/Uploads"), "xyz.jpg");

            int length = 0;
            using (FileStream writer = new FileStream(FilePath, FileMode.Create))
            {
                int readCount;
                var buffer = new byte[8192];
                while ((readCount = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    writer.Write(buffer, 0, readCount);
                    length += readCount;
                }
            }
        }


    }
}