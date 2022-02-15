using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Tolitech.CodeGenerator.Infrastructure.Services;

namespace Tolitech.CodeGenerator.Infrastructure.Tests
{
    public class EmailServiceTest
    {
        [Fact(DisplayName = "EmailService - GetTemplate - Valid")]
        public void EmailService_GetTemplate_Valid()
        {
            var logger = new Mock<ILogger>();
            var emailService = new EmailService(logger.Object);

            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Assets",
                "Test.html");

            string template = emailService.GetTemplate(filePath, "Tolitech");
            string expected = "<html><body><p>Hello, Tolitech.</p></body></html>";

            Assert.Equal(expected, template);
        }

        [Fact(DisplayName = "EmailService - Send - Valid")]
        public void EmailService_Send_Valid()
        {
            var logger = new Mock<ILogger>();
            var emailService = new EmailService(logger.Object);

            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Assets",
                "Test.html");

            string template = emailService.GetTemplate(filePath, "Tolitech");
            var attachments = new List<Attachment>();
            attachments.Add(new Attachment(filePath));

            emailService.Send("localhost", 587, "test", "test", "Test <test@test.com>", "test@test.com", "Test", template, "test@test.com", "test@test.com", attachments);
        }
    }
}
