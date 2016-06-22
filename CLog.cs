using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace SManApi
{
    public class CLog
    {


        private string getLogDir()
        {
            string logDir = CReadSettings.getLogDir();
            string dir = HttpContext.Current.Server.MapPath(logDir);                            
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);                
            return dir;
        }


        private string getLogFileName()
        {
            return "log" + System.DateTime.Today.ToString("yyyyMMdd") + ".log";
        }

        public void log(string logMsg, string errCode)
        {
            if (!CReadSettings.getLogEnabled())
                return;
            if (errCode == "1")
                errCode += " OK ";
            else if (errCode == "0")
                errCode += " None ";
            else
                errCode += "Error";
            string fileName = Path.Combine(getLogDir(), getLogFileName());
            string msg = System.DateTime.Now.ToString() + "\t" + logMsg + "\t" + errCode + "\r\n";
            File.AppendAllText(fileName, msg);                        
        }


    }
}