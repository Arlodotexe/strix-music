using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace OwlCore.Net.HttpClientHandlers
{
    /// <summary>
    /// Holds an action and associated data, to be used in a <see cref="CompositeHttpClientHandler"/>.
    /// </summary>
    public abstract class CompositeHttpClientHandlerActionBase
    {
        /// <summary>
        /// The action that will be executed when the HTTP request is made.
        /// </summary>
        /// <param name="request">The HTTP request message.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <param name="baseSendAsync">The equivalent of <see cref="HttpClientHandler.SendAsync(HttpRequestMessage, CancellationToken)"/>.</param>
        /// <returns></returns>
        public abstract Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken, Func<Task<HttpResponseMessage>> baseSendAsync);
    }
}