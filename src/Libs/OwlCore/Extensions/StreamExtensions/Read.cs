using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OwlCore.Extensions
{
    public static partial class StreamExtensions
    {
        /// <summary>
        /// Reads a sequence of bytes from the current stream and advances the position within
        /// the stream by the number of bytes read, until a null byte is read.
        /// </summary>
        /// <returns>An array of the bytes read, excluding the null terminator.</returns>
        public static byte[] ReadToNull(this Stream input)
        {
            var bytes = new List<byte>();
            while (input.Position < input.Length)
            {
                byte b = (byte)input.ReadByte();
                if (b == 0x00)
                {
                    // End of string
                    break;
                }
                else
                {
                    bytes.Add(b);
                }
            }
            return bytes.ToArray();
        }

        /// <summary>
        /// Reads a null-terminated string from the current stream location.
        /// </summary>
        /// <returns>Returns a string using the specified <see cref="Encoding"/>.</returns>
        public static string ReadNullTerminatedString(this Stream input, Encoding encoding)
        {
            return encoding.GetString(input.ReadToNull());
        }
    }
}
