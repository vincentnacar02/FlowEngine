using FlowEngine.SDK;
using FlowEngine.SDK.interfaces;
using FlowEngine.SDK.types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SMTPSendEmail
{
    /// <summary>
    /// SMTPSendEmail
    /// @author: Vincent Nacar
    /// </summary>
    public class SMTPSendEmail : Activity
    {
        public SMTPSendEmail(object Id, IProperties props)
            : base(Id, props)
        {

        }

        public override IResult run()
        {
            try
            {
                string MailFrom = this.getProperties().getProperty("MailFrom").getValue().ToString();
                string SmtpServer = this.getProperties().getProperty("SmtpServer").getValue().ToString();
                string SmtpUserName = this.getProperties().getProperty("SmtpUserName").getValue().ToString();
                string SmtpPassword = this.getProperties().getProperty("SmtpPassword").getValue().ToString();
                string SmtpEnableSsl = this.getProperties().getProperty("SmtpEnableSsl").getValue().ToString();
                string SmtpPort = this.getProperties().getProperty("SmtpPort").getValue().ToString();
                string MailTo = this.getProperties().getProperty("MailTo").getValue().ToString();
                string Subject = this.getProperties().getProperty("Subject").getValue().ToString();
                String Body = this.getProperties().getProperty("Body").getValue().ToString();

                int port = Convert.ToInt32(SmtpPort);

                using (SmtpClient smtpClient = new SmtpClient(SmtpServer, port))
                using (MailMessage mail = new MailMessage())
                {
                    mail.IsBodyHtml = true;

                    if (!string.IsNullOrEmpty(SmtpUserName) && !string.IsNullOrEmpty(SmtpPassword))
                        smtpClient.Credentials = new NetworkCredential(SmtpUserName, SmtpPassword);

                    mail.From = new MailAddress(MailFrom);
                    mail.To.Add(MailTo);

                    mail.Subject = Subject;
                    mail.Body = Body;

                    if (SmtpEnableSsl == "y" || SmtpEnableSsl.Equals("true"))
                        smtpClient.EnableSsl = true;
                    smtpClient.Send(mail);
                }
            }
            catch (Exception ex)
            {
                return new ActivityResult(null, ResultStatus.HAS_ERROR, ex);
            }
            return new ActivityResult(null, ResultStatus.SUCCESS);
        }

        public override void onInit()
        {

        }
    }

}
