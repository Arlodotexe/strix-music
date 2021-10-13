using Microsoft.Graph;
using Microsoft.Identity.Client;
using OwlCore.AbstractUI.Models;
using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace StrixMusic.Cores.OneDrive.Services
{
    /// <summary>
    /// Manages MSAL authentication.
    /// </summary>
    public class AuthenticationManager
    {
        private readonly string _authorityUri = "https://login.microsoftonline.com/consumers";
        private readonly string[] _scopes = { "Files.Read.All", "User.Read", "Files.ReadWrite" };

        private readonly IPublicClientApplication _clientApp;
        private readonly OneDriveCoreConfig _coreConfig;

        /// <summary>
        /// The access token used to authenticate with OneDrive.
        /// </summary>
        internal string? AccessToken { get; private set; }

        public string? EmailAddress { get; private set; }

        public string? DisplayName {  get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="AuthenticationManager"/>.
        /// </summary>
        /// <param name="clientId">The client id of the app in azure portal.</param>
        /// <param name="tenantId">The tenant id generated.</param>
        /// <param name="redirectUri">The redirect URI to use with the connected application, if any.</param>
        public AuthenticationManager(OneDriveCoreConfig coreConfig, string clientId, string tenantId, string? redirectUri = null)
        {
            _coreConfig = coreConfig;

            var authority = new Uri($"{_authorityUri}/{tenantId}");

            var builder = PublicClientApplicationBuilder
                .Create(clientId)
                .WithAuthority(authority, false);

            if (!string.IsNullOrWhiteSpace(redirectUri))
                builder.WithRedirectUri(redirectUri);

            _clientApp = builder.Build();
        }

        /// <summary>
        /// Performs user authorization and returns the a graph client with access token setup.
        /// </summary>
        /// <returns></returns>
        public async Task<GraphServiceClient?> GenerateGraphToken()
        {
            GraphServiceClient? graphClient = null;
            var accounts = await _clientApp.GetAccountsAsync();
            AuthenticationResult result;

            try
            {
                result = await _clientApp.AcquireTokenSilent(_scopes, accounts.FirstOrDefault()).ExecuteAsync();

                AccessToken = result.AccessToken;
                EmailAddress = result.Account.Username;

                var authProvider = new DelegateAuthenticationProvider(requestMessage =>
                {
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", AccessToken);
                    return Task.CompletedTask;
                });

                graphClient = new GraphServiceClient("https://graph.microsoft.com/v1.0", authProvider);

                var x = await graphClient.Users.Request().GetAsync();
                DisplayName = x.FirstOrDefault()?.DisplayName;
            }
            catch (MsalUiRequiredException)
            {
                try
                {
                    result = await _clientApp.AcquireTokenWithDeviceCode(_scopes, dcr =>
                    {
                        _coreConfig.DisplayDeviceCodeResult(dcr);
                        return Task.CompletedTask;
                    }).ExecuteAsync();

                    if (result != null && !string.IsNullOrWhiteSpace(result.AccessToken))
                    {
                        AccessToken = result.AccessToken;
                        EmailAddress = result.Account.Username;

                        var authProvider = new DelegateAuthenticationProvider(requestMessage =>
                        {
                            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", AccessToken);
                            return Task.CompletedTask;
                        });

                        graphClient = new GraphServiceClient("https://graph.microsoft.com/v1.0", authProvider);
                    }

                }
                catch (Exception)
                {
                    // TODO: Show a dialog with the error.
                    return graphClient;
                }
            }

            return graphClient;
        }
    }
}
