using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using OwlCore.Events;
using StrixMusic.Sdk.Models.Core;

namespace StrixMusic.Cores.OwlCoreRpc.Tests.Mock
{
    public class MockCoreUser : ICoreUser
    {
        public ICoreLibrary Library => throw new NotImplementedException();

        public string Id => throw new NotImplementedException();

        public string DisplayName => throw new NotImplementedException();

        public string? FullName => throw new NotImplementedException();

        public string? Email => throw new NotImplementedException();

        public DateTime? Birthdate => throw new NotImplementedException();

        public CultureInfo Region => throw new NotImplementedException();

        public bool IsChangeDisplayNameAvailable => throw new NotImplementedException();

        public bool IsChangeBirthDateAsyncAvailable => throw new NotImplementedException();

        public bool IsChangeFullNameAsyncAsyncAvailable => throw new NotImplementedException();

        public bool IsChangeRegionAsyncAvailable => throw new NotImplementedException();

        public bool IsChangeEmailAsyncAvailable => throw new NotImplementedException();

        public int TotalUrlCount => throw new NotImplementedException();

        public int TotalImageCount => throw new NotImplementedException();

        public ICore SourceCore => throw new NotImplementedException();

        public event EventHandler<string>? DisplayNameChanged;
        public event EventHandler<DateTime>? BirthDateChanged;
        public event EventHandler<string>? FullNameChanged;
        public event EventHandler<CultureInfo>? RegionChanged;
        public event EventHandler<string?>? EmailChanged;
        public event CollectionChangedEventHandler<ICoreUrl>? UrlsChanged;
        public event EventHandler<int>? UrlsCountChanged;
        public event CollectionChangedEventHandler<ICoreImage>? ImagesChanged;
        public event EventHandler<int>? ImagesCountChanged;

        public Task AddImageAsync(ICoreImage image, int index, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task AddUrlAsync(ICoreUrl url, int index, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task ChangeBirthDateAsync(DateTime birthdate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task ChangeDisplayNameAsync(string displayName, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task ChangeEmailAsync(string? email, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task ChangeFullNameAsync(string fullname, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task ChangeRegionAsync(CultureInfo region, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public ValueTask DisposeAsync()
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<ICoreImage> GetImagesAsync(int limit, int offset, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<ICoreUrl> GetUrlsAsync(int limit, int offset, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsAddImageAvailableAsync(int index, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsAddUrlAvailableAsync(int index, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsRemoveImageAvailableAsync(int index, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsRemoveUrlAvailableAsync(int index, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task RemoveImageAsync(int index, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task RemoveUrlAsync(int index, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
