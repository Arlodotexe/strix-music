using OwlCore.Remoting;
using System;
using System.Threading.Tasks;

namespace OwlCore.Tests.Remoting.Mock
{
    public partial class MemberRemoteTestClass
    {
        [RemoteOptions(RemotingDirection.None), RemoteMethod]
        public void RemoteMethod_Void_StructParam_None(DateTime dateTime) => RemoteMethodCalledStruct?.Invoke(this, dateTime);

        [RemoteOptions(RemotingDirection.Bidirectional), RemoteMethod]
        public void RemoteMethod_Void_StructParam_Bidirectional(DateTime dateTime) => RemoteMethodCalledStruct?.Invoke(this, dateTime);

        [RemoteOptions(RemotingDirection.HostToClient), RemoteMethod]
        public void RemoteMethod_Void_StructParam_HostToClient(DateTime dateTime) => RemoteMethodCalledStruct?.Invoke(this, dateTime);

        [RemoteOptions(RemotingDirection.ClientToHost), RemoteMethod]
        public void RemoteMethod_Void_StructParam_ClientToHost(DateTime dateTime) => RemoteMethodCalledStruct?.Invoke(this, dateTime);

        [RemoteOptions(RemotingDirection.Inbound), RemoteMethod]
        public void RemoteMethod_Void_StructParam_Inbound(DateTime dateTime) => RemoteMethodCalledStruct?.Invoke(this, dateTime);

        [RemoteOptions(RemotingDirection.InboundClient), RemoteMethod]
        public void RemoteMethod_Void_StructParam_InboundClient(DateTime dateTime) => RemoteMethodCalledStruct?.Invoke(this, dateTime);

        [RemoteOptions(RemotingDirection.InboundHost), RemoteMethod]
        public void RemoteMethod_Void_StructParam_InboundHost(DateTime dateTime) => RemoteMethodCalledStruct?.Invoke(this, dateTime);

        [RemoteOptions(RemotingDirection.Outbound), RemoteMethod]
        public void RemoteMethod_Void_StructParam_Outbound(DateTime dateTime) => RemoteMethodCalledStruct?.Invoke(this, dateTime);

        [RemoteOptions(RemotingDirection.OutboundClient), RemoteMethod]
        public void RemoteMethod_Void_StructParam_OutboundClient(DateTime dateTime) => RemoteMethodCalledStruct?.Invoke(this, dateTime);

        [RemoteOptions(RemotingDirection.OutboundHost), RemoteMethod]
        public void RemoteMethod_Void_StructParam_OutboundHost(DateTime dateTime) => RemoteMethodCalledStruct?.Invoke(this, dateTime);

        // ===============================================================================

        public RemotingDirection RouteEnumWithEventCall(DateTime dateTime)
        {
            RemoteMethodCalledStruct?.Invoke(this, dateTime);
            return ReturnedRemoteMethodEnum;
        }

