using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using OwlCore.Extensions;
using StrixMusic.Sdk.Services;
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
        private readonly ILogger<AuthenticationManager> _logger;

        /// <summary>
        /// Creates a new instance of <see cref="AuthenticationManager"/>.
        /// </summary>
        /// <param name="clientId">Client ID of a registered MS Graph application that the user will be authenticated against.</param>
        /// <param name="tenantId">Tenant ID for authenticating the user against a registered MS Graph application</param>
        /// <param name="redirectUri">The redirect URI to use with the connected application, if any.</param>
        public AuthenticationManager(string clientId, string tenantId, string? redirectUri = null)
        {
            _logger = Ioc.Default.GetRequiredService<ILogger<AuthenticationManager>>();

            _logger.LogInformation($"Creating {nameof(PublicClientApplicationBuilder)}");

            var authority = new Uri($"{_authorityUri}/{tenantId}");

            var builder = PublicClientApplicationBuilder
                .Create(clientId)
                .WithAuthority(authority, false);

            if (!string.IsNullOrWhiteSpace(redirectUri))
                builder.WithRedirectUri(redirectUri);

            _logger.LogInformation($"Adding uno helpers to {nameof(PublicClientApplicationBuilder)}");
#warning TODO remove UnoHelpers dependency.
            builder = Ioc.Default.GetRequiredService<ISharedFactory>().WithUnoHelpers(builder);

            _logger.LogInformation($"Building {nameof(IPublicClientApplication)}");
            _clientApp = builder.Build();
        }

        /// <summary>
        /// A custom message handler to use for network requests during authentication.
        /// </summary>
        public HttpMessageHandler HttpMessageHandler { get; set; } = new HttpClientHandler();

        public GraphServiceClient CreateGraphClient(string accessToken)
        {
            _logger.LogInformation($"Creating graph client");

            var authProvider = new DelegateAuthenticationProvider(requestMessage =>
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", accessToken);
                return Task.CompletedTask;
            });

            var handlers = GraphClientFactory.CreateDefaultHandlers(authProvider);
            var httpClient = GraphClientFactory.Create(handlers, finalHandler: HttpMessageHandler);

            return new GraphServiceClient(httpClient);
        }

        public async Task<string> GetDisplayNameAsync(GraphServiceClient graphClient)
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

        public async Task<AuthenticationResult?> TryAcquireCachedTokenAsync(string accountIdentifier, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Acquiring token from cache.");

            if (string.IsNullOrWhiteSpace(accountIdentifier))
                return null;

            try
            {
                _logger.LogInformation($"Getting accounts");
                var account = await _clientApp.GetAccountAsync(accountIdentifier);

                _logger.LogInformation($"Executing via {nameof(_clientApp.AcquireTokenSilent)}");
                return await _clientApp.AcquireTokenSilent(_scopes, account).ExecuteAsync(cancellationToken);
            }
            catch (MsalUiRequiredException)
            {
                return null;
            }
        }

        public Task<AuthenticationResult?> TryAcquireTokenViaInteractiveLoginAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Acquiring token via interactive login");

            _logger.LogInformation($"Building via {nameof(_clientApp.AcquireTokenInteractive)}");
            var builder = _clientApp.AcquireTokenInteractive(_scopes)
                .WithPrompt(Microsoft.Identity.Client.Prompt.SelectAccount);

            _logger.LogInformation($"Adding Uno helpers");
            builder = Ioc.Default.GetRequiredService<ISharedFactory>().WithUnoHelpers(builder);

            _logger.LogInformation($"Executing builder");

            return builder.ExecuteAsync(cancellationToken);
        }

        public async Task<AuthenticationResult?> TryAcquireTokenViaDeviceCodeLoginAsync(Func<DeviceCodeResult, Task> deviceCodeResultCallback, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Acquiring token via device code");

            _logger.LogInformation($"Building via {nameof(_clientApp.AcquireTokenWithDeviceCode)}");
            var builder = _clientApp.AcquireTokenWithDeviceCode(_scopes, deviceCodeResultCallback);

            _logger.LogInformation($"Executing builder");
            
            try
            {
                return await builder.ExecuteAsync(cancellationToken);
            }
            catch (MsalServiceException ex)
            {
                return null;
            }
        }
    }
}
