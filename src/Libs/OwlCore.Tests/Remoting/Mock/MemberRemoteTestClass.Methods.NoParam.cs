using OwlCore.Remoting;
using System;
using System.Threading.Tasks;

namespace OwlCore.Tests.Remoting.Mock
{
    public partial class MemberRemoteTestClass
    {
        [RemoteOptions(RemotingDirection.None), RemoteMethod]
        public void RemoteMethod_Void_NoParam_None() => RemoteMethodCalledVoid?.Invoke(this, EventArgs.Empty);

        [RemoteOptions(RemotingDirection.Bidirectional), RemoteMethod]
        public void RemoteMethod_Void_NoParam_Bidirectional() => RemoteMethodCalledVoid?.Invoke(this, EventArgs.Empty);

        [RemoteOptions(RemotingDirection.HostToClient), RemoteMethod]
        public void RemoteMethod_Void_NoParam_HostToClient() => RemoteMethodCalledVoid?.Invoke(this, EventArgs.Empty);

        [RemoteOptions(RemotingDirection.ClientToHost), RemoteMethod]
        public void RemoteMethod_Void_NoParam_ClientToHost() => RemoteMethodCalledVoid?.Invoke(this, EventArgs.Empty);

        [RemoteOptions(RemotingDirection.Inbound), RemoteMethod]
        public void RemoteMethod_Void_NoParam_Inbound() => RemoteMethodCalledVoid?.Invoke(this, EventArgs.Empty);

        [RemoteOptions(RemotingDirection.InboundClient), RemoteMethod]
        public void RemoteMethod_Void_NoParam_InboundClient() => RemoteMethodCalledVoid?.Invoke(this, EventArgs.Empty);

        [RemoteOptions(RemotingDirection.InboundHost), RemoteMethod]
        public void RemoteMethod_Void_NoParam_InboundHost() => RemoteMethodCalledVoid?.Invoke(this, EventArgs.Empty);

        [RemoteOptions(RemotingDirection.Outbound), RemoteMethod]
        public void RemoteMethod_Void_NoParam_Outbound() => RemoteMethodCalledVoid?.Invoke(this, EventArgs.Empty);

        [RemoteOptions(RemotingDirection.OutboundClient), RemoteMethod]
        public void RemoteMethod_Void_NoParam_OutboundClient() => RemoteMethodCalledVoid?.Invoke(this, EventArgs.Empty);

        [RemoteOptions(RemotingDirection.OutboundHost), RemoteMethod]
        public void RemoteMethod_Void_NoParam_OutboundHost() => RemoteMethodCalledVoid?.Invoke(this, EventArgs.Empty);

        // ===============================================================================

        [RemoteOptions(RemotingDirection.None), RemoteMethod]
        public RemotingDirection RemoteMethod_Enum_NoParam_None() => ReturnedRemoteMethodEnum;

        [RemoteOptions(RemotingDirection.Bidirectional), RemoteMethod]
        public RemotingDirection RemoteMethod_Enum_NoParam_Bidirectional() => ReturnedRemoteMethodEnum;

        [RemoteOptions(RemotingDirection.HostToClient), RemoteMethod]
        public RemotingDirection RemoteMethod_Enum_NoParam_HostToClient() => ReturnedRemoteMethodEnum;

        [RemoteOptions(RemotingDirection.ClientToHost), RemoteMethod]
        public RemotingDirection RemoteMethod_Enum_NoParam_ClientToHost() => ReturnedRemoteMethodEnum;

        [RemoteOptions(RemotingDirection.Inbound), RemoteMethod]
        public RemotingDirection RemoteMethod_Enum_NoParam_Inbound() => ReturnedRemoteMethodEnum;

        [RemoteOptions(RemotingDirection.InboundClient), RemoteMethod]
        public RemotingDirection RemoteMethod_Enum_NoParam_InboundClient() => ReturnedRemoteMethodEnum;

        [RemoteOptions(RemotingDirection.InboundHost), RemoteMethod]
        public RemotingDirection RemoteMethod_Enum_NoParam_InboundHost() => ReturnedRemoteMethodEnum;

        [RemoteOptions(RemotingDirection.Outbound), RemoteMethod]
        public RemotingDirection RemoteMethod_Enum_NoParam_Outbound() => ReturnedRemoteMethodEnum;

