using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Kiota.Abstractions.Authentication;

namespace StrixMusic.AppModels;

/// <summary>
/// Delegates an access token for OneDrive login.
/// </summary>
public class OneDriveAccessTokenProvider : IAccessTokenProvider
{
    private readonly string _token;

    /// <summary>
    /// Creates a new instance of <see cref="OneDriveAccessTokenProvider"/>.
    /// </summary>
    /// <param name="token">The token to provide.</param>
    public OneDriveAccessTokenProvider(string token)
    {
        _token = token;
    }

    /// <inheritdoc />
    public Task<string> GetAuthorizationTokenAsync(Uri uri, Dictionary<string, object>? additionalAuthenticationContext = default, CancellationToken cancellationToken = default) => Task.FromResult(_token);

    /// <inheritdoc/>
    public AllowedHostsValidator AllowedHostsValidator => new();
}
