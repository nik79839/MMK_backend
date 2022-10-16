using BLL.Rastrwin3;

namespace BLL.Interfaces
{
    public interface IRastrSchemeInfoService
    {
        public Task<RastrSchemeInfo> GetRastrSchemeInfo();
    }
}
