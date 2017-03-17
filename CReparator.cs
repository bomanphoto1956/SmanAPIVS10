using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using NexusDB.ADOProvider;
using System.Collections;
using System.Collections.Specialized;

namespace SManApi
{

    /// <summary>
    /// Class for reparator
    /// </summary>
    public class CReparator
    {

        CDB cdb = null;
        // Constructor that creates an instance of the DB class
        public CReparator()
        {
            cdb = new CDB();
        }

        /// <summary>
        /// Gives a name of a reparator given an ID
        /// </summary>
        /// <param name="anvID">ID to look for</param>
        /// <returns>Name or empty string if no name is found</returns>
        public string getName(string anvID)
        {

            string sSql = " SELECT reparator FROM reparator "
                        + " where anvID = :pAnvId "
                        + " and visas = true ";

            NxParameterCollection pc = new NxParameterCollection();            
            pc.Add("pAnvID", anvID);

            string errSt = "";
            DataTable dt = cdb.getData(sSql, ref errSt, pc);

            // If name exist then return reparator
            // otherwise return empty string
            if (dt.Rows.Count == 1)
                return dt.Rows[0][0].ToString();
            return "";
        }

        /// <summary>
        /// Maintains the authenticate table with ValidUntil time
        /// </summary>
        /// <param name="AnvID">AnvandarID</param>
        /// <returns>The current or newly created GUID that the consumer shall use when calling other functions</returns>
        private string UpdateAuthenticate(string AnvID)
        {
            // SQL clause for reading values for the reparator/user
            String sSql = " SELECT anvID, Ident, ValidUntil "
                        + " FROM Authenticate "
                        + " where AnvID = :pAnvID ";
            // Create paramater collection
            NxParameterCollection pc = new NxParameterCollection();            
            pc.Add("pAnvID", AnvID);

            string errSt = "";
            DataTable dt = cdb.getData(sSql, ref errSt, pc);

            // Get current date and time
            DateTime validUntil = DateTime.Now;
            // Add one day
            validUntil = validUntil.AddDays(1);

            // Add one mor parameter
            pc.Add("pValidUntil", validUntil.ToString());

            // Init variable
            string Ident = "";
            
            // If there are one entry for the current anvandare
            if (dt.Rows.Count > 0)
            {
                // 2016-02-16 KJBO
                DateTime savedValidUntil = Convert.ToDateTime(dt.Rows[0]["ValidUntil"]);
                

                if (savedValidUntil < DateTime.Now)                
                    Ident = Guid.NewGuid().ToString(); 
                else
                    Ident = dt.Rows[0]["Ident"].ToString();

                pc.Add("ident", Ident);

                // Update the validUntil (give 24 hours from now)
                sSql = " update Authenticate "
                    + " set validUntil = :pValidUntil "
                    + " , ident = :ident "
                    + " where anvID = :pAnvID ";
                // Update the database
                string errStr = "";
                int iCount = cdb.updateData(sSql, ref errStr, pc);                
            }
                // If no entry exists for the current user then insert a new row
            else
            {
                // Get a new identity (random)
                Ident = Guid.NewGuid().ToString();
                // Add this to a parameter
                pc.Add("pIdent",Ident);

                // Insert clause with parameters
                sSql = " insert into Authenticate (anvId, Ident, ValidUntil) "
                    + " values ( :pAnvID, :pIdent, :pValidUntil) ";

                string errStr = "";
                // Update the database
                int iCount = cdb.updateData(sSql, ref errStr, pc);
            }
            // Return the identity
            return Ident;

        }




        /// <summary>
        /// Login function
        /// Also takes care of handling guids for the anvandare
        /// </summary>
        /// <param name="AnvID">Userid</param>
        /// <param name="pwd">Password</param>
        /// <returns>A guid valid in 24 hours to identify the reparator/user</returns>
        public string login(string AnvID, string pwd)
        {
            // Create parameter collection
            NxParameterCollection pc = new NxParameterCollection();
            // Add anvandID and pwd as parameters
            pc.Add("pAnvID", AnvID);
            pc.Add("pPwd", pwd);

            // Create SQL clause
            string sSql = " select reparator from reparator "
                        + " where anvID = :pAnvID "
                        + " and pwd = :pPwd "
                        + " and visas = true ";
            string errSt = "";
            DataTable dt = cdb.getData(sSql, ref errSt, pc);

            if (errSt != "")
                return "-1" + errSt;

            if (dt.Rows.Count == 1)
                return UpdateAuthenticate(AnvID);                
            return "";

        }


        



