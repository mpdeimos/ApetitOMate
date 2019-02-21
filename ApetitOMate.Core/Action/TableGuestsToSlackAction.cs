using System;
using System.Collections.Generic;
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
        private readonly ISlackClient slackApi;

        public TableGuestsToSlackAction(Config config, ApetitoApi apetitoApi, ISlackClient slackApi)
        {
            this.config = config;
            this.apetitoApi = apetitoApi;
            this.slackApi = slackApi;
        }

        public async Task Run()
        {
            TableGuest[] guests = await this.apetitoApi.GetTableGuests(DateTime.Today.ToString("yyyy-MM-dd"));

            foreach (var message in CreateMessagesForGuests(guests))
            {
                await slackApi.PostAsync(message);
            }
        }

        private SlackMessage[] CreateMessagesForGuests(TableGuest[] guests)
        {
            if (guests.Length == 0)
            {
                return new SlackMessage[] { CreateMessage(":exclamation: No menus ordered for today.") };
            }

            return guests.GroupBy(guest => guest.PickupTime.PickupTimeSpan).Select(guestsByPickup =>
                CreateMessage($"Ordered {guestsByPickup.Count()} menus for {guestsByPickup.Key}", guestsByPickup.GroupBy(guest => (name: guest.ArticleDescription, number: guest.ArticleNumber)).Select(guestsByArticle =>
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
                    ).ToArray()
                )
            ).Append(CreateMessage(":question: <!here> Who is heating the meals?")).ToArray();
        }

        private SlackMessage CreateMessage(string text, params SlackAttachment[] attachments)
        {
            return new SlackMessage
            {
                Channel = this.config.SlackConfig.Channel,
                Username = "Apetito Bot",
                IconEmoji = Emoji.ForkAndKnife,
                Text = text,
                Attachments = attachments.ToList()
            };
        }
    }
}