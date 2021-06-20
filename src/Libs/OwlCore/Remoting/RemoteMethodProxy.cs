using OwlCore.Remoting.Transfer;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace OwlCore.Remoting
{
    /// <summary>
    /// Controls the publishing and receiving of the result of a method call.
    /// </summary>
    /// <typeparam name="TResult">The type being returned by the method.</typeparam>
    public class RemoteMethodProxy<TResult> : IDisposable
    {
        private readonly string _methodName;
        private readonly MemberRemote _memberRemote;
        private TaskCompletionSource<TResult> _receiveResCompletionSource;

        /// <summary>
        /// Creates a new instance of <see cref="RemoteMethodProxy{TResult}"/>.
        /// </summary>
        /// <param name="methodName">The name of the method being modified.</param>
        /// <param name="memberRemote">The <see cref="MemberRemote"/> that handles the instance the relevant method is a member of.</param>
        public RemoteMethodProxy(string methodName, MemberRemote memberRemote)
        {
            // TODO: Throw if more than 2 nodes are present. Cannot designate which node to get data from in this scenario.
            _receiveResCompletionSource = new TaskCompletionSource<TResult>();
            _methodName = methodName;
            _memberRemote = memberRemote;

            Id = $"{memberRemote.Id}.MethodCall.{methodName}.{Guid.NewGuid()}";

            AttachEvents();
        }

        private void AttachEvents()
        {
            _memberRemote.MessageHandler.MessageReceived += MessageHandler_MessageReceived;
        }

        private async void MessageHandler_MessageReceived(object sender, byte[] e)
        {
            var message = await _memberRemote.MessageHandler.MessageConverter.DeserializeAsync(e);
            if (message is null || !(message is RemoteMethodProxyMessage proxyMessage) || proxyMessage.MemberInstanceId != Id)
                return;

            _receiveResCompletionSource.TrySetResult((TResult)proxyMessage.Result!);
        }

        private void DetachEvents()
        {
            _memberRemote.MessageHandler.MessageReceived += MessageHandler_MessageReceived;
        }

        /// <summary>
        /// A unique identifier for the calling of this method on this object instance.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Publish the result value for this method call to all listeners.
        /// </summary>
        /// <param name="resultValue">The value to publish as the result.</param>
        public async Task<TResult> PublishResult(TResult resultValue)
        {
            var message = await _memberRemote.MessageHandler.MessageConverter.SerializeAsync(new RemoteMethodProxyMessage(Id, _methodName, resultValue));
            await _memberRemote.MessageHandler.SendMessageAsync(message);

            return resultValue;
        }

        /// <summary>
        /// Waits for another node to call <see cref="PublishResult(TResult)"/>, and returns the given data.
        /// </summary>
        /// <returns></returns>
        public Task<TResult> ReceiveResult()
        {
            return _receiveResCompletionSource.Task;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            DetachEvents();
        }
    }
}
