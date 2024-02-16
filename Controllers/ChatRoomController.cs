using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace OntrackDb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatRoomController : ControllerBase
    {
        private readonly IWebHostEnvironment _hostingEnvironment;

        public ChatRoomController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet("GetHtml")]
        public IActionResult Index()
        {
            var path = Path.Combine(_hostingEnvironment.WebRootPath, "Content", "index.html");
            var fileStream = System.IO.File.OpenRead(path);
            return File(fileStream, "text/html");
        }
    }
}
