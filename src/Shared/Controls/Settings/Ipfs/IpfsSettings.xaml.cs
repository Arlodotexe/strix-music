using System;
using CommunityToolkit.Diagnostics;
using OwlCore.Kubo;
using StrixMusic.AppModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace StrixMusic.Controls.Settings.Ipfs;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class IpfsSettings : UserControl
{
    /// <summary>
    /// Creates a new instance of <see cref="IpfsSettings"/>
    /// </summary>
    public IpfsSettings()
    {
        this.InitializeComponent();
    }

    /// <summary>
    /// The backing dependency property for <see cref="Ipfs"/>.
    /// </summary>
    public static readonly DependencyProperty IpfsProperty =
        DependencyProperty.Register(nameof(Ipfs), typeof(IpfsAccess), typeof(IpfsSettings), new PropertyMetadata(null));

    /// <summary>
    /// A configured <see cref="IpfsAccess"/> instance.
    /// </summary>
    public IpfsAccess? Ipfs
    {
        get => (IpfsAccess?)GetValue(IpfsProperty);
        set => SetValue(IpfsProperty, value);
    }

    private bool InvertBool(bool? b) => !b ?? false;

    private Visibility BoolToVisibility(bool b) => b ? Visibility.Visible : Visibility.Collapsed;

    private Visibility InvertBoolToVisibility(bool? b) => BoolToVisibility(InvertBool(b));

    private string IsNull(object? obj) => obj is null ? "true" : "false";

    private Visibility IsNotNullOrWhiteSpaceToVisibility(string obj) => BoolToVisibility(!string.IsNullOrWhiteSpace(obj));

    private bool And(bool a, bool b) => a && b;

    private string IpfsPathToProtocolUrl(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return "?";

        Guard.IsTrue(path.StartsWith("/ipfs/"), nameof(path), "Path must start with /ipfs/");

        path = path.Split("/ipfs/")[1];

        return $"ipfs://{path}";
    }

    private string IpnsPathToProtocolUrl(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return "?";

        Guard.IsTrue(path.StartsWith("/ipns/"), nameof(path), "Path must start with /ipns/");

        path = path.Split("/ipns/")[1];

        return $"ipns://{path}";
    }

    private string RoutingModeToDescription(DhtRoutingMode mode) => mode switch
    {
        DhtRoutingMode.Dht => "This is the default routing mode. In the normal DHT mode, IPFS can retrieve content from any peer and seed content to other peers outside your local network.",
        DhtRoutingMode.DhtClient => "Ideal for devices with constrained resources. In the \"dhtclient\" mode, IPFS can ask other peers for content, but it will not seed content to peers outside of your local network.",
        _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
    };
}
