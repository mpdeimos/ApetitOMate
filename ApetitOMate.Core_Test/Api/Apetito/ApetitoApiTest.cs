using System.Linq;
using System.Threading.Tasks;
using ApetitOMate.Core.Api.Apetito;
using FluentAssertions;
using NUnit.Framework;

namespace ApetitOMate.Core.Api.Apetito
{
    public class ApetitoApiTest
    {
        private ApetitoApi api = new ApetitoApiFactory(Config.Instance.ApetitoConfig).Build();

        [Test]
        public async Task TestApi()
        {
            Order[] guests = await this.api.GetTableGuests("2019-02-05");
            guests.Where(guest => guest.ArticleDescription == "Geschnittene Currywurst").Should().HaveCount(3);
        }

        [Test]
        public async Task TestIncompleteTableMenu()
        {
            Order[] guests = await this.api.GetTableGuests("2019-03-01");
            guests.Where(guest => !guest.IsOrderFulfilled)
                .Should().HaveCount(1)
                .And.Subject.First()
                    .Should().Match<Order>(guest => guest.OrderState == "4" && guest.OrderPositionState == "UnsuficcientStock");
        }
    }
}