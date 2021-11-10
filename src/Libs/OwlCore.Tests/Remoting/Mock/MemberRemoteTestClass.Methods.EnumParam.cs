using OwlCore.Remoting;
using System;
using System.Threading.Tasks;

namespace OwlCore.Tests.Remoting.Mock
{
    public partial class MemberRemoteTestClass
    {
        [RemoteOptions(RemotingDirection.None), RemoteMethod]
        public void RemoteMethod_Void_EnumParam_None(RemotingDirection direction) => RemoteMethodCalledEnum?.Invoke(this, direction);

        [RemoteOptions(RemotingDirection.Bidirectional), RemoteMethod]
        public void RemoteMethod_Void_EnumParam_Bidirectional(RemotingDirection direction) => RemoteMethodCalledEnum?.Invoke(this, direction);

        [RemoteOptions(RemotingDirection.HostToClient), RemoteMethod]
        public void RemoteMethod_Void_EnumParam_HostToClient(RemotingDirection direction) => RemoteMethodCalledEnum?.Invoke(this, direction);

        [RemoteOptions(RemotingDirection.ClientToHost), RemoteMethod]
        public void RemoteMethod_Void_EnumParam_ClientToHost(RemotingDirection direction) => RemoteMethodCalledEnum?.Invoke(this, direction);

        [RemoteOptions(RemotingDirection.Inbound), RemoteMethod]
        public void RemoteMethod_Void_EnumParam_Inbound(RemotingDirection direction) => RemoteMethodCalledEnum?.Invoke(this, direction);

        [RemoteOptions(RemotingDirection.InboundClient), RemoteMethod]
        public void RemoteMethod_Void_EnumParam_InboundClient(RemotingDirection direction) => RemoteMethodCalledEnum?.Invoke(this, direction);

        [RemoteOptions(RemotingDirection.InboundHost), RemoteMethod]
        public void RemoteMethod_Void_EnumParam_InboundHost(RemotingDirection direction) => RemoteMethodCalledEnum?.Invoke(this, direction);

        [RemoteOptions(RemotingDirection.Outbound), RemoteMethod]
        public void RemoteMethod_Void_EnumParam_Outbound(RemotingDirection direction) => RemoteMethodCalledEnum?.Invoke(this, direction);

        [RemoteOptions(RemotingDirection.OutboundClient), RemoteMethod]
        public void RemoteMethod_Void_EnumParam_OutboundClient(RemotingDirection direction) => RemoteMethodCalledEnum?.Invoke(this, direction);

        [RemoteOptions(RemotingDirection.OutboundHost), RemoteMethod]
        public void RemoteMethod_Void_EnumParam_OutboundHost(RemotingDirection direction) => RemoteMethodCalledEnum?.Invoke(this, direction);

        // ===============================================================================

        public RemotingDirection RouteEnumWithEventCall(RemotingDirection direction)
        {
            RemoteMethodCalledEnum?.Invoke(this, direction);
            return direction;
        }

        [RemoteOptions(RemotingDirection.None), RemoteMethod]
        public RemotingDirection RemoteMethod_Enum_EnumParam_None(RemotingDirection direction) => RouteEnumWithEventCall(direction);

        [RemoteOptions(RemotingDirection.Bidirectional), RemoteMethod]
        public RemotingDirection RemoteMethod_Enum_EnumParam_Bidirectional(RemotingDirection direction) => RouteEnumWithEventCall(direction);

        [RemoteOptions(RemotingDirection.HostToClient), RemoteMethod]
        public RemotingDirection RemoteMethod_Enum_EnumParam_HostToClient(RemotingDirection direction) => RouteEnumWithEventCall(direction);

        [RemoteOptions(RemotingDirection.ClientToHost), RemoteMethod]
        public RemotingDirection RemoteMethod_Enum_EnumParam_ClientToHost(RemotingDirection direction) => RouteEnumWithEventCall(direction);

        [RemoteOptions(RemotingDirection.Inbound), RemoteMethod]
        public RemotingDirection RemoteMethod_Enum_EnumParam_Inbound(RemotingDirection direction) => RouteEnumWithEventCall(direction);

        [RemoteOptions(RemotingDirection.InboundClient), RemoteMethod]
        public RemotingDirection RemoteMethod_Enum_EnumParam_InboundClient(RemotingDirection direction) => RouteEnumWithEventCall(direction);

