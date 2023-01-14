using Domain;
using Domain.InitialResult;
using Domain.Rastrwin3.RastrModel;

namespace Application.Interfaces
{
    public interface ICalcModel
    {
        List<Brunch> AllLapBrunchesToList();
        List<Node> AllLoadNodesToList();
        List<Node> AllNodesToList();
        void ChangePn(List<int> nodes, int percent);
        void CreateInstanceRastr(string pathToRegim, string? pathToSech = null);
        List<District> DistrictList();
        int FindBranchIndex(int ip, int iq, int np);
        int FindBranchIndexByName(string name);
        int FindNodeIndex(int ny);
        T GetParameterByIndex<T>(string table, string column, int index);
        void RastrTestBalance();
        List<Sech> SechList();
        void WorseningRandom(List<WorseningSettings> nodes, int percent);
        List<VoltageResult> GetVoltageResults(Guid id, int implementation, List<int> uNodes);
        List<CurrentResult> GetCurrentResults(Guid id, int implementation, List<string> iBrunches)
    }
}