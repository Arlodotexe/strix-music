using System.IO;
using System.Text;
using OwlCore.IO.Streams;

namespace OwlCore.Extensions
{
    /// <summary>
    /// Extensions to encode and decode base62 data.
    /// </summary>
    public static partial class EncodingExtensions
    {
        private static string Base62CodingSpace = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        /// <summary>
        /// Convert a byte array to a Base62 encoded string.
        /// </summary>
        /// <param name="original">Byte array</param>
        /// <returns>Base62 string</returns>
        public static string ToBase62(this byte[] original)
        {
            var sb = new StringBuilder();
            var stream = new BitStream(original);                     // Set up the BitStream
            var read = new byte[1];                                   // Only read 6-bit at a time

            while (true)
            {
                read[0] = 0;
                var length = stream.Read(read, 0, 6);  // Try to read 6 bits

                if (length == 6)                                      // Not reaching the end
                {
                    switch (read[0] >> 3)
                    {
                        // First 5-bit is 11111
                        case 0x1f:
                            sb.Append(Base62CodingSpace[61]);
                            stream.Seek(-1, SeekOrigin.Current);// Leave the 6th bit to next group
                            break;
                        // First 5-bit is 11110
                        case 0x1e:
                            sb.Append(Base62CodingSpace[60]);
                            stream.Seek(-1, SeekOrigin.Current);
                            break;
                        // Encode 6-bit
                        default:
                            sb.Append(Base62CodingSpace[read[0] >> 2]);
                            break;
                    }
                }
                else if (length == 0)                                 // Reached the end completely
                {
                    break;
                }
                else                                                  // Reached the end with some bits left
                {
                    // Padding 0s to make the last bits to 6 bit
                    sb.Append(Base62CodingSpace[read[0] >> (8 - length)]);
                    break;
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Convert a Base62 encoded string to byte array
        /// </summary>
        /// <param name="base62">Base62 string</param>
        /// <returns>Byte array</returns>
        public static byte[] FromBase62(this string base62)
        {
            // Character count
            var count = 0;

            // Set up the BitStream
            var stream = new BitStream(base62.Length * 6 / 8);

            foreach (char c in base62)
            {
                // Look up coding table
                var index = Base62CodingSpace.IndexOf(c);

                // If end is reached
                if (count == base62.Length - 1)
                {
                    // Check if the ending is good
                    var mod = (int)(stream.Position % 8);
                    if (mod == 0)
                        throw new InvalidDataException("an extra character was found");

                    if (index >> (8 - mod) > 0)
                        throw new InvalidDataException("invalid ending character was found");

                    stream.Write(new[] { (byte)(index << mod) }, 0, 8 - mod);
                }
                else
                {
                    switch (index)
                    {
                        // If 60 or 61 then only write 5 bits to the stream, otherwise 6 bits.
                        case 60:
                            stream.Write(new byte[] { 0xf0 }, 0, 5);
                            break;
                        case 61:
                            stream.Write(new byte[] { 0xf8 }, 0, 5);
                            break;
                        default:
                            stream.Write(new[] { (byte)index }, 2, 6);
                            break;
                    }
                }
                count++;
            }

            // Dump out the bytes
            byte[] result = new byte[stream.Position / 8];
            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(result, 0, result.Length * 8);
            return result;
        }
    }
}