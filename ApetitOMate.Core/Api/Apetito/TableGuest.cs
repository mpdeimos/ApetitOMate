namespace ApetitOMate.Core.Api.Apetito
{
    public class TableGuest
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string ArticleDescription { get; set; }

        public string ArticleNumber { get; set; }

        public PickupTime PickupTime { get; set; }
    }

    public class PickupTime
    {
        public string PickupTimeSpan { get; set; }
    }
}
