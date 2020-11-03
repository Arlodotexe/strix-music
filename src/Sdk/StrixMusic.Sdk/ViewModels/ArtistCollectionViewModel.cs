using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using OwlCore.Collections;
using OwlCore.Helpers;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Extensions.SdkMember;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// A wrapper for <see cref="ICoreArtistCollection"/> that contains props and methods for a ViewModel.
    /// </summary>
    public class ArtistCollectionViewModel : ObservableObject, IArtistCollectionViewModel
    {
        private readonly IArtistCollection _collection;

        /// <summary>
        /// Creates a new instance of <see cref="ArtistCollectionViewModel"/>.
        /// </summary>
        /// <param name="collection">The <see cref="IArtistCollection"/> to wrap around.</param>
        public ArtistCollectionViewModel(IArtistCollection collection)
        {
            _collection = collection ?? throw new ArgumentNullException(nameof(collection));

            PopulateMoreArtistsCommand = new AsyncRelayCommand<int>(PopulateMoreArtistsAsync);

            SourceCores = collection.GetSourceCores<ICoreArtistCollection>().Select(MainViewModel.GetLoadedCore).ToList();
            Artists = Threading.InvokeOnUI(() => new SynchronizedObservableCollection<IArtistCollectionItem>());

            AttachEvents();
        }

        private void AttachEvents()
        {
            PlaybackStateChanged += OnPlaybackStateChanged;
            NameChanged += OnNameChanged;
            DescriptionChanged += OnDescriptionChanged;
            UrlChanged += OnUrlChanged;
        }

        private void DetachEvents()
        {
            PlaybackStateChanged -= OnPlaybackStateChanged;
            NameChanged -= OnNameChanged;
            DescriptionChanged -= OnDescriptionChanged;
            UrlChanged -= OnUrlChanged;
        }

        private void OnUrlChanged(object sender, Uri? e) => Url = e;

        private void OnNameChanged(object sender, string e) => Name = e;

        private void OnDescriptionChanged(object sender, string? e) => Description = e;

        private void OnPlaybackStateChanged(object sender, PlaybackState e) => PlaybackState = e;

        /// <inheritdoc />
        public event EventHandler<PlaybackState> PlaybackStateChanged
        {
            add => _collection.PlaybackStateChanged += value;
            remove => _collection.PlaybackStateChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<string> NameChanged
        {
            add => _collection.NameChanged += value;
            remove => _collection.NameChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<string?> DescriptionChanged
        {
            add => _collection.DescriptionChanged += value;
            remove => _collection.DescriptionChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<Uri?> UrlChanged
        {
            add => _collection.UrlChanged += value;
            remove => _collection.UrlChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<TimeSpan>? DurationChanged
        {
            add => _collection.DurationChanged += value;
            remove => _collection.DurationChanged -= value;
        }

        /// <inheritdoc />
        public string Id => _collection.Id;

        /// <inheritdoc />
        public bool IsPlayAsyncSupported => _collection.IsPlayAsyncSupported;

        /// <inheritdoc />
        public bool IsPauseAsyncSupported => _collection.IsPauseAsyncSupported;

        /// <inheritdoc />
        public bool IsChangeNameAsyncSupported => _collection.IsChangeNameAsyncSupported;

        /// <inheritdoc />
        public bool IsChangeDescriptionAsyncSupported => _collection.IsChangeDescriptionAsyncSupported;

        /// <inheritdoc />
        public bool IsChangeDurationAsyncSupported => _collection.IsChangeDurationAsyncSupported;

        /// <inheritdoc />
        public Uri? Url
        {
            get => _collection.Url;
            set => SetProperty(() => _collection.Url, value);
        }

        /// <inheritdoc />
        public string Name
        {
            get => _collection.Name;
            set => SetProperty(() => _collection.Name, value);
        }

        /// <inheritdoc />
        public string? Description
        {
            get => _collection.Description;
            set => SetProperty(() => _collection.Description, value);
        }

        /// <inheritdoc />
        public PlaybackState PlaybackState
        {
            get => _collection.PlaybackState;
            set => SetProperty(() => _collection.PlaybackState, value);
        }

        /// <inheritdoc />
        public TimeSpan Duration
        {
            get => _collection.Duration;
            set => SetProperty(() => _collection.Duration, value);
        }

        /// <inheritdoc />
        public Task PlayAsync()
        {
            return _collection.PlayAsync();
        }

        /// <inheritdoc />
        public Task PauseAsync()
        {
            return _collection.PauseAsync();
        }

        /// <inheritdoc />
        public Task ChangeNameAsync(string name) => _collection.ChangeNameAsync(name);

        /// <inheritdoc />
        public Task ChangeDescriptionAsync(string? description) => _collection.ChangeDescriptionAsync(description);

        /// <inheritdoc />
        public Task ChangeDurationAsync(TimeSpan duration) => _collection.ChangeDurationAsync(duration);

        /// <inheritdoc />
        public SynchronizedObservableCollection<IArtistCollectionItem> Artists { get; }

        /// <inheritdoc cref="ISdkMember{T}.SourceCores" />
        public IReadOnlyList<ICore> SourceCores { get; }

        /// <summary>
        /// The sources that were merged to form this member.
        /// </summary>
        public IReadOnlyList<ICoreArtistCollection> Sources => this.GetSources<ICoreArtistCollection>();

        /// <inheritdoc />
        IReadOnlyList<ICoreImageCollection> ISdkMember<ICoreImageCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreArtistCollection> ISdkMember<ICoreArtistCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreArtistCollectionItem> ISdkMember<ICoreArtistCollectionItem>.Sources => Sources;

        /// <inheritdoc />
        public int TotalArtistItemsCount => _collection.TotalArtistItemsCount;

        /// <inheritdoc />
        public SynchronizedObservableCollection<IImage> Images => _collection.Images;

        /// <inheritdoc />
        public Task<bool> IsAddArtistSupported(int index) => _collection.IsAddArtistSupported(index);

        /// <inheritdoc />
        public Task<bool> IsAddImageSupported(int index) => _collection.IsAddImageSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveArtistSupported(int index) => _collection.IsRemoveArtistSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveImageSupported(int index) => _collection.IsRemoveImageSupported(index);

        /// <inheritdoc />
        public Task<IReadOnlyList<IArtistCollectionItem>> GetArtistItemsAsync(int limit, int offset) =>
            _collection.GetArtistItemsAsync(limit, offset);

        /// <inheritdoc />
        public async Task PopulateMoreArtistsAsync(int limit)
        {
            foreach (var item in await _collection.GetArtistItemsAsync(limit, Artists.Count))
            {
                switch (item)
                {
                    case IArtist artist:
                        Artists.Add(new ArtistViewModel(artist));
                        break;
                    case IArtistCollection collection:
                        Artists.Add(new ArtistCollectionViewModel(collection));
                        break;
                }
            }
        }

        /// <inheritdoc />
        public Task AddArtistItemAsync(IArtistCollectionItem artist, int index) => _collection.AddArtistItemAsync(artist, index);

        /// <inheritdoc />
        public Task RemoveArtistAsync(int index) => _collection.RemoveArtistAsync(index);

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreArtistsCommand { get; }
    }
}