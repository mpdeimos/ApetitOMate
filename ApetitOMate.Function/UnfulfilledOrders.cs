using System;
using System.Threading.Tasks;
using ApetitOMate.Core;
using ApetitOMate.Core.Action;
using ApetitOMate.Core.Api.Apetito;
using ApetitOMate.Core.Api.Slack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace ApetitOMate.Function
{
    public static class UnfulfilledOrders
    {
        [FunctionName(nameof(UnfulfilledOrders))]
        public static async Task<object> RunHttp(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = nameof(UnfulfilledOrders))] HttpRequest req,
            ILogger log)
        {
            string from = req.Query["from"];
            if (from == null)
            {
                return new BadRequestObjectResult("Parameter 'from' is required.");
            }

            string until = req.Query["until"];
            if (until == null)
            {
                until = from;
            }

            var retriever = new TableGuestsUnfulfilledOrderRetriever(new ApetitoApiFactory(Config.Instance.ApetitoConfig).Build());
            return await retriever.Get(DateTime.Parse(from), DateTime.Parse(until));
        }
    }
}
