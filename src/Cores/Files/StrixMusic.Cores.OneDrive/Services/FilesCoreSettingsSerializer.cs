using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using OwlCore.Services;

namespace StrixMusic.Cores.OneDrive.Services
{
    /// <summary>
    /// An <see cref="IAsyncSerializer{TSerialized}"/> and implementation for serializing and deserializing streams using System.Text.Json.
    /// </summary>
    public class FilesCoreSettingsSerializer : IAsyncSerializer<Stream>
    {
        /// <summary>
        /// A singleton instance for <see cref="FilesCoreSettingsSerializer"/>.
        /// </summary>
        public static FilesCoreSettingsSerializer Singleton { get; } = new();

        /// <inheritdoc />
        public async Task<Stream> SerializeAsync<T>(T data, CancellationToken? cancellationToken = null)
        {
            var stream = new MemoryStream();
            await JsonSerializer.SerializeAsync(stream, data, typeof(T), context: FilesCoreSettingsSerializerContext.Default, cancellationToken: cancellationToken ?? CancellationToken.None);
            return stream;
        }

        /// <inheritdoc />
        public async Task<Stream> SerializeAsync(Type inputType, object data, CancellationToken? cancellationToken = null)
        {
            var stream = new MemoryStream();
            await JsonSerializer.SerializeAsync(stream, data, inputType, context: FilesCoreSettingsSerializerContext.Default, cancellationToken: cancellationToken ?? CancellationToken.None);
            return stream;
        }

        /// <inheritdoc />
        public Task<TResult> DeserializeAsync<TResult>(Stream serialized, CancellationToken? cancellationToken = null)
            => JsonSerializer.DeserializeAsync<TResult>(serialized).AsTask()!;

        /// <inheritdoc />
        public Task<object> DeserializeAsync(Type returnType, Stream serialized, CancellationToken? cancellationToken = null)
            => JsonSerializer.DeserializeAsync(serialized, returnType).AsTask()!;
    }
}
