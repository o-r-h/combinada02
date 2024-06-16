using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Hosting;

namespace Veritrade2017.Helpers
{
    public sealed class ExceptionUtility
    {
        // All methods are static, so this can be private
        private ExceptionUtility()
        { }

        // Log an Exception
        public static void LogException(Exception exc, string source, Dictionary<string, string> parametros = null)
        {
            //Format source
            int index = source.IndexOf(">");
            if (index > 0)
            {
                source = source.Replace("+<", ".");
                source = source.Substring(0, source.IndexOf(">"));
            }
            // Include logic for logging exceptions
            // Get the absolute path to the log file
            string logFile = "~/App_Data/ErrorLog-" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
            logFile = HostingEnvironment.MapPath(logFile);

            if (logFile != null) { 
                // Open the log file for append and write the log
                StreamWriter sw = new StreamWriter(logFile, true);
                sw.WriteLine("********** {0} **********", DateTime.Now);
                if (exc.InnerException != null)
                {
                    sw.Write("Inner Exception Type: ");
                    sw.WriteLine(exc.InnerException.GetType().ToString());
                    sw.Write("Inner Exception: ");
                    sw.WriteLine(exc.InnerException.Message);
                    sw.Write("Inner Source: ");
                    sw.WriteLine(exc.InnerException.Source);
                    if (exc.InnerException.StackTrace != null)
                    {
                        sw.WriteLine("Inner Stack Trace: ");
                        sw.WriteLine(exc.InnerException.StackTrace);
                    }
                }
                sw.Write("Exception Type: ");
                sw.WriteLine(exc.GetType().ToString());
                sw.WriteLine("Exception: " + exc.Message);
                sw.WriteLine("Source: " + source);
                sw.WriteLine("Stack Trace: ");
                if (exc.StackTrace != null)
                {
                    sw.WriteLine(exc.StackTrace);
                    sw.WriteLine();
                }
                sw.WriteLine("********** Datos del Cliente **********");
                sw.WriteLine();
                if(parametros != null)
                {
                    foreach (KeyValuePair<string, string> pair in parametros)
                    {
                        sw.WriteLine("{0}: {1}", pair.Key, pair.Value);
                    }
                }
            
                sw.WriteLine();
                sw.Close();
            }
        }

        // Log an Result
        public static void LogSignUp(Dictionary<string, string> parametros)
        {
            // Include logic for logging exceptions
            // Get the absolute path to the log file
            string logFile = "~/App_Data/SignUp-" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
            logFile = HttpContext.Current.Server.MapPath(logFile);

            // Open the log file for append and write the log
            StreamWriter sw = new StreamWriter(logFile, true);
            sw.WriteLine("********** {0} **********", DateTime.Now);
            sw.WriteLine();
            foreach (KeyValuePair<string, string> pair in parametros)
            {
                sw.WriteLine("{0}: {1}", pair.Key, pair.Value);
            }
            sw.WriteLine();
            sw.Close();
        }
    }
}