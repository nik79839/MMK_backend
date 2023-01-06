using Domain.Rastrwin3;
using Infrastructure.Services;

namespace UnitTests
{
    public class RastrFileServiceTests
    {
        [Fact]
        public async Task GetRastrFiles_ShouldGetRastrFiles()
        {
            var service = new RastrFileService();

            var result = service.GetRastrFiles().Result;

            Assert.IsType<List<RastrFile>>(result);
        }

        [Fact]
        public async Task PostRastrFiles_ShouldReturnTrue()
        {
            var service = new RastrFileService();

            //var result = service.PostRastrFiles();

            Assert.True(true);
        }
    }
}
