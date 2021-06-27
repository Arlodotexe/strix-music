using Microsoft.Toolkit.Diagnostics;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OwlCore.Remoting.Transfer.MessageConverters
{
    /// <inheritdoc/>
    public class NewtonsoftRemoteMessageConverter : IRemoteMessageConverter
    {
        /// <inheritdoc/>
        public Task<IRemoteMessage> DeserializeAsync(byte[] message, CancellationToken? cancellationToken = null)
        {
            var jsonStr = Encoding.UTF8.GetString(message);

            var deserializedBase = JsonConvert.DeserializeObject<RemoteMemberMessageBase>(jsonStr);

            IRemoteMessage result = deserializedBase.Action switch
            {
                RemotingAction.None => deserializedBase,
                RemotingAction.MethodCall => JsonConvert.DeserializeObject<RemoteMethodCallMessage>(jsonStr),
                RemotingAction.PropertyChange => JsonConvert.DeserializeObject<RemotePropertyChangeMessage>(jsonStr),
                RemotingAction.RemoteDataProxy => JsonConvert.DeserializeObject<RemoteDataMessage>(jsonStr),
                RemotingAction.ExceptionThrown => JsonConvert.DeserializeObject<RemoteExceptionDataMessage>(jsonStr),
                _ => ThrowHelper.ThrowNotSupportedException<IRemoteMemberMessage>(),
            };

            if (result is RemoteMethodCallMessage memberMessage)
                memberMessage.TargetMemberSignature = memberMessage.TargetMemberSignature.Replace("TARGETNAME_", "");

            // Newtonsoft doesn't deserialize these to proper types when holder type is T, object, or dynamic,
            // presumably because they don't implement IConvertible.
            if (result is RemoteDataMessage dataMsg)
            {
                if (Guid.TryParse(dataMsg.Result?.ToString(), out Guid guid))
                    dataMsg.Result = guid;
            }

            return Task.FromResult(result);
        }

        /// <inheritdoc/>
        public async Task<byte[]> SerializeAsync(IRemoteMessage message, CancellationToken? cancellationToken = null)
        {
            var methodCallMessage = message as RemoteMethodCallMessage;

            // Newtonsoft won't serialize a string containing a method signature.
            if (methodCallMessage != null)
                methodCallMessage.TargetMemberSignature = $"TARGETNAME_{methodCallMessage.TargetMemberSignature}";

            var jsonStr = JsonConvert.SerializeObject(message);
            var bytes = Encoding.UTF8.GetBytes(jsonStr);

            // Newtonsoft won't serialize a string containing a method signature.
            if (methodCallMessage != null)
                methodCallMessage.TargetMemberSignature = methodCallMessage.TargetMemberSignature.Replace("TARGETNAME_", "");

            var x = await DeserializeAsync(bytes);

            return bytes;
        }
    }
}
