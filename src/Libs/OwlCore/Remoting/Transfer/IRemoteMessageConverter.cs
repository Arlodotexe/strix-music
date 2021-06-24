using OwlCore.Remoting.Transfer.MessageConverters;
using System.Threading;
using System.Threading.Tasks;

namespace OwlCore.Remoting.Transfer
{
    /// <summary>
    /// Prepares the data in a <see cref="IRemoteMessage"/> for generic data transfer.
    /// </summary>
    /// <remarks>
    /// Several default implementations are given in the <see cref="MessageConverters"/> namespace.
    /// Of these, the <see cref="NewtonsoftRemoteMessageConverter"/> is the most reliable and should be used by default, unless you know what you're doing.
    /// <para>
    /// When crafting your own <see cref="IRemoteMessageConverter"/>, keep in mind that unless you're extra careful to remote only primitive types, object instances being serialized may need to handle circular references.
    /// </para>
    /// </remarks>
    public interface IRemoteMessageConverter
    {
        /// <summary>
        /// Converts the given <paramref name="message"/> to a byte array that is safe for generic data transfer.
        /// </summary>
        /// <param name="message">The message being sent.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the ongoing task.</param>
        /// <returns></returns>
        public Task<byte[]> SerializeAsync(IRemoteMessage message, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Converts the given byte array back to a valid <see cref="IRemoteMessage"/>.
        /// </summary>
        /// <param name="bytes">The byte array to deserialize.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the ongoing task.</param>
        /// <returns></returns>
        public Task<IRemoteMessage> DeserializeAsync(byte[] bytes, CancellationToken? cancellationToken = null);
    }
}
