using CommunityToolkit.Diagnostics;
using StrixMusic.Settings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Controls.MusicSources;

/// <summary>
/// Selects the DataTemplate to control editing a specific settings implementation.
/// </summary>
internal sealed class MusicSourceSetupTemplateSelector : DataTemplateSelector
{
    /// <summary>
    /// The template to use for <see cref="LocalStorageCoreSettings"/>.
    /// </summary>
    public DataTemplate? LocalStorageTemplate { get; set; }

    /// <summary>
    /// Selects the DataTemplate to control editing a specific settings implementation.
    /// </summary>
    /// <returns>The data template that was selected for the provided data.</returns>
    protected override DataTemplate SelectTemplateCore(object item, DependencyObject container) => item switch
    {
        LocalStorageCoreSettings => LocalStorageTemplate ?? base.SelectTemplateCore(item, container),
        _ => ThrowHelper.ThrowNotSupportedException<DataTemplate>($"Settings template has not been set up for {item.GetType()}"),
    };
}
