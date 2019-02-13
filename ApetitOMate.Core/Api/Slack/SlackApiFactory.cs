using Slack.Webhooks;

namespace ApetitOMate.Core.Api.Slack
{
    public class SlackApiFactory
    {
        private readonly SlackConfig config;

        public SlackApiFactory(SlackConfig config)
        {
            this.config = config;
        }

        public ISlackClient Build() => new SlackClient(this.config.WebhookUrl);
    }
}