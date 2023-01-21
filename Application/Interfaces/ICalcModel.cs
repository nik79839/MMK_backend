using Domain;
using Domain.Enums;
using Domain.InitialResult;
using Domain.Rastrwin3.RastrModel;

namespace Application.Interfaces
{
    public interface ICalcModel
    {
        List<Brunch> AllBrunchesToList();
        List<Node> AllNodesToList();
        void ChangePn(List<int> nodes, int percent);
        void CreateInstanceRastr(string pathToRegim, string? pathToSech = null);
        List<District> DistrictList();
        void RastrTestBalance();
        List<Sech> SechList();
        void WorseningRandom(List<WorseningSettings> nodes, int percent);
        IEnumerable<CalculationResultBase> GetResults<T>(List<T> parameters, Guid id, int implementation, ParamType paramType);
    }
}