using System;
using System.Collections.Concurrent;

namespace Veritrade2018.Helpers
{
    public static class EmailUtils
    {
        public const string ServidorEmail = "smtp.gmail.com";
        public const int ServidorEmailPuerto = 587;
        public const string EmailEnvio = "verinews@veritrade-ltd.com";
        public const string EmailEnvioNombre = "Informativo VERINEWS";

        public static string EnviaEmail3(string FromName, string FromEmail, string ToName, string ToEmail, string CcName, string CcEmail, string BccName, string BccEmail, string Subject, string URL, string EmailEnvio1, string EmailEnvioPassword1)
        {
            string Observacion = "";
            try
            {
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