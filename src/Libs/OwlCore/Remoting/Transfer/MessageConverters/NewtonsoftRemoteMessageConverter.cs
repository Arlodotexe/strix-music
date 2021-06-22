using Microsoft.Toolkit.Diagnostics;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;

namespace OwlCore.Remoting.Transfer.MessageConverters
{
    /// <inheritdoc/>
    public class NewtonsoftRemoteMessageConverter : IRemoteMessageConverter
    {
        /// <inheritdoc/>
        public Task<IRemoteMemberMessage> DeserializeAsync(byte[] message)
        {
            var jsonStr = Encoding.UTF8.GetString(message);

            var deserializedBase = JsonConvert.DeserializeObject<RemoteMemberMessageBase>(jsonStr);

            IRemoteMemberMessage result = deserializedBase.Action switch
            {
                RemotingAction.None => deserializedBase,
                RemotingAction.MethodCall => JsonConvert.DeserializeObject<RemoteMethodCallMessage>(jsonStr),
                RemotingAction.PropertyChange => JsonConvert.DeserializeObject<RemotePropertyChangeMessage>(jsonStr),
                RemotingAction.EventInvocation => JsonConvert.DeserializeObject<RemoteEventInvocationMessage>(jsonStr),
                RemotingAction.RemoteMethodProxy => JsonConvert.DeserializeObject<RemoteMethodProxyMessage>(jsonStr),
                _ => ThrowHelper.ThrowArgumentOutOfRangeException<IRemoteMemberMessage>(),
            };

            result.TargetMemberSignature = result.TargetMemberSignature.Replace("TARGETNAME_", "");

            return Task.FromResult(result);
        }

        /// <inheritdoc/>
        public Task<byte[]> SerializeAsync(IRemoteMemberMessage message)
        {
            // Newtonsoft won't serialize a string containing a method signature.
            message.TargetMemberSignature = $"TARGETNAME_{message.TargetMemberSignature}";

            var jsonStr = JsonConvert.SerializeObject(message);
            var bytes = Encoding.UTF8.GetBytes(jsonStr);

            message.TargetMemberSignature = message.TargetMemberSignature.Replace("TARGETNAME_", "");
            return Task.FromResult(bytes);
        }
    }
}
