using BLL.Interfaces;
using BLL.Rastrwin3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class RastrFileService : IRastrFileService
    {
        public async Task<List<RastrFile>> GetRastrFiles()
        {
            List<RastrFile> rastrFiles = new();
            string[] rastrFilesPath = Directory.GetFiles(@"C:\Users\otrok\source\repos\Диплом_УР_Автоматизация\Server\RastrFiles");
            foreach (string file in rastrFilesPath)
            {
                FileInfo fileInfo = new(file);
                rastrFiles.Add(new RastrFile() { Name = Path.GetFileName(file), LastModified = fileInfo.LastWriteTime.ToUniversalTime() });
            }
            return rastrFiles;
        }

        public Task PostRastrFiles()
        {
            throw new NotImplementedException();
        }
    }
}