        /// <summary>
        /// Get a reparator class from ID
        /// </summary>
        /// <param name="AnvID">ID=PK</param>
        /// <returns>One reparator</returns>
        /// 
        public ReparatorCL getReparator(string ident)
        {

            int identOK = checkIdent(ident);

            if (identOK == -1)
            {

                ReparatorCL r = new ReparatorCL();
                r.Reparator = "";
                r.AnvID = "";
                r.RepKatID = "";
                r.ErrCode = -10;
                r.ErrMessage = "Ogiltigt login";
                return r;
            }


            NxParameterCollection pc = new NxParameterCollection();            
            pc.Add("pIdent", ident);

            string sSql = "SELECT r.reparator, r.rep_kat_id, r.AnvID "
                        + " FROM reparator r"
                        + " join authenticate a on r.AnvID = a.anvID"
                        + " where a.Ident = :pIdent "
                        + " and r.visas = true ";
            string errTxt = "";
            DataTable dt = cdb.getData(sSql,ref errTxt, pc);

            if (errTxt != "")
            {
                ReparatorCL r = new ReparatorCL();
                r.Reparator = "";
                r.AnvID = "";
                r.RepKatID = "";
                r.ErrCode = -1;
                r.ErrMessage = "Databasfel : " + errTxt;
                return r;
            }


            if (dt.Rows.Count == 1)
            {
                ReparatorCL r = new ReparatorCL();
                r.Reparator = dt.Rows[0]["reparator"].ToString();
                r.AnvID = dt.Rows[0]["AnvID"].ToString();
                r.RepKatID = dt.Rows[0]["rep_kat_id"].ToString();
                return r;
            }
            return null;
        }



        /// <summary>
        /// Get a reparator class from anvID
        /// </summary>
        /// <param name="AnvID">ID=PK</param>
        /// <returns>One reparator</returns>
        /// 
        public ReparatorCL getReparatorFromID(string anvID)
        {



            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("pAnvID", anvID);

            string sSql = "SELECT r.reparator, r.rep_kat_id, r.AnvID "
                        + " FROM reparator r"
                        + " where r.AnvID = :pAnvID "
                        + " and r.visas = true ";
            string errTxt = "";
            DataTable dt = cdb.getData(sSql, ref errTxt, pc);

            if (errTxt != "")
            {
                ReparatorCL r = new ReparatorCL();
                r.Reparator = "";
                r.AnvID = "";
                r.RepKatID = "";
                r.ErrCode = -1;
                r.ErrMessage = "Databasfel : " + errTxt;
                return r;
            }


            if (dt.Rows.Count == 1)
            {
                ReparatorCL r = new ReparatorCL();
                r.Reparator = dt.Rows[0]["reparator"].ToString();
                r.AnvID = dt.Rows[0]["AnvID"].ToString();
                r.RepKatID = dt.Rows[0]["rep_kat_id"].ToString();
                return r;
            }
            return null;
        }



        /// <summary>
        /// Get all reparators 
        /// 
        /// </summary>
        /// <returns>List of reparators</returns>
        public List<ReparatorCL> getReparators( string ident)
        {

            int identOK = checkIdent(ident);

            List<ReparatorCL> repList = new List<ReparatorCL>();
            if (identOK == -1)
            {

                ReparatorCL r = new ReparatorCL();
                r.Reparator = "";
                r.AnvID = "";
                r.RepKatID = "";
                r.ErrCode = -10;
                r.ErrMessage = "Ogiltigt login";
                repList.Add(r);
                return repList;
            }



            string sSql = "SELECT reparator, rep_kat_id, AnvID "
            + " FROM reparator "
            + " where visas = true ";

            string errSt = "";
            DataTable dt = cdb.getData(sSql, ref errSt);

            if (errSt != "")
            {
                if (errSt.Length > 2000)
                    errSt = errSt.Substring(1,2000);

                ReparatorCL r = new ReparatorCL();
                r.Reparator = "";
                r.AnvID = "";
                r.RepKatID = "";
                r.ErrCode = -100;
                r.ErrMessage = "Databasfel : " + errSt;
                repList.Add(r);
                return repList;
            }


            foreach (DataRow dr in dt.Rows)
            {
                ReparatorCL r = new ReparatorCL();
                r.Reparator = dr["reparator"].ToString();
                r.AnvID = dr["AnvID"].ToString();
                r.RepKatID = dr["rep_kat_id"].ToString();
                r.ErrCode = 0;
                r.ErrMessage = "";
                repList.Add(r);
            }

            return repList;
        }


