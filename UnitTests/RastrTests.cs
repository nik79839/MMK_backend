using Domain.Rastrwin3;
using Infrastructure.Services;
using RastrAdapter;

namespace UnitTests
{
    public class RastrTests
    {
        private readonly RastrCOMClient _rastr;

        public RastrTests()
        {
            _rastr = new();
            _rastr.CreateInstanceRastr(@"C:\Users\otrok\Desktop\Файлы ворд\Диплом_УР\Дипломмаг\Мой\СБЭК_СХН.rg2",
                @"C:\Users\otrok\Desktop\Файлы ворд\Диплом_УР\Дипломмаг\Мой\СБЭК_сечения.sch");
        }
    }
}
