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

namespace ApetitOMate.Function.Mail
{
    public static class TableGuestBilling
    {
        [FunctionName(nameof(TableGuestBilling))]
        public static async Task Run([TimerTrigger("0 0 0 1 * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"{nameof(TableGuestBilling)} timer trigger function executed at: {DateTime.Now}");
            await SendBillingMail(log);
        }

        [FunctionName(nameof(TableGuestBilling) + "_Http")]
        public static async Task RunHttp(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = nameof(TableGuestBilling))] HttpRequest req,
            ILogger log)
        {
            await SendBillingMail(log);
        }


        private static async Task SendBillingMail(ILogger log)
        {
            try
            {
                Config config = Config.Instance;
                using (MailClient mailClient = new MailClientFactory(config.MailConfig).Build())
                {
                    await new TableGuestBillingMailAction(
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
