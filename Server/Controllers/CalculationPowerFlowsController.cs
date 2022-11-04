using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain;
using Domain.Events;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Server.Hub;

namespace Server.Controllers
{
    /// <summary>
    /// ���������� ������ � ���������
    /// </summary>
    [ApiController]
    [Route("[controller]")]
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
        }

        /// <summary>
        /// ������ ������
        /// </summary>
        /// <param name="calculationSettings"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("PostCalculations")]
        public async Task<IActionResult> PostCalculations([FromBody]CalculationSettings calculationSettings)
        {
            CancellationToken cancellationToken = HttpContext.RequestAborted;
            calculationSettings.PathToRegim = @"C:\Users\otrok\Desktop\����� ����\������_��\���������\���\����_���.rg2";
            calculationSettings.PathToSech = @"C:\Users\otrok\Desktop\����� ����\������_��\���������\���\����_�������.sch"; ;
            Calculations calculations = new() { Name = calculationSettings.Name, Description = calculationSettings.Description,
                CalculationEnd = null, PathToRegim = calculationSettings.PathToRegim, PercentLoad = calculationSettings.PercentLoad,
                PercentForWorsening = calculationSettings.PercentForWorsening
            };
            _calculationService.CalculationProgress += EventHandler;
            await _calculationService.StartCalculation(calculations, calculationSettings, cancellationToken);
            Console.WriteLine("������ ��������");
            return Ok($"������ ��������.");
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
            return Ok(calculationResultInfo);
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