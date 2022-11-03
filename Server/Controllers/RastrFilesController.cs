using Domain.Rastrwin3;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers
{
    //[ApiController]
    //[Route("[controller]")]
    public class RastrFilesController : ControllerBase
    {
        [Route("RastrFiles/GetRastrFiles")]
        [HttpGet]
        public async Task<IActionResult> GetRastrFiles()
        {
            List<RastrFile> rastrFiles = new();
            string[] rastrFilesPath = Directory.GetFiles(@"C:\Users\otrok\source\repos\Диплом_УР_Автоматизация\Server\RastrFiles");
            foreach (string file in rastrFilesPath)
            {
                FileInfo fileInfo = new(file);
                rastrFiles.Add(new RastrFile() { Name= Path.GetFileName(file), LastModified = fileInfo.LastWriteTime.ToUniversalTime()});
            }
            return Ok(rastrFiles);
        }

        [Route("RastrFiles/PostRastrFiles")]
        [HttpPost]
        public async Task<IActionResult> PostRastrFiles()
        {
            IFormFileCollection files = Request.Form.Files;
            string uploadPath = @"C:\Users\otrok\source\repos\Диплом_УР_Автоматизация\Server\RastrFiles";
            foreach (var file in files)
            {
                string fullPath = $@"{uploadPath}/{file.FileName}";
                using (var fileStream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyToAsync(fileStream);
                }
            }
            return Ok();
        }
    }
}