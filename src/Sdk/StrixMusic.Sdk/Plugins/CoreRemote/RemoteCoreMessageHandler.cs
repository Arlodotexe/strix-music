// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using OwlCore.Remoting;
using OwlCore.Remoting.Transfer;
using OwlCore.Remoting.Transfer.MessageConverters;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Plugins.CoreRemote
{
    /// <summary>
    /// The remote message handler use for external cores.
    /// </summary>
    public sealed class RemoteCoreMessageHandler : IRemoteMessageHandler
    {
        /// <summary>
        /// The message handler used when we have the actual core implementation.
        /// </summary>
        public static RemoteCoreMessageHandler SingletonHost { get; } = new RemoteCoreMessageHandler(RemotingMode.Host);

        /// <summary>
        /// The message handler used when the core implementation is elsewhere and we want to remote control it.
        /// </summary>
        public static RemoteCoreMessageHandler SingletonClient { get; } = new RemoteCoreMessageHandler(RemotingMode.Client);

        /// <summary>
        /// Creates a new instance of <see cref="RemoteCoreMessageHandler"/>.
        /// </summary>
        /// <param name="remotingMode">The mode to use for this handler.
        /// If <see cref="RemotingMode.Host"/>, it's assumed we have the actual core implementation, else we're a <see cref="RemotingMode.Client"/> and are controlling a remote core.
        /// </param>
        public RemoteCoreMessageHandler(RemotingMode remotingMode)
        {
            Mode = remotingMode;
        }

        /// <inheritdoc/>
        public RemotingMode Mode { get; set; }

        /// <inheritdoc/>
        public MemberSignatureScope MemberSignatureScope { get; set; } = MemberSignatureScope.AssemblyQualifiedName;

        /// <inheritdoc/>
        public IRemoteMessageConverter MessageConverter { get; } = new NewtonsoftRemoteMessageConverter();

        /// <inheritdoc/>
        public bool IsInitialized { get; private set; }

        /// <inheritdoc/>
        public event EventHandler<IRemoteMessage>? MessageReceived;

        /// <inheritdoc/>
        public Task InitAsync()
        {
            IsInitialized = true;
            return Task.CompletedTask;
        }

        /// <summary>
        /// Digests an incoming remote message.
        /// </summary>
        /// <param name="message">The message to digest.</param>
        public void DigestMessageAsync(IRemoteMessage message) => MessageReceived?.Invoke(this, message);

        /// <summary>
        /// Raised when an outbound message is created.
        /// </summary>
        public event EventHandler<IRemoteMessage>? MessageOutbound;

        /// <inheritdoc/>
        public Task SendMessageAsync(IRemoteMessage memberMessage, CancellationToken? cancellationToken = null)
        {
            MessageOutbound?.Invoke(this, memberMessage);
            return Task.CompletedTask;
        }
    }
}