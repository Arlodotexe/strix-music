using System;
using System.Collections.Generic;
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
        /// <param name="messageHandler">The message handler to use when communicating changes. If not given, the handler given to <see cref="SetDefaultMessageHandler(IRemoteMessageHandler)"/> will be used instead.</param>
        public MemberRemote(object classInstance, string id, IRemoteMessageHandler? messageHandler = null)
        {
            Type = classInstance.GetType();
            Instance = classInstance;
            Id = id;

            MessageHandler = messageHandler ?? _defaultRemoteMessageHandler ?? ThrowHelper.ThrowArgumentNullException<IRemoteMessageHandler>($"No {nameof(messageHandler)} was specified and no default handler was set.");

            var members = Type.GetMembers(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            Properties = members.Where(x => x.MemberType == MemberTypes.Property).Cast<PropertyInfo>();
            Methods = members.Where(x => x.MemberType == MemberTypes.Method).Cast<MethodInfo>();

            AttachEvents();
        }

        private void AttachEvents()
        {
            MessageHandler.MessageReceived += MessageHandler_DataReceived;

            RemoteMethodAttribute.Entered += OnMethodEntered;
            RemotePropertyAttribute.SetEntered += OnPropertySetEntered;
        }

        private void DetachEvents()
        {
            MessageHandler.MessageReceived -= MessageHandler_DataReceived;

            RemoteMethodAttribute.Entered += OnMethodEntered;
            RemotePropertyAttribute.SetEntered -= OnPropertySetEntered;
        }

        internal async void MessageHandler_DataReceived(object sender, byte[] e)
        {
            var message = await MessageHandler.MessageConverter.DeserializeAsync(e);

            if (message.MemberRemoteId != Id)
                return;

            if (message is RemoteMethodCallMessage methodCallMsg)
                HandleIncomingRemoteMethodCall(methodCallMsg);

            if (message is RemotePropertyChangeMessage propertyChangeMsg)
                HandleIncomingRemotePropertyChange(propertyChangeMsg);
        }

        private async void OnMethodEntered(object sender, MethodEnteredEventArgs e)
        {
            if (e.Instance != Instance)
                return;

            if (!IsValidRemotingDirection(e.MethodBase, e.MethodBase.DeclaringType, false))
                return;

            var paramData = CreateMethodParameterData(e.MethodBase, e.Values);
            var memberSignature = CreateMemberSignature(e.MethodBase);
            var remoteMessage = new RemoteMethodCallMessage(Id, memberSignature, paramData);

            await EmitRemotingMessageToHandler(remoteMessage);
        }

        private async void OnPropertySetEntered(object sender, PropertySetEnteredEventArgs e)
        {
            if (e.PropertyInterceptionInfo.Instance != Instance)
                return;

            if (!IsValidRemotingDirection(e.PropertyInterceptionInfo.PropertyType, e.PropertyInterceptionInfo.DeclaringType, false))
                return;

            var propertyInfo = e.PropertyInterceptionInfo.ToPropertyInfo();
            var memberSignature = CreateMemberSignature(propertyInfo);
            var remoteMessage = new RemotePropertyChangeMessage(Id, memberSignature, e.NewValue, e.OldValue);

            await EmitRemotingMessageToHandler(remoteMessage);
        }

        /// <summary>
        /// A unique identifier for this instance, consistent between hosts and clients.
        /// </summary>
        public string Id { get; }

        /// <inheritdoc cref="RemotingMode" />
        public RemotingMode Mode => MessageHandler.Mode;

        /// <summary>
        /// The <see cref="IRemoteMessageHandler"/> used by this <see cref="MemberRemote"/> instance to transfer member states.
        /// </summary>
        public IRemoteMessageHandler MessageHandler { get; set; }

        internal Type Type { get; }

        internal object Instance { get; }

        internal IEnumerable<MethodInfo> Methods { get; }

        internal IEnumerable<PropertyInfo> Properties { get; }

        /// <summary>
        /// Prepares and sends the given <paramref name="message"/> outbound.
        /// </summary>
        /// <param name="message">The message</param>
        /// <returns>This should be used in scenarios where you need to send a custom <see cref="IRemoteMemberMessage"/>, or where you need to force emit a member change remotely without executing the change on the current node.</returns>
        public async Task EmitRemotingMessageToHandler(IRemoteMemberMessage message)
        {
            await MessageHandler.InitAsync();

            var data = await MessageHandler.MessageConverter.SerializeAsync(message);

            await MessageHandler.SendMessageAsync(data);
        }

        private void HandleIncomingRemotePropertyChange(RemotePropertyChangeMessage propertyChangeMessage)
        {
            var propertyInfo = Properties.First(x => CreateMemberSignature(x) == propertyChangeMessage.TargetMemberSignature);

            if (!IsValidRemotingDirection(propertyInfo, propertyInfo.DeclaringType, true))
                return;

            var castValue = Convert.ChangeType(propertyChangeMessage.NewValue, propertyInfo.PropertyType);
            propertyInfo.SetValue(Instance, castValue);
        }

        private void HandleIncomingRemoteMethodCall(RemoteMethodCallMessage methodCallMsg)
        {
            var methodInfo = Methods.First(x => CreateMemberSignature(x) == methodCallMsg.TargetMemberSignature);

            if (!IsValidRemotingDirection(methodInfo, methodInfo.DeclaringType, true))
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

        private static Dictionary<string, object?> CreateMethodParameterData(MethodBase method, object?[] parameterValues)
        {
            var parameterInfo = method.GetParameters();
            var paramData = new Dictionary<string, object?>();

            for (int i = 0; i < parameterInfo.Length; i++)
            {
                ParameterInfo? parameter = parameterInfo[i];

                // TODO: Generic types.
                paramData.Add(parameter.ParameterType.AssemblyQualifiedName, parameterValues[i]);
            }

            return paramData;
        }

        private bool IsValidRemotingDirection(MemberInfo memberInfo, MemberInfo declaringType, bool isReceiving)
        {
            var attribute = memberInfo.GetCustomAttribute<RemoteOptionsAttribute>();
            if (attribute is null)
            {
                var declaringClassType = declaringType;
                if (declaringClassType.MemberType != MemberTypes.TypeInfo)
                    return false;

                attribute = declaringClassType.GetCustomAttribute<RemoteOptionsAttribute>();

                if (attribute is null)
                    return false;
            }

            var isHost = Mode.HasFlag(RemotingMode.Host);
            var isClient = Mode.HasFlag(RemotingMode.Client);

            var targetOutbound = (attribute.Direction.HasFlag(RemotingDirection.OutboundClient) && isClient) || (attribute.Direction.HasFlag(RemotingDirection.OutboundHost) && isHost);
            var targetInbound = (attribute.Direction.HasFlag(RemotingDirection.InboundClient) && isClient) || (attribute.Direction.HasFlag(RemotingDirection.InboundHost) && isHost);

            return (!isReceiving && targetInbound) || (isReceiving && targetOutbound);
        }

        /// <summary>
        /// Creates a member signature that is internally used to identify members in a type when sending/receiving data.
        /// </summary>
        /// <param name="memberInfo">The <see cref="MemberInfo"/> to generate a signature for.</param>
        /// <returns>A unique member signature for the given <see cref="MemberInfo"/>.</returns>
        /// <remarks>
        /// Useful in scenarios where you need to construct your own <see cref="IRemoteMemberMessage"/> instance.
        /// </remarks>
        public static string CreateMemberSignature(MemberInfo memberInfo)
        {
            if (memberInfo is MethodBase methodBase)
                return methodBase.ToString();

            if (memberInfo is PropertyInfo propertyInfo)
            {
                return $"{propertyInfo.DeclaringType} {propertyInfo.PropertyType} {propertyInfo.Name}";
            }

            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            DetachEvents();
        }
    }
}
