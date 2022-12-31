using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OwlCore.Extensions;
using StrixMusic.AppModels;
using OwlCore.Storage;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Settings;
using StrixMusic.Sdk.Extensions;

namespace StrixMusic.Controls.Settings.MusicSources;

/// <summary>
/// A control to view available music source and add new music sources.
/// </summary>
[ObservableObject]
public sealed partial class MusicSourcesSettingsEditor : UserControl
{
    private readonly SynchronizationContext _syncContext;

    /// <summary>
    /// Creates a new instance of <see cref="MusicSourcesSettingsEditor"/>.
    /// </summary>
    public MusicSourcesSettingsEditor()
    {
        InitializeComponent();
        _syncContext = SynchronizationContext.Current;
    }

    /// <summary>
    /// The backing dependency property for <see cref="AppRoot"/>.
    /// </summary>
    public static readonly DependencyProperty AppRootProperty =
        DependencyProperty.Register(nameof(AppRoot), typeof(AppRoot), typeof(MusicSourcesSettingsEditor), new PropertyMetadata(null));

    /// <summary>
    /// The configured app instance for displaying configured cores.
    /// </summary>
    public AppRoot? AppRoot
    {
        get => (AppRoot?)GetValue(AppRootProperty);
        set => SetValue(AppRootProperty, value);
    }

    private void RemoveMusicSource(ICore core)
    {
        Guard.IsNotNull(AppRoot?.MusicSourcesSettings);

        if (TryRemoveFrom(AppRoot.MusicSourcesSettings.ConfiguredLocalStorageCores, core.InstanceId))
            return;

        if (TryRemoveFrom(AppRoot.MusicSourcesSettings.ConfiguredOneDriveCores, core.InstanceId))
            return;
    }

    private static bool TryRemoveFrom<T>(ObservableCollection<T> collection, string instanceId)
        where T : CoreSettingsBase, IInstanceId
    {
        var target = collection.FirstOrDefault(x => x.InstanceId == instanceId);

        if (target is null)
            return false;

        return collection.Remove(target);
    }

    private async void DeleteMenuFlyoutItem_OnClick(object sender, RoutedEventArgs e)
    {
        // At least one core must stay present in MergedCore.
        // AppRoot is not yet set up to safely deconstruct MergedCore, ViewModels, plugins, etc.
        if (AppRoot?.StrixDataRoot?.Sources.Count == 1)
        {
            Guard.IsNotNull(AppRoot.Diagnostics);

            new ContentDialog
            {
                Title = "App restart required",
                Content = new TextBlock { Text = "This action requires an app restart. Are you sure?" },
                PrimaryButtonText = "Yes, restart",
                PrimaryButtonCommand = new AsyncRelayCommand(async () =>
                {
                    Guard.IsNotNull(AppRoot.MusicSourcesSettings);

                    AppRoot.MusicSourcesSettings.ResetAllSettings();
                    await AppRoot.MusicSourcesSettings.SaveAsync();
                    await AppRoot.Diagnostics.RestartAppCommand.ExecuteAsync(null);
                }),
                CloseButtonText = "Cancel",
            }
            .ShowAsync(ShowType.Interrupt);

            return;
        }

        var menuFlyoutItem = (MenuFlyoutItem)sender;
        var core = (CoreViewModel)menuFlyoutItem.DataContext;

        Guard.IsNotNull(AppRoot?.MergedCore);
        Guard.IsNotNull(AppRoot?.MusicSourcesSettings);

        await new ContentDialog
        {
            Title = "Are you sure?",
            Content = new TextBlock { Text = $"{core.DisplayName} ({core.InstanceDescriptor}) will be removed." },
            CloseButtonText = "Cancel",
            PrimaryButtonText = "Yes",
            PrimaryButtonCommand = new RelayCommand(() => RemoveMusicSource(core)),
        }.ShowAsync(ShowType.QueueNext);
    }

    [RelayCommand]
    private async Task AddNewMusicSourceAsync()
    {
        var param = new ConnectNew.ConnectNewMusicSourceNavigationParams()
        {
            AppRoot = AppRoot,
        };

        ConnectNewSourceFrame.Visibility = Visibility.Visible;

        ConnectNewSourceFrame.Navigate(typeof(ConnectNew.ConnectNewMusicSource), param);

        try
        {
            await param.SetupCompleteTaskCompletionSource.Task;
        }
        catch (OperationCanceledException _)
        {
            // Ignored
        }

        ConnectNewSourceFrame.Visibility = Visibility.Collapsed;
    }

    /// <summary>
    /// Converts the input to the object's Type.
    /// </summary>
    /// <returns>The Type of the given object.</returns>
    public Type ObjectToType(object value) => value.GetType();

    private static bool IsGreaterThanOne(int arg) => arg > 1;

    private Visibility IsZeroToVisibility(int arg) => arg == 0 ? Visibility.Visible : Visibility.Collapsed;

    private Visibility IsNotZeroToVisibility(int arg) => arg != 0 ? Visibility.Visible : Visibility.Collapsed;

    private IFolder? AsFolder(object obj) => obj as IFolder;

    private bool IsFolder(object obj) => obj is IFolder;

    private bool And(bool val1, bool val2) => val1 && val2;

    private bool AndNot(bool val1, bool val2) => !(val1 && val2);

    private bool AllNotNullOrWhiteSpace(string value, string value2) => !string.IsNullOrWhiteSpace(value) && !string.IsNullOrWhiteSpace(value2);

    private bool IsNull(object? obj) => obj is null;

    private bool IsNotNull(object? obj) => obj is not null;

    private Visibility IsNullToVisibility(object? obj) => obj is null ? Visibility.Visible : Visibility.Collapsed;

    private Visibility IsNotNullToVisibility(object? obj) => obj is not null ? Visibility.Visible : Visibility.Collapsed;

    private bool InvertBool(bool val) => !val;

    private Visibility BoolToVisibility(bool val) => val ? Visibility.Visible : Visibility.Collapsed;

    private Visibility InvertBoolToVisibility(bool val) => !val ? Visibility.Visible : Visibility.Collapsed;

    private Uri StringToUri(string value) => new Uri(value);
}
