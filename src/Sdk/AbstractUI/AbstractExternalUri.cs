using System;

namespace StrixMusic.Sdk.AbstractUI
{
    /// <summary>
    /// Presents a link to the user.
    /// </summary>
    /// <remarks>This can be displayed in the UI however it wants (Button, text link, Icons, custom, etc)</remarks>
    public class AbstractExternalUri : AbstractUIMetadata
    {
        /// <summary>
        /// Creates a new instance of <see cref="AbstractExternalUri"/>.
        /// </summary>
        /// <param name="id"><inheritdoc cref="AbstractUIBase.Id"/></param>
        /// <param name="externalUri"><inheritdoc cref="ExternalUri"/></param>
        public AbstractExternalUri(string id, Uri externalUri)
            : base(id)
        {
            ExternalUri = externalUri;
        }

        /// <summary>
        /// A link pointing to a web resource.
        /// </summary>
        public Uri ExternalUri { get; protected set; }
    }
}
