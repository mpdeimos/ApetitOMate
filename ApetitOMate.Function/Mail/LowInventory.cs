using System;
using System.Threading.Tasks;
using ApetitOMate.Core;
using ApetitOMate.Core.Action;
using ApetitOMate.Core.Api.Apetito;
using ApetitOMate.Core.Api.Mail;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace ApetitOMate.Function.Mail
{
    public static class LowInventory
    {
        [FunctionName(nameof(LowInventory))]
        public static async Task Run([TimerTrigger("0 30 11 * * Mo,Mi")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"{nameof(LowInventory)} timer trigger function executed at: {DateTime.Now}");
            await SendLowInventoryMail(log);
        }

        [FunctionName(nameof(LowInventory) + "_Http")]
        public static async Task RunHttp(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = nameof(TableGuestBilling))] HttpRequest req,
            ILogger log)
        {
            await SendLowInventoryMail(log);
        }


        private static async Task SendLowInventoryMail(ILogger log)
        {
            try
            {
                Config config = Config.Instance;
                using (MailClient mailClient = new MailClientFactory(config.MailConfig).Build())
                {
                    await new LowInventoryMailAction(
                            config.ApetitoConfig,
                            new ApetitoApiFactory(config.ApetitoConfig).Build(),
                            mailClient
                        ).Run();
                }
            }
            catch (Exception e)
            {
                log.LogError(e, "Could not perform perform task");
                throw;
            }
        }
    }
}