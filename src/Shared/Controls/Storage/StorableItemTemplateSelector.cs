using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using CommunityToolkit.Diagnostics;
using OwlCore.Storage;

namespace StrixMusic.Controls.Storage;

/// <summary>
/// When provided an instance of <see cref="IStorable"/>, select a templates for a derived symbol (such as <see cref="IFile"/> and <see cref="IFolder"/>).
/// </summary>
public class StorableItemTemplateSelector : DataTemplateSelector
{
    /// <summary>
    /// The template to use for an <see cref="IFolder"/>.
    /// </summary>
    public DataTemplate? FolderTemplate { get; set; }

    /// <summary>
    /// The template to use for an <see cref="IFile"/>.
    /// </summary>
    public DataTemplate? FileTemplate { get; set; }

    /// <inheritdoc />
    override protected DataTemplate SelectTemplateCore(object item, DependencyObject container)
    {
        if (item is IFile)
            return FileTemplate ?? ThrowHelper.ThrowArgumentNullException<DataTemplate>();

        if (item is IFolder)
            return FolderTemplate ?? ThrowHelper.ThrowArgumentNullException<DataTemplate>();

        return base.SelectTemplateCore(item, container);
    }
}
