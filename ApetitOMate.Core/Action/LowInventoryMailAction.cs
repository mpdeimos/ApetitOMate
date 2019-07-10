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
            foreach (StorageLocation location in await this.apetitoApi.GetStorageLocations())
            {
                Inventory[] inventory = await this.apetitoApi.GetInventory(location);
                int available = inventory.Sum(i => i.AvailableQuantity);

                if (available > apetitoConfig.LowInventory)
                {
                    continue;
                }

                var builder = new BodyBuilder()
                {
                    TextBody = EMailText(location, available)
                };

                receiver = receiver ?? this.apetitoConfig.EMail;
                await this.mailClient.Send(EMailSubject(location, available), builder.ToMessageBody(), receiver);
            }
        }

        private string EMailSubject(StorageLocation location, int available)
        => $@"Low Apetito Stock ({available} meals left in {location.Name})";

        private string EMailText(StorageLocation location, int available) // TODO: mention until when we should book
        => $@"Apetito inventory has low stock, only {available} meals are left in {location.Name}.
                
Please order new meals at: {InventoryUrl}";
    }
}