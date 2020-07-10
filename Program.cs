using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using BlazorStandaloneGraphAPISimpleMeDemo.Authorization;

namespace BlazorStandaloneGraphAPISimpleMeDemo
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddMsalAuthentication(options =>
            {
                builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);
            });

            builder.Services.AddTransient<MSGraphAuthorizationMessageHandler>();
            builder.Services
            .AddHttpClient(
               "MSGraph",
                (sp, client) => 
                    client.BaseAddress = new Uri(sp.GetService<IConfiguration>()
                        .GetSection(MSGraphAuthorizationMessageHandler.CONFIG_PATH + ":"+ MSGraphAuthorizationMessageHandler.CONFIG_BASEURL_ITEM)
                        .Value))
            .AddHttpMessageHandler<MSGraphAuthorizationMessageHandler>();

            await builder.Build().RunAsync();
        }
    }
}
