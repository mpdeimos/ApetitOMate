using System;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace ApetitOMate.Core.Api.Mail
{
    public class MailClient : IDisposable
    {
        private readonly SmtpClient client = new SmtpClient()
        {
            ServerCertificateValidationCallback = (s, c, h, e) => true
        };
        private readonly MailConfig config;

        public MailClient(MailConfig config)
        {
            this.config = config;
        }

        public void Dispose()
        {
            if (this.client.IsConnected)
            {
                this.client.Disconnect(true);
            }

            this.client.Dispose();
        }

        private async Task EnsureConnected()
        {
            if (!this.client.IsConnected)
            {
                await this.client.ConnectAsync(config.Host, config.Port, SecureSocketOptions.SslOnConnect);
            }

            if (!this.client.IsAuthenticated)
            {
                await client.AuthenticateAsync(config.Username, config.Password);
            }
        }

        public async Task Send(string subject, string text, params string[] to)
        {
            await this.EnsureConnected();

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(this.config.Name ?? this.config.Username, this.config.Username));
            message.To.AddRange(to.Select(a => new MailboxAddress(a)));
            message.Subject = subject;
            message.Body = new TextPart("plain")
            {
                Text = text
            };

            await this.client.SendAsync(message);
        }
    }
}