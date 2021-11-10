using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.Extensions;
using OwlCore.Remoting.Transfer;

namespace OwlCore.Remoting
{
    /// <summary>
    /// Automatically sync properties, method calls and fields between two object instances, even if the code runs on another machine.
    /// </summary>
    /// <remarks>
    /// The instance passed into the constructor must use the <see cref="RemoteMethodAttribute"/> or <see cref="RemotePropertyAttribute"/> and <see cref="RemoteOptionsAttribute"/> to configure class members for remoting.
    /// <para/>
    /// Note that the <see cref="Id"/> of two <see cref="MemberRemote"/>s must match or received member change messages will be ignored.
    /// <para/>
    /// Failing to call <see cref="Dispose"/> may result in the passed instance not being cleaned up by the Garbage Collector.
    /// </remarks>
    public class MemberRemote : IDisposable
    {
        private static IRemoteMessageHandler? _defaultRemoteMessageHandler;
        private readonly SemaphoreSlim _messageReceivedSemaphore = new SemaphoreSlim(1, 1);

        /// <summary>
        /// Used to internally indicated when a member operation is performed by a <see cref="MemberRemote"/> on a given thread.
        /// </summary>
        internal static Dictionary<int, object> MemberHandleExpectancyMap { get; set; } = new Dictionary<int, object>();

        /// <summary>
        /// Set the default message handler to use for all instances of <see cref="MemberRemote"/>, unless given an overload.
        /// </summary>
        /// <param name="messageHandler">The default message handler to use.</param>
        public static void SetDefaultMessageHandler(IRemoteMessageHandler messageHandler) => _defaultRemoteMessageHandler = messageHandler;

