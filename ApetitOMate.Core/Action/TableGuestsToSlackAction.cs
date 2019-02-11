using System;
using System.Linq;
using System.Threading.Tasks;
using ApetitOMate.Core.Api.Apetito;
using ApetitOMate.Core.Api.Slack;
using Slack.Webhooks;

namespace ApetitOMate.Core.Action
{
    public class TableGuestsToSlackAction
    {
        private readonly Config config;
        private readonly ApetitoApi apetitoApi;
        private readonly SlackClient slackApi;

        public TableGuestsToSlackAction(Config config, ApetitoApi apetitoApi, SlackClient slackApi)
        {
            this.config = config;
            this.apetitoApi = apetitoApi;
            this.slackApi = slackApi;
        }

        public async Task Run()
        {
            TableGuest[] guests = await this.apetitoApi.GetTableGuests(DateTime.Today.ToString("yyyy-MM-dd"));

            var messages = guests.GroupBy(guest => guest.PickupTime.PickupTimeSpan).Select(guestsByPickup =>
                new SlackMessage
                {
                    Channel = this.config.SlackConfig.Channel,
                    Username = "Apetito Bot",
                    IconEmoji = Emoji.ForkAndKnife,
                    Text = $"Ordered {guestsByPickup.Count()} menus for {guestsByPickup.Key}",
                    Attachments = guestsByPickup.GroupBy(guest => (name: guest.ArticleDescription, number: guest.ArticleNumber)).Select(guestsByArticle =>
                        new SlackAttachment
                        {
                            Title = $"{guestsByArticle.Count()}x {guestsByArticle.Key.name.Trim()} ({guestsByArticle.Key.number})",
                            Fields = guestsByArticle.Select(guest =>
                                new SlackField
                                {
                                    Value = $"{guest.FirstName} {guest.LastName}"
                                }
                            ).ToList()
                        }
                    ).ToList()
                }
            ).ToList();

            messages.ForEach(message => slackApi.Post(message));
        }
    }
}