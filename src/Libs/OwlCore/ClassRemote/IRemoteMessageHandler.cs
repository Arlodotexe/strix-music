using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OwlCore.ClassRemote
{
    public interface IRemoteMessageHandler
    {
        // think these through more
        public event EventHandler<IRemotingMessage[]>? DataInbound;
        public event EventHandler<IRemotingMessage[]>? DataOutbound;

        // think these through more
        Task SendData();
        Task ReceiveData();
    }

    public interface IRemotingMessage
    {
        /// <summary>
        /// A unique identifier for the <see cref="RemoteViewModel"/> being remotely changed.
        /// </summary>
        
        // What if the IDs are unknown until they're created?
        // Sender decides ID... So the ID must be sent as part of an "instance registration".
        public string TargetInstanceId { get; set; }
        
        // think these through more
        public byte[] Payload { get; set; }
        public List<byte[]> PropertyChanges { get; set; }
        public List<byte[]> EventInvocations { get; set; }
        public List<byte[]> MethodCalls { get; set; }
    }
}
