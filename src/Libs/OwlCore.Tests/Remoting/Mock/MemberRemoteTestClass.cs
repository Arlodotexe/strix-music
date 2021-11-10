using System;
using System.Threading.Tasks;
using OwlCore.Remoting;
using OwlCore.Tests.Remoting;
using OwlCore.Tests.Remoting.Transfer;

namespace OwlCore.Tests.Remoting.Mock
{
    /// <summary>
    /// A class used exclusively to test <see cref="OwlCore.Remoting.MemberRemote"/>.
    /// </summary>
    public partial class MemberRemoteTestClass : IDisposable
    {
        [RemoteOptions(RemotingDirection.None)]
        private readonly OwlCore.Remoting.MemberRemote _memberRemote;

        public MemberRemoteTestClass(string id, LoopbackMockMessageHandler messageHandler)
        {
            _memberRemote = new OwlCore.Remoting.MemberRemote(this, id, messageHandler);
            MessageHandler = messageHandler;
        }

        public LoopbackMockMessageHandler MessageHandler { get; }

        public RemotingDirection ReturnedRemoteMethodEnum
        {
            get
            {
                var val = MemberRemoteTests.GetDefaultTestValue<RemotingDirection>(MethodReturnType.Enum);
                RemoteMethodCalledEnum?.Invoke(this, val);
                return val;
            }
        }

        public DateTime ReturnedRemoteMethodStruct
        {
            get
            {
                var val = MemberRemoteTests.GetDefaultTestValue<DateTime>(MethodReturnType.Struct);
                RemoteMethodCalledStruct?.Invoke(this, val);
                return val;
            }
        }

        public object ReturnedRemoteMethodObject
        {
            get
            {
                var val = MemberRemoteTests.GetDefaultTestValue<object>(MethodReturnType.Object);
                RemoteMethodCalledObject?.Invoke(this, val);

                return val;
            }
        }

        public int ReturnedRemoteMethodPrimitive
        {
            get
            {
                var val = MemberRemoteTests.GetDefaultTestValue<int>(MethodReturnType.Primitive);
                RemoteMethodCalledPrimitive?.Invoke(this, val);
                return val;
            }
        }

        public Task ReturnedRemoteMethodTask
        {
            get
            {
                RemoteMethodCalledTask?.Invoke(this, Task.CompletedTask);
                return Task.CompletedTask;
            }
        }

        public Task<RemotingDirection> ReturnedRemoteMethodTaskEnum
        {
            get
            {
                var val = Task.FromResult(MemberRemoteTests.GetDefaultTestValue<RemotingDirection>(MethodReturnType.TaskEnum));
                RemoteMethodCalledTaskEnum?.Invoke(this, val);
                return val;
            }
        }

        public Task<DateTime> ReturnedRemoteMethodTaskStruct
        {
            get
            {
                var val = Task.FromResult(MemberRemoteTests.GetDefaultTestValue<DateTime>(MethodReturnType.TaskStruct));
                RemoteMethodCalledTaskStruct?.Invoke(this, val);
                return val;
            }
        }

        public Task<object> ReturnedRemoteMethodTaskObject
        {
            get
            {
                var val = Task.FromResult(MemberRemoteTests.GetDefaultTestValue<object>(MethodReturnType.TaskObject));
                RemoteMethodCalledTaskObject?.Invoke(this, val);
                return val;
            }
        }

        public Task<int> ReturnedRemoteMethodTaskPrimitive
        {
            get
            {
                var val = Task.FromResult(MemberRemoteTests.GetDefaultTestValue<int>(MethodReturnType.TaskPrimitive));
                RemoteMethodCalledTaskPrimitive?.Invoke(this, val);
                return val;
            }
        }

        // Used for detecting remote method calls, and for validating passed parameters.
        // EventArgs should be ignored if no parameters were passed.
        public event EventHandler RemoteMethodCalledVoid;
        public event EventHandler<RemotingDirection> RemoteMethodCalledEnum;
        public event EventHandler<DateTime> RemoteMethodCalledStruct;
        public event EventHandler<object> RemoteMethodCalledObject;
        public event EventHandler<int> RemoteMethodCalledPrimitive;

        public event EventHandler<Task> RemoteMethodCalledTask;
        public event EventHandler<Task<RemotingDirection>> RemoteMethodCalledTaskEnum;
        public event EventHandler<Task<DateTime>> RemoteMethodCalledTaskStruct;
        public event EventHandler<Task<object>> RemoteMethodCalledTaskObject;
        public event EventHandler<Task<int>> RemoteMethodCalledTaskPrimitive;

        [RemoteOptions(RemotingDirection.None)]
        public void Dispose()
        {
            ((IDisposable)_memberRemote).Dispose();
        }
    }
}
