using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

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
        IReadOnlyList<Uri>? Urls { get; }

        /// <summary>
        /// The user's country or region of origin.
        /// </summary>
        CultureInfo Region { get; }

        /// <summary>
        /// If true, <see cref="ChangeDisplayNameAsync(string)"/> is supported.
        /// </summary>
        bool IsChangeDisplayNameSupported { get; }

        /// <summary>
        /// If true, <see cref="ChangeImagesAsync(IReadOnlyList{IImage})"/> is supported.
        /// </summary>
        bool IsChangeImagesAsyncSupported { get; }

        /// <summary>
        /// If true, <see cref="ChangeBirthDateAsync(DateTime)"/> is supported.
        /// </summary>
        bool IsChangeBirthDateAsyncSupported { get; }

        /// <summary>
        /// If true, <see cref="ChangeFullNameAsync(string)"/> is supported.
        /// </summary>
        bool IsChangeFullNameAsyncAsyncSupported { get; }

        /// <summary>
        /// If true, <see cref="ChangeUrlsAsync(IReadOnlyList{Uri})"/> is supported.
        /// </summary>
        bool IsChangeUrlsAsyncSupported { get; }

        /// <summary>
        /// If true, <see cref="ChangeRegionAsync(CultureInfo)"/> is supported.
        /// </summary>
        bool IsChangeRegionAsyncSupported { get; }

        /// <summary>
        /// If true, <see cref="ChangeEmailAsync(string?)"/> is supported.
        /// </summary>
        bool IsChangeEmailAsyncSupported { get; }

        /// <summary>
        /// Changes the <see cref="DisplayName"/> for this user.
        /// </summary>
        /// <param name="displayName">The new display name.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ChangeDisplayNameAsync(string displayName);

        /// <summary>
        /// Changes the <see cref="Images"/> for this user.
        /// </summary>
        /// <param name="images">The new images.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ChangeImagesAsync(IReadOnlyList<IImage> images);

        /// <summary>
        /// Changes the <see cref="Birthdate"/> for this user.
        /// </summary>
        /// <param name="birthdate">The new birthdate.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ChangeBirthDateAsync(DateTime birthdate);

        /// <summary>
        /// Changes the <see cref="FullName"/> for this user.
        /// </summary>
        /// <param name="fullname">The full name.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ChangeFullNameAsync(string fullname);

        /// <summary>
        /// Changes the <see cref="Urls"/> for this user.
        /// </summary>
        /// <param name="urls">The new urls.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ChangeUrlsAsync(IReadOnlyList<Uri> urls);

        /// <summary>
        /// Changes the <see cref="Region"/> for this user.
        /// </summary>
        /// <param name="region">The new region.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ChangeRegionAsync(CultureInfo region);

        /// <summary>
        /// Changes the <see cref="Email"/> for this user.
        /// </summary>
        /// <param name="email">The new email.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ChangeEmailAsync(string? email);

        /// <summary>
        /// Fires when the <see cref="DisplayName"/> has changed.
        /// </summary>
        event EventHandler<CollectionChangedEventArgs<string>>? DisplayNameChanged;

        /// <summary>
        /// Fires when the <see cref="ImagesChanged"/> has changed.
        /// </summary>
        event EventHandler<CollectionChangedEventArgs<IImage>>? ImagesChanged;

        /// <summary>
        /// Fires when the <see cref="Birthdate"/> has changed.
        /// </summary>
        event EventHandler<CollectionChangedEventArgs<DateTime>>? BirthDateChanged;

        /// <summary>
        /// Fires when the <see cref="FullName"/> has changed.
        /// </summary>
        event EventHandler<CollectionChangedEventArgs<string>>? FullNameChanged;

        /// <summary>
        /// Fires when the <see cref="Urls"/> has changed.
        /// </summary>
        event EventHandler<CollectionChangedEventArgs<Uri>>? UrlsChanged;

        /// <summary>
        /// Fires when the <see cref="Region"/> has changed.
        /// </summary>
        event EventHandler<CultureInfo>? RegionChanged;

        /// <summary>
        /// Fires when the <see cref="Email"/> has changed.
        /// </summary>
        event EventHandler<string?>? EmailChanged;
    }
}