        [RemoteOptions(RemotingDirection.OutboundClient), RemoteMethod]
        public RemotingDirection RemoteMethod_Enum_NoParam_OutboundClient() => ReturnedRemoteMethodEnum;

        [RemoteOptions(RemotingDirection.OutboundHost), RemoteMethod]
        public RemotingDirection RemoteMethod_Enum_NoParam_OutboundHost() => ReturnedRemoteMethodEnum;

        // ===============================================================================

        [RemoteOptions(RemotingDirection.None), RemoteMethod]
        public DateTime RemoteMethod_Struct_NoParam_None() => ReturnedRemoteMethodStruct;

        [RemoteOptions(RemotingDirection.Bidirectional), RemoteMethod]
        public DateTime RemoteMethod_Struct_NoParam_Bidirectional() => ReturnedRemoteMethodStruct;

        [RemoteOptions(RemotingDirection.HostToClient), RemoteMethod]
        public DateTime RemoteMethod_Struct_NoParam_HostToClient() => ReturnedRemoteMethodStruct;

        [RemoteOptions(RemotingDirection.ClientToHost), RemoteMethod]
        public DateTime RemoteMethod_Struct_NoParam_ClientToHost() => ReturnedRemoteMethodStruct;

        [RemoteOptions(RemotingDirection.Inbound), RemoteMethod]
        public DateTime RemoteMethod_Struct_NoParam_Inbound() => ReturnedRemoteMethodStruct;

        [RemoteOptions(RemotingDirection.InboundClient), RemoteMethod]
        public DateTime RemoteMethod_Struct_NoParam_InboundClient() => ReturnedRemoteMethodStruct;

        [RemoteOptions(RemotingDirection.InboundHost), RemoteMethod]
        public DateTime RemoteMethod_Struct_NoParam_InboundHost() => ReturnedRemoteMethodStruct;

        [RemoteOptions(RemotingDirection.Outbound), RemoteMethod]
        public DateTime RemoteMethod_Struct_NoParam_Outbound() => ReturnedRemoteMethodStruct;

        [RemoteOptions(RemotingDirection.OutboundClient), RemoteMethod]
        public DateTime RemoteMethod_Struct_NoParam_OutboundClient() => ReturnedRemoteMethodStruct;

        [RemoteOptions(RemotingDirection.OutboundHost), RemoteMethod]
        public DateTime RemoteMethod_Struct_NoParam_OutboundHost() => ReturnedRemoteMethodStruct;

        // ===============================================================================

        [RemoteOptions(RemotingDirection.None), RemoteMethod]
        public object RemoteMethod_Object_NoParam_None() => ReturnedRemoteMethodObject;

        [RemoteOptions(RemotingDirection.Bidirectional), RemoteMethod]
        public object RemoteMethod_Object_NoParam_Bidirectional() => ReturnedRemoteMethodObject;

        [RemoteOptions(RemotingDirection.HostToClient), RemoteMethod]
        public object RemoteMethod_Object_NoParam_HostToClient() => ReturnedRemoteMethodObject;

        [RemoteOptions(RemotingDirection.ClientToHost), RemoteMethod]
        public object RemoteMethod_Object_NoParam_ClientToHost() => ReturnedRemoteMethodObject;

        [RemoteOptions(RemotingDirection.Inbound), RemoteMethod]
        public object RemoteMethod_Object_NoParam_Inbound() => ReturnedRemoteMethodObject;

        [RemoteOptions(RemotingDirection.InboundClient), RemoteMethod]
        public object RemoteMethod_Object_NoParam_InboundClient() => ReturnedRemoteMethodObject;

        [RemoteOptions(RemotingDirection.InboundHost), RemoteMethod]
        public object RemoteMethod_Object_NoParam_InboundHost() => ReturnedRemoteMethodObject;

        [RemoteOptions(RemotingDirection.Outbound), RemoteMethod]
        public object RemoteMethod_Object_NoParam_Outbound() => ReturnedRemoteMethodObject;

        [RemoteOptions(RemotingDirection.OutboundClient), RemoteMethod]
        public object RemoteMethod_Object_NoParam_OutboundClient() => ReturnedRemoteMethodObject;

        [RemoteOptions(RemotingDirection.OutboundHost), RemoteMethod]
        public object RemoteMethod_Object_NoParam_OutboundHost() => ReturnedRemoteMethodObject;

        // ===============================================================================

