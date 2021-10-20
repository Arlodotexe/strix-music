using OwlCore.Remoting;
using OwlCore.Remoting.Transfer;
using OwlCore.Remoting.Transfer.MessageConverters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StrixMusic.Cores.External
{
    /// <summary>
    /// The remote message handler use for external cores.
    /// </summary>
    public class ExternalCoreRemoteMessageHandler : IRemoteMessageHandler
    {
        /// <summary>
        /// A singleton instance of this class.
        /// </summary>
        public static ExternalCoreRemoteMessageHandler Singleton { get; } = new ExternalCoreRemoteMessageHandler();

        /// <inheritdoc/>
        public RemotingMode Mode { get; set; }

        /// <inheritdoc/>
        public MemberSignatureScope MemberSignatureScope { get; set; } = MemberSignatureScope.MemberName;

        /// <inheritdoc/>
        public IRemoteMessageConverter MessageConverter { get; } = new NewtonsoftRemoteMessageConverter();

        /// <inheritdoc/>
        public bool IsInitialized { get; private set; }

        /// <inheritdoc/>
        public event EventHandler<IRemoteMessage>? MessageReceived;

        /// <inheritdoc/>
        public Task InitAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task SendMessageAsync(IRemoteMessage memberMessage, CancellationToken? cancellationToken = null)
        {
            throw new NotImplementedException();
        }
    }
}
