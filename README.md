# Using non-Azure authentication in an AutoRest library
With the complete re-write of AutoRest in version 3, the clients generated from the OpenApi spec are tightly couples to the `Microsoft.Identity.Client` and `Azure.Identity` libraries.  Most classes and constructors in these libraries are `internal`, which makes it difficult to extend their capabilities to use a non-Azure security token service (STS).

To work around these limitations, you will need to:
 - Create a new credential class that inherits from `TokenCredential`
 - Create a new client class that mimics [`MsalConfidentialClient`](https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/identity/Azure.Identity/src/MsalConfidentialClient.cs)
 - Explicitly reference `Microsoft.Identity.Client` v4.54.1 until `Azure.Identity` references that version or greater.

## Library generation
This particular example was generated using the following AutoRest options (found in [autorest.md](autorest.md)
 - `--public-clients` - Allows the subclients to have public constructors
 - `--skip-csproj` - Doesn't overwrite the `.csproj` file with default settings everytime it's generated
 - `--generation1-convenience-client` - Creates models and returns `Response<ModelHere>` for API methods

## ExampleCredential

This class inherits from `TokenCredential` and closely mimics [`ClientSecretCredential`](https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/identity/Azure.Identity/src/Credentials/ClientSecretCredential.cs).  

I have also chosen to include Scopes in this credential.  The MSAL library REQUIRES scopes on all `client_credentials` so they must be supplied.  As the scopes change from app to app, there needed to be a way to inject them into the client.

```csharp
// Program.cs
var credential = new ExampleCredential(
    builder.Configuration["MySTS:ClientId"],
    builder.Configuration["MySTS:ClientSecret"],
    builder.Configuration["MySTS:Scope"]?.Split(' ', StringSplitOptions.RemoveEmptyEntries));

builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddExampleClient();

    clientBuilder.UseCredential(credential);
});

```

## ExampleConfidentialClient

This class takes care of acquiring the access token for use in API calls.   In order to use a non-Azure authority host URL, you must utilize the `WithGenericAuthority()` method.  This method is paired with the `WithExperimentalFeatures()` flag.

```csharp
// ExampleConfidentialClient.cs

protected ValueTask<IConfidentialClientApplication> CreateClientAsync(bool async, CancellationToken cancellationToken)
{
    var identityClient = ConfidentialClientApplicationBuilder.Create(ClientId)
        .WithExperimentalFeatures()     // for WithGenericAuthority
        .WithGenericAuthority(AuthorityHost.AbsoluteUri)
        .WithClientId(ClientId)
        .WithClientSecret(_clientSecret)
        .Build();

    return new ValueTask<IConfidentialClientApplication>(identityClient);
}
```

### Please note
You need to explicitly set a reference to `Microsoft.Identity.Client` v4.54.1 in the library that houses the `ExampleConfidentialClient`.   This allows access to the `WithGenericAuthority` extension.  Once `Azure.Identity` package references v4.54.1 or greater of the package, the explicit package reference can be removed.