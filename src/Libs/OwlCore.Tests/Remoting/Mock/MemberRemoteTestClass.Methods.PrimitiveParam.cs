using OwlCore.Remoting;
using System;
using System.Threading.Tasks;

namespace OwlCore.Tests.Remoting.Mock
{
    public partial class MemberRemoteTestClass
    {
        [RemoteOptions(RemotingDirection.None), RemoteMethod]
        public void RemoteMethod_Void_PrimitiveParam_None(int primitive) => RemoteMethodCalledPrimitive?.Invoke(this, primitive);

        [RemoteOptions(RemotingDirection.Bidirectional), RemoteMethod]
        public void RemoteMethod_Void_PrimitiveParam_Bidirectional(int primitive) => RemoteMethodCalledPrimitive?.Invoke(this, primitive);

        [RemoteOptions(RemotingDirection.HostToClient), RemoteMethod]
        public void RemoteMethod_Void_PrimitiveParam_HostToClient(int primitive) => RemoteMethodCalledPrimitive?.Invoke(this, primitive);

        [RemoteOptions(RemotingDirection.ClientToHost), RemoteMethod]
        public void RemoteMethod_Void_PrimitiveParam_ClientToHost(int primitive) => RemoteMethodCalledPrimitive?.Invoke(this, primitive);

        [RemoteOptions(RemotingDirection.Inbound), RemoteMethod]
        public void RemoteMethod_Void_PrimitiveParam_Inbound(int primitive) => RemoteMethodCalledPrimitive?.Invoke(this, primitive);

        [RemoteOptions(RemotingDirection.InboundClient), RemoteMethod]
        public void RemoteMethod_Void_PrimitiveParam_InboundClient(int primitive) => RemoteMethodCalledPrimitive?.Invoke(this, primitive);

        [RemoteOptions(RemotingDirection.InboundHost), RemoteMethod]
        public void RemoteMethod_Void_PrimitiveParam_InboundHost(int primitive) => RemoteMethodCalledPrimitive?.Invoke(this, primitive);

        [RemoteOptions(RemotingDirection.Outbound), RemoteMethod]
        public void RemoteMethod_Void_PrimitiveParam_Outbound(int primitive) => RemoteMethodCalledPrimitive?.Invoke(this, primitive);

        [RemoteOptions(RemotingDirection.OutboundClient), RemoteMethod]
        public void RemoteMethod_Void_PrimitiveParam_OutboundClient(int primitive) => RemoteMethodCalledPrimitive?.Invoke(this, primitive);

        [RemoteOptions(RemotingDirection.OutboundHost), RemoteMethod]
        public void RemoteMethod_Void_PrimitiveParam_OutboundHost(int primitive) => RemoteMethodCalledPrimitive?.Invoke(this, primitive);

        // ===============================================================================

        public RemotingDirection RouteEnumWithEventCall(int primitive)
        {
            RemoteMethodCalledPrimitive?.Invoke(this, primitive);
            return ReturnedRemoteMethodEnum;
        }

