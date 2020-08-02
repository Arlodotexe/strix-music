using System;
using System.Collections.Generic;

namespace StrixMusic.CoreInterfaces.Interfaces
{
    /// <summary>
    /// Describes a generic User.
    /// </summary>
    public interface IUser
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
        IList<Image> Images { get; set; }

        /// <summary>
        /// The <see cref="DateTime"/> the user was born.
        /// </summary>
        /// <remarks>
        /// If missing data, replace the day, month and/or year with part of 1/1/1970
        /// </remarks>
        DateTime Birthdate { get; set; }

        /// <summary>
        /// A link to the user's profile
        /// </summary>
        IList<Uri> Url { get; }

        /// <summary>
        /// The user's country or region of origin.
        /// </summary>
        string Region { get; set; }
    }
}
