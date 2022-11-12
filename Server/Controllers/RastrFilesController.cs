using Domain.Rastrwin3;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers
{
    /// <summary>
    /// ���������� ��� ������ � ������� RastrWin3
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class RastrFilesController : ControllerBase
    {
        /// <summary>
        /// �������� ���������� � ������ ������ �� �������
        /// </summary>
        /// <returns></returns>
        [Route("GetRastrFiles")]
        [HttpGet]
        public async Task<IActionResult> GetRastrFiles()
        {
            List<RastrFile> rastrFiles = new();
            string[] rastrFilesPath = Directory.GetFiles(@"C:\Users\otrok\source\repos\������_��_�������������\Server\RastrFiles");
            foreach (string file in rastrFilesPath)
            {
                FileInfo fileInfo = new(file);
                rastrFiles.Add(new RastrFile(Path.GetFileName(file), fileInfo.LastWriteTime.ToUniversalTime()));
            }
            return Ok(rastrFiles);
        }

        /// <summary>
        /// ��������� ���� ������ �� ������
        /// </summary>
        /// <returns></returns>
        [Route("PostRastrFiles")]
        [HttpPost]
        public async Task<IActionResult> PostRastrFiles()
        {
            IFormFileCollection files = Request.Form.Files;
            string uploadPath = @"C:\Users\otrok\source\repos\������_��_�������������\Server\RastrFiles";
            foreach (var file in files)
            {
                string fullPath = $@"{uploadPath}/{file.FileName}";
                using var fileStream = new FileStream(fullPath, FileMode.Create);
                await file.CopyToAsync(fileStream);
            }
            return Ok();
        }
    }
}