using System;
using System.Linq;
using System.Threading.Tasks;
using ApetitOMate.Core.Api.Apetito;

namespace ApetitOMate.Core.Action
{
    public class UnfulfilledOrderRetriever
    {
        private readonly ApetitoApi apetitoApi;

        public UnfulfilledOrderRetriever(ApetitoApi apetitoApi)
        {
            this.apetitoApi = apetitoApi;
        }

        public async Task<Order[]> Get(DateTime from, DateTime until)
        {
            Order[] guests = await this.apetitoApi.GetTableGuests(from.ToString("yyyy-MM-dd"), until.ToString("yyyy-MM-dd"));
            return guests.Where(guest => !guest.IsOrderFulfilled).ToArray();
        }
    }
}