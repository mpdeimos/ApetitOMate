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

namespace ApetitOMate.Function.Slack
{
    public static class DailyOrderReminder
    {
        [FunctionName(nameof(DailyOrderReminder))]
        public static async Task Run([TimerTrigger("0 40 10 * * Mo-Fr")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"{nameof(DailyOrderReminder)} timer trigger function executed at: {DateTime.Now}");

            try
            {
                Config config = Config.Instance;
                await new OrderReminderSlackAction(
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

        [FunctionName(nameof(DailyOrderReminder) + "_Http")]
        public static async Task RunHttp(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = nameof(DailyOrderReminder))] HttpRequest req,
            ILogger log)
        {
            await Run(null as TimerInfo, log);
        }
    }
}
