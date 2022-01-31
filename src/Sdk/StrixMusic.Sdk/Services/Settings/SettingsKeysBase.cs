namespace StrixMusic.Sdk.Services
{
    /// <summary>
    /// Base class used for all SettingsKeys classes.
    /// </summary>
    public abstract class SettingsKeysBase
    {
        /// <summary>
        /// Gets the default value for a setting
        /// </summary>
        /// <param name="settingKey">The key for the setting</param>
        /// <returns>The default value for the given key.</returns>
        public abstract object GetDefaultValue(string settingKey);
    }
}