using Azure.Core;
using Azure.Core.Pipeline;
using System;

namespace AutoRestExample
{
    /// <summary>
    /// This class is what Azure SDK refers to as a ServiceClient.
    /// </summary>
    /// <remarks>It handles instantiation of subclients
    /// https://azure.github.io/azure-sdk/dotnet_introduction.html#dotnet-subclients
    /// </remarks>
    /// 
    public class ExampleClient
    {
        public const string DefaultBaseUrl = "https://api.service.com";

        private readonly HttpPipeline _pipeline;
        private readonly Uri _endpoint;

        /// <summary> Initializes a new instance of ExampleClient. </summary>
        public ExampleClient(TokenCredential credential)
            : this(new Uri(DefaultBaseUrl), credential, null)
        {
        }

        /// <summary> Initializes a new instance of ExampleClient. </summary>
        public ExampleClient(string endpoint, TokenCredential credential)
            : this(new Uri(endpoint), credential, null)
        {
        }

        /// <summary> Initializes a new instance of ExampleClient. </summary>
        public ExampleClient(TokenCredential credential, ExampleApiClientOptions options)
            : this(new Uri(DefaultBaseUrl), credential, null, options)
        {
        }

        /// <summary> Initializes a new instance of ExampleClient. </summary>
        public ExampleClient(string endpoint, TokenCredential credential, ExampleApiClientOptions options)
            : this(new Uri(endpoint), credential, null, options)
        {
        }

        /// <summary> Initializes a new instance of ExampleClient. </summary>
        public ExampleClient(TokenCredential credential, string[]? scopes)
            : this(new Uri(DefaultBaseUrl), credential, scopes)
        {
        }

        /// <summary> Initializes a new instance of ExampleClient. </summary>
        public ExampleClient(string endpoint, TokenCredential credential, string[]? scopes)
            : this(new Uri(endpoint), credential, scopes)
        {
        }

        /// <summary> Initializes a new instance of ExampleClient. </summary>
        public ExampleClient(TokenCredential credential, string[]? scopes, ExampleApiClientOptions options)
            : this(new Uri(DefaultBaseUrl), credential, scopes, options)
        {
        }

        /// <summary> Initializes a new instance of ExampleClient. </summary>
        public ExampleClient(string endpoint, TokenCredential credential, string[]? scopes, ExampleApiClientOptions options)
            : this(new Uri(endpoint), credential, scopes, options)
        {
        }

        /// <summary> The ClientDiagnostics is used to provide tracing support for the client library. </summary>
        internal ClientDiagnostics ClientDiagnostics { get; }

        /// <summary> Initializes a new instance. </summary>
        /// <param name="endpoint"> Supported service endpoint </param>
        /// <param name="credential">A credential used to authenticate to an Example Corporation service</param>
        /// <param name="scopes">Scopes used to access Example Corporation service</param>
        /// <param name="options"> The options for configuring the client. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="endpoint"/> is null. </exception>
        public ExampleClient(Uri endpoint, TokenCredential credential, string[]? scopes, ExampleApiClientOptions? options = null)
        {
            Argument.AssertNotNull(endpoint, nameof(endpoint));
            options ??= new ExampleApiClientOptions();

            ClientDiagnostics = new ClientDiagnostics(options, true);
            _pipeline = HttpPipelineBuilder.Build(options, new BearerTokenAuthenticationPolicy(credential, scopes ?? Array.Empty<string>()));
            _endpoint = endpoint;
        }

        /// <summary>
        /// Create a new <see cref="ExampleGeneratedClient"/> object
        /// </summary>
        /// <returns>A <see cref="ExampleGeneratedClient"/></returns>
        public virtual ExampleGeneratedClient GetExampleClient() => new ExampleGeneratedClient(ClientDiagnostics, _pipeline, _endpoint);
    }
}