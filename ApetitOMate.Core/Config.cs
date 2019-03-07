using System;
using ApetitOMate.Core.Api.Apetito;
using ApetitOMate.Core.Api.Mail;
using ApetitOMate.Core.Api.Slack;
using Microsoft.Extensions.Configuration;

namespace ApetitOMate.Core
{
    public class Config
    {
        private static Config instance;
        public static Config Instance => instance ?? (instance = new Config());

        public ApetitoConfig ApetitoConfig { get; }
        public SlackConfig SlackConfig { get; }
        public MailConfig MailConfig { get; }

        private Config()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddIniFile("ApetitOMate.ini", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            this.ApetitoConfig = configuration.GetSection("Apetito").Get<ApetitoConfig>();
            this.SlackConfig = configuration.GetSection("Slack").Get<SlackConfig>();
            this.MailConfig = configuration.GetSection("Mail").Get<MailConfig>();
        }
    }
}