        [RemoteOptions(RemotingDirection.None), RemoteMethod]
        public int RemoteMethod_Primitive_NoParam_None() => ReturnedRemoteMethodPrimitive;

        [RemoteOptions(RemotingDirection.Bidirectional), RemoteMethod]
        public int RemoteMethod_Primitive_NoParam_Bidirectional() => ReturnedRemoteMethodPrimitive;

        [RemoteOptions(RemotingDirection.HostToClient), RemoteMethod]
        public int RemoteMethod_Primitive_NoParam_HostToClient() => ReturnedRemoteMethodPrimitive;

        [RemoteOptions(RemotingDirection.ClientToHost), RemoteMethod]
        public int RemoteMethod_Primitive_NoParam_ClientToHost() => ReturnedRemoteMethodPrimitive;

        [RemoteOptions(RemotingDirection.Inbound), RemoteMethod]
        public int RemoteMethod_Primitive_NoParam_Inbound() => ReturnedRemoteMethodPrimitive;

        [RemoteOptions(RemotingDirection.InboundClient), RemoteMethod]
        public int RemoteMethod_Primitive_NoParam_InboundClient() => ReturnedRemoteMethodPrimitive;

        [RemoteOptions(RemotingDirection.InboundHost), RemoteMethod]
        public int RemoteMethod_Primitive_NoParam_InboundHost() => ReturnedRemoteMethodPrimitive;

        [RemoteOptions(RemotingDirection.Outbound), RemoteMethod]
        public int RemoteMethod_Primitive_NoParam_Outbound() => ReturnedRemoteMethodPrimitive;

        [RemoteOptions(RemotingDirection.OutboundClient), RemoteMethod]
        public int RemoteMethod_Primitive_NoParam_OutboundClient() => ReturnedRemoteMethodPrimitive;

        [RemoteOptions(RemotingDirection.OutboundHost), RemoteMethod]
        public int RemoteMethod_Primitive_NoParam_OutboundHost() => ReturnedRemoteMethodPrimitive;

        // ===============================================================================

        [RemoteOptions(RemotingDirection.None), RemoteMethod]
        public Task RemoteMethod_Task_NoParam_None() => ReturnedRemoteMethodTask;

        [RemoteOptions(RemotingDirection.Bidirectional), RemoteMethod]
        public Task RemoteMethod_Task_NoParam_Bidirectional() => ReturnedRemoteMethodTask;

        [RemoteOptions(RemotingDirection.HostToClient), RemoteMethod]
        public Task RemoteMethod_Task_NoParam_HostToClient() => ReturnedRemoteMethodTask;

        [RemoteOptions(RemotingDirection.ClientToHost), RemoteMethod]
        public Task RemoteMethod_Task_NoParam_ClientToHost() => ReturnedRemoteMethodTask;

        [RemoteOptions(RemotingDirection.Inbound), RemoteMethod]
        public Task RemoteMethod_Task_NoParam_Inbound() => ReturnedRemoteMethodTask;

        [RemoteOptions(RemotingDirection.InboundClient), RemoteMethod]
        public Task RemoteMethod_Task_NoParam_InboundClient() => ReturnedRemoteMethodTask;

        [RemoteOptions(RemotingDirection.InboundHost), RemoteMethod]
        public Task RemoteMethod_Task_NoParam_InboundHost() => ReturnedRemoteMethodTask;

        [RemoteOptions(RemotingDirection.Outbound), RemoteMethod]
        public Task RemoteMethod_Task_NoParam_Outbound() => ReturnedRemoteMethodTask;

        [RemoteOptions(RemotingDirection.OutboundClient), RemoteMethod]
        public Task RemoteMethod_Task_NoParam_OutboundClient() => ReturnedRemoteMethodTask;

        [RemoteOptions(RemotingDirection.OutboundHost), RemoteMethod]
        public Task RemoteMethod_Task_NoParam_OutboundHost() => ReturnedRemoteMethodTask;

        // ===============================================================================

        [RemoteOptions(RemotingDirection.None), RemoteMethod]
        public Task<RemotingDirection> RemoteMethod_TaskEnum_NoParam_None() => ReturnedRemoteMethodTaskEnum;

        [RemoteOptions(RemotingDirection.Bidirectional), RemoteMethod]
        public Task<RemotingDirection> RemoteMethod_TaskEnum_NoParam_Bidirectional() => ReturnedRemoteMethodTaskEnum;

