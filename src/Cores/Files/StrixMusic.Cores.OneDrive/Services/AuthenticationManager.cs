using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using Logger = OwlCore.Services.Logger;

namespace StrixMusic.Cores.OneDrive.Services
{
    /// <summary>
    /// Manages MSAL authentication.
    /// </summary>
    public class AuthenticationManager
    {
        private readonly string _authorityUri = "https://login.microsoftonline.com/consumers";
        private readonly string[] _scopes = { "Files.Read.All", "User.Read", "Files.ReadWrite" };
        private readonly string _clientId;
        private readonly string _tenantId;
        private readonly string? _redirectUri;

        /// <summary>
        /// Creates a new instance of <see cref="AuthenticationManager"/>.
        /// </summary>
        /// <param name="clientId">Client ID of a registered MS Graph application that the user will be authenticated against.</param>
        /// <param name="tenantId">Tenant ID for authenticating the user against a registered MS Graph application</param>
        /// <param name="redirectUri">The redirect URI to use with the connected application, if any.</param>
        public AuthenticationManager(string clientId, string tenantId, string? redirectUri = null)
        {
            _clientId = clientId;
            _tenantId = tenantId;
            _redirectUri = redirectUri;
        }

        /// <summary>
        /// A custom message handler to use for network requests during authentication.
        /// </summary>
        public HttpMessageHandler HttpMessageHandler { get; set; } = new HttpClientHandler();

        /// <inheritdoc cref="MsalPublicClientApplicationBuilderCreatedEventArgs" />
        public event EventHandler<MsalPublicClientApplicationBuilderCreatedEventArgs>? MsalPublicClientApplicationBuilderCreated;

        /// <inheritdoc cref="AcquireTokenInteractiveParameterBuilderCreatedEventArgs" />
        public event EventHandler<AcquireTokenInteractiveParameterBuilderCreatedEventArgs>? MsalAcquireTokenInteractiveParameterBuilderCreated;

        public GraphServiceClient CreateGraphClient(string accessToken)
        {
            Logger.LogInformation($"Creating graph client");

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
                Logger.LogInformation($"Getting user");
                var user = await graphClient.Users.Request().GetAsync();

                if (user.Count == 0)
                {
                    Logger.LogInformation($"No available users");
                    return string.Empty;
                }

                Logger.LogInformation($"Got user. Display name {user.FirstOrDefault()?.DisplayName}");
                return user[0].DisplayName;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Failed to get user: {ex}");
                return string.Empty;
            }
        }

        public async Task<AuthenticationResult?> TryAcquireCachedTokenAsync(string accountIdentifier, CancellationToken cancellationToken = default)
        {
            var clientApp = BuildPublicClientApplication();

            Logger.LogInformation($"Acquiring token from cache.");

            if (string.IsNullOrWhiteSpace(accountIdentifier))
                return null;

            try
            {
                Logger.LogInformation($"Getting accounts");
                var account = await clientApp.GetAccountAsync(accountIdentifier);

                Logger.LogInformation($"Executing via {nameof(clientApp.AcquireTokenSilent)}");
                return await clientApp.AcquireTokenSilent(_scopes, account).ExecuteAsync(cancellationToken);
            }
            catch (MsalUiRequiredException)
            {
                return null;
            }
        }

        public Task<AuthenticationResult?> TryAcquireTokenViaInteractiveLoginAsync(CancellationToken cancellationToken = default)
        {
            var clientApp = BuildPublicClientApplication();
            Logger.LogInformation($"Acquiring token via interactive login");

            Logger.LogInformation($"Building via {nameof(clientApp.AcquireTokenInteractive)}");
            var builder = clientApp.AcquireTokenInteractive(_scopes)
                .WithPrompt(Microsoft.Identity.Client.Prompt.SelectAccount);

            var createdArgs = new AcquireTokenInteractiveParameterBuilderCreatedEventArgs(builder);
            MsalAcquireTokenInteractiveParameterBuilderCreated?.Invoke(this, createdArgs);
            builder = createdArgs.Builder;

            Logger.LogInformation($"Executing builder");

            return builder.ExecuteAsync(cancellationToken);
        }

        public async Task<AuthenticationResult?> TryAcquireTokenViaDeviceCodeLoginAsync(Func<DeviceCodeResult, Task> deviceCodeResultCallback, CancellationToken cancellationToken = default)
        {
            var clientApp = BuildPublicClientApplication();
            Logger.LogInformation($"Acquiring token via device code");

            Logger.LogInformation($"Building via {nameof(clientApp.AcquireTokenWithDeviceCode)}");
            var builder = clientApp.AcquireTokenWithDeviceCode(_scopes, deviceCodeResultCallback);

            Logger.LogInformation($"Executing builder");

            try
            {
                return await builder.ExecuteAsync(cancellationToken);
            }
            catch (MsalServiceException)
            {
                return null;
            }
        }

        private IPublicClientApplication BuildPublicClientApplication()
        {
            var authority = new Uri($"{_authorityUri}/{_tenantId}");

            var builder = PublicClientApplicationBuilder
                .Create(_clientId)
                .WithAuthority(authority, false);

            if (!string.IsNullOrWhiteSpace(_redirectUri))
                builder.WithRedirectUri(_redirectUri);

            var createdArgs = new MsalPublicClientApplicationBuilderCreatedEventArgs(builder);
            MsalPublicClientApplicationBuilderCreated?.Invoke(this, createdArgs);

            return createdArgs.Builder.Build();
        }
    }
}
