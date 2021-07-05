using Microsoft.Toolkit.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OwlCore.Remoting.Transfer.MessageConverters
{
    public class NewtonsoftRemoteContractResolver : DefaultContractResolver
    {
        protected override JsonDynamicContract CreateDynamicContract(Type objectType)
        {
            return base.CreateDynamicContract(objectType);
        }

        protected override JsonObjectContract CreateObjectContract(Type objectType)
        {
            return base.CreateObjectContract(objectType);
        }

        protected override IValueProvider CreateMemberValueProvider(MemberInfo member)
        {
            return base.CreateMemberValueProvider(member);
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            return base.CreateProperty(member, memberSerialization);
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            return base.CreateProperties(type, memberSerialization);
        }

        protected override JsonDictionaryContract CreateDictionaryContract(Type objectType)
        {
            return base.CreateDictionaryContract(objectType);
        }

        protected override JsonArrayContract CreateArrayContract(Type objectType)
        {
            return base.CreateArrayContract(objectType);
        }
    }

    /// <summary>
    /// Prepares the data in a <see cref="IRemoteMessage"/> for generic data transfer using <see cref="Newtonsoft.Json"/>.
    /// </summary>
    public class NewtonsoftRemoteMessageConverter : IRemoteMessageConverter
    {
        /// <summary>
        /// The settings used when serializing and deserializing json.
        /// </summary>
        public JsonSerializerSettings SerializerSettings { get; set; } = new JsonSerializerSettings()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
            PreserveReferencesHandling = PreserveReferencesHandling.All,
            TypeNameHandling = TypeNameHandling.All,
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full,
            ContractResolver = new NewtonsoftRemoteContractResolver(),
        };

        /// <inheritdoc/>
        public Task<IRemoteMessage> DeserializeAsync(byte[] message, CancellationToken? cancellationToken = null)
        {
            var jsonStr = Encoding.UTF8.GetString(message);
            Guard.IsNotNullOrWhiteSpace(jsonStr, nameof(jsonStr));

            var deserializedBase = JsonConvert.DeserializeObject<RemoteMessageBase>(jsonStr, SerializerSettings);
            Guard.IsNotNull(deserializedBase, nameof(deserializedBase));

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
            {
                memberMessage.TargetMemberSignature = memberMessage.TargetMemberSignature.Replace("TARGETNAME_", "");

                foreach (var param in memberMessage.Parameters)
                {
                    if (param.Value is JObject jObj)
                    {
                        var targetType = Type.GetType(param.AssemblyQualifiedName);
                        param.Value = jObj.ToObject(targetType);
                    }
                }
            }

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
        public Task<byte[]> SerializeAsync(IRemoteMessage message, CancellationToken? cancellationToken = null)
        {
            var methodCallMessage = message as RemoteMethodCallMessage;

            // Newtonsoft won't serialize a string containing a method signature.
            if (methodCallMessage != null)
                methodCallMessage.TargetMemberSignature = $"TARGETNAME_{methodCallMessage.TargetMemberSignature}";

            var jsonStr = JsonConvert.SerializeObject(message, SerializerSettings);
            var bytes = Encoding.UTF8.GetBytes(jsonStr);

            // Newtonsoft won't serialize a string containing a method signature.
            if (methodCallMessage != null)
                methodCallMessage.TargetMemberSignature = methodCallMessage.TargetMemberSignature.Replace("TARGETNAME_", "");

            return Task.FromResult(bytes);
        }
    }
}
