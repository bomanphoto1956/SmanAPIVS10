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

    }
}