        [RemoteOptions(RemotingDirection.InboundHost), RemoteMethod]
        public RemotingDirection RemoteMethod_Enum_EnumParam_InboundHost(RemotingDirection direction) => RouteEnumWithEventCall(direction);

        [RemoteOptions(RemotingDirection.Outbound), RemoteMethod]
        public RemotingDirection RemoteMethod_Enum_EnumParam_Outbound(RemotingDirection direction) => RouteEnumWithEventCall(direction);

        [RemoteOptions(RemotingDirection.OutboundClient), RemoteMethod]
        public RemotingDirection RemoteMethod_Enum_EnumParam_OutboundClient(RemotingDirection direction) => RouteEnumWithEventCall(direction);

        [RemoteOptions(RemotingDirection.OutboundHost), RemoteMethod]
        public RemotingDirection RemoteMethod_Enum_EnumParam_OutboundHost(RemotingDirection direction) => RouteEnumWithEventCall(direction);

        // ===============================================================================

        public DateTime RouteDateTimeWithEventCall(RemotingDirection direction)
        {
            RemoteMethodCalledEnum?.Invoke(this, direction);
            return ReturnedRemoteMethodStruct;
        }

        [RemoteOptions(RemotingDirection.None), RemoteMethod]
        public DateTime RemoteMethod_Struct_EnumParam_None(RemotingDirection direction) => RouteDateTimeWithEventCall(direction);

        [RemoteOptions(RemotingDirection.Bidirectional), RemoteMethod]
        public DateTime RemoteMethod_Struct_EnumParam_Bidirectional(RemotingDirection direction) => RouteDateTimeWithEventCall(direction);

        [RemoteOptions(RemotingDirection.HostToClient), RemoteMethod]
        public DateTime RemoteMethod_Struct_EnumParam_HostToClient(RemotingDirection direction) => RouteDateTimeWithEventCall(direction);

        [RemoteOptions(RemotingDirection.ClientToHost), RemoteMethod]
        public DateTime RemoteMethod_Struct_EnumParam_ClientToHost(RemotingDirection direction) => RouteDateTimeWithEventCall(direction);

        [RemoteOptions(RemotingDirection.Inbound), RemoteMethod]
        public DateTime RemoteMethod_Struct_EnumParam_Inbound(RemotingDirection direction) => RouteDateTimeWithEventCall(direction);

        [RemoteOptions(RemotingDirection.InboundClient), RemoteMethod]
        public DateTime RemoteMethod_Struct_EnumParam_InboundClient(RemotingDirection direction) => RouteDateTimeWithEventCall(direction);

        [RemoteOptions(RemotingDirection.InboundHost), RemoteMethod]
        public DateTime RemoteMethod_Struct_EnumParam_InboundHost(RemotingDirection direction) => RouteDateTimeWithEventCall(direction);

        [RemoteOptions(RemotingDirection.Outbound), RemoteMethod]
        public DateTime RemoteMethod_Struct_EnumParam_Outbound(RemotingDirection direction) => RouteDateTimeWithEventCall(direction);

        [RemoteOptions(RemotingDirection.OutboundClient), RemoteMethod]
        public DateTime RemoteMethod_Struct_EnumParam_OutboundClient(RemotingDirection direction) => RouteDateTimeWithEventCall(direction);

        [RemoteOptions(RemotingDirection.OutboundHost), RemoteMethod]
        public DateTime RemoteMethod_Struct_EnumParam_OutboundHost(RemotingDirection direction) => RouteDateTimeWithEventCall(direction);

        // ===============================================================================

        public object RouteObjectWithEventCall(RemotingDirection direction)
        {
            RemoteMethodCalledEnum?.Invoke(this, direction);
            return ReturnedRemoteMethodObject;
        }

        [RemoteOptions(RemotingDirection.None), RemoteMethod]
        public object RemoteMethod_Object_EnumParam_None(RemotingDirection direction) => RouteObjectWithEventCall(direction);

        [RemoteOptions(RemotingDirection.Bidirectional), RemoteMethod]
        public object RemoteMethod_Object_EnumParam_Bidirectional(RemotingDirection direction) => RouteObjectWithEventCall(direction);

        [RemoteOptions(RemotingDirection.HostToClient), RemoteMethod]
        public object RemoteMethod_Object_EnumParam_HostToClient(RemotingDirection direction) => RouteObjectWithEventCall(direction);

