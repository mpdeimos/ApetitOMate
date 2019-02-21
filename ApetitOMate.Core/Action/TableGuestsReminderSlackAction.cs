using System;
using System.Linq;
using System.Threading.Tasks;
using ApetitOMate.Core.Api.Apetito;
using ApetitOMate.Core.Api.Slack;
using Slack.Webhooks;

namespace ApetitOMate.Core.Action
{
    public class TableGuestsReminderSlackAction
    {
        private readonly Config config;
        private readonly ISlackClient slackApi;

        public TableGuestsReminderSlackAction(Config config, ISlackClient slackApi)
        {
            this.config = config;
            this.slackApi = slackApi;
        }

        public async Task Run()
        {
            await slackApi.PostAsync(new SlackMessage
            {
                Channel = this.config.SlackConfig.Channel,
                Username = "Apetito Bot",
                IconEmoji = Emoji.ForkAndKnife,
                Text = ":mega: <!channel> Please order your Apetito menu until 11:00 via <https://www.mylunch-apetito.de>",
            });
        }
    }
}