using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace StrixMusic.Core.MusicBrainz.Utils
{
    /// <summary>
    /// A cache to handle cache entry.
    /// </summary>
    internal class CacheEntry
    {
        // Cache file header (512 bytes):
        //
        //      0  Timestamp (Int64 = 8 bytes)
        //      8  Request string (char*, max 504 bytes, null-terminated)
        private const int HEADER_LENGTH = 512;

        /// <summary>
        /// Reusable stream used when reading and writing cache.
        /// </summary>
        public Stream? Stream { get; private set; }

        /// <summary>
        /// Timestamp for the cache.
        /// </summary>
        public DateTime TimeStamp { get; private set; }

        /// <summary>
        /// Request holder.
        /// </summary>
        public string? Request { get; set; }

        /// <summary>
        /// Read cache data.
        /// </summary>
        /// <param name="path">Path to the cache file</param>
        /// <param name="request">API request information</param>
        /// <returns>Information related to cache in a <see cref="CacheEntry"/></returns>
        public static CacheEntry? Read(string path, string request)
        {
            const int REQUEST_LENGTH = HEADER_LENGTH - 8; // sizeof(long)

            // The byte buffer to hold the request string.
            var buffer = new byte[REQUEST_LENGTH];

            var size = Math.Min(request.Length, REQUEST_LENGTH);

            Encoding.UTF8.GetBytes(request, 0, size, buffer, 0);

            var file = GetCacheFileName(path, buffer, size);

            if (!File.Exists(file))
            {
                return null;
            }

            var stream = File.OpenRead(file);

            CacheEntry entry;

            using (var reader = new BinaryReader(stream, Encoding.UTF8, true))
            {
                entry = new CacheEntry();

                var timestamp = reader.ReadInt64();

                entry.TimeStamp = TimestampToDateTime(timestamp, DateTimeKind.Utc);

                reader.Read(buffer, 0, REQUEST_LENGTH);

                size = 0;

                while (buffer[size++] != 0 && size < REQUEST_LENGTH)
                {
                }

                entry.Request = Encoding.UTF8.GetString(buffer, 0, size - 1);
            }

            // Check if the cached request matches the given (could be a hash collision).
            if (!request.Contains(entry.Request))
            {
                // Couldn't find content: invalidate the cache entry.
                entry.TimeStamp = DateTime.MinValue;
            }

            stream.Seek(HEADER_LENGTH, SeekOrigin.Begin);

            entry.Stream = stream;

            return entry;
        }

        /// <summary>
        /// Writes cache to the file.
        /// </summary>
        /// <param name="path">Path to cache file.</param>
        /// <param name="request">API request information.</param>
        /// <param name="response">The response string to be cached.</param>
        /// <returns>Returns a <see cref="Task" /></returns>
        public static async Task Write(string path, string request, Stream response)
        {
            const int REQUEST_LENGTH = HEADER_LENGTH - 8; // sizeof(long)

            // The byte buffer to hold the request string.
            var buffer = new byte[REQUEST_LENGTH];

            var size = Math.Min(request.Length, REQUEST_LENGTH);

            Encoding.UTF8.GetBytes(request, 0, size, buffer, 0);

            var name = GetCacheFileName(path, buffer, size);

            try
            {
                using (var stream = File.OpenWrite(name))
                using (var writer = new BinaryWriter(stream))
                {
                    writer.Write(GetUnixTimestamp());
                    writer.Write(buffer);

                    writer.Flush();

                    await response.CopyToAsync(writer.BaseStream);
                }
            }
            catch (Exception)
            {
                // ignore
            }

            // Set the position of the response stream back to 0.
            response.Seek(0, SeekOrigin.Begin);
        }

        /// <summary>
        /// Get's the timestamp for the file.
        /// </summary>
        /// <param name="file">File path for the timestamp.</param>
        /// <returns>The timestamp in <see cref="DateTime"/></returns>
        public static DateTime GetTimestamp(string file)
        {
            long timestamp;

            using (var stream = File.OpenRead(file))
            using (var reader = new BinaryReader(stream))
            {
                timestamp = reader.ReadInt64();
            }

            return TimestampToDateTime(timestamp, DateTimeKind.Utc);
        }

        /// <summary>
        /// Generates a file for the cache.
        /// </summary>
        /// <param name="path">Path to the directory.</param>
        /// <param name="buffer">The data buffer.</param>
        /// <param name="size">Size of the buffer.</param>
        /// <returns>The file path.</returns>
        private static string GetCacheFileName(string path, byte[] buffer, int size)
        {
            return Path.Combine(path, GetHash(buffer, size)) + ".mb-cache";
        }

        /// <summary>
        /// Returns hash of a string (based on MD5, but only 16 instead of 32 bytes).
        /// </summary>
        /// <param name="bytes">Input bytes.</param>
        /// <param name="size">The total size of the hash.</param>
        /// <returns>MD5 hash.</returns>
        private static string GetHash(byte[] bytes, int size)
        {
            var md5 = MD5.Create();

            bytes = md5.ComputeHash(bytes, 0, size);

            var buffer = new StringBuilder();

            for (var i = 0; i < bytes.Length; i += 2)
            {
                buffer.Append(bytes[i].ToString("x2").ToLower());
            }

            return buffer.ToString();
        }

        private static DateTime TimestampToDateTime(long timestamp, DateTimeKind kind)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0, kind).AddSeconds(timestamp).ToLocalTime();
        }

        private static long DateTimeToUtcTimestamp(DateTime dateTime)
        {
            var baseDate = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

            var span = dateTime.ToUniversalTime() - baseDate;

            return (long)span.TotalSeconds;
        }

        private static long GetUnixTimestamp()
        {
            return DateTimeToUtcTimestamp(DateTime.Now);
        }
    }
}
