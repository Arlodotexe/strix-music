using System.IO;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace OwlCore.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="Stream"/>
    /// </summary>
    public static partial class StreamExtensions
    {
        /// <summary>
        /// Converts the <paramref name="input"/> <see cref="Stream"/> to a byte array.
        /// </summary>
        /// <remarks>
        /// The method will seek to the start of the stream, read (and copy into a MemoryStream) until it runs out of data. It then asks the MemoryStream to return a copy of the data in an array.
        /// </remarks>
        public static byte[] ToBytes(this Stream input)
        {
            var originalPosition = input.Position;
            input.Position = 0;

            using var memStream = new MemoryStream();
            input.CopyTo(memStream);

            input.Position = originalPosition;
            return memStream.ToArray();
        }

        /// <summary>
        /// Converts the <paramref name="input"/> <see cref="Stream"/> to a byte array.
        /// </summary>
        /// <remarks>
        /// The method will seek to the start of the stream, read (and copy into a MemoryStream) until it runs out of data. It then asks the MemoryStream to return a copy of the data in an array.
        /// </remarks>
        public static async Task<byte[]> ToBytesAsync(this Stream input)
        {
            var originalPosition = input.Position;
            input.Position = 0;

            using var memStream = new MemoryStream();
            await input.CopyToAsync(memStream);

            input.Position = originalPosition;
            return memStream.ToArray();
        }
    }
}
