using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NexusDB.ADOProvider;
using System.Data;
using System.Collections;
using System.Collections.Specialized;
using System.Drawing;

namespace SManApi
{
    // Class for database communication
    public class CDB
    {
        
        

        /// <summary>
        /// Get a connection with connection string from web.config
        /// </summary>
        /// <returns></returns>
        public NxConnection getConn()
        {
            string cs = CConfig.getCS();
            NxConnection cn = new NxConnection(cs);
            return cn;
        }

        
        
         


        /// <summary>
        /// Takes an sql clause and returns a DataTable with the result
        /// 2016-02-02 KJBO Pegas AB
        /// </summary>
        /// <param name="sSql">The SQL clause to be executed</param>
        /// <param name="errText">Reference parameter that returns error string</param>
        /// <param name="pc">Parameter collection</param>
        /// <returns>Result of SQL clause in a DataTable</returns>
        public DataTable getData(string sSql, ref string errText, NxParameterCollection pc)
        {
            NxConnection cn = getConn();
            NxCommand cm = new NxCommand(sSql, cn);
            

            errText = "";

            if (pc != null)
            {
                foreach (NxParameter np in pc)
                {
                    NxParameter npInsert = (NxParameter)np.Clone();
                    cm.Parameters.Add(npInsert);                    
                }
            }

            // Create a data adapter
            NxDataAdapter da = new NxDataAdapter(cm);

            // Datatable for the result
            DataTable dt = new DataTable();
            try
            {
                // Fill data table and...
                da.Fill(dt);
            }
            catch (Exception ex)
            {                
                errText = ex.Message;
            }
            // return result
            return dt;
        }







        /// <summary>
        /// Takes an sql clause and returns a DataTable with the result
        /// 2016-02-02 KJBO Pegas AB
        /// </summary>
        /// <param name="sSql">The SQL clause to be executed</param>
        /// <param name="errText">Reference parameter that returns error string</param>
        /// <returns>Result of SQL clause in a DataTable</returns>
        public DataTable getData(string sSql, ref string errText)
        {
            return getData(sSql, ref errText, null);
        }

        public int updateData(string sSql, ref string errText)
        {
            return updateData(sSql, ref errText, null);
        }


        /// <summary>
        /// Updates the database from the given SQL Clause
        /// </summary>
        /// <param name="sSql">SQL Clause to be executed</param>
        /// <param name="errText">Refernce string that returns error string</param>
        /// <param name="pc">Collection of parameters</param>
        /// <returns>Number of updated rows</returns>
        public int updateData( string sSql, ref string errText, NxParameterCollection pc)
        {
            NxConnection cn = getConn();
            NxCommand cm = new NxCommand(sSql, cn);

            // Check if there are any parameters
            if (pc != null)
            {
                foreach (NxParameter np in pc)
                {
                    NxParameter npInsert = (NxParameter)np.Clone();
                    cm.Parameters.Add(npInsert);
                }
            }
            errText = "";
            int result = 0;
            try
            {
                cn.Open();
                result = cm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                errText = ex.Message;
                result = -1;
            }
            finally
            {
                if (cn.State == ConnectionState.Open)
                    cn.Close();
            }
            return result;
        }


        public bool bCheckIfTableExist(string tablename)
        {
            string sSql = " select count(*) as countTable "
                        + " from #tables "
                        + " where table_name = '" + tablename + "' ";

            string err = "";
            DataTable dt = getData(sSql, ref err);

            return Convert.ToInt16(dt.Rows[0][0]) == 1;
        }


        public int createAnvServRadTable()
        {
            string sSql = " select cast('' as ShortString(10)) as anvID, sr.* "
                        + " into anvServicerad "
                        + " from servicerad sr "
                        + " where 1 = 0 ";

            string err = "";
            return updateData(sSql, ref err);

        }



    }
}