        [RemoteOptions(RemotingDirection.None), RemoteMethod]
        public RemotingDirection RemoteMethod_Enum_PrimitiveParam_None(int primitive) => RouteEnumWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.Bidirectional), RemoteMethod]
        public RemotingDirection RemoteMethod_Enum_PrimitiveParam_Bidirectional(int primitive) => RouteEnumWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.HostToClient), RemoteMethod]
        public RemotingDirection RemoteMethod_Enum_PrimitiveParam_HostToClient(int primitive) => RouteEnumWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.ClientToHost), RemoteMethod]
        public RemotingDirection RemoteMethod_Enum_PrimitiveParam_ClientToHost(int primitive) => RouteEnumWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.Inbound), RemoteMethod]
        public RemotingDirection RemoteMethod_Enum_PrimitiveParam_Inbound(int primitive) => RouteEnumWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.InboundClient), RemoteMethod]
        public RemotingDirection RemoteMethod_Enum_PrimitiveParam_InboundClient(int primitive) => RouteEnumWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.InboundHost), RemoteMethod]
        public RemotingDirection RemoteMethod_Enum_PrimitiveParam_InboundHost(int primitive) => RouteEnumWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.Outbound), RemoteMethod]
        public RemotingDirection RemoteMethod_Enum_PrimitiveParam_Outbound(int primitive) => RouteEnumWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.OutboundClient), RemoteMethod]
        public RemotingDirection RemoteMethod_Enum_PrimitiveParam_OutboundClient(int primitive) => RouteEnumWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.OutboundHost), RemoteMethod]
        public RemotingDirection RemoteMethod_Enum_PrimitiveParam_OutboundHost(int primitive) => RouteEnumWithEventCall(primitive);

        // ===============================================================================

        public DateTime RouteDateTimeWithEventCall(int primitive)
        {
            RemoteMethodCalledPrimitive?.Invoke(this, primitive);
            return ReturnedRemoteMethodStruct;
        }

        [RemoteOptions(RemotingDirection.None), RemoteMethod]
        public DateTime RemoteMethod_Struct_PrimitiveParam_None(int primitive) => RouteDateTimeWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.Bidirectional), RemoteMethod]
        public DateTime RemoteMethod_Struct_PrimitiveParam_Bidirectional(int primitive) => RouteDateTimeWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.HostToClient), RemoteMethod]
        public DateTime RemoteMethod_Struct_PrimitiveParam_HostToClient(int primitive) => RouteDateTimeWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.ClientToHost), RemoteMethod]
        public DateTime RemoteMethod_Struct_PrimitiveParam_ClientToHost(int primitive) => RouteDateTimeWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.Inbound), RemoteMethod]
        public DateTime RemoteMethod_Struct_PrimitiveParam_Inbound(int primitive) => RouteDateTimeWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.InboundClient), RemoteMethod]
        public DateTime RemoteMethod_Struct_PrimitiveParam_InboundClient(int primitive) => RouteDateTimeWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.InboundHost), RemoteMethod]
        public DateTime RemoteMethod_Struct_PrimitiveParam_InboundHost(int primitive) => RouteDateTimeWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.Outbound), RemoteMethod]
        public DateTime RemoteMethod_Struct_PrimitiveParam_Outbound(int primitive) => RouteDateTimeWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.OutboundClient), RemoteMethod]
        public DateTime RemoteMethod_Struct_PrimitiveParam_OutboundClient(int primitive) => RouteDateTimeWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.OutboundHost), RemoteMethod]
        public DateTime RemoteMethod_Struct_PrimitiveParam_OutboundHost(int primitive) => RouteDateTimeWithEventCall(primitive);

        // ===============================================================================

        public object RouteObjectWithEventCall(int primitive)
        {
            RemoteMethodCalledPrimitive?.Invoke(this, primitive);
            return ReturnedRemoteMethodObject;
        }

        [RemoteOptions(RemotingDirection.None), RemoteMethod]
        public object RemoteMethod_Object_PrimitiveParam_None(int primitive) => RouteObjectWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.Bidirectional), RemoteMethod]
        public object RemoteMethod_Object_PrimitiveParam_Bidirectional(int primitive) => RouteObjectWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.HostToClient), RemoteMethod]
        public object RemoteMethod_Object_PrimitiveParam_HostToClient(int primitive) => RouteObjectWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.ClientToHost), RemoteMethod]
        public object RemoteMethod_Object_PrimitiveParam_ClientToHost(int primitive) => RouteObjectWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.Inbound), RemoteMethod]
        public object RemoteMethod_Object_PrimitiveParam_Inbound(int primitive) => RouteObjectWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.InboundClient), RemoteMethod]
        public object RemoteMethod_Object_PrimitiveParam_InboundClient(int primitive) => RouteObjectWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.InboundHost), RemoteMethod]
        public object RemoteMethod_Object_PrimitiveParam_InboundHost(int primitive) => RouteObjectWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.Outbound), RemoteMethod]
        public object RemoteMethod_Object_PrimitiveParam_Outbound(int primitive) => RouteObjectWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.OutboundClient), RemoteMethod]
        public object RemoteMethod_Object_PrimitiveParam_OutboundClient(int primitive) => RouteObjectWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.OutboundHost), RemoteMethod]
        public object RemoteMethod_Object_PrimitiveParam_OutboundHost(int primitive) => RouteObjectWithEventCall(primitive);

        // ===============================================================================

        public int RoutePrimitiveWithEventCall(int primitive)
        {
            RemoteMethodCalledPrimitive?.Invoke(this, primitive);
            return primitive;
        }

        [RemoteOptions(RemotingDirection.None), RemoteMethod]
        public int RemoteMethod_Primitive_PrimitiveParam_None(int primitive) => RoutePrimitiveWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.Bidirectional), RemoteMethod]
        public int RemoteMethod_Primitive_PrimitiveParam_Bidirectional(int primitive) => RoutePrimitiveWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.HostToClient), RemoteMethod]
        public int RemoteMethod_Primitive_PrimitiveParam_HostToClient(int primitive) => RoutePrimitiveWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.ClientToHost), RemoteMethod]
        public int RemoteMethod_Primitive_PrimitiveParam_ClientToHost(int primitive) => RoutePrimitiveWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.Inbound), RemoteMethod]
        public int RemoteMethod_Primitive_PrimitiveParam_Inbound(int primitive) => RoutePrimitiveWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.InboundClient), RemoteMethod]
        public int RemoteMethod_Primitive_PrimitiveParam_InboundClient(int primitive) => RoutePrimitiveWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.InboundHost), RemoteMethod]
        public int RemoteMethod_Primitive_PrimitiveParam_InboundHost(int primitive) => RoutePrimitiveWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.Outbound), RemoteMethod]
        public int RemoteMethod_Primitive_PrimitiveParam_Outbound(int primitive) => RoutePrimitiveWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.OutboundClient), RemoteMethod]
        public int RemoteMethod_Primitive_PrimitiveParam_OutboundClient(int primitive) => RoutePrimitiveWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.OutboundHost), RemoteMethod]
        public int RemoteMethod_Primitive_PrimitiveParam_OutboundHost(int primitive) => RoutePrimitiveWithEventCall(primitive);

        // ===============================================================================

        public Task RouteTaskWithEventCall(int primitive)
        {
            RemoteMethodCalledPrimitive?.Invoke(this, primitive);
            return ReturnedRemoteMethodTask;
        }

        [RemoteOptions(RemotingDirection.None), RemoteMethod]
        public Task RemoteMethod_Task_PrimitiveParam_None(int primitive) => RouteTaskWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.Bidirectional), RemoteMethod]
        public Task RemoteMethod_Task_PrimitiveParam_Bidirectional(int primitive) => RouteTaskWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.HostToClient), RemoteMethod]
        public Task RemoteMethod_Task_PrimitiveParam_HostToClient(int primitive) => RouteTaskWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.ClientToHost), RemoteMethod]
        public Task RemoteMethod_Task_PrimitiveParam_ClientToHost(int primitive) => RouteTaskWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.Inbound), RemoteMethod]
        public Task RemoteMethod_Task_PrimitiveParam_Inbound(int primitive) => RouteTaskWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.InboundClient), RemoteMethod]
        public Task RemoteMethod_Task_PrimitiveParam_InboundClient(int primitive) => RouteTaskWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.InboundHost), RemoteMethod]
        public Task RemoteMethod_Task_PrimitiveParam_InboundHost(int primitive) => RouteTaskWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.Outbound), RemoteMethod]
        public Task RemoteMethod_Task_PrimitiveParam_Outbound(int primitive) => RouteTaskWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.OutboundClient), RemoteMethod]
        public Task RemoteMethod_Task_PrimitiveParam_OutboundClient(int primitive) => RouteTaskWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.OutboundHost), RemoteMethod]
        public Task RemoteMethod_Task_PrimitiveParam_OutboundHost(int primitive) => RouteTaskWithEventCall(primitive);

        // ===============================================================================

        public Task<RemotingDirection> RouteTaskEnumWithEventCall(int primitive)
        {
            RemoteMethodCalledPrimitive?.Invoke(this, primitive);
            return ReturnedRemoteMethodTaskEnum;
        }

        [RemoteOptions(RemotingDirection.None), RemoteMethod]
        public Task<RemotingDirection> RemoteMethod_TaskEnum_PrimitiveParam_None(int primitive) => RouteTaskEnumWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.Bidirectional), RemoteMethod]
        public Task<RemotingDirection> RemoteMethod_TaskEnum_PrimitiveParam_Bidirectional(int primitive) => RouteTaskEnumWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.HostToClient), RemoteMethod]
        public Task<RemotingDirection> RemoteMethod_TaskEnum_PrimitiveParam_HostToClient(int primitive) => RouteTaskEnumWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.ClientToHost), RemoteMethod]
        public Task<RemotingDirection> RemoteMethod_TaskEnum_PrimitiveParam_ClientToHost(int primitive) => RouteTaskEnumWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.Inbound), RemoteMethod]
        public Task<RemotingDirection> RemoteMethod_TaskEnum_PrimitiveParam_Inbound(int primitive) => RouteTaskEnumWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.InboundClient), RemoteMethod]
        public Task<RemotingDirection> RemoteMethod_TaskEnum_PrimitiveParam_InboundClient(int primitive) => RouteTaskEnumWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.InboundHost), RemoteMethod]
        public Task<RemotingDirection> RemoteMethod_TaskEnum_PrimitiveParam_InboundHost(int primitive) => RouteTaskEnumWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.Outbound), RemoteMethod]
        public Task<RemotingDirection> RemoteMethod_TaskEnum_PrimitiveParam_Outbound(int primitive) => RouteTaskEnumWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.OutboundClient), RemoteMethod]
        public Task<RemotingDirection> RemoteMethod_TaskEnum_PrimitiveParam_OutboundClient(int primitive) => RouteTaskEnumWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.OutboundHost), RemoteMethod]
        public Task<RemotingDirection> RemoteMethod_TaskEnum_PrimitiveParam_OutboundHost(int primitive) => RouteTaskEnumWithEventCall(primitive);

        // ===============================================================================

        public Task<DateTime> RouteTaskStructWithEventCall(int primitive)
        {
            RemoteMethodCalledPrimitive?.Invoke(this, primitive);
            return ReturnedRemoteMethodTaskStruct;
        }

        [RemoteOptions(RemotingDirection.None), RemoteMethod]
        public Task<DateTime> RemoteMethod_TaskStruct_PrimitiveParam_None(int primitive) => RouteTaskStructWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.Bidirectional), RemoteMethod]
        public Task<DateTime> RemoteMethod_TaskStruct_PrimitiveParam_Bidirectional(int primitive) => RouteTaskStructWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.HostToClient), RemoteMethod]
        public Task<DateTime> RemoteMethod_TaskStruct_PrimitiveParam_HostToClient(int primitive) => RouteTaskStructWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.ClientToHost), RemoteMethod]
        public Task<DateTime> RemoteMethod_TaskStruct_PrimitiveParam_ClientToHost(int primitive) => RouteTaskStructWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.Inbound), RemoteMethod]
        public Task<DateTime> RemoteMethod_TaskStruct_PrimitiveParam_Inbound(int primitive) => RouteTaskStructWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.InboundClient), RemoteMethod]
        public Task<DateTime> RemoteMethod_TaskStruct_PrimitiveParam_InboundClient(int primitive) => RouteTaskStructWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.InboundHost), RemoteMethod]
        public Task<DateTime> RemoteMethod_TaskStruct_PrimitiveParam_InboundHost(int primitive) => RouteTaskStructWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.Outbound), RemoteMethod]
        public Task<DateTime> RemoteMethod_TaskStruct_PrimitiveParam_Outbound(int primitive) => RouteTaskStructWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.OutboundClient), RemoteMethod]
        public Task<DateTime> RemoteMethod_TaskStruct_PrimitiveParam_OutboundClient(int primitive) => RouteTaskStructWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.OutboundHost), RemoteMethod]
        public Task<DateTime> RemoteMethod_TaskStruct_PrimitiveParam_OutboundHost(int primitive) => RouteTaskStructWithEventCall(primitive);

        // ===============================================================================

        public Task<object> RouteTaskObjectWithEventCall(int primitive)
        {
            RemoteMethodCalledPrimitive?.Invoke(this, primitive);
            return ReturnedRemoteMethodTaskObject;
        }

        [RemoteOptions(RemotingDirection.None), RemoteMethod]
        public Task<object> RemoteMethod_TaskObject_PrimitiveParam_None(int primitive) => RouteTaskObjectWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.Bidirectional), RemoteMethod]
        public Task<object> RemoteMethod_TaskObject_PrimitiveParam_Bidirectional(int primitive) => RouteTaskObjectWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.HostToClient), RemoteMethod]
        public Task<object> RemoteMethod_TaskObject_PrimitiveParam_HostToClient(int primitive) => RouteTaskObjectWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.ClientToHost), RemoteMethod]
        public Task<object> RemoteMethod_TaskObject_PrimitiveParam_ClientToHost(int primitive) => RouteTaskObjectWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.Inbound), RemoteMethod]
        public Task<object> RemoteMethod_TaskObject_PrimitiveParam_Inbound(int primitive) => RouteTaskObjectWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.InboundClient), RemoteMethod]
        public Task<object> RemoteMethod_TaskObject_PrimitiveParam_InboundClient(int primitive) => RouteTaskObjectWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.InboundHost), RemoteMethod]
        public Task<object> RemoteMethod_TaskObject_PrimitiveParam_InboundHost(int primitive) => RouteTaskObjectWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.Outbound), RemoteMethod]
        public Task<object> RemoteMethod_TaskObject_PrimitiveParam_Outbound(int primitive) => RouteTaskObjectWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.OutboundClient), RemoteMethod]
        public Task<object> RemoteMethod_TaskObject_PrimitiveParam_OutboundClient(int primitive) => RouteTaskObjectWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.OutboundHost), RemoteMethod]
        public Task<object> RemoteMethod_TaskObject_PrimitiveParam_OutboundHost(int primitive) => RouteTaskObjectWithEventCall(primitive);

        // ===============================================================================

        public Task<int> RouteTaskPrimitiveWithEventCall(int primitive)
        {
            RemoteMethodCalledPrimitive?.Invoke(this, primitive);
            return Task.FromResult(primitive);
        }

        [RemoteOptions(RemotingDirection.None), RemoteMethod]
        public Task<int> RemoteMethod_TaskPrimitive_PrimitiveParam_None(int primitive) => RouteTaskPrimitiveWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.Bidirectional), RemoteMethod]
        public Task<int> RemoteMethod_TaskPrimitive_PrimitiveParam_Bidirectional(int primitive) => RouteTaskPrimitiveWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.HostToClient), RemoteMethod]
        public Task<int> RemoteMethod_TaskPrimitive_PrimitiveParam_HostToClient(int primitive) => RouteTaskPrimitiveWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.ClientToHost), RemoteMethod]
        public Task<int> RemoteMethod_TaskPrimitive_PrimitiveParam_ClientToHost(int primitive) => RouteTaskPrimitiveWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.Inbound), RemoteMethod]
        public Task<int> RemoteMethod_TaskPrimitive_PrimitiveParam_Inbound(int primitive) => RouteTaskPrimitiveWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.InboundClient), RemoteMethod]
        public Task<int> RemoteMethod_TaskPrimitive_PrimitiveParam_InboundClient(int primitive) => RouteTaskPrimitiveWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.InboundHost), RemoteMethod]
        public Task<int> RemoteMethod_TaskPrimitive_PrimitiveParam_InboundHost(int primitive) => RouteTaskPrimitiveWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.Outbound), RemoteMethod]
        public Task<int> RemoteMethod_TaskPrimitive_PrimitiveParam_Outbound(int primitive) => RouteTaskPrimitiveWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.OutboundClient), RemoteMethod]
        public Task<int> RemoteMethod_TaskPrimitive_PrimitiveParam_OutboundClient(int primitive) => RouteTaskPrimitiveWithEventCall(primitive);

        [RemoteOptions(RemotingDirection.OutboundHost), RemoteMethod]
        public Task<int> RemoteMethod_TaskPrimitive_PrimitiveParam_OutboundHost(int primitive) => RouteTaskPrimitiveWithEventCall(primitive);
    }
}
