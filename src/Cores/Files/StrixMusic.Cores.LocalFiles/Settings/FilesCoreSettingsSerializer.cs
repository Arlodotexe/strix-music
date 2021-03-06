using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using OwlCore.Services;

namespace StrixMusic.Cores.LocalFiles.Settings
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
        public async Task<TResult> DeserializeAsync<TResult>(Stream serialized, CancellationToken? cancellationToken = null)
        {
            var result = await JsonSerializer.DeserializeAsync(serialized, typeof(TResult), FilesCoreSettingsSerializerContext.Default);
            Guard.IsNotNull(result);
            return (TResult)result;
        }

        /// <inheritdoc />
        public async Task<object> DeserializeAsync(Type returnType, Stream serialized, CancellationToken? cancellationToken = null)
        {
            var result = await JsonSerializer.DeserializeAsync(serialized, returnType, FilesCoreSettingsSerializerContext.Default);
            Guard.IsNotNull(result);
            return result;
        }
    }
}
