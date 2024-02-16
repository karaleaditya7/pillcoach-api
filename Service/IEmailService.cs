using Microsoft.AspNetCore.Http;
using OntrackDb.Authentication;
using OntrackDb.Entities;
using System.Threading.Tasks;

namespace OntrackDb.Service
{
    public interface IEmailService
    {
       Task SendEmailAsync(string ToEmail,string subject,string content,string link);
       Task SendReportsOnEmail(IFormFile file,string email);

        Task SendCmrPdfEmail(TakeawayVerify takeawayVerify, string email, int patientId);
        Task SendMedRecPdfEmail(TakeawayVerify takeawayVerify, string email, int patientId);
    }
}
