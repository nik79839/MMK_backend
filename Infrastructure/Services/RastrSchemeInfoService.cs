using Application.Interfaces;
using Domain.Rastrwin3;
using RastrAdapter;

namespace Infrastructure.Services
{
    public class RastrSchemeInfoService : IRastrSchemeInfoService
    {
        //TODO: pathTO
        public async Task<RastrSchemeInfo> GetRastrSchemeInfo()
        {
            RastrCOMClient rastrComClient = new(@"C:\Users\otrok\Desktop\Файлы ворд\Диплом_УР\Дипломмаг\Мой\СБЭК_СХН.rg2",
                @"C:\Users\otrok\Desktop\Файлы ворд\Диплом_УР\Дипломмаг\Мой\СБЭК_сечения.sch");
            return new(rastrComClient.AllLoadNodesToList(), rastrComClient.SechList(), rastrComClient.DistrictList());
        }
    }
}
