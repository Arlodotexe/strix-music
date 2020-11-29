using System.IO;

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
        /// The method will keep reading (and copying into a MemoryStream) until it runs out of data. It then asks the MemoryStream to return a copy of the data in an array.
        /// </remarks>
        /// <seealso href="https://stackoverflow.com/a/221941"/>
        public static byte[] ToBytes(this Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using MemoryStream ms = new MemoryStream();

            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                ms.Write(buffer, 0, read);
            }

            return ms.ToArray();
        }
    }
}
