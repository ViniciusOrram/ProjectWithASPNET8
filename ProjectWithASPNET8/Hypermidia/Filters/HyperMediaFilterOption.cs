using ProjectWithASPNET8.Hypermidia.Abstract;

namespace ProjectWithASPNET8.Hypermidia.Filters
{
    public class HyperMediaFilterOption
    {
        public List<IResponseEnricher> ContentResponseEnricherList { get; set;} = new List<IResponseEnricher>();
    }
}
