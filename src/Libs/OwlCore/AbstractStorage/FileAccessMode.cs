namespace OwlCore.AbstractStorage
{
    /// <summary>
    /// Specifies whether to access a file in read-only mode or in read/write mode.
    /// </summary>
    public enum FileAccessMode
    {
        /// <summary>
        /// Access the file stream in read-only mode.
        /// </summary>
        /// <remarks>
        /// Keep read-only file streams open only while your app is actively reading from the file. You can use multiple independent read-only streams to read a file. If the file is changed while you're reading it, subsequent read operations fail.
        /// </remarks>
        Read,

        /// <summary>
        /// Access the file stream in read/write mode.
        /// </summary>
        /// <remarks>
        /// A file supports a single stream for writing. Writing is non-transactional and occurs in place; that is, data is written immediately to the target file without using a temporary file. Use read/write mode only when you're ready to write immediately in order to avoid conflicts with other operations.
        /// </remarks>
        ReadWrite,
    }
}