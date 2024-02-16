using Microsoft.AspNetCore.Http;
using OntrackDb.Model;
using System.Threading.Tasks;

namespace OntrackDb.Service
{
    public interface IPdfStorageService
    {
        Task<string> UploadPDFForCmr(PdfStorageModel model);

        Task<byte[]> GetPDFURIForCmr(int id);

        Task<string> GetPDFUrlByPatientIdForCmr(int id);
       // Task<string> GetPDFURIForCmrPdf(int id);
      //  Task<string> GetPDFURIForMedRec(int id);

        Task<string> UploadPDFForMedRec(PdfStorageModel model);

        Task<byte[]> GetPDFURIForMedRec(int id);
    }
}
