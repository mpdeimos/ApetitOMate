using System.Threading.Tasks;
using RestEase;

namespace ApetitOMate.Core.Api.Apetito
{
    public interface ApetitoApi
    {
        [Get("order/mylunch/orders/caretaker/tableguestsoverview")]
        Task<Order[]> GetTableGuests([Query] string timeFrom, [Query] string timeTo, [Query] int pickupLocationId = -1, [Query] int pickupTimeId = -1);
    }

    public static class ApetitoApiExtensions
    {
        public static async Task<Order[]> GetTableGuests(this ApetitoApi api, [Query] string date, [Query] int pickupLocationId = -1, [Query] int pickupTimeId = -1)
            => await api.GetTableGuests(date, date, pickupLocationId, pickupTimeId);
    }
}