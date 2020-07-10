# Blazor-Standalone-GraphAPI-Simple-Me-Demo
Shows how to access Microsoft Graph API using both Restful Http Client and C# SDK with little configuration.


## Need to know first ##

This version of demo was based on aspnetcore 3.1 version. 

### Generation of project ###
Project was initialed by instructions of this document:

https://docs.microsoft.com/en-us/aspnet/core/blazor/security/webassembly/standalone-with-azure-active-directory?view=aspnetcore-3.1

``` 
dotnet new blazorwasm -au SingleOrg --client-id "{CLIENT ID}" -o {APP NAME} --tenant-id "{TENANT ID}"
```

### Add custome AuthorizationMessageHandler Class to attach tokens to outgoing requests ###
Based on the project that cli generated which can support AAD login perfectly,  accroding artical below, we managed to created a HttpClient Factory which can construct Http Client with Access Tokon in every out going request header.


https://docs.microsoft.com/en-us/aspnet/core/blazor/security/webassembly/additional-scenarios?view=aspnetcore-3.1

The settings was stored in ```wwwroot/appsettings.json```. There is no secret so it is okay to be downloaded and store in the cache if browser.


### Setup your Tenant ID(AAD ID) and  Client ID (Application ID)  ### 
- Which you should copy from your AAD Application Registrations
``` json
{
  "AzureAd": {
    "Authority": "https://login.microsoftonline.com/72f988bf-86f1-41af-91ab-2d7cd011db47", /*This is Microsft Internal AAD.  It is publicly known so nothing to hide*/
    "ClientId": "da35xxxx-xxxx-4d9d-xxxx-520a8e7dxxxx",  /*This one is a little team secret. sorry for masking  */
    "ValidateAuthority": true,
    "MSGraph": {
      "Scopes": "",
      "BaseUrl": "https://graph.microsoft.com/",
    }
  }
}
```



### Configure DI in Program,cs   ###

``` csharp
builder.Services.AddTransient<MSGraphAuthorizationMessageHandler>();
builder.Services
.AddHttpClient(
    "MSGraph",
    (sp, client) => 
        client.BaseAddress = new Uri(sp.GetService<IConfiguration>()
            .GetSection(MSGraphAuthorizationMessageHandler.CONFIG_PATH + ":"+ MSGraphAuthorizationMessageHandler.CONFIG_BASEURL_ITEM)
            .Value))
.AddHttpMessageHandler<MSGraphAuthorizationMessageHandler>();

```


### Resolving HttpClient in Page and consume ### 

For Restful API

``` csharp
try
{
    using (var http = httpFactory.CreateClient("MSGraph"))
    {
        var s = await http.GetStringAsync("v1.0/me");
        executionResult = $@"Graph API Fetched:{s}";
    }
}
catch (Exception ex)
{
    executionResult = ex.ToString();
}
```

For C# SDK
``` csharp
try
{
    using (var http = httpFactory.CreateClient("MSGraph"))
    {
        Microsoft.Graph.GraphServiceClient graphClient = new Microsoft.Graph.GraphServiceClient(http) { AuthenticationProvider = new Microsoft.Graph.DelegateAuthenticationProvider(e => Task.CompletedTask) };
        var user = await graphClient.Me
            .Request()
            .GetAsync();
        executionResult = $@"Graph SDK Fetched: {user.DisplayName},{user.Mail},{user.MailNickname}";
    }
}
catch (Exception ex)
{
    executionResult = ex.ToString();
}

```


# Enjoy! #