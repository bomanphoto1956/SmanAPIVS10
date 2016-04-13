using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using System.Collections.Specialized;
using System.Data;

namespace SManApi
{
    public class CStdText
    {

        CDB cdb = null;

        public CStdText()
        {
            cdb = new CDB();
        }


        private string getKatName(int katID)
        {
            switch (katID)
            {
                case 1: return "Allmän";                    
                case 2: return "Kontroll före service";                    
                case 3: return "Utfört servicearbete";                    
                case 4: return "Övr. anm.";
                case 5: return "Reservdelar";                    
                case 6: return "Notering";                    
                case 7: return "Anmärkning";                    
            }
            return "";
        }



        /// <summary>
        /// Returns all standardtext
        /// </summary>
        /// <param name="ident"></param>
        /// <returns></returns>
        public List<StandardTextCL> getAllSttText(string ident)
        {
            CReparator cr = new CReparator();
            int identOK = cr.checkIdent(ident);

            List<StandardTextCL> stList = new List<StandardTextCL>();
            if (identOK == -1)
            {
                StandardTextCL st = new StandardTextCL();
                st.ErrCode = -10;
                st.ErrMessage = "Ogiltigt login";
                st.StdTextID = "";
                st.Text = "";
                st.Kategori = 0;
                st.KategoriBeskr = "";                                
                st.ventilkatID = 0;
                stList.Add(st);
                return stList;
            }

            string sSql = " SELECT stdtext_id, \"text\", kategori, ventilkategori "
                        + " FROM standardtext ";

            string errSt = "";
            DataTable dt = cdb.getData(sSql, ref errSt);

            if (errSt != "")
            {
                if (errSt.Length > 2000)
                    errSt = errSt.Substring(1, 2000);

                StandardTextCL st = new StandardTextCL();
                st.StdTextID = "";
                st.Text = "";
                st.Kategori = 0;
                st.KategoriBeskr = "";
                st.ventilkatID = 0;
                st.ErrCode = -100;
                st.ErrMessage = "Databasfel : " + errSt;
                stList.Add(st);
                return stList;
            }

            foreach (DataRow dr in dt.Rows)
            {
                int kategori = Convert.ToInt32(dr["kategori"]); 
                StandardTextCL st = new StandardTextCL();
                st.StdTextID = dr["stdtext_id"].ToString();
                st.Text = dr["text"].ToString();
                st.Kategori = kategori;
                st.KategoriBeskr = getKatName(kategori);
                st.ventilkatID = 0;
                if (dr["ventilkategori"] == DBNull.Value)
                    st.ventilkatID = 0;
                else
                    st.ventilkatID = Convert.ToInt32(dr["ventilkategori"]);
                st.ErrCode = 0;
                st.ErrMessage = "";
                stList.Add(st);
            }

            return stList;

        }

    }
}