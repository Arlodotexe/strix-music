using System;

namespace StrixMusic.Sdk.AbstractUI
{
    /// <summary>
    /// Presents a link to the user.
    /// </summary>
    /// <remarks>This can be displayed in the UI however it wants (Button, text link, Icons, custom, etc)</remarks>
    public interface IAbstractExternalUri : IAbstractUIMetadata
    {
        /// <summary>
        /// A link pointing to a web resource.
        /// </summary>
        public Uri ExternalUri { get; }
    }
}
