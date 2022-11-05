using Domain.Rastrwin3.RastrModel;

namespace Infrastructure
{
    public interface ICalcModel
    {
        List<Node> AllLoadNodesToList();
        void ChangePn(List<int> nodes, int percent);
        List<District> DistrictList();
        int FindBranchIndex(int ip, int iq, int np);
        int FindNodeIndex(int ny);
        void RastrTestBalance();
        List<Sech> SechList();
        List<int> SkrmNodesToList();
        void WorseningRandom(List<int> nodes,  int percent);
    }
}