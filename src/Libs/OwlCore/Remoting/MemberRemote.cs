using Microsoft.Toolkit.Mvvm.ComponentModel;
using OwlCore.Extensions;
using OwlCore.MemberInterception;
using OwlCore.Remoting.Attributes;
using OwlCore.Remoting.EventArgs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace OwlCore.Remoting
{
    /// <summary>
    /// Automatically sync events, properties, method calls and fields remotely between two object instances, even if the code runs on another machine.
    /// </summary>
    public class MemberRemote : IDisposable
    {
        private Type _type;
        private object _instance;
        private string _id;
        private readonly RemotingMode _mode;

        private IEnumerable<MethodInfo> _methods;
        private IEnumerable<EventInfo> _events;
        private IEnumerable<FieldInfo> _fields;
        private IEnumerable<PropertyInfo> _properties;

        /// <summary>
        /// Creates a new instance of <see cref="MemberRemote"/>.
        /// </summary>
        /// <param name="classInstance">The instance to remote.</param>
        /// <param name="id">A unique identifier for this instance, consistent between hosts and clients.</param>
        /// <param name="mode"></param>
        public static MemberRemote Register(object classInstance, string id, RemotingMode mode)
        {
            return new MemberRemote(classInstance, id, mode);
        }

        private MemberRemote(object classInstance, string id, RemotingMode mode)
        {
            _type = classInstance.GetType();
            _instance = classInstance;
            _id = id;
            _mode = mode;

            var members = _type.GetMembers();

            _events = members.Where(x => x.MemberType == MemberTypes.Event).Cast<EventInfo>(); // TODO: Intercepts
            _fields = members.Where(x => x.MemberType == MemberTypes.Field).Cast<FieldInfo>(); // TODO: Intercepts
            _properties = members.Where(x => x.MemberType == MemberTypes.Property).Cast<PropertyInfo>(); // TODO: Intercepts
            _methods = members.Where(x => x.MemberType == MemberTypes.Method).Cast<MethodInfo>();

            foreach (var method in _methods)
            {
                var attr = method.GetCustomAttribute<RemoteMethod_v1Attribute>();
                if (attr is null)
                    return;

                attr.Entered += OnMethodEntered;
            }
        }

        private void OnMethodEntered(object sender, MethodEnteredEventArgs e)
        {
            var attribute = (RemoteMethod_v1Attribute)sender;

            if (_mode == RemotingMode.None || attribute.Direction == RemotingDirection.None)
                return;

            var isValidHostConfig = _mode == RemotingMode.Host && attribute.Direction == (RemotingDirection.InboundHost | RemotingDirection.OutboundHost);
            var isValidClientConfig = _mode == RemotingMode.Client && attribute.Direction == (RemotingDirection.InboundClient | RemotingDirection.OutboundClient);

            // emit the data
            if (isValidClientConfig || isValidHostConfig)
            {
                //_someDataTransferService.SendAsync(_id, methodInfo.Name, parameters);
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }

    public class Test
    {
        public Test()
        {
            MemberRemote.Register(this, "myId", RemotingMode.Full);
            Method();
            MethodAsync();
            OtherMethodAsync();
        }

        [RemoteMethod_v1(RemotingDirection.Bidirectional)]
        public void Method()
        {
            System.Diagnostics.Debug.WriteLine("Method");
        }

        [RemoteMethod_v1(RemotingDirection.Outbound | RemotingDirection.InboundClient)]
        public async Task MethodAsync()
        {
            await Task.CompletedTask;
            System.Diagnostics.Debug.WriteLine("MethodAsync");
        }

        [RemoteMethod_v1(RemotingDirection.Outbound)]
        public Task OtherMethodAsync()
        {
            System.Diagnostics.Debug.WriteLine("OtherAsync");

            return Task.CompletedTask;
        }
    }
}