        [RemoteOptions(RemotingDirection.None), RemoteMethod]
        public RemotingDirection RemoteMethod_Enum_StructParam_None(DateTime dateTime) => RouteEnumWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.Bidirectional), RemoteMethod]
        public RemotingDirection RemoteMethod_Enum_StructParam_Bidirectional(DateTime dateTime) => RouteEnumWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.HostToClient), RemoteMethod]
        public RemotingDirection RemoteMethod_Enum_StructParam_HostToClient(DateTime dateTime) => RouteEnumWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.ClientToHost), RemoteMethod]
        public RemotingDirection RemoteMethod_Enum_StructParam_ClientToHost(DateTime dateTime) => RouteEnumWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.Inbound), RemoteMethod]
        public RemotingDirection RemoteMethod_Enum_StructParam_Inbound(DateTime dateTime) => RouteEnumWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.InboundClient), RemoteMethod]
        public RemotingDirection RemoteMethod_Enum_StructParam_InboundClient(DateTime dateTime) => RouteEnumWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.InboundHost), RemoteMethod]
        public RemotingDirection RemoteMethod_Enum_StructParam_InboundHost(DateTime dateTime) => RouteEnumWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.Outbound), RemoteMethod]
        public RemotingDirection RemoteMethod_Enum_StructParam_Outbound(DateTime dateTime) => RouteEnumWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.OutboundClient), RemoteMethod]
        public RemotingDirection RemoteMethod_Enum_StructParam_OutboundClient(DateTime dateTime) => RouteEnumWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.OutboundHost), RemoteMethod]
        public RemotingDirection RemoteMethod_Enum_StructParam_OutboundHost(DateTime dateTime) => RouteEnumWithEventCall(dateTime);

        // ===============================================================================

        public DateTime RouteDateTimeWithEventCall(DateTime dateTime)
        {
            RemoteMethodCalledStruct?.Invoke(this, dateTime);
            return dateTime;
        }

        [RemoteOptions(RemotingDirection.None), RemoteMethod]
        public DateTime RemoteMethod_Struct_StructParam_None(DateTime dateTime) => RouteDateTimeWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.Bidirectional), RemoteMethod]
        public DateTime RemoteMethod_Struct_StructParam_Bidirectional(DateTime dateTime) => RouteDateTimeWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.HostToClient), RemoteMethod]
        public DateTime RemoteMethod_Struct_StructParam_HostToClient(DateTime dateTime) => RouteDateTimeWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.ClientToHost), RemoteMethod]
        public DateTime RemoteMethod_Struct_StructParam_ClientToHost(DateTime dateTime) => RouteDateTimeWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.Inbound), RemoteMethod]
        public DateTime RemoteMethod_Struct_StructParam_Inbound(DateTime dateTime) => RouteDateTimeWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.InboundClient), RemoteMethod]
        public DateTime RemoteMethod_Struct_StructParam_InboundClient(DateTime dateTime) => RouteDateTimeWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.InboundHost), RemoteMethod]
        public DateTime RemoteMethod_Struct_StructParam_InboundHost(DateTime dateTime) => RouteDateTimeWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.Outbound), RemoteMethod]
        public DateTime RemoteMethod_Struct_StructParam_Outbound(DateTime dateTime) => RouteDateTimeWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.OutboundClient), RemoteMethod]
        public DateTime RemoteMethod_Struct_StructParam_OutboundClient(DateTime dateTime) => RouteDateTimeWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.OutboundHost), RemoteMethod]
        public DateTime RemoteMethod_Struct_StructParam_OutboundHost(DateTime dateTime) => RouteDateTimeWithEventCall(dateTime);

        // ===============================================================================

        public object RouteObjectWithEventCall(DateTime dateTime)
        {
            RemoteMethodCalledStruct?.Invoke(this, dateTime);
            return ReturnedRemoteMethodObject;
        }

        [RemoteOptions(RemotingDirection.None), RemoteMethod]
        public object RemoteMethod_Object_StructParam_None(DateTime dateTime) => RouteObjectWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.Bidirectional), RemoteMethod]
        public object RemoteMethod_Object_StructParam_Bidirectional(DateTime dateTime) => RouteObjectWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.HostToClient), RemoteMethod]
        public object RemoteMethod_Object_StructParam_HostToClient(DateTime dateTime) => RouteObjectWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.ClientToHost), RemoteMethod]
        public object RemoteMethod_Object_StructParam_ClientToHost(DateTime dateTime) => RouteObjectWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.Inbound), RemoteMethod]
        public object RemoteMethod_Object_StructParam_Inbound(DateTime dateTime) => RouteObjectWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.InboundClient), RemoteMethod]
        public object RemoteMethod_Object_StructParam_InboundClient(DateTime dateTime) => RouteObjectWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.InboundHost), RemoteMethod]
        public object RemoteMethod_Object_StructParam_InboundHost(DateTime dateTime) => RouteObjectWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.Outbound), RemoteMethod]
        public object RemoteMethod_Object_StructParam_Outbound(DateTime dateTime) => RouteObjectWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.OutboundClient), RemoteMethod]
        public object RemoteMethod_Object_StructParam_OutboundClient(DateTime dateTime) => RouteObjectWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.OutboundHost), RemoteMethod]
        public object RemoteMethod_Object_StructParam_OutboundHost(DateTime dateTime) => RouteObjectWithEventCall(dateTime);

        // ===============================================================================

        public int RoutePrimitiveWithEventCall(DateTime dateTime)
        {
            RemoteMethodCalledStruct?.Invoke(this, dateTime);
            return ReturnedRemoteMethodPrimitive;
        }

        [RemoteOptions(RemotingDirection.None), RemoteMethod]
        public int RemoteMethod_Primitive_StructParam_None(DateTime dateTime) => RoutePrimitiveWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.Bidirectional), RemoteMethod]
        public int RemoteMethod_Primitive_StructParam_Bidirectional(DateTime dateTime) => RoutePrimitiveWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.HostToClient), RemoteMethod]
        public int RemoteMethod_Primitive_StructParam_HostToClient(DateTime dateTime) => RoutePrimitiveWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.ClientToHost), RemoteMethod]
        public int RemoteMethod_Primitive_StructParam_ClientToHost(DateTime dateTime) => RoutePrimitiveWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.Inbound), RemoteMethod]
        public int RemoteMethod_Primitive_StructParam_Inbound(DateTime dateTime) => RoutePrimitiveWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.InboundClient), RemoteMethod]
        public int RemoteMethod_Primitive_StructParam_InboundClient(DateTime dateTime) => RoutePrimitiveWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.InboundHost), RemoteMethod]
        public int RemoteMethod_Primitive_StructParam_InboundHost(DateTime dateTime) => RoutePrimitiveWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.Outbound), RemoteMethod]
        public int RemoteMethod_Primitive_StructParam_Outbound(DateTime dateTime) => RoutePrimitiveWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.OutboundClient), RemoteMethod]
        public int RemoteMethod_Primitive_StructParam_OutboundClient(DateTime dateTime) => RoutePrimitiveWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.OutboundHost), RemoteMethod]
        public int RemoteMethod_Primitive_StructParam_OutboundHost(DateTime dateTime) => RoutePrimitiveWithEventCall(dateTime);

        // ===============================================================================

        public Task RouteTaskWithEventCall(DateTime dateTime)
        {
            RemoteMethodCalledStruct?.Invoke(this, dateTime);
            return ReturnedRemoteMethodTask;
        }

        [RemoteOptions(RemotingDirection.None), RemoteMethod]
        public Task RemoteMethod_Task_StructParam_None(DateTime dateTime) => RouteTaskWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.Bidirectional), RemoteMethod]
        public Task RemoteMethod_Task_StructParam_Bidirectional(DateTime dateTime) => RouteTaskWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.HostToClient), RemoteMethod]
        public Task RemoteMethod_Task_StructParam_HostToClient(DateTime dateTime) => RouteTaskWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.ClientToHost), RemoteMethod]
        public Task RemoteMethod_Task_StructParam_ClientToHost(DateTime dateTime) => RouteTaskWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.Inbound), RemoteMethod]
        public Task RemoteMethod_Task_StructParam_Inbound(DateTime dateTime) => RouteTaskWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.InboundClient), RemoteMethod]
        public Task RemoteMethod_Task_StructParam_InboundClient(DateTime dateTime) => RouteTaskWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.InboundHost), RemoteMethod]
        public Task RemoteMethod_Task_StructParam_InboundHost(DateTime dateTime) => RouteTaskWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.Outbound), RemoteMethod]
        public Task RemoteMethod_Task_StructParam_Outbound(DateTime dateTime) => RouteTaskWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.OutboundClient), RemoteMethod]
        public Task RemoteMethod_Task_StructParam_OutboundClient(DateTime dateTime) => RouteTaskWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.OutboundHost), RemoteMethod]
        public Task RemoteMethod_Task_StructParam_OutboundHost(DateTime dateTime) => RouteTaskWithEventCall(dateTime);

        // ===============================================================================

        public Task<RemotingDirection> RouteTaskEnumWithEventCall(DateTime dateTime)
        {
            RemoteMethodCalledStruct?.Invoke(this, dateTime);
            return ReturnedRemoteMethodTaskEnum;
        }

        [RemoteOptions(RemotingDirection.None), RemoteMethod]
        public Task<RemotingDirection> RemoteMethod_TaskEnum_StructParam_None(DateTime dateTime) => RouteTaskEnumWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.Bidirectional), RemoteMethod]
        public Task<RemotingDirection> RemoteMethod_TaskEnum_StructParam_Bidirectional(DateTime dateTime) => RouteTaskEnumWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.HostToClient), RemoteMethod]
        public Task<RemotingDirection> RemoteMethod_TaskEnum_StructParam_HostToClient(DateTime dateTime) => RouteTaskEnumWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.ClientToHost), RemoteMethod]
        public Task<RemotingDirection> RemoteMethod_TaskEnum_StructParam_ClientToHost(DateTime dateTime) => RouteTaskEnumWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.Inbound), RemoteMethod]
        public Task<RemotingDirection> RemoteMethod_TaskEnum_StructParam_Inbound(DateTime dateTime) => RouteTaskEnumWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.InboundClient), RemoteMethod]
        public Task<RemotingDirection> RemoteMethod_TaskEnum_StructParam_InboundClient(DateTime dateTime) => RouteTaskEnumWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.InboundHost), RemoteMethod]
        public Task<RemotingDirection> RemoteMethod_TaskEnum_StructParam_InboundHost(DateTime dateTime) => RouteTaskEnumWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.Outbound), RemoteMethod]
        public Task<RemotingDirection> RemoteMethod_TaskEnum_StructParam_Outbound(DateTime dateTime) => RouteTaskEnumWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.OutboundClient), RemoteMethod]
        public Task<RemotingDirection> RemoteMethod_TaskEnum_StructParam_OutboundClient(DateTime dateTime) => RouteTaskEnumWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.OutboundHost), RemoteMethod]
        public Task<RemotingDirection> RemoteMethod_TaskEnum_StructParam_OutboundHost(DateTime dateTime) => RouteTaskEnumWithEventCall(dateTime);

        // ===============================================================================

        public Task<DateTime> RouteTaskStructWithEventCall(DateTime dateTime)
        {
            RemoteMethodCalledStruct?.Invoke(this, dateTime);
            return Task.FromResult(dateTime);
        }

        [RemoteOptions(RemotingDirection.None), RemoteMethod]
        public Task<DateTime> RemoteMethod_TaskStruct_StructParam_None(DateTime dateTime) => RouteTaskStructWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.Bidirectional), RemoteMethod]
        public Task<DateTime> RemoteMethod_TaskStruct_StructParam_Bidirectional(DateTime dateTime) => RouteTaskStructWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.HostToClient), RemoteMethod]
        public Task<DateTime> RemoteMethod_TaskStruct_StructParam_HostToClient(DateTime dateTime) => RouteTaskStructWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.ClientToHost), RemoteMethod]
        public Task<DateTime> RemoteMethod_TaskStruct_StructParam_ClientToHost(DateTime dateTime) => RouteTaskStructWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.Inbound), RemoteMethod]
        public Task<DateTime> RemoteMethod_TaskStruct_StructParam_Inbound(DateTime dateTime) => RouteTaskStructWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.InboundClient), RemoteMethod]
        public Task<DateTime> RemoteMethod_TaskStruct_StructParam_InboundClient(DateTime dateTime) => RouteTaskStructWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.InboundHost), RemoteMethod]
        public Task<DateTime> RemoteMethod_TaskStruct_StructParam_InboundHost(DateTime dateTime) => RouteTaskStructWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.Outbound), RemoteMethod]
        public Task<DateTime> RemoteMethod_TaskStruct_StructParam_Outbound(DateTime dateTime) => RouteTaskStructWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.OutboundClient), RemoteMethod]
        public Task<DateTime> RemoteMethod_TaskStruct_StructParam_OutboundClient(DateTime dateTime) => RouteTaskStructWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.OutboundHost), RemoteMethod]
        public Task<DateTime> RemoteMethod_TaskStruct_StructParam_OutboundHost(DateTime dateTime) => RouteTaskStructWithEventCall(dateTime);

        // ===============================================================================

        public Task<object> RouteTaskObjectWithEventCall(DateTime dateTime)
        {
            RemoteMethodCalledStruct?.Invoke(this, dateTime);
            return ReturnedRemoteMethodTaskObject;
        }

        [RemoteOptions(RemotingDirection.None), RemoteMethod]
        public Task<object> RemoteMethod_TaskObject_StructParam_None(DateTime dateTime) => RouteTaskObjectWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.Bidirectional), RemoteMethod]
        public Task<object> RemoteMethod_TaskObject_StructParam_Bidirectional(DateTime dateTime) => RouteTaskObjectWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.HostToClient), RemoteMethod]
        public Task<object> RemoteMethod_TaskObject_StructParam_HostToClient(DateTime dateTime) => RouteTaskObjectWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.ClientToHost), RemoteMethod]
        public Task<object> RemoteMethod_TaskObject_StructParam_ClientToHost(DateTime dateTime) => RouteTaskObjectWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.Inbound), RemoteMethod]
        public Task<object> RemoteMethod_TaskObject_StructParam_Inbound(DateTime dateTime) => RouteTaskObjectWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.InboundClient), RemoteMethod]
        public Task<object> RemoteMethod_TaskObject_StructParam_InboundClient(DateTime dateTime) => RouteTaskObjectWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.InboundHost), RemoteMethod]
        public Task<object> RemoteMethod_TaskObject_StructParam_InboundHost(DateTime dateTime) => RouteTaskObjectWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.Outbound), RemoteMethod]
        public Task<object> RemoteMethod_TaskObject_StructParam_Outbound(DateTime dateTime) => RouteTaskObjectWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.OutboundClient), RemoteMethod]
        public Task<object> RemoteMethod_TaskObject_StructParam_OutboundClient(DateTime dateTime) => RouteTaskObjectWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.OutboundHost), RemoteMethod]
        public Task<object> RemoteMethod_TaskObject_StructParam_OutboundHost(DateTime dateTime) => RouteTaskObjectWithEventCall(dateTime);

        // ===============================================================================

        public Task<int> RouteTaskPrimitiveWithEventCall(DateTime dateTime)
        {
            RemoteMethodCalledStruct?.Invoke(this, dateTime);
            return ReturnedRemoteMethodTaskPrimitive;
        }

        [RemoteOptions(RemotingDirection.None), RemoteMethod]
        public Task<int> RemoteMethod_TaskPrimitive_StructParam_None(DateTime dateTime) => RouteTaskPrimitiveWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.Bidirectional), RemoteMethod]
        public Task<int> RemoteMethod_TaskPrimitive_StructParam_Bidirectional(DateTime dateTime) => RouteTaskPrimitiveWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.HostToClient), RemoteMethod]
        public Task<int> RemoteMethod_TaskPrimitive_StructParam_HostToClient(DateTime dateTime) => RouteTaskPrimitiveWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.ClientToHost), RemoteMethod]
        public Task<int> RemoteMethod_TaskPrimitive_StructParam_ClientToHost(DateTime dateTime) => RouteTaskPrimitiveWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.Inbound), RemoteMethod]
        public Task<int> RemoteMethod_TaskPrimitive_StructParam_Inbound(DateTime dateTime) => RouteTaskPrimitiveWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.InboundClient), RemoteMethod]
        public Task<int> RemoteMethod_TaskPrimitive_StructParam_InboundClient(DateTime dateTime) => RouteTaskPrimitiveWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.InboundHost), RemoteMethod]
        public Task<int> RemoteMethod_TaskPrimitive_StructParam_InboundHost(DateTime dateTime) => RouteTaskPrimitiveWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.Outbound), RemoteMethod]
        public Task<int> RemoteMethod_TaskPrimitive_StructParam_Outbound(DateTime dateTime) => RouteTaskPrimitiveWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.OutboundClient), RemoteMethod]
        public Task<int> RemoteMethod_TaskPrimitive_StructParam_OutboundClient(DateTime dateTime) => RouteTaskPrimitiveWithEventCall(dateTime);

        [RemoteOptions(RemotingDirection.OutboundHost), RemoteMethod]
        public Task<int> RemoteMethod_TaskPrimitive_StructParam_OutboundHost(DateTime dateTime) => RouteTaskPrimitiveWithEventCall(dateTime);
    }
}
