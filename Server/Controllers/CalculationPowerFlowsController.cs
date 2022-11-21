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
    /// Контроллер работы с расчетами
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
        /// Начать расчет
        /// </summary>
        /// <param name="calculationSettings"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("PostCalculations")]
        public async Task<IActionResult> PostCalculations([FromBody]CalculationSettingsRequest calculationSettingsRequest)
        {
            CancellationToken cancellationToken = HttpContext.RequestAborted;
            var calculationSettings = _mapper.Map<CalculationSettingsRequest, CalculationSettings>(calculationSettingsRequest);
            calculationSettings.PathToRegim = @"C:\Users\otrok\Desktop\Файлы ворд\Диплом_УР\Дипломмаг\Мой\СБЭК_СХН.rg2";
            calculationSettings.PathToSech = @"C:\Users\otrok\Desktop\Файлы ворд\Диплом_УР\Дипломмаг\Мой\СБЭК_сечения.sch"; ;
            await _calculationService.StartCalculation(calculationSettings, cancellationToken);
            Console.WriteLine("Расчет завершен");
            return Ok("Расчет завершен.");
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