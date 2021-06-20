using System;
using System.Threading.Tasks;

namespace OwlCore.Remoting.Transfer
{
    /// <inheritdoc/>
    public class DefaultRemoteMessageHandler : IRemoteMessageHandler
    {
        /// <summary>
        /// Creates a new instance of <see cref="DefaultRemoteMessageHandler"/>.
        /// </summary>
        public DefaultRemoteMessageHandler(RemotingMode mode)
        {
            MessageConverter = null!; //todo 
            Mode = mode;
        }

        /// <inheritdoc/>
        public RemotingMode Mode { get; set; }

        /// <inheritdoc/>
        public IRemoteMessageConverter MessageConverter { get; }

        /// <inheritdoc/>
        public bool IsInitialized { get; private set; }

        /// <inheritdoc/>
        public event EventHandler<byte[]>? MessageReceived;

        /// <inheritdoc/>
        public event EventHandler? OnConnected;

        /// <inheritdoc/>
        public event EventHandler? OnDisconnected;

        /// <inheritdoc/>
        public Task InitAsync()
        {
            IsInitialized = true;

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task SendMessageAsync(byte[] memberMessage)
        {
            throw new NotImplementedException();
        }
    }
}
