using System;
using Android.Runtime;
using Com.Nostra13.Universalimageloader.Core;
using Windows.UI.Xaml.Media;

namespace Strix_Music.Droid
{
    /// <summary>
    /// From the Uno template
    /// </summary>
    [global::Android.App.Application(
        Label = "@string/ApplicationName",
        LargeHeap = true,
        HardwareAccelerated = true,
        Theme = "@style/AppTheme")]
    public class Application : Windows.UI.Xaml.NativeApplication
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Application"/> class.
        /// </summary>
        /// <param name="javaReference">From the Uno template</param>
        /// <param name="transfer">From the Uno template</param>
        public Application(IntPtr javaReference, JniHandleOwnership transfer)
            : base(() => new App(), javaReference, transfer)
        {
            ConfigureUniversalImageLoader();
        }

        private void ConfigureUniversalImageLoader()
        {
            // Create global configuration and initialize ImageLoader with this config
            ImageLoaderConfiguration config = new ImageLoaderConfiguration
                .Builder(Context)
                .Build();

            ImageLoader.Instance.Init(config);

            ImageSource.DefaultImageLoader = ImageLoader.Instance.LoadImageAsync;
        }
    }
}
