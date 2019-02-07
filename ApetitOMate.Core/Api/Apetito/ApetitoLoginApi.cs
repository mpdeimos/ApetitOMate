using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Io;

namespace ApetitOMate.Core.Api.Apetito
{
    public class ApetitoLoginApi
    {
        private const string LoginUrl = "https://www.meinapetito.de/_layouts/15/ppauthentication/login.aspx";
        private readonly ApetitoConfig config;

        public ApetitoLoginApi(ApetitoConfig config)
        {
            this.config = config;
        }

        public async Task<ApetitoApiToken> Login()
        {
            var config = Configuration.Default.WithDefaultLoader().With(new LenientCookieProvider());
            var context = BrowsingContext.New(config);

            var document = await context.OpenAsync(LoginUrl);
            var inputElements = GetInputElements(document);

            GetElementWithNameEnding(inputElements, "signInControl$UserName").Value = this.config.EMail;
            GetElementWithNameEnding(inputElements, "signInControl$password").Value = this.config.Password;
            var loginButton = GetElementWithNameEnding(inputElements, "signInControl$login");

            document = await loginButton.Form.SubmitAsync(loginButton);
            inputElements = GetInputElements(document);

            return new ApetitoApiToken
            {
                CustomerId = GetElementWithNameEnding(inputElements, "hiddenCustomerId").Value,
                BearerToken = GetElementWithNameEnding(inputElements, "hiddenBearerToken").Value
            };
        }

        private List<IHtmlInputElement> GetInputElements(IDocument document)
            => document.Forms.SelectMany(form => form.Elements.OfType<IHtmlInputElement>()).ToList();

        private IHtmlInputElement GetElementWithNameEnding(List<IHtmlInputElement> inputElements, string name)
            => inputElements.First(f => f.Name?.EndsWith(name, StringComparison.InvariantCultureIgnoreCase) ?? false);

        /// <summary>
        /// Lenient cookie management that ignores non-parseable cookies (as provided by Apetito).
        /// </summary>
        public class LenientCookieProvider : ICookieProvider
        {
            private ICookieProvider cookieProvider = new MemoryCookieProvider();

            public string GetCookie(Url url)
            {
                return cookieProvider.GetCookie(url);
            }

            public void SetCookie(Url url, string value)
            {
                try
                {
                    cookieProvider.SetCookie(url, value);
                }
                catch
                {
                    // Ignore wrong formatted cookies
                }
            }
        }
    }
}