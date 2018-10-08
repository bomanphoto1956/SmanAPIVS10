using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace SManApi
{
    public class CMKund
    {
        CDB cdb = null;

        public CMKund()
        {
            cdb = new CDB();
        }

        /// <summary>
        /// Returns a list of customers in a key-value pair
        /// To be used by dropdowns or similar
        /// </summary>
        /// <param name="ident"></param>
        /// <returns></returns>
        public List<ListKundCL> getCustList(string ident)
        {
            List<ListKundCL> custList = new List<ListKundCL>();
            string sSql = " select kund_id, kund + ', ' + coalesce(stad,'') + ', ' + foretagskod kund "
                        + " from kund "
                        + " where kund_id <> '' "
                        + " and foretagskod is not null "
                        + " order by kund ";

            ErrorCL err = new ErrorCL();
            
            DataTable dt = cdb.getData(sSql, ident, ref err, null);

            if (err.ErrCode != 0)
            {
                ListKundCL cust = new ListKundCL();
                cust.ErrCode = err.ErrCode;
                cust.ErrMessage = err.ErrMessage;
                custList.Add(cust);
                return custList;
            }

            foreach (DataRow dr in dt.Rows )
            {
                ListKundCL cust = new ListKundCL();
                cust.ErrCode = 0;
                cust.ErrMessage = "";
                cust.kund_id = dr["kund_id"].ToString();
                cust.kund = dr["kund"].ToString();
                custList.Add(cust);
            }

            return custList;

        }


    }
}