using System.Linq;
using System.Threading.Tasks;
using ApetitOMate.Core.Api.Apetito;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace ApetitOMate.Core.Api.Apetito
{
    public class ApetitoApiTest
    {
        private ApetitoApi api = new ApetitoApiFactory(Config.Instance.ApetitoConfig).Build();

        [Test]
        public async Task TestGetOrders()
        {
            Order[] guests = await this.api.GetOrders("2019-02-05");
            guests.Where(guest => guest.ArticleDescription == "Geschnittene Currywurst").Should().HaveCount(3);
        }

        [Test]
        public async Task TestIncompleteOrders()
        {
            Order[] guests = await this.api.GetOrders("2019-03-01");
            guests.Where(guest => !guest.IsOrderFulfilled)
                .Should().HaveCount(1)
                .And.Subject.First()
                    .Should().Match<Order>(guest => guest.OrderState == "4" && guest.OrderPositionState == "UnsuficcientStock");
        }


        [Test]
        public async Task TestGetTableGuests()
        {
            TableGuest[] guests = await this.api.GetTableGuests();
            guests.Where(guest => guest.EmailAddress.EndsWith("@cqse.eu")).Should().NotBeEmpty();
            foreach (TableGuest disabledGuest in guests.Where(guest => guest.IsLocked == true))
            {
                disabledGuest.IsLocked = false;
                TableGuest updated = await this.api.UpdateTableGuest(disabledGuest.Id, disabledGuest);
                updated.IsLocked.Should().BeFalse();
            }

        }

        [Test]
        public async Task TestGetTableGuestGroups()
        {
            TableGuestGroup[] groups = await this.api.GetTableGuestGroups();
            groups.Where(group => group.Id == 197).Should().HaveCount(1)
                .And.Subject.First().Should().Match<TableGuestGroup>(group => group.GroupName == "MAUS");
        }
    }
}