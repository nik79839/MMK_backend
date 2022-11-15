using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers
{
    /// <summary>
    /// Контроллер для получения информации о режиме из RastrWin3
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class RastrSchemeInfoController : ControllerBase
    {
        private readonly IRastrSchemeInfoService _rastrSchemeInfoService;

        public RastrSchemeInfoController(IRastrSchemeInfoService rastrSchemeInfoService)
        {
            _rastrSchemeInfoService = rastrSchemeInfoService;
        }

        /// <summary>
        /// Получение информации о списке нагрузочных узлов, сечений и районов
        /// </summary>
        /// <returns></returns>
        [Route("GetRastrSchemeInfo")]
        [HttpGet]
        public async Task<IActionResult> GetRastrSchemeInfo()
        {
            return Ok(_rastrSchemeInfoService.GetRastrSchemeInfo().Result);
        }
    }
}