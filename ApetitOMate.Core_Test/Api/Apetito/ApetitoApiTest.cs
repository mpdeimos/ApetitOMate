using System.Linq;
using System.Threading.Tasks;
using ApetitOMate.Core.Api.Apetito;
using FluentAssertions;
using NUnit.Framework;

namespace ApetitOMate.Core.Api.Apetito
{
    public class ApetitoApiTest
    {
        [Test]
        public async Task TestApi()
        {
            var api = new ApetitoApiFactory(Config.Instance.ApetitoConfig).Build();
            TableGuest[] guests = await api.GetTableGuests("2019-02-05");
            guests.Where(guest => guest.ArticleDescription == "Geschnittene Currywurst").Should().HaveCount(3);
        }
    }
}