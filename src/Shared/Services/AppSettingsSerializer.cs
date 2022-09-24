﻿using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using OwlCore.ComponentModel;

namespace StrixMusic.Services
{
    /// <summary>
    /// An <see cref="IAsyncSerializer{TSerialized}"/> and implementation for serializing and deserializing streams using System.Text.Json.
    /// </summary>
    public class AppSettingsSerializer : IAsyncSerializer<Stream>
    {
        /// <summary>
        /// A singleton instance for <see cref="AppSettingsSerializer"/>.
        /// </summary>
        public static AppSettingsSerializer Singleton { get; } = new();

        /// <inheritdoc />
        public async Task<Stream> SerializeAsync<T>(T data, CancellationToken? cancellationToken = null)
        {
            var stream = new MemoryStream();
            await JsonSerializer.SerializeAsync(stream, data, typeof(T), context: AppSettingsSerializerContext.Default, cancellationToken: cancellationToken ?? CancellationToken.None);
            return stream;
        }

        /// <inheritdoc />
        public async Task<Stream> SerializeAsync(Type inputType, object data, CancellationToken? cancellationToken = null)
        {
            var stream = new MemoryStream();
            await JsonSerializer.SerializeAsync(stream, data, inputType, context: AppSettingsSerializerContext.Default, cancellationToken: cancellationToken ?? CancellationToken.None);
            return stream;
        }

        /// <inheritdoc />
        public async Task<TResult> DeserializeAsync<TResult>(Stream serialized, CancellationToken? cancellationToken = null)
        {
            var result = await JsonSerializer.DeserializeAsync(serialized, typeof(TResult), AppSettingsSerializerContext.Default);
            Guard.IsNotNull(result);
            return (TResult)result;
        }

        /// <inheritdoc />
        public async Task<object> DeserializeAsync(Type returnType, Stream serialized, CancellationToken? cancellationToken = null)
        {
            var result = await JsonSerializer.DeserializeAsync(serialized, returnType, AppSettingsSerializerContext.Default);
            Guard.IsNotNull(result);
            return result;
        }
    }
}
