using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Serialization;
using NexusDB.ADOProvider;
using System.Data;
using System.Net;
using System.Diagnostics;

namespace SManApi.CompactStore
{

    public class CCompactStore
    {

        CDB cdb = null;
        string TempCompStoreDir = "";
        string FinalCompStoreDir = "";
        string FileMoveProgrDir = "";
        string localReadyFolder = "";

        public CCompactStore()
        {
            cdb = new CDB();
            CDBInstalln cdbi = new CDBInstalln();
            TempCompStoreDir = cdbi.TempCompStoreDir;
            FinalCompStoreDir = cdbi.FinalCompStoreDir;
            FileMoveProgrDir = cdbi.FileMoveProgrDir;
            localReadyFolder = cdbi.localReadyFolder;
        }

        private string getTempPath(ref string error)
        {
            error = "";
            try
            {
                //CDBInstalln inst = new CDBInstalln();


                string PhysicalPath = HttpContext.Current.Server.MapPath(TempCompStoreDir);
                if (!Directory.Exists(PhysicalPath))
                    Directory.CreateDirectory(PhysicalPath);
            }
            catch (Exception ex)
            {
                error = ex.Message;
                TempCompStoreDir = "";
            }
            return TempCompStoreDir;
        }

        private string getFileName(string vartOrdernr)
        {
            string dtString = System.DateTime.Now.ToString();
            dtString = dtString.Replace("-", "");
            dtString = dtString.Replace(":", "");
            dtString = dtString.Replace(".", "");
            dtString = dtString.Replace(" ", "");
            return vartOrdernr + "_" + dtString + ".xml";
        }

        /// <summary>
        /// Generate file for lagerautomat
        /// </summary>
        /// <param name="vartOrdernr"></param>
        /// <returns></returns>
        /// 2018-04-24 Indentive AB
        public ErrorCL genCompStoreData(string ident, string vartOrdernr)
        {
            ErrorCL err = new ErrorCL();
            err.ErrCode = 0;
            err.ErrMessage = "";

            CReparator cr = new CReparator();
            int identOK = cr.checkIdent(ident);
            if (identOK == -1)
            {
                err.ErrCode = -100;
                err.ErrMessage = "Ogiltigt login";
                return err;
            }
            Decimal countArticles = countArtForStorageAut(vartOrdernr);

            if (countArticles > -0.001M && countArticles < 0.001M)
            {
                err.ErrCode = 1;
                err.ErrMessage = "Inga artiklar att skicka till lageraut. Order number : " + vartOrdernr;
                return err;

            }

            string error = "";
            string tempDir = getTempPath(ref error);

            if (error == "")
            {
                error = createFile(tempDir, vartOrdernr);
            }

            if (error != "")
            {
                err.ErrCode = -1207;
                err.ErrMessage = error;
            }
            return err;
        }


