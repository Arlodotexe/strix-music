using OwlCore.Remoting;
using System;
using System.Threading.Tasks;

namespace OwlCore.Tests.Remoting.Mock
{
    public partial class MemberRemoteTestClass
    {
        [RemoteOptions(RemotingDirection.None), RemoteMethod]
        public void RemoteMethod_Void_ObjectParam_None(object obj) => RemoteMethodCalledObject?.Invoke(this, obj);

        [RemoteOptions(RemotingDirection.Bidirectional), RemoteMethod]
        public void RemoteMethod_Void_ObjectParam_Bidirectional(object obj) => RemoteMethodCalledObject?.Invoke(this, obj);

        [RemoteOptions(RemotingDirection.HostToClient), RemoteMethod]
        public void RemoteMethod_Void_ObjectParam_HostToClient(object obj) => RemoteMethodCalledObject?.Invoke(this, obj);

        [RemoteOptions(RemotingDirection.ClientToHost), RemoteMethod]
        public void RemoteMethod_Void_ObjectParam_ClientToHost(object obj) => RemoteMethodCalledObject?.Invoke(this, obj);

        [RemoteOptions(RemotingDirection.Inbound), RemoteMethod]
        public void RemoteMethod_Void_ObjectParam_Inbound(object obj) => RemoteMethodCalledObject?.Invoke(this, obj);

        [RemoteOptions(RemotingDirection.InboundClient), RemoteMethod]
        public void RemoteMethod_Void_ObjectParam_InboundClient(object obj) => RemoteMethodCalledObject?.Invoke(this, obj);

        [RemoteOptions(RemotingDirection.InboundHost), RemoteMethod]
        public void RemoteMethod_Void_ObjectParam_InboundHost(object obj) => RemoteMethodCalledObject?.Invoke(this, obj);

        [RemoteOptions(RemotingDirection.Outbound), RemoteMethod]
        public void RemoteMethod_Void_ObjectParam_Outbound(object obj) => RemoteMethodCalledObject?.Invoke(this, obj);

        [RemoteOptions(RemotingDirection.OutboundClient), RemoteMethod]
        public void RemoteMethod_Void_ObjectParam_OutboundClient(object obj) => RemoteMethodCalledObject?.Invoke(this, obj);

        [RemoteOptions(RemotingDirection.OutboundHost), RemoteMethod]
        public void RemoteMethod_Void_ObjectParam_OutboundHost(object obj) => RemoteMethodCalledObject?.Invoke(this, obj);

        // ===============================================================================

        public RemotingDirection RouteEnumWithEventCall(object obj)
        {
            RemoteMethodCalledObject?.Invoke(this, obj);
            return ReturnedRemoteMethodEnum;
        }

        [RemoteOptions(RemotingDirection.None), RemoteMethod]
        public RemotingDirection RemoteMethod_Enum_ObjectParam_None(object obj) => RouteEnumWithEventCall(obj);

        [RemoteOptions(RemotingDirection.Bidirectional), RemoteMethod]
        public RemotingDirection RemoteMethod_Enum_ObjectParam_Bidirectional(object obj) => RouteEnumWithEventCall(obj);

        [RemoteOptions(RemotingDirection.HostToClient), RemoteMethod]
        public RemotingDirection RemoteMethod_Enum_ObjectParam_HostToClient(object obj) => RouteEnumWithEventCall(obj);

        [RemoteOptions(RemotingDirection.ClientToHost), RemoteMethod]
        public RemotingDirection RemoteMethod_Enum_ObjectParam_ClientToHost(object obj) => RouteEnumWithEventCall(obj);

        [RemoteOptions(RemotingDirection.Inbound), RemoteMethod]
        public RemotingDirection RemoteMethod_Enum_ObjectParam_Inbound(object obj) => RouteEnumWithEventCall(obj);

        [RemoteOptions(RemotingDirection.InboundClient), RemoteMethod]
        public RemotingDirection RemoteMethod_Enum_ObjectParam_InboundClient(object obj) => RouteEnumWithEventCall(obj);

