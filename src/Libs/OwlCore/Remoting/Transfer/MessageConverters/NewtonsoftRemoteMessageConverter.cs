using System;
using System.Collections;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;

namespace OwlCore.Remoting.Transfer.MessageConverters
{
    /// <summary>
    /// Prepares the data in a <see cref="IRemoteMessage"/> for generic data transfer using <see cref="Newtonsoft.Json"/>.
    /// </summary>
    public class NewtonsoftRemoteMessageConverter : IRemoteMessageConverter
    {
        private JsonSerializerSettings _jsonSerializerSettings;

        /// <summary>
        /// Creates an instance of <see cref="NewtonsoftRemoteMessageConverter"/>.
        /// </summary>
        public NewtonsoftRemoteMessageConverter()
        {
            _jsonSerializerSettings = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                TypeNameHandling = TypeNameHandling.All,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full,
                MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateParseHandling = DateParseHandling.DateTime,
                FloatFormatHandling = FloatFormatHandling.String,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            };
        }

        /// <inheritdoc/>
        public async Task<IRemoteMessage> DeserializeAsync(byte[] message, CancellationToken? cancellationToken = null)
        {
            var deserializedBase = await DeserializeAsync<IRemoteMessage>();
            Guard.IsNotNull(deserializedBase, nameof(deserializedBase));

            IRemoteMessage? result = deserializedBase.Action switch
            {
                RemotingAction.None => deserializedBase,
                RemotingAction.MethodCall => await DeserializeAsync<RemoteMethodCallMessage>(),
                RemotingAction.PropertyChange => await DeserializeAsync<RemotePropertyChangeMessage>(),
                RemotingAction.RemoteDataProxy => await DeserializeAsync<RemoteDataMessage>(),
                RemotingAction.ExceptionThrown => await DeserializeAsync<RemoteExceptionDataMessage>(),
                _ => ThrowHelper.ThrowNotSupportedException<IRemoteMessage>(),
            };

            if (result is RemoteMethodCallMessage memberMessage)
            {
                memberMessage.TargetMemberSignature = memberMessage.TargetMemberSignature.Replace("TARGETNAME_", "");

                foreach (var param in memberMessage.Parameters)
                {
                    if (param.Value is JContainer container)
                    {
                        var targetType = Type.GetType(param.AssemblyQualifiedName);
                        param.Value = container.ToObject(targetType);
                    }

                    Guard.IsNotNullOrWhiteSpace(param.AssemblyQualifiedName, nameof(param.AssemblyQualifiedName));

                    param.Value = TrySmartTypeConversion(param.Value, param.AssemblyQualifiedName);
                }
            }

            if (result is RemoteDataMessage dataMsg)
            {
                if (dataMsg.Result is JContainer container)
                {
                    var targetType = Type.GetType(dataMsg.TargetMemberSignature);
                    dataMsg.Result = container.ToObject(targetType);
                }

                dataMsg.Result = TrySmartTypeConversion(dataMsg.Result, dataMsg.TargetMemberSignature);
            }

            if (result is RemotePropertyChangeMessage remoteProp)
            {
                remoteProp.NewValue = TrySmartTypeConversion(remoteProp.NewValue, remoteProp.TargetMemberSignature);
                remoteProp.OldValue = TrySmartTypeConversion(remoteProp.OldValue, remoteProp.TargetMemberSignature);
            }

            Guard.IsNotNull(result, nameof(result));
            return result;

            Task<T?> DeserializeAsync<T>()
            {
                var str = Encoding.UTF8.GetString(message);

                return Task.FromResult(JsonConvert.DeserializeObject<T>(str, _jsonSerializerSettings));
            }
        }