        [RemoteOptions(RemotingDirection.ClientToHost), RemoteMethod]
        public object RemoteMethod_Object_EnumParam_ClientToHost(RemotingDirection direction) => RouteObjectWithEventCall(direction);

        [RemoteOptions(RemotingDirection.Inbound), RemoteMethod]
        public object RemoteMethod_Object_EnumParam_Inbound(RemotingDirection direction) => RouteObjectWithEventCall(direction);

        [RemoteOptions(RemotingDirection.InboundClient), RemoteMethod]
        public object RemoteMethod_Object_EnumParam_InboundClient(RemotingDirection direction) => RouteObjectWithEventCall(direction);

        [RemoteOptions(RemotingDirection.InboundHost), RemoteMethod]
        public object RemoteMethod_Object_EnumParam_InboundHost(RemotingDirection direction) => RouteObjectWithEventCall(direction);

        [RemoteOptions(RemotingDirection.Outbound), RemoteMethod]
        public object RemoteMethod_Object_EnumParam_Outbound(RemotingDirection direction) => RouteObjectWithEventCall(direction);

        [RemoteOptions(RemotingDirection.OutboundClient), RemoteMethod]
        public object RemoteMethod_Object_EnumParam_OutboundClient(RemotingDirection direction) => RouteObjectWithEventCall(direction);

        [RemoteOptions(RemotingDirection.OutboundHost), RemoteMethod]
        public object RemoteMethod_Object_EnumParam_OutboundHost(RemotingDirection direction) => RouteObjectWithEventCall(direction);

        // ===============================================================================

        public int RoutePrimitiveWithEventCall(RemotingDirection direction)
        {
            RemoteMethodCalledEnum?.Invoke(this, direction);
            return ReturnedRemoteMethodPrimitive;
        }

        [RemoteOptions(RemotingDirection.None), RemoteMethod]
        public int RemoteMethod_Primitive_EnumParam_None(RemotingDirection direction) => RoutePrimitiveWithEventCall(direction);

        [RemoteOptions(RemotingDirection.Bidirectional), RemoteMethod]
        public int RemoteMethod_Primitive_EnumParam_Bidirectional(RemotingDirection direction) => RoutePrimitiveWithEventCall(direction);

        [RemoteOptions(RemotingDirection.HostToClient), RemoteMethod]
        public int RemoteMethod_Primitive_EnumParam_HostToClient(RemotingDirection direction) => RoutePrimitiveWithEventCall(direction);

        [RemoteOptions(RemotingDirection.ClientToHost), RemoteMethod]
        public int RemoteMethod_Primitive_EnumParam_ClientToHost(RemotingDirection direction) => RoutePrimitiveWithEventCall(direction);

        [RemoteOptions(RemotingDirection.Inbound), RemoteMethod]
        public int RemoteMethod_Primitive_EnumParam_Inbound(RemotingDirection direction) => RoutePrimitiveWithEventCall(direction);

        [RemoteOptions(RemotingDirection.InboundClient), RemoteMethod]
        public int RemoteMethod_Primitive_EnumParam_InboundClient(RemotingDirection direction) => RoutePrimitiveWithEventCall(direction);

        [RemoteOptions(RemotingDirection.InboundHost), RemoteMethod]
        public int RemoteMethod_Primitive_EnumParam_InboundHost(RemotingDirection direction) => RoutePrimitiveWithEventCall(direction);

        [RemoteOptions(RemotingDirection.Outbound), RemoteMethod]
        public int RemoteMethod_Primitive_EnumParam_Outbound(RemotingDirection direction) => RoutePrimitiveWithEventCall(direction);

        [RemoteOptions(RemotingDirection.OutboundClient), RemoteMethod]
        public int RemoteMethod_Primitive_EnumParam_OutboundClient(RemotingDirection direction) => RoutePrimitiveWithEventCall(direction);

        [RemoteOptions(RemotingDirection.OutboundHost), RemoteMethod]
        public int RemoteMethod_Primitive_EnumParam_OutboundHost(RemotingDirection direction) => RoutePrimitiveWithEventCall(direction);

        // ===============================================================================

        public Task RouteTaskWithEventCall(RemotingDirection direction)
        {
            RemoteMethodCalledEnum?.Invoke(this, direction);
            return ReturnedRemoteMethodTask;
        }

        [RemoteOptions(RemotingDirection.None), RemoteMethod]
        public Task RemoteMethod_Task_EnumParam_None(RemotingDirection direction) => RouteTaskWithEventCall(direction);

