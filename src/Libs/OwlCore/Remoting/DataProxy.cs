using System;
using System.Threading;
using System.Threading.Tasks;
using OwlCore.Remoting.Transfer;

namespace OwlCore.Remoting
{
    /// <summary>
    /// Contains extension methods for easily sending and receving data.
    /// </summary>
    public static class DataProxy
    {
        /// <summary>
        /// Waits for data to be received that contains a matching <paramref name="token"/>, scoped to the given <paramref name="memberRemote"/>.
        /// </summary>
        /// <typeparam name="TResult">The type of data being received.</typeparam>
        /// <param name="memberRemote">The member remote used to receive the message.</param>
        /// <param name="token">A unique token used to identify the response sent from the server.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the ongoing task.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. Value is the received data.</returns>
        public static async Task<TResult?> ReceiveDataAsync<TResult>(this MemberRemote memberRemote, string token, CancellationToken? cancellationToken = null)
        {
            var taskCompletionSource = new TaskCompletionSource<TResult?>(cancellationToken);
            memberRemote.MessageReceived += MemberRemote_MessageReceived;

            void MemberRemote_MessageReceived(object sender, IRemoteMessage e)
            {
                if (e is RemoteDataMessage remoteDataMessage)
                {
                    if (remoteDataMessage.Token != token)
                        return;

                    var type = Type.GetType(remoteDataMessage.TargetMemberSignature);
                    var mostDerivedType = remoteDataMessage.Result?.GetType();

                    if (typeof(TResult?) != type)
                    {
                        throw new ArgumentException($"Generic type argument does not match the received member signature. " +
                                                    $"Expected {typeof(TResult?).AssemblyQualifiedName}, " +
                                                    $"received ({remoteDataMessage.TargetMemberSignature}).");
                    }

                    if (!type?.IsAssignableFrom(mostDerivedType) ?? false)
                    {
                        if (!type?.IsSubclassOf(typeof(IConvertible)) ?? false)
                        {
                            throw new NotSupportedException($"Received data {mostDerivedType?.FullName ?? "null"} is not assignable from received type {type?.FullName ?? "null"} " +
                                                            $"and must implement {nameof(IConvertible)} for automatic type conversion. " +
                                                            $"Either handle conversion of {nameof(RemoteDataMessage)}.{nameof(RemoteDataMessage.Result)} " +
                                                            $"to this type in your {nameof(IRemoteMessageHandler.MessageConverter)} " +
                                                            $"or use a primitive type that implements {nameof(IConvertible)}.");
                        }

                        remoteDataMessage.Result = Convert.ChangeType(remoteDataMessage.Result, type);
                    }

                    taskCompletionSource.SetResult((TResult?)remoteDataMessage.Result);
                }
            }

            var resultData = await taskCompletionSource.Task;
            memberRemote.MessageReceived -= MemberRemote_MessageReceived;

            return resultData;
        }

        /// <summary>
        /// Publishes data remotely, to be received by <see cref="ReceiveDataAsync{TResult}(MemberRemote, string, CancellationToken?)"/>.
        /// </summary>
        /// <typeparam name="T">The type of data being sent.</typeparam>
        /// <param name="memberRemote">The member remote used to send the message.</param>
        /// <param name="token">A unique token used to identify the data being sent.</param>
        /// <param name="data">The data to send outbound.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the ongoing task.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. Value is the exact data given to <paramref name="data"/>.</returns>
        public static async Task<T?> PublishDataAsync<T>(this MemberRemote memberRemote, T? data, string token, CancellationToken? cancellationToken = null)
        {
            var memberSignature = MemberRemote.CreateMemberSignature(typeof(T?), memberRemote.MemberSignatureScope);

            await memberRemote.SendRemotingMessageAsync(new RemoteDataMessage(memberRemote.Id, token, memberSignature, data), cancellationToken);
            return data;
        }
    }
}
