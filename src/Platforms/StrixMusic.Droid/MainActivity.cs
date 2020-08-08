using Android.App;
using Android.Views;

namespace StrixMusic.Droid
{
    /// <summary>
    /// From the Uno template
    /// </summary>
    [Activity(
            MainLauncher = true,
            ConfigurationChanges = Uno.UI.ActivityHelper.AllConfigChanges,
            WindowSoftInputMode = SoftInput.AdjustPan | SoftInput.StateHidden)]
    public class MainActivity : Windows.UI.Xaml.ApplicationActivity
    {
    }
}
