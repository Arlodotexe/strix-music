using OwlCore.ComponentModel;
using StrixMusic.Sdk.CoreModels;

namespace StrixMusic.Cores.Storage;

/// <summary>
/// The wait behavior of the metadata scanner when <see cref="IAsyncInit.InitAsync"/> is called on a file-based <see cref="ICore"/>.
/// </summary>
public enum ScannerWaitBehavior
{
    /// <summary>
    /// <see cref="IAsyncInit.InitAsync"/> will only wait for the scanner to complete if it there's no cached scan data.
    /// </summary>
    WaitIfNoData,

    /// <summary>
    /// <see cref="IAsyncInit.InitAsync"/> will always wait for the scanner to complete.
    /// </summary>
    AlwaysWait,

    /// <summary>
    /// <see cref="IAsyncInit.InitAsync"/> will never wait for the scanner to complete.
    /// </summary>
    NeverWait,
}