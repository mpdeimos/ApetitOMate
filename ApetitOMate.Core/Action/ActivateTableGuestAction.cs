using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApetitOMate.Core.Api.Apetito;
using ApetitOMate.Core.Api.Mail;

namespace ApetitOMate.Core.Action
{
    public class ActivateTableGuestAction
    {
        private readonly ApetitoConfig apetitoConfig;
        private readonly ApetitoApi apetitoApi;

        public ActivateTableGuestAction(ApetitoConfig apetitoConfig, ApetitoApi apetitoApi)
        {
            this.apetitoConfig = apetitoConfig;
            this.apetitoApi = apetitoApi;
        }

        public async Task<TableGuest[]> Run()
        {
            var activated = new List<TableGuest>();
            TableGuest[] guests = await this.apetitoApi.GetTableGuests();
            TableGuest[] unactivatedGuests = guests.Where(guest => guest.Status == TableGuest.RegistrationStatus.Registered && HasAccountDomain(guest)).ToArray();
            await this.apetitoApi.ActivateTableGuests(unactivatedGuests);
            return unactivatedGuests;
        }

        private bool HasAccountDomain(TableGuest guest)
        {
            if (guest.EmailAddress.Contains("apetitotest1"))
            {
                return false;
            }

            var domain = this.apetitoConfig.EMail.Split('@', 2).Last();
            return guest.EmailAddress.EndsWith(domain, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}