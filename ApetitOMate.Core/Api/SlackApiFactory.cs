using Slack.Webhooks;

namespace ApetitOMate.Core.Api
{
    public class SlackApiFactory
    {
        private readonly SlackApiConfig config;

        public SlackApiFactory(SlackApiConfig config)
        {
            this.config = config;
        }

        public SlackClient Build() => new SlackClient(this.config.WebhookUrl);
    }
}