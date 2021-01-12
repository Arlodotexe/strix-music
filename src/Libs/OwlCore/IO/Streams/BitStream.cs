using System;
using System.IO;

namespace OwlCore.IO.Streams
{
    /// <summary>
    /// Utility that read and write bits in byte array
    /// </summary>
    // Source: https://github.com/renmengye/base62-csharp/blob/master/BitStream.cs
    public class BitStream : Stream
    {
        private byte[] Source { get; set; }

        /// <summary>
        /// Initialize the stream with capacity
        /// </summary>
        /// <param name="capacity">Capacity of the stream</param>
        public BitStream(int capacity)
        {
            Source = new byte[capacity];
        }

        /// <summary>
        /// Initialize the stream with a source byte array
        /// </summary>
        /// <param name="source"></param>
        public BitStream(byte[] source)
        {
            Source = source;
        }

        /// <inheritdoc/>
        public override bool CanRead => true;

        /// <inheritdoc/>
        public override bool CanSeek => true;

        /// <inheritdoc/>
        public override bool CanWrite => true;

        /// <inheritdoc/>
        public override void Flush()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Bit length of the stream
        /// </summary>
        public override long Length => Source.Length * 8;

        /// <summary>
        /// Bit position of the stream
        /// </summary>
        public override long Position { get; set; }

        /// <summary>
        /// Read the stream to the buffer
        /// </summary>
        /// <param name="buffer">Buffer</param>
        /// <param name="offset">Offset bit start position of the stream</param>
        /// <param name="count">Number of bits to read</param>
        /// <returns>Number of bits read</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            // Temporary position cursor
            var tempPos = Position;
            tempPos += offset;

            // Buffer byte position and in-byte position
            int readPosCount = 0, readPosMod = 0;

            // Stream byte position and in-byte position
            var posCount = tempPos >> 3;
            var posMod = (int)(tempPos - ((tempPos >> 3) << 3));

            while (tempPos < Position + offset + count && tempPos < Length)
            {
                // Copy the bit from the stream to buffer
                if ((Source[posCount] & (0x1 << (7 - posMod))) != 0)
                {
                    buffer[readPosCount] = (byte)(buffer[readPosCount] | (0x1 << (7 - readPosMod)));
                }
                else
                {
                    buffer[readPosCount] = (byte)(buffer[readPosCount] & (0xffffffff - (0x1 << (7 - readPosMod))));
                }

                // Increment position cursors
                tempPos++;
                if (posMod == 7)
                {
                    posMod = 0;
                    posCount++;
                }
                else
                {
                    posMod++;
                }
                if (readPosMod == 7)
                {
                    readPosMod = 0;
                    readPosCount++;
                }
                else
                {
                    readPosMod++;
                }
            }

            var bits = (int)(tempPos - Position - offset);
            Position = tempPos;
            return bits;
        }

        /// <summary>
        /// Set up the stream position
        /// </summary>
        /// <param name="offset">Position</param>
        /// <param name="origin">Position origin</param>
        /// <returns>Position after setup</returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case (SeekOrigin.Begin):
                    {
                        Position = offset;
                        break;
                    }
                case (SeekOrigin.Current):
                    {
                        Position += offset;
                        break;
                    }
                case (SeekOrigin.End):
                    {
                        Position = Length + offset;
                        break;
                    }
            }
            return Position;
        }

        /// <inheritdoc />
        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Write from buffer to the stream
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset">Offset start bit position of buffer</param>
        /// <param name="count">Number of bits</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            // Temporary position cursor
            var tempPos = Position;

            // Buffer byte position and in-byte position
            int readPosCount = offset >> 3, readPosMod = offset - ((offset >> 3) << 3);

            // Stream byte position and in-byte position
            var posCount = tempPos >> 3;
            var posMod = (int)(tempPos - ((tempPos >> 3) << 3));

            while (tempPos < Position + count && tempPos < Length)
            {
                // Copy the bit from buffer to the stream
                if ((buffer[readPosCount] & (0x1 << (7 - readPosMod))) != 0)
                {
                    Source[posCount] = (byte)(Source[posCount] | (0x1 << (7 - posMod)));
                }
                else
                {
                    Source[posCount] = (byte)(Source[posCount] & (0xffffffff - (0x1 << (7 - posMod))));
                }

                // Increment position cursors
                tempPos++;
                if (posMod == 7)
                {
                    posMod = 0;
                    posCount++;
                }
                else
                {
                    posMod++;
                }
                if (readPosMod == 7)
                {
                    readPosMod = 0;
                    readPosCount++;
                }
                else
                {
                    readPosMod++;
                }
            }
            Position = tempPos;
        }
    }
}