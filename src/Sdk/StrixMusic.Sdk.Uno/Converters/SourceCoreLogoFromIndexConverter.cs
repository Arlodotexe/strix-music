using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Toolkit.Diagnostics;
using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Sdk.Uno.Converters
{
    /// <summary>
    /// Gets a logo from a specific core in a collection of source cores.
    /// </summary>
    public sealed class SourceCoreLogoFromIndexConverter
    {
        /// <inheritdoc cref="SourceCoreLogoFromIndexConverter"/>
        public static Uri? Convert(object value, int index)
        {
            if (value is IEnumerable<ICore> cores)
            {
                return cores.ElementAtOrDefault(index)?.CoreConfig.LogoSvgUrl;
            }

            return ThrowHelper.ThrowInvalidOperationException<Uri>(nameof(value));
        }
    }
}