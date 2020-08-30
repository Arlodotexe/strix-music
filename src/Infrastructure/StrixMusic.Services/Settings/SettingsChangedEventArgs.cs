namespace StrixMusic.Services.Settings
{
    /// <summary>
    /// Holds information about a changed setting.
    /// </summary>
    public class SettingChangedEventArgs
    {
        /// <summary>
        /// The identifier for this setting
        /// </summary>
        public string? Key { get; set; }

        /// <summary>
        /// The value of the new setting.
        /// </summary>
        public object? Value { get; set; }
    }
}
