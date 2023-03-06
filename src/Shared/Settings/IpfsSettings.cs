using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using OwlCore.ComponentModel;
using OwlCore.Kubo;
using OwlCore.Storage;
using StrixMusic.AppModels;
using StrixMusic.Plugins;

namespace StrixMusic.Settings;

/// <summary>
/// A container for all settings related to Ipfs.
/// </summary>
public partial class IpfsSettings : SettingsBase
{
    [JsonIgnore]
    private readonly SemaphoreSlim _saveLoadMutex = new(1, 1);

    /// <summary>
    /// Initializes a new instance of the <see cref="IpfsSettings"/> class.
    /// </summary>
    /// <param name="folder">The folder where settings are stored.</param>
    public IpfsSettings(IModifiableFolder folder)
        : base(folder, AppSettingsSerializer.Singleton)
    {
    }

    /// <summary>
    /// Gets or sets a value that indicates if Ipfs is enabled.
    /// </summary>
    public bool Enabled
    {
        get => GetSetting(() => false);
        set => SetSetting(value);
    }

    /// <summary>
    /// Gets or sets a value that indicates if the user has been shown a firewall warning when starting IPFS.
    /// </summary>
    public bool FirewallWarningDisplayed
    {
        get => GetSetting(() => false);
        set => SetSetting(value);
    }

    /// <summary>
    /// Gets or sets a custom Kubo RPC API url set by the user.
    /// </summary>
    public int NodeApiPort
    {
        get => GetSetting(() => 5001);
        set => SetSetting(value);
    }

    /// <summary>
    /// Gets or sets a custom Kubo RPC API url set by the user.
    /// </summary>
    public int NodeGatewayPort
    {
        get => GetSetting(() => 8080);
        set => SetSetting(value);
    }

    /// <summary>
    /// Gets or sets the name of the downloaded kubo binary, if any.
    /// </summary>
    public string DownloadKuboBinaryFileId
    {
        get => GetSetting(() => string.Empty);
        set => SetSetting(value);
    }

    /// <summary>
    /// Gets or sets a value that indicates if the <see cref="GlobalPlaybackStateCountPlugin"/> should be enabled.
    /// </summary>
    /// <remarks>This option is not currently used. Needs further research.</remarks>
    public bool GlobalPlaybackStateCountPluginEnabled
    {
        get => GetSetting(() => false);
        set => SetSetting(value);
    }

    /// <summary>
    /// An IPNS address where releases are published. Used to help rehost content, check for updates, and more.
    /// </summary>
    public string ReleaseIpns
    {
        get => GetSetting(() => Environment.GetEnvironmentVariable(nameof(ReleaseIpns)) ?? "/ipns/latest.strixmusic.com");
        set => SetSetting(value);
    }

    /// <summary>
    /// The last known resolved CID of <see cref="ReleaseIpns" />.
    /// </summary>
    public string ReleaseIpnsResolved
    {
        get => GetSetting(() => string.Empty);
        set => SetSetting(value);
    }

    /// <summary>
    /// The app release content bundles that the user has chosen to store on their local node.
    /// </summary>
    public List<AppReleaseContentBundle> ReleaseContentBundles
    {
        get => GetSetting(() => new List<AppReleaseContentBundle>());
        set => SetSetting(value);
    }

    /// <summary>
    /// Specified a routing mode for the Kubo DHT.
    /// </summary>
    public DhtRoutingMode BootstrapNodeDhtRouting
    {
        get => GetSetting(() => DhtRoutingMode.Dht);
        set
        {
            // Two-way binding this to a ComboBox.SelectedItem causes a StackOverflow without this check.
            if (value != BootstrapNodeDhtRouting)
                SetSetting(value);
        }
    }

    /// <summary>
    /// Gets or sets a value that indicates if the bootstrapped node should enable discovery of devices on the local network.
    /// </summary>
    public bool BootstrapNodeEnableLocalDiscovery
    {
        get => GetSetting(() => true);
        set => SetSetting(value);
    }

    /// <inheritdoc />
    [RelayCommand]
    public override async Task SaveAsync(CancellationToken? cancellationToken = null) => await base.SaveAsync(cancellationToken);

    /// <inheritdoc />
    [RelayCommand]
    public override async Task LoadAsync(CancellationToken? cancellationToken = null) => await base.LoadAsync(cancellationToken);
}
