using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using RestEase;

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

                var uriBuilder = new UriBuilder(request.RequestUri.OriginalString);
                var query = HttpUtility.ParseQueryString(request.RequestUri.Query);
                query["customerId"] = token.CustomerId;
                uriBuilder.Query = query.ToString();
                request.RequestUri = uriBuilder.Uri;
            });
        }

        private async Task<ApetitoApiToken> RefreshApiToken(ApetitoConfig config)
            => await new ApetitoLoginApi(config).Login();
    }
}