using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
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
                var mainVieWModel = Ioc.Default.GetRequiredService<MainViewModel>();
                var relevantCore = cores.ElementAt(index);
                var coreVm = mainVieWModel.Cores.First(x => x.InstanceId == relevantCore.InstanceId);

                return coreVm.LogoSvgUrl;
            }

            return ThrowHelper.ThrowInvalidOperationException<Uri>(nameof(value));
        }
    }
}