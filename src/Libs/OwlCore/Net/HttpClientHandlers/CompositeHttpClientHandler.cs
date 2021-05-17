using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using OwlCore.Extensions;

namespace OwlCore.Net.HttpClientHandlers
{
    /// <summary>
    /// An <see cref="HttpClientHandler"/> that can perform multiple arbitrary actions on a single request.
    /// </summary>
    public class CompositeHttpClientHandler : HttpClientHandler
    {
        private readonly List<CompositeHttpClientHandlerAction> _actions;
        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        /// <summary>
        /// Creates a new instance of <see cref="CompositeHttpClientHandler"/>.
        /// </summary>
        /// <param name="actions">The actions to perform when executing <see cref="SendAsync(HttpRequestMessage, CancellationToken)"/>.</param>
        public CompositeHttpClientHandler(params CompositeHttpClientHandlerAction[] actions)
        {
            _actions = actions.ToOrAsList();
        }

        /// <summary>
        /// Creates a new instance of <see cref="CompositeHttpClientHandler"/>.
        /// </summary>
        public CompositeHttpClientHandler()
        {
            _actions = new List<CompositeHttpClientHandlerAction>();
        }

        /// <summary>
        /// Creates a new instance of <see cref="CompositeHttpClientHandler"/>.
        /// </summary>
        /// <param name="actions">The actions to perform when executing <see cref="SendAsync(HttpRequestMessage, CancellationToken)"/>.</param>
        public CompositeHttpClientHandler(params CompositeHttpClientHandlerActionBase[] actions)
        {
            _actions = actions.Select<CompositeHttpClientHandlerActionBase, CompositeHttpClientHandlerAction>(x => x.SendAsync).ToOrAsList();
        }

        /// <summary>
        /// Adds a new action to be run when <see cref="SendAsync(HttpRequestMessage, CancellationToken)"/> is called.
        /// </summary>
        /// <returns></returns>
        public async Task AddAction(CompositeHttpClientHandlerAction action)
        {
            await semaphore.WaitAsync();
            _actions.Insert(0, action);
            semaphore.Release();
        }

        /// <summary>
        /// Adds a new action to be run when <see cref="SendAsync(HttpRequestMessage, CancellationToken)"/> is called.
        /// </summary>
        /// <returns></returns>
        public async Task RemoveAction(CompositeHttpClientHandlerAction action)
        {
            await semaphore.WaitAsync();
            _actions.Remove(action);
            semaphore.Release();
        }

        /// <summary>
        /// Adds a new action to be run when <see cref="SendAsync(HttpRequestMessage, CancellationToken)"/> is called.
        /// </summary>
        /// <returns></returns>
        public async Task AddAction(CompositeHttpClientHandlerActionBase action)
        {
            await semaphore.WaitAsync();
            _actions.Insert(0, action.SendAsync);
            semaphore.Release();
        }

        /// <summary>
        /// Adds a new action to be run when <see cref="SendAsync(HttpRequestMessage, CancellationToken)"/> is called.
        /// </summary>
        /// <returns></returns>
        public async Task RemoveAction(CompositeHttpClientHandlerActionBase action)
        {
            await semaphore.WaitAsync();
            _actions.Remove(action.SendAsync);
            semaphore.Release();
        }

        /// <inheritdoc />
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Func<Task<HttpResponseMessage>> sendActionChain = () => base.SendAsync(request, cancellationToken);

            // Prevents adding a new action while the chain is building.
            await semaphore.WaitAsync(cancellationToken);

            // Build the chain.
            // The last added item will naturally execute first, so we'll invert the chain so the first added item executes first.
            for (var i = _actions.Count - 1; i >= 0; i--)
            {
                // Pass the current chainLink into the next action, returning that action's result as a Fun that we can evaluate, pass THAT into the next action in the list, and so on until we run out.
                // Assuming each delegate correctly returns the given lastSendAction, this chains each action into the next like a linked list.
                var chainLink = sendActionChain;
                var currentAction = _actions[i];

                sendActionChain = () => currentAction(request, cancellationToken, chainLink);
            }

            semaphore.Release();

            // Once the chain is built, simply awaiting it will evaluate cause the entire chain to evaluate from last to first.
            // If at any point, an action internally awaits the given SendAsyncAction, the remainder of the chain will evaluate and return a task to whoever requested it.
            var result = await sendActionChain();

            return result;
        }
    }

    /// <summary>
    /// A delegate for the <see cref="HttpClientHandler.SendAsync(HttpRequestMessage, CancellationToken)"/> action.
    /// </summary>
    /// <param name="request">The HTTP request message.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the request.</param>
    /// <param name="baseSendAsync">The send async action </param>
    /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
    public delegate Task<HttpResponseMessage> CompositeHttpClientHandlerAction(HttpRequestMessage request, CancellationToken cancellationToken, Func<Task<HttpResponseMessage>> baseSendAsync);
}
