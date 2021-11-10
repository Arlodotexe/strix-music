using OwlCore.Remoting.Transfer;

namespace OwlCore.Remoting
{
    /// <summary>
    /// <see cref="System.EventArgs"/> for <see cref="MemberRemote.MessageReceiving"/>.
    /// </summary>
    public class RemoteMessageReceivingEventArgs : System.EventArgs
    {
        /// <summary>
        /// Creates a new instance of <see cref="RemoteMessageReceivingEventArgs"/>.
        /// </summary>
        /// <param name="message">The message being received.</param>
        public RemoteMessageReceivingEventArgs(IRemoteMessage message)
        {
            Message = message;
        }

        /// <summary>
        /// The message being received.
        /// </summary>
        public IRemoteMessage Message { get; }

        /// <summary>
        /// <see langword="true"/> if the event is handled; otherwise, <see langword="false"/>.
        /// </summary>
        public bool Handled { get; set; }
    }
}
