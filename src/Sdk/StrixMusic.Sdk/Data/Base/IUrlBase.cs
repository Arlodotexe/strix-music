using System;

namespace StrixMusic.Sdk.Data.Base
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
        /// Classifies this as a specific type of URL.
        /// </summary>
        public UrlType Type { get; }
    }

    public enum UrlType
    {
        /// <summary>
        /// 
        /// </summary>
        Generic,

        /// <summary>
        /// 
        /// </summary>
        Social,

        // TODO
    }
}