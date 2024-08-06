using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Configuration;
using System.Web.Configuration;
using System.Web;

namespace EmailServicesLib
{
    public class Services
    {
        public async Task<string> SendEmail(string emailFrom, string emailTo, string emailSubject, string emailBody)
        {
            string result = string.Empty;
            bool emailDelivered = false;
            int emailAttempts = 0;

            using (MailKit.Net.Smtp.SmtpClient smptClient = new MailKit.Net.Smtp.SmtpClient())
            {
                while (emailAttempts <= 3 && !emailDelivered)
                {
                    MimeMessage email = new MimeMessage();
                    Configuration configuration = null;
                    try
                    {
                        email.From.Add(new MailboxAddress("Eduardo Rodriguez", emailFrom));
                        email.To.Add(new MailboxAddress("Recipient", emailTo));
                        email.Subject = emailSubject;
                        email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                        {
                            Text = emailBody
                        };

                        if (HttpContext.Current != null)
                            configuration = WebConfigurationManager.OpenWebConfiguration("~");
                        else
                            configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                        SmtpSection section = (SmtpSection)configuration.GetSection("system.net/mailSettings/smtp");
                        smptClient.Connect(section.Network.Host, section.Network.Port, false);
                        smptClient.Authenticate(section.Network.UserName, section.Network.Password);
                        smptClient.Send(email);
                        smptClient.Disconnect(true);

                        emailDelivered = true;
                        result = "Email sent to " + email.To.ToString() + " with subject: " + email.Subject.ToString();  
                        LogEmailTransactions(email.From.ToString(), email.To.ToString(), email.Subject, emailBody, "EmailSent");
                    }
                    catch (Exception exception)
                    {
                        SmtpException smtpException = new SmtpException();
                        string exceptionmsg = smtpException.StatusCode.ToString() + " " + exception.Message;
                        LogEmailTransactions(emailFrom, emailTo, emailSubject, emailBody, exceptionmsg);
                        return exception.Message;
                    }
                    emailAttempts++;
                }
            }
            return result;
        }

        private void LogEmailTransactions(string emailSender, string emailRecipient, string emailSubject, string emailBody, string emailStatus)
        {
            string path = @"c:\EmailServices";
            try
            {
                //if EmailServices folder doesnt exist, create it 
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                //if EmailServicesLog file doesnt exist, create it 
                string datetimeStamp = DateTime.Today.ToString("MMddyyyy");
                string fileName = "EmailServicesLog" + datetimeStamp + ".txt";
                path = path + "\\" + fileName;

                if (!File.Exists(path))
                {
                    using (StreamWriter streamWriter = File.CreateText(path))
                    {
                        streamWriter.WriteLine("Email Sender, Email Recipient, EmailSubject, EmailBody, Email Status, SentDate");
                        streamWriter.Close();
                    }
                }

                using (StreamWriter streamReader =  new StreamWriter(path, true))
                {
                    streamReader.WriteLine(string.Format("{0}, {1}, {2}, {3}, {4}, {5}", emailSender, emailRecipient, emailSubject, emailBody, emailStatus, DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")));
                    streamReader.Close();
                }
            }
            catch (Exception exception)
            {
                using (StreamWriter streamReader = new StreamWriter(path, true))
                {
                    streamReader.WriteLine(string.Format("Error Ocurred in Writing Log file: {0}", exception));
                    streamReader.Close();
                }
            }
        }
    }
}
