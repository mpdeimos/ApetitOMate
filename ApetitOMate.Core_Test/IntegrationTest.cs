using System;
using System.Threading.Tasks;
using ApetitOMate.Core;
using ApetitOMate.Core.Action;
using ApetitOMate.Core.Api.Apetito;
using ApetitOMate.Core.Api.Mail;
using ApetitOMate.Core.Api.Slack;
using NUnit.Framework;

namespace ApetitOMate.Core_Test
{
    public class IntegrationTest
    {
        private readonly Config config = Config.Instance;

        [Test]
        public async Task TestSlackAction()
        {
            await new OrderOverviewSlackAction(
                config,
                new ApetitoApiFactory(config.ApetitoConfig).Build(),
                new SlackApiFactory(config.SlackConfig).Build()
            ).Run(DateTime.Parse("2019-05-03"));
        }

        [Test]
        public async Task TestActivationAction()
        {
            await new ActivateTableGuestAction(
                config.ApetitoConfig,
                new ApetitoApiFactory(config.ApetitoConfig).Build(),
                new MailClientFactory(config.MailConfig).Build()
            ).Run();
        }

        [Test]
        public async Task TestBillingMail()
        {
            await new TableGuestBillingMailAction(
                config.ApetitoConfig,
                new ApetitoApiFactory(config.ApetitoConfig).Build(),
                new MailClientFactory(config.MailConfig).Build()
            ).Run(receiver: "poehlmann@cqse.eu");
        }
    }
}