        /// <summary>
        /// Check if a user has a valid authentication
        /// </summary>
        /// <param name="ident">Identity GUID</param>
        /// <returns>1 = OK, -1 doesnt exist</returns>
        public int checkIdent(string ident)
        {

            string sSql = " select AnvID "
                        + " from Authenticate "
                        + " where ident = :pIdent "
                        + " and validUntil >= :pValidUntil ";

            NxParameterCollection pColl = new NxParameterCollection();

            DateTime validUntil = DateTime.Now;

            pColl.Add("pIdent", ident);
            pColl.Add("pValidUntil", validUntil);

            string errSt = "";
            DataTable dt = cdb.getData(sSql, ref errSt, pColl);

            CLog cl = new CLog();
            if (dt.Rows.Count > 0)
            {
                string anvID = dt.Rows[0]["AnvID"].ToString();
                cl.log("Authenticating identity : " + anvID + " (" + ident + ")", "1");
                return 1;
            }
            cl.log("Authenticating identity : " + ident, "-1");
            return -1;
        }


        /// <summary>
        /// Get a list of all reparators assigned to one 
        /// servicehuvud identified by vartOrdernr
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="vartOrdernr"></param>
        /// <returns>A list of reparators or error</returns>
        /// 2017-03-14 Added functionality
        /// RepKatID is now current for this ordernr
        public List<ReparatorCL> getReparatorsForServiceHuvud( string ident, string vartOrdernr)
        {


            int identOK = checkIdent(ident);
            List<ReparatorCL> repList = new List<ReparatorCL>();

            ReparatorCL rep = new ReparatorCL();
            if (identOK == -1)
            {
                rep.Reparator = "";
                rep.AnvID = "";
                rep.RepKatID = "";
                rep.ErrCode = -10;
                rep.ErrMessage = "Ogiltigt login";
                repList.Add(rep);
                return repList;
            }

            string sSql = " select Coalesce(allRep,false) allRep,  Coalesce(OpenForApp,false) OpenforApp, orderAdmin "
                        + " from servicehuvud "
                        + " where vart_ordernr = :vart_ordernr ";

            NxParameterCollection np = new NxParameterCollection();
            np.Add("vart_ordernr", vartOrdernr);
            
            string errSt = "";            
            DataTable dt = cdb.getData(sSql, ref errSt, np);

            int errCode = -100;

            if (errSt == "" && dt.Rows.Count == 0)
            {
                errSt = "Vårt ordernr är felaktigt";
                errCode = 0;
            }
            

            if (errSt != "")
            {
                if (errSt.Length > 2000)
                    errSt = errSt.Substring(1, 2000);
                rep.ErrCode = errCode;
                rep.ErrMessage = errSt;
                repList.Add(rep);
                return repList;
            }

            if (Convert.ToBoolean(dt.Rows[0]["OpenforApp"]) == false)
            {
                rep.ErrCode = errCode;
                rep.ErrMessage = "Ordern är stängd för AppAnvändning";
                repList.Add(rep);
                return repList;
            }

            string orderAdmin = dt.Rows[0]["orderAdmin"] == DBNull.Value ? "" : dt.Rows[0]["orderAdmin"].ToString();
            


            if (Convert.ToBoolean(dt.Rows[0]["allRep"]) == true)
            {
                sSql = " select r.anvID, r.Reparator, r.Rep_kat_id "
                        + " from reparator r "
                        + " where r.visas = true ";


            }
            else
            {
                sSql = " select r.anvID, r.Reparator, r.Rep_kat_id "
                       + " from reparator r "
                       + " join servicehuvud s on r.anvID = s.OrderAdmin "
                       + " where s.vart_ordernr = :vart_ordernr "
                       + " and r.visas = true "
                       + " union "
                       + " select r.anvID, r.Reparator, r.Rep_kat_id "
                       + " from reparator r "
                       + " join shReparator shr on r.anvID = shr.anvID "
                       + " join servicehuvud sh on shr.vart_ordernr = sh.vart_ordernr "
                       + " where sh.vart_ordernr = :vart_ordernr "
                       + " and r.visas = true ";
            }


            errSt = "";
            dt = cdb.getData(sSql, ref errSt, np);

            errCode = -100;

            if (errSt == "" && dt.Rows.Count == 0)
            {
                errSt = "Det finns inga reparatörer med behörighet till aktuell order";
                errCode = 0;
            }


            if (errSt != "")
            {
                if (errSt.Length > 2000)
                    errSt = errSt.Substring(1, 2000);
                rep.ErrCode = errCode;
                rep.ErrMessage = errSt;
                repList.Add(rep);
                return repList;
            }

            foreach (DataRow dr in dt.Rows)
            {
                ReparatorCL r = new ReparatorCL();            
                r.AnvID = dr["anvID"].ToString();
                r.Reparator = dr["Reparator"].ToString();
                if (dr["anvID"].ToString() == orderAdmin)
                    r.RepKatID = "AL_ST";
                else
                    r.RepKatID = "REPARATOR";                
                r.ErrCode = 0;
                r.ErrMessage = "";
                repList.Add(r);
            }

            return repList;
        }

