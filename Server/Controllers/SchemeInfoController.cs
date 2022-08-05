using ASTRALib;
using Microsoft.AspNetCore.Mvc;
using Model;
using Model.RastrModel;

namespace Server.Controllers
{
    //[ApiController]
    //[Route("[controller]")]
    public class SchemeInfoController : ControllerBase
    {
        [Route("SchemeInfo/GetDistricts")]
        [HttpGet]
        public List<District> GetDistricts()
        {
            Rastr rastr = new Rastr();
            string PathToRegim = @"C:\Users\otrok\Desktop\Файлы ворд\Диплом_УР\Дипломмаг\Мой\СБЭК_СХН.rg2";
            rastr.Load(RG_KOD.RG_REPL, PathToRegim, PathToRegim);;
            return RastrManager.DistrictList(rastr);
        }

        [Route("SchemeInfo/GetSech")]
        [HttpGet]
        public List<Sech> GetSech()
        {
            Rastr rastr = new Rastr();
            string PathToRegim = @"C:\Users\otrok\Desktop\Файлы ворд\Диплом_УР\Дипломмаг\Мой\СБЭК_СХН.rg2";
            string PathToSech = @"C:\Users\otrok\Desktop\Файлы ворд\Диплом_УР\Дипломмаг\Мой\СБЭК_сечения.sch";
            rastr.Load(RG_KOD.RG_REPL, PathToRegim, PathToRegim);
            rastr.Load(RG_KOD.RG_REPL, PathToSech, PathToSech);
            return RastrManager.SechList(rastr);
        }
    }
}