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
        private const string ActivationSubject = "Apetito Account Activated";
        private const string ActivationText =
@"Your Apetito account has been activated.

You can now login via: https://www.mylunch-apetito.de/
";
        private readonly ApetitoConfig apetitoConfig;
        private readonly ApetitoApi apetitoApi;
        private readonly MailClient mailClient;

        public ActivateTableGuestAction(ApetitoConfig apetitoConfig, ApetitoApi apetitoApi, MailClient mailClient)
        {
            this.apetitoConfig = apetitoConfig;
            this.apetitoApi = apetitoApi;
            this.mailClient = mailClient;
        }

        public async Task<TableGuest[]> Run()
        {
            var activated = new List<TableGuest>();
            TableGuest[] guests = await this.apetitoApi.GetTableGuests();
            foreach (TableGuest disabledGuest in guests.Where(guest => guest.IsLocked == true && HasAccountDomain(guest)))
            {
                disabledGuest.IsLocked = false;

                if (this.apetitoConfig.DefaultGroupName != null)
                {
                    TableGuestGroup[] groups = await this.apetitoApi.GetTableGuestGroups();
                    var defaultGroup = groups.FirstOrDefault(group => group.GroupName == this.apetitoConfig.DefaultGroupName);
                    if (defaultGroup != null)
                    {
                        disabledGuest.SetGroup(defaultGroup);
                    }
                }

                TableGuest updated = await this.apetitoApi.UpdateTableGuest(disabledGuest.Id, disabledGuest);

                await this.mailClient.Send(ActivationSubject, ActivationText, updated.EmailAddress);
                activated.Add(updated);
            }

            return activated.ToArray();
        }

        private bool HasAccountDomain(TableGuest guest)
        {
            var domain = this.apetitoConfig.EMail.Split('@', 2).Last();
            return guest.EmailAddress.EndsWith(domain, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}