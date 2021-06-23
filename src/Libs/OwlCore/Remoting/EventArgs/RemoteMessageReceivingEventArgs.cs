using OwlCore.Remoting.Transfer;

namespace OwlCore.Remoting.EventArgs
{
    /// <summary>
    /// <see cref="EventArgs"/> for <see cref="MemberRemote.MessageReceiving"/>.
    /// </summary>
    public class RemoteMessageReceivingEventArgs : System.EventArgs
    {
        /// <summary>
        /// Creates a new instance of <see cref="RemoteMessageReceivingEventArgs"/>.
        /// </summary>
        /// <param name="message">The received message.</param>
        public RemoteMessageReceivingEventArgs(IRemoteMessage message)
        {
            Message = message;
        }

        /// <summary>
        /// The received message.
        /// </summary>
        public IRemoteMessage Message { get; }

        /// <summary>
        /// <see langword="true"/> if the event is handled; otherwise, <see langword="false"/>.
        /// </summary>
        public bool Handled { get; set; }
    }
}