        [RemoteOptions(RemotingDirection.InboundHost), RemoteMethod]
        public RemotingDirection RemoteMethod_Enum_ObjectParam_InboundHost(object obj) => RouteEnumWithEventCall(obj);

        [RemoteOptions(RemotingDirection.Outbound), RemoteMethod]
        public RemotingDirection RemoteMethod_Enum_ObjectParam_Outbound(object obj) => RouteEnumWithEventCall(obj);

        [RemoteOptions(RemotingDirection.OutboundClient), RemoteMethod]
        public RemotingDirection RemoteMethod_Enum_ObjectParam_OutboundClient(object obj) => RouteEnumWithEventCall(obj);

        [RemoteOptions(RemotingDirection.OutboundHost), RemoteMethod]
        public RemotingDirection RemoteMethod_Enum_ObjectParam_OutboundHost(object obj) => RouteEnumWithEventCall(obj);

        // ===============================================================================

        public DateTime RouteDateTimeWithEventCall(object obj)
        {
            RemoteMethodCalledObject?.Invoke(this, obj);
            return ReturnedRemoteMethodStruct;
        }

        [RemoteOptions(RemotingDirection.None), RemoteMethod]
        public DateTime RemoteMethod_Struct_ObjectParam_None(object obj) => RouteDateTimeWithEventCall(obj);

        [RemoteOptions(RemotingDirection.Bidirectional), RemoteMethod]
        public DateTime RemoteMethod_Struct_ObjectParam_Bidirectional(object obj) => RouteDateTimeWithEventCall(obj);

        [RemoteOptions(RemotingDirection.HostToClient), RemoteMethod]
        public DateTime RemoteMethod_Struct_ObjectParam_HostToClient(object obj) => RouteDateTimeWithEventCall(obj);

        [RemoteOptions(RemotingDirection.ClientToHost), RemoteMethod]
        public DateTime RemoteMethod_Struct_ObjectParam_ClientToHost(object obj) => RouteDateTimeWithEventCall(obj);

        [RemoteOptions(RemotingDirection.Inbound), RemoteMethod]
        public DateTime RemoteMethod_Struct_ObjectParam_Inbound(object obj) => RouteDateTimeWithEventCall(obj);

        [RemoteOptions(RemotingDirection.InboundClient), RemoteMethod]
        public DateTime RemoteMethod_Struct_ObjectParam_InboundClient(object obj) => RouteDateTimeWithEventCall(obj);

        [RemoteOptions(RemotingDirection.InboundHost), RemoteMethod]
        public DateTime RemoteMethod_Struct_ObjectParam_InboundHost(object obj) => RouteDateTimeWithEventCall(obj);

        [RemoteOptions(RemotingDirection.Outbound), RemoteMethod]
        public DateTime RemoteMethod_Struct_ObjectParam_Outbound(object obj) => RouteDateTimeWithEventCall(obj);

        [RemoteOptions(RemotingDirection.OutboundClient), RemoteMethod]
        public DateTime RemoteMethod_Struct_ObjectParam_OutboundClient(object obj) => RouteDateTimeWithEventCall(obj);

        [RemoteOptions(RemotingDirection.OutboundHost), RemoteMethod]
        public DateTime RemoteMethod_Struct_ObjectParam_OutboundHost(object obj) => RouteDateTimeWithEventCall(obj);

        // ===============================================================================

        public object RouteObjectWithEventCall(object obj)
        {
            RemoteMethodCalledObject?.Invoke(this, obj);
            return obj;
        }

        [RemoteOptions(RemotingDirection.None), RemoteMethod]
        public object RemoteMethod_Object_ObjectParam_None(object obj) => RouteObjectWithEventCall(obj);

        [RemoteOptions(RemotingDirection.Bidirectional), RemoteMethod]
        public object RemoteMethod_Object_ObjectParam_Bidirectional(object obj) => RouteObjectWithEventCall(obj);

        [RemoteOptions(RemotingDirection.HostToClient), RemoteMethod]
        public object RemoteMethod_Object_ObjectParam_HostToClient(object obj) => RouteObjectWithEventCall(obj);

        [RemoteOptions(RemotingDirection.ClientToHost), RemoteMethod]
        public object RemoteMethod_Object_ObjectParam_ClientToHost(object obj) => RouteObjectWithEventCall(obj);

