using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace SManApi
{
    public class CReadSettings
    {
                
        public static string getLogDir()
        {
            return ConfigurationManager.AppSettings["logDir"].ToString();
        }
        public static bool getLogEnabled()
        {
            return (ConfigurationManager.AppSettings["logEnabled"].ToString() == "1");
        }



    }
}