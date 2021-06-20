using System.Threading.Tasks;

namespace OwlCore.Remoting.Transfer
{
    /// <summary>
    /// Prepares the data in a <see cref="IRemoteMemberMessage"/> for generic data transfer.
    /// </summary>
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
        /// <param name="message"></param>
        /// <returns></returns>
        public Task<IRemoteMemberMessage> DeserializeAsync(byte[] bytes);
    }
}
