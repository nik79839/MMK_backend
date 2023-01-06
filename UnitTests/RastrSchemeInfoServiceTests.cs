using Application.Interfaces;
using Domain;
using Domain.InitialResult;
using Domain.Rastrwin3;
using Infrastructure.Services;
using Moq;
using RastrAdapter;

namespace UnitTests
{
    public class RastrSchemeInfoServiceTests
    {
        [Fact]
        public async Task GetSchemeInfo_ShouldGetRastrSchemeInfo()
        {
            RastrCOMClient rastrCOMClient = new(@"C:\Users\otrok\Desktop\Файлы ворд\Диплом_УР\Дипломмаг\Мой\СБЭК_СХН.rg2",
                @"C:\Users\otrok\Desktop\Файлы ворд\Диплом_УР\Дипломмаг\Мой\СБЭК_сечения.sch");
            var service = new RastrSchemeInfoService(rastrCOMClient);

            var result = service.GetRastrSchemeInfo().Result;

            Assert.IsType<RastrSchemeInfo>(result);
        }
    }
}
