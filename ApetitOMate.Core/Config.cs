using System;
using ApetitOMate.Core.Api;
using Microsoft.Extensions.Configuration;

namespace ApetitOMate.Core
{
    public class Config
    {
        private static Config instance;
        public static Config Instance => instance ?? (instance = new Config());

        public ApetitoApiConfig ApetitoApiConfig { get; }
        public SlackApiConfig SlackApiConfig { get; }

        private Config()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddIniFile("ApetitOMate.ini", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            this.ApetitoApiConfig = configuration.GetSection("ApetitoApi").Get<ApetitoApiConfig>();
            this.SlackApiConfig = configuration.GetSection("SlackApi").Get<SlackApiConfig>();
        }
    }
}