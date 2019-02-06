using System.Net.Http.Headers;
using System.Threading.Tasks;
using RestEase;

namespace ApetitOMate.Core.Api
{
    public class ApetitoApiFactory
    {
        private readonly ApetitoApiConfig config;
        public ApetitoApiFactory(ApetitoApiConfig config)
        {
            this.config = config;
        }

        public ApetitoApi Build()
        {
            var api = RestClient.For<ApetitoApi>("https://api.apetito.de", (request, cancellationToken) =>
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", config.BearerToken);
                return Task.FromResult(request);
            });
            api.CustomerId = this.config.CustomerId;

            return api;
        }
    }
}