using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.DependencyInjection;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Models.Core;
using Windows.UI.Xaml.Data;

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

                return coreVm.LogoUri;
            }

            return ThrowHelper.ThrowInvalidOperationException<Uri>(nameof(value));
        }
    }
}