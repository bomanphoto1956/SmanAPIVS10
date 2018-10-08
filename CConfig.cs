using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace SManApi
{
    public class CConfig
    {

        public static string getCS()
        {
            string cs = ConfigurationManager.ConnectionStrings["ffServManCS"].ConnectionString;

            return cs;
        }

        public static int sendToPyramid
        {
            get
            {                
                return Convert.ToInt16(ConfigurationManager.AppSettings["sendToPyramid"]);
            }
        }

        public static int orderStartNumber
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["orderStartNumber"]);
            }
        }

        public static int devLogLevel
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["devLogLevel"]);
            }
        }

        public static DateTime startDateForCoList
        {
            get
            {
                return Convert.ToDateTime(ConfigurationManager.AppSettings["startDateForCoList"]);
            }
        }
        

    }
}