        [RemoteOptions(RemotingDirection.Inbound), RemoteMethod]
        public object RemoteMethod_Object_ObjectParam_Inbound(object obj) => RouteObjectWithEventCall(obj);

        [RemoteOptions(RemotingDirection.InboundClient), RemoteMethod]
        public object RemoteMethod_Object_ObjectParam_InboundClient(object obj) => RouteObjectWithEventCall(obj);

        [RemoteOptions(RemotingDirection.InboundHost), RemoteMethod]
        public object RemoteMethod_Object_ObjectParam_InboundHost(object obj) => RouteObjectWithEventCall(obj);

        [RemoteOptions(RemotingDirection.Outbound), RemoteMethod]
        public object RemoteMethod_Object_ObjectParam_Outbound(object obj) => RouteObjectWithEventCall(obj);

        [RemoteOptions(RemotingDirection.OutboundClient), RemoteMethod]
        public object RemoteMethod_Object_ObjectParam_OutboundClient(object obj) => RouteObjectWithEventCall(obj);

        [RemoteOptions(RemotingDirection.OutboundHost), RemoteMethod]
        public object RemoteMethod_Object_ObjectParam_OutboundHost(object obj) => RouteObjectWithEventCall(obj);

        // ===============================================================================

        public int RoutePrimitiveWithEventCall(object obj)
        {
            RemoteMethodCalledObject?.Invoke(this, obj);
            return ReturnedRemoteMethodPrimitive;
        }

        [RemoteOptions(RemotingDirection.None), RemoteMethod]
        public int RemoteMethod_Primitive_ObjectParam_None(object obj) => RoutePrimitiveWithEventCall(obj);

        [RemoteOptions(RemotingDirection.Bidirectional), RemoteMethod]
        public int RemoteMethod_Primitive_ObjectParam_Bidirectional(object obj) => RoutePrimitiveWithEventCall(obj);

        [RemoteOptions(RemotingDirection.HostToClient), RemoteMethod]
        public int RemoteMethod_Primitive_ObjectParam_HostToClient(object obj) => RoutePrimitiveWithEventCall(obj);

        [RemoteOptions(RemotingDirection.ClientToHost), RemoteMethod]
        public int RemoteMethod_Primitive_ObjectParam_ClientToHost(object obj) => RoutePrimitiveWithEventCall(obj);

        [RemoteOptions(RemotingDirection.Inbound), RemoteMethod]
        public int RemoteMethod_Primitive_ObjectParam_Inbound(object obj) => RoutePrimitiveWithEventCall(obj);

        [RemoteOptions(RemotingDirection.InboundClient), RemoteMethod]
        public int RemoteMethod_Primitive_ObjectParam_InboundClient(object obj) => RoutePrimitiveWithEventCall(obj);

        [RemoteOptions(RemotingDirection.InboundHost), RemoteMethod]
        public int RemoteMethod_Primitive_ObjectParam_InboundHost(object obj) => RoutePrimitiveWithEventCall(obj);

        [RemoteOptions(RemotingDirection.Outbound), RemoteMethod]
        public int RemoteMethod_Primitive_ObjectParam_Outbound(object obj) => RoutePrimitiveWithEventCall(obj);

        [RemoteOptions(RemotingDirection.OutboundClient), RemoteMethod]
        public int RemoteMethod_Primitive_ObjectParam_OutboundClient(object obj) => RoutePrimitiveWithEventCall(obj);

        [RemoteOptions(RemotingDirection.OutboundHost), RemoteMethod]
        public int RemoteMethod_Primitive_ObjectParam_OutboundHost(object obj) => RoutePrimitiveWithEventCall(obj);

        // ===============================================================================

        public Task RouteTaskWithEventCall(object obj)
        {
            RemoteMethodCalledObject?.Invoke(this, obj);
            return ReturnedRemoteMethodTask;
        }

        [RemoteOptions(RemotingDirection.None), RemoteMethod]
        public Task RemoteMethod_Task_ObjectParam_None(object obj) => RouteTaskWithEventCall(obj);

