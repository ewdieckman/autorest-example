using Azure.Core;
using Azure.Core.Pipeline;
using Azure.Identity;
using Microsoft.Identity.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AutoRestExample
{
    /// <summary>
    /// Enables authentication to a generic Security Token Service using a client id and secret.
    /// A refactor from https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/identity/Azure.Identity/src/Credentials/ClientSecretCredential.cs
    /// </summary>
    public class ExampleCredential : TokenCredential
    {
        internal ExampleConfidentialClient? Client { get; }

        /// <summary>
        /// Gets the client (application) ID
        /// </summary>
        internal string? ClientId { get; }

        /// <summary>
        /// Gets the client secret
        /// </summary>
        internal string? ClientSecret { get; }

        /// <summary>
        /// Gets the client scopes
        /// </summary>
        internal string[] Scopes { get; }

#pragma warning disable CS8618 // exiting constructor with non-null value
        /// <summary>
        /// Protected constructor for mocking.
        /// </summary>
        protected ExampleCredential()
        {
        }
#pragma warning restore CS8618 // exiting constructor with non-null value

        /// <summary>
        /// Creates an instance of the <see cref="ExampleCredential"/> with the details needed to authenticate against a security token service with a client secret.
        /// </summary>
        /// <param name="clientId">The client (application) ID of the service principal</param>
        /// <param name="clientSecret">A client secret that was generated for the App Registration used to authenticate the client.</param>
        public ExampleCredential(string? clientId, string? clientSecret)
            : this(clientId, clientSecret, null, null, null)
        {
        }

        /// <summary>
        /// Creates an instance of the <see cref="ExampleCredential"/> with the details needed to authenticate against Azure Active Directory with a client secret.
        /// </summary>
        /// <param name="clientId">The client (application) ID of the service principal</param>
        /// <param name="clientSecret">A client secret that was generated for the App Registration used to authenticate the client.</param>
        /// <param name="options">Options that allow to configure the management of the requests sent to the Azure Active Directory service.</param>
        public ExampleCredential(string? clientId, string? clientSecret, TokenCredentialOptions? options)
            : this(clientId, clientSecret, null, options, null)
        {
        }

        /// <summary>
        /// Creates an instance of the <see cref="ExampleCredential"/> with the details needed to authenticate against a security token service with a client secret.
        /// </summary>
        /// <param name="clientId">The client (application) ID of the service principal</param>
        /// <param name="clientSecret">A client secret that was generated for the App Registration used to authenticate the client.</param>
        /// <param name="scopes">Scopes for determining access to a particular resource</param>
        public ExampleCredential(string? clientId, string? clientSecret, string[]? scopes)
            : this(clientId, clientSecret, scopes, null, null)
        {
        }

        /// <summary>
        /// Creates an instance of the <see cref="ExampleCredential"/> with the details needed to authenticate against Azure Active Directory with a client secret.
        /// </summary>
        /// <param name="clientId">The client (application) ID of the service principal</param>
        /// <param name="clientSecret">A client secret that was generated for the App Registration used to authenticate the client.</param>
        /// <param name="scopes">Scopes for determining access to a particular resource</param>
        /// <param name="options">Options that allow to configure the management of the requests sent to the Azure Active Directory service.</param>
        public ExampleCredential(string? clientId, string? clientSecret, string[]? scopes, TokenCredentialOptions? options)
            : this(clientId, clientSecret, scopes, options, null)
        {
        }

        internal ExampleCredential(string? clientId, string? clientSecret, string[]? scopes, TokenCredentialOptions? options, ExampleConfidentialClient? client)
        {
            Argument.AssertNotNull(clientId, nameof(clientId));
            Argument.AssertNotNull(clientSecret, nameof(clientSecret));
            ClientId = clientId ?? throw new ArgumentNullException(nameof(clientId));

            ClientSecret = clientSecret;
            Scopes = scopes ?? Array.Empty<string>();
            Client = client ??
                     new ExampleConfidentialClient(
                         clientId,
                         clientSecret,
                         options);
        }

        /// <summary>
        /// Obtains a token from the Azure Active Directory service, using the specified client secret to authenticate. Acquired tokens are cached by the credential instance. Token lifetime and refreshing is handled automatically. Where possible, reuse credential instances to optimize cache effectiveness.
        /// </summary>
        /// <param name="requestContext">The details of the authentication request.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> controlling the request lifetime.</param>
        /// <returns>An <see cref="AccessToken"/> which can be used to authenticate service client calls.</returns>
        public override async ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken = default)
        {
            if (Client == null) throw new Exception("Client is null");
            string[] mergedScopes = MergeScopes(Scopes, requestContext.Scopes);
            AuthenticationResult result = await Client.AcquireTokenForClientAsync(mergedScopes, true, cancellationToken).ConfigureAwait(false);

            return new AccessToken(result.AccessToken, result.ExpiresOn);
        }

        /// <summary>
        /// Obtains a token from the Azure Active Directory service, using the specified client secret to authenticate. Acquired tokens are cached by the credential instance. Token lifetime and refreshing is handled automatically. Where possible, reuse credential instances to optimize cache effectiveness.
        /// </summary>
        /// <param name="requestContext">The details of the authentication request.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> controlling the request lifetime.</param>
        /// <returns>An <see cref="AccessToken"/> which can be used to authenticate service client calls.</returns>
        public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken = default)
        {
            if (Client == null) throw new Exception("Client is null");
            string[] mergedScopes = MergeScopes(Scopes, requestContext.Scopes);
            AuthenticationResult result = Client.AcquireTokenForClientAsync(mergedScopes, false, cancellationToken).EnsureCompleted();

            if (result.AccessToken == null) throw new Exception("Access token is null");
            return new AccessToken(result.AccessToken, result.ExpiresOn);
        }

        /// <summary>
        /// Merges two scope arrays
        /// </summary>
        /// <param name="scopeA"></param>
        /// <param name="scopeB"></param>
        /// <returns></returns>
        internal static string[] MergeScopes(string[] scopeA, string[] scopeB)
        {
            var newArray = new string[scopeA.Length + scopeB.Length];
            scopeA.CopyTo(newArray, 0);
            scopeB.CopyTo(newArray, scopeA.Length);

            return newArray;
        }
    }
}

