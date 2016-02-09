using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NexusDB.ADOProvider;
using System.Data;

namespace SManApi
{
    public class CKund
    {
        CDB cdb = null;
        public CKund()
        {
            cdb = new CDB();
        }


        public int validateKund(string  kundID)
        {
            string sSql = " Select count(*) as antal "
                        + " from kund "
                        + " where kund_id = :pKundID ";

            NxParameterCollection np = new NxParameterCollection();
            np.Add("pKundID", kundID);

            string errText = "";

            DataTable dt = cdb.getData(sSql, ref errText, np);

            return Convert.ToInt16(dt.Rows[0][0]);
        }


    }
}