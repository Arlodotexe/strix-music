using MessagePack;
using System.Threading.Tasks;

namespace OwlCore.Remoting.Transfer.MessageConverters
{
    /// <inheritdoc/>
    public class MessagePackRemoteMessageConverter : IRemoteMessageConverter
    {
        /// <inheritdoc/>
        public Task<IRemoteMessage> DeserializeAsync(byte[] bytes)
        {
            var data = MessagePackSerializer.Deserialize<IRemoteMessage>(bytes, MessagePack.Resolvers.ContractlessStandardResolver.Options);

            return Task.FromResult(data);
        }

        /// <inheritdoc/>
        public Task<byte[]> SerializeAsync(IRemoteMessage message)
        {
            var data = MessagePackSerializer.Serialize(message, MessagePack.Resolvers.ContractlessStandardResolver.Options);

            return Task.FromResult(data);
        }
    }
}
