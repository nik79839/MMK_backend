using Application.Interfaces;
using Domain;
using Infrastructure.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.Services
{
    public class CalculationServiceTest
    {
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
                UNodes = new() {2658 },
                IBrunches = new() { "Улькан - Дабан"}
            };
            var calculationRepositoryMock = new Mock<ICalculationResultRepository>();
            var service = new CalculationService(calculationRepositoryMock.Object);

            //act

            var result = service.StartCalculation(calculationSettings, new CancellationToken(false));

            //assert

            Assert.True(result.IsCompletedSuccessfully);
        }

        [Fact]
        public async Task GetCalculations_ShouldReturnTrue()
        {
            var calculationRepositoryMock = new Mock<ICalculationResultRepository>();
            calculationRepositoryMock
                .Setup(x => x.GetCalculations())
                .Returns(() => Task.FromResult(new List<Calculations>() { new Calculations() { Id = Guid.NewGuid(),Name="test",SechName="afag"} }))
                .Verifiable();

            var service = new CalculationService(calculationRepositoryMock.Object);
            var result = service.GetCalculations();

            calculationRepositoryMock.VerifyAll();
            //Assert.True(result.IsCompletedSuccessfully);
        }

        /*[Fact]
        public async Task GetCalculationsByid_ShouldReturnTrue()
        {
            var calculationRepositoryMock = new Mock<ICalculationResultRepository>();
            calculationRepositoryMock
                .Setup(x => x.GetResultInitialById("testId"))
                .Returns(() => Task.FromResult(new List<Calculations>() { new Calculations() { Id = Guid.NewGuid(), Name = "test", SechName = "afag" } }))
                .Verifiable();

            var service = new CalculationService(calculationRepositoryMock.Object);
            var result = service.GetCalculations();

            calculationRepositoryMock.VerifyAll();
            //Assert.True(result.IsCompletedSuccessfully);
        }*/
    }
}