        [RemoteOptions(RemotingDirection.Bidirectional), RemoteMethod]
        public Task RemoteMethod_Task_EnumParam_Bidirectional(RemotingDirection direction) => RouteTaskWithEventCall(direction);

        [RemoteOptions(RemotingDirection.HostToClient), RemoteMethod]
        public Task RemoteMethod_Task_EnumParam_HostToClient(RemotingDirection direction) => RouteTaskWithEventCall(direction);

        [RemoteOptions(RemotingDirection.ClientToHost), RemoteMethod]
        public Task RemoteMethod_Task_EnumParam_ClientToHost(RemotingDirection direction) => RouteTaskWithEventCall(direction);

        [RemoteOptions(RemotingDirection.Inbound), RemoteMethod]
        public Task RemoteMethod_Task_EnumParam_Inbound(RemotingDirection direction) => RouteTaskWithEventCall(direction);

        [RemoteOptions(RemotingDirection.InboundClient), RemoteMethod]
        public Task RemoteMethod_Task_EnumParam_InboundClient(RemotingDirection direction) => RouteTaskWithEventCall(direction);

        [RemoteOptions(RemotingDirection.InboundHost), RemoteMethod]
        public Task RemoteMethod_Task_EnumParam_InboundHost(RemotingDirection direction) => RouteTaskWithEventCall(direction);

        [RemoteOptions(RemotingDirection.Outbound), RemoteMethod]
        public Task RemoteMethod_Task_EnumParam_Outbound(RemotingDirection direction) => RouteTaskWithEventCall(direction);

        [RemoteOptions(RemotingDirection.OutboundClient), RemoteMethod]
        public Task RemoteMethod_Task_EnumParam_OutboundClient(RemotingDirection direction) => RouteTaskWithEventCall(direction);

        [RemoteOptions(RemotingDirection.OutboundHost), RemoteMethod]
        public Task RemoteMethod_Task_EnumParam_OutboundHost(RemotingDirection direction) => RouteTaskWithEventCall(direction);

        // ===============================================================================

        public Task<RemotingDirection> RouteTaskEnumWithEventCall(RemotingDirection direction)
        {
            RemoteMethodCalledEnum?.Invoke(this, direction);
            return Task.FromResult(direction);
        }

        [RemoteOptions(RemotingDirection.None), RemoteMethod]
        public Task<RemotingDirection> RemoteMethod_TaskEnum_EnumParam_None(RemotingDirection direction) => RouteTaskEnumWithEventCall(direction);

        [RemoteOptions(RemotingDirection.Bidirectional), RemoteMethod]
        public Task<RemotingDirection> RemoteMethod_TaskEnum_EnumParam_Bidirectional(RemotingDirection direction) => RouteTaskEnumWithEventCall(direction);

        [RemoteOptions(RemotingDirection.HostToClient), RemoteMethod]
        public Task<RemotingDirection> RemoteMethod_TaskEnum_EnumParam_HostToClient(RemotingDirection direction) => RouteTaskEnumWithEventCall(direction);

        [RemoteOptions(RemotingDirection.ClientToHost), RemoteMethod]
        public Task<RemotingDirection> RemoteMethod_TaskEnum_EnumParam_ClientToHost(RemotingDirection direction) => RouteTaskEnumWithEventCall(direction);

        [RemoteOptions(RemotingDirection.Inbound), RemoteMethod]
        public Task<RemotingDirection> RemoteMethod_TaskEnum_EnumParam_Inbound(RemotingDirection direction) => RouteTaskEnumWithEventCall(direction);

        [RemoteOptions(RemotingDirection.InboundClient), RemoteMethod]
        public Task<RemotingDirection> RemoteMethod_TaskEnum_EnumParam_InboundClient(RemotingDirection direction) => RouteTaskEnumWithEventCall(direction);

        [RemoteOptions(RemotingDirection.InboundHost), RemoteMethod]
        public Task<RemotingDirection> RemoteMethod_TaskEnum_EnumParam_InboundHost(RemotingDirection direction) => RouteTaskEnumWithEventCall(direction);

        [RemoteOptions(RemotingDirection.Outbound), RemoteMethod]
        public Task<RemotingDirection> RemoteMethod_TaskEnum_EnumParam_Outbound(RemotingDirection direction) => RouteTaskEnumWithEventCall(direction);

