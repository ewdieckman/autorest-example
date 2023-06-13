using System;

namespace AutoRestExample
{
    public static class ExampleAuthorityHosts
    {
        // be sure to include trailing slashes
        private const string ExampleAuthorityHostUrl = "https://mysts.server.com/";  // be sure to include trailing slash

        /// <summary>
        /// The host of the Example Corporation
        /// </summary>
        public static Uri ExampleAuthorityHost { get; } = new Uri(ExampleAuthorityHostUrl);

        internal static Uri GetDefault()
        {
            if (EnvironmentVariables.AuthorityHost != null)
            {
                return new Uri(EnvironmentVariables.AuthorityHost);
            }

            return ExampleAuthorityHost;
        }
    }
}