using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;
using SManApi.CompactStore;
using SManApi.ExportToPyramid;

namespace SManApi.ArticleCommit
{

    public class CArticleCommit
    {
        private CDB cdb = new CDB();
        string tempArtCommitDir = "";
        private string localArtCommitReadyFolder = "";
        string extens = "";


        public CArticleCommit()
        {
            cdb = new CDB();
            CDBInstalln cdbi = new CDBInstalln();
            tempArtCommitDir = cdbi.tempArtCommitDir;
            localArtCommitReadyFolder = cdbi.localArtCommitReadyFolder;
        }

        public ErrorCL generateFile(List<CArticleCommitData> data, string aExtens)
        {
            ErrorCL err = new ErrorCL();
            err.ErrCode = 0;
            err.ErrMessage = "";

            if (data.Count == 0)
                return err;
            extens = aExtens;
            string error = "";
            string tempDir = getTempPath(ref error);

            if (error == "")
            {
                error = createFile(data);
            }

            if (error != "")
            {
                err.ErrCode = -1210;
                err.ErrMessage = error;
            }
            return err;
        }


        private string createFile(List<CArticleCommitData> data)
        {

            string error = "";

            string fileName = "";
            

            try
            {
                if (!tempArtCommitDir.EndsWith("/"))
                    tempArtCommitDir += "/";
                fileName = getFileName(data[0]);
                tempArtCommitDir += fileName;
                tempArtCommitDir = HttpContext.Current.Server.MapPath(tempArtCommitDir);
                List<string> rows = new List<string>();
                foreach (CArticleCommitData acData in data)
                {
                    string commitType = "2";
                    Decimal quant = acData.quantity;
                    if (acData.quantity < 0)
                    {
                        commitType = "1";
                        quant = -quant;                        
                    }
                    rows.Add("<ART>" + commitType + ";" + acData.orderNumber + ";" + quant.ToString() + ";" + acData.articleNumber + ";1");
                    //string text = "<ART>" + commitType + ";" + orderNumber + ";" + quantity.ToString() + ";" + articleNumber + ";1";
                }
                Encoding encoding = Encoding.GetEncoding("ISO-8859-1");
                File.WriteAllLines(tempArtCommitDir, rows, encoding);
                //File.WriteAllText(tempArtCommitDir, text, encoding);
                error = copyToFinal(tempArtCommitDir, fileName);
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            CCompactStore cs = new CCompactStore();
            List<updateOAStorageData> dataList = new List<updateOAStorageData>();
            foreach (CArticleCommitData acData in data)
            {
                updateOAStorageData oasData = new updateOAStorageData();
                oasData.orderArtId = acData.orderArtID;
                
                oasData.stockToSend = acData.quantity;
                oasData.error = error;
                dataList.Add(oasData);
            }
            cs.updateDbFromArticleCommit(dataList, fileName, "");


            /*
            if (extens == "3")
            {
                CExportToPyramid expPyr = new CExportToPyramid();
                expPyr.addToPyramidOrder()
                addToPyramidOrder

            }
            */



            //UpdateDB
            return error;
        }
        private string getTempPath(ref string error)
        {
            error = "";
            try
            {
                string PhysicalPath = HttpContext.Current.Server.MapPath(tempArtCommitDir);
                if (!Directory.Exists(PhysicalPath))
                    Directory.CreateDirectory(PhysicalPath);
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return tempArtCommitDir;

        }


        private string getFileName(CArticleCommitData acData)
        {

            string dtString = System.DateTime.Now.ToString();
            dtString = dtString.Replace("-", "");
            dtString = dtString.Replace(":", "");
            dtString = dtString.Replace(".", "");
            dtString = dtString.Replace(" ", "");
            string fileName = acData.orderNumber + "_" + dtString + extens;
            string fileNameTry = fileName;
            int version = 0;
            while (File.Exists(fileNameTry + ".txt"))
            {
                version++;
                fileName += version.ToString();
                fileNameTry = fileName;
            }
            return fileNameTry + ".txt";
        }


        private void ensurePhysicalPathExists(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }


        private string copyToFinal(string copyFrom, string fileName)
        {
            string error = "";
            try
            {
                string fn = Path.GetFileName(copyFrom);
                string toPath = Path.GetDirectoryName(copyFrom);
                toPath = Path.Combine(toPath, localArtCommitReadyFolder);
                toPath = toPath.EndsWith("\\") ? toPath : toPath + "\\";
                ensurePhysicalPathExists(toPath);
                toPath += fn;
                File.Copy(copyFrom, toPath);
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            return error;
        }








    }
}





