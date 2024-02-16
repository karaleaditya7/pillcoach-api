using DinkToPdf;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OntrackDb.Model;
using System.Threading.Tasks;
using RestSharp;
#nullable enable
namespace OntrackDb.Helper
{
    public interface IUtilityFax
    {
        
        Task<string?> SendFax(PdfModel model, string filePath);
        Task<string?> GetFaxDescriptionByIds(PdfModel model);

    }
}
