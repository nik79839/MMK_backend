using Domain.Rastrwin3;

namespace Application.Interfaces
{
    public interface IRastrSchemeInfoService
    {
        public Task<RastrSchemeInfo> GetRastrSchemeInfo();
    }
}
