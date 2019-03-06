using System;
using System.Linq;
using System.Threading.Tasks;
using ApetitOMate.Core.Api.Apetito;

namespace ApetitOMate.Core.Action
{
    public class TableGuestsUnfulfilledOrderRetriever
    {
        private readonly ApetitoApi apetitoApi;

        public TableGuestsUnfulfilledOrderRetriever(ApetitoApi apetitoApi)
        {
            this.apetitoApi = apetitoApi;
        }

        public async Task<TableGuest[]> Get(DateTime from, DateTime until)
        {
            TableGuest[] guests = await this.apetitoApi.GetTableGuests(from.ToString("yyyy-MM-dd"), until.ToString("yyyy-MM-dd"));
            return guests.Where(guest => !guest.IsOrderFulfilled).ToArray();
        }
    }
}