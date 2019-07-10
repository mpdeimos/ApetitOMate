using System;

namespace ApetitOMate.Core.Api.Apetito
{
    public class InventoryOrder
    {
        public int Id { get; set; }

        public Receipt GoodsReceipt { get; set; }

        public DateTime OrderDate { get; set; }

        public DateTime DeliveryDate { get; set; }

        public class Receipt
        {
            public int Id { get; set; }

            public bool IsBookedToStorageLocations { get; set; }
        }
    }
}