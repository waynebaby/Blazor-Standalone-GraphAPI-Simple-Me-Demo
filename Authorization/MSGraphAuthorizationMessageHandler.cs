using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorStandaloneGraphAPISimpleMeDemo.Authorization
{
    public class MSGraphAuthorizationMessageHandler : AuthorizationMessageHandler
    {
        public const string CONFIG_PATH = "AzureAd:MSGraph";
        public const string CONFIG_BASEURL_ITEM = "BaseUrl";
        public const string CONFIG_SCOPES_ITEM = "Scopes";

        public MSGraphAuthorizationMessageHandler(IAccessTokenProvider provider,
            NavigationManager navigationManager, IConfiguration config)
            : base(provider, navigationManager)
        {
            var scopesString = config.GetSection(CONFIG_PATH + ":" + CONFIG_SCOPES_ITEM).Value;
            var baseUrl = config.GetSection(CONFIG_PATH + ":" + CONFIG_BASEURL_ITEM).Value;
            ConfigureHandler(
                scopes: scopesString.Split(";"),
                authorizedUrls: new[] { baseUrl }
                );

        }
    }
}
