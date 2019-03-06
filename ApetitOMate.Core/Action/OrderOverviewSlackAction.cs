using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApetitOMate.Core.Api.Apetito;
using ApetitOMate.Core.Api.Slack;
using Slack.Webhooks;

namespace ApetitOMate.Core.Action
{
    public class OrderOverviewSlackAction
    {
        private readonly Config config;
        private readonly ApetitoApi apetitoApi;
        private readonly ISlackClient slackApi;

        public OrderOverviewSlackAction(Config config, ApetitoApi apetitoApi, ISlackClient slackApi)
        {
            this.config = config;
            this.apetitoApi = apetitoApi;
            this.slackApi = slackApi;
        }

        public async Task Run(DateTime? date = null)
        {
            date = date ?? DateTime.Today;
            Order[] guests = await this.apetitoApi.GetTableGuests(date?.ToString("yyyy-MM-dd"));

            foreach (var message in CreateMessagesForGuests(guests))
            {
                await slackApi.PostAsync(message);
            }
        }

        private SlackMessage[] CreateMessagesForGuests(Order[] guests)
        {
            if (guests.Length == 0)
            {
                return new SlackMessage[] { CreateMessage(":exclamation: No menus ordered for today.") };
            }

            var messages = new List<SlackMessage>();
            messages.AddRange(guests.Where(guest => guest.IsOrderFulfilled).GroupBy(guest => guest.PickupTime.PickupTimeSpan).Select(guestsByPickup =>
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
            ));

            var incompleteOrders = guests.Where(guest => !guest.IsOrderFulfilled).ToList();
            if (incompleteOrders.Any())
            {
                messages.Add(CreateMessage(null, new SlackAttachment[]
                    {
                        new SlackAttachment
                        {
                            Title = ":exclamation: The following orders could not be fulfilled:",
                            Color = "#AA0000",
                            Fields =  incompleteOrders.Select(guest => new SlackField
                            {
                                Value = $"{guest.FirstName} {guest.LastName}: {guest.ArticleDescription.Trim()} ({guest.OrderPositionState})"
                            }).ToList()
                        }
                    }
                ));

            }

            messages.Add(CreateMessage(":question: <!here> Who is heating the meals?"));

            return messages.ToArray();
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