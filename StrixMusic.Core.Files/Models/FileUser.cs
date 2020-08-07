using System;
using System.Collections.Generic;
using StrixMusic.CoreInterfaces.Interfaces;

namespace StrixMusic.Core.Files.Models
{
    /// <summary>
    /// Represents a the user that is accessing files.
    /// </summary>
    public class FileUser : IUser
    {
        public FileUser(ICore sourceCore)
        {
            SourceCore = sourceCore;
        }

        /// <inheritdoc/>
        public IList<IDevice> Devices => throw new NotImplementedException();

        /// <inheritdoc/>
        public string Id => nameof(FileUser);

        /// <inheritdoc/>
        public string DisplayName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <inheritdoc/>
        public string FullName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <inheritdoc/>
        public string Email { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <inheritdoc/>
        public IList<IImage> Images => throw new NotImplementedException();

        /// <inheritdoc/>
        public DateTime? Birthdate => null;

        /// <inheritdoc/>
        public IList<Uri> Url => throw new NotImplementedException();

        /// <inheritdoc/>
        public string Region { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <inheritdoc/>
        public ICore SourceCore { get; set; }
    }
}
