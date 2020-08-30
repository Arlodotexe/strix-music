// Special thanks to Sergio Pedri for this design from Legere
// Sergio's GitHub: https://github.com/Sergio0694
// Legere: https://www.microsoft.com/store/apps/9PHJRVCSKVJZ

namespace StrixMusic.Services.Settings
{
    /// <summary>
    /// The instance of <see cref="ISettingsService"/> used by default
    /// <remarks>Not used by Cores. User-configurable settings go here.</remarks>
    /// </summary>
    public class DefaultSettingsService : SettingsServiceBase
    {
        /// <inheritdoc/>
        public override string Id => "Default";
    }
}
