using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SManApi
{

    /// <summary>
    /// Functions available for 
    /// all other classes
    /// </summary>
    public class CCommonFunc
    {



        /// <summary>
        /// Add a % sign to the end of the string
        /// if the last character is not already %
        /// </summary>
        /// <param name="s"></param>
        /// <returns>The new string</returns>
        /// 2016-02-01 KJBO Pergas AB
        public static string addWildCard(string s)
        {
            if (s.Length > 0)
            {
                if (s.Substring(s.Length - 1) != "%")
                    s += "%";
            }
            else
                s = "%";
            return s;
        }

    }
}