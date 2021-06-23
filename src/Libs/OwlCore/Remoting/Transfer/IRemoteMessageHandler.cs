using OwlCore.Provisos;
using System;
using System.Threading.Tasks;

namespace OwlCore.Remoting.Transfer
{
    /// <summary>
    /// Handles sending and receiving of serialized <see cref="IRemoteMessage"/>s.
    /// </summary>
    public interface IRemoteMessageHandler : IAsyncInit
    {
        /// <inheritdoc cref="RemotingMode" />
        public RemotingMode Mode { get; set; }

        /// <summary>
        /// Emit a remoting message to all connected nodes.
        /// </summary>
        public Task SendMessageAsync(byte[] memberMessage);

        /// <summary>
        /// Raised when a new remoting message is received.
        /// </summary>
        public event EventHandler<byte[]>? MessageReceived;

        /// <inheritdoc cref="IRemoteMessageConverter"/>
        public IRemoteMessageConverter MessageConverter { get; }
    }
}
