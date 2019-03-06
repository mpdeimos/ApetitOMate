using System;

namespace ApetitOMate.Core.Api.Apetito
{
    public class Order
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string ArticleDescription { get; set; }

        public string ArticleNumber { get; set; }

        public string OrderState { get; set; }

        public string OrderPositionState { get; set; }

        public bool IsOrderFulfilled => this.OrderPositionState == "Fulfilled";

        public PickupTime PickupTime { get; set; }

        public DateTime ConsumptionDate { get; set; }
    }

    public class PickupTime
    {
        public string PickupTimeSpan { get; set; }
    }
}
