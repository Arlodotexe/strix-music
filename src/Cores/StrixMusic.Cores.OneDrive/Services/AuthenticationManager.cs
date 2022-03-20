using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using StrixMusic.Sdk.Services;
using Microsoft.Toolkit.Diagnostics;
using StrixMusic.Sdk.Helpers;
using OwlCore.Extensions;
using TaskStatus = System.Threading.Tasks.TaskStatus;

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
        private readonly ILogger<AuthenticationManager> _logger;

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
            _logger = Ioc.Default.GetRequiredService<ILogger<AuthenticationManager>>();

#warning Remove platform helper dependency
            LoginMethod = PlatformHelper.Current switch
            {
                Platform.UWP => LoginMethod.DeviceCode,
                Platform.WASM => LoginMethod.Interactive,
                Platform.Droid => LoginMethod.Interactive,
                Platform.Unknown => LoginMethod.None,
                _ => ThrowHelper.ThrowNotSupportedException<LoginMethod>($"Current platform {PlatformHelper.Current} not supported."),
            };

            _logger.LogInformation($"Creating {nameof(PublicClientApplicationBuilder)}");

            var authority = new Uri($"{_authorityUri}/{tenantId}");

            var builder = PublicClientApplicationBuilder
                .Create(clientId)
                .WithAuthority(authority, false);

            if (!string.IsNullOrWhiteSpace(redirectUri))
                builder.WithRedirectUri(redirectUri);

            _logger.LogInformation($"Adding uno helpers to {nameof(PublicClientApplicationBuilder)}");
            builder = Ioc.Default.GetRequiredService<ISharedFactory>().WithUnoHelpers(builder);

            _logger.LogInformation($"Building {nameof(IPublicClientApplication)}");
            _clientApp = builder.Build();
        }

        /// <summary>
        /// Generate a <see cref="GraphServiceClient"/> and handles login if needed.
        /// </summary>
        /// <returns></returns>
        public async Task<GraphServiceClient?> TryLoginAsync(CancellationToken? cancellationToken = null)
        {
            var cancellationTokenInternal = cancellationToken ?? CancellationToken.None;

            _logger.LogInformation($"Entered {nameof(TryLoginAsync)}");

            var authenticationResult = await TryAcquireCachedTokenAsync();

            if (authenticationResult is null)
                authenticationResult = await TryAcquireTokenViaLoginAsync(cancellationTokenInternal);

            if (authenticationResult is null)
                return null;

            var graphClient = CreateGraphClient(authenticationResult.AccessToken);

            _logger.LogInformation($"Got username: {authenticationResult.Account.Username}");
            EmailAddress = authenticationResult.Account.Username;

            DisplayName = await GetDisplayNameAsync(graphClient);

            _logger.LogInformation($"{nameof(TryLoginAsync)} completed.");
            return graphClient;
        }

        private async Task<string> GetDisplayNameAsync(GraphServiceClient graphClient)
        {
            try
            {
                _logger.LogInformation($"Getting user");
                var user = await graphClient.Users.Request().GetAsync();

                if (user.Count == 0)
                {
                    _logger.LogInformation($"No available users");
                    return string.Empty;
                }

                _logger.LogInformation($"Got user. Display name {user.FirstOrDefault()?.DisplayName}");
                return user[0].DisplayName;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get user: {ex}");
                return string.Empty;
            }
        }

        private async Task<AuthenticationResult?> TryAcquireCachedTokenAsync()
        {
            _logger.LogInformation($"Acquiring token from cache.");

            // TODO: Cache and encrypt the token ourselves, and add manual token refreshing.
            // Using AcquireTokenSilent means we're restricted to only 1 account per device.
            // Maybe multiple accounts are supported by default? Needs investigation.
            try
            {
                _logger.LogInformation($"Getting accounts");
                var accounts = await _clientApp.GetAccountsAsync();

                _logger.LogInformation($"Executing via {nameof(_clientApp.AcquireTokenSilent)}");
                return await _clientApp.AcquireTokenSilent(_scopes, accounts.FirstOrDefault()).ExecuteAsync();
            }
            catch (MsalUiRequiredException)
            {
                return null;
            }
        }

        private async Task<AuthenticationResult?> TryAcquireTokenViaLoginAsync(CancellationToken? cancellationToken = null)
        {
            var cancellationTokenInternal = cancellationToken ?? CancellationToken.None;

            if (LoginMethod == LoginMethod.Interactive)
            {
                _logger.LogInformation($"Acquiring token via interactive login");

                _logger.LogInformation($"Building via {nameof(_clientApp.AcquireTokenInteractive)}");
                var builder = _clientApp.AcquireTokenInteractive(_scopes)
                    .WithPrompt(Microsoft.Identity.Client.Prompt.SelectAccount);

                _logger.LogInformation($"Adding Uno helpers");
                builder = Ioc.Default.GetRequiredService<ISharedFactory>().WithUnoHelpers(builder);

                _logger.LogInformation($"Executing builder");

                return await builder.ExecuteAsync(cancellationTokenInternal);
            }

            if (LoginMethod == LoginMethod.DeviceCode)
            {
                // Create a token source we can cancel, that also cancels when the passed token is canceled.
                var userCancellableTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationTokenInternal);
                var builderCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationTokenInternal);

                _logger.LogInformation($"Acquiring token via device code");

                _logger.LogInformation($"Building via {nameof(_clientApp.AcquireTokenWithDeviceCode)}");
                var builder = _clientApp.AcquireTokenWithDeviceCode(_scopes, dcr =>
                {
                    _logger.LogInformation($"Displaying device code result");
                    _coreConfig.DisplayDeviceCodeResult(dcr, userCancellableTokenSource);

                    return Task.CompletedTask;
                });

                _logger.LogInformation($"Executing builder");

                // canceledTask will complete first if the token is cancelled by the user.
                // builderTask will complete first if setup is complete.
                var builderTask = builder.ExecuteAsync(builderCancellationTokenSource.Token);
                var canceledTask = userCancellableTokenSource.Token.WhenCancelled();

                await Task.WhenAny(canceledTask, builderTask);

                if (builderTask.IsFaulted)
                    throw builderTask.Exception;

                // Throw if the user cancelled it manually.
                if (userCancellableTokenSource.IsCancellationRequested)
                    userCancellableTokenSource.Token.ThrowIfCancellationRequested();

                // Otherwise, cancel to trigger WhenCancelled cleanup.
                if (!canceledTask.IsCanceled && !userCancellableTokenSource.IsCancellationRequested)
                    userCancellableTokenSource.Cancel();

                // If builder hasn't finished, cancel it.
                if (builderTask.Status != TaskStatus.RanToCompletion)
                    builderCancellationTokenSource.Cancel();

                userCancellableTokenSource.Dispose();
                builderCancellationTokenSource.Dispose();

                return !builderTask.IsCanceled ? builderTask.Result : null;
            }

            return ThrowHelper.ThrowArgumentOutOfRangeException<AuthenticationResult?>("Invalid login method specified");
        }

        private GraphServiceClient CreateGraphClient(string accessToken)
        {
            _logger.LogInformation($"Creating graph client");

            var httpHandler = _coreConfig.SourceCore.Cast<OneDriveCore>().HttpMessageHandler;

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
