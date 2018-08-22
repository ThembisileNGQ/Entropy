using System;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Akka.Websockets.Manager.Common.Networking;

namespace Akka.Websockets.Manager.Common.Strategies
{
    public abstract class MethodInvocationStrategy
    {
        /// <summary>
        /// Called when an invoke method call has been received.
        /// </summary>
        /// <param name="socket">The web-socket of the client that wants to invoke a method.</param>
        /// <param name="invocationDescriptor">
        /// The invocation descriptor containing the method name and parameters.
        /// </param>
        /// <returns>Awaitable Task.</returns>
        public virtual Task<object> OnInvokeMethodReceivedAsync(WebSocket socket, InvocationDescriptor invocationDescriptor)
        {
            throw new NotImplementedException();
        }
    }
}