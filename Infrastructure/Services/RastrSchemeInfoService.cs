using Application.Interfaces;
using Domain.Rastrwin3;

namespace Infrastructure.Services
{
    public class RastrSchemeInfoService : IRastrSchemeInfoService
    {
        //TODO: pathTO
        public async Task<RastrSchemeInfo> GetRastrSchemeInfo()
        {
            RastrProvider rastrManager = new(@"C:\Users\otrok\Desktop\Файлы ворд\Диплом_УР\Дипломмаг\Мой\СБЭК_СХН.rg2",
                @"C:\Users\otrok\Desktop\Файлы ворд\Диплом_УР\Дипломмаг\Мой\СБЭК_сечения.sch");
            RastrSchemeInfo rastrSchemeInfo = new(rastrManager.AllLoadNodesToList(), rastrManager.SechList(), rastrManager.DistrictList());
            return rastrSchemeInfo;
        }
    }
}
