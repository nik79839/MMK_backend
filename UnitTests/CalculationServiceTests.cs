using Application.Interfaces;
using Domain;
using Domain.InitialResult;
using Infrastructure.Services;
using Moq;
using RastrAdapter;

namespace UnitTests
{
    public class CalculationServiceTests
    {
        private readonly Mock<ICalculationResultRepository> _calculationRepositoryMock;
        private readonly Mock<ICalcModel> _rastrMock;

        public CalculationServiceTests()
        {
            _calculationRepositoryMock = new Mock<ICalculationResultRepository>();
            _rastrMock = new Mock<ICalcModel>();
        }

        [Fact]
        public async Task StartCalculation_ShouldReturnTrue()
        {
            //arange

            CalculationSettings calculationSettings = new()
            {
                Name = "testName",
                Description = "testDescription",
                PathToRegim = @"C:\Users\otrok\Desktop\Файлы ворд\Диплом_УР\Дипломмаг\Мой\СБЭК_СХН.rg2",
                PercentLoad = 10,
                PercentForWorsening = 15,
                CountOfImplementations = 2,
                SechNumber = 2,
                WorseningSettings = new() { new(Guid.NewGuid(), 60408134,null), new(Guid.NewGuid(), 60408133, null),new(Guid.NewGuid(), 60405013, null),
                    new(Guid.NewGuid(), 60405029,null),new(Guid.NewGuid(), 60405049,null),new(Guid.NewGuid(), 60405014,null),new(Guid.NewGuid(), 60405020, null),
                    new(Guid.NewGuid(), 60405012,null),new(Guid.NewGuid(), 1800,null), new(Guid.NewGuid(), 1655, null),new(Guid.NewGuid(), 1654, null),},
                PathToSech = @"C:\Users\otrok\Desktop\Файлы ворд\Диплом_УР\Дипломмаг\Мой\СБЭК_сечения.sch",
                UNodes = new() { 2658 },
                IBrunches = new() { "ПС 220 кВ Улькан - ПС 220 кВ Дабан" }
            };
            RastrCOMClient rastrCOMClient = new(calculationSettings.PathToRegim, calculationSettings.PathToSech);
            var service = new CalculationService(_calculationRepositoryMock.Object, rastrCOMClient);

            //act
            var result = service.StartCalculation(calculationSettings, new CancellationToken(false));

            //assert
            Assert.True(result.IsCompletedSuccessfully);
        }

        [Fact]
        public async Task GetCalculations_ShouldGetCalculations()
        {
            _calculationRepositoryMock
                .Setup(x => x.GetCalculations())
                .Returns(() => Task.FromResult(new List<Calculations>() { new Calculations() { Id = Guid.NewGuid(), Name = "test", SechName = "afag" } }))
                .Verifiable();
            var service = new CalculationService(_calculationRepositoryMock.Object, _rastrMock.Object);
            var result = service.GetCalculations();

            _calculationRepositoryMock.Verify(x => x.GetCalculations(), Times.Once);
            Assert.True(result.Count > 0);
        }

        [Fact]
        public async Task GetCalculationsByid_ShouldGetCalculationResult()
        {
            _calculationRepositoryMock
                .Setup(x => x.GetResultInitialById("testId"))
                .Returns(() => Task.FromResult(new List<CalculationResultBase>() { new PowerFlowResult(Guid.NewGuid(),1,123),
                    new PowerFlowResult(Guid.NewGuid(),2,155), new VoltageResult(Guid.NewGuid(),1,123,"name",125)} as IEnumerable<CalculationResultBase>))
                .Verifiable();
            var service = new CalculationService(_calculationRepositoryMock.Object, _rastrMock.Object);

            var result = service.GetCalculationResultById("testId");

            _calculationRepositoryMock.Verify(x => x.GetResultInitialById("testId"), Times.Once);
            Assert.True(result.ToList().Count > 0);
        }

        [Fact]
        public async Task DeleteCalculation_ShouldReturnTrue()
        {
            _calculationRepositoryMock
                .Setup(x => x.DeleteCalculationsById("test"));
            var service = new CalculationService(_calculationRepositoryMock.Object, _rastrMock.Object);
            var result = service.DeleteCalculationById("test");

            _calculationRepositoryMock.Verify(x => x.DeleteCalculationsById("test"), Times.Once);
            Assert.True(result.IsCompletedSuccessfully);
        }
    }
}
