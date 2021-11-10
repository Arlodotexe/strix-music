using Microsoft.Toolkit.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OwlCore.Extensions;
using OwlCore.Remoting;
using OwlCore.Tests.Remoting.Mock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OwlCore.Tests.Remoting
{
    public partial class MemberRemoteTests
    {
        private static Dictionary<MemberType, object> _defaultTestValues = new Dictionary<MemberType, object>
        {
            { MemberType.Object, new AbstractUI.Models.AbstractButton("testid", "test button") },
            { MemberType.Enum, RemotingDirection.Bidirectional },
            { MemberType.Struct, DateTime.Today },
            { MemberType.Primitive, 5 },
        };

        // T is needed for generic returns and boxing / unboxing as objects.
        public static T GetDefaultTestValue<T>(MemberType paramType) => (T)_defaultTestValues[paramType];

        // T is needed for generic returns and boxing / unboxing as objects.
        public static T GetDefaultTestValue<T>(MethodReturnType returnType) => returnType switch
        {
            MethodReturnType.None => throw new NotSupportedException(),
            MethodReturnType.Void => throw new NotSupportedException(),
            MethodReturnType.Enum => GetDefaultTestValue<T>(MemberType.Enum),
            MethodReturnType.Struct => GetDefaultTestValue<T>(MemberType.Struct),
            MethodReturnType.Object => GetDefaultTestValue<T>(MemberType.Object),
            MethodReturnType.Primitive => GetDefaultTestValue<T>(MemberType.Primitive),
            MethodReturnType.Task => throw new NotSupportedException(),
            MethodReturnType.TaskEnum => GetDefaultTestValue<T>(MemberType.Enum),
            MethodReturnType.TaskStruct => GetDefaultTestValue<T>(MemberType.Struct),
            MethodReturnType.TaskObject => GetDefaultTestValue<T>(MemberType.Object),
            MethodReturnType.TaskPrimitive => GetDefaultTestValue<T>(MemberType.Primitive),
            _ => throw new NotSupportedException(),
        };

        [DataRow(RemotingDirection.None, false, RemotingMode.Full, RemotingMode.Client)]
        [DataRow(RemotingDirection.None, false, RemotingMode.Full, RemotingMode.Host)]
        [DataRow(RemotingDirection.None, false, RemotingMode.Host, RemotingMode.Full)]
        [DataRow(RemotingDirection.None, false, RemotingMode.Client, RemotingMode.Full)]
        [DataRow(RemotingDirection.None, false, RemotingMode.Host, RemotingMode.Client)]
        [DataRow(RemotingDirection.None, false, RemotingMode.Client, RemotingMode.Host)]
        [DataRow(RemotingDirection.None, false, RemotingMode.Host, RemotingMode.Host)]
        [DataRow(RemotingDirection.None, false, RemotingMode.Client, RemotingMode.Client)]
        [TestMethod, Timeout(5000)]
        public Task RemoteMethod_None(RemotingDirection direction, bool allNonFirstNodesShouldReceive, RemotingMode senderMode, params RemotingMode[] listenerModes)
        {
            return RemoteMethodTest_Internal(direction, allNonFirstNodesShouldReceive, senderMode, listenerModes);
        }

        [DataRow(RemotingDirection.Bidirectional, true, RemotingMode.Full, RemotingMode.Client)]
        [DataRow(RemotingDirection.Bidirectional, true, RemotingMode.Full, RemotingMode.Host)]
        [DataRow(RemotingDirection.Bidirectional, true, RemotingMode.Client, RemotingMode.Full)]
        [DataRow(RemotingDirection.Bidirectional, true, RemotingMode.Host, RemotingMode.Full)]

        [DataRow(RemotingDirection.Bidirectional, true, RemotingMode.Host, RemotingMode.Host)]
        [DataRow(RemotingDirection.Bidirectional, true, RemotingMode.Client, RemotingMode.Client)]
        [DataRow(RemotingDirection.Bidirectional, true, RemotingMode.Host, RemotingMode.Client)]
        [DataRow(RemotingDirection.Bidirectional, true, RemotingMode.Client, RemotingMode.Host)]
        [TestMethod, Timeout(5000)]
        public Task RemoteMethod_Bidirectional(RemotingDirection direction, bool allNonFirstNodesShouldReceive, RemotingMode senderMode, params RemotingMode[] listenerModes)
        {
            return RemoteMethodTest_Internal(direction, allNonFirstNodesShouldReceive, senderMode, listenerModes);
        }

        [DataRow(RemotingDirection.ClientToHost, true, RemotingMode.Client, RemotingMode.Full)]
        [DataRow(RemotingDirection.ClientToHost, false, RemotingMode.Host, RemotingMode.Full)]
        [DataRow(RemotingDirection.ClientToHost, false, RemotingMode.Full, RemotingMode.Client)]
        [DataRow(RemotingDirection.ClientToHost, true, RemotingMode.Full, RemotingMode.Host)]

        [DataRow(RemotingDirection.ClientToHost, false, RemotingMode.Client, RemotingMode.Client)]
        [DataRow(RemotingDirection.ClientToHost, false, RemotingMode.Host, RemotingMode.Host)]
        [DataRow(RemotingDirection.ClientToHost, true, RemotingMode.Client, RemotingMode.Host)]
        [DataRow(RemotingDirection.ClientToHost, false, RemotingMode.Host, RemotingMode.Client)]
        [TestMethod, Timeout(5000)]
        public Task RemoteMethod_ClientToHost(RemotingDirection direction, bool allNonFirstNodesShouldReceive, RemotingMode senderMode, params RemotingMode[] listenerModes)
        {
            return RemoteMethodTest_Internal(direction, allNonFirstNodesShouldReceive, senderMode, listenerModes);
        }

        [DataRow(RemotingDirection.Inbound, false, RemotingMode.Client, RemotingMode.Full)]
        [DataRow(RemotingDirection.Inbound, false, RemotingMode.Host, RemotingMode.Full)]
        [DataRow(RemotingDirection.Inbound, false, RemotingMode.Full, RemotingMode.Client)]
        [DataRow(RemotingDirection.Inbound, false, RemotingMode.Full, RemotingMode.Host)]

        [DataRow(RemotingDirection.Inbound, false, RemotingMode.Host, RemotingMode.Host)]
        [DataRow(RemotingDirection.Inbound, false, RemotingMode.Client, RemotingMode.Client)]
        [DataRow(RemotingDirection.Inbound, false, RemotingMode.Client, RemotingMode.Host)]
        [DataRow(RemotingDirection.Inbound, false, RemotingMode.Host, RemotingMode.Client)]
        [TestMethod, Timeout(5000)]
        public Task RemoteMethod_Inbound(RemotingDirection direction, bool allNonFirstNodesShouldReceive, RemotingMode senderMode, params RemotingMode[] listenerModes)
        {
            return RemoteMethodTest_Internal(direction, allNonFirstNodesShouldReceive, senderMode, listenerModes);
        }

        [DataRow(RemotingDirection.InboundHost, false, RemotingMode.Client, RemotingMode.Full)]
        [DataRow(RemotingDirection.InboundHost, false, RemotingMode.Host, RemotingMode.Full)]
        [DataRow(RemotingDirection.InboundHost, false, RemotingMode.Full, RemotingMode.Client)]
        [DataRow(RemotingDirection.InboundHost, false, RemotingMode.Full, RemotingMode.Host)]

        [DataRow(RemotingDirection.InboundHost, false, RemotingMode.Host, RemotingMode.Host)]
        [DataRow(RemotingDirection.InboundHost, false, RemotingMode.Client, RemotingMode.Client)]
        [DataRow(RemotingDirection.InboundHost, false, RemotingMode.Client, RemotingMode.Host)]
        [DataRow(RemotingDirection.InboundHost, false, RemotingMode.Host, RemotingMode.Client)]
        [TestMethod]
        public Task RemoteMethod_InboundHost(RemotingDirection direction, bool allNonFirstNodesShouldReceive, RemotingMode senderMode, params RemotingMode[] listenerModes)
        {
            return RemoteMethodTest_Internal(direction, allNonFirstNodesShouldReceive, senderMode, listenerModes);
        }

        [DataRow(RemotingDirection.InboundClient, false, RemotingMode.Client, RemotingMode.Full)]
        [DataRow(RemotingDirection.InboundClient, false, RemotingMode.Host, RemotingMode.Full)]
        [DataRow(RemotingDirection.InboundClient, false, RemotingMode.Full, RemotingMode.Client)]
        [DataRow(RemotingDirection.InboundClient, false, RemotingMode.Full, RemotingMode.Host)]

        [DataRow(RemotingDirection.InboundClient, false, RemotingMode.Host, RemotingMode.Host)]
        [DataRow(RemotingDirection.InboundClient, false, RemotingMode.Client, RemotingMode.Client)]
        [DataRow(RemotingDirection.InboundClient, false, RemotingMode.Client, RemotingMode.Host)]
        [DataRow(RemotingDirection.InboundClient, false, RemotingMode.Host, RemotingMode.Client)]
        [TestMethod, Timeout(5000)]
        public Task RemoteMethod_InboundClient(RemotingDirection direction, bool allNonFirstNodesShouldReceive, RemotingMode senderMode, params RemotingMode[] listenerModes)
        {
            return RemoteMethodTest_Internal(direction, allNonFirstNodesShouldReceive, senderMode, listenerModes);
        }

        [DataRow(RemotingDirection.Outbound, false, RemotingMode.Client, RemotingMode.Full)]
        [DataRow(RemotingDirection.Outbound, false, RemotingMode.Host, RemotingMode.Full)]
        [DataRow(RemotingDirection.Outbound, false, RemotingMode.Full, RemotingMode.Client)]
        [DataRow(RemotingDirection.Outbound, false, RemotingMode.Full, RemotingMode.Host)]

        [DataRow(RemotingDirection.Outbound, false, RemotingMode.Host, RemotingMode.Host)]
        [DataRow(RemotingDirection.Outbound, false, RemotingMode.Client, RemotingMode.Client)]
        [DataRow(RemotingDirection.Outbound, false, RemotingMode.Client, RemotingMode.Host)]
        [DataRow(RemotingDirection.Outbound, false, RemotingMode.Host, RemotingMode.Client)]
        [TestMethod, Timeout(5000)]
        public Task RemoteMethod_Outbound(RemotingDirection direction, bool allNonFirstNodesShouldReceive, RemotingMode senderMode, params RemotingMode[] listenerModes)
        {
            return RemoteMethodTest_Internal(direction, allNonFirstNodesShouldReceive, senderMode, listenerModes);
        }

        [DataRow(RemotingDirection.OutboundHost, false, RemotingMode.Client, RemotingMode.Full)]
        [DataRow(RemotingDirection.OutboundHost, false, RemotingMode.Host, RemotingMode.Full)]
        [DataRow(RemotingDirection.OutboundHost, false, RemotingMode.Full, RemotingMode.Client)]
        [DataRow(RemotingDirection.OutboundHost, false, RemotingMode.Full, RemotingMode.Host)]

        [DataRow(RemotingDirection.OutboundHost, false, RemotingMode.Host, RemotingMode.Host)]
        [DataRow(RemotingDirection.OutboundHost, false, RemotingMode.Client, RemotingMode.Client)]
        [DataRow(RemotingDirection.OutboundHost, false, RemotingMode.Client, RemotingMode.Host)]
        [DataRow(RemotingDirection.OutboundHost, false, RemotingMode.Host, RemotingMode.Client)]
        [TestMethod, Timeout(5000)]
        public Task RemoteMethod_OutboundHost(RemotingDirection direction, bool allNonFirstNodesShouldReceive, RemotingMode senderMode, params RemotingMode[] listenerModes)
        {
            return RemoteMethodTest_Internal(direction, allNonFirstNodesShouldReceive, senderMode, listenerModes);
        }

        [DataRow(RemotingDirection.OutboundClient, false, RemotingMode.Client, RemotingMode.Full)]
        [DataRow(RemotingDirection.OutboundClient, false, RemotingMode.Host, RemotingMode.Full)]
        [DataRow(RemotingDirection.OutboundClient, false, RemotingMode.Full, RemotingMode.Client)]
        [DataRow(RemotingDirection.OutboundClient, false, RemotingMode.Full, RemotingMode.Host)]

        [DataRow(RemotingDirection.OutboundClient, false, RemotingMode.Host, RemotingMode.Host)]
        [DataRow(RemotingDirection.OutboundClient, false, RemotingMode.Client, RemotingMode.Client)]
        [DataRow(RemotingDirection.OutboundClient, false, RemotingMode.Client, RemotingMode.Host)]
        [DataRow(RemotingDirection.OutboundClient, false, RemotingMode.Host, RemotingMode.Client)]
        [TestMethod, Timeout(5000)]
        public Task RemoteMethod_OutboundClient(RemotingDirection direction, bool allNonFirstNodesShouldReceive, RemotingMode senderMode, params RemotingMode[] listenerModes)
        {
            return RemoteMethodTest_Internal(direction, allNonFirstNodesShouldReceive, senderMode, listenerModes);
        }

        private async Task RemoteMethodTest_Internal(RemotingDirection direction, bool allNonFirstNodesShouldReceive, RemotingMode senderMode, params RemotingMode[] listenerModes)
        {
            var returnTypesToTest = new MethodReturnType[]
            {
                MethodReturnType.Void,
                MethodReturnType.Enum,
                MethodReturnType.Struct,
                MethodReturnType.Object,
                MethodReturnType.Primitive,

                MethodReturnType.Task,
                MethodReturnType.TaskEnum,
                MethodReturnType.TaskStruct,
                MethodReturnType.TaskObject,
                MethodReturnType.TaskPrimitive,
            };

            var paramTypesToTestPerReturnType = new Dictionary<MemberType, MethodReturnType[]>()
            {
                { MemberType.None, returnTypesToTest },
                { MemberType.Enum, returnTypesToTest },
                { MemberType.Struct, returnTypesToTest },
                { MemberType.Object, returnTypesToTest },
                { MemberType.Primitive, returnTypesToTest },
            };

            var combinationsToTest = paramTypesToTestPerReturnType.SelectMany(x => x.Value, (a, b) => new ValueTuple<MemberType, MethodReturnType>(a.Key, b));

            await combinationsToTest.InParallel(item => RemoteMethodTest_Internal(item.Item2, item.Item1, direction, allNonFirstNodesShouldReceive, senderMode, listenerModes));
        }

        private async Task RemoteMethodTest_Internal(MethodReturnType returnType, MemberType paramType, RemotingDirection direction, bool allNonFirstNodesShouldReceive, RemotingMode senderMode, params RemotingMode[] listenerModes)
        {
            // Setup sender and listeners
            var classes = CreateMemberRemoteTestClasses(senderMode.IntoList().Concat(listenerModes));
            var listenerClasses = classes.Skip(1).ToList();
            var senderClass = classes.ElementAt(0);

            // Setup listening to "remote" method invocation and catching results.
            // Values passed into params are returned directly.
            // Methods with no parameters use a default return value.
            // Remote "return" values are captured through an invoked event within the method.
            var remotelyCalledMethodCompletionSources = listenerClasses.Select(instance => (instance, new TaskCompletionSource<object?>())).ToList();

            foreach (var item in remotelyCalledMethodCompletionSources)
                NotifyCompletionOnRemoteMethodInvocation(item.instance, returnType, paramType, item.Item2);

            // Setup "local" method invocation
            object? expectedReturnValue = null;
            object? expectedParamValue = null;

            if (paramType != MemberType.None)
                expectedParamValue = GetDefaultTestValue<object>(paramType);

            if (returnType == MethodReturnType.Void)
                GetActionToInvoke(senderClass, direction, paramType)();
            else
                expectedReturnValue = GetFuncToInvoke(senderClass, direction, returnType, paramType)();

            // Internal remoting system is not synchronous, even when code is local.
            // Wait until either all methods are remotely invoked by listener classes
            // or until the test method times out.
            await Task.WhenAny(Task.Delay(200), remotelyCalledMethodCompletionSources.InParallel(x => x.Item2.Task));

            var allReceived = remotelyCalledMethodCompletionSources.All(x => x.Item2.Task.Status == TaskStatus.RanToCompletion);

            var listenersNotReceived = remotelyCalledMethodCompletionSources.Where(x => x.Item2.Task.Status != TaskStatus.RanToCompletion).ToList();
            var listenersReceived = remotelyCalledMethodCompletionSources.Where(x => x.Item2.Task.Status == TaskStatus.RanToCompletion).ToList();

            if (allNonFirstNodesShouldReceive)
            {
                Assert.IsTrue(allReceived, $"Unexpectedly timed out while waiting for remote method to be called. {listenersNotReceived.Count} listeners did not receive the message.");

                foreach (var item in remotelyCalledMethodCompletionSources)
                    AssertMatchingReceivedListenerValue(expectedParamValue ?? expectedReturnValue, item.instance, returnType, paramType, item.Item2);
            }
            else
            {
                Assert.IsFalse(allReceived, $"Expected and did not time out while waiting for remote method to be called. {listenersReceived.Count} listeners received the message.");
            }

            foreach (var listener in classes)
                listener.Dispose();
        }

        private void AssertMatchingReceivedListenerValue(object? expectedReturnOrParamValue, MemberRemoteTestClass instance, MethodReturnType returnType, MemberType paramType, TaskCompletionSource<object?> taskCompletionSource)
        {
            // Verify that the return type is the same as the locally called method.
            // Required for methods that take no parameters.
            if (returnType != MethodReturnType.None && returnType != MethodReturnType.Void)
            {
                // Funky if statement to help with debugging. Can't use Assert.AreEqual b/c it breaks with boxed enums.
                if (!((expectedReturnOrParamValue == null && taskCompletionSource.Task.Result == null) || (expectedReturnOrParamValue?.Equals(taskCompletionSource.Task.Result) ?? false)))
                {
                    Assert.IsTrue(true);
                }
            }

            // Verify that the remotely received parameters match the sender parameter values.
            // Required for methods that return void.
            if (paramType != MemberType.None)
            {
                if (!((expectedReturnOrParamValue == null && taskCompletionSource.Task.Result == null) || (expectedReturnOrParamValue?.Equals(taskCompletionSource.Task.Result) ?? false)))
                {
                    Assert.IsTrue(true);
                }
            }
        }

        // This doesn't allow for unsubscribing the anonymous event handler delegate, but unit tests are short lived and these instances are disposed at the end of each test.
        private void NotifyCompletionOnRemoteMethodInvocation(MemberRemoteTestClass instance, MethodReturnType returnType, MemberType paramType, TaskCompletionSource<object?> taskCompletionSource)
        {
            var action = paramType switch
            {
                // Parameterless methods should watch the Event with a type matches the return type
                MemberType.None => returnType switch
                {
                    MethodReturnType.Void => () => instance.RemoteMethodCalledVoid += (s, e) => taskCompletionSource.SetResult(e),
                    MethodReturnType.Enum => () => instance.RemoteMethodCalledEnum += (s, e) => taskCompletionSource.SetResult(e),
                    MethodReturnType.Struct => () => instance.RemoteMethodCalledStruct += (s, e) => taskCompletionSource.SetResult(e),
                    MethodReturnType.Object => () => instance.RemoteMethodCalledObject += (s, e) => taskCompletionSource.SetResult(e),
                    MethodReturnType.Primitive => () => instance.RemoteMethodCalledPrimitive += (s, e) => taskCompletionSource.SetResult(e),

                    MethodReturnType.Task => () => instance.RemoteMethodCalledTask += (s, e) => taskCompletionSource.SetResult(e),
                    MethodReturnType.TaskEnum => () => instance.RemoteMethodCalledTaskEnum += (s, e) => taskCompletionSource.SetResult(e.Result),
                    MethodReturnType.TaskStruct => () => instance.RemoteMethodCalledTaskStruct += (s, e) => taskCompletionSource.SetResult(e.Result),
                    MethodReturnType.TaskObject => () => instance.RemoteMethodCalledTaskObject += (s, e) => taskCompletionSource.SetResult(e.Result),
                    MethodReturnType.TaskPrimitive => () => instance.RemoteMethodCalledTaskPrimitive += (s, e) => taskCompletionSource.SetResult(e.Result),
                    _ => ThrowHelper.ThrowArgumentOutOfRangeException<Action>(),
                },
                // Methods with params should watch the Event with a type that matches the parameter
                MemberType.Enum => () => instance.RemoteMethodCalledEnum += (s, e) => taskCompletionSource.SetResult(e),
                MemberType.Struct => () => instance.RemoteMethodCalledStruct += (s, e) => taskCompletionSource.SetResult(e),
                MemberType.Object => () => instance.RemoteMethodCalledObject += (s, e) => taskCompletionSource.SetResult(e),
                MemberType.Primitive => () => instance.RemoteMethodCalledPrimitive += (s, e) => taskCompletionSource.SetResult(e),
                _ => ThrowHelper.ThrowArgumentOutOfRangeException<Action>(),
            };

            action();
        }

        // For void methods only
        private Action GetActionToInvoke(MemberRemoteTestClass instance, RemotingDirection direction, MemberType paramType)
        {
            return paramType switch
            {
                MemberType.None => GetMethodForDirection_Void_NoParam(instance, direction),
                MemberType.Enum => GetMethodForDirection_Void_EnumParam(instance, direction, GetDefaultTestValue<RemotingDirection>(paramType)),
                MemberType.Struct => GetMethodForDirection_Void_StructParam(instance, direction, GetDefaultTestValue<DateTime>(paramType)),
                MemberType.Primitive => GetMethodForDirection_Void_PrimitiveParam(instance, direction, GetDefaultTestValue<int>(paramType)),
                MemberType.Object => GetMethodForDirection_Void_ObjectParam(instance, direction, GetDefaultTestValue<object>(paramType)),
                _ => throw new NotSupportedException(),
            };

            throw new NotImplementedException();
        }

        // For methods with return values only
        private Func<object> GetFuncToInvoke(MemberRemoteTestClass instance, RemotingDirection direction, MethodReturnType returnType, MemberType paramType)
        {
            switch (paramType)
            {
                case MemberType.None:
                    return returnType switch
                    {
                        MethodReturnType.Enum => GetMethodForDirection_Enum_NoParam(instance, direction),
                        MethodReturnType.Struct => GetMethodForDirection_Struct_NoParam(instance, direction),
                        MethodReturnType.Object => GetMethodForDirection_Object_NoParam(instance, direction),
                        MethodReturnType.Primitive => GetMethodForDirection_Primitive_NoParam(instance, direction),
                        MethodReturnType.Task => GetMethodForDirection_Task_NoParam(instance, direction),
                        MethodReturnType.TaskEnum => GetMethodForDirection_TaskEnum_NoParam(instance, direction),
                        MethodReturnType.TaskStruct => GetMethodForDirection_TaskStruct_NoParam(instance, direction),
                        MethodReturnType.TaskObject => GetMethodForDirection_TaskObject_NoParam(instance, direction),
                        MethodReturnType.TaskPrimitive => GetMethodForDirection_TaskPrimitive_NoParam(instance, direction),
                        _ => ThrowHelper.ThrowArgumentOutOfRangeException<Func<object>>(),
                    };

                case MemberType.Enum:
                    var val = GetDefaultTestValue<RemotingDirection>(MemberType.Enum);
                    return returnType switch
                    {
                        MethodReturnType.Enum => GetMethodForDirection_Enum_EnumParam(instance, direction, val),
                        MethodReturnType.Struct => GetMethodForDirection_Struct_EnumParam(instance, direction, val),
                        MethodReturnType.Object => GetMethodForDirection_Object_EnumParam(instance, direction, val),
                        MethodReturnType.Primitive => GetMethodForDirection_Primitive_EnumParam(instance, direction, val),
                        MethodReturnType.Task => GetMethodForDirection_Task_EnumParam(instance, direction, val),
                        MethodReturnType.TaskEnum => GetMethodForDirection_TaskEnum_EnumParam(instance, direction, val),
                        MethodReturnType.TaskStruct => GetMethodForDirection_TaskStruct_EnumParam(instance, direction, val),
                        MethodReturnType.TaskObject => GetMethodForDirection_TaskObject_EnumParam(instance, direction, val),
                        MethodReturnType.TaskPrimitive => GetMethodForDirection_TaskPrimitive_EnumParam(instance, direction, val),
                        _ => ThrowHelper.ThrowArgumentOutOfRangeException<Func<object>>(),
                    };

                case MemberType.Struct:
                    {
                        var structVal = GetDefaultTestValue<DateTime>(paramType);

                        return returnType switch
                        {
                            MethodReturnType.Enum => GetMethodForDirection_Enum_StructParam(instance, direction, structVal),
                            MethodReturnType.Struct => GetMethodForDirection_Struct_StructParam(instance, direction, structVal),
                            MethodReturnType.Object => GetMethodForDirection_Object_StructParam(instance, direction, structVal),
                            MethodReturnType.Primitive => GetMethodForDirection_Primitive_StructParam(instance, direction, structVal),
                            MethodReturnType.Task => GetMethodForDirection_Task_StructParam(instance, direction, structVal),
                            MethodReturnType.TaskEnum => GetMethodForDirection_TaskEnum_StructParam(instance, direction, structVal),
                            MethodReturnType.TaskStruct => GetMethodForDirection_TaskStruct_StructParam(instance, direction, structVal),
                            MethodReturnType.TaskObject => GetMethodForDirection_TaskObject_StructParam(instance, direction, structVal),
                            MethodReturnType.TaskPrimitive => GetMethodForDirection_TaskPrimitive_StructParam(instance, direction, structVal),
                            _ => ThrowHelper.ThrowArgumentOutOfRangeException<Func<object>>(),
                        };
                    }

                case MemberType.Object:
                    {
                        var objectVal = GetDefaultTestValue<object>(paramType);

                        return returnType switch
                        {
                            MethodReturnType.Enum => GetMethodForDirection_Enum_ObjectParam(instance, direction, objectVal),
                            MethodReturnType.Struct => GetMethodForDirection_Struct_ObjectParam(instance, direction, objectVal),
                            MethodReturnType.Object => GetMethodForDirection_Object_ObjectParam(instance, direction, objectVal),
                            MethodReturnType.Primitive => GetMethodForDirection_Primitive_ObjectParam(instance, direction, objectVal),
                            MethodReturnType.Task => GetMethodForDirection_Task_ObjectParam(instance, direction, objectVal),
                            MethodReturnType.TaskEnum => GetMethodForDirection_TaskEnum_ObjectParam(instance, direction, objectVal),
                            MethodReturnType.TaskStruct => GetMethodForDirection_TaskStruct_ObjectParam(instance, direction, objectVal),
                            MethodReturnType.TaskObject => GetMethodForDirection_TaskObject_ObjectParam(instance, direction, objectVal),
                            MethodReturnType.TaskPrimitive => GetMethodForDirection_TaskPrimitive_ObjectParam(instance, direction, objectVal),
                            _ => ThrowHelper.ThrowArgumentOutOfRangeException<Func<object>>(),
                        };
                    }

                case MemberType.Primitive:
                    {
                        var primitiveVal = GetDefaultTestValue<int>(paramType);

                        return returnType switch
                        {
                            MethodReturnType.Enum => GetMethodForDirection_Enum_PrimitiveParam(instance, direction, primitiveVal),
                            MethodReturnType.Struct => GetMethodForDirection_Struct_PrimitiveParam(instance, direction, primitiveVal),
                            MethodReturnType.Object => GetMethodForDirection_Object_PrimitiveParam(instance, direction, primitiveVal),
                            MethodReturnType.Primitive => GetMethodForDirection_Primitive_PrimitiveParam(instance, direction, primitiveVal),
                            MethodReturnType.Task => GetMethodForDirection_Task_PrimitiveParam(instance, direction, primitiveVal),
                            MethodReturnType.TaskEnum => GetMethodForDirection_TaskEnum_PrimitiveParam(instance, direction, primitiveVal),
                            MethodReturnType.TaskStruct => GetMethodForDirection_TaskStruct_PrimitiveParam(instance, direction, primitiveVal),
                            MethodReturnType.TaskObject => GetMethodForDirection_TaskObject_PrimitiveParam(instance, direction, primitiveVal),
                            MethodReturnType.TaskPrimitive => GetMethodForDirection_TaskPrimitive_PrimitiveParam(instance, direction, primitiveVal),
                            _ => ThrowHelper.ThrowArgumentOutOfRangeException<Func<object>>(),
                        };
                    }

                default:
                    return ThrowHelper.ThrowArgumentOutOfRangeException<Func<object>>();
            }
        }

        #region GetMethodForDirection_NoParam

        private Action GetMethodForDirection_Void_NoParam(MemberRemoteTestClass testClass, RemotingDirection direction) => direction switch
        {
            RemotingDirection.None => () => testClass.RemoteMethod_Void_NoParam_None(),
            RemotingDirection.Bidirectional => () => testClass.RemoteMethod_Void_NoParam_Bidirectional(),
            RemotingDirection.ClientToHost => () => testClass.RemoteMethod_Void_NoParam_ClientToHost(),
            RemotingDirection.HostToClient => () => testClass.RemoteMethod_Void_NoParam_HostToClient(),
            RemotingDirection.Inbound => () => testClass.RemoteMethod_Void_NoParam_Inbound(),
            RemotingDirection.InboundClient => () => testClass.RemoteMethod_Void_NoParam_InboundClient(),
            RemotingDirection.InboundHost => () => testClass.RemoteMethod_Void_NoParam_InboundHost(),
            RemotingDirection.Outbound => () => testClass.RemoteMethod_Void_NoParam_Outbound(),
            RemotingDirection.OutboundClient => () => testClass.RemoteMethod_Void_NoParam_OutboundClient(),
            RemotingDirection.OutboundHost => () => testClass.RemoteMethod_Void_NoParam_OutboundHost(),
            _ => ThrowHelper.ThrowArgumentOutOfRangeException<Action>(),
        };

        private Func<object> GetMethodForDirection_Enum_NoParam(MemberRemoteTestClass testClass, RemotingDirection direction) => direction switch
        {
            RemotingDirection.None => () => testClass.RemoteMethod_Enum_NoParam_None(),
            RemotingDirection.Bidirectional => () => testClass.RemoteMethod_Enum_NoParam_Bidirectional(),
            RemotingDirection.ClientToHost => () => testClass.RemoteMethod_Enum_NoParam_ClientToHost(),
            RemotingDirection.HostToClient => () => testClass.RemoteMethod_Enum_NoParam_HostToClient(),
            RemotingDirection.Inbound => () => testClass.RemoteMethod_Enum_NoParam_Inbound(),
            RemotingDirection.InboundClient => () => testClass.RemoteMethod_Enum_NoParam_InboundClient(),
            RemotingDirection.InboundHost => () => testClass.RemoteMethod_Enum_NoParam_InboundHost(),
            RemotingDirection.Outbound => () => testClass.RemoteMethod_Enum_NoParam_Outbound(),
            RemotingDirection.OutboundClient => () => testClass.RemoteMethod_Enum_NoParam_OutboundClient(),
            RemotingDirection.OutboundHost => () => testClass.RemoteMethod_Enum_NoParam_OutboundHost(),
            _ => ThrowHelper.ThrowArgumentOutOfRangeException<Func<object>>(),
        };

        private Func<object> GetMethodForDirection_Struct_NoParam(MemberRemoteTestClass testClass, RemotingDirection direction) => direction switch
        {
            RemotingDirection.None => () => testClass.RemoteMethod_Struct_NoParam_None(),
            RemotingDirection.Bidirectional => () => testClass.RemoteMethod_Struct_NoParam_Bidirectional(),
            RemotingDirection.ClientToHost => () => testClass.RemoteMethod_Struct_NoParam_ClientToHost(),
            RemotingDirection.HostToClient => () => testClass.RemoteMethod_Struct_NoParam_HostToClient(),
            RemotingDirection.Inbound => () => testClass.RemoteMethod_Struct_NoParam_Inbound(),
            RemotingDirection.InboundClient => () => testClass.RemoteMethod_Struct_NoParam_InboundClient(),
            RemotingDirection.InboundHost => () => testClass.RemoteMethod_Struct_NoParam_InboundHost(),
            RemotingDirection.Outbound => () => testClass.RemoteMethod_Struct_NoParam_Outbound(),
            RemotingDirection.OutboundClient => () => testClass.RemoteMethod_Struct_NoParam_OutboundClient(),
            RemotingDirection.OutboundHost => () => testClass.RemoteMethod_Struct_NoParam_OutboundHost(),
            _ => ThrowHelper.ThrowArgumentOutOfRangeException<Func<object>>(),
        };

        private Func<object> GetMethodForDirection_Object_NoParam(MemberRemoteTestClass testClass, RemotingDirection direction) => direction switch
        {
            RemotingDirection.None => () => testClass.RemoteMethod_Object_NoParam_None(),
            RemotingDirection.Bidirectional => () => testClass.RemoteMethod_Object_NoParam_Bidirectional(),
            RemotingDirection.ClientToHost => () => testClass.RemoteMethod_Object_NoParam_ClientToHost(),
            RemotingDirection.HostToClient => () => testClass.RemoteMethod_Object_NoParam_HostToClient(),
            RemotingDirection.Inbound => () => testClass.RemoteMethod_Object_NoParam_Inbound(),
            RemotingDirection.InboundClient => () => testClass.RemoteMethod_Object_NoParam_InboundClient(),
            RemotingDirection.InboundHost => () => testClass.RemoteMethod_Object_NoParam_InboundHost(),
            RemotingDirection.Outbound => () => testClass.RemoteMethod_Object_NoParam_Outbound(),
            RemotingDirection.OutboundClient => () => testClass.RemoteMethod_Object_NoParam_OutboundClient(),
            RemotingDirection.OutboundHost => () => testClass.RemoteMethod_Object_NoParam_OutboundHost(),
            _ => ThrowHelper.ThrowArgumentOutOfRangeException<Func<object>>(),
        };

        private Func<object> GetMethodForDirection_Primitive_NoParam(MemberRemoteTestClass testClass, RemotingDirection direction) => direction switch
        {
            RemotingDirection.None => () => testClass.RemoteMethod_Primitive_NoParam_None(),
            RemotingDirection.Bidirectional => () => testClass.RemoteMethod_Primitive_NoParam_Bidirectional(),
            RemotingDirection.ClientToHost => () => testClass.RemoteMethod_Primitive_NoParam_ClientToHost(),
            RemotingDirection.HostToClient => () => testClass.RemoteMethod_Primitive_NoParam_HostToClient(),
            RemotingDirection.Inbound => () => testClass.RemoteMethod_Primitive_NoParam_Inbound(),
            RemotingDirection.InboundClient => () => testClass.RemoteMethod_Primitive_NoParam_InboundClient(),
            RemotingDirection.InboundHost => () => testClass.RemoteMethod_Primitive_NoParam_InboundHost(),
            RemotingDirection.Outbound => () => testClass.RemoteMethod_Primitive_NoParam_Outbound(),
            RemotingDirection.OutboundClient => () => testClass.RemoteMethod_Primitive_NoParam_OutboundClient(),
            RemotingDirection.OutboundHost => () => testClass.RemoteMethod_Primitive_NoParam_OutboundHost(),
            _ => ThrowHelper.ThrowArgumentOutOfRangeException<Func<object>>(),
        };

        private Func<object> GetMethodForDirection_Task_NoParam(MemberRemoteTestClass testClass, RemotingDirection direction) => direction switch
        {
            RemotingDirection.None => () => testClass.RemoteMethod_Task_NoParam_None(),
            RemotingDirection.Bidirectional => () => testClass.RemoteMethod_Task_NoParam_Bidirectional(),
            RemotingDirection.ClientToHost => () => testClass.RemoteMethod_Task_NoParam_ClientToHost(),
            RemotingDirection.HostToClient => () => testClass.RemoteMethod_Task_NoParam_HostToClient(),
            RemotingDirection.Inbound => () => testClass.RemoteMethod_Task_NoParam_Inbound(),
            RemotingDirection.InboundClient => () => testClass.RemoteMethod_Task_NoParam_InboundClient(),
            RemotingDirection.InboundHost => () => testClass.RemoteMethod_Task_NoParam_InboundHost(),
            RemotingDirection.Outbound => () => testClass.RemoteMethod_Task_NoParam_Outbound(),
            RemotingDirection.OutboundClient => () => testClass.RemoteMethod_Task_NoParam_OutboundClient(),
            RemotingDirection.OutboundHost => () => testClass.RemoteMethod_Task_NoParam_OutboundHost(),
            _ => ThrowHelper.ThrowArgumentOutOfRangeException<Func<object>>(),
        };

        private Func<object> GetMethodForDirection_TaskEnum_NoParam(MemberRemoteTestClass testClass, RemotingDirection direction) => direction switch
        {
            RemotingDirection.None => () => testClass.RemoteMethod_TaskEnum_NoParam_None().Result,
            RemotingDirection.Bidirectional => () => testClass.RemoteMethod_TaskEnum_NoParam_Bidirectional().Result,
            RemotingDirection.ClientToHost => () => testClass.RemoteMethod_TaskEnum_NoParam_ClientToHost().Result,
            RemotingDirection.HostToClient => () => testClass.RemoteMethod_TaskEnum_NoParam_HostToClient().Result,
            RemotingDirection.Inbound => () => testClass.RemoteMethod_TaskEnum_NoParam_Inbound().Result,
            RemotingDirection.InboundClient => () => testClass.RemoteMethod_TaskEnum_NoParam_InboundClient().Result,
            RemotingDirection.InboundHost => () => testClass.RemoteMethod_TaskEnum_NoParam_InboundHost().Result,
            RemotingDirection.Outbound => () => testClass.RemoteMethod_TaskEnum_NoParam_Outbound().Result,
            RemotingDirection.OutboundClient => () => testClass.RemoteMethod_TaskEnum_NoParam_OutboundClient().Result,
            RemotingDirection.OutboundHost => () => testClass.RemoteMethod_TaskEnum_NoParam_OutboundHost().Result,
            _ => ThrowHelper.ThrowArgumentOutOfRangeException<Func<object>>(),
        };

        private Func<object> GetMethodForDirection_TaskStruct_NoParam(MemberRemoteTestClass testClass, RemotingDirection direction) => direction switch
        {
            RemotingDirection.None => () => testClass.RemoteMethod_TaskStruct_NoParam_None().Result,
            RemotingDirection.Bidirectional => () => testClass.RemoteMethod_TaskStruct_NoParam_Bidirectional().Result,
            RemotingDirection.ClientToHost => () => testClass.RemoteMethod_TaskStruct_NoParam_ClientToHost().Result,
            RemotingDirection.HostToClient => () => testClass.RemoteMethod_TaskStruct_NoParam_HostToClient().Result,
            RemotingDirection.Inbound => () => testClass.RemoteMethod_TaskStruct_NoParam_Inbound().Result,
            RemotingDirection.InboundClient => () => testClass.RemoteMethod_TaskStruct_NoParam_InboundClient().Result,
            RemotingDirection.InboundHost => () => testClass.RemoteMethod_TaskStruct_NoParam_InboundHost().Result,
            RemotingDirection.Outbound => () => testClass.RemoteMethod_TaskStruct_NoParam_Outbound().Result,
            RemotingDirection.OutboundClient => () => testClass.RemoteMethod_TaskStruct_NoParam_OutboundClient().Result,
            RemotingDirection.OutboundHost => () => testClass.RemoteMethod_TaskStruct_NoParam_OutboundHost().Result,
            _ => ThrowHelper.ThrowArgumentOutOfRangeException<Func<object>>(),
        };

        private Func<object> GetMethodForDirection_TaskObject_NoParam(MemberRemoteTestClass testClass, RemotingDirection direction) => direction switch
        {
            RemotingDirection.None => () => testClass.RemoteMethod_TaskObject_NoParam_None().Result,
            RemotingDirection.Bidirectional => () => testClass.RemoteMethod_TaskObject_NoParam_Bidirectional().Result,
            RemotingDirection.ClientToHost => () => testClass.RemoteMethod_TaskObject_NoParam_ClientToHost().Result,
            RemotingDirection.HostToClient => () => testClass.RemoteMethod_TaskObject_NoParam_HostToClient().Result,
            RemotingDirection.Inbound => () => testClass.RemoteMethod_TaskObject_NoParam_Inbound().Result,
            RemotingDirection.InboundClient => () => testClass.RemoteMethod_TaskObject_NoParam_InboundClient().Result,
            RemotingDirection.InboundHost => () => testClass.RemoteMethod_TaskObject_NoParam_InboundHost().Result,
            RemotingDirection.Outbound => () => testClass.RemoteMethod_TaskObject_NoParam_Outbound().Result,
            RemotingDirection.OutboundClient => () => testClass.RemoteMethod_TaskObject_NoParam_OutboundClient().Result,
            RemotingDirection.OutboundHost => () => testClass.RemoteMethod_TaskObject_NoParam_OutboundHost().Result,
            _ => ThrowHelper.ThrowArgumentOutOfRangeException<Func<object>>(),
        };

        private Func<object> GetMethodForDirection_TaskPrimitive_NoParam(MemberRemoteTestClass testClass, RemotingDirection direction) => direction switch
        {
            RemotingDirection.None => () => testClass.RemoteMethod_TaskPrimitive_NoParam_None().Result,
            RemotingDirection.Bidirectional => () => testClass.RemoteMethod_TaskPrimitive_NoParam_Bidirectional().Result,
            RemotingDirection.ClientToHost => () => testClass.RemoteMethod_TaskPrimitive_NoParam_ClientToHost().Result,
            RemotingDirection.HostToClient => () => testClass.RemoteMethod_TaskPrimitive_NoParam_HostToClient().Result,
            RemotingDirection.Inbound => () => testClass.RemoteMethod_TaskPrimitive_NoParam_Inbound().Result,
            RemotingDirection.InboundClient => () => testClass.RemoteMethod_TaskPrimitive_NoParam_InboundClient().Result,
            RemotingDirection.InboundHost => () => testClass.RemoteMethod_TaskPrimitive_NoParam_InboundHost().Result,
            RemotingDirection.Outbound => () => testClass.RemoteMethod_TaskPrimitive_NoParam_Outbound().Result,
            RemotingDirection.OutboundClient => () => testClass.RemoteMethod_TaskPrimitive_NoParam_OutboundClient().Result,
            RemotingDirection.OutboundHost => () => testClass.RemoteMethod_TaskPrimitive_NoParam_OutboundHost().Result,
            _ => ThrowHelper.ThrowArgumentOutOfRangeException<Func<object>>(),
        };

        #endregion

        #region GetMethodForDirection_EnumParam

        private Action GetMethodForDirection_Void_EnumParam(MemberRemoteTestClass testClass, RemotingDirection direction, RemotingDirection paramValue) => direction switch
        {
            RemotingDirection.None => () => testClass.RemoteMethod_Void_EnumParam_None(paramValue),
            RemotingDirection.Bidirectional => () => testClass.RemoteMethod_Void_EnumParam_Bidirectional(paramValue),
            RemotingDirection.ClientToHost => () => testClass.RemoteMethod_Void_EnumParam_ClientToHost(paramValue),
            RemotingDirection.HostToClient => () => testClass.RemoteMethod_Void_EnumParam_HostToClient(paramValue),
            RemotingDirection.Inbound => () => testClass.RemoteMethod_Void_EnumParam_Inbound(paramValue),
            RemotingDirection.InboundClient => () => testClass.RemoteMethod_Void_EnumParam_InboundClient(paramValue),
            RemotingDirection.InboundHost => () => testClass.RemoteMethod_Void_EnumParam_InboundHost(paramValue),
            RemotingDirection.Outbound => () => testClass.RemoteMethod_Void_EnumParam_Outbound(paramValue),
            RemotingDirection.OutboundClient => () => testClass.RemoteMethod_Void_EnumParam_OutboundClient(paramValue),
            RemotingDirection.OutboundHost => () => testClass.RemoteMethod_Void_EnumParam_OutboundHost(paramValue),
            _ => ThrowHelper.ThrowArgumentOutOfRangeException<Action>(),
        };

        private Func<object> GetMethodForDirection_Enum_EnumParam(MemberRemoteTestClass testClass, RemotingDirection direction, RemotingDirection paramValue) => direction switch
        {
            RemotingDirection.None => () => testClass.RemoteMethod_Enum_EnumParam_None(paramValue),
            RemotingDirection.Bidirectional => () => testClass.RemoteMethod_Enum_EnumParam_Bidirectional(paramValue),
            RemotingDirection.ClientToHost => () => testClass.RemoteMethod_Enum_EnumParam_ClientToHost(paramValue),
            RemotingDirection.HostToClient => () => testClass.RemoteMethod_Enum_EnumParam_HostToClient(paramValue),
            RemotingDirection.Inbound => () => testClass.RemoteMethod_Enum_EnumParam_Inbound(paramValue),
            RemotingDirection.InboundClient => () => testClass.RemoteMethod_Enum_EnumParam_InboundClient(paramValue),
            RemotingDirection.InboundHost => () => testClass.RemoteMethod_Enum_EnumParam_InboundHost(paramValue),
            RemotingDirection.Outbound => () => testClass.RemoteMethod_Enum_EnumParam_Outbound(paramValue),
            RemotingDirection.OutboundClient => () => testClass.RemoteMethod_Enum_EnumParam_OutboundClient(paramValue),
            RemotingDirection.OutboundHost => () => testClass.RemoteMethod_Enum_EnumParam_OutboundHost(paramValue),
            _ => ThrowHelper.ThrowArgumentOutOfRangeException<Func<object>>(),
        };

        private Func<object> GetMethodForDirection_Struct_EnumParam(MemberRemoteTestClass testClass, RemotingDirection direction, RemotingDirection paramValue) => direction switch
        {
            RemotingDirection.None => () => testClass.RemoteMethod_Struct_EnumParam_None(paramValue),
            RemotingDirection.Bidirectional => () => testClass.RemoteMethod_Struct_EnumParam_Bidirectional(paramValue),
            RemotingDirection.ClientToHost => () => testClass.RemoteMethod_Struct_EnumParam_ClientToHost(paramValue),
            RemotingDirection.HostToClient => () => testClass.RemoteMethod_Struct_EnumParam_HostToClient(paramValue),
            RemotingDirection.Inbound => () => testClass.RemoteMethod_Struct_EnumParam_Inbound(paramValue),
            RemotingDirection.InboundClient => () => testClass.RemoteMethod_Struct_EnumParam_InboundClient(paramValue),
            RemotingDirection.InboundHost => () => testClass.RemoteMethod_Struct_EnumParam_InboundHost(paramValue),
            RemotingDirection.Outbound => () => testClass.RemoteMethod_Struct_EnumParam_Outbound(paramValue),
            RemotingDirection.OutboundClient => () => testClass.RemoteMethod_Struct_EnumParam_OutboundClient(paramValue),
            RemotingDirection.OutboundHost => () => testClass.RemoteMethod_Struct_EnumParam_OutboundHost(paramValue),
            _ => ThrowHelper.ThrowArgumentOutOfRangeException<Func<object>>(),
        };

        private Func<object> GetMethodForDirection_Object_EnumParam(MemberRemoteTestClass testClass, RemotingDirection direction, RemotingDirection paramValue) => direction switch
        {
            RemotingDirection.None => () => testClass.RemoteMethod_Object_EnumParam_None(paramValue),
            RemotingDirection.Bidirectional => () => testClass.RemoteMethod_Object_EnumParam_Bidirectional(paramValue),
            RemotingDirection.ClientToHost => () => testClass.RemoteMethod_Object_EnumParam_ClientToHost(paramValue),
            RemotingDirection.HostToClient => () => testClass.RemoteMethod_Object_EnumParam_HostToClient(paramValue),
            RemotingDirection.Inbound => () => testClass.RemoteMethod_Object_EnumParam_Inbound(paramValue),
            RemotingDirection.InboundClient => () => testClass.RemoteMethod_Object_EnumParam_InboundClient(paramValue),
            RemotingDirection.InboundHost => () => testClass.RemoteMethod_Object_EnumParam_InboundHost(paramValue),
            RemotingDirection.Outbound => () => testClass.RemoteMethod_Object_EnumParam_Outbound(paramValue),
            RemotingDirection.OutboundClient => () => testClass.RemoteMethod_Object_EnumParam_OutboundClient(paramValue),
            RemotingDirection.OutboundHost => () => testClass.RemoteMethod_Object_EnumParam_OutboundHost(paramValue),
            _ => ThrowHelper.ThrowArgumentOutOfRangeException<Func<object>>(),
        };

        private Func<object> GetMethodForDirection_Primitive_EnumParam(MemberRemoteTestClass testClass, RemotingDirection direction, RemotingDirection paramValue) => direction switch
        {
            RemotingDirection.None => () => testClass.RemoteMethod_Primitive_EnumParam_None(paramValue),
            RemotingDirection.Bidirectional => () => testClass.RemoteMethod_Primitive_EnumParam_Bidirectional(paramValue),
            RemotingDirection.ClientToHost => () => testClass.RemoteMethod_Primitive_EnumParam_ClientToHost(paramValue),
            RemotingDirection.HostToClient => () => testClass.RemoteMethod_Primitive_EnumParam_HostToClient(paramValue),
            RemotingDirection.Inbound => () => testClass.RemoteMethod_Primitive_EnumParam_Inbound(paramValue),
            RemotingDirection.InboundClient => () => testClass.RemoteMethod_Primitive_EnumParam_InboundClient(paramValue),
            RemotingDirection.InboundHost => () => testClass.RemoteMethod_Primitive_EnumParam_InboundHost(paramValue),
            RemotingDirection.Outbound => () => testClass.RemoteMethod_Primitive_EnumParam_Outbound(paramValue),
            RemotingDirection.OutboundClient => () => testClass.RemoteMethod_Primitive_EnumParam_OutboundClient(paramValue),
            RemotingDirection.OutboundHost => () => testClass.RemoteMethod_Primitive_EnumParam_OutboundHost(paramValue),
            _ => ThrowHelper.ThrowArgumentOutOfRangeException<Func<object>>(),
        };

        private Func<object> GetMethodForDirection_Task_EnumParam(MemberRemoteTestClass testClass, RemotingDirection direction, RemotingDirection paramValue) => direction switch
        {
            RemotingDirection.None => () => testClass.RemoteMethod_Task_EnumParam_None(paramValue),
            RemotingDirection.Bidirectional => () => testClass.RemoteMethod_Task_EnumParam_Bidirectional(paramValue),
            RemotingDirection.ClientToHost => () => testClass.RemoteMethod_Task_EnumParam_ClientToHost(paramValue),
            RemotingDirection.HostToClient => () => testClass.RemoteMethod_Task_EnumParam_HostToClient(paramValue),
            RemotingDirection.Inbound => () => testClass.RemoteMethod_Task_EnumParam_Inbound(paramValue),
            RemotingDirection.InboundClient => () => testClass.RemoteMethod_Task_EnumParam_InboundClient(paramValue),
            RemotingDirection.InboundHost => () => testClass.RemoteMethod_Task_EnumParam_InboundHost(paramValue),
            RemotingDirection.Outbound => () => testClass.RemoteMethod_Task_EnumParam_Outbound(paramValue),
            RemotingDirection.OutboundClient => () => testClass.RemoteMethod_Task_EnumParam_OutboundClient(paramValue),
            RemotingDirection.OutboundHost => () => testClass.RemoteMethod_Task_EnumParam_OutboundHost(paramValue),
            _ => ThrowHelper.ThrowArgumentOutOfRangeException<Func<object>>(),
        };

        private Func<object> GetMethodForDirection_TaskEnum_EnumParam(MemberRemoteTestClass testClass, RemotingDirection direction, RemotingDirection paramValue) => direction switch
        {
            RemotingDirection.None => () => testClass.RemoteMethod_TaskEnum_EnumParam_None(paramValue).Result,
            RemotingDirection.Bidirectional => () => testClass.RemoteMethod_TaskEnum_EnumParam_Bidirectional(paramValue).Result,
            RemotingDirection.ClientToHost => () => testClass.RemoteMethod_TaskEnum_EnumParam_ClientToHost(paramValue).Result,
            RemotingDirection.HostToClient => () => testClass.RemoteMethod_TaskEnum_EnumParam_HostToClient(paramValue).Result,
            RemotingDirection.Inbound => () => testClass.RemoteMethod_TaskEnum_EnumParam_Inbound(paramValue).Result,
            RemotingDirection.InboundClient => () => testClass.RemoteMethod_TaskEnum_EnumParam_InboundClient(paramValue).Result,
            RemotingDirection.InboundHost => () => testClass.RemoteMethod_TaskEnum_EnumParam_InboundHost(paramValue).Result,
            RemotingDirection.Outbound => () => testClass.RemoteMethod_TaskEnum_EnumParam_Outbound(paramValue).Result,
            RemotingDirection.OutboundClient => () => testClass.RemoteMethod_TaskEnum_EnumParam_OutboundClient(paramValue).Result,
            RemotingDirection.OutboundHost => () => testClass.RemoteMethod_TaskEnum_EnumParam_OutboundHost(paramValue).Result,
            _ => ThrowHelper.ThrowArgumentOutOfRangeException<Func<object>>(),
        };

        private Func<object> GetMethodForDirection_TaskStruct_EnumParam(MemberRemoteTestClass testClass, RemotingDirection direction, RemotingDirection paramValue) => direction switch
        {
            RemotingDirection.None => () => testClass.RemoteMethod_TaskStruct_EnumParam_None(paramValue).Result,
            RemotingDirection.Bidirectional => () => testClass.RemoteMethod_TaskStruct_EnumParam_Bidirectional(paramValue).Result,
            RemotingDirection.ClientToHost => () => testClass.RemoteMethod_TaskStruct_EnumParam_ClientToHost(paramValue).Result,
            RemotingDirection.HostToClient => () => testClass.RemoteMethod_TaskStruct_EnumParam_HostToClient(paramValue).Result,
            RemotingDirection.Inbound => () => testClass.RemoteMethod_TaskStruct_EnumParam_Inbound(paramValue).Result,
            RemotingDirection.InboundClient => () => testClass.RemoteMethod_TaskStruct_EnumParam_InboundClient(paramValue).Result,
            RemotingDirection.InboundHost => () => testClass.RemoteMethod_TaskStruct_EnumParam_InboundHost(paramValue).Result,
            RemotingDirection.Outbound => () => testClass.RemoteMethod_TaskStruct_EnumParam_Outbound(paramValue).Result,
            RemotingDirection.OutboundClient => () => testClass.RemoteMethod_TaskStruct_EnumParam_OutboundClient(paramValue).Result,
            RemotingDirection.OutboundHost => () => testClass.RemoteMethod_TaskStruct_EnumParam_OutboundHost(paramValue).Result,
            _ => ThrowHelper.ThrowArgumentOutOfRangeException<Func<object>>(),
        };

        private Func<object> GetMethodForDirection_TaskObject_EnumParam(MemberRemoteTestClass testClass, RemotingDirection direction, RemotingDirection paramValue) => direction switch
        {
            RemotingDirection.None => () => testClass.RemoteMethod_TaskObject_EnumParam_None(paramValue).Result,
            RemotingDirection.Bidirectional => () => testClass.RemoteMethod_TaskObject_EnumParam_Bidirectional(paramValue).Result,
            RemotingDirection.ClientToHost => () => testClass.RemoteMethod_TaskObject_EnumParam_ClientToHost(paramValue).Result,
            RemotingDirection.HostToClient => () => testClass.RemoteMethod_TaskObject_EnumParam_HostToClient(paramValue).Result,
            RemotingDirection.Inbound => () => testClass.RemoteMethod_TaskObject_EnumParam_Inbound(paramValue).Result,
            RemotingDirection.InboundClient => () => testClass.RemoteMethod_TaskObject_EnumParam_InboundClient(paramValue).Result,
            RemotingDirection.InboundHost => () => testClass.RemoteMethod_TaskObject_EnumParam_InboundHost(paramValue).Result,
            RemotingDirection.Outbound => () => testClass.RemoteMethod_TaskObject_EnumParam_Outbound(paramValue).Result,
            RemotingDirection.OutboundClient => () => testClass.RemoteMethod_TaskObject_EnumParam_OutboundClient(paramValue).Result,
            RemotingDirection.OutboundHost => () => testClass.RemoteMethod_TaskObject_EnumParam_OutboundHost(paramValue).Result,
            _ => ThrowHelper.ThrowArgumentOutOfRangeException<Func<object>>(),
        };

        private Func<object> GetMethodForDirection_TaskPrimitive_EnumParam(MemberRemoteTestClass testClass, RemotingDirection direction, RemotingDirection paramValue) => direction switch
        {
            RemotingDirection.None => () => testClass.RemoteMethod_TaskPrimitive_EnumParam_None(paramValue).Result,
            RemotingDirection.Bidirectional => () => testClass.RemoteMethod_TaskPrimitive_EnumParam_Bidirectional(paramValue).Result,
            RemotingDirection.ClientToHost => () => testClass.RemoteMethod_TaskPrimitive_EnumParam_ClientToHost(paramValue).Result,
            RemotingDirection.HostToClient => () => testClass.RemoteMethod_TaskPrimitive_EnumParam_HostToClient(paramValue).Result,
            RemotingDirection.Inbound => () => testClass.RemoteMethod_TaskPrimitive_EnumParam_Inbound(paramValue).Result,
            RemotingDirection.InboundClient => () => testClass.RemoteMethod_TaskPrimitive_EnumParam_InboundClient(paramValue).Result,
            RemotingDirection.InboundHost => () => testClass.RemoteMethod_TaskPrimitive_EnumParam_InboundHost(paramValue).Result,
            RemotingDirection.Outbound => () => testClass.RemoteMethod_TaskPrimitive_EnumParam_Outbound(paramValue).Result,
            RemotingDirection.OutboundClient => () => testClass.RemoteMethod_TaskPrimitive_EnumParam_OutboundClient(paramValue).Result,
            RemotingDirection.OutboundHost => () => testClass.RemoteMethod_TaskPrimitive_EnumParam_OutboundHost(paramValue).Result,
            _ => ThrowHelper.ThrowArgumentOutOfRangeException<Func<object>>(),
        };

        #endregion

        #region GetMethodForDirection_StructParam

        private Action GetMethodForDirection_Void_StructParam(MemberRemoteTestClass testClass, RemotingDirection direction, DateTime structVal) => direction switch
        {
            RemotingDirection.None => () => testClass.RemoteMethod_Void_StructParam_None(structVal),
            RemotingDirection.Bidirectional => () => testClass.RemoteMethod_Void_StructParam_Bidirectional(structVal),
            RemotingDirection.ClientToHost => () => testClass.RemoteMethod_Void_StructParam_ClientToHost(structVal),
            RemotingDirection.HostToClient => () => testClass.RemoteMethod_Void_StructParam_HostToClient(structVal),
            RemotingDirection.Inbound => () => testClass.RemoteMethod_Void_StructParam_Inbound(structVal),
            RemotingDirection.InboundClient => () => testClass.RemoteMethod_Void_StructParam_InboundClient(structVal),
            RemotingDirection.InboundHost => () => testClass.RemoteMethod_Void_StructParam_InboundHost(structVal),
            RemotingDirection.Outbound => () => testClass.RemoteMethod_Void_StructParam_Outbound(structVal),
            RemotingDirection.OutboundClient => () => testClass.RemoteMethod_Void_StructParam_OutboundClient(structVal),
            RemotingDirection.OutboundHost => () => testClass.RemoteMethod_Void_StructParam_OutboundHost(structVal),
            _ => ThrowHelper.ThrowArgumentOutOfRangeException<Action>(),
        };

        private Func<object> GetMethodForDirection_Enum_StructParam(MemberRemoteTestClass testClass, RemotingDirection direction, DateTime structVal) => direction switch
        {
            RemotingDirection.None => () => testClass.RemoteMethod_Enum_StructParam_None(structVal),
            RemotingDirection.Bidirectional => () => testClass.RemoteMethod_Enum_StructParam_Bidirectional(structVal),
            RemotingDirection.ClientToHost => () => testClass.RemoteMethod_Enum_StructParam_ClientToHost(structVal),
            RemotingDirection.HostToClient => () => testClass.RemoteMethod_Enum_StructParam_HostToClient(structVal),
            RemotingDirection.Inbound => () => testClass.RemoteMethod_Enum_StructParam_Inbound(structVal),
            RemotingDirection.InboundClient => () => testClass.RemoteMethod_Enum_StructParam_InboundClient(structVal),
            RemotingDirection.InboundHost => () => testClass.RemoteMethod_Enum_StructParam_InboundHost(structVal),
            RemotingDirection.Outbound => () => testClass.RemoteMethod_Enum_StructParam_Outbound(structVal),
            RemotingDirection.OutboundClient => () => testClass.RemoteMethod_Enum_StructParam_OutboundClient(structVal),
            RemotingDirection.OutboundHost => () => testClass.RemoteMethod_Enum_StructParam_OutboundHost(structVal),
            _ => ThrowHelper.ThrowArgumentOutOfRangeException<Func<object>>(),
        };

        private Func<object> GetMethodForDirection_Struct_StructParam(MemberRemoteTestClass testClass, RemotingDirection direction, DateTime structVal) => direction switch
        {
            RemotingDirection.None => () => testClass.RemoteMethod_Struct_StructParam_None(structVal),
            RemotingDirection.Bidirectional => () => testClass.RemoteMethod_Struct_StructParam_Bidirectional(structVal),
            RemotingDirection.ClientToHost => () => testClass.RemoteMethod_Struct_StructParam_ClientToHost(structVal),
            RemotingDirection.HostToClient => () => testClass.RemoteMethod_Struct_StructParam_HostToClient(structVal),
            RemotingDirection.Inbound => () => testClass.RemoteMethod_Struct_StructParam_Inbound(structVal),
            RemotingDirection.InboundClient => () => testClass.RemoteMethod_Struct_StructParam_InboundClient(structVal),
            RemotingDirection.InboundHost => () => testClass.RemoteMethod_Struct_StructParam_InboundHost(structVal),
            RemotingDirection.Outbound => () => testClass.RemoteMethod_Struct_StructParam_Outbound(structVal),
            RemotingDirection.OutboundClient => () => testClass.RemoteMethod_Struct_StructParam_OutboundClient(structVal),
            RemotingDirection.OutboundHost => () => testClass.RemoteMethod_Struct_StructParam_OutboundHost(structVal),
            _ => ThrowHelper.ThrowArgumentOutOfRangeException<Func<object>>(),
        };

        private Func<object> GetMethodForDirection_Object_StructParam(MemberRemoteTestClass testClass, RemotingDirection direction, DateTime structVal) => direction switch
        {
            RemotingDirection.None => () => testClass.RemoteMethod_Object_StructParam_None(structVal),
            RemotingDirection.Bidirectional => () => testClass.RemoteMethod_Object_StructParam_Bidirectional(structVal),
            RemotingDirection.ClientToHost => () => testClass.RemoteMethod_Object_StructParam_ClientToHost(structVal),
            RemotingDirection.HostToClient => () => testClass.RemoteMethod_Object_StructParam_HostToClient(structVal),
            RemotingDirection.Inbound => () => testClass.RemoteMethod_Object_StructParam_Inbound(structVal),
            RemotingDirection.InboundClient => () => testClass.RemoteMethod_Object_StructParam_InboundClient(structVal),
            RemotingDirection.InboundHost => () => testClass.RemoteMethod_Object_StructParam_InboundHost(structVal),
            RemotingDirection.Outbound => () => testClass.RemoteMethod_Object_StructParam_Outbound(structVal),
            RemotingDirection.OutboundClient => () => testClass.RemoteMethod_Object_StructParam_OutboundClient(structVal),
            RemotingDirection.OutboundHost => () => testClass.RemoteMethod_Object_StructParam_OutboundHost(structVal),
            _ => ThrowHelper.ThrowArgumentOutOfRangeException<Func<object>>(),
        };

        private Func<object> GetMethodForDirection_Primitive_StructParam(MemberRemoteTestClass testClass, RemotingDirection direction, DateTime structVal) => direction switch
        {
            RemotingDirection.None => () => testClass.RemoteMethod_Primitive_StructParam_None(structVal),
            RemotingDirection.Bidirectional => () => testClass.RemoteMethod_Primitive_StructParam_Bidirectional(structVal),
            RemotingDirection.ClientToHost => () => testClass.RemoteMethod_Primitive_StructParam_ClientToHost(structVal),
            RemotingDirection.HostToClient => () => testClass.RemoteMethod_Primitive_StructParam_HostToClient(structVal),
            RemotingDirection.Inbound => () => testClass.RemoteMethod_Primitive_StructParam_Inbound(structVal),
            RemotingDirection.InboundClient => () => testClass.RemoteMethod_Primitive_StructParam_InboundClient(structVal),
            RemotingDirection.InboundHost => () => testClass.RemoteMethod_Primitive_StructParam_InboundHost(structVal),
            RemotingDirection.Outbound => () => testClass.RemoteMethod_Primitive_StructParam_Outbound(structVal),
            RemotingDirection.OutboundClient => () => testClass.RemoteMethod_Primitive_StructParam_OutboundClient(structVal),
            RemotingDirection.OutboundHost => () => testClass.RemoteMethod_Primitive_StructParam_OutboundHost(structVal),
            _ => ThrowHelper.ThrowArgumentOutOfRangeException<Func<object>>(),
        };

        private Func<object> GetMethodForDirection_Task_StructParam(MemberRemoteTestClass testClass, RemotingDirection direction, DateTime structVal) => direction switch
        {
            RemotingDirection.None => () => testClass.RemoteMethod_Task_StructParam_None(structVal),
            RemotingDirection.Bidirectional => () => testClass.RemoteMethod_Task_StructParam_Bidirectional(structVal),
            RemotingDirection.ClientToHost => () => testClass.RemoteMethod_Task_StructParam_ClientToHost(structVal),
            RemotingDirection.HostToClient => () => testClass.RemoteMethod_Task_StructParam_HostToClient(structVal),
            RemotingDirection.Inbound => () => testClass.RemoteMethod_Task_StructParam_Inbound(structVal),
            RemotingDirection.InboundClient => () => testClass.RemoteMethod_Task_StructParam_InboundClient(structVal),
            RemotingDirection.InboundHost => () => testClass.RemoteMethod_Task_StructParam_InboundHost(structVal),
            RemotingDirection.Outbound => () => testClass.RemoteMethod_Task_StructParam_Outbound(structVal),
            RemotingDirection.OutboundClient => () => testClass.RemoteMethod_Task_StructParam_OutboundClient(structVal),
            RemotingDirection.OutboundHost => () => testClass.RemoteMethod_Task_StructParam_OutboundHost(structVal),
            _ => ThrowHelper.ThrowArgumentOutOfRangeException<Func<object>>(),
        };

        private Func<object> GetMethodForDirection_TaskEnum_StructParam(MemberRemoteTestClass testClass, RemotingDirection direction, DateTime structVal) => direction switch
        {
            RemotingDirection.None => () => testClass.RemoteMethod_TaskEnum_StructParam_None(structVal).Result,
            RemotingDirection.Bidirectional => () => testClass.RemoteMethod_TaskEnum_StructParam_Bidirectional(structVal).Result,
            RemotingDirection.ClientToHost => () => testClass.RemoteMethod_TaskEnum_StructParam_ClientToHost(structVal).Result,
            RemotingDirection.HostToClient => () => testClass.RemoteMethod_TaskEnum_StructParam_HostToClient(structVal).Result,
            RemotingDirection.Inbound => () => testClass.RemoteMethod_TaskEnum_StructParam_Inbound(structVal).Result,
            RemotingDirection.InboundClient => () => testClass.RemoteMethod_TaskEnum_StructParam_InboundClient(structVal).Result,
            RemotingDirection.InboundHost => () => testClass.RemoteMethod_TaskEnum_StructParam_InboundHost(structVal).Result,
            RemotingDirection.Outbound => () => testClass.RemoteMethod_TaskEnum_StructParam_Outbound(structVal).Result,
            RemotingDirection.OutboundClient => () => testClass.RemoteMethod_TaskEnum_StructParam_OutboundClient(structVal).Result,
            RemotingDirection.OutboundHost => () => testClass.RemoteMethod_TaskEnum_StructParam_OutboundHost(structVal).Result,
            _ => ThrowHelper.ThrowArgumentOutOfRangeException<Func<object>>(),
        };

        private Func<object> GetMethodForDirection_TaskStruct_StructParam(MemberRemoteTestClass testClass, RemotingDirection direction, DateTime structVal) => direction switch
        {
            RemotingDirection.None => () => testClass.RemoteMethod_TaskStruct_StructParam_None(structVal).Result,
            RemotingDirection.Bidirectional => () => testClass.RemoteMethod_TaskStruct_StructParam_Bidirectional(structVal).Result,
            RemotingDirection.ClientToHost => () => testClass.RemoteMethod_TaskStruct_StructParam_ClientToHost(structVal).Result,
            RemotingDirection.HostToClient => () => testClass.RemoteMethod_TaskStruct_StructParam_HostToClient(structVal).Result,
            RemotingDirection.Inbound => () => testClass.RemoteMethod_TaskStruct_StructParam_Inbound(structVal).Result,
            RemotingDirection.InboundClient => () => testClass.RemoteMethod_TaskStruct_StructParam_InboundClient(structVal).Result,
            RemotingDirection.InboundHost => () => testClass.RemoteMethod_TaskStruct_StructParam_InboundHost(structVal).Result,
            RemotingDirection.Outbound => () => testClass.RemoteMethod_TaskStruct_StructParam_Outbound(structVal).Result,
            RemotingDirection.OutboundClient => () => testClass.RemoteMethod_TaskStruct_StructParam_OutboundClient(structVal).Result,
            RemotingDirection.OutboundHost => () => testClass.RemoteMethod_TaskStruct_StructParam_OutboundHost(structVal).Result,
            _ => ThrowHelper.ThrowArgumentOutOfRangeException<Func<object>>(),
        };

        private Func<object> GetMethodForDirection_TaskObject_StructParam(MemberRemoteTestClass testClass, RemotingDirection direction, DateTime structVal) => direction switch
        {
            RemotingDirection.None => () => testClass.RemoteMethod_TaskObject_StructParam_None(structVal).Result,
            RemotingDirection.Bidirectional => () => testClass.RemoteMethod_TaskObject_StructParam_Bidirectional(structVal).Result,
            RemotingDirection.ClientToHost => () => testClass.RemoteMethod_TaskObject_StructParam_ClientToHost(structVal).Result,
            RemotingDirection.HostToClient => () => testClass.RemoteMethod_TaskObject_StructParam_HostToClient(structVal).Result,
            RemotingDirection.Inbound => () => testClass.RemoteMethod_TaskObject_StructParam_Inbound(structVal).Result,
            RemotingDirection.InboundClient => () => testClass.RemoteMethod_TaskObject_StructParam_InboundClient(structVal).Result,
            RemotingDirection.InboundHost => () => testClass.RemoteMethod_TaskObject_StructParam_InboundHost(structVal).Result,
            RemotingDirection.Outbound => () => testClass.RemoteMethod_TaskObject_StructParam_Outbound(structVal).Result,
            RemotingDirection.OutboundClient => () => testClass.RemoteMethod_TaskObject_StructParam_OutboundClient(structVal).Result,
            RemotingDirection.OutboundHost => () => testClass.RemoteMethod_TaskObject_StructParam_OutboundHost(structVal).Result,
            _ => ThrowHelper.ThrowArgumentOutOfRangeException<Func<object>>(),
        };

        private Func<object> GetMethodForDirection_TaskPrimitive_StructParam(MemberRemoteTestClass testClass, RemotingDirection direction, DateTime structVal) => direction switch
        {
            RemotingDirection.None => () => testClass.RemoteMethod_TaskPrimitive_StructParam_None(structVal).Result,
            RemotingDirection.Bidirectional => () => testClass.RemoteMethod_TaskPrimitive_StructParam_Bidirectional(structVal).Result,
            RemotingDirection.ClientToHost => () => testClass.RemoteMethod_TaskPrimitive_StructParam_ClientToHost(structVal).Result,
            RemotingDirection.HostToClient => () => testClass.RemoteMethod_TaskPrimitive_StructParam_HostToClient(structVal).Result,
            RemotingDirection.Inbound => () => testClass.RemoteMethod_TaskPrimitive_StructParam_Inbound(structVal).Result,
            RemotingDirection.InboundClient => () => testClass.RemoteMethod_TaskPrimitive_StructParam_InboundClient(structVal).Result,
            RemotingDirection.InboundHost => () => testClass.RemoteMethod_TaskPrimitive_StructParam_InboundHost(structVal).Result,
            RemotingDirection.Outbound => () => testClass.RemoteMethod_TaskPrimitive_StructParam_Outbound(structVal).Result,
            RemotingDirection.OutboundClient => () => testClass.RemoteMethod_TaskPrimitive_StructParam_OutboundClient(structVal).Result,
            RemotingDirection.OutboundHost => () => testClass.RemoteMethod_TaskPrimitive_StructParam_OutboundHost(structVal).Result,
            _ => ThrowHelper.ThrowArgumentOutOfRangeException<Func<object>>(),
        };

        #endregion

        #region GetMethodForDirection_PrimitiveParam

        private Action GetMethodForDirection_Void_PrimitiveParam(MemberRemoteTestClass testClass, RemotingDirection direction, int primitive) => direction switch
        {
            RemotingDirection.None => () => testClass.RemoteMethod_Void_PrimitiveParam_None(primitive),
            RemotingDirection.Bidirectional => () => testClass.RemoteMethod_Void_PrimitiveParam_Bidirectional(primitive),
            RemotingDirection.ClientToHost => () => testClass.RemoteMethod_Void_PrimitiveParam_ClientToHost(primitive),
            RemotingDirection.HostToClient => () => testClass.RemoteMethod_Void_PrimitiveParam_HostToClient(primitive),
            RemotingDirection.Inbound => () => testClass.RemoteMethod_Void_PrimitiveParam_Inbound(primitive),
            RemotingDirection.InboundClient => () => testClass.RemoteMethod_Void_PrimitiveParam_InboundClient(primitive),
            RemotingDirection.InboundHost => () => testClass.RemoteMethod_Void_PrimitiveParam_InboundHost(primitive),
            RemotingDirection.Outbound => () => testClass.RemoteMethod_Void_PrimitiveParam_Outbound(primitive),
            RemotingDirection.OutboundClient => () => testClass.RemoteMethod_Void_PrimitiveParam_OutboundClient(primitive),
            RemotingDirection.OutboundHost => () => testClass.RemoteMethod_Void_PrimitiveParam_OutboundHost(primitive),
            _ => ThrowHelper.ThrowArgumentOutOfRangeException<Action>(),
        };

        private Func<object> GetMethodForDirection_Enum_PrimitiveParam(MemberRemoteTestClass testClass, RemotingDirection direction, int primitive) => direction switch
        {
            RemotingDirection.None => () => testClass.RemoteMethod_Enum_PrimitiveParam_None(primitive),
            RemotingDirection.Bidirectional => () => testClass.RemoteMethod_Enum_PrimitiveParam_Bidirectional(primitive),
            RemotingDirection.ClientToHost => () => testClass.RemoteMethod_Enum_PrimitiveParam_ClientToHost(primitive),
            RemotingDirection.HostToClient => () => testClass.RemoteMethod_Enum_PrimitiveParam_HostToClient(primitive),
            RemotingDirection.Inbound => () => testClass.RemoteMethod_Enum_PrimitiveParam_Inbound(primitive),
            RemotingDirection.InboundClient => () => testClass.RemoteMethod_Enum_PrimitiveParam_InboundClient(primitive),
            RemotingDirection.InboundHost => () => testClass.RemoteMethod_Enum_PrimitiveParam_InboundHost(primitive),
            RemotingDirection.Outbound => () => testClass.RemoteMethod_Enum_PrimitiveParam_Outbound(primitive),
            RemotingDirection.OutboundClient => () => testClass.RemoteMethod_Enum_PrimitiveParam_OutboundClient(primitive),
            RemotingDirection.OutboundHost => () => testClass.RemoteMethod_Enum_PrimitiveParam_OutboundHost(primitive),
            _ => ThrowHelper.ThrowArgumentOutOfRangeException<Func<object>>(),
        };

        private Func<object> GetMethodForDirection_Struct_PrimitiveParam(MemberRemoteTestClass testClass, RemotingDirection direction, int primitive) => direction switch
        {
            RemotingDirection.None => () => testClass.RemoteMethod_Struct_PrimitiveParam_None(primitive),
            RemotingDirection.Bidirectional => () => testClass.RemoteMethod_Struct_PrimitiveParam_Bidirectional(primitive),
            RemotingDirection.ClientToHost => () => testClass.RemoteMethod_Struct_PrimitiveParam_ClientToHost(primitive),
            RemotingDirection.HostToClient => () => testClass.RemoteMethod_Struct_PrimitiveParam_HostToClient(primitive),
            RemotingDirection.Inbound => () => testClass.RemoteMethod_Struct_PrimitiveParam_Inbound(primitive),
            RemotingDirection.InboundClient => () => testClass.RemoteMethod_Struct_PrimitiveParam_InboundClient(primitive),
            RemotingDirection.InboundHost => () => testClass.RemoteMethod_Struct_PrimitiveParam_InboundHost(primitive),
            RemotingDirection.Outbound => () => testClass.RemoteMethod_Struct_PrimitiveParam_Outbound(primitive),
            RemotingDirection.OutboundClient => () => testClass.RemoteMethod_Struct_PrimitiveParam_OutboundClient(primitive),
            RemotingDirection.OutboundHost => () => testClass.RemoteMethod_Struct_PrimitiveParam_OutboundHost(primitive),
            _ => ThrowHelper.ThrowArgumentOutOfRangeException<Func<object>>(),
        };

        private Func<object> GetMethodForDirection_Object_PrimitiveParam(MemberRemoteTestClass testClass, RemotingDirection direction, int primitive) => direction switch
        {
            RemotingDirection.None => () => testClass.RemoteMethod_Object_PrimitiveParam_None(primitive),
            RemotingDirection.Bidirectional => () => testClass.RemoteMethod_Object_PrimitiveParam_Bidirectional(primitive),
            RemotingDirection.ClientToHost => () => testClass.RemoteMethod_Object_PrimitiveParam_ClientToHost(primitive),
            RemotingDirection.HostToClient => () => testClass.RemoteMethod_Object_PrimitiveParam_HostToClient(primitive),
            RemotingDirection.Inbound => () => testClass.RemoteMethod_Object_PrimitiveParam_Inbound(primitive),
            RemotingDirection.InboundClient => () => testClass.RemoteMethod_Object_PrimitiveParam_InboundClient(primitive),
            RemotingDirection.InboundHost => () => testClass.RemoteMethod_Object_PrimitiveParam_InboundHost(primitive),
            RemotingDirection.Outbound => () => testClass.RemoteMethod_Object_PrimitiveParam_Outbound(primitive),
            RemotingDirection.OutboundClient => () => testClass.RemoteMethod_Object_PrimitiveParam_OutboundClient(primitive),
            RemotingDirection.OutboundHost => () => testClass.RemoteMethod_Object_PrimitiveParam_OutboundHost(primitive),
            _ => ThrowHelper.ThrowArgumentOutOfRangeException<Func<object>>(),
        };

        private Func<object> GetMethodForDirection_Primitive_PrimitiveParam(MemberRemoteTestClass testClass, RemotingDirection direction, int primitive) => direction switch
        {
            RemotingDirection.None => () => testClass.RemoteMethod_Primitive_PrimitiveParam_None(primitive),
            RemotingDirection.Bidirectional => () => testClass.RemoteMethod_Primitive_PrimitiveParam_Bidirectional(primitive),
            RemotingDirection.ClientToHost => () => testClass.RemoteMethod_Primitive_PrimitiveParam_ClientToHost(primitive),
            RemotingDirection.HostToClient => () => testClass.RemoteMethod_Primitive_PrimitiveParam_HostToClient(primitive),
            RemotingDirection.Inbound => () => testClass.RemoteMethod_Primitive_PrimitiveParam_Inbound(primitive),
            RemotingDirection.InboundClient => () => testClass.RemoteMethod_Primitive_PrimitiveParam_InboundClient(primitive),
            RemotingDirection.InboundHost => () => testClass.RemoteMethod_Primitive_PrimitiveParam_InboundHost(primitive),
            RemotingDirection.Outbound => () => testClass.RemoteMethod_Primitive_PrimitiveParam_Outbound(primitive),
            RemotingDirection.OutboundClient => () => testClass.RemoteMethod_Primitive_PrimitiveParam_OutboundClient(primitive),
            RemotingDirection.OutboundHost => () => testClass.RemoteMethod_Primitive_PrimitiveParam_OutboundHost(primitive),
            _ => ThrowHelper.ThrowArgumentOutOfRangeException<Func<object>>(),
        };

        private Func<object> GetMethodForDirection_Task_PrimitiveParam(MemberRemoteTestClass testClass, RemotingDirection direction, int primitive) => direction switch
        {
            RemotingDirection.None => () => testClass.RemoteMethod_Task_PrimitiveParam_None(primitive),
            RemotingDirection.Bidirectional => () => testClass.RemoteMethod_Task_PrimitiveParam_Bidirectional(primitive),
            RemotingDirection.ClientToHost => () => testClass.RemoteMethod_Task_PrimitiveParam_ClientToHost(primitive),
            RemotingDirection.HostToClient => () => testClass.RemoteMethod_Task_PrimitiveParam_HostToClient(primitive),
            RemotingDirection.Inbound => () => testClass.RemoteMethod_Task_PrimitiveParam_Inbound(primitive),
            RemotingDirection.InboundClient => () => testClass.RemoteMethod_Task_PrimitiveParam_InboundClient(primitive),
            RemotingDirection.InboundHost => () => testClass.RemoteMethod_Task_PrimitiveParam_InboundHost(primitive),
            RemotingDirection.Outbound => () => testClass.RemoteMethod_Task_PrimitiveParam_Outbound(primitive),
            RemotingDirection.OutboundClient => () => testClass.RemoteMethod_Task_PrimitiveParam_OutboundClient(primitive),
            RemotingDirection.OutboundHost => () => testClass.RemoteMethod_Task_PrimitiveParam_OutboundHost(primitive),
            _ => ThrowHelper.ThrowArgumentOutOfRangeException<Func<object>>(),
        };

        private Func<object> GetMethodForDirection_TaskEnum_PrimitiveParam(MemberRemoteTestClass testClass, RemotingDirection direction, int primitive) => direction switch
        {
            RemotingDirection.None => () => testClass.RemoteMethod_TaskEnum_PrimitiveParam_None(primitive).Result,
            RemotingDirection.Bidirectional => () => testClass.RemoteMethod_TaskEnum_PrimitiveParam_Bidirectional(primitive).Result,
            RemotingDirection.ClientToHost => () => testClass.RemoteMethod_TaskEnum_PrimitiveParam_ClientToHost(primitive).Result,
            RemotingDirection.HostToClient => () => testClass.RemoteMethod_TaskEnum_PrimitiveParam_HostToClient(primitive).Result,
            RemotingDirection.Inbound => () => testClass.RemoteMethod_TaskEnum_PrimitiveParam_Inbound(primitive).Result,
            RemotingDirection.InboundClient => () => testClass.RemoteMethod_TaskEnum_PrimitiveParam_InboundClient(primitive).Result,
            RemotingDirection.InboundHost => () => testClass.RemoteMethod_TaskEnum_PrimitiveParam_InboundHost(primitive).Result,
            RemotingDirection.Outbound => () => testClass.RemoteMethod_TaskEnum_PrimitiveParam_Outbound(primitive).Result,
            RemotingDirection.OutboundClient => () => testClass.RemoteMethod_TaskEnum_PrimitiveParam_OutboundClient(primitive).Result,
            RemotingDirection.OutboundHost => () => testClass.RemoteMethod_TaskEnum_PrimitiveParam_OutboundHost(primitive).Result,
            _ => ThrowHelper.ThrowArgumentOutOfRangeException<Func<object>>(),
        };

        private Func<object> GetMethodForDirection_TaskStruct_PrimitiveParam(MemberRemoteTestClass testClass, RemotingDirection direction, int primitive) => direction switch
        {
            RemotingDirection.None => () => testClass.RemoteMethod_TaskStruct_PrimitiveParam_None(primitive).Result,
            RemotingDirection.Bidirectional => () => testClass.RemoteMethod_TaskStruct_PrimitiveParam_Bidirectional(primitive).Result,
            RemotingDirection.ClientToHost => () => testClass.RemoteMethod_TaskStruct_PrimitiveParam_ClientToHost(primitive).Result,
            RemotingDirection.HostToClient => () => testClass.RemoteMethod_TaskStruct_PrimitiveParam_HostToClient(primitive).Result,
            RemotingDirection.Inbound => () => testClass.RemoteMethod_TaskStruct_PrimitiveParam_Inbound(primitive).Result,
            RemotingDirection.InboundClient => () => testClass.RemoteMethod_TaskStruct_PrimitiveParam_InboundClient(primitive).Result,
            RemotingDirection.InboundHost => () => testClass.RemoteMethod_TaskStruct_PrimitiveParam_InboundHost(primitive).Result,
            RemotingDirection.Outbound => () => testClass.RemoteMethod_TaskStruct_PrimitiveParam_Outbound(primitive).Result,
            RemotingDirection.OutboundClient => () => testClass.RemoteMethod_TaskStruct_PrimitiveParam_OutboundClient(primitive).Result,
            RemotingDirection.OutboundHost => () => testClass.RemoteMethod_TaskStruct_PrimitiveParam_OutboundHost(primitive).Result,
            _ => ThrowHelper.ThrowArgumentOutOfRangeException<Func<object>>(),
        };

        private Func<object> GetMethodForDirection_TaskObject_PrimitiveParam(MemberRemoteTestClass testClass, RemotingDirection direction, int primitive) => direction switch
        {
            RemotingDirection.None => () => testClass.RemoteMethod_TaskObject_PrimitiveParam_None(primitive).Result,
            RemotingDirection.Bidirectional => () => testClass.RemoteMethod_TaskObject_PrimitiveParam_Bidirectional(primitive).Result,
            RemotingDirection.ClientToHost => () => testClass.RemoteMethod_TaskObject_PrimitiveParam_ClientToHost(primitive).Result,
            RemotingDirection.HostToClient => () => testClass.RemoteMethod_TaskObject_PrimitiveParam_HostToClient(primitive).Result,
            RemotingDirection.Inbound => () => testClass.RemoteMethod_TaskObject_PrimitiveParam_Inbound(primitive).Result,
            RemotingDirection.InboundClient => () => testClass.RemoteMethod_TaskObject_PrimitiveParam_InboundClient(primitive).Result,
            RemotingDirection.InboundHost => () => testClass.RemoteMethod_TaskObject_PrimitiveParam_InboundHost(primitive).Result,
            RemotingDirection.Outbound => () => testClass.RemoteMethod_TaskObject_PrimitiveParam_Outbound(primitive).Result,
            RemotingDirection.OutboundClient => () => testClass.RemoteMethod_TaskObject_PrimitiveParam_OutboundClient(primitive).Result,
            RemotingDirection.OutboundHost => () => testClass.RemoteMethod_TaskObject_PrimitiveParam_OutboundHost(primitive).Result,
            _ => ThrowHelper.ThrowArgumentOutOfRangeException<Func<object>>(),
        };

        private Func<object> GetMethodForDirection_TaskPrimitive_PrimitiveParam(MemberRemoteTestClass testClass, RemotingDirection direction, int primitive) => direction switch
        {
            RemotingDirection.None => () => testClass.RemoteMethod_TaskPrimitive_PrimitiveParam_None(primitive).Result,
            RemotingDirection.Bidirectional => () => testClass.RemoteMethod_TaskPrimitive_PrimitiveParam_Bidirectional(primitive).Result,
            RemotingDirection.ClientToHost => () => testClass.RemoteMethod_TaskPrimitive_PrimitiveParam_ClientToHost(primitive).Result,
            RemotingDirection.HostToClient => () => testClass.RemoteMethod_TaskPrimitive_PrimitiveParam_HostToClient(primitive).Result,
            RemotingDirection.Inbound => () => testClass.RemoteMethod_TaskPrimitive_PrimitiveParam_Inbound(primitive).Result,
            RemotingDirection.InboundClient => () => testClass.RemoteMethod_TaskPrimitive_PrimitiveParam_InboundClient(primitive).Result,
            RemotingDirection.InboundHost => () => testClass.RemoteMethod_TaskPrimitive_PrimitiveParam_InboundHost(primitive).Result,
            RemotingDirection.Outbound => () => testClass.RemoteMethod_TaskPrimitive_PrimitiveParam_Outbound(primitive).Result,
            RemotingDirection.OutboundClient => () => testClass.RemoteMethod_TaskPrimitive_PrimitiveParam_OutboundClient(primitive).Result,
            RemotingDirection.OutboundHost => () => testClass.RemoteMethod_TaskPrimitive_PrimitiveParam_OutboundHost(primitive).Result,
            _ => ThrowHelper.ThrowArgumentOutOfRangeException<Func<object>>(),
        };

        #endregion

        #region GetMethodForDirection_ObjectParam

        private Action GetMethodForDirection_Void_ObjectParam(MemberRemoteTestClass testClass, RemotingDirection direction, object obj) => direction switch
        {
            RemotingDirection.None => () => testClass.RemoteMethod_Void_ObjectParam_None(obj),
            RemotingDirection.Bidirectional => () => testClass.RemoteMethod_Void_ObjectParam_Bidirectional(obj),
            RemotingDirection.ClientToHost => () => testClass.RemoteMethod_Void_ObjectParam_ClientToHost(obj),
            RemotingDirection.HostToClient => () => testClass.RemoteMethod_Void_ObjectParam_HostToClient(obj),
            RemotingDirection.Inbound => () => testClass.RemoteMethod_Void_ObjectParam_Inbound(obj),
            RemotingDirection.InboundClient => () => testClass.RemoteMethod_Void_ObjectParam_InboundClient(obj),
            RemotingDirection.InboundHost => () => testClass.RemoteMethod_Void_ObjectParam_InboundHost(obj),
            RemotingDirection.Outbound => () => testClass.RemoteMethod_Void_ObjectParam_Outbound(obj),
            RemotingDirection.OutboundClient => () => testClass.RemoteMethod_Void_ObjectParam_OutboundClient(obj),
            RemotingDirection.OutboundHost => () => testClass.RemoteMethod_Void_ObjectParam_OutboundHost(obj),
            _ => ThrowHelper.ThrowArgumentOutOfRangeException<Action>(),
        };

        private Func<object> GetMethodForDirection_Enum_ObjectParam(MemberRemoteTestClass testClass, RemotingDirection direction, object obj) => direction switch
        {
            RemotingDirection.None => () => testClass.RemoteMethod_Enum_ObjectParam_None(obj),
            RemotingDirection.Bidirectional => () => testClass.RemoteMethod_Enum_ObjectParam_Bidirectional(obj),
            RemotingDirection.ClientToHost => () => testClass.RemoteMethod_Enum_ObjectParam_ClientToHost(obj),
            RemotingDirection.HostToClient => () => testClass.RemoteMethod_Enum_ObjectParam_HostToClient(obj),
            RemotingDirection.Inbound => () => testClass.RemoteMethod_Enum_ObjectParam_Inbound(obj),
            RemotingDirection.InboundClient => () => testClass.RemoteMethod_Enum_ObjectParam_InboundClient(obj),
            RemotingDirection.InboundHost => () => testClass.RemoteMethod_Enum_ObjectParam_InboundHost(obj),
            RemotingDirection.Outbound => () => testClass.RemoteMethod_Enum_ObjectParam_Outbound(obj),
            RemotingDirection.OutboundClient => () => testClass.RemoteMethod_Enum_ObjectParam_OutboundClient(obj),
            RemotingDirection.OutboundHost => () => testClass.RemoteMethod_Enum_ObjectParam_OutboundHost(obj),
            _ => ThrowHelper.ThrowArgumentOutOfRangeException<Func<object>>(),
        };

        private Func<object> GetMethodForDirection_Struct_ObjectParam(MemberRemoteTestClass testClass, RemotingDirection direction, object obj) => direction switch
        {
            RemotingDirection.None => () => testClass.RemoteMethod_Struct_ObjectParam_None(obj),
            RemotingDirection.Bidirectional => () => testClass.RemoteMethod_Struct_ObjectParam_Bidirectional(obj),
            RemotingDirection.ClientToHost => () => testClass.RemoteMethod_Struct_ObjectParam_ClientToHost(obj),
            RemotingDirection.HostToClient => () => testClass.RemoteMethod_Struct_ObjectParam_HostToClient(obj),
            RemotingDirection.Inbound => () => testClass.RemoteMethod_Struct_ObjectParam_Inbound(obj),
            RemotingDirection.InboundClient => () => testClass.RemoteMethod_Struct_ObjectParam_InboundClient(obj),
            RemotingDirection.InboundHost => () => testClass.RemoteMethod_Struct_ObjectParam_InboundHost(obj),
            RemotingDirection.Outbound => () => testClass.RemoteMethod_Struct_ObjectParam_Outbound(obj),
            RemotingDirection.OutboundClient => () => testClass.RemoteMethod_Struct_ObjectParam_OutboundClient(obj),
            RemotingDirection.OutboundHost => () => testClass.RemoteMethod_Struct_ObjectParam_OutboundHost(obj),
            _ => ThrowHelper.ThrowArgumentOutOfRangeException<Func<object>>(),
        };

        private Func<object> GetMethodForDirection_Object_ObjectParam(MemberRemoteTestClass testClass, RemotingDirection direction, object obj) => direction switch
        {
            RemotingDirection.None => () => testClass.RemoteMethod_Object_ObjectParam_None(obj),
            RemotingDirection.Bidirectional => () => testClass.RemoteMethod_Object_ObjectParam_Bidirectional(obj),
            RemotingDirection.ClientToHost => () => testClass.RemoteMethod_Object_ObjectParam_ClientToHost(obj),
            RemotingDirection.HostToClient => () => testClass.RemoteMethod_Object_ObjectParam_HostToClient(obj),
            RemotingDirection.Inbound => () => testClass.RemoteMethod_Object_ObjectParam_Inbound(obj),
            RemotingDirection.InboundClient => () => testClass.RemoteMethod_Object_ObjectParam_InboundClient(obj),
            RemotingDirection.InboundHost => () => testClass.RemoteMethod_Object_ObjectParam_InboundHost(obj),
            RemotingDirection.Outbound => () => testClass.RemoteMethod_Object_ObjectParam_Outbound(obj),
            RemotingDirection.OutboundClient => () => testClass.RemoteMethod_Object_ObjectParam_OutboundClient(obj),
            RemotingDirection.OutboundHost => () => testClass.RemoteMethod_Object_ObjectParam_OutboundHost(obj),
            _ => ThrowHelper.ThrowArgumentOutOfRangeException<Func<object>>(),
        };

        private Func<object> GetMethodForDirection_Primitive_ObjectParam(MemberRemoteTestClass testClass, RemotingDirection direction, object obj) => direction switch
        {
            RemotingDirection.None => () => testClass.RemoteMethod_Primitive_ObjectParam_None(obj),
            RemotingDirection.Bidirectional => () => testClass.RemoteMethod_Primitive_ObjectParam_Bidirectional(obj),
            RemotingDirection.ClientToHost => () => testClass.RemoteMethod_Primitive_ObjectParam_ClientToHost(obj),
            RemotingDirection.HostToClient => () => testClass.RemoteMethod_Primitive_ObjectParam_HostToClient(obj),
            RemotingDirection.Inbound => () => testClass.RemoteMethod_Primitive_ObjectParam_Inbound(obj),
            RemotingDirection.InboundClient => () => testClass.RemoteMethod_Primitive_ObjectParam_InboundClient(obj),
            RemotingDirection.InboundHost => () => testClass.RemoteMethod_Primitive_ObjectParam_InboundHost(obj),
            RemotingDirection.Outbound => () => testClass.RemoteMethod_Primitive_ObjectParam_Outbound(obj),
            RemotingDirection.OutboundClient => () => testClass.RemoteMethod_Primitive_ObjectParam_OutboundClient(obj),
            RemotingDirection.OutboundHost => () => testClass.RemoteMethod_Primitive_ObjectParam_OutboundHost(obj),
            _ => ThrowHelper.ThrowArgumentOutOfRangeException<Func<object>>(),
        };

        private Func<object> GetMethodForDirection_Task_ObjectParam(MemberRemoteTestClass testClass, RemotingDirection direction, object obj) => direction switch
        {
            RemotingDirection.None => () => testClass.RemoteMethod_Task_ObjectParam_None(obj),
            RemotingDirection.Bidirectional => () => testClass.RemoteMethod_Task_ObjectParam_Bidirectional(obj),
            RemotingDirection.ClientToHost => () => testClass.RemoteMethod_Task_ObjectParam_ClientToHost(obj),
            RemotingDirection.HostToClient => () => testClass.RemoteMethod_Task_ObjectParam_HostToClient(obj),
            RemotingDirection.Inbound => () => testClass.RemoteMethod_Task_ObjectParam_Inbound(obj),
            RemotingDirection.InboundClient => () => testClass.RemoteMethod_Task_ObjectParam_InboundClient(obj),
            RemotingDirection.InboundHost => () => testClass.RemoteMethod_Task_ObjectParam_InboundHost(obj),
            RemotingDirection.Outbound => () => testClass.RemoteMethod_Task_ObjectParam_Outbound(obj),
            RemotingDirection.OutboundClient => () => testClass.RemoteMethod_Task_ObjectParam_OutboundClient(obj),
            RemotingDirection.OutboundHost => () => testClass.RemoteMethod_Task_ObjectParam_OutboundHost(obj),
            _ => ThrowHelper.ThrowArgumentOutOfRangeException<Func<object>>(),
        };

        private Func<object> GetMethodForDirection_TaskEnum_ObjectParam(MemberRemoteTestClass testClass, RemotingDirection direction, object obj) => direction switch
        {
            RemotingDirection.None => () => testClass.RemoteMethod_TaskEnum_ObjectParam_None(obj).Result,
            RemotingDirection.Bidirectional => () => testClass.RemoteMethod_TaskEnum_ObjectParam_Bidirectional(obj).Result,
            RemotingDirection.ClientToHost => () => testClass.RemoteMethod_TaskEnum_ObjectParam_ClientToHost(obj).Result,
            RemotingDirection.HostToClient => () => testClass.RemoteMethod_TaskEnum_ObjectParam_HostToClient(obj).Result,
            RemotingDirection.Inbound => () => testClass.RemoteMethod_TaskEnum_ObjectParam_Inbound(obj).Result,
            RemotingDirection.InboundClient => () => testClass.RemoteMethod_TaskEnum_ObjectParam_InboundClient(obj).Result,
            RemotingDirection.InboundHost => () => testClass.RemoteMethod_TaskEnum_ObjectParam_InboundHost(obj).Result,
            RemotingDirection.Outbound => () => testClass.RemoteMethod_TaskEnum_ObjectParam_Outbound(obj).Result,
            RemotingDirection.OutboundClient => () => testClass.RemoteMethod_TaskEnum_ObjectParam_OutboundClient(obj).Result,
            RemotingDirection.OutboundHost => () => testClass.RemoteMethod_TaskEnum_ObjectParam_OutboundHost(obj).Result,
            _ => ThrowHelper.ThrowArgumentOutOfRangeException<Func<object>>(),
        };

        private Func<object> GetMethodForDirection_TaskStruct_ObjectParam(MemberRemoteTestClass testClass, RemotingDirection direction, object obj) => direction switch
        {
            RemotingDirection.None => () => testClass.RemoteMethod_TaskStruct_ObjectParam_None(obj).Result,
            RemotingDirection.Bidirectional => () => testClass.RemoteMethod_TaskStruct_ObjectParam_Bidirectional(obj).Result,
            RemotingDirection.ClientToHost => () => testClass.RemoteMethod_TaskStruct_ObjectParam_ClientToHost(obj).Result,
            RemotingDirection.HostToClient => () => testClass.RemoteMethod_TaskStruct_ObjectParam_HostToClient(obj).Result,
            RemotingDirection.Inbound => () => testClass.RemoteMethod_TaskStruct_ObjectParam_Inbound(obj).Result,
            RemotingDirection.InboundClient => () => testClass.RemoteMethod_TaskStruct_ObjectParam_InboundClient(obj).Result,
            RemotingDirection.InboundHost => () => testClass.RemoteMethod_TaskStruct_ObjectParam_InboundHost(obj).Result,
            RemotingDirection.Outbound => () => testClass.RemoteMethod_TaskStruct_ObjectParam_Outbound(obj).Result,
            RemotingDirection.OutboundClient => () => testClass.RemoteMethod_TaskStruct_ObjectParam_OutboundClient(obj).Result,
            RemotingDirection.OutboundHost => () => testClass.RemoteMethod_TaskStruct_ObjectParam_OutboundHost(obj).Result,
            _ => ThrowHelper.ThrowArgumentOutOfRangeException<Func<object>>(),
        };

        private Func<object> GetMethodForDirection_TaskObject_ObjectParam(MemberRemoteTestClass testClass, RemotingDirection direction, object obj) => direction switch
        {
            RemotingDirection.None => () => testClass.RemoteMethod_TaskObject_ObjectParam_None(obj).Result,
            RemotingDirection.Bidirectional => () => testClass.RemoteMethod_TaskObject_ObjectParam_Bidirectional(obj).Result,
            RemotingDirection.ClientToHost => () => testClass.RemoteMethod_TaskObject_ObjectParam_ClientToHost(obj).Result,
            RemotingDirection.HostToClient => () => testClass.RemoteMethod_TaskObject_ObjectParam_HostToClient(obj).Result,
            RemotingDirection.Inbound => () => testClass.RemoteMethod_TaskObject_ObjectParam_Inbound(obj).Result,
            RemotingDirection.InboundClient => () => testClass.RemoteMethod_TaskObject_ObjectParam_InboundClient(obj).Result,
            RemotingDirection.InboundHost => () => testClass.RemoteMethod_TaskObject_ObjectParam_InboundHost(obj).Result,
            RemotingDirection.Outbound => () => testClass.RemoteMethod_TaskObject_ObjectParam_Outbound(obj).Result,
            RemotingDirection.OutboundClient => () => testClass.RemoteMethod_TaskObject_ObjectParam_OutboundClient(obj).Result,
            RemotingDirection.OutboundHost => () => testClass.RemoteMethod_TaskObject_ObjectParam_OutboundHost(obj).Result,
            _ => ThrowHelper.ThrowArgumentOutOfRangeException<Func<object>>(),
        };

        private Func<object> GetMethodForDirection_TaskPrimitive_ObjectParam(MemberRemoteTestClass testClass, RemotingDirection direction, object obj) => direction switch
        {
            RemotingDirection.None => () => testClass.RemoteMethod_TaskPrimitive_ObjectParam_None(obj).Result,
            RemotingDirection.Bidirectional => () => testClass.RemoteMethod_TaskPrimitive_ObjectParam_Bidirectional(obj).Result,
            RemotingDirection.ClientToHost => () => testClass.RemoteMethod_TaskPrimitive_ObjectParam_ClientToHost(obj).Result,
            RemotingDirection.HostToClient => () => testClass.RemoteMethod_TaskPrimitive_ObjectParam_HostToClient(obj).Result,
            RemotingDirection.Inbound => () => testClass.RemoteMethod_TaskPrimitive_ObjectParam_Inbound(obj).Result,
            RemotingDirection.InboundClient => () => testClass.RemoteMethod_TaskPrimitive_ObjectParam_InboundClient(obj).Result,
            RemotingDirection.InboundHost => () => testClass.RemoteMethod_TaskPrimitive_ObjectParam_InboundHost(obj).Result,
            RemotingDirection.Outbound => () => testClass.RemoteMethod_TaskPrimitive_ObjectParam_Outbound(obj).Result,
            RemotingDirection.OutboundClient => () => testClass.RemoteMethod_TaskPrimitive_ObjectParam_OutboundClient(obj).Result,
            RemotingDirection.OutboundHost => () => testClass.RemoteMethod_TaskPrimitive_ObjectParam_OutboundHost(obj).Result,
            _ => ThrowHelper.ThrowArgumentOutOfRangeException<Func<object>>(),
        };

        #endregion
    }
}
