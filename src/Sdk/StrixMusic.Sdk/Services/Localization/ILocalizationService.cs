// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

namespace StrixMusic.Sdk.Services
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
    }
}
