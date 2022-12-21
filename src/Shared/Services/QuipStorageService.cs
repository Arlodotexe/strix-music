using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using OwlCore.AbstractStorage;
using OwlCore.Provisos;
using StrixMusic.Helpers.Quips;

namespace StrixMusic.Services
{
    /// <summary>
    /// A service for loading quips.
    /// </summary>
    public class QuipStorageService : IAsyncInit
    {
        private int _sumWeight;
        private IFolderData _quipFolder;
        private List<QuipGroup> _activeQuipGroups;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuipStorageService"/> class.
        /// </summary>
        public QuipStorageService(IFolderData quipFolder)
        {
            _quipFolder = quipFolder;
            _activeQuipGroups = new List<QuipGroup>();
        }
        
        /// <inheritdoc/>
        public bool IsInitialized { get; private set; }

        /// <inheritdoc/>
        public async Task InitAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            // TODO: Get language with LocalizationService
            var regCul = CultureInfo.CurrentCulture.Name;
            var cul = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

            var files = await _quipFolder.GetFilesAsync();
            foreach (var file in files)
            {
                if (file.DisplayName.EndsWith(regCul) || file.DisplayName.EndsWith(cul))
                {
                    using var stream = await file.GetStreamAsync();
                    var quipFile = JsonSerializer.Deserialize<QuipFile>(stream);

                    if (quipFile?.QuipGroups is not null)
                    {
                        LoadGroupList(quipFile.QuipGroups, DateTime.Now);
                    }
                }
            }

            IsInitialized = true;
        }

        /// <summary>
        /// Gets a random quip.
        /// </summary>
        /// <returns>A random quip.</returns>
        public string GetQuip()
        {
            var rand = new Random();
            var i = rand.Next(_sumWeight);

            foreach (var group in _activeQuipGroups)
            {
                if (i <= (group.Weight * group.Quips?.Length) - 1)
                {
                    i /= group.Weight;
                    return group.Quips![i];
                }

                i -= group.Weight * group.Quips?.Length ?? 0;
            }

            return string.Empty;
        }
        
        private void LoadGroupList(IEnumerable<QuipGroup> groups, DateTime now)
        {
            foreach (var group in groups)
            {
                if (group.Applies(now))
                {
                    _sumWeight += group.Weight * group.Quips?.Length ?? 0;
                    _activeQuipGroups.Add(group);
                }
            }
        }
    }
}
