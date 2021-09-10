using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using System.Linq;
using System.Threading;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.Provisos;

namespace StrixMusic.Cores.OneDrive.Services
{
    /// <summary>
    /// Manages MSAL authentication.
    /// </summary>
    public class AuthenticationManager
    {
        private readonly string _authorityUri = "https://login.microsoftonline.com/consumers";
        private readonly string[] _scopes = { "Files.Read.All" };

        private IPublicClientApplication _clientApp;
        private Uri _authority;

        public string? AccessToken { get; private set; }

        /// <summary>
        /// Initializes the <see cref="IPublicClientApplication"/>.
        /// </summary>
        /// <param name="clientId">The client id of the app in azure portal.</param>
        /// <param name="tenantId">The tenant id generated.</param>
        public AuthenticationManager(string clientId, string tenantId, string redirectUri = null)
        {
            _authority = new Uri($"{_authorityUri}/{tenantId}");

            if (string.IsNullOrEmpty(redirectUri))
            {
                _clientApp = PublicClientApplicationBuilder
                    .Create(clientId)
                    .WithAuthority(_authority, false)
                    .Build();
            }
            else
            {
                _clientApp = PublicClientApplicationBuilder
                    .Create(clientId)
                    .WithRedirectUri(redirectUri)
                    .WithAuthority(_authority, false)
                    .Build();
            }
        }

        /// <summary>
        /// Performs user authorization and returns the a valid token for the app.
        /// </summary>
        /// <returns></returns>
        public async Task<GraphServiceClient> GenerateGraphToken()
        {
            Guard.IsNotNull(typeof(IPublicClientApplication), "Client application not initialized. Make sure you initliaze the app before acquiring token.");

            GraphServiceClient graphClient = null;
            var accounts = await _clientApp.GetAccountsAsync();
            AuthenticationResult result;

            try
            {
                result = await _clientApp.AcquireTokenSilent(_scopes, accounts.FirstOrDefault())
                    .ExecuteAsync();

                AccessToken = result.AccessToken;

                graphClient = new GraphServiceClient(
            "https://graph.microsoft.com/v1.0",
            new DelegateAuthenticationProvider(
                async (requestMessage) =>
                {
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", AccessToken);
                }));
            }
            catch (MsalUiRequiredException)
            {
                try
                {
                    result = await _clientApp.AcquireTokenInteractive(_scopes)
                       .ExecuteAsync(CancellationToken.None);

                    if (result != null && !string.IsNullOrWhiteSpace(result.AccessToken))
                    {
                        AccessToken = result.AccessToken;


                        graphClient = new GraphServiceClient(
                    "https://graph.microsoft.com/v1.0",
                    new DelegateAuthenticationProvider(
                        async (requestMessage) =>
                        {
                            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", AccessToken);
                        }));
                    }

                }
                catch (Exception)
                {
                    // TODO: Show a dialog with the error.
                }
            }

            return graphClient;
        }

    }
}
