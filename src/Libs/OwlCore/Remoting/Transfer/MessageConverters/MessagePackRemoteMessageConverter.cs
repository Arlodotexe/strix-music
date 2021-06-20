using MessagePack;
using System.Threading.Tasks;

namespace OwlCore.Remoting.Transfer.MessageConverters
{
    /// <inheritdoc/>
    public class MessagePackRemoteMessageConverter : IRemoteMessageConverter
    {
        /// <inheritdoc/>
        public Task<IRemoteMemberMessage> DeserializeAsync(byte[] bytes)
        {
            var data = MessagePackSerializer.Deserialize<IRemoteMemberMessage>(bytes, MessagePack.Resolvers.ContractlessStandardResolver.Options);

            return Task.FromResult(data);
        }

        /// <inheritdoc/>
        public Task<byte[]> SerializeAsync(IRemoteMemberMessage message)
        {
            var data = MessagePackSerializer.Serialize(message, MessagePack.Resolvers.ContractlessStandardResolver.Options);

            return Task.FromResult(data);
        }
    }
}
