using System;
using System.Collections.Concurrent;
using System.Configuration;
using System.Net.Configuration;

namespace Veritrade2017.Helpers
{
    public static class EmailUtils
    {
        //public static string EnviaEmail(string FromName, string FromEmail, string ToName, string ToEmail, string CcName, string CcEmail, string BccName, string BccEmail, string Subject,             string body, string EmailEnvio1, string EmailEnvioPassword1)
        public static void EnviaEmail(object parameters)
        {
            var parameterArray = parameters as object[];
            if (parameterArray != null)
            {
                string Observacion = "";
                var FromName = (string)parameterArray[0];
                var FromEmail = (string)parameterArray[1];
                var ToName = (string)parameterArray[2];
                var ToEmail = (string)parameterArray[3];
                var CcName = (string)parameterArray[4];
                var CcEmail = (string)parameterArray[5];
                var BccName = (string)parameterArray[6];
                var BccEmail = (string)parameterArray[7];
                var Subject = (string)parameterArray[8];
                var body = (string)parameterArray[9];
                var EmailEnvio1 = (string)parameterArray[10];
                var EmailEnvioPassword1 = (string)parameterArray[11];
                try
                {
                    SmtpSection smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
                    string EmailEnvio = smtpSection.Network.UserName;
                    string EmailEnvioPassword = smtpSection.Network.Password;
                    string ServidorEmail = smtpSection.Network.Host;
                    int ServidorEmailPuerto = smtpSection.Network.Port;
                    System.Net.Mail.MailAddress From, To, Cc, Bcc;

                    From = new System.Net.Mail.MailAddress(FromEmail, FromName);
                    To = new System.Net.Mail.MailAddress(ToEmail, ToName);

                    //(1) Create the MailMessage instance
                    System.Net.Mail.MailMessage mm;
                    mm = new System.Net.Mail.MailMessage(From, To);

                    if (CcName != null)
                    {
                        Cc = new System.Net.Mail.MailAddress(CcEmail, CcName);
                        mm.CC.Add(Cc);
                    }

                    if (BccName != null)
                    {
                        Bcc = new System.Net.Mail.MailAddress(BccEmail, BccName);
                        mm.Bcc.Add(Bcc);
                    }

                    mm.IsBodyHtml = true;

                    //(2) Assign the MailMessage's properties
                    mm.Subject = Subject;

                    mm.Body = body;

                    //(3) Create the SmtpClient object
                    System.Net.Mail.SmtpClient smtp;
                    smtp = new System.Net.Mail.SmtpClient(ServidorEmail);
                    smtp.Port = ServidorEmailPuerto;
                    //smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.EnableSsl = true;
                    smtp.UseDefaultCredentials = false;
                    //smtp.Credentials = new System.Net.NetworkCredential(EmailEnvio1, EmailEnvioPassword1);
                    smtp.Credentials = new System.Net.NetworkCredential(EmailEnvio, EmailEnvioPassword);

                    //(4) Send the MailMessage (will use the Web.config settings)
                    smtp.Send(mm);
                }
                catch (Exception ex)
                {
                    ExceptionUtility.LogException(ex, "EnviaMail");
                }
            }
            
        }

        public static string EnviaEmail3(string FromName, string FromEmail, string ToName, string ToEmail, string CcName, string CcEmail, string BccName, string BccEmail, string Subject, string URL, string EmailEnvio1, string EmailEnvioPassword1)
        {
            string Observacion = "";
            try
            {
                SmtpSection smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
                string EmailEnvio = smtpSection.Network.UserName;
                string EmailEnvioPassword = smtpSection.Network.Password;
                string ServidorEmail = smtpSection.Network.Host;
                int ServidorEmailPuerto = smtpSection.Network.Port;
                System.Net.Mail.MailAddress From, To, Cc, Bcc;

                From = new System.Net.Mail.MailAddress(FromEmail, FromName);
                To = new System.Net.Mail.MailAddress(ToEmail, ToName);

                //(1) Create the MailMessage instance
                System.Net.Mail.MailMessage mm;
                mm = new System.Net.Mail.MailMessage(From, To);

                if (CcName != null)
                {
                    Cc = new System.Net.Mail.MailAddress(CcEmail, CcName);
                    mm.CC.Add(Cc);
                }

                if (BccName != null)
                {
                    Bcc = new System.Net.Mail.MailAddress(BccEmail, BccName);
                    mm.Bcc.Add(Bcc);
                }

                mm.IsBodyHtml = true;

                //(2) Assign the MailMessage's properties
                mm.Subject = Subject;
                // Converting URL to HTML
                System.Net.WebRequest objRequest = System.Net.HttpWebRequest.Create(URL);
                System.IO.StreamReader sr = new System.IO.StreamReader(objRequest.GetResponse().GetResponseStream());
                string html = sr.ReadToEnd();
                sr.Close();

                mm.Body = html;

                //(3) Create the SmtpClient object
                System.Net.Mail.SmtpClient smtp;
                smtp = new System.Net.Mail.SmtpClient(ServidorEmail);
                smtp.Port = ServidorEmailPuerto;
                //smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential(EmailEnvio1, EmailEnvioPassword1);

                //(4) Send the MailMessage (will use the Web.config settings)
                smtp.Send(mm);
            }
            catch (Exception ex)
            {
                Observacion = ex.Message;
            }
            return Observacion;
        }
    }
}