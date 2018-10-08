using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace SManApi
{
    public class CDBInstalln
    {
        CDB cdb = null;
        DataTable dt = null;
        public CDBInstalln()
        {
            cdb = new CDB();
            getBaseData();
        }

        private void getBaseData()
        {
            string sSql = " select TempCompStoreDir , FinalCompStoreDir, FileMoveProgrDir, localReadyFolder, TempArticleCommitDir, LocalArticleCommitReadyFolder  from installn ";
            string ErrStr = "";
            dt = cdb.getData(sSql, ref ErrStr);
        }

        public string TempCompStoreDir
        {
            get { return dt.Rows[0]["TempCompStoreDir"].ToString(); }
        }

        public string FinalCompStoreDir
        {
            get { return dt.Rows[0]["FinalCompStoreDir"].ToString(); }
        }

        public string FileMoveProgrDir
        {
            get { return dt.Rows[0]["FileMoveProgrDir"].ToString(); }
        }
        public string localReadyFolder
        {
            get { return dt.Rows[0]["localReadyFolder"].ToString(); }
        }

        /// <summary>
        /// Temporary directory when creating Article commit information
        /// This directory is relative to the web project
        /// </summary>
        /// 2018-08-24 KJBO
        public string tempArtCommitDir
        {
            get { return dt.Rows[0]["TempArticleCommitDir"].ToString(); }
        }

        /// <summary>
        /// Folder (unnder tempArtCommitDir) to copy the file
        /// when file creation is ready
        /// </summary>
        /// 2018-08-24 KJBO
        public string localArtCommitReadyFolder
        {
            get { return dt.Rows[0]["LocalArticleCommitReadyFolder"].ToString(); }
        }


    }
}