using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers
{
    //[ApiController]
    //[Route("[controller]")]
    public class RastrSchemeInfoController : ControllerBase
    {
        private readonly IRastrSchemeInfoService _rastrSchemeInfoService;

        public RastrSchemeInfoController(IRastrSchemeInfoService rastrSchemeInfoService)
        {
            _rastrSchemeInfoService = rastrSchemeInfoService;
        }

        [Route("RastrSchemeInfo/GetRastrSchemeInfo")]
        [HttpGet]
        public async Task<IActionResult> GetDistricts()
        {
            return StatusCode(200, _rastrSchemeInfoService.GetRastrSchemeInfo());
        }
    }
}