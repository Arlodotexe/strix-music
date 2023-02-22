using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using OwlCore.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using Windows.UI.Xaml.Input;

namespace StrixMusic.Controls.Storage;

/// <summary>
/// Provided an <see cref="IFolder"/>, allows the user to browse the contents.
/// </summary>
[ObservableObject]
public sealed partial class FolderBrowser : UserControl
{
    [ObservableProperty] private IStorable? _selectedItem;

    /// <summary>
    /// The backing dependency property for <see cref="InitialFolder"/>.
    /// </summary>
    public static readonly DependencyProperty InitialFolderProperty =
        DependencyProperty.Register(nameof(InitialFolder), typeof(IFolder), typeof(FolderBrowser), new PropertyMetadata(null));

    /// <summary>
    /// The backing dependency property for <see cref="CurrentFolder"/>.
    /// </summary>
    public static readonly DependencyProperty CurrentFolderProperty =
        DependencyProperty.Register(nameof(CurrentFolder), typeof(IFolder), typeof(FolderBrowser), new PropertyMetadata(null, (d, e) => _ = ((FolderBrowser)d).OnCurrentFolderChanged(e.OldValue as IFolder, e.NewValue as IFolder)));

    /// <summary>
    /// Creates a new instance of <see cref="FolderBrowser"/>.
    /// </summary>
    public FolderBrowser()
    {
        this.InitializeComponent();
    }

    /// <summary>
    /// The items that are in the <see cref="CurrentFolder"/>.
    /// </summary>
    internal ObservableCollection<IStorable> CurrentFolderItems { get; } = new();

    /// <summary>
    /// The initial folder to display to the user for browsing.
    /// </summary>
    public IFolder? InitialFolder
    {
        get => (IFolder?)GetValue(InitialFolderProperty);
        set
        {
            SetValue(InitialFolderProperty, value);
            SetValue(CurrentFolderProperty, value);
        }
    }

    /// <summary>
    /// The folder that the user is currently viewing.
    /// </summary>
    public IFolder? CurrentFolder
    {
        get => (IFolder?)GetValue(CurrentFolderProperty);
        set
        {
            if (value is IFolder)
                SetValue(CurrentFolderProperty, value);
        }
    }

    private async Task OnCurrentFolderChanged(IFolder? oldValue, IFolder? newValue)
    {
        if (oldValue is not null)
            CurrentFolderItems.Clear();

        if (newValue is null)
            return;

        await foreach (var item in newValue.GetItemsAsync())
            CurrentFolderItems.Add(item);
    }

    [RelayCommand(FlowExceptionsToTaskScheduler = true, IncludeCancelCommand = true)]
    private async Task GoToParentAsync(CancellationToken cancellationToken)
    {
        if (CurrentFolder is not IStorableChild addressableStorable)
        {
            ThrowHelper.ThrowArgumentException("Current folder is not addressable.");
            return;
        }

        var parent = await addressableStorable.GetParentAsync(cancellationToken);
        CurrentFolder = parent;
    }

    private string? AsAddressableStorablePath(object obj) => (obj as IStorableChild)?.Path;

    private bool IsAddressableStorable(object obj) => obj is IStorableChild;

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

    private bool IsZero(int value) => value == 0;

    private Visibility IsZeroToVisibility(int value) => BoolToVisibility(IsZero(value));

    private void FolderGrid_OnDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
    {
        var gridView = (GridView)sender;

        if (gridView.SelectedItem is IFolder folder)
            CurrentFolder = folder;
    }
}
