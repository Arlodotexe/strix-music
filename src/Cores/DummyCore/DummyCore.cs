using StrixMusic.CoreInterfaces.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DummyCore
{
    public class DummyCore : ICore
    {
        public string Name => "Dummy Core";

        public IUser User => throw new NotImplementedException();

        public event EventHandler<IDevice> DeviceAdded;
        public event EventHandler<IDevice> DeviceRemoved;
        public event EventHandler<IPlayableCollectionGroup> LibraryChanged;
        public event EventHandler<IPlayableCollectionGroup> RecentlyPlayedChanged;
        public event EventHandler<IPlayableCollectionGroup> DiscoverableAdded;
        public event EventHandler<IPlayableCollectionGroup> DiscoverableRemoved;

        public Task<IList<IDevice>> GetDevices()
        {
            return null;
        }

        public Task<IList<IPlayableCollectionGroup>> GetDiscoverables()
        {
            throw new NotImplementedException();
        }

        public Task<IPlayableCollectionGroup> GetLibrary()
        {
            throw new NotImplementedException();
        }

        public Task<IPlayableCollectionGroup> GetRecentlyPlayed()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> GetSearchAutoComplete(string query)
        {
            throw new NotImplementedException();
        }

        public Task<ISearchResults> GetSearchResults(string query)
        {
            throw new NotImplementedException();
        }
    }
}