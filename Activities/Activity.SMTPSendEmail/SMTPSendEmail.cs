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
        private string MailFrom;
        private string SmtpServer;
        private string SmtpUserName;
        private string SmtpPassword;
        private string SmtpEnableSsl;
        private string SmtpPort;
        private string MailTo;
        private string Subject;
        private string Body;

        public SMTPSendEmail(object Id, IProperties props)
            : base(Id, props)
        {

        }

        public override IResult run()
        {
            try
            {
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
            this.MailFrom = this.getProperties().getProperty("MailFrom").getValue().ToString();
            this.SmtpServer = this.getProperties().getProperty("SmtpServer").getValue().ToString();
            this.SmtpUserName = this.getProperties().getProperty("SmtpUserName").getValue().ToString();
            this.SmtpPassword = this.getProperties().getProperty("SmtpPassword").getValue().ToString();
            this.SmtpEnableSsl = this.getProperties().getProperty("SmtpEnableSsl").getValue().ToString();
            this.SmtpPort = this.getProperties().getProperty("SmtpPort").getValue().ToString();
            this.MailTo = this.getProperties().getProperty("MailTo").getValue().ToString();
            this.Subject = this.getProperties().getProperty("Subject").getValue().ToString();
            this.Body = this.getProperties().getProperty("Body").getValue().ToString();

            if (string.IsNullOrEmpty(this.MailFrom))
                throw new Exception("MailFrom must not be empty.");
            if (string.IsNullOrEmpty(this.SmtpServer))
                throw new Exception("SmtpServer must not be empty.");
            if (string.IsNullOrEmpty(this.SmtpUserName))
                throw new Exception("SmtpUsername must not be empty.");
            if (string.IsNullOrEmpty(this.SmtpPassword))
                throw new Exception("SmtpPassword must not be empty.");
            if (string.IsNullOrEmpty(this.SmtpPort))
                throw new Exception("SmtpPort must not be empty.");
            if (string.IsNullOrEmpty(this.MailTo))
                throw new Exception("SmtpPort must not be empty.");
        }
    }

}
