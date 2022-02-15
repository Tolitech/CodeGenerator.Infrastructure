using System;
using System.ComponentModel;
using System.Net;
using System.Net.Mail;
using System.Text;
using Microsoft.Extensions.Logging;
using Tolitech.CodeGenerator.Domain.Services;

namespace Tolitech.CodeGenerator.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger _logger;

        public EmailService(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Get template.
        /// </summary>
        /// <param name="filePath">file path</param>
        /// <param name="parameters">parameters</param>
        /// <returns>template</returns>
        public string GetTemplate(string filePath, params string[] parameters)
        {
            string template = "";

            if (File.Exists(filePath))
            {
                template = File.ReadAllText(filePath, Encoding.Default);

                int index = 0;
                foreach (string p in parameters)
                {
                    template = template.Replace("{" + index + "}", p);
                    index++;
                }
            }

            return template;
        }

        /// <summary>
        /// Send email.
        /// </summary>
        /// <param name="host">host</param>
        /// <param name="port">port</param>
        /// <param name="username">username</param>
        /// <param name="password">password</param>
        /// <param name="from">from</param>
        /// <param name="to">to</param>
        /// <param name="subject">subject</param>
        /// <param name="body">body</param>
        /// <param name="cc">cc</param>
        /// <param name="bcc">bcc</param>
        /// <param name="attachments">attachments</param>
        public void Send(string host, int port, string username, string password, string from, string to, string subject, string body, string? cc = null, string? bcc = null, IList<Attachment>? attachments = null)
        {
            var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(username, password)
            };

            client.SendCompleted += new SendCompletedEventHandler(Client_SendCompleted);

            var message = new MailMessage
            {
                From = new MailAddress(from)
            };

            AddTo(to, message);
            AddCC(cc, message);
            AddBCC(bcc, message);
            AddAttachments(attachments, message);

            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;

            client.SendAsync(message, null);
        }

        private static void AddTo(string? to, MailMessage message)
        {
            string[] emails = Split(to);

            foreach (string email in emails)
            {
                message.To.Add(email);
            }
        }

        private static void AddCC(string? cc, MailMessage message)
        {
            string[] emails = Split(cc);

            foreach (string email in emails)
            {
                message.CC.Add(email);
            }
        }

        private static void AddBCC(string? bcc, MailMessage message)
        {
            string[] emails = Split(bcc);

            foreach (string email in emails)
            {
                message.Bcc.Add(email);
            }
        }

        private static string[] Split(string? emails)
        {
            IEnumerable<string> list = new List<string>();

            if (!string.IsNullOrEmpty(emails))
            {
                emails = emails.Replace(",", ";");
                list = emails.Split(';').AsEnumerable<string>();
            }

            return list.ToArray();
        }

        private static void AddAttachments(IList<Attachment>? attachments, MailMessage message)
        {
            if (attachments != null)
            {
                foreach (Attachment attachment in attachments)
                {
                    message.Attachments.Add(attachment);
                }
            }
        }

        private void Client_SendCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                if (e.Error != null)
                {
                    _logger.LogError(e.Error.ToString());

                    Console.WriteLine(e.Error.ToString());
                }
            }
        }
    }
}