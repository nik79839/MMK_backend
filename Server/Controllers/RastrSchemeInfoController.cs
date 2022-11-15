using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers
{
    /// <summary>
    /// ���������� ��� ��������� ���������� � ������ �� RastrWin3
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
        /// ��������� ���������� � ������ ����������� �����, ������� � �������
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