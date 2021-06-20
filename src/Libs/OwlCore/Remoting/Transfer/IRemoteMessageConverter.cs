using OwlCore.Remoting.Transfer.MessageConverters;
using System.Threading.Tasks;

namespace OwlCore.Remoting.Transfer
{
    /// <summary>
    /// Prepares the data in a <see cref="IRemoteMemberMessage"/> for generic data transfer.
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
        /// <returns></returns>
        public Task<byte[]> SerializeAsync(IRemoteMemberMessage message);

        /// <summary>
        /// Converts the given byte array back to a valid <see cref="IRemoteMemberMessage"/>.
        /// </summary>
        /// <param name="bytes">The byte array to deserialize.</param>
        /// <returns></returns>
        public Task<IRemoteMemberMessage> DeserializeAsync(byte[] bytes);
    }
}
