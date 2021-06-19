using Microsoft.Toolkit.Mvvm.ComponentModel;
using OwlCore.Extensions;
using OwlCore.MemberInterception;
using OwlCore.Remoting.Attributes;
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

        private IEnumerable<MethodIntercept> _methods;
        private IEnumerable<EventInfo> _events;
        private IEnumerable<FieldInfo> _fields;
        private IEnumerable<PropertyInfo> _properties;

        /// <summary>
        /// Creates a new instance of <see cref="MemberRemote"/>.
        /// </summary>
        /// <param name="classInstance">The instance to listen for </param>
        /// <param name="id"></param>
        /// <param name="mode"></param>
        public MemberRemote(object classInstance, string id, RemotingMode mode)
        {
            _type = classInstance.GetType();
            _instance = classInstance;
            _id = id;
            _mode = mode;

            var members = _type.GetMembers();

            _events = members.Where(x => x.MemberType == MemberTypes.Event).Cast<EventInfo>(); // TODO: Intercepts
            _fields = members.Where(x => x.MemberType == MemberTypes.Field).Cast<FieldInfo>(); // TODO: Intercepts
            _properties = members.Where(x => x.MemberType == MemberTypes.Property).Cast<PropertyInfo>(); // TODO: Intercepts
            _methods = members.Where(x => x.MemberType == MemberTypes.Method).Select(x => new MethodIntercept((MethodInfo)x, _instance));


            // Might need "async lock" on every method
            // Normal lock on sync methods / properties setter?
            foreach (var method in _methods)
            {
                method.MethodExecuted += OnMethodExecuted;
            }
        }

        private void OnMethodExecuted(object sender, object[] parameters)
        {
            var intercept = (MethodIntercept)sender;
            var methodInfo = intercept.OriginalMethodInfo;
            var options = methodInfo.GetCustomAttribute<RemoteMethodAttribute>();

            if (_mode == RemotingMode.None || options.Direction == RemotingDirection.None)
                return;

            var isValidHostConfig = _mode == RemotingMode.Host && options.Direction == (RemotingDirection.InboundHost | RemotingDirection.OutboundHost);
            var isValidClientConfig = _mode == RemotingMode.Client && options.Direction == (RemotingDirection.InboundClient | RemotingDirection.OutboundClient);

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

    public static class FakeValues
    {
        public static RemotingMode GlobalRemotingMode;
    }

    public class Test : IDisposable
    {
        private MemberRemote _remote;

        public Test()
        {
            _remote = new MemberRemote(this, "someId", FakeValues.GlobalRemotingMode);
        }

        // Fully remote method, synced between all hosts and clients (all connected nodes).
        [RemoteMethod(RemotingDirection.OutboundHost | RemotingDirection.InboundClient)]
        public void TestMethod(int data)
        {
            System.Diagnostics.Debug.WriteLine(data);
        }

        /// <inheritdoc/>
        [RemoteMethod(RemotingDirection.None)]
        public void Dispose()
        {
            _remote.Dispose();
        }
    }
}
