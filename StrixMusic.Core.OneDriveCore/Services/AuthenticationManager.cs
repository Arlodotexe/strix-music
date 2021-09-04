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

namespace StrixMusic.Core.OneDriveCore.Services
{
    /// <summary>
    /// Manages MSAL authentication.
    /// </summary>
    public class AuthenticationManager
    {
        private readonly string _authorityUri = "https://login.microsoftonline.com";

        private string[] _scopes = { "Files.ReadWrite.All" };
        private IPublicClientApplication _clientApp;
        private Uri _authority;

        public string AccessToken { get; private set; }

        /// <summary>
        /// Initalizes the <see cref="IPublicClientApplication"/>.
        /// </summary>
        /// <param name="clientId">The client id of the app in azure portal.</param>
        /// <param name="tenantId">The tenent id generated.</param>
        public void Init(string clientId, string tenantId)
        {
            _authority = new Uri($"{_authorityUri}/{tenantId}");

            _clientApp = PublicClientApplicationBuilder
                 .Create(clientId)
                 .WithAuthority(_authority)
                 .Build();
        }

        /// <summary>
        /// Performs user authorization and returns the a valid token for the app.
        /// </summary>
        /// <returns></returns>
        public async Task GenerateGraphToken()
        {
            Guard.IsNotNull(typeof(IPublicClientApplication), "Client application not initialized. Make sure you initliaze the app before acquiring token.");

            var accounts = await _clientApp.GetAccountsAsync();

            AuthenticationResult result;
            try
            {
                result = await _clientApp.AcquireTokenSilent(_scopes, accounts.FirstOrDefault())
                    .ExecuteAsync();

                AccessToken = result.AccessToken;
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
                    }
                }
                catch (Exception)
                {
                    // TODO: Show a dialog with the error.
                }
            }
        }

    }
}