        /// <inheritdoc/>
        public Task<byte[]> SerializeAsync(IRemoteMessage message, CancellationToken? cancellationToken = null)
        {
            var methodCallMessage = message as RemoteMethodCallMessage;

            // Newtonsoft won't serialize a string containing a method signature.
            if (methodCallMessage != null)
                methodCallMessage.TargetMemberSignature = $"TARGETNAME_{methodCallMessage.TargetMemberSignature}";

            using var stream = new MemoryStream();
            var str = JsonConvert.SerializeObject(message, _jsonSerializerSettings);

            // Newtonsoft won't serialize a string containing a method signature.
            if (methodCallMessage != null)
                methodCallMessage.TargetMemberSignature = methodCallMessage.TargetMemberSignature.Replace("TARGETNAME_", "");

            return Task.FromResult(Encoding.UTF8.GetBytes(str));
        }

        /// <summary>
        /// A reflection-free way to handle additional type conversion not properly handled by newtonsoft.
        /// </summary>
        private object? TrySmartTypeConversion(object? value, string assemblyQualifiedName)
        {
            value = TrySmartTypeConversion_Primitives(value, assemblyQualifiedName);
            value = TrySmartTypeConversion_Structs(value, assemblyQualifiedName);
            value = TrySmartTypeConversion_EnumerableItems(value);

            return value;
        }

        private object? TrySmartTypeConversion_EnumerableItems(object? value)
        {
            if (!(value is string) && value is IEnumerable collection)
                return TrySmartTypeConversion_EnumerableItems_Internal(collection);

            return value;

            // The only sane way to do this without going completely overboard on possible collection types.
            IEnumerable TrySmartTypeConversion_EnumerableItems_Internal(IEnumerable enumerable)
            {
                foreach (var item in enumerable)
                    yield return TrySmartTypeConversion(item, item?.GetType().AssemblyQualifiedName ?? string.Empty);
            }
        }

        private object? TrySmartTypeConversion_Structs(object? value, string assemblyQualifiedName)
        {
            if (value is string valStr)
            {
                if (assemblyQualifiedName == typeof(TimeSpan).AssemblyQualifiedName && TimeSpan.TryParse(valStr, out var timeSpan))
                    return timeSpan;

                if (assemblyQualifiedName == typeof(DateTime).AssemblyQualifiedName && DateTime.TryParse(valStr, out var dateTime))
                    return dateTime;

                if (assemblyQualifiedName == typeof(Guid).AssemblyQualifiedName && Guid.TryParse(valStr, out var guid))
                    return guid;
            }

            return value;
        }

        private object? TrySmartTypeConversion_Primitives(object? value, string assemblyQualifiedName)
        {
            if (assemblyQualifiedName == typeof(short).AssemblyQualifiedName)
                return Convert.ToInt16(value);

            if (assemblyQualifiedName == typeof(ushort).AssemblyQualifiedName)
                return Convert.ToUInt16(value);

            if (assemblyQualifiedName == typeof(int).AssemblyQualifiedName)
                return Convert.ToInt32(value);

            if (assemblyQualifiedName == typeof(uint).AssemblyQualifiedName)
                return Convert.ToUInt32(value);

            if (assemblyQualifiedName == typeof(double).AssemblyQualifiedName)
            {
                if (value is string doubleStr)
                {
                    if (doubleStr == "Infinity")
                        return double.PositiveInfinity;

                    if (doubleStr == "-Infinity")
                        return double.NegativeInfinity;

                    if (doubleStr == "NaN")
                        return double.NaN;
                }

                return Convert.ToDouble(value);
            }

            if (assemblyQualifiedName == typeof(float).AssemblyQualifiedName)
            {
                if (value is string floatStr)
                {
                    if (floatStr == "Infinity")
                        return float.PositiveInfinity;

                    if (floatStr == "-Infinity")
                        return float.NegativeInfinity;

                    if (floatStr == "NaN")
                        return float.NaN;
                }

                return Convert.ToSingle(value);
            }

            if (assemblyQualifiedName == typeof(byte).AssemblyQualifiedName)
                return Convert.ToByte(value);

            if (assemblyQualifiedName == typeof(sbyte).AssemblyQualifiedName)
                return Convert.ToSByte(value);

            return value;
        }
    }
}
