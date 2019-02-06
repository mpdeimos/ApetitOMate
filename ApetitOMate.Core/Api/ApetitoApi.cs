using System.Threading.Tasks;
using ApetitOMate.Core.Model;
using RestEase;

namespace ApetitOMate.Core.Api
{
    public interface ApetitoApi
    {
        [Query("customerId")]
        int CustomerId { get; set; }

        [Get("order/mylunch/orders/caretaker/tableguestsoverview")]
        Task<TableGuest[]> GetTableGuests([Query] string timeFrom, [Query] string timeTo, [Query] int pickupLocationId = -1, [Query] int pickupTimeId = -1);
    }
}