        [RemoteOptions(RemotingDirection.HostToClient), RemoteMethod]
        public Task<RemotingDirection> RemoteMethod_TaskEnum_NoParam_HostToClient() => ReturnedRemoteMethodTaskEnum;

        [RemoteOptions(RemotingDirection.ClientToHost), RemoteMethod]
        public Task<RemotingDirection> RemoteMethod_TaskEnum_NoParam_ClientToHost() => ReturnedRemoteMethodTaskEnum;

        [RemoteOptions(RemotingDirection.Inbound), RemoteMethod]
        public Task<RemotingDirection> RemoteMethod_TaskEnum_NoParam_Inbound() => ReturnedRemoteMethodTaskEnum;

        [RemoteOptions(RemotingDirection.InboundClient), RemoteMethod]
        public Task<RemotingDirection> RemoteMethod_TaskEnum_NoParam_InboundClient() => ReturnedRemoteMethodTaskEnum;

        [RemoteOptions(RemotingDirection.InboundHost), RemoteMethod]
        public Task<RemotingDirection> RemoteMethod_TaskEnum_NoParam_InboundHost() => ReturnedRemoteMethodTaskEnum;

        [RemoteOptions(RemotingDirection.Outbound), RemoteMethod]
        public Task<RemotingDirection> RemoteMethod_TaskEnum_NoParam_Outbound() => ReturnedRemoteMethodTaskEnum;

        [RemoteOptions(RemotingDirection.OutboundClient), RemoteMethod]
        public Task<RemotingDirection> RemoteMethod_TaskEnum_NoParam_OutboundClient() => ReturnedRemoteMethodTaskEnum;

        [RemoteOptions(RemotingDirection.OutboundHost), RemoteMethod]
        public Task<RemotingDirection> RemoteMethod_TaskEnum_NoParam_OutboundHost() => ReturnedRemoteMethodTaskEnum;

        // ===============================================================================

        [RemoteOptions(RemotingDirection.None), RemoteMethod]
        public Task<DateTime> RemoteMethod_TaskStruct_NoParam_None() => ReturnedRemoteMethodTaskStruct;

        [RemoteOptions(RemotingDirection.Bidirectional), RemoteMethod]
        public Task<DateTime> RemoteMethod_TaskStruct_NoParam_Bidirectional() => ReturnedRemoteMethodTaskStruct;

        [RemoteOptions(RemotingDirection.HostToClient), RemoteMethod]
        public Task<DateTime> RemoteMethod_TaskStruct_NoParam_HostToClient() => ReturnedRemoteMethodTaskStruct;

        [RemoteOptions(RemotingDirection.ClientToHost), RemoteMethod]
        public Task<DateTime> RemoteMethod_TaskStruct_NoParam_ClientToHost() => ReturnedRemoteMethodTaskStruct;

        [RemoteOptions(RemotingDirection.Inbound), RemoteMethod]
        public Task<DateTime> RemoteMethod_TaskStruct_NoParam_Inbound() => ReturnedRemoteMethodTaskStruct;

        [RemoteOptions(RemotingDirection.InboundClient), RemoteMethod]
        public Task<DateTime> RemoteMethod_TaskStruct_NoParam_InboundClient() => ReturnedRemoteMethodTaskStruct;

        [RemoteOptions(RemotingDirection.InboundHost), RemoteMethod]
        public Task<DateTime> RemoteMethod_TaskStruct_NoParam_InboundHost() => ReturnedRemoteMethodTaskStruct;

        [RemoteOptions(RemotingDirection.Outbound), RemoteMethod]
        public Task<DateTime> RemoteMethod_TaskStruct_NoParam_Outbound() => ReturnedRemoteMethodTaskStruct;

        [RemoteOptions(RemotingDirection.OutboundClient), RemoteMethod]
        public Task<DateTime> RemoteMethod_TaskStruct_NoParam_OutboundClient() => ReturnedRemoteMethodTaskStruct;

        [RemoteOptions(RemotingDirection.OutboundHost), RemoteMethod]
        public Task<DateTime> RemoteMethod_TaskStruct_NoParam_OutboundHost() => ReturnedRemoteMethodTaskStruct;

        // ===============================================================================

        [RemoteOptions(RemotingDirection.None), RemoteMethod]
        public Task<object> RemoteMethod_TaskObject_NoParam_None() => ReturnedRemoteMethodTaskObject;