        /// <summary>
        /// Creates a new instance of <see cref="MemberRemote"/>.
        /// </summary>
        /// <param name="classInstance">The instance to remote.</param>
        /// <param name="id">A unique identifier for this instance, consistent between hosts and clients.</param>
        /// <param name="messageHandler">The message handler to use when communicating changes. If not given, the handler given to <see cref="SetDefaultMessageHandler(IRemoteMessageHandler)"/> will be used instead.</param>
        /// <exception cref="ArgumentNullException">An argument is unexpectedly null or the default message handler has not been defined.</exception>
        public MemberRemote(object classInstance, string id, IRemoteMessageHandler? messageHandler = null)
        {
            Guard.IsNotNull(classInstance, nameof(classInstance));
            Guard.IsNotNull(id, nameof(id));

            Type = classInstance.GetType();
            Instance = classInstance;
            Id = id;

            MessageHandler = messageHandler ?? _defaultRemoteMessageHandler ?? ThrowHelper.ThrowArgumentNullException<IRemoteMessageHandler>($"No {nameof(messageHandler)} was specified and no default handler was set.");

            var members = Type.GetMembers(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            Properties = members.Where(x => x.MemberType == MemberTypes.Property).Cast<PropertyInfo>();
            Methods = members.Where(x => x.MemberType == MemberTypes.Method).Cast<MethodInfo>();
            Events = members.Where(x => x.MemberType == MemberTypes.Event).Cast<EventInfo>();

            AttachEvents();
        }

        private void AttachEvents()
        {
            MessageHandler.MessageReceived += MessageHandler_DataReceived;

            RemoteMethodAttribute.Entered += OnMethodEntered;
            RemoteMethodAttribute.ExceptionRaised += OnInterceptExceptionRaised;

            RemotePropertyAttribute.SetEntered += OnPropertySetEntered;
            RemotePropertyAttribute.ExceptionRaised += OnInterceptExceptionRaised;
        }

        private void DetachEvents()
        {
            MessageHandler.MessageReceived -= MessageHandler_DataReceived;

            RemoteMethodAttribute.Entered -= OnMethodEntered;
            RemoteMethodAttribute.ExceptionRaised -= OnInterceptExceptionRaised;

            RemotePropertyAttribute.SetEntered -= OnPropertySetEntered;
            RemotePropertyAttribute.ExceptionRaised -= OnInterceptExceptionRaised;
        }

        /// <summary>
        /// This is functional prototype code and should not be used.
        /// </summary>
        private void SetupEventInterception()
        {
            var eventsToRemote = Events.Where(x => x.GetCustomAttribute<RemoteEventAttribute>() != null);

            foreach (var eventInfo in eventsToRemote)
            {
                var tDelegate = eventInfo.EventHandlerType;

                var returnType = GetDelegateReturnType(tDelegate);
                if (returnType != typeof(void))
                    ThrowHelper.ThrowInvalidOperationException($"Event {eventInfo.Name} has a return type.");

                var paramTypes = GetDelegateParameterTypes(tDelegate);
                var handler = new DynamicMethod("", null, paramTypes, eventInfo.DeclaringType);

                ILGenerator ilgen = handler.GetILGenerator();

                var methodInfo = ((Action<object[]>)RemoteEventAttribute.HandleEventInvocation).Method;

                var arr = ilgen.DeclareLocal(typeof(object[]));

                ilgen.Emit(OpCodes.Ldc_I4, paramTypes.Length);
                ilgen.Emit(OpCodes.Newarr, typeof(object));
                ilgen.Emit(OpCodes.Stloc, arr);

                for (var i = 0; i < paramTypes.Length; i++)
                {
                    ilgen.Emit(OpCodes.Ldloc, arr);
                    ilgen.Emit(OpCodes.Ldc_I4, i);
                    ilgen.Emit(OpCodes.Ldarg, i);
                    ilgen.Emit(OpCodes.Stelem_Ref);
                }

                ilgen.Emit(OpCodes.Ldloc, arr);
                ilgen.EmitCall(OpCodes.Call, methodInfo, null);

                ilgen.Emit(OpCodes.Nop);
                ilgen.Emit(OpCodes.Ret);

                Delegate dEmitted = handler.CreateDelegate(tDelegate);

                var addHandler = eventInfo.GetAddMethod();

                addHandler.Invoke(Instance, new object[] { dEmitted });
            }

            Type[] GetDelegateParameterTypes(Type d)
            {
                if (d.BaseType != typeof(MulticastDelegate))
                    throw new ArgumentException("Not a delegate.", nameof(d));

                var invoke = d.GetMethod("Invoke");
                if (invoke == null)
                    throw new ArgumentException("Not a delegate.", nameof(d));

                var parameters = invoke.GetParameters();
                var typeParameters = new Type[parameters.Length];

                for (int i = 0; i < parameters.Length; i++)
                    typeParameters[i] = parameters[i].ParameterType;

                return typeParameters;
            }

            Type GetDelegateReturnType(Type d)
            {
                if (d.BaseType != typeof(MulticastDelegate))
                    throw new ArgumentException("Not a delegate.", nameof(d));

                MethodInfo invoke = d.GetMethod("Invoke");
                if (invoke == null)
                    throw new ArgumentException("Not a delegate.", nameof(d));

                return invoke.ReturnType;
            }
        }

        internal void MessageHandler_DataReceived(object sender, IRemoteMessage message)
        {
            _messageReceivedSemaphore.Wait();

            if (message is IRemoteMemberMessage memberMsg && memberMsg.MemberRemoteId != Id)
            {
                _messageReceivedSemaphore.Release();
                return;
            }

            var receivedArgs = new RemoteMessageReceivingEventArgs(message);
            MessageReceiving?.Invoke(this, receivedArgs);

            if (receivedArgs.Handled)
            {
                _messageReceivedSemaphore.Release();
                return;
            }

            if (message is RemoteMethodCallMessage methodCallMsg)
                HandleIncomingRemoteMethodCall(methodCallMsg);

            if (message is RemotePropertyChangeMessage propertyChangeMsg)
                HandleIncomingRemotePropertyChange(propertyChangeMsg);

            MessageReceived?.Invoke(this, message);
            _messageReceivedSemaphore.Release();
        }

        private async void OnMethodEntered(object sender, MethodEnteredEventArgs e)
        {
            if (!ReferenceEquals(e.Instance, Instance))
                return;

            if (!IsValidRemotingDirection(e.MethodBase, false))
                return;

            var paramData = CreateMethodParameterData(e.MethodBase, e.Values);
            var memberSignature = CreateMemberSignature(e.MethodBase, MemberSignatureScope);

            var remoteMessage = new RemoteMethodCallMessage(Id, memberSignature, paramData);

            await SendRemotingMessageAsync(remoteMessage);
        }

        private async void OnInterceptExceptionRaised(object sender, Exception exception)
        {
            var message = exception.Message;
            var stackTrace = exception.StackTrace;
            var targetSiteSignature = exception.TargetSite is null ? "Unknown" : CreateMemberSignature(exception.TargetSite, MemberSignatureScope);

            var exceptionMessage = new RemoteExceptionDataMessage(message, stackTrace, targetSiteSignature);
            await SendRemotingMessageAsync(exceptionMessage);
        }

        private async void OnPropertySetEntered(object sender, PropertySetEnteredEventArgs e)
        {
            if (!ReferenceEquals(e.PropertyInterceptionInfo.Instance, Instance))
                return;

            var propertyInfo = e.PropertyInterceptionInfo.ToPropertyInfo();

            if (!IsValidRemotingDirection(propertyInfo, false))
                return;

            var memberSignature = CreateMemberSignature(propertyInfo, MemberSignatureScope);
            var remoteMessage = new RemotePropertyChangeMessage(Id, memberSignature, propertyInfo.PropertyType.AssemblyQualifiedName, e.NewValue, e.OldValue);

            await SendRemotingMessageAsync(remoteMessage);
        }

        /// <summary>
        /// Raised when an inbound remote message is received and about to be processed.
        /// </summary>
        public event EventHandler<RemoteMessageReceivingEventArgs>? MessageReceiving;

        /// <summary>
        /// Raised when an inbound remote message is received and has been processed.
        /// </summary>
        public event EventHandler<IRemoteMessage>? MessageReceived;

        /// <summary>
        /// Raised when an outbound remote message is about to be sent.
        /// </summary>
        public event EventHandler<RemoteMessageSendingEventArgs>? MessageSending;

        /// <summary>
        /// Raised when an outbound remote message has been sent.
        /// </summary>
        public event EventHandler<IRemoteMessage>? MessageSent;

        /// <summary>
        /// A unique identifier for this instance, consistent between hosts and clients.
        /// </summary>
        public string Id { get; }

        /// <inheritdoc cref="RemotingMode" />
        public RemotingMode Mode => MessageHandler.Mode;

        /// <summary>
        /// Indicates the matching mode used when generating and comparing a <see cref="IRemoteMemberMessage.TargetMemberSignature"/>. 
        /// In addition to the options specified here, the <see cref="MemberRemote.Id"/> must match for messages to be received.
        /// <para/>
        /// If you're unsure which option to use, stick with <see cref="MemberSignatureScope.AssemblyQualifiedName"/>.
        /// </summary>
        public MemberSignatureScope MemberSignatureScope => MessageHandler.MemberSignatureScope;

        /// <summary>
        /// The <see cref="IRemoteMessageHandler"/> used by this <see cref="MemberRemote"/> instance to transfer member states.
        /// </summary>
        public IRemoteMessageHandler MessageHandler { get; set; }

        internal Type Type { get; }

        internal object Instance { get; }

        internal IEnumerable<MethodInfo> Methods { get; }

        internal IEnumerable<PropertyInfo> Properties { get; }

        internal IEnumerable<EventInfo> Events { get; }

        /// <summary>
        /// Prepares and sends the given <paramref name="message"/> outbound.
        /// </summary>
        /// <param name="message">The message being emitted.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the ongoing request. Request may still be emitted.</param>
        /// <returns>This should be used in scenarios where you need to send a custom <see cref="IRemoteMessage"/>, or where you need to force emit a member change remotely without executing the change on the current node.</returns>
        public async Task SendRemotingMessageAsync(IRemoteMessage message, CancellationToken? cancellationToken = null)
        {
            var eventArgs = new RemoteMessageSendingEventArgs(message);
            MessageSending?.Invoke(this, eventArgs);

            if (eventArgs.Handled)
                return;

            await MessageHandler.InitAsync();
            await MessageHandler.SendMessageAsync(message, cancellationToken);

            MessageSent?.Invoke(this, message);
        }

        private void HandleIncomingRemotePropertyChange(RemotePropertyChangeMessage propertyChangeMessage)
        {
            var propertyInfo = Properties.FirstOrDefault(x => CreateMemberSignature(x, MemberSignatureScope) == propertyChangeMessage.TargetMemberSignature);

            if (propertyInfo == default)
                return;

            lock (MemberHandleExpectancyMap)
                MemberHandleExpectancyMap.Add(Thread.CurrentThread.ManagedThreadId, Instance);

            if (!IsValidRemotingDirection(propertyInfo, isReceiving: true))
            {
                lock (MemberHandleExpectancyMap)
                    MemberHandleExpectancyMap.Remove(Thread.CurrentThread.ManagedThreadId);

                return;
            }

            var originalType = Type.GetType(propertyChangeMessage.AssemblyQualifiedName);
            var mostDerivedType = propertyChangeMessage.NewValue?.GetType();

            // If the received value's type doesn't match the original type.
            if (!(propertyChangeMessage.NewValue == null && !originalType.IsPrimitive) && !(originalType?.IsAssignableFrom(mostDerivedType) ?? false))
            {
                // If the original type cannot be converted automatically.
                if (!originalType?.IsSubclassOf(typeof(IConvertible)) ?? false)
                {
                    throw new NotSupportedException($"Received parameter value {mostDerivedType?.FullName ?? "null"} is not compatible with received type {originalType?.FullName ?? "null"} " +
                                                    $"and must implement {nameof(IConvertible)} for automatic type conversion. " +
                                                    $"Either handle conversion of {nameof(ParameterData)}.{nameof(ParameterData.Value)} " +
                                                    $"to this type in your {nameof(IRemoteMessageHandler.MessageConverter)} " +
                                                    $"or use a type that implements {nameof(IConvertible)}. ");
                }

                propertyChangeMessage.NewValue = Convert.ChangeType(propertyChangeMessage.NewValue, originalType);
            }

            propertyInfo.SetValue(Instance, propertyChangeMessage.NewValue);
        }

        internal void HandleIncomingRemoteMethodCall(RemoteMethodCallMessage methodCallMsg)
        {
            var methodInfo = Methods.FirstOrDefault(x => CreateMemberSignature(x, MemberSignatureScope) == methodCallMsg.TargetMemberSignature);

            if (methodInfo == default)
                return;

            lock (MemberHandleExpectancyMap)
                MemberHandleExpectancyMap.Add(Thread.CurrentThread.ManagedThreadId, Instance);

            if (!IsValidRemotingDirection(methodInfo, isReceiving: true))
            {
                lock (MemberHandleExpectancyMap)
                    MemberHandleExpectancyMap.Remove(Thread.CurrentThread.ManagedThreadId);

                return;
            }

            var parameterValues = new List<object?>();
            var paramInfos = methodInfo.GetParameters();
            var paramDatas = methodCallMsg.Parameters.ToArray();

            for (var i = 0; i < paramInfos.Length; i++)
            {
                var paramInfo = paramInfos[i];
                var parameterData = paramDatas[i];

                var originalType = Type.GetType(parameterData.AssemblyQualifiedName);
                var mostDerivedType = parameterData.Value?.GetType();

                // If the received value's type doesn't match the original type.
                if (!(parameterData.Value == null && !originalType.IsPrimitive) && !(originalType?.IsAssignableFrom(mostDerivedType) ?? false))
                {
                    // If the original type cannot be converted automatically.
                    if (!originalType?.IsSubclassOf(typeof(IConvertible)) ?? false)
                    {
                        throw new NotSupportedException($"Received parameter value {mostDerivedType?.FullName ?? "null"} is not compatible with received type {originalType?.FullName ?? "null"} " +
                                                        $"and must implement {nameof(IConvertible)} for automatic type conversion. " +
                                                        $"Either handle conversion of {nameof(ParameterData)}.{nameof(ParameterData.Value)} " +
                                                        $"to this type in your {nameof(IRemoteMessageHandler.MessageConverter)} " +
                                                        $"or use a type that implements {nameof(IConvertible)}. ");
                    }

                    parameterData.Value = Convert.ChangeType(parameterData.Value, originalType);
                }

                parameterValues.Add(parameterData.Value);
            }

            methodInfo?.Invoke(Instance, parameterValues.ToArray());
        }

        private static List<ParameterData> CreateMethodParameterData(MethodBase method, object?[] parameterValues)
        {
            var parameterInfo = method.GetParameters();
            var paramData = new List<ParameterData>();

            for (int i = 0; i < parameterInfo.Length; i++)
            {
                ParameterInfo? parameter = parameterInfo[i];

                paramData.Add(new ParameterData()
                {
                    AssemblyQualifiedName = parameter.ParameterType.AssemblyQualifiedName,
                    Value = parameterValues[i],
                });
            }

            return paramData;
        }

        private bool IsValidRemotingDirection(MemberInfo memberInfo, bool isReceiving)
        {
            var attribute = memberInfo.GetCustomAttribute<RemoteOptionsAttribute>();
            if (attribute is null)
            {
                var declaringClassType = memberInfo.DeclaringType;
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

            return (isReceiving && targetInbound) || (!isReceiving && targetOutbound);
        }

        /// <summary>
        /// Creates a member signature that is internally used to identify members in a type when sending/receiving data.
        /// </summary>
        /// <param name="memberInfo">The <see cref="MemberInfo"/> to generate a signature for.</param>
        /// <param name="matchingKind">The <see cref="Remoting.MemberSignatureScope"/> to use for generating a signature.</param>
        /// <returns>A unique member signature for the given <see cref="MemberInfo"/>.</returns>
        /// <remarks>
        /// Useful in scenarios where you need to construct your own <see cref="IRemoteMemberMessage"/> instance.
        /// </remarks>
        public static string CreateMemberSignature(MemberInfo memberInfo, MemberSignatureScope matchingKind)
        {
            return matchingKind switch
            {
                MemberSignatureScope.AssemblyQualifiedName => CreateAssemblyMemberSignature(memberInfo),
                MemberSignatureScope.FullName => CreateFullNameMemberSignature(memberInfo),
                MemberSignatureScope.DeclaringType => CreateDeclaringTypeMemberSignature(memberInfo),
                MemberSignatureScope.MemberName => CreateMemberNameMemberSignature(memberInfo),
                _ => ThrowHelper.ThrowArgumentOutOfRangeException<string>(),
            };
        }

        private static string CreateMemberNameMemberSignature(MemberInfo memberInfo)
        {
            if (memberInfo is MethodBase methodBase)
                return $"{methodBase.Name} {string.Join(" ", methodBase.GetParameters().Select(x => $"{x.ParameterType.Name}.{x.Name}"))}";

            if (memberInfo is PropertyInfo propertyInfo)
                return $"{propertyInfo.PropertyType.Name} {propertyInfo.Name}";

            if (memberInfo is Type type)
                return $"{type.Name}";

            throw new NotSupportedException();
        }

        private static string CreateDeclaringTypeMemberSignature(MemberInfo memberInfo)
        {
            if (memberInfo is MethodBase methodBase)
                return $"{methodBase.DeclaringType.Name} {methodBase.Name} {string.Join(" ", methodBase.GetParameters().Select(x => $"{x.ParameterType.Name}.{x.Name}"))}";

            if (memberInfo is PropertyInfo propertyInfo)
                return $"{propertyInfo.DeclaringType.Name} {propertyInfo.PropertyType.Name} {propertyInfo.Name}";

            if (memberInfo is Type type)
                return $"{type.DeclaringType.Name} {type.Name}";

            throw new NotSupportedException();
        }

        private static string CreateFullNameMemberSignature(MemberInfo memberInfo)
        {
            if (memberInfo is MethodBase methodBase)
                return $"{methodBase.DeclaringType.FullName} {methodBase.Name} {string.Join(" ", methodBase.GetParameters().Select(x => $"{x.ParameterType.Name}.{x.Name}"))}";

            if (memberInfo is PropertyInfo propertyInfo)
                return $"{propertyInfo.DeclaringType.FullName} {propertyInfo.PropertyType.Name} {propertyInfo.Name}";

            if (memberInfo is Type type)
                return $"{type.DeclaringType.FullName} {type.Name}";

            throw new NotSupportedException();
        }

        private static string CreateAssemblyMemberSignature(MemberInfo memberInfo)
        {
            if (memberInfo is MethodBase methodBase)
                return $"{methodBase.DeclaringType.AssemblyQualifiedName} {methodBase.Name} {string.Join(" ", methodBase.GetParameters().Select(x => $"{x.ParameterType.Name}.{x.Name}"))}";

            if (memberInfo is PropertyInfo propertyInfo)
            {
                return $"{propertyInfo.DeclaringType.AssemblyQualifiedName} {propertyInfo.PropertyType.Name} {propertyInfo.Name}";
            }

            if (memberInfo is Type type)
            {
                return type.AssemblyQualifiedName;
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
