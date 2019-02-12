using System;
using System.Threading.Tasks;
using ApetitOMate.Core;
using ApetitOMate.Core.Action;
using ApetitOMate.Core.Api.Apetito;
using ApetitOMate.Core.Api.Slack;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace ApetitOMate.Function
{
    public static class DailyTableGuestsReminderSlack
    {
        [FunctionName(nameof(DailyTableGuestsReminderSlack))]
        public static async Task Run([TimerTrigger("0 40 10 * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"{nameof(DailyTableGuestsReminderSlack)} timer trigger function executed at: {DateTime.Now}");

            try
            {
                Config config = Config.Instance;
                await new TableGuestsReminderSlackAction(
                    config,
                    new SlackApiFactory(config.SlackConfig).Build()
                ).Run();
            }
            catch (Exception e)
            {
                log.LogError(e, "Could not perform perform task");
                throw;
            }
        }

        [FunctionName(nameof(DailyTableGuestsReminderSlack) + "_Http")]
        public static async Task RunHttp(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = nameof(DailyTableGuestsReminderSlack))] HttpRequest req,
            ILogger log)
        {
            await Run(null as TimerInfo, log);
        }
    }
}