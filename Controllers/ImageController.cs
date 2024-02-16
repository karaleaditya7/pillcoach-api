
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OntrackDb.Authentication;
using OntrackDb.Dto;
using OntrackDb.Model;
using OntrackDb.Service;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace OntrackDb.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;
        private readonly IUserService _userService;

        public ImageController(IImageService imageService,IUserService userService)
        {
            _imageService = imageService;
            _userService = userService;
        }


        /// <summary>
        /// Uploading a new image . Role[Admin,Employee]
        /// </summary>
        /// <remarks>
        ///  This API Will be used for uplaod a image.
        /// </remarks>

        [Route("upload")]
        [HttpPost]
        public async Task<IActionResult> Upload([FromForm] FileModel model, [FromHeader] string authorization)
        {
            OntrackDb.Dto.Response<Response<BlobContentInfo>> response = new OntrackDb.Dto.Response<Response<BlobContentInfo>>();
            if (model.ImageFile != null)
            {
                var imageName = await _imageService.Upload(model);
            
                var result = await _userService.UpdateUserImageName(imageName.ToString(), authorization);
                return Ok(result);
            }
            response.Success = false;
            response.Message = "Image Not Found";
            return Ok(response);
        }


        /// <summary>
        /// Get a Image Uri . Role[Admin,Employee]
        /// </summary>
        /// <remarks>
        ///  This API Will be used for Getting a DelegationSasBlob.
        /// </remarks>
        [Route("get")]
        [HttpGet]
        public string GetUserDelegationSasBlob([FromQuery]string imageName)
        {

           var result = _imageService.GetImageURI(imageName);
         
            return result;
        }
    }
}