        private string createFile(string tempDir, string vartOrdernr)
        {
            string error = "";
            string fileName = "";
            List<updateOAStorageData> updateList = new List<updateOAStorageData>();
            try
            {
                if (!tempDir.EndsWith("/"))
                    tempDir += "/";
                fileName = getFileName(vartOrdernr);                                    
                tempDir += fileName;
                tempDir = HttpContext.Current.Server.MapPath(tempDir);
                XmlSerializer serializer =
               new XmlSerializer(typeof(CCompStoreData.TRANSACTION));

                var appendMode = false;
                Encoding encoding = Encoding.GetEncoding("ISO-8859-1");
                TextWriter writer = new StreamWriter(tempDir, appendMode, encoding);
                CCompStoreData.TRANSACTION trans = new CCompStoreData.TRANSACTION();

                trans.Company = 1;
                string s = System.DateTime.Today.ToString().Replace("-", "");
                trans.Date = System.DateTime.Today.ToString("yyMMdd");
                trans.Time = System.DateTime.Now.ToString();


                DataTable dt = getOrderArtRows(vartOrdernr, ref error);


                if (error != "")
                    return error;
                List<CCompStoreData.TRANSACTIONORDERLINE> transLines = new List<CCompStoreData.TRANSACTIONORDERLINE>();



                foreach (DataRow dr in dt.Rows)
                {
                    Decimal stockToSend = 0;
                    string errTxt = "";
                    try
                    {
                        errTxt = "";
                        stockToSend = Convert.ToDecimal(dr["sum_antal"]) - Convert.ToDecimal(dr["oa_sum"]);
                        if (stockToSend > -0.001M && stockToSend < 0.001M)
                            continue;
                        CCompStoreData.TRANSACTIONORDERLINE transOrder = new CCompStoreData.TRANSACTIONORDERLINE();
                        int AssignmentTypeId = 2;
                        if (stockToSend < 0)
                            AssignmentTypeId = 1;
                        transOrder.AssignmentTypeId = Convert.ToSByte(AssignmentTypeId);
                        transOrder.ItemDescription = dr["artnamn"].ToString();
                        transOrder.ItemNo = dr["artikelkod"].ToString();
                        transOrder.OrderlineNo = getNextRowNo().ToString();
                        transOrder.OrderNo = vartOrdernr;
                        transOrder.ReqQuantity = calculateQuantity(stockToSend);
                        transOrder.Status = 0;


                        transLines.Add(transOrder);
                    }
                    catch (Exception ex)
                    {
                        errTxt = ex.Message;
                    }
                    updateOAStorageData us = new updateOAStorageData();
                    Decimal stockToSend2 = errTxt == "" ? stockToSend : 0;
                    us.orderArtId = Convert.ToInt32(dr["orderArtId"]);
                    us.stockToSend = stockToSend2;
                    us.error = errTxt;
                    updateList.Add(us);
                }
                trans.ORDERLINES = transLines;
                serializer.Serialize(writer, trans);
                writer.Close();                
                error = copyToCSshare(tempDir, fileName);
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            if (error == "")
                error = updateDB(updateList, fileName, error);
            return error;
        }

        private string calculateQuantity(Decimal ldQuantity)
        {
            ldQuantity = Math.Abs(ldQuantity);
            Decimal quant = ldQuantity * 100;
            int quantInt = Convert.ToInt32(quant);
            if (quantInt % 100 == 0)
            {
                return Convert.ToInt32(ldQuantity).ToString().Replace(".", ",");
            }
            return ldQuantity.ToString().Replace(".", ",");

        }


        private string copyToCSshare(string copyFrom, string fileName)
        {
            string error = "";
            try
            {
                string fn = Path.GetFileName(copyFrom);
                string toPath = Path.GetDirectoryName(copyFrom);
                toPath = Path.Combine(toPath, localReadyFolder);
                toPath = toPath.EndsWith("\\") ? toPath : toPath + "\\";
                toPath += fn;
                File.Copy(copyFrom, toPath);
                /*
                                             
                toPath = toPath.EndsWith("\\") ? toPath : toPath + "\\";
                toPath += fn;
                File.Copy(copyFrom, toPath);

                

                

                string copyTo = FinalCompStoreDir.EndsWith("\\") ? FinalCompStoreDir : FinalCompStoreDir + "\\";             
                copyTo += fileName;
                File.Copy(copyFrom, copyTo, false);
                
                
                NetworkCredential credentials = new NetworkCredential(CompStoreUser, CompStorePwd, "vtm");
                using (new CNetworkConnection(copyTo, credentials))
                {
                    copyTo += fileName;
                    File.Copy(copyFrom, copyTo);
                }

    */


            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            /*
            if (error == "")
            {
                try
                {
                    
                    Process proc = new Process();
                    System.Security.SecureString ssPwd = new System.Security.SecureString();
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.FileName = FileMoveProgrDir;
                    //proc.StartInfo.Domain = "WIN2012";
                    proc.StartInfo.UserName = "administratör";
                    string password = "vent-vent-1";
                    for (int i = 0; i < password.Length; i++)
                        ssPwd.AppendChar(password[i]);
                    proc.StartInfo.Password = ssPwd;
                    
                    Process.Start(FileMoveProgrDir);
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                }
            }                
            */
            return error;
        }


        private string updateDB(List<updateOAStorageData> updateList, string fileName, string error)
        {
            string errorTxt = "";            
            string sSqlInsert = "insert into oaStorage (orderArtId, antal, regdat, result, fileName, resultDescr) "
                              + " values(:orderArtId, :antal, :regdat, :result, :fileName, :resultDescr) ";
            NxConnection cn = cdb.getConn();
            NxCommand cm = new NxCommand(sSqlInsert, cn);
            cm.Parameters.Add("orderArtId", DbType.Int32);
            cm.Parameters.Add("antal", DbType.Decimal);
            cm.Parameters.Add("result", DbType.String);
            cm.Parameters.Add("regdat", System.DateTime.Now);
            cm.Parameters.Add("fileName", fileName);
            cm.Parameters.Add("resultDescr", error);
            try
            {
                foreach (updateOAStorageData oad in updateList)
                {
                    cm.Parameters["orderArtId"].Value = oad.orderArtId;
                    cm.Parameters["antal"].Value = oad.stockToSend;
                    int result = oad.error == "" ? 1 : 0;
                    cm.Parameters["result"].Value = result;
                    cm.Parameters["resultDescr"].Value = oad.error.Length > 50 ? oad.error.Substring(0, 49) : oad.error;
                    cn.Open();
                    cm.ExecuteNonQuery();
                    cn.Close();
                }
            }
            catch (Exception ex)
            {
                errorTxt = "Fel vid uppdatering av oaStorage : " + ex.Message;
            }
            return errorTxt;
        }

        public string updateDbWithoutSend(updateOAStorageData data, string filename)
        {
            List<updateOAStorageData> list = new List<updateOAStorageData>();
            list.Add(data);
            return updateDB(list, filename, "");       
        }


        public string updateDbFromArticleCommit(List<updateOAStorageData> oasData, string fileName, string error)
        {
            return updateDB(oasData, fileName, error);
        }



        /// <summary>
        /// Count how many articles that are du to
        /// be sent to CompactStore
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="vartOrdernr"></param>
        /// <returns>No of items or -1 if error (ident fails)</returns>
        /// 2018-04-22
        public Decimal countArtForStorageAut(string ident, string vartOrdernr)
        {
            CReparator cr = new CReparator();
            int identOK = cr.checkIdent(ident);
            if (identOK == -1)
            {
                return -1;
            }
            return countArtForStorageAut(vartOrdernr);
        }


        /// <summary>
        /// Count how many articles that are du to
        /// be sent to CompactStore
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="vartOrdernr"></param>
        /// <returns>No of items or -1 if error (ident fails)</returns>
        /// 2018-04-22
        private Decimal countArtForStorageAut(string vartOrdernr)
        {
            string sSql = "SELECT coalesce(sum(oa.coAntal),0) - coalesce(sum(oa.ciAntal),0) sum_antal "
                        + " FROM orderArt oa "
                        + " where oa.vart_ordernr = :vart_ordernr ";
            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("vart_ordernr", vartOrdernr);
            string error = "";
            DataTable dt = cdb.getData(sSql, ref error, pc);
            Decimal orderArtSum = 0;
            if (dt.Rows.Count > 0)
                orderArtSum = Convert.ToDecimal(dt.Rows[0]["sum_antal"]);
            sSql = "select coalesce(sum(oas.antal),0) sum_oas "
                + " from oaStorage oas "
                + " join orderArt oa on oas.orderArtId = oa.orderArtId "
                + " where oa.vart_ordernr = :vart_ordernr ";
            error = "";
            dt = cdb.getData(sSql, ref error, pc);
            Decimal sum_oas = 0;
            if (dt.Rows.Count > 0)
                sum_oas = Convert.ToDecimal(dt.Rows[0]["sum_oas"]);
            return orderArtSum - sum_oas;
        }

        private DataTable getOrderArtRows(string vartOrdernr, ref string error)
        {
            String sSql = "select oa.orderArtId, a.artnamn, a.artikelkod, coalesce(sum(oa.coAntal),0) - coalesce(sum(oa.ciAntal),0) sum_antal "
                        + " from orderArt oa "
                        + " join artikel a on oa.artnr = a.artnr "
                        + " where oa.vart_ordernr = :vart_ordernr "
                        + " group by oa.orderArtId, a.artnamn, a.artikelkod ";

            String sSqlOa = "select coalesce(sum(oas.antal),0) oa_sum "
                        + " from oaStorage oas "
                        + " where oas.orderArtId = :orderArtId ";

            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("vart_ordernr", vartOrdernr);
            pc.Add("orderArtId", DbType.Int32);
            error = "";
            DataTable dt = cdb.getData(sSql, ref error, pc);
            dt.Columns.Add("oa_sum", Type.GetType("System.Decimal"));

            DataTable dt2 = null;
            foreach (DataRow dr in dt.Rows)
            {
                int orderArtId = Convert.ToInt32(dr["orderArtId"]);
                pc["orderArtId"].Value = orderArtId;
                dt2 = cdb.getData(sSqlOa, ref error, pc);
                if (dt2.Rows.Count > 0)
                    dr["oa_sum"] = Convert.ToDecimal(dt2.Rows[0]["oa_sum"]);
            }
            return dt;

        }


        private int getNextRowNo()
        {
            string sSql = " SELECT NextRowNo "
                        + " FROM CompactStoreRow ";
            NxConnection cn = cdb.getConn();
            NxCommand cm = new NxCommand(sSql, cn);
            cn.Open();
            int nextRowNo = Convert.ToInt32(cm.ExecuteScalar());
            cn.Close();
            int updateRow = nextRowNo + 1;
            string sSqlUpdate = " update CompactStoreRow "
                            + " set NextRowNo = " + updateRow.ToString() + " ";
            NxCommand cmUpdate = new NxCommand(sSqlUpdate, cn);
            cn.Open();
            cmUpdate.ExecuteNonQuery();
            cn.Close();
            return nextRowNo;

        }




    }
}
