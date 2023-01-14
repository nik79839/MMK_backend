using Application.DTOs;
using Application.DTOs.InitialResult;
using Application.DTOs.ProcessedResult;
using Application.DTOs.Requests;
using Application.DTOs.Response;
using Application.Interfaces;
using AutoMapper;
using Domain;
using Domain.Events;
using Domain.InitialResult;
using Domain.ProcessedResult;
using Infrastructure.RabbitMQ;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Server.Hub;
using System.Security.Claims;
using System.Security.Principal;

namespace Server.Controllers
{
    /// <summary>
    /// Контроллер работы с расчетами
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CalculationsController : ControllerBase
    {
        private readonly IHubContext<ProgressHub> _hubContext;
        private readonly ICalculationService _calculationService;
        private readonly IProcessResultService _processResultService;
        private readonly IRabbitMQProducer _rabitMQProducer;
        private readonly IMapper _mapper;
        public CalculationsController(IHubContext<ProgressHub> hubContext, ICalculationService calculationService, 
            IMapper mapper, IProcessResultService processResultService, IRabbitMQProducer rabbitMQProducer)
        {
            _hubContext = hubContext;
            _mapper = mapper;
            _calculationService = calculationService;
            _calculationService.CalculationProgress += EventHandler;
            _processResultService = processResultService;
            _rabitMQProducer = rabbitMQProducer;
        }

        /// <summary>
        /// Начать расчет
        /// </summary>
        /// <param name="calculationSettings"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("PostCalculations")]
        public async Task<IActionResult> PostCalculations([FromBody]CalculationSettingsRequest calculationSettingsRequest)
        {
            _rabitMQProducer.SendProductMessage(calculationSettingsRequest);
            return Ok();
            /*CancellationToken cancellationToken = HttpContext.RequestAborted;
            int userId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier));
            var calculationSettings = _mapper.Map<CalculationSettingsRequest, CalculationSettings>(calculationSettingsRequest);
            calculationSettings.PathToRegim = @"C:\Users\otrok\Desktop\Файлы ворд\Диплом_УР\Дипломмаг\Мой\СБЭК_СХН.rg2";
            calculationSettings.PathToSech = @"C:\Users\otrok\Desktop\Файлы ворд\Диплом_УР\Дипломмаг\Мой\СБЭК_сечения.sch";
            try
            {
                await _calculationService.StartCalculation(calculationSettings, cancellationToken, userId);
                Console.WriteLine("Расчет завершен");
                return Ok("Расчет завершен.");
            }
            catch (Exception ex)
            {
                return Problem($"Ошибка при выполнении расчета: {ex.Message}");
            }*/
        }

        /// <summary>
        /// Получить описания всех расчетов
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

        //TODO: Изменить маппер
        /// <summary>
        /// Получить результаты определенного расчета
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetCalculations/{id}")]
        public async Task<IActionResult> GetCalculationsById(string? id)
        {
            try
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
                var response = new CalculationResultInfoResponse()
                {
                    PowerFlowResults = _mapper.Map<List<PowerFlowResult>, List<PowerFlowResultDto>>(powerFlowResults),
                    VoltageResults = _mapper.Map<List<VoltageResult>, List<VoltageResultDto>>(voltageResults),
                    CurrentResults = _mapper.Map<List<CurrentResult>, List<CurrentResultDto>>(currentResults),
                    PowerFlowResultProcessed = _mapper.Map<PowerFlowResultProcessed, PowerFlowResultProcessedDto>(_processResultService.Processing(powerFlowResults) as PowerFlowResultProcessed),
                    VoltageResultProcessed = _mapper.Map<List<VoltageResultProcessed>, List<VoltageResultProcessedDto>>(_processResultService.Processing(voltageResults) as List<VoltageResultProcessed>),
                    CurrentResultProcessed = _mapper.Map<List<CurrentResultProcessed>, List<CurrentResultProcessedDto>>(_processResultService.Processing(currentResults) as List<CurrentResultProcessed>)
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        /// <summary>
        /// Удалить определенный расчет
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
            Console.WriteLine(e.Percent + "%, осталось " + e.Time + " мин");
            _hubContext.Clients.All.SendAsync("SendProgress", e.Percent, e.CalculationId);
        }
    }
}