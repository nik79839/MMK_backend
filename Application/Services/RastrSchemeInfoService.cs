using Application.Interfaces;
using Domain.Rastrwin3;

namespace Infrastructure.Services
{
    public class RastrSchemeInfoService : IRastrSchemeInfoService
    {
        private readonly ICalcModel _rastrClient;

        public RastrSchemeInfoService(ICalcModel rastrClient)
        {
            _rastrClient = rastrClient;
        }

        //TODO: pathTO
        public async Task<RastrSchemeInfo> GetRastrSchemeInfo()
        {
            _rastrClient.CreateInstanceRastr(@"C:\Users\otrok\Desktop\Файлы ворд\Диплом_УР\Дипломмаг\Мой\СБЭК_СХН.rg2",
                @"C:\Users\otrok\Desktop\Файлы ворд\Диплом_УР\Дипломмаг\Мой\СБЭК_сечения.sch");
            return new(_rastrClient.AllNodesToList().Where(x => x.Pn !=0).ToList(), _rastrClient.SechList(), 
                _rastrClient.DistrictList(), _rastrClient.AllNodesToList(), _rastrClient.AllBrunchesToList().Where(x => x.Type == 0).ToList());
        }
    }
}
