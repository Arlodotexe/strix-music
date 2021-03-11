using System;
using System.Linq;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.ViewModels;

// ReSharper disable CheckNamespace
namespace StrixMusic.Sdk
{
    public partial class MainViewModel
    {
        /// <summary>
        /// Gets a loaded core using the <see cref="MainViewModel"/> from the default <see cref="Ioc"/>.
        /// </summary>
        /// <param name="reference">The core to look for.</param>
        /// <returns>The loaded, observable core.</returns>
        public static CoreViewModel GetLoadedCore(ICore reference)
        {
            return Ioc.Default.GetRequiredService<MainViewModel>().Cores.First(x => x.InstanceId == reference.InstanceId);
        }
    }
}
