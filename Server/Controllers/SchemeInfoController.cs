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
            string PathToRegim = @"C:\Users\otrok\Desktop\����� ����\������_��\���������\���\����_���.rg2";
            string PathToSech = @"C:\Users\otrok\Desktop\����� ����\������_��\���������\���\����_�������.sch";
            rastr.Load(RG_KOD.RG_REPL, PathToRegim, PathToRegim);
            rastr.Load(RG_KOD.RG_REPL, PathToSech, PathToSech);
            ITable sch = (ITable)rastr.Tables.Item("sechen");
            return RastrManager.DistrictList(rastr);
        }
    }
}