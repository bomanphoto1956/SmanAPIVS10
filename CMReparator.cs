using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using NexusDB.ADOProvider;

namespace SManApi
{
    public class CMReparator
    {
        CDB cdb = null;
        public CMReparator()
        {
            cdb = new CDB();
        }


        /// <summary>
        /// Creates a list of all active reparatör
        /// in alphabetic order. To be used by dropdown controls
        /// or elsewhere when a key-value collection is needed
        /// </summary>
        /// <param name="ident"></param>
        /// <returns></returns>
        public List<ListRepCL> getReparatorList(string ident)
        {
            List<ListRepCL> repList = new List<ListRepCL>();
            string sSql = " SELECT AnvID, reparator "
                        + " FROM reparator "
                        + " where visas = true "
                        + " order by reparator ";
            ErrorCL err = new ErrorCL();            
            DataTable dt = cdb.getData(sSql, ident, ref err, null);

            if (err.ErrCode != 0)
            {
                ListRepCL rep = new ListRepCL();
                rep.ErrCode = err.ErrCode;
                rep.ErrMessage = err.ErrMessage;
                repList.Add(rep);
                return repList;
            }

            foreach (DataRow dr in dt.Rows)
            {
                ListRepCL rep = new ListRepCL();
                rep.ErrCode = 0;
                rep.ErrMessage = "";
                rep.AnvId = dr["AnvID"].ToString();
                rep.Reparator = dr["reparator"].ToString();
                repList.Add(rep);
            }

            return repList;
        }


        /// <summary>
        /// Get a list of all reparators connected to an order
        /// </summary>
        /// <param name="VartOrdernr"></param>
        /// <param name="ident"></param>
        /// <returns></returns>
        /// 2018-02-02 KJBO
        public List<ShReparatorCL> getShReparatorList(string VartOrdernr, string ident)
        {
            List<ShReparatorCL> shRepList = new List<ShReparatorCL>();
            string sSql = "SELECT id, vart_ordernr, AnvID "
                        + " FROM shReparator "
                        + " where vart_ordernr = :vart_ordernr ";
            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("vart_ordernr", VartOrdernr);
            ErrorCL err = new ErrorCL();
            DataTable dt = cdb.getData(sSql, ident, ref err, pc);

            if (err.ErrCode != 0)
            {
                ShReparatorCL shRep = new ShReparatorCL();
                shRep.ErrCode = err.ErrCode;
                shRep.ErrMessage = err.ErrMessage;
                shRepList.Add(shRep);
                return shRepList;
            }

            foreach (DataRow dr in dt.Rows)
            {
                ShReparatorCL shRep = new ShReparatorCL();
                shRep.Id = Convert.ToInt32(dr["id"]);
                shRep.VartOrdernr = dr["vart_ordernr"].ToString();
                shRep.AnvID = dr["AnvID"].ToString();
                shRepList.Add(shRep);
            }
            return shRepList;
        }



        /// <summary>
        /// Updates the table shReparator.
        /// If the parameter addTo order then a new row will be added
        /// with the current ordernr and AnvId
        /// if false then the row is deleted
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="VartOrdernr"></param>
        /// <param name="AnvID"></param>
        /// <param name="addToOrder"></param>
        /// <returns></returns>
        /// 2018-02-05  KJBO    
        public ErrorCL updateShrep(string ident, string VartOrdernr, string AnvID, Boolean addToOrder)
        {
            ErrorCL err = new ErrorCL();
            err.ErrCode = 0;
            err.ErrMessage = "";
            string sSql = "";
            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("vart_ordernr", VartOrdernr);
            pc.Add("AnvID", AnvID);
            if (addToOrder)
            {
                sSql = "select count(*) antal "
                    + " from shReparator "
                    + " where vart_ordernr = :vart_ordernr "
                    + " and AnvID = :AnvID ";


                DataTable dtCount = cdb.getData(sSql, ident, ref err, pc);

                if (err.ErrMessage != "")
                    return err;
                int antal = 0;
                if (dtCount.Rows.Count == 1)
                    antal = Convert.ToInt32(dtCount.Rows[0]["antal"]);

                if (antal == 0)
                {
                    sSql = "insert into shReparator (vart_ordernr, AnvID, reg, regdat) "
                        + " values(:vart_ordernr, :AnvID, :reg, :regdat); ";


                    CReparator cr = new CReparator();
                    ReparatorCL rep = cr.getReparator(ident);

                    pc.Add("reg", rep.AnvID);
                    pc.Add("regdat", DateTime.Now);

                    string errTxt = "";
                    cdb.updateData(sSql, ref errTxt, pc);

                    if (errTxt != "")
                    {
                        errTxt = "Error while inserting row in shReparator table. Error message : " + errTxt;
                        if (errTxt.Length > 2000)
                            errTxt = errTxt.Substring(1, 2000);
                        err.ErrCode = -100;
                        err.ErrMessage = errTxt;
                        return err;
                    }

                }

            }
            else
            {
                sSql = " delete from shReparator "
                    + " where vart_ordernr = :vart_ordernr "
                    + " and AnvID = :AnvID ";

                string errTxt = "";
                cdb.updateData(sSql, ref errTxt, pc);

                if (errTxt != "")
                {
                    errTxt = "Error while inserting row in shReparator table. Error message : " + errTxt;
                    if (errTxt.Length > 2000)
                        errTxt = errTxt.Substring(1, 2000);
                    err.ErrCode = -100;
                    err.ErrMessage = errTxt;
                    return err;
                }

            }
            return err;
        }


    }
}