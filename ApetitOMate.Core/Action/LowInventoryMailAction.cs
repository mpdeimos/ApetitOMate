using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ApetitOMate.Core.Api.Apetito;
using ApetitOMate.Core.Api.Mail;
using MimeKit;

namespace ApetitOMate.Core.Action
{
    public class LowInventoryMailAction
    {
        private const string InventoryUrl = "https://www.meinapetito.de/mylunch/Seiten/Inventory.aspx";

        private readonly ApetitoConfig apetitoConfig;
        private readonly ApetitoApi apetitoApi;
        private readonly MailClient mailClient;

        public LowInventoryMailAction(ApetitoConfig apetitoConfig, ApetitoApi apetitoApi, MailClient mailClient)
        {
            this.apetitoConfig = apetitoConfig;
            this.apetitoApi = apetitoApi;
            this.mailClient = mailClient;
        }

        public async Task Run(string receiver = null)
        {
            StorageLocation[] locations = await this.apetitoApi.GetStorageLocations();
            IEnumerable<Inventory[]> allInventory = await Task.WhenAll(locations.Select(location => this.apetitoApi.GetInventory(location)));
            int available = allInventory.SelectMany(inventory => inventory).Sum(inventory => inventory.AvailableQuantity);

            if (available > apetitoConfig.LowInventory)
            {
                return;
            }

            var builder = new BodyBuilder()
            {
                TextBody = EMailText(available)
            };

            receiver = receiver ?? this.apetitoConfig.EMail;
            await this.mailClient.Send(EMailSubject(available), builder.ToMessageBody(), receiver);
        }

        private string EMailSubject(int available)
         => $@"Low Apetito Stock ({available} meals left)";

        private string EMailText(int available) // TODO: mention until when we should book
        => $@"Apetito inventory has low stock, only {available} meals are left.
                
Please order new meals at: {InventoryUrl}";
    }
}