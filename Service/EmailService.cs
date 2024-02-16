using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using OntrackDb.Assets;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.IO;
using System.Net.Http.Headers;
using System.Net;
using System.Threading.Tasks;
using OntrackDb.Authorization;
using OntrackDb.Repositories;
using OntrackDb.Authentication;
using OntrackDb.Entities;

namespace OntrackDb.Service
{
    public class EmailService:IEmailService
    {
        private IConfiguration _configuration;
        


        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
           
              
           
        }

        public async Task SendEmailAsync(string ToEmail, string subject,string content,string link)
        {
            var apiKey = _configuration["SendGridApikey"];
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(_configuration["SendingEmail"], _configuration["SupportTeam"]);
            var to = new EmailAddress(ToEmail);
            var msg = MailHelper.CreateSingleEmail(from,to,subject,content,link);
            var response = await client.SendEmailAsync(msg);
        
            Console.WriteLine(response);
        }

        public async Task SendReportsOnEmail(IFormFile file, string email)
        {

            String htmlText = EmailTemplates.GetReportsEmailTemplate(null);
            var apiKey = _configuration["SendGridApikey"];
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(_configuration["SendingEmail"], _configuration["SupportTeam"]);
            var to = new EmailAddress(email);
            var msg = MailHelper.CreateSingleEmail(from, to, "PillCoach Report", "Pharmacy Reports", htmlText);
            if (file.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    file.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    Attachment atts = new Attachment();
                    atts.Filename = file.FileName;
                    atts.Content = Convert.ToBase64String(fileBytes);
         
                    msg.AddAttachment(atts);
                }
            }
            var response = await client.SendEmailAsync(msg);

            Console.WriteLine(response);
        }

        public async Task SendCmrPdfEmail(TakeawayVerify takeawayVerify, string email ,int patientId)
        {

            string url = _configuration["VerifyUrl"] + "/" + patientId + "?uuid=" + takeawayVerify.UUID.ToString();
            String htmlText = EmailTemplates.GetCmrPdfEmailTemplate(url);

            var apiKey = _configuration["SendGridApikey"];
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(_configuration["SendingEmail"], _configuration["SupportTeam"]);
            var to = new EmailAddress(email);
            
            var msg = MailHelper.CreateSingleEmail(from, to, "Shared Document", "Patient Reports", htmlText);
            
            var response = await client.SendEmailAsync(msg);

            Console.WriteLine(response);
        }

        public async Task SendMedRecPdfEmail(TakeawayVerify takeawayVerify, string email, int patientId)
        {

            string url = _configuration["VerifyUrl"] + "/" + patientId + "?uuid=" + takeawayVerify.UUID.ToString() + "?flag=" + takeawayVerify.IsServiceTakeAwayMedRec;
            String htmlText = EmailTemplates.GetCmrPdfEmailTemplate(url);

            var apiKey = _configuration["SendGridApikey"];
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(_configuration["SendingEmail"], _configuration["SupportTeam"]);
            var to = new EmailAddress(email);

            var msg = MailHelper.CreateSingleEmail(from, to, "Shared Document", "Patient Reports", htmlText);

            var response = await client.SendEmailAsync(msg);

            Console.WriteLine(response);
        }

    }
}
