using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace OntrackDb.Service
{
    public interface IPdfService
    {
        Task<byte[]> EncryptionAndDecryptionOfPDF(IFormFile form ,  int patientId);
    }
}
