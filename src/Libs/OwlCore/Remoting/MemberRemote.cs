using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.Extensions;
using OwlCore.Remoting.Attributes;
using OwlCore.Remoting.EventArgs;
using OwlCore.Remoting.Transfer;

namespace OwlCore.Remoting
{
    /// <summary>
    /// Automatically sync events, properties, method calls and fields remotely between two object instances, even if the code runs on another machine.
    /// </summary>
    public class MemberRemote : IDisposable
    {
        private static IRemoteMessageHandler? _defaultRemoteMessageHandler;

        /// <summary>
        /// Set the default message handler to use for all instances of <see cref="MemberRemote"/>, unless given an overload.
        /// </summary>
        /// <param name="messageHandler"></param>
        public static void SetDefaultMessageHandler(IRemoteMessageHandler messageHandler) => _defaultRemoteMessageHandler = messageHandler;

        /// <summary>
        /// Creates a new instance of <see cref="MemberRemote"/>.
        /// </summary>
        /// <param name="classInstance">The instance to remote.</param>
        /// <param name="id">A unique identifier for this instance, consistent between hosts and clients.</param>
        /// <param name="messageHandler">The message handler to use when communicating changes.</param>
        public MemberRemote(object classInstance, string id, IRemoteMessageHandler? messageHandler = null)
        {
            Type = classInstance.GetType();
            Instance = classInstance;
            Id = id;

            MessageHandler = messageHandler ?? _defaultRemoteMessageHandler ?? ThrowHelper.ThrowArgumentNullException<IRemoteMessageHandler>($"No {nameof(messageHandler)} was specified and no default handler was set.");

            var members = Type.GetMembers();

            Events = members.Where(x => x.MemberType == MemberTypes.Event).Cast<EventInfo>(); // TODO: Intercepts
            Fields = members.Where(x => x.MemberType == MemberTypes.Field).Cast<FieldInfo>(); // TODO: Intercepts
            Properties = members.Where(x => x.MemberType == MemberTypes.Property).Cast<PropertyInfo>(); // TODO: Intercepts
            Methods = members.Where(x => x.MemberType == MemberTypes.Method).Cast<MethodInfo>();

            AttachEvents();
        }

        private void AttachEvents()
        {
            MessageHandler.MessageReceived += MessageHandler_DataReceived;

            RemoteMethodAttribute.Entered += OnMethodEntered;
        }

        private void DetachEvents()
        {
            MessageHandler.MessageReceived -= MessageHandler_DataReceived;

            RemoteMethodAttribute.Entered -= OnMethodEntered;
        }

        internal async void MessageHandler_DataReceived(object sender, byte[] e)
        {
            var message = await MessageHandler.MessageConverter.DeserializeAsync(e);

            if (message.MemberInstanceId != Id)
                return;

            if (message is RemoteMethodCallMessage methodCallMsg)
            {
                var methodInfo = Methods.First(x => x.ToString() == message.TargetName);

                if (!IsValidRemotingDirection(methodInfo, true))
                    return;

                var parameterValues = new List<object?>();

                foreach (var param in methodCallMsg.Parameters)
                {
                    var type = Type.GetType(param.Key, true);
                    var castValue = Convert.ChangeType(param.Value, type);

                    parameterValues.Add(castValue);
                }

                methodInfo?.Invoke(Instance, parameterValues.ToArray());
            }
            else
            {
                ThrowHelper.ThrowArgumentOutOfRangeException();
            }
        }

        private bool IsValidRemotingDirection(MemberInfo memberInfo, bool isReceiving)
        {
            var attribute = memberInfo.GetCustomAttribute<RemoteMemberAttribute>();
            if (attribute is null)
            {
                var declaringClassType = memberInfo.DeclaringType;
                if (declaringClassType.MemberType != MemberTypes.TypeInfo)
                    return false;

                attribute = declaringClassType.GetCustomAttribute<RemoteMemberAttribute>();

                if (attribute is null)
                    return false;
            }

            var isHost = Mode.HasFlag(RemotingMode.Host);
            var isClient = Mode.HasFlag(RemotingMode.Client);
            
            var targetOutbound = (attribute.Direction.HasFlag(RemotingDirection.OutboundClient) && isClient) || (attribute.Direction.HasFlag(RemotingDirection.OutboundHost) && isHost);
            var targetInbound = (attribute.Direction.HasFlag(RemotingDirection.InboundClient) && isClient) || (attribute.Direction.HasFlag(RemotingDirection.InboundHost) && isHost);

            return (!isReceiving && targetInbound) || (isReceiving && targetOutbound);
        }

        private async void OnMethodEntered(object sender, MethodEnteredEventArgs e)
        {
            if (e.Instance != Instance)
                return;

            await MessageHandler.InitAsync();

            if (!IsValidRemotingDirection(e.MethodBase, false))
                return;

            // emit the data
            var parameterInfo = e.MethodBase.GetParameters();
            var paramData = new Dictionary<string, object?>();

            for (int i = 0; i < parameterInfo.Length; i++)
            {
                ParameterInfo? parameter = parameterInfo[i];

                // TODO: Generic types.
                paramData.Add(parameter.ParameterType.AssemblyQualifiedName, e.Values[i]);
            }

            var remoteMessage = new RemoteMethodCallMessage(Id, e.MethodBase.ToString(), paramData);
            var data = await MessageHandler.MessageConverter.SerializeAsync(remoteMessage);

            await MessageHandler.SendMessageAsync(data);
        }

        /// <inheritdoc cref="RemotingMode" />
        public RemotingMode Mode => MessageHandler.Mode;

        /// <summary>
        /// The <see cref="IRemoteMessageHandler"/> used by this <see cref="MemberRemote"/> instance to transfer member states.
        /// </summary>
        public IRemoteMessageHandler MessageHandler { get; set; }

        /// <summary>
        /// A unique identifier for this instance, consistent between hosts and clients.
        /// </summary>
        public string Id { get; }

        internal Type Type { get; }

        internal object Instance { get; }

        internal IEnumerable<MethodInfo> Methods { get; }

        internal IEnumerable<EventInfo> Events { get; }

        internal IEnumerable<FieldInfo> Fields { get; }

        internal IEnumerable<PropertyInfo> Properties { get; }

        /// <inheritdoc/>
        public void Dispose()
        {
            DetachEvents();
        }
    }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member


    [RemoteMember(RemotingDirection.Bidirectional)]
    public class Test : IDisposable
    {
        private MemberRemote _memberRemote;

        public Test()
        {
            _memberRemote = new MemberRemote(this, "myId");
        }

        [RemoteMethod]
        public void Log()
        {
            Console.WriteLine("Method");
        }

        [RemoteMember(RemotingDirection.HostToClient), RemoteMethod]
        public async Task<string> MethodAsync(int number, Test? test = null, ICollection<int>? collection = null)
        {
            var rpc = new RemoteMethodProxy<string>(_memberRemote);
            if (_memberRemote.Mode == RemotingMode.Client)
                return await rpc.ReceiveResult();

            // Do something here

            return await rpc.PublishResult("MethodResultValue");
        }

        [RemoteMember(RemotingDirection.Bidirectional), RemoteMethod]
        public Task LogAsync(string logMessage)
        {
            Console.WriteLine(logMessage);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _memberRemote.Dispose();
        }
    }
}
