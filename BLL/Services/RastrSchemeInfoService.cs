using BLL.Interfaces;
using BLL.Rastrwin3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class RastrSchemeInfoService : IRastrSchemeInfoService
    {
        //TODO: pathTO
        public async Task<RastrSchemeInfo> GetRastrSchemeInfo()
        {
            RastrProvider rastrManager = new(@"C:\Users\otrok\Desktop\Файлы ворд\Диплом_УР\Дипломмаг\Мой\СБЭК_СХН.rg2",
                @"C:\Users\otrok\Desktop\Файлы ворд\Диплом_УР\Дипломмаг\Мой\СБЭК_сечения.sch");
            RastrSchemeInfo rastrSchemeInfo = new(rastrManager.AllLoadNodesToList(),rastrManager.SechList(),rastrManager.DistrictList());
            return rastrSchemeInfo;
        }
    }
}
