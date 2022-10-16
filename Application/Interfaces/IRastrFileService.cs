using Domain.Rastrwin3;

namespace Application.Interfaces
{
    public interface IRastrFileService
    {
        public Task<List<RastrFile>> GetRastrFiles();
        public Task PostRastrFiles();
    }
}