        [RemoteOptions(RemotingDirection.Bidirectional), RemoteMethod]
        public Task RemoteMethod_Task_ObjectParam_Bidirectional(object obj) => RouteTaskWithEventCall(obj);

        [RemoteOptions(RemotingDirection.HostToClient), RemoteMethod]
        public Task RemoteMethod_Task_ObjectParam_HostToClient(object obj) => RouteTaskWithEventCall(obj);

        [RemoteOptions(RemotingDirection.ClientToHost), RemoteMethod]
        public Task RemoteMethod_Task_ObjectParam_ClientToHost(object obj) => RouteTaskWithEventCall(obj);

        [RemoteOptions(RemotingDirection.Inbound), RemoteMethod]
        public Task RemoteMethod_Task_ObjectParam_Inbound(object obj) => RouteTaskWithEventCall(obj);

        [RemoteOptions(RemotingDirection.InboundClient), RemoteMethod]
        public Task RemoteMethod_Task_ObjectParam_InboundClient(object obj) => RouteTaskWithEventCall(obj);

        [RemoteOptions(RemotingDirection.InboundHost), RemoteMethod]
        public Task RemoteMethod_Task_ObjectParam_InboundHost(object obj) => RouteTaskWithEventCall(obj);

        [RemoteOptions(RemotingDirection.Outbound), RemoteMethod]
        public Task RemoteMethod_Task_ObjectParam_Outbound(object obj) => RouteTaskWithEventCall(obj);

        [RemoteOptions(RemotingDirection.OutboundClient), RemoteMethod]
        public Task RemoteMethod_Task_ObjectParam_OutboundClient(object obj) => RouteTaskWithEventCall(obj);

        [RemoteOptions(RemotingDirection.OutboundHost), RemoteMethod]
        public Task RemoteMethod_Task_ObjectParam_OutboundHost(object obj) => RouteTaskWithEventCall(obj);

        // ===============================================================================

        public Task<RemotingDirection> RouteTaskEnumWithEventCall(object obj)
        {
            RemoteMethodCalledObject?.Invoke(this, obj);
            return ReturnedRemoteMethodTaskEnum;
        }

        [RemoteOptions(RemotingDirection.None), RemoteMethod]
        public Task<RemotingDirection> RemoteMethod_TaskEnum_ObjectParam_None(object obj) => RouteTaskEnumWithEventCall(obj);

        [RemoteOptions(RemotingDirection.Bidirectional), RemoteMethod]
        public Task<RemotingDirection> RemoteMethod_TaskEnum_ObjectParam_Bidirectional(object obj) => RouteTaskEnumWithEventCall(obj);

        [RemoteOptions(RemotingDirection.HostToClient), RemoteMethod]
        public Task<RemotingDirection> RemoteMethod_TaskEnum_ObjectParam_HostToClient(object obj) => RouteTaskEnumWithEventCall(obj);

        [RemoteOptions(RemotingDirection.ClientToHost), RemoteMethod]
        public Task<RemotingDirection> RemoteMethod_TaskEnum_ObjectParam_ClientToHost(object obj) => RouteTaskEnumWithEventCall(obj);

        [RemoteOptions(RemotingDirection.Inbound), RemoteMethod]
        public Task<RemotingDirection> RemoteMethod_TaskEnum_ObjectParam_Inbound(object obj) => RouteTaskEnumWithEventCall(obj);

        [RemoteOptions(RemotingDirection.InboundClient), RemoteMethod]
        public Task<RemotingDirection> RemoteMethod_TaskEnum_ObjectParam_InboundClient(object obj) => RouteTaskEnumWithEventCall(obj);

        [RemoteOptions(RemotingDirection.InboundHost), RemoteMethod]
        public Task<RemotingDirection> RemoteMethod_TaskEnum_ObjectParam_InboundHost(object obj) => RouteTaskEnumWithEventCall(obj);

        [RemoteOptions(RemotingDirection.Outbound), RemoteMethod]
        public Task<RemotingDirection> RemoteMethod_TaskEnum_ObjectParam_Outbound(object obj) => RouteTaskEnumWithEventCall(obj);

