namespace StrixMusic.Sdk.Services.Localization
{
    /// <summary>
    /// A Service for getting localized strings from resource providers.
    /// </summary>
    public interface ILocalizationService
    {
        /// <summary>
        /// Gets the localized <see cref="string"/> for a resource key.
        /// </summary>
        /// <param name="provider">The resource loader to retrieve the resource from.</param>
        /// <param name="key">The key to identify the resource.</param>
        /// <returns>The localized <see cref="string"/> for a resource key</returns>
        string this[string provider, string key] { get; }

        /// <summary>
        /// Localizes a string to a user-friendly value if <paramref name="value"/> is null or empty.
        /// </summary>
        /// <param name="value">The string to conditionally localize.</param>
        /// <param name="provider">The resource loader to retrieve the resource from.</param>
        /// <param name="key">The key to identify the resource.</param>
        /// <returns>The localized string.</returns>
        string LocalizeIfNullOrEmpty(string value, string provider, string key);

        /// <summary>
        /// Localizes a string to a user-friendly value if <paramref name="value"/> is null or empty.
        /// </summary>
        /// <param name="value">The string to conditionally localize.</param>
        /// <param name="sender">The data that <paramref name="value"/> was pulled from.</param>
        /// <returns>The localized string.</returns>
        string LocalizeIfNullOrEmpty<T>(string value, T sender);
    }
}
