
namespace ApetitOMate.Core.Api.Mail
{
    public class MailClientFactory
    {
        private readonly MailConfig config;

        public MailClientFactory(MailConfig config)
        {
            this.config = config;
        }

        public MailClient Build() => new MailClient(this.config);
    }
}