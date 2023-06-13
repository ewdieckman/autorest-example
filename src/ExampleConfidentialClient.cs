using Azure.Core;
using Azure.Identity;
using Microsoft.Identity.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AutoRestExample
{
    /// <summary>
    /// modeled after https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/identity/Azure.Identity/src/MsalClientBase.cs#L60
    /// child class:  https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/identity/Azure.Identity/src/MsalConfidentialClient.cs
    /// </summary>
    public class ExampleConfidentialClient
    {
        private readonly AsyncLockWithValue<IConfidentialClientApplication> _clientAsyncLock;
        internal readonly string _clientSecret;

        public ExampleConfidentialClient(string? clientId, string? clientSecret, TokenCredentialOptions? options)
        {
            //TokenEndpoint = new Uri(tokenEndpoint ?? string.Empty);
            ClientId = clientId ?? string.Empty;
            _clientSecret = clientSecret ?? string.Empty;
            _clientAsyncLock = new AsyncLockWithValue<IConfidentialClientApplication>();
            AuthorityHost = options?.AuthorityHost ?? ExampleAuthorityHosts.GetDefault();
            Options = options;
        }

        //internal Uri TokenEndpoint { get; }

        internal string ClientId { get; }

        internal Uri AuthorityHost { get; }

        internal TokenCredentialOptions? Options { get; }

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

        protected async ValueTask<IConfidentialClientApplication> GetClientAsync(bool async, CancellationToken cancellationToken)
        {
            using var asyncLock = await _clientAsyncLock.GetLockOrValueAsync(async, cancellationToken).ConfigureAwait(false);
            if (asyncLock.HasValue)
            {
                return asyncLock.Value;
            }

            var client = await CreateClientAsync(async, cancellationToken).ConfigureAwait(false);

            asyncLock.SetValue(client);
            return client;
        }

        public virtual async ValueTask<AuthenticationResult> AcquireTokenForClientAsync(
            string[] scopes,
            bool async,
            CancellationToken cancellationToken)
        {
            var result = await AcquireTokenForClientCoreAsync(scopes, async, cancellationToken).ConfigureAwait(false);
            return result;
        }

        public virtual async ValueTask<AuthenticationResult> AcquireTokenForClientCoreAsync(
            string[] scopes,
            bool async,
            CancellationToken cancellationToken)
        {
            IConfidentialClientApplication client = await GetClientAsync(async, cancellationToken).ConfigureAwait(false);

            var builder = client
                .AcquireTokenForClient(scopes);

            Microsoft.Identity.Client.AuthenticationResult result = async
                ? await builder.ExecuteAsync(cancellationToken).ConfigureAwait(false)
#pragma warning disable AZC0102 // Do not use GetAwaiter().GetResult(). Use the TaskExtensions.EnsureCompleted() extension method instead.
                : builder.ExecuteAsync(cancellationToken).GetAwaiter().GetResult();
#pragma warning restore AZC0102 // Do not use GetAwaiter().GetResult(). Use the TaskExtensions.EnsureCompleted() extension method instead.

            return result;
        }
    }
}