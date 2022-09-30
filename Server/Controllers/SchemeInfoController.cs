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
            string pathToRegim = @"C:\Users\otrok\Desktop\Файлы ворд\Диплом_УР\Дипломмаг\Мой\СБЭК_СХН.rg2";
            return RastrManager.DistrictList(pathToRegim);
        }

        [Route("SchemeInfo/GetSech")]
        [HttpGet]
        public List<Sech> GetSech()
        {
            string pathToRegim = @"C:\Users\otrok\Desktop\Файлы ворд\Диплом_УР\Дипломмаг\Мой\СБЭК_СХН.rg2";
            return RastrManager.SechList(pathToRegim);
        }

        [Route("SchemeInfo/GetLoadNodes")]
        [HttpGet]
        public List<Node> GetLoadNodes()
        {
            string pathToRegim = @"C:\Users\otrok\Desktop\Файлы ворд\Диплом_УР\Дипломмаг\Мой\СБЭК_СХН.rg2";
            return RastrManager.AllLoadNodesToList(pathToRegim);
        }
    }
}