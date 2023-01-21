using Application.Interfaces;
using ASTRALib;
using Domain;
using Domain.Rastrwin3.RastrModel;
using Domain.InitialResult;
using Domain.Enums;
using RastrAdapter.Tables;

namespace RastrAdapter
{
    public class RastrCOMClient : ICalcModel
    {
        private Rastr _rastr = new();
        private RastrTableBase<Sech> _sechTable;
        private RastrTableNode _nodeTable;
        private RastrTableBase<Brunch> _vetvTable;
        private RastrTableBase<District> _areaTable;

        public RastrCOMClient(string pathToRegim, string? pathToSech = null)
        {
            _rastr.Load(RG_KOD.RG_REPL, pathToRegim, pathToRegim);
        }
        public RastrCOMClient()
        {

        }

        public void CreateInstanceRastr(string pathToRegim, string? pathToSech = null)
        {
            _rastr.Load(RG_KOD.RG_REPL, pathToRegim, pathToRegim);
            _nodeTable = new RastrTableNode((ITable)_rastr.Tables.Item("node"));
            _vetvTable = new RastrTableVetv((ITable)_rastr.Tables.Item("vetv"));
            _areaTable = new RastrTableArea((ITable)_rastr.Tables.Item("area"));
            if (!string.IsNullOrEmpty(pathToSech))
            {
                _rastr.Load(RG_KOD.RG_REPL, pathToSech, pathToSech);
                _sechTable = new RastrTableSech((ITable)_rastr.Tables.Item("sechen"));
            }
            RastrTestBalance();
        }

        /// <summary>
        /// Получение списка всех узлов
        /// </summary>
        /// <param name="rastr"></param>
        public List<Node> AllNodesToList() => _nodeTable.ToList();

        public List<Brunch> AllBrunchesToList() => _vetvTable.ToList();

        /// <summary>
        /// Возвращает список районов
        /// </summary>
        /// <param name="rastr"></param>
        /// <returns></returns>
        public List<District> DistrictList() => _areaTable.ToList();

        /// <summary>
        /// Возвращает список сечений
        /// </summary>
        /// <param name="rastr"></param>
        /// <returns></returns>
        public List<Sech> SechList() => _sechTable.ToList();

        /// <summary>
        /// Случайная нагрузка для каждой реализации
        /// </summary>
        /// <param name="rastr"></param>
        /// <param name="nodes">Список узлов с нагрузкой</param>
        /// <param name="tgValues">Списко cos а для узлов нагрузки</param>
        /// <param name="percent">Количество процентов по обе стороны от номинального значения</param>
        public void ChangePn(List<int> nodes, int percent)
        {
            Random randPn = new();
            Random randTg = new();
            foreach (int node in nodes)
            {
                int index = _nodeTable.FindIndexByNum(node);
                float randomPercent = randPn.Next(100 - percent, 100 + percent) / 100;
                _nodeTable.Pn.Set(index, _nodeTable.Pn[index] * randomPercent);
                _nodeTable.Qn.Set(index, _nodeTable.Pn[index] * ((randTg.NextDouble() * 0.14) + 0.48));
            }
        }

        /// <summary>
        /// Случайное утяжеление, утяжеляет нагрузки на случайный процент до расхождения режима
        /// </summary>
        /// <param name="rastr"></param>
        /// <param name="nodes">Узлы утяжеления</param>
        /// <param name="tgvalues">Список cos f, генерируется в другом методе случайным образом</param>
        /// <param name="unode">Список напряжений </param>
        /// <param name="percent">Процент приращения</param>
        public void WorseningRandom(List<WorseningSettings> nodes, int percent)
        {
            Random randPercent = new();
            Random randTg = new();
            RastrRetCode kod = _rastr.rgm("p");
            if (kod == 0)
            {
                do // Основное утяжеление
                {
                    foreach (var node in nodes)
                    {
                        int index = _nodeTable.FindIndexByNum(node.NodeNumber);
                        float randomPercent = 1 + ((float)randPercent.Next(0, percent) / 100);
                        node.MaxValue ??= 10000;
                        if (_nodeTable.Pn[index] < node.MaxValue)
                        {
                            _nodeTable.Pn.Set(index, _nodeTable.Pn[index] * randomPercent);
                            _nodeTable.Qn.Set(index, _nodeTable.Pn[index] * ((randTg.NextDouble() * 0.14) + 0.48));
                        }
                    }
                    kod = _rastr.rgm("p");
                }
                while (kod == 0);
                while (kod != 0) // Откат на последний сходяийся режим
                {
                    foreach (var node in nodes)
                    {
                        int index = _nodeTable.FindIndexByNum(node.NodeNumber);
                        _nodeTable.Pn.Set(index, _nodeTable.Pn[index] / 1.02);
                        _nodeTable.Qn.Set(index, _nodeTable.Pn[index] * ((randTg.NextDouble() * 0.14) + 0.48));
                    }
                    kod = _rastr.rgm("p");
                }
            }
        }

        public void RastrTestBalance()
        {
            if (_rastr.rgm("p") == RastrRetCode.AST_NB)
            {
                throw new Exception("Итерация не завершена из-за несходимости режима.");
            }
        }

        public IEnumerable<CalculationResultBase> GetResults<T>(List<T> parameters, Guid id, int implementation, ParamType paramType)
        {
            return paramType switch
            {
                ParamType.CURRENT => (from string brunch in parameters // Запись токов
                                      let vetv = _vetvTable.ToList().First(x => x.Name == brunch)
                                      select new CurrentResult(id, implementation + 1, brunch, vetv.Current)).ToList(),
                ParamType.VOLTAGE => (from int uNode in parameters
                                      let node = _nodeTable.ToList().First(x => x.Number == uNode)
                                      select new VoltageResult(id, implementation + 1, uNode, node.Name, node.Voltage)).ToList(),
                _ => new List<CalculationResultBase>(),
            };
        }
    }
}
