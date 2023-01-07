using Infrastructure;
using Infrastructure.Persistance.Entities;
using Infrastructure.Persistance.Entities.Result;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests.Database
{
    public class DatabaseTest
    {
        private readonly CalculationResultContext _context;

        public DatabaseTest()
        {
            var serviceProvider = new ServiceCollection().AddEntityFrameworkNpgsql().BuildServiceProvider();
            var builder = new DbContextOptionsBuilder<CalculationResultContext>();
            builder.UseNpgsql("Host=localhost;Port=5432;Database=VKRTest;Username=postgres;Password=N7983915031;").
                UseInternalServiceProvider(serviceProvider);
            _context = new CalculationResultContext(builder.Options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        }

        [Fact]
        public async Task Calculations_ShouldAddAndGetEntity()
        {
            _context.Calculations.Add(new CalculationEntity() { Name = "test", PercentLoad = 10, PercentForWorsening = 10, CalculationStart = "10.12.2022" });
            _context.SaveChanges();
            var calculationsEntity = _context.Calculations.ToList();
            Assert.Equal("test", calculationsEntity[0].Name);
        }

        [Fact]
        public async Task PowerFlowResults_ShouldAddAndGetEntity()
        {
            Guid uid = Guid.NewGuid();
            _context.Calculations.Add(new CalculationEntity() {Id = uid, Name = "test", PercentLoad = 10, PercentForWorsening = 10,
                CalculationStart = "10.12.2022" });
            _context.PowerFlowResults.Add(new PowerFlowResultEntity() { CalculationId = uid, ImplementationId = 1, Value = 100});
            _context.SaveChanges();
            var powerFlowResultEntity = _context.PowerFlowResults.ToList();
            Assert.Equal(100, powerFlowResultEntity[0].Value);
        }

        [Fact]
        public async Task WorseningSettings_ShouldAddAndGetEntity()
        {
            Guid uid = Guid.NewGuid();
            _context.Calculations.Add(new CalculationEntity() { Id = uid, Name = "test", PercentLoad = 10, PercentForWorsening = 10,
                CalculationStart = "10.12.2022" });
            _context.WorseningSettings.Add(new WorseningSettingsEntity() { CalculationId = uid, NodeNumber = 2658, MaxValue = 250});
            _context.SaveChanges();
            var worseningSettingsEntity = _context.WorseningSettings.ToList();
            Assert.Equal(2658, worseningSettingsEntity[0].NodeNumber);
        }

        [Fact]
        public async Task Users_ShouldAddAndGetEntity()
        {
            _context.Users.Add(new UserEntity() { Id = 1, Login = "admin",Password = "admin" ,Name = "name", SurName = "surname", 
                LastName = "lastname", Post = "post" });
            _context.SaveChanges();
            var userEntity = _context.Users.ToList();
            Assert.Equal("admin", userEntity[0].Login);
        }
    }
}
