using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ApetitOMate.Core.Api.Apetito
{
    [JsonConverter(typeof(TableGuestConverter))]
    public class TableGuest
    {
        private readonly JObject backing;

        public Guid Id => Guid.Parse(this.backing.Value<string>(nameof(Id)));

        public string EmailAddress => this.backing.Value<string>(nameof(EmailAddress));

        public RegistrationStatus Status
        {
            get => Enum.Parse<RegistrationStatus>(this.backing.Value<string>(nameof(Status)));
            set => this.backing[nameof(Status)] = Enum.GetName(typeof(RegistrationStatus), value);
        }

        public bool IsLocked
        {
            get => this.backing.Value<bool>(nameof(IsLocked));
            set => this.backing[nameof(IsLocked)] = value;
        }

        private TableGuest(JObject backing)
        {
            this.backing = backing;
        }

        public void SetGroup(TableGuestGroup defaultGroup)
        {
            var group = this.backing.Value<JObject>(nameof(TableGuestGroup));
            group[nameof(TableGuestGroup.Id)] = defaultGroup.Id;
            group[nameof(TableGuestGroup.GroupName)] = defaultGroup.GroupName;
            group[nameof(TableGuestGroup.IsEligibleForSubsidy)] = defaultGroup.IsEligibleForSubsidy;
        }

        private class TableGuestConverter : JsonConverter<TableGuest>
        {
            public override void WriteJson(JsonWriter writer, TableGuest value, JsonSerializer serializer)
            {
                serializer.Serialize(writer, value.backing);
            }

            public override TableGuest ReadJson(JsonReader reader, Type objectType, TableGuest existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                return new TableGuest(serializer.Deserialize<JObject>(reader));
            }
        }


        public enum RegistrationStatus
        {
            Registered = 2,
            Activated = 3
        }
    }
}