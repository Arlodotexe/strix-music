using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using OwlCore.AbstractUI.Models;
using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using StrixMusic.Sdk.Services;
using Microsoft.Toolkit.Diagnostics;
using StrixMusic.Sdk.Data;

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
        /// The user's email address, if known.
        /// </summary>
        public string? EmailAddress { get; private set; }

        /// <summary>
        /// The user's display name, if known.
        /// </summary>
        public string? DisplayName { get; private set; }

        /// <summary>
        /// The method used to log the user in.
        /// </summary>
        public LoginMethod LoginMethod { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="AuthenticationManager"/>.
        /// </summary>
        /// <param name="clientId">The client id of the app in azure portal.</param>
        /// <param name="tenantId">The tenant id generated.</param>
        /// <param name="redirectUri">The redirect URI to use with the connected application, if any.</param>
        public AuthenticationManager(OneDriveCoreConfig coreConfig, string clientId, string tenantId, string? redirectUri = null)
        {
            _coreConfig = coreConfig;

            LoginMethod = Sdk.Helpers.PlatformHelper.Current switch
            {
                Platform.UWP => LoginMethod.DeviceCode,
                Platform.WASM => LoginMethod.Interactive,
                Platform.Droid => LoginMethod.Interactive,
                Platform.Unknown => LoginMethod.None,
                _ => ThrowHelper.ThrowNotSupportedException<LoginMethod>(),
            };

            var authority = new Uri($"{_authorityUri}/{tenantId}");

            var builder = PublicClientApplicationBuilder
                .Create(clientId)
                .WithAuthority(authority, false);

            if (!string.IsNullOrWhiteSpace(redirectUri))
                builder.WithRedirectUri(redirectUri);

            builder = Ioc.Default.GetRequiredService<ISharedFactory>().WithUnoHelpers(builder);

            _clientApp = builder.Build();
        }

        /// <summary>
        /// Generate a <see cref="GraphServiceClient"/> and handles login if needed.
        /// </summary>
        /// <returns></returns>
        public async Task<GraphServiceClient?> TryLogin()
        {
            var authenticationResult = await TryAcquireCachedToken();

            if (authenticationResult is null)
            {
                var cancellationTokenSource = new CancellationTokenSource();
                authenticationResult = await TryAcquireTokenViaLogin(cancellationTokenSource);
            }

            if (authenticationResult is null)
                return null;

            var graphClient = CreateGraphClient(authenticationResult.AccessToken);
            var user = await graphClient.Users.Request().GetAsync();

            EmailAddress = authenticationResult.Account.Username;
            DisplayName = user.FirstOrDefault()?.DisplayName;

            return graphClient;
        }

        private async Task<AuthenticationResult?> TryAcquireCachedToken()
        {
            // TODO: Cache and encrypt the token ourselves, and add manual token refreshing.
            // Using AcquireTokenSilent means we're restricted to only 1 account per device.
            // Maybe multiple accounts are supported by default? Needs investigation.
            try
            {
                var accounts = await _clientApp.GetAccountsAsync();
                return await _clientApp.AcquireTokenSilent(_scopes, accounts.FirstOrDefault()).ExecuteAsync();
            }
            catch (MsalUiRequiredException)
            {
                return null;
            }
        }

        private async Task<AuthenticationResult?> TryAcquireTokenViaLogin(CancellationTokenSource cancellationTokenSource)
        {
            if (LoginMethod == LoginMethod.Interactive)
            {
                var builder = _clientApp.AcquireTokenInteractive(_scopes)
                    .WithPrompt(Microsoft.Identity.Client.Prompt.SelectAccount);

                builder = Ioc.Default.GetRequiredService<ISharedFactory>().WithUnoHelpers(builder);

                return await builder.ExecuteAsync(cancellationTokenSource.Token);
            }

            if (LoginMethod == LoginMethod.DeviceCode)
            {
                var builder = _clientApp.AcquireTokenWithDeviceCode(_scopes, dcr =>
                {
                    _coreConfig.DisplayDeviceCodeResult(dcr, cancellationTokenSource);
                    return Task.CompletedTask;
                });

                return await builder.ExecuteAsync(cancellationTokenSource.Token);
            }

            return ThrowHelper.ThrowArgumentOutOfRangeException<AuthenticationResult?>("Invalid login method specified");
        }

        private static GraphServiceClient CreateGraphClient(string accessToken)
        {
            var httpHandler = Ioc.Default.GetRequiredService<ISharedFactory>().GetPlatformSpecificHttpClientHandler();

            var authProvider = new DelegateAuthenticationProvider(requestMessage =>
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", accessToken);
                return Task.CompletedTask;
            });

            var handlers = GraphClientFactory.CreateDefaultHandlers(authProvider);
            var httpClient = GraphClientFactory.Create(handlers, finalHandler: httpHandler);

            return new GraphServiceClient(httpClient);
        }
    }
}
