using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using NexusDB.ADOProvider;

namespace SManApi
{
    public class CSalart
    {

        CDB cdb = null;
        // Constructor that creates an instance of the DB class
        public CSalart()
        {
            cdb = new CDB();
        }


        public int validateSalart(int salartID, bool forServiceDetalj, ref int salartCatID)
        {
            string sSql = " select st.SalartCatID "
                        + " FROM Salart s "
                        + " join SalartType st on s.salartTypeID = st.SalartTypeID ";
            if (forServiceDetalj)
                sSql += "where s.SalartTypeID = 1";
            else
                sSql += "where s.SalartTypeID > 1";
            sSql += " and s.SalartId = :salartID ";

            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("salartID", salartID);

            string errText = "";

            DataTable dt = cdb.getData(sSql, ref errText, pc);
            
            int rc = -1;
            salartCatID = 0;
            if (dt.Rows.Count == 1)
            {
                rc = 1;
                salartCatID = Convert.ToInt32(dt.Rows[0]["SalartCatID"]);
            }

            return rc;
        }
    }
}