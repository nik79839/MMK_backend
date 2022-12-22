using Application.DTOs;
using Application.DTOs.InitialResult;
using Application.DTOs.ProcessedResult;
using Application.DTOs.Response;
using Application.Interfaces;
using AutoMapper;
using Domain;
using Domain.Events;
using Domain.InitialResult;
using Domain.ProcessedResult;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Server.Hub;
using System.Security.Claims;
using System.Security.Principal;

namespace Server.Controllers
{
    /// <summary>
    /// ���������� ������ � ���������
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CalculationsController : ControllerBase
    {
        private readonly IHubContext<ProgressHub> _hubContext;
        private readonly ICalculationService _calculationService;
        private readonly IProcessResultService _processResultService;
        private readonly IMapper _mapper;
        public CalculationsController(IHubContext<ProgressHub> hubContext, ICalculationService calculationService, IMapper mapper, IProcessResultService processResultService)
        {
            _hubContext = hubContext;
            _mapper = mapper;
            _calculationService = calculationService;
            _calculationService.CalculationProgress += EventHandler;
            _processResultService = processResultService;
        }

        /// <summary>
        /// ������ ������
        /// </summary>
        /// <param name="calculationSettings"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("PostCalculations")]
        public async Task<IActionResult> PostCalculations([FromBody]CalculationSettingsRequest calculationSettingsRequest)
        {
            CancellationToken cancellationToken = HttpContext.RequestAborted;
            int userId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier));
            var calculationSettings = _mapper.Map<CalculationSettingsRequest, CalculationSettings>(calculationSettingsRequest);
            calculationSettings.PathToRegim = @"C:\Users\otrok\Desktop\����� ����\������_��\���������\���\����_���.rg2";
            calculationSettings.PathToSech = @"C:\Users\otrok\Desktop\����� ����\������_��\���������\���\����_�������.sch";
            await _calculationService.StartCalculation(calculationSettings, cancellationToken, userId);
            Console.WriteLine("������ ��������");
            return Ok("������ ��������.");
        }

        /// <summary>
        /// �������� �������� ���� ��������
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetCalculations")]
        public async Task<IActionResult> GetCalculations()
        {
            var calculations = _calculationService.GetCalculations();
            var response = new GetCalculationsResponse
            {
                CalculationAmount = calculations.Count,
                Calculations = _mapper.Map<List<Calculations>, List<CalculationDto>>(calculations)
            };
            return Ok(response);
        }

        //TODO: �������� ������
        /// <summary>
        /// �������� ���������� ������������� �������
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetCalculations/{id}")]
        public async Task<IActionResult> GetCalculationsById(string? id)
        {
            var calculationResultInfo = _calculationService.GetCalculationsById(id);
            CalculationResultInitialDto calculationResultInitial = new()
            {
                PowerFlowResults = _mapper.Map<List<CalculationResultBase>, List<PowerFlowResultDto>>(calculationResultInfo.PowerFlowResults),
                VoltageResults = _mapper.Map<List<VoltageResult>, List<VoltageResultDto>>(calculationResultInfo.VoltageResults),
                CurrentResults = _mapper.Map<List<CurrentResult>, List<CurrentResultDto>>(calculationResultInfo.CurrentResults)
            };
            CalculationResultProcessedDto calculationResultProcessed = new()
            {
                PowerFlowResultProcessed = _mapper.Map<StatisticBase, StatisticBaseDto>(_processResultService.Processing(calculationResultInfo.PowerFlowResults)),
                VoltageResultProcessed = _mapper.Map<List<VoltageResultProcessed>, List<VoltageResultProcessedDto>>(_processResultService.Processing(calculationResultInfo.VoltageResults) as List<VoltageResultProcessed>),
                CurrentResultProcessed = _mapper.Map<List<CurrentResultProcessed>, List<CurrentResultProcessedDto>>(_processResultService.Processing(calculationResultInfo.CurrentResults) as List<CurrentResultProcessed>)
            };
            IEnumerable<StatisticBase> statisticBases = _processResultService.Processing(calculationResultInfo.VoltageResults);
            var response = new CalculationResultInfoResponse(calculationResultInitial, calculationResultProcessed);
            return Ok(response);
        }

        /// <summary>
        /// ������� ������������ ������
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("DeleteCalculations/{id}")]
        public async Task<IActionResult> DeleteCalculationsById(string? id)
        {
            await _calculationService.DeleteCalculationById(id);
            return Ok();
        }

        [NonAction]
        public void EventHandler(object sender, CalculationProgressEventArgs e)
        {
            Console.WriteLine(e.Percent + "%, �������� " + e.Time + " ���");
            _hubContext.Clients.All.SendAsync("SendProgress", e.Percent, e.CalculationId);
        }
    }
}