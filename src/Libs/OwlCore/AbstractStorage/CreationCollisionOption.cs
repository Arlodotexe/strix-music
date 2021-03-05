namespace OwlCore.AbstractStorage
{
    /// <summary>
    /// Specifies what to do if a file or folder with the specified name already exists in the current folder when you create a new file or folder.
    /// </summary>
    /// <remarks>
    /// Methods that don't explicitly pass a value from the CreationCollisionOption enumeration use the FailIfExists value as the default when you create a file or folder.
    /// Mirrors the <see langword="enum"/> at https://docs.microsoft.com/en-us/uwp/api/Windows.Storage.CreationCollisionOption?view=winrt-19041
    /// </remarks>
    public enum CreationCollisionOption : byte
    {
        /// <summary>
        /// Automatically append a number to the base of the specified name if the file or folder already exists.
        /// </summary>
        /// <remarks>For example, if MyFile.txt already exists, then the new file is named MyFile (2).txt. If MyFolder already exists, then the new folder is named MyFolder (2).</remarks>
        GenerateUniqueName = 0,

        /// <summary>
        /// Replace the existing item if the file or folder already exists.
        /// </summary>
        ReplaceExisting = 1,

        /// <summary>
        /// Return the existing item if the file or folder already exists.
        /// </summary>
        OpenIfExists = 2,

        /// <summary>
        /// Raise an exception of type System.Exception if the file or folder already exists.
        /// </summary>
        /// <remarks>Methods that don't explicitly pass a value from the CreationCollisionOption enumeration use the FailIfExists value as the default when you try to create, rename, copy, or move a file or folder.</remarks>
        FailIfExists = 3,
    }
}