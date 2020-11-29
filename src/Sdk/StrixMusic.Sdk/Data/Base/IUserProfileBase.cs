using System;
using System.Globalization;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Data.Base
{
    /// <summary>
    /// Describes a generic profile.
    /// </summary>
    public interface IUserProfileBase : IImageCollectionBase, IUrlCollection
    {
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
        /// The <see cref="DateTime"/> the user was born.
        /// </summary>
        /// <remarks>
        /// If missing data, replace the day, month and/or year with part of 1/1/1970.
        /// </remarks>
        DateTime? Birthdate { get; }

        /// <summary>
        /// The user's country or region of origin.
        /// </summary>
        CultureInfo Region { get; }

        /// <summary>
        /// If true, <see cref="ChangeDisplayNameAsync(string)"/> is supported.
        /// </summary>
        bool IsChangeDisplayNameSupported { get; }

        /// <summary>
        /// If true, <see cref="ChangeBirthDateAsync(DateTime)"/> is supported.
        /// </summary>
        bool IsChangeBirthDateAsyncSupported { get; }

        /// <summary>
        /// If true, <see cref="ChangeFullNameAsync(string)"/> is supported.
        /// </summary>
        bool IsChangeFullNameAsyncAsyncSupported { get; }

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
        event EventHandler<string>? DisplayNameChanged;

        /// <summary>
        /// Fires when the <see cref="Birthdate"/> has changed.
        /// </summary>
        event EventHandler<DateTime>? BirthDateChanged;

        /// <summary>
        /// Fires when the <see cref="FullName"/> has changed.
        /// </summary>
        event EventHandler<string>? FullNameChanged;

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
