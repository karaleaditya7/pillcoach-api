using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using OntrackDb.Authentication;
using OntrackDb.Authorization;
using OntrackDb.Model;
using OntrackDb.Repositories;
using System;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace OntrackDb.Service
{
    public class ImageService : IImageService
    {
        private readonly BlobServiceClient _blobServiceClient;
    
        private readonly IJwtUtils _jwtUtils;
        
        public ImageService(BlobServiceClient blobServiceClient, IJwtUtils jwtUtils)
        {
            _blobServiceClient = blobServiceClient;
         
            _jwtUtils = jwtUtils;
           

        }

        public  string GetImageURI(string imageName)
        {
            if (imageName != null) { 
            var containerClient = _blobServiceClient.GetBlobContainerClient("images");
            if (containerClient.CanGenerateSasUri)
            {

                BlobSasBuilder sasBuilder = new BlobSasBuilder()
                {
                    BlobContainerName = containerClient.Name,
                    Resource = "b",

                };

                sasBuilder.StartsOn = DateTimeOffset.UtcNow;

                sasBuilder.ExpiresOn = DateTimeOffset.UtcNow.AddSeconds(100);
                sasBuilder.SetPermissions(BlobContainerSasPermissions.Read);


                Uri sasUri = containerClient.GenerateSasUri(sasBuilder);
                string sasUrl = sasUri.ToString();
                string [] arr = sasUrl.Split("?");
                string url = arr[0] + "/"+imageName+"?" + arr[1];
                Console.WriteLine("SAS URI for blob container is: {0}", url);
                Console.WriteLine();

                return url;
            }
            }
            else
            {
                Console.WriteLine(@"BlobContainerClient must be authorized with Shared Key 
                          credentials to create a service SAS.");
                return null;
            }
            return null;


        }

        public async Task<string> Upload(FileModel model)
        {

            OntrackDb.Dto.Response<Response<BlobContentInfo>> response = new OntrackDb.Dto.Response<Response<BlobContentInfo>>();
            var blobContainer = _blobServiceClient.GetBlobContainerClient("images");
            
           
            string imageName = String.Format("user-photo-{0}{1}", Guid.NewGuid().ToString(), Path.GetExtension(model.ImageFile.FileName));

            var blobClient = blobContainer.GetBlobClient(imageName);

            try
            {
                var result = await blobClient.UploadAsync(model.ImageFile.OpenReadStream());
            }
            catch (Exception ex)
            {
                return "Error Occured While Uplaoding Image :" + ex.Message;
            }

            return imageName;
            
        }

    }
}
