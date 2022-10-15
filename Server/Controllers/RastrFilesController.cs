using Microsoft.AspNetCore.Mvc;
using Data;
using Data.RastrModel;

namespace Server.Controllers
{
    //[ApiController]
    //[Route("[controller]")]
    public class RastrFilesController : ControllerBase
    {
        [Route("RastrFiles/GetRastrFiles")]
        [HttpGet]
        public List<RastrFile> GetRastrFiles()
        {
            List<RastrFile> rastrFiles = new();
            string[] rastrFilesPath = Directory.GetFiles(@"C:\Users\otrok\source\repos\Диплом_УР_Автоматизация\Server\RastrFiles");
            foreach (string file in rastrFilesPath)
            {
                FileInfo fileInfo = new(file);
                rastrFiles.Add(new RastrFile() { Name= Path.GetFileName(file), LastModified = fileInfo.LastWriteTime.ToUniversalTime()});
            }
            return rastrFiles;
        }

        [Route("RastrFiles/PostRastrFiles")]
        [HttpPost]
        public void PostRastrFiles()
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
        }
    }
}