        [RemoteOptions(RemotingDirection.OutboundClient), RemoteMethod]
        public Task<RemotingDirection> RemoteMethod_TaskEnum_ObjectParam_OutboundClient(object obj) => RouteTaskEnumWithEventCall(obj);

        [RemoteOptions(RemotingDirection.OutboundHost), RemoteMethod]
        public Task<RemotingDirection> RemoteMethod_TaskEnum_ObjectParam_OutboundHost(object obj) => RouteTaskEnumWithEventCall(obj);

        // ===============================================================================

        public Task<DateTime> RouteTaskStructWithEventCall(object obj)
        {
            RemoteMethodCalledObject?.Invoke(this, obj);
            return ReturnedRemoteMethodTaskStruct;
        }

        [RemoteOptions(RemotingDirection.None), RemoteMethod]
        public Task<DateTime> RemoteMethod_TaskStruct_ObjectParam_None(object obj) => RouteTaskStructWithEventCall(obj);

        [RemoteOptions(RemotingDirection.Bidirectional), RemoteMethod]
        public Task<DateTime> RemoteMethod_TaskStruct_ObjectParam_Bidirectional(object obj) => RouteTaskStructWithEventCall(obj);

        [RemoteOptions(RemotingDirection.HostToClient), RemoteMethod]
        public Task<DateTime> RemoteMethod_TaskStruct_ObjectParam_HostToClient(object obj) => RouteTaskStructWithEventCall(obj);

        [RemoteOptions(RemotingDirection.ClientToHost), RemoteMethod]
        public Task<DateTime> RemoteMethod_TaskStruct_ObjectParam_ClientToHost(object obj) => RouteTaskStructWithEventCall(obj);

        [RemoteOptions(RemotingDirection.Inbound), RemoteMethod]
        public Task<DateTime> RemoteMethod_TaskStruct_ObjectParam_Inbound(object obj) => RouteTaskStructWithEventCall(obj);

        [RemoteOptions(RemotingDirection.InboundClient), RemoteMethod]
        public Task<DateTime> RemoteMethod_TaskStruct_ObjectParam_InboundClient(object obj) => RouteTaskStructWithEventCall(obj);

        [RemoteOptions(RemotingDirection.InboundHost), RemoteMethod]
        public Task<DateTime> RemoteMethod_TaskStruct_ObjectParam_InboundHost(object obj) => RouteTaskStructWithEventCall(obj);

        [RemoteOptions(RemotingDirection.Outbound), RemoteMethod]
        public Task<DateTime> RemoteMethod_TaskStruct_ObjectParam_Outbound(object obj) => RouteTaskStructWithEventCall(obj);

        [RemoteOptions(RemotingDirection.OutboundClient), RemoteMethod]
        public Task<DateTime> RemoteMethod_TaskStruct_ObjectParam_OutboundClient(object obj) => RouteTaskStructWithEventCall(obj);

        [RemoteOptions(RemotingDirection.OutboundHost), RemoteMethod]
        public Task<DateTime> RemoteMethod_TaskStruct_ObjectParam_OutboundHost(object obj) => RouteTaskStructWithEventCall(obj);

        // ===============================================================================

        public Task<object> RouteTaskObjectWithEventCall(object obj)
        {
            RemoteMethodCalledObject?.Invoke(this, obj);
            return Task.FromResult(obj);
        }

        [RemoteOptions(RemotingDirection.None), RemoteMethod]
        public Task<object> RemoteMethod_TaskObject_ObjectParam_None(object obj) => RouteTaskObjectWithEventCall(obj);

        [RemoteOptions(RemotingDirection.Bidirectional), RemoteMethod]
        public Task<object> RemoteMethod_TaskObject_ObjectParam_Bidirectional(object obj) => RouteTaskObjectWithEventCall(obj);

        [RemoteOptions(RemotingDirection.HostToClient), RemoteMethod]
        public Task<object> RemoteMethod_TaskObject_ObjectParam_HostToClient(object obj) => RouteTaskObjectWithEventCall(obj);

        [RemoteOptions(RemotingDirection.ClientToHost), RemoteMethod]
        public Task<object> RemoteMethod_TaskObject_ObjectParam_ClientToHost(object obj) => RouteTaskObjectWithEventCall(obj);