        [RemoteOptions(RemotingDirection.OutboundClient), RemoteMethod]
        public Task<RemotingDirection> RemoteMethod_TaskEnum_EnumParam_OutboundClient(RemotingDirection direction) => RouteTaskEnumWithEventCall(direction);

        [RemoteOptions(RemotingDirection.OutboundHost), RemoteMethod]
        public Task<RemotingDirection> RemoteMethod_TaskEnum_EnumParam_OutboundHost(RemotingDirection direction) => RouteTaskEnumWithEventCall(direction);

        // ===============================================================================

        public Task<DateTime> RouteTaskStructWithEventCall(RemotingDirection direction)
        {
            RemoteMethodCalledEnum?.Invoke(this, direction);
            return ReturnedRemoteMethodTaskStruct;
        }

        [RemoteOptions(RemotingDirection.None), RemoteMethod]
        public Task<DateTime> RemoteMethod_TaskStruct_EnumParam_None(RemotingDirection direction) => RouteTaskStructWithEventCall(direction);

        [RemoteOptions(RemotingDirection.Bidirectional), RemoteMethod]
        public Task<DateTime> RemoteMethod_TaskStruct_EnumParam_Bidirectional(RemotingDirection direction) => RouteTaskStructWithEventCall(direction);

        [RemoteOptions(RemotingDirection.HostToClient), RemoteMethod]
        public Task<DateTime> RemoteMethod_TaskStruct_EnumParam_HostToClient(RemotingDirection direction) => RouteTaskStructWithEventCall(direction);

        [RemoteOptions(RemotingDirection.ClientToHost), RemoteMethod]
        public Task<DateTime> RemoteMethod_TaskStruct_EnumParam_ClientToHost(RemotingDirection direction) => RouteTaskStructWithEventCall(direction);

        [RemoteOptions(RemotingDirection.Inbound), RemoteMethod]
        public Task<DateTime> RemoteMethod_TaskStruct_EnumParam_Inbound(RemotingDirection direction) => RouteTaskStructWithEventCall(direction);

        [RemoteOptions(RemotingDirection.InboundClient), RemoteMethod]
        public Task<DateTime> RemoteMethod_TaskStruct_EnumParam_InboundClient(RemotingDirection direction) => RouteTaskStructWithEventCall(direction);

        [RemoteOptions(RemotingDirection.InboundHost), RemoteMethod]
        public Task<DateTime> RemoteMethod_TaskStruct_EnumParam_InboundHost(RemotingDirection direction) => RouteTaskStructWithEventCall(direction);

        [RemoteOptions(RemotingDirection.Outbound), RemoteMethod]
        public Task<DateTime> RemoteMethod_TaskStruct_EnumParam_Outbound(RemotingDirection direction) => RouteTaskStructWithEventCall(direction);

        [RemoteOptions(RemotingDirection.OutboundClient), RemoteMethod]
        public Task<DateTime> RemoteMethod_TaskStruct_EnumParam_OutboundClient(RemotingDirection direction) => RouteTaskStructWithEventCall(direction);

        [RemoteOptions(RemotingDirection.OutboundHost), RemoteMethod]
        public Task<DateTime> RemoteMethod_TaskStruct_EnumParam_OutboundHost(RemotingDirection direction) => RouteTaskStructWithEventCall(direction);

        // ===============================================================================

        public Task<object> RouteTaskObjectWithEventCall(RemotingDirection direction)
        {
            RemoteMethodCalledEnum?.Invoke(this, direction);
            return ReturnedRemoteMethodTaskObject;
        }

        [RemoteOptions(RemotingDirection.None), RemoteMethod]
        public Task<object> RemoteMethod_TaskObject_EnumParam_None(RemotingDirection direction) => RouteTaskObjectWithEventCall(direction);

        [RemoteOptions(RemotingDirection.Bidirectional), RemoteMethod]
        public Task<object> RemoteMethod_TaskObject_EnumParam_Bidirectional(RemotingDirection direction) => RouteTaskObjectWithEventCall(direction);

        [RemoteOptions(RemotingDirection.HostToClient), RemoteMethod]
        public Task<object> RemoteMethod_TaskObject_EnumParam_HostToClient(RemotingDirection direction) => RouteTaskObjectWithEventCall(direction);

        [RemoteOptions(RemotingDirection.ClientToHost), RemoteMethod]
        public Task<object> RemoteMethod_TaskObject_EnumParam_ClientToHost(RemotingDirection direction) => RouteTaskObjectWithEventCall(direction);

