using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ProjectWithASPNET8.Hypermidia.Filters
{
    public class HyperMediaFilter : ResultFilterAttribute
    {
        private readonly HyperMediaFilterOption _hyperMediaFilterOption;

        public HyperMediaFilter(HyperMediaFilterOption hyperMediaFilterOption)
        {
            _hyperMediaFilterOption = hyperMediaFilterOption;
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            TryEnrichResult(context);
            base.OnResultExecuting(context);
        }

        private void TryEnrichResult(ResultExecutingContext context)
        {
            if (context.Result is OkObjectResult okObjectResult)
            {
                var enricher = _hyperMediaFilterOption
                    .ContentResponseEnricherList
                    .FirstOrDefault(x => x.CanEnrich(context));

                if (enricher != null) Task.FromResult(enricher.Enrich(context));
            };
        }
    }
}
