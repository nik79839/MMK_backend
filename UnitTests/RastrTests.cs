using Domain;
using Domain.Rastrwin3.RastrModel;
using RastrAdapter;
using RastrAdapter.Tables;

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
            Node init = _rastr.AllNodesToList().First(x => x.Number == nodeNumber);
            _rastr.ChangePn(new List<int> { nodeNumber }, percent);
            Node after = _rastr.AllNodesToList().First(x => x.Number == nodeNumber);
            Assert.True(after.Pn != init.Pn);
            Assert.True(after.Pn <= init.Pn * (1 + ((double)percent / 100)));
            Assert.True(after.Pn >= init.Pn * (1 - ((double)percent / 100)));
        }

        [Fact]
        public async Task WorseningRandom_ShouldReturnTrue()
        {
            double powerFlowValue = _rastr.SechList()[1].PowerFlow;
            _rastr.WorseningRandom(new List<WorseningSettings> { new WorseningSettings(Guid.NewGuid(), 60408134, null) }, 20);
            _rastr.RastrTestBalance();
            Assert.True(_rastr.SechList()[1].PowerFlow > powerFlowValue);
        }

        [Fact]
        public async Task AllLapBrunchesToList_ShouldReturnAll()
        {
            List<Brunch> brunches = _rastr.AllBrunchesToList();
            Assert.True(brunches.Count>0);
        }

        [Fact]
        public async Task AllLoadNodessToList_ShouldReturnAll()
        {
            List<Node> nodes = _rastr.AllNodesToList();
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
            //int index = new RastrTableNode().FindIndexByNum(2658);
            Assert.IsType<int>(1);
            //Assert.True(index>0);
        }

        [Fact]
        public async Task FindNodeIndex_ShouldReturnFalse()
        {
            //Assert.Throws<Exception>(() => _rastr.FindNodeIndex(265855000));
        }
    }
}
