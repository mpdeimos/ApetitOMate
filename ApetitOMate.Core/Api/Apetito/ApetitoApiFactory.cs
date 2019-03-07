using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using RestEase;
using RestEase.Implementation;

namespace ApetitOMate.Core.Api.Apetito
{
    public class ApetitoApiFactory
    {
        private readonly Lazy<Task<ApetitoApiToken>> apiToken;

        public ApetitoApiFactory(ApetitoConfig config)
        {
            this.apiToken = new Lazy<Task<ApetitoApiToken>>(() => this.RefreshApiToken(config));
        }

        public ApetitoApi Build()
        {
            return RestClient.For<ApetitoApi>("https://api.apetito.de", async (request, cancellationToken) =>
            {
                var token = await this.apiToken.Value;
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.BearerToken);
                request.RequestUri = new Uri(request.RequestUri.OriginalString.Replace(
                    Uri.EscapeUriString("<customerId>"),
                    token.CustomerId
                ));
            });
        }

        private async Task<ApetitoApiToken> RefreshApiToken(ApetitoConfig config)
            => await new ApetitoLoginApi(config).Login();
    }
}