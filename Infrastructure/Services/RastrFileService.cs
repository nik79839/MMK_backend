using Application.Interfaces;
using Domain.Rastrwin3;

namespace Infrastructure.Services
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
                rastrFiles.Add(new RastrFile(Path.GetFileName(file), fileInfo.LastWriteTime.ToUniversalTime()));
            }
            return rastrFiles;
        }

        public Task PostRastrFiles()
        {
            throw new NotImplementedException();
        }
    }
}
