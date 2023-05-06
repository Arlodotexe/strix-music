using System;
using Windows.UI.Xaml.Markup;

namespace OwlCore.Extensions;

[MarkupExtensionReturnType(ReturnType = typeof(Type))]
public sealed class TypeExtension : MarkupExtension
{
    public string? Fullname { get; set; }

    /// <inheritdoc/>
    protected override object? ProvideValue() => Fullname is null ? null : Type.GetType(Fullname);
}
