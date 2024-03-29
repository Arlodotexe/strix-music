﻿using System.IO;

namespace StrixMusic.Cores.Storage.FileMetadata.Scanners;

/// <inheritdoc/>
internal sealed class FileAbstraction : TagLib.File.IFileAbstraction
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FileAbstraction"/> class.
    /// </summary>
    public FileAbstraction(string fileName, Stream file)
    {
        Name = fileName;
        Stream = file;
    }

    /// <summary>
    /// The name of the file.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The stream used to construct this <see cref="FileAbstraction"/>.
    /// </summary>
    public Stream Stream { get; }

    /// <summary>
    /// A stream that can read the file.
    /// </summary>
    public Stream ReadStream => Stream;

    /// <summary>
    /// A stream that can write to the file.
    /// </summary>
    public Stream WriteStream => Stream;

    /// <summary>
    /// Closes the stream.
    /// </summary>
    public void CloseStream(Stream stream)
    {
        stream.Close();
    }
}