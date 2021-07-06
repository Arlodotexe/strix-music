using MessagePack;
using System.Threading;
using System.Threading.Tasks;

namespace OwlCore.Remoting.Transfer.MessageConverters
{
    /// <inheritdoc/>
    public class MessagePackRemoteMessageConverter : IRemoteMessageConverter
    {
        /// <inheritdoc/>
        public Task<IRemoteMessage> DeserializeAsync(byte[] bytes, CancellationToken? cancellationToken = null)
        {
            var data = MessagePackSerializer.Deserialize<IRemoteMessage>(bytes, MessagePack.Resolvers.ContractlessStandardResolver.Options);

            return Task.FromResult(data);
        }

        /// <inheritdoc/>
        public Task<byte[]> SerializeAsync(IRemoteMessage message, CancellationToken? cancellationToken = null)
        {
            var data = MessagePackSerializer.Serialize(message, MessagePack.Resolvers.ContractlessStandardResolver.Options);

            return Task.FromResult(data);
        }
    }
}
