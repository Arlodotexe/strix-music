using OwlCore.Provisos;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OwlCore.Remoting.Transfer
{
    /// <summary>
    /// Handles sending and receiving of <see cref="IRemoteMessage"/>s.
    /// </summary>
    public interface IRemoteMessageHandler : IAsyncInit
    {
        /// <inheritdoc cref="RemotingMode" />
        public RemotingMode Mode { get; set; }

        /// <summary>
        /// Emit a remoting message to all connected nodes.
        /// </summary>
        /// <param name="memberMessage">The message being sent.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the ongoing task.</param>
        public Task SendMessageAsync(IRemoteMessage memberMessage, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Raised when a new remoting message is received.
        /// </summary>
        public event EventHandler<IRemoteMessage>? MessageReceived;

        /// <inheritdoc cref="IRemoteMessageConverter"/>
        public IRemoteMessageConverter MessageConverter { get; }
    }
}
