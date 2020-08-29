using System;
using System.Collections.Generic;

namespace StrixMusic.CoreInterfaces.Interfaces
{
    /// <summary>
    /// Describes a generic profile.
    /// </summary>
    public interface IUserProfile
    {
        /// <summary>
        /// The source core which created the parent.
        /// </summary>
        public ICore SourceCore { get; }

        /// <summary>
        /// Identifier for the user
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Username shown to the user or others
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// The user's full name
        /// </summary>
        string? FullName { get; }

        /// <summary>
        /// The user's email
        /// </summary>
        string? Email { get; }

        /// <summary>
        /// Profile images or other images related to this user.
        /// </summary>
        IReadOnlyList<IImage> Images { get; }

        /// <summary>
        /// The <see cref="DateTime"/> the user was born.
        /// </summary>
        /// <remarks>
        /// If missing data, replace the day, month and/or year with part of 1/1/1970.
        /// </remarks>
        DateTime? Birthdate { get; }

        /// <summary>
        /// Links to the users' profile(s).
        /// </summary>
        IReadOnlyList<Uri>? Url { get; }

        /// <summary>
        /// The user's country or region of origin.
        /// </summary>
        string? Region { get; }

        /// <summary>
        ///  Fires when the <see cref="DisplayName"/> has changed.
        /// </summary>
        event EventHandler<CollectionChangedEventArgs<string>>? DisplayNameChanged;

        /// <summary>
        ///  Fires when the <see cref="ImagesChanged"/> has changed.
        /// </summary>
        event EventHandler<CollectionChangedEventArgs<IImage>>? ImagesChanged;

        /// <summary>
        ///  Fires when the <see cref="Birthdate"/> has changed.
        /// </summary>
        event EventHandler<CollectionChangedEventArgs<DateTime>>? BirthDateChanged;

        /// <summary>
        ///  Fires when the <see cref="FullName"/> has changed.
        /// </summary>
        event EventHandler<CollectionChangedEventArgs<DateTime>>? FullNameChanged;

        /// <summary>
        ///  Fires when the <see cref="Url"/> has changed.
        /// </summary>
        event EventHandler<CollectionChangedEventArgs<DateTime>>? UrlChanged;

        /// <summary>
        ///  Fires when the <see cref="Region"/> has changed.
        /// </summary>
        event EventHandler<CollectionChangedEventArgs<DateTime>>? RegionChanged;

        /// <summary>
        ///  Fires when the <see cref="Email"/> has changed.
        /// </summary>
        event EventHandler<CollectionChangedEventArgs<DateTime>>? EmailChanged;
    }
}
