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
            var calcResultInit = _calculationService.GetCalculationsById(id);
            List<PowerFlowResult> powerFlowResults = new();
            List<CurrentResult> currentResults = new();
            List<VoltageResult> voltageResults = new();
            foreach (var calc in calcResultInit)
            {
                switch (calc)
                {
                    case PowerFlowResult powerFlowResult:
                        powerFlowResults.Add(powerFlowResult);
                        break;
                    case CurrentResult calculationResultBase:
                        currentResults.Add(calculationResultBase);
                        break;
                    case VoltageResult voltageResult:
                        voltageResults.Add(voltageResult);
                        break;
                }
            }
            CalculationResultInitialDto calculationResultInitial = new()
            {
                PowerFlowResults = _mapper.Map<List<PowerFlowResult>, List<PowerFlowResultDto>>(powerFlowResults),
                VoltageResults = _mapper.Map<List<VoltageResult>, List<VoltageResultDto>>(voltageResults),
                CurrentResults = _mapper.Map<List<CurrentResult>, List<CurrentResultDto>>(currentResults)
            };

            CalculationResultProcessedDto calculationResultProcessed = new()
            {
                PowerFlowResultProcessed = _mapper.Map<PowerFlowResultProcessed, PowerFlowResultProcessedDto>(_processResultService.Processing(powerFlowResults) as PowerFlowResultProcessed),
                VoltageResultProcessed = _mapper.Map<List<VoltageResultProcessed>, List<VoltageResultProcessedDto>>(_processResultService.Processing(voltageResults) as List<VoltageResultProcessed>),
                CurrentResultProcessed = _mapper.Map<List<CurrentResultProcessed>, List<CurrentResultProcessedDto>>(_processResultService.Processing(currentResults) as List<CurrentResultProcessed>)
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