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
    public class TableGuestBillingMailAction
    {
        private readonly ApetitoConfig apetitoConfig;
        private readonly ApetitoApi apetitoApi;
        private readonly MailClient mailClient;

        public TableGuestBillingMailAction(ApetitoConfig apetitoConfig, ApetitoApi apetitoApi, MailClient mailClient)
        {
            this.apetitoConfig = apetitoConfig;
            this.apetitoApi = apetitoApi;
            this.mailClient = mailClient;
        }

        public async Task Run(DateTime? from = null, DateTime? to = null, string receiver = null)
        {
            if (to == null)
            {
                to = DateTime.Now.AddDays(-DateTime.Now.Day);
            }

            if (from == null)
            {
                from = to.Value.AddDays(-to.Value.Day + 1);
            }

            var guestGroups = await this.apetitoApi.GetTableGuestGroups();
            var options = new TableGuestBillingRequestOptions
            {
                Kind = EKind.Gesamt,
                TableGuestGroupIds = guestGroups.Select(g => g.Id).ToArray(),
                AllChecked = true,
            };

            Stream summary = await this.apetitoApi.DownloadTableGuestBilling(options, from.Value.ToString("yyyy-MM-dd"), to.Value.ToString("yyyy-MM-dd"));
            options.Kind = EKind.Detail;

            Stream details = await this.apetitoApi.DownloadTableGuestBilling(options, from.Value.ToString("yyyy-MM-dd"), to.Value.ToString("yyyy-MM-dd"));

            var builder = new BodyBuilder();
            builder.TextBody = EMailText(from.Value, to.Value);
            builder.Attachments.Add($"Apetito_Summary_{from.Value.ToString("yyyyMMdd")}-{to.Value.ToString("yyyyMMdd")}.xlsx", summary);
            builder.Attachments.Add($"Apetito_Details_{from.Value.ToString("yyyyMMdd")}-{to.Value.ToString("yyyyMMdd")}.xlsx", details);

            receiver = receiver ?? this.apetitoConfig.EMail;
            await this.mailClient.Send(EMailSubject(from.Value, to.Value), builder.ToMessageBody(), receiver);
        }

        private string EMailSubject(DateTime from, DateTime to)
         => $@"Apetito Guest Billing ({from.ToString("yyyy-MM-dd")} - {to.ToString("yyyy-MM-dd")}).";

        private string EMailText(DateTime from, DateTime to)
        => $@"You can find the Apetito guest billing from {from.ToString("yyyy-MM-dd")} to {to.ToString("yyyy-MM-dd")} attached to this mail.

Please forward the mail to the payroll administration office.";
    }
}