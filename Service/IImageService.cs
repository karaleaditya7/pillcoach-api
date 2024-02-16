using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using OntrackDb.Model;
using System;
using System.Threading.Tasks;

namespace OntrackDb.Service
{
    public interface IImageService
    {
        Task<string> Upload(FileModel model);
        string GetImageURI(string imageName);
    }
}