        public int validateRepKat(string repKatID)
        {
            string sSql = " SELECT count(*) countRepKat "
                        + " FROM rep_kat "
                        + " where rep_kat_id = :rep_kat_id ";

            NxParameterCollection pc = new NxParameterCollection();
            pc.Add("rep_kat_id", repKatID);
            string errText = "";

            DataTable dt = cdb.getData(sSql, ref errText, pc);

            int errCode = -100;

            if (errText == "" && dt.Rows.Count == 0)
            {
                return -1;
            }

            if (dt.Rows.Count == 1)
            {
                int count = Convert.ToInt32(dt.Rows[0]["countRepKat"]);
                if (count == 1)
                    return 1;
                else
                    return -1;
            }

            return -1;
        }


        /// <summary>
        /// Get all available repKat for 
        /// timeregistration version 2
        /// </summary>
        /// <param name="ident"></param>
        /// <returns></returns>
        /// 2017-03-14 KJBO
        public List<RepKatCL> getRepKat(string ident)
        {

            int identOK = checkIdent(ident);
            List<RepKatCL> repList = new List<RepKatCL>();

            
            if (identOK == -1)
            {
                RepKatCL rep = new RepKatCL();
                rep.RepKatID = "";
                rep.RepKat = "";
                rep.ErrCode = -10;
                rep.ErrMessage = "Ogiltigt login";
                repList.Add(rep);
                return repList;
            }

            string sSql = " select rep_kat_id, rep_kat "
                        + " from rep_kat "
                        + " order by rep_kat ";

            string errSt = "";
            DataTable dt = cdb.getData(sSql, ref errSt);

            int errCode = -100;

            if (errSt == "" && dt.Rows.Count == 0)
            {                
                errSt = "Reparatörskategorier saknas";
                errCode = 0;
            }


            if (errSt != "")
            {
                RepKatCL rep = new RepKatCL();
                if (errSt.Length > 2000)
                    errSt = errSt.Substring(1, 2000);
                rep.ErrCode = errCode;
                rep.ErrMessage = errSt;
                repList.Add(rep);
                return repList;
            }

            foreach (DataRow dr in dt.Rows)
            {
                RepKatCL rep = new RepKatCL();
                rep.RepKatID = dr["rep_kat_id"].ToString();
                rep.RepKat = dr["rep_kat"].ToString();
                repList.Add(rep);
            }

            return repList;

        }

 
      

    }
}