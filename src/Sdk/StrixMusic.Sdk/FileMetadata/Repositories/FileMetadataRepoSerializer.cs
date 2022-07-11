// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using OwlCore.Services;

namespace StrixMusic.Sdk.FileMetadata.Repositories
{
    /// <summary>
    /// An <see cref="IAsyncSerializer{TSerialized}"/> and implementation for serializing and deserializing streams using System.Text.Json.
    /// </summary>
    public class FileMetadataRepoSerializer : IAsyncSerializer<Stream>
    {
        /// <summary>
        /// A singleton instance for <see cref="FileMetadataRepoSerializer"/>.
        /// </summary>
        public static FileMetadataRepoSerializer Singleton { get; } = new();

        /// <inheritdoc />
        public async Task<Stream> SerializeAsync<T>(T data, CancellationToken? cancellationToken = null)
        {
            var stream = new MemoryStream();
            await JsonSerializer.SerializeAsync(stream, data, typeof(T), context: FileMetadataRepoSerializerContext.Default, cancellationToken: cancellationToken ?? CancellationToken.None);
            return stream;
        }

        /// <inheritdoc />
        public async Task<Stream> SerializeAsync(Type inputType, object data, CancellationToken? cancellationToken = null)
        {
            var stream = new MemoryStream();
            await JsonSerializer.SerializeAsync(stream, data, inputType, context: FileMetadataRepoSerializerContext.Default, cancellationToken: cancellationToken ?? CancellationToken.None);
            return stream;
        }

        /// <inheritdoc />
        public async Task<TResult> DeserializeAsync<TResult>(Stream serialized, CancellationToken? cancellationToken = null)
        {
            var result = await JsonSerializer.DeserializeAsync(serialized, typeof(TResult), FileMetadataRepoSerializerContext.Default);
            Guard.IsNotNull(result);
            return (TResult)result;
        }

        /// <inheritdoc />
        public async Task<object> DeserializeAsync(Type returnType, Stream serialized, CancellationToken? cancellationToken = null)
        {
            var result = await JsonSerializer.DeserializeAsync(serialized, returnType, FileMetadataRepoSerializerContext.Default);
            Guard.IsNotNull(result);
            return result;
        }
    }
}
