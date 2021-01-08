using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApetitOMate.Core;
using ApetitOMate.Core.Action;
using ApetitOMate.Core.Api.Apetito;
using ApetitOMate.Core.Api.Mail;
using ApetitOMate.Core.Api.Slack;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace ApetitOMate.Function
{
    public static class ActivateTableGuests
    {
        [FunctionName(nameof(ActivateTableGuests))]
        public static async Task Run([TimerTrigger("0 0 9-14 * * Mo-Fr")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"{nameof(ActivateTableGuests)} timer trigger function executed at: {DateTime.Now}");
            await ActivateGuests(log);
        }

        [FunctionName(nameof(ActivateTableGuests) + "_Http")]
        public static async Task<TableGuest[]> RunHttp(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = nameof(ActivateTableGuests))] HttpRequest req,
            ILogger log)
        {
            return await ActivateGuests(log);
        }


        private static async Task<TableGuest[]> ActivateGuests(ILogger log)
        {
            try
            {
                Config config = Config.Instance;
                TableGuest[] activated = await new ActivateTableGuestAction(
                        config.ApetitoConfig,
                        new ApetitoApiFactory(config.ApetitoConfig).Build()
                    ).Run();

                log.LogInformation($"Activated {activated.Length} guests: " + string.Join(", ", activated.Select(a => a.EmailAddress)));
                return activated;
            }
            catch (Exception e)
            {
                log.LogError(e, "Could not perform perform task");
                throw;
            }
        }
    }
}
