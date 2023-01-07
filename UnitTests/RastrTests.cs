using Domain;
using Domain.Rastrwin3.RastrModel;
using RastrAdapter;

namespace UnitTests
{
    public class RastrTests
    {
        private readonly RastrCOMClient _rastr;

        public RastrTests()
        {
            _rastr = new();
            _rastr.CreateInstanceRastr(@"C:\Users\otrok\Desktop\Файлы ворд\Диплом_УР\Дипломмаг\Мой\СБЭК_СХН.rg2",
                @"C:\Users\otrok\Desktop\Файлы ворд\Диплом_УР\Дипломмаг\Мой\СБЭК_сечения.sch");
        }

        [Fact]
        public async Task ChangePn_ShouldReturnTrue()
        {
            int nodeNumber = 1655;
            int percent = 10;
            int index = _rastr.FindNodeIndex(nodeNumber);
            double initValue = _rastr.GetParameterByIndex<double>("node", "pn", index);
            _rastr.ChangePn(new List<int> { nodeNumber }, percent);
            Assert.True(_rastr.GetParameterByIndex<double>("node", "pn", index) != initValue);
            Assert.True(_rastr.GetParameterByIndex<double>("node", "pn", index) <= initValue * (1 + ((double)percent / 100)));
            Assert.True(_rastr.GetParameterByIndex<double>("node", "pn", index) >= initValue * (1 - ((double)percent / 100)));
        }

        [Fact]
        public async Task WorseningRandom_ShouldReturnTrue()
        {
            double powerFlowValue = _rastr.GetParameterByIndex<double>("sechen", "psech", 1);
            _rastr.WorseningRandom(new List<WorseningSettings> { new WorseningSettings(Guid.NewGuid(), 60408134, null) }, 20);
            _rastr.RastrTestBalance();
            Assert.True(_rastr.GetParameterByIndex<double>("sechen", "psech", 1) > powerFlowValue);
        }

        [Fact]
        public async Task AllLapBrunchesToList_ShouldReturnAll()
        {
            List<Brunch> brunches = _rastr.AllLapBrunchesToList();
            Assert.True(brunches.Count>0);
        }

        [Fact]
        public async Task AllLoadNodessToList_ShouldReturnAll()
        {
            List<Node> nodes = _rastr.AllLoadNodesToList();
            Assert.True(nodes.Count > 0);
        }

        [Fact]
        public async Task DistrictList_ShouldReturnAll()
        {
            List<District> districts = _rastr.DistrictList();
            Assert.True(districts.Count > 0);
        }

        [Fact]
        public async Task FindNodeIndex_ShouldReturnTrue()
        {
            int index = _rastr.FindNodeIndex(2658);
            Assert.IsType<int>(index);
            Assert.True(index>0);
        }

        [Fact]
        public async Task FindNodeIndex_ShouldReturnFalse()
        {
            Assert.Throws<Exception>(() => _rastr.FindNodeIndex(265855000));
        }
    }
}