        [RemoteOptions(RemotingDirection.Bidirectional), RemoteMethod]
        public Task<object> RemoteMethod_TaskObject_NoParam_Bidirectional() => ReturnedRemoteMethodTaskObject;

        [RemoteOptions(RemotingDirection.HostToClient), RemoteMethod]
        public Task<object> RemoteMethod_TaskObject_NoParam_HostToClient() => ReturnedRemoteMethodTaskObject;

        [RemoteOptions(RemotingDirection.ClientToHost), RemoteMethod]
        public Task<object> RemoteMethod_TaskObject_NoParam_ClientToHost() => ReturnedRemoteMethodTaskObject;

        [RemoteOptions(RemotingDirection.Inbound), RemoteMethod]
        public Task<object> RemoteMethod_TaskObject_NoParam_Inbound() => ReturnedRemoteMethodTaskObject;

        [RemoteOptions(RemotingDirection.InboundClient), RemoteMethod]
        public Task<object> RemoteMethod_TaskObject_NoParam_InboundClient() => ReturnedRemoteMethodTaskObject;

        [RemoteOptions(RemotingDirection.InboundHost), RemoteMethod]
        public Task<object> RemoteMethod_TaskObject_NoParam_InboundHost() => ReturnedRemoteMethodTaskObject;

        [RemoteOptions(RemotingDirection.Outbound), RemoteMethod]
        public Task<object> RemoteMethod_TaskObject_NoParam_Outbound() => ReturnedRemoteMethodTaskObject;

        [RemoteOptions(RemotingDirection.OutboundClient), RemoteMethod]
        public Task<object> RemoteMethod_TaskObject_NoParam_OutboundClient() => ReturnedRemoteMethodTaskObject;

        [RemoteOptions(RemotingDirection.OutboundHost), RemoteMethod]
        public Task<object> RemoteMethod_TaskObject_NoParam_OutboundHost() => ReturnedRemoteMethodTaskObject;

        // ===============================================================================

        [RemoteOptions(RemotingDirection.None), RemoteMethod]
        public Task<int> RemoteMethod_TaskPrimitive_NoParam_None() => ReturnedRemoteMethodTaskPrimitive;

        [RemoteOptions(RemotingDirection.Bidirectional), RemoteMethod]
        public Task<int> RemoteMethod_TaskPrimitive_NoParam_Bidirectional() => ReturnedRemoteMethodTaskPrimitive;

        [RemoteOptions(RemotingDirection.HostToClient), RemoteMethod]
        public Task<int> RemoteMethod_TaskPrimitive_NoParam_HostToClient() => ReturnedRemoteMethodTaskPrimitive;

        [RemoteOptions(RemotingDirection.ClientToHost), RemoteMethod]
        public Task<int> RemoteMethod_TaskPrimitive_NoParam_ClientToHost() => ReturnedRemoteMethodTaskPrimitive;

        [RemoteOptions(RemotingDirection.Inbound), RemoteMethod]
        public Task<int> RemoteMethod_TaskPrimitive_NoParam_Inbound() => ReturnedRemoteMethodTaskPrimitive;

        [RemoteOptions(RemotingDirection.InboundClient), RemoteMethod]
        public Task<int> RemoteMethod_TaskPrimitive_NoParam_InboundClient() => ReturnedRemoteMethodTaskPrimitive;

        [RemoteOptions(RemotingDirection.InboundHost), RemoteMethod]
        public Task<int> RemoteMethod_TaskPrimitive_NoParam_InboundHost() => ReturnedRemoteMethodTaskPrimitive;

        [RemoteOptions(RemotingDirection.Outbound), RemoteMethod]
        public Task<int> RemoteMethod_TaskPrimitive_NoParam_Outbound() => ReturnedRemoteMethodTaskPrimitive;

        [RemoteOptions(RemotingDirection.OutboundClient), RemoteMethod]
        public Task<int> RemoteMethod_TaskPrimitive_NoParam_OutboundClient() => ReturnedRemoteMethodTaskPrimitive;

        [RemoteOptions(RemotingDirection.OutboundHost), RemoteMethod]
        public Task<int> RemoteMethod_TaskPrimitive_NoParam_OutboundHost() => ReturnedRemoteMethodTaskPrimitive;
    }
}
