// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Threading.Tasks;
using OwlCore.Remoting;
using StrixMusic.Sdk.CoreModels;

namespace StrixMusic.Sdk.Plugins.CoreRemote
{
    /// <summary>
    /// An external, remotely synchronized implementation of <see cref="ICoreImage"/>
    /// </summary>
    public sealed class RemoteCoreImage : ICoreImage
    {
        /// <summary>
        /// Creates a new instance of <see cref="RemoteCoreLibrary"/>. Interacts with a remote core, identified by the given parameters.
        /// </summary>
        /// <param name="sourceCoreInstanceId">The ID of the core that created this instance.</param>
        /// <param name="id">Uniquely identifies the instance being remoted.</param>
        internal RemoteCoreImage(string sourceCoreInstanceId, string id)
        {
            SourceCore = RemoteCore.GetInstance(sourceCoreInstanceId, RemotingMode.Client);
            Uri = new Uri("https://strixmusic.com/favicon.ico");
        }

        /// <summary>
        /// Creates a new instance of <see cref="RemoteCoreImage"/>. Wraps around the given <paramref name="image"/> for remote interaction.
        /// </summary>
        /// <param name="image">The image to control remotely.</param>
        internal RemoteCoreImage(ICoreImage image)
        {
            SourceCore = RemoteCore.GetInstance(image.SourceCore.InstanceId, RemotingMode.Host);
            Uri = image.Uri;
            Height = image.Height;
            Width = image.Width;
        }

        /// <inheritdoc />
        public ICore SourceCore { get; }

        /// <inheritdoc />
        [RemoteProperty]
        public Uri Uri { get; set; }

        /// <inheritdoc />
        [RemoteProperty]
        public double Height { get; set; }

        /// <inheritdoc />
        [RemoteProperty]
        public double Width { get; set; }

        /// <inheritdoc />
        public ValueTask DisposeAsync() => new ValueTask(Task.Run(() =>
        {
            return Task.CompletedTask;
        }));
    }
}
