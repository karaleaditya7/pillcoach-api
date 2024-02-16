using Microsoft.AspNetCore.Hosting;
using OntrackDb.Model;
using System;
using System.IO;
using System.Web;

namespace OntrackDb.Assets
{
    public static class EmailTemplates
    {
        private static string EmailTemplate;
        public static string EmailConfirmationTemplate;
        public static string EmailConfirmationDeclineTemplate;
        private static IWebHostEnvironment _hostingEnvironment;

        public static void Initialize(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        public static string GetForgotPasswordTemplate(string url)
        {
            EmailTemplate = File.ReadAllText(MapPath(@"Assets\Forgot_Password_Template.html"));
            EmailTemplate = EmailTemplate.Replace("REPLACE_URL_HERE", url);
            return EmailTemplate;
        }
        public static string GetInviteUserTemplate(string url)
        {
            EmailTemplate = File.ReadAllText(MapPath(@"Assets\Invite_User_Template.html"));
            EmailTemplate = EmailTemplate.Replace("REPLACE_URL_HERE", url);
            return EmailTemplate;
        }

        public static string GetReportsEmailTemplate(string url)
        {
            EmailTemplate = File.ReadAllText(MapPath(@"Assets\Reports_Email_Template.html"));

            return EmailTemplate;
        }

        public static string GetCmrPdfEmailTemplate(string url)
        {
            EmailConfirmationTemplate = File.ReadAllText(MapPath(@"Assets\Email_Pdf_Template.html"));
            EmailConfirmationTemplate = EmailConfirmationTemplate.Replace("REPLACE_URL_HERE", url);
            return EmailConfirmationTemplate;
        }

        public static string GetFaxTemplate(PdfModel model)
        {
            EmailTemplate = File.ReadAllText(MapPath(@"Assets\FaxTemplate.html"));
           
            return EmailTemplate;
        }

        public static string GetEmailConfirmationTemplate(string url)
        {
            EmailConfirmationTemplate = File.ReadAllText(MapPath(@"Assets\UserEmailConfirmation.html"));
            EmailConfirmationTemplate = EmailConfirmationTemplate.Replace("REPLACE_URL_HERE", url);
            return EmailConfirmationTemplate;
        }

        public static string GetEmailApprovedTemplate(string url)
        {
            EmailConfirmationTemplate = File.ReadAllText(MapPath(@"Assets\ApprovedEmailTemplate.html"));
            EmailConfirmationTemplate = EmailConfirmationTemplate.Replace("REPLACE_URL_HERE", url);
            return EmailConfirmationTemplate;
        }

        public static string GetEmailDeclinedTemplate(string url)
        {
            EmailConfirmationDeclineTemplate = File.ReadAllText(MapPath(@"Assets\DeclinedEmailTemplate.html"));
            EmailConfirmationDeclineTemplate = EmailConfirmationDeclineTemplate.Replace("REPLACE_URL_HERE", url);
            return EmailConfirmationDeclineTemplate;
        }

        public static string Get2FACodeTemplate(string verificationCode)
        {
            try
            {
                var template = File.ReadAllText(MapPath(@"Assets\2FA_Code_Template.html"));
                return template.Replace("VERIFICATION_CODE", verificationCode);
            }
            catch { }

            return null;
        }

        private static string MapPath(string path)
        {
            var basePath = _hostingEnvironment.ContentRootPath;
            return Path.Combine(basePath, path);
        }
    }
}
