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
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Server.Hub;

namespace Server.Controllers
{
    /// <summary>
    /// ���������� ������ � ���������
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CalculationPowerFlowsController : ControllerBase
    {
        private readonly IHubContext<ProgressHub> _hubContext;
        private readonly ICalculationService _calculationService;
        private readonly IMapper _mapper;
        public CalculationPowerFlowsController(IHubContext<ProgressHub> hubContext, ICalculationService calculationService, IMapper mapper)
        {
            _hubContext = hubContext;
            _mapper = mapper;
            _calculationService = calculationService;
            _calculationService.CalculationProgress += EventHandler;
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
            var calculationSettings = _mapper.Map<CalculationSettingsRequest, CalculationSettings>(calculationSettingsRequest);
            calculationSettings.PathToRegim = @"C:\Users\otrok\Desktop\����� ����\������_��\���������\���\����_���.rg2";
            calculationSettings.PathToSech = @"C:\Users\otrok\Desktop\����� ����\������_��\���������\���\����_�������.sch"; ;
            await _calculationService.StartCalculation(calculationSettings, cancellationToken);
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
                PowerFlowResults = calculationResultInfo.InitialResult.PowerFlowResults.Select(x => x.PowerFlowLimit).ToList(),
                VoltageResults = _mapper.Map<List<VoltageResult>, List<VoltageResultDto>>(calculationResultInfo.InitialResult.VoltageResults)
            };
            CalculationResultProcessedDto calculationResultProcessed = new()
            {
                PowerFlowResultProcessed = _mapper.Map<StatisticBase, StatisticBaseDto>(calculationResultInfo.ProcessedResult.PowerFlowResultProcessed),
                VoltageResultProcessed = _mapper.Map<List<VoltageResultProcessed>, List<VoltageResultProcessedDto>>(calculationResultInfo.ProcessedResult.VoltageResultProcessed)
            };
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