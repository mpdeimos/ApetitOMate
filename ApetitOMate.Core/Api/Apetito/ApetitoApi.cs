using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RestEase;

namespace ApetitOMate.Core.Api.Apetito
{
    public interface ApetitoApi
    {
        [Get("order/mylunch/orders/caretaker/tableguestsoverview?customerId=<customerId>")]
        Task<Order[]> GetOrders([Query] string timeFrom, [Query] string timeTo, [Query] int pickupLocationId = -1, [Query] int pickupTimeId = -1);

        [Get("tableguest/TableGuests/<customerId>")]
        Task<TableGuest[]> GetTableGuests();

        [Put("tableguest/TableGuests/<customerId>/{guestId}")]
        Task<TableGuest> UpdateTableGuest([Path] int guestId, [Body] TableGuest guest);

        [Get("tableguest/tableguestgroups/<customerId>")]
        Task<TableGuestGroup[]> GetTableGuestGroups();
    }

    public static class ApetitoApiExtensions
    {
        public static async Task<Order[]> GetOrders(this ApetitoApi api, [Query] string date, [Query] int pickupLocationId = -1, [Query] int pickupTimeId = -1)
            => await api.GetOrders(date, date, pickupLocationId, pickupTimeId);
    }
}