        [RemoteOptions(RemotingDirection.Inbound), RemoteMethod]
        public Task<object> RemoteMethod_TaskObject_ObjectParam_Inbound(object obj) => RouteTaskObjectWithEventCall(obj);

        [RemoteOptions(RemotingDirection.InboundClient), RemoteMethod]
        public Task<object> RemoteMethod_TaskObject_ObjectParam_InboundClient(object obj) => RouteTaskObjectWithEventCall(obj);

        [RemoteOptions(RemotingDirection.InboundHost), RemoteMethod]
        public Task<object> RemoteMethod_TaskObject_ObjectParam_InboundHost(object obj) => RouteTaskObjectWithEventCall(obj);

        [RemoteOptions(RemotingDirection.Outbound), RemoteMethod]
        public Task<object> RemoteMethod_TaskObject_ObjectParam_Outbound(object obj) => RouteTaskObjectWithEventCall(obj);

        [RemoteOptions(RemotingDirection.OutboundClient), RemoteMethod]
        public Task<object> RemoteMethod_TaskObject_ObjectParam_OutboundClient(object obj) => RouteTaskObjectWithEventCall(obj);

        [RemoteOptions(RemotingDirection.OutboundHost), RemoteMethod]
        public Task<object> RemoteMethod_TaskObject_ObjectParam_OutboundHost(object obj) => RouteTaskObjectWithEventCall(obj);

        // ===============================================================================

        public Task<int> RouteTaskPrimitiveWithEventCall(object obj)
        {
            RemoteMethodCalledObject?.Invoke(this, obj);
            return ReturnedRemoteMethodTaskPrimitive;
        }

        [RemoteOptions(RemotingDirection.None), RemoteMethod]
        public Task<int> RemoteMethod_TaskPrimitive_ObjectParam_None(object obj) => RouteTaskPrimitiveWithEventCall(obj);

        [RemoteOptions(RemotingDirection.Bidirectional), RemoteMethod]
        public Task<int> RemoteMethod_TaskPrimitive_ObjectParam_Bidirectional(object obj) => RouteTaskPrimitiveWithEventCall(obj);

        [RemoteOptions(RemotingDirection.HostToClient), RemoteMethod]
        public Task<int> RemoteMethod_TaskPrimitive_ObjectParam_HostToClient(object obj) => RouteTaskPrimitiveWithEventCall(obj);

        [RemoteOptions(RemotingDirection.ClientToHost), RemoteMethod]
        public Task<int> RemoteMethod_TaskPrimitive_ObjectParam_ClientToHost(object obj) => RouteTaskPrimitiveWithEventCall(obj);

        [RemoteOptions(RemotingDirection.Inbound), RemoteMethod]
        public Task<int> RemoteMethod_TaskPrimitive_ObjectParam_Inbound(object obj) => RouteTaskPrimitiveWithEventCall(obj);

        [RemoteOptions(RemotingDirection.InboundClient), RemoteMethod]
        public Task<int> RemoteMethod_TaskPrimitive_ObjectParam_InboundClient(object obj) => RouteTaskPrimitiveWithEventCall(obj);

        [RemoteOptions(RemotingDirection.InboundHost), RemoteMethod]
        public Task<int> RemoteMethod_TaskPrimitive_ObjectParam_InboundHost(object obj) => RouteTaskPrimitiveWithEventCall(obj);

        [RemoteOptions(RemotingDirection.Outbound), RemoteMethod]
        public Task<int> RemoteMethod_TaskPrimitive_ObjectParam_Outbound(object obj) => RouteTaskPrimitiveWithEventCall(obj);

        [RemoteOptions(RemotingDirection.OutboundClient), RemoteMethod]
        public Task<int> RemoteMethod_TaskPrimitive_ObjectParam_OutboundClient(object obj) => RouteTaskPrimitiveWithEventCall(obj);

        [RemoteOptions(RemotingDirection.OutboundHost), RemoteMethod]
        public Task<int> RemoteMethod_TaskPrimitive_ObjectParam_OutboundHost(object obj) => RouteTaskPrimitiveWithEventCall(obj);
    }
}
