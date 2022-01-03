using System;

namespace StrixMusic.Sdk.Models.Base
{
    /// <summary>
    /// Holds details about a url.
    /// </summary>
    public interface IUrlBase : ICollectionItemBase
    {
        /// <summary>
        /// A label for the <see cref="Url"/>.
        /// </summary>
        public string Label { get; }

        /// <summary>
        /// A <see cref="Uri"/> pointing to an external resource.
        /// </summary>
        public Uri Url { get; }

        /// <summary>
        /// Classifies this as a specific kind of URL.
        /// </summary>
        public UrlType Type { get; }
    }
}