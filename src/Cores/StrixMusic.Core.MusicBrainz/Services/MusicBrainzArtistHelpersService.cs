using System.Linq;
using System.Threading.Tasks;
using Hqub.MusicBrainz.API;
using Hqub.MusicBrainz.API.Entities;
using StrixMusic.Core.MusicBrainz.Models;
using StrixMusic.Core.MusicBrainz.Statics;

namespace StrixMusic.Core.MusicBrainz.Services
{
    /// <summary>
    /// Various helper methods for interacting and creating <see cref="MusicBrainzArtist"/>
    /// </summary>
    public class MusicBrainzArtistHelpersService
    {
        private readonly MusicBrainzClient _musicBrainzClient;

        /// <summary>
        /// Creates a new instance of <see cref="MusicBrainzArtistHelpersService"/>.
        /// </summary>
        /// <param name="musicBrainzClient">The <see cref="MusicBrainzClient"/> to use for getting data from the API.</param>
        public MusicBrainzArtistHelpersService(MusicBrainzClient musicBrainzClient)
        {
            _musicBrainzClient = musicBrainzClient;
        }

        /// <summary>
        /// Get the total track count for a given <see cref="Artist"/>.
        /// </summary>
        /// <param name="artist">The <see cref="Artist"/> to check.</param>
        /// <returns>The total number of tracks for the <see cref="Artist"/>.</returns>
        public async Task<int> GetTotalTracksCount(Artist artist)
        {
            var firstPage = await _musicBrainzClient.Releases.BrowseAsync("artist", artist.Id, 100, 0, RelationshipQueries.Releases);

            if (firstPage.Items.Count < firstPage.Count)
            {
                var remainingItems = await OwlCore.Helpers.APIs.GetAllItemsAsync(firstPage.Count, firstPage.Items, async currentOffset =>
                {
                    return (await _musicBrainzClient.Releases.BrowseAsync("artist", artist.Id, 100, currentOffset, RelationshipQueries.Releases))?.Items;
                });

                firstPage.Items.AddRange(remainingItems);
            }

            return firstPage.Items.SelectMany(x => x.Media, (release, medium) => medium.TrackCount).Sum();
        }
    }
}
