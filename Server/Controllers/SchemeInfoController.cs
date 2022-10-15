using Microsoft.AspNetCore.Mvc;
using Data;
using Data.RastrModel;

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
            RastrManager rastrManager = new(@"C:\Users\otrok\Desktop\����� ����\������_��\���������\���\����_���.rg2");
            return rastrManager.DistrictList();
        }

        [Route("SchemeInfo/GetSech")]
        [HttpGet]
        public List<Sech> GetSech()
        {
            RastrManager rastrManager = new(@"C:\Users\otrok\Desktop\����� ����\������_��\���������\���\����_���.rg2",
                @"C:\Users\otrok\Desktop\����� ����\������_��\���������\���\����_�������.sch");
            return rastrManager.SechList();
        }

        [Route("SchemeInfo/GetLoadNodes")]
        [HttpGet]
        public List<Node> GetLoadNodes()
        {
            RastrManager rastrManager = new(@"C:\Users\otrok\Desktop\����� ����\������_��\���������\���\����_���.rg2");
            return rastrManager.AllLoadNodesToList();
        }
    }
}