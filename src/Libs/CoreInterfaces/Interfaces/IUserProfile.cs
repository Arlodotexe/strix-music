using System;
using System.Collections.Generic;
using StrixMusic.Services.Settings.Enums;

namespace StrixMusic.CoreInterfaces.Interfaces
{
    /// <summary>
    /// Describes a generic profile.
    /// </summary>
    public interface IUserProfile
    {
        /// <summary>
        /// Identifier for the user
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// Username shown to the user or others
        /// </summary>
        string DisplayName { get; set; }

        /// <summary>
        /// The user's full name
        /// </summary>
        string? FullName { get; set; }

        /// <summary>
        /// The user's email
        /// </summary>
        string? Email { get; set; }

        /// <summary>
        /// Profile images or other images related to this user.
        /// </summary>
        IList<IImage> Images { get; }

        /// <summary>
        /// The <see cref="DateTime"/> the user was born.
        /// </summary>
        /// <remarks>
        /// If missing data, replace the day, month and/or year with part of 1/1/1970
        /// </remarks>
        DateTime? Birthdate { get; }

        /// <summary>
        /// Links to the users' profile(s).
        /// </summary>
        IList<Uri>? Url { get; }

        /// <summary>
        /// The user's country or region of origin.
        /// </summary>
        string? Region { get; set; }

        /// <summary>
        /// The source core which created the parent.
        /// </summary>
        public Core SourceCore { get; set; }
    }
}
