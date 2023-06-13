using Azure.Core.Extensions;
using System;

namespace AutoRestExample
{
    public static class IAzureClientBuilderExtensions
    {
        /// <summary>
        /// Registers a <see cref="ExampleClient"/> instance with the provided <paramref name="vaultUri"/>
        /// </summary>
        public static IAzureClientBuilder<ExampleClient, ExampleApiClientOptions> AddExampleClient<TBuilder>(this TBuilder builder)
            where TBuilder : IAzureClientFactoryBuilderWithCredential
        {
            return builder.RegisterClientFactory<ExampleClient, ExampleApiClientOptions>((options, cred) => new ExampleClient(cred, null, options));
        }

        /// <summary>
        /// Registers a <see cref="ExampleClient"/> instance with the provided <paramref name="baseUri"/>
        /// </summary>
        public static IAzureClientBuilder<ExampleClient, ExampleApiClientOptions> AddExampleClient<TBuilder>(this TBuilder builder, Uri baseUri)
            where TBuilder : IAzureClientFactoryBuilderWithCredential
        {
            return builder.RegisterClientFactory<ExampleClient, ExampleApiClientOptions>((options, cred) => new ExampleClient(baseUri, cred, null, options));
        }

    }
}