using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ApetitOMate.Core.Api.Apetito
{
    public class TableGuestBillingRequestOptions
    {
        [JsonProperty("tableGuestGroups")]
        public string[] TableGuestGroups { get; set; } = { };

        [JsonProperty("art")]
        public EKind Kind { get; set; } = EKind.Gesamt;

        public bool AllChecked { get; set; } = true;

        [JsonProperty("SelectedTableGuests")]
        public string[] TableGuests { get; set; } = { };
    }


    [JsonConverter(typeof(StringEnumConverter))]
    public enum EKind
    {
        Gesamt,

        Detail
    }
}