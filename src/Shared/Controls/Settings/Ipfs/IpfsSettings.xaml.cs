using System;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using OwlCore.Kubo;
using StrixMusic.AppModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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

    private bool And(bool a, bool b) => a && b;

    private Visibility And_InvertSecondAndThirdBool_ToVisibility(bool a, bool b, bool c) => BoolToVisibility(a && !b && c);
}