        [RemoteOptions(RemotingDirection.Inbound), RemoteMethod]
        public Task<object> RemoteMethod_TaskObject_EnumParam_Inbound(RemotingDirection direction) => RouteTaskObjectWithEventCall(direction);

        [RemoteOptions(RemotingDirection.InboundClient), RemoteMethod]
        public Task<object> RemoteMethod_TaskObject_EnumParam_InboundClient(RemotingDirection direction) => RouteTaskObjectWithEventCall(direction);

        [RemoteOptions(RemotingDirection.InboundHost), RemoteMethod]
        public Task<object> RemoteMethod_TaskObject_EnumParam_InboundHost(RemotingDirection direction) => RouteTaskObjectWithEventCall(direction);

        [RemoteOptions(RemotingDirection.Outbound), RemoteMethod]
        public Task<object> RemoteMethod_TaskObject_EnumParam_Outbound(RemotingDirection direction) => RouteTaskObjectWithEventCall(direction);

        [RemoteOptions(RemotingDirection.OutboundClient), RemoteMethod]
        public Task<object> RemoteMethod_TaskObject_EnumParam_OutboundClient(RemotingDirection direction) => RouteTaskObjectWithEventCall(direction);

        [RemoteOptions(RemotingDirection.OutboundHost), RemoteMethod]
        public Task<object> RemoteMethod_TaskObject_EnumParam_OutboundHost(RemotingDirection direction) => RouteTaskObjectWithEventCall(direction);

        // ===============================================================================

        public Task<int> RouteTaskPrimitiveWithEventCall(RemotingDirection direction)
        {
            RemoteMethodCalledEnum?.Invoke(this, direction);
            return ReturnedRemoteMethodTaskPrimitive;
        }

        [RemoteOptions(RemotingDirection.None), RemoteMethod]
        public Task<int> RemoteMethod_TaskPrimitive_EnumParam_None(RemotingDirection direction) => RouteTaskPrimitiveWithEventCall(direction);

        [RemoteOptions(RemotingDirection.Bidirectional), RemoteMethod]
        public Task<int> RemoteMethod_TaskPrimitive_EnumParam_Bidirectional(RemotingDirection direction) => RouteTaskPrimitiveWithEventCall(direction);

        [RemoteOptions(RemotingDirection.HostToClient), RemoteMethod]
        public Task<int> RemoteMethod_TaskPrimitive_EnumParam_HostToClient(RemotingDirection direction) => RouteTaskPrimitiveWithEventCall(direction);

        [RemoteOptions(RemotingDirection.ClientToHost), RemoteMethod]
        public Task<int> RemoteMethod_TaskPrimitive_EnumParam_ClientToHost(RemotingDirection direction) => RouteTaskPrimitiveWithEventCall(direction);

        [RemoteOptions(RemotingDirection.Inbound), RemoteMethod]
        public Task<int> RemoteMethod_TaskPrimitive_EnumParam_Inbound(RemotingDirection direction) => RouteTaskPrimitiveWithEventCall(direction);

        [RemoteOptions(RemotingDirection.InboundClient), RemoteMethod]
        public Task<int> RemoteMethod_TaskPrimitive_EnumParam_InboundClient(RemotingDirection direction) => RouteTaskPrimitiveWithEventCall(direction);

        [RemoteOptions(RemotingDirection.InboundHost), RemoteMethod]
        public Task<int> RemoteMethod_TaskPrimitive_EnumParam_InboundHost(RemotingDirection direction) => RouteTaskPrimitiveWithEventCall(direction);

        [RemoteOptions(RemotingDirection.Outbound), RemoteMethod]
        public Task<int> RemoteMethod_TaskPrimitive_EnumParam_Outbound(RemotingDirection direction) => RouteTaskPrimitiveWithEventCall(direction);

        [RemoteOptions(RemotingDirection.OutboundClient), RemoteMethod]
        public Task<int> RemoteMethod_TaskPrimitive_EnumParam_OutboundClient(RemotingDirection direction) => RouteTaskPrimitiveWithEventCall(direction);

        [RemoteOptions(RemotingDirection.OutboundHost), RemoteMethod]
        public Task<int> RemoteMethod_TaskPrimitive_EnumParam_OutboundHost(RemotingDirection direction) => RouteTaskPrimitiveWithEventCall(direction);
    }
}
