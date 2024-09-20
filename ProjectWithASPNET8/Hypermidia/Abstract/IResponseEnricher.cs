using Microsoft.AspNetCore.Mvc.Filters;

namespace ProjectWithASPNET8.Hypermidia.Abstract
{
    public interface IResponseEnricher
    {
        bool CanEnrich(ResultExecutingContext context);
        Task Enrich(ResultExecutingContext context);
    }
}
