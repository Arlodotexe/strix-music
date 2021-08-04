using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using OwlCore.Remoting.Transfer;

namespace OwlCore.Remoting
{
    /// <summary>
    /// Extension methods for asynchronously locking entry of a thread until it is released remotely.
    /// </summary>
    public static class RemoteLock
    {
        private static readonly ConcurrentDictionary<string, Task<object?>> _remoteLockHandles = new ConcurrentDictionary<string, Task<object?>>();

        /// <summary>
        /// Locks entry until released remotely, scoped to the given <paramref name="memberRemote"/> and <paramref name="token"/>.
        /// </summary>
        /// <param name="memberRemote">The member remote used to receive the message.</param>
        /// <param name="token">A unique token used to identify this lock between remoting nodes.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the ongoing task.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. Value is the received data.</returns>
        public static async Task RemoteWaitAsync(this MemberRemote memberRemote, string token, CancellationToken? cancellationToken = null)
        {
            var scopedToken = $"{memberRemote.Id}.{token}.{nameof(RemoteLock)}";

            if (_remoteLockHandles.TryGetValue(scopedToken, out var completionTask))
                await completionTask;

            var taskCompletionSource = new TaskCompletionSource<object?>(cancellationToken);
            memberRemote.MessageReceived += MemberRemote_MessageReceived;

            _remoteLockHandles.TryAdd(scopedToken, taskCompletionSource.Task);

            void MemberRemote_MessageReceived(object sender, IRemoteMessage e)
            {
                if (e is not RemoteDataMessage remoteDataMessage)
                    return;

                if (remoteDataMessage.Token != token)
                    return;

                _remoteLockHandles.TryRemove(scopedToken, out var _);
                taskCompletionSource.SetResult(null);
            }

            var resultData = await taskCompletionSource.Task;
            memberRemote.MessageReceived -= MemberRemote_MessageReceived;
        }

        /// <summary>
        /// Remotely releases all locks that match the given <paramref name="token"/>.
        /// </summary>
        /// <param name="memberRemote">The member remote used to send the message.</param>
        /// <param name="token">A unique token used to identify the lock being released.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the ongoing task.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static Task RemoteReleaseAsync(this MemberRemote memberRemote, string token, CancellationToken? cancellationToken = null)
        {
            var scopedToken = $"{memberRemote.Id}.{token}.{nameof(RemoteLock)}";

            return memberRemote.SendRemotingMessageAsync(new RemoteDataMessage(memberRemote.Id, scopedToken, string.Empty, null), cancellationToken);
        }
    }
}
