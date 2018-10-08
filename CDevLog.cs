using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NexusDB.ADOProvider;

namespace SManApi
{
    
    public class CDevLog
    {
        CDB cdb = null;
        public CDevLog()
        {
            cdb = new CDB();
        }

        public void logMessage(int logLevel, string message, string messageDescr = "")
        {

            if (logLevel <= CConfig.devLogLevel)
            {
                string sSql = " insert into devLog (logDate, logLevel, message, messageDescr) "
                        + " values(:logDate, :logLevel, :message, :messageDescr) ";

                NxParameterCollection pc = new NxParameterCollection();
                pc.Add("logDate", DateTime.Now);
                pc.Add("logLevel", logLevel);
                pc.Add("message", message);
                pc.Add("messageDescr", messageDescr);

                string err = "";
                cdb.updateData(sSql, ref err, pc);
            }

        }

    }
}