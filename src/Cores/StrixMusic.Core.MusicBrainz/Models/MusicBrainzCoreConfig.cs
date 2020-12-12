using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Hqub.MusicBrainz.API;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.AbstractStorage;
using OwlCore.AbstractUI.Models;
using StrixMusic.Core.MusicBrainz.Services;
using StrixMusic.Core.MusicBrainz.Utils;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Core.MusicBrainz.Models
{
    /// <summary>
    /// Configures the MusicBrainz core.
    /// </summary>
    public class MusicBrainzCoreConfig : ICoreConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MusicBrainzCoreConfig"/> class.
        /// </summary>
        public MusicBrainzCoreConfig(ICore sourceCore)
        {
            SourceCore = sourceCore;

            var textBlock = new AbstractTextBox("testBox", "The initial value")
            {
                Title = "Password entry",
                Subtitle = @"Enter ""something useful"".",
            };

            textBlock.ValueChanged += TextBlock_ValueChanged;

            var button = new AbstractButton(Guid.NewGuid().ToString(), "Pick folder")
            {
                Title = "Add a folder",
                IconCode = "\uE2B1",
            };

            button.Clicked += Button_Clicked;

            var allDoneButton = new AbstractButton(Guid.NewGuid().ToString(), "Done")
            {
                IconCode = "\uE0AB",
            };

            allDoneButton.Clicked += AllDoneButton_Clicked;

            button.Clicked += Button_Clicked;

            var dataListItems = new List<AbstractUIMetadata>
            {
                new AbstractUIMetadata(Guid.NewGuid().ToString())
                {
                    Title = "Item 1",
                    Subtitle = "Subtitle: The test",
                    ImagePath = "https://image.redbull.com/rbcom/052/2017-06-19/3965fbe6-3488-40f8-88bc-b82eb8d1a230/0010/1/800/800/1/pogchamp-twitch.png",
                },
                new AbstractUIMetadata(Guid.NewGuid().ToString())
                {
                    Title = "Item 2",
                    Subtitle = "Subtitle: The sequel",
                    IconCode = "\uE2B1",
                },
                new AbstractUIMetadata(Guid.NewGuid().ToString())
                {
                    Title = "Item 3",
                    IconCode = "\uE7F6",
                },
                new AbstractUIMetadata(Guid.NewGuid().ToString())
                {
                    Title = "Item 4",
                    IconCode = "\uE753",
                },
            };

            var dataList = new AbstractDataList(id: "testList", items: dataListItems)
            {
                PreferredDisplayMode = AbstractDataListPreferredDisplayMode.List,
                Title = "DataList test",
            };

            var dataListGrid = new AbstractDataList(id: "testList", items: dataListItems)
            {
                PreferredDisplayMode = AbstractDataListPreferredDisplayMode.Grid,
                Title = "DataList grid test",
            };

            AbstractUIElements = new List<AbstractUIElementGroup>()
            {
                new AbstractUIElementGroup("about", PreferredOrientation.Horizontal)
                {
                    Title = "MusicBrainz Core",
                    Subtitle = "AbstractUI Demo",
                    Items =  new List<AbstractUIElement>()
                    {
                        textBlock,
                        button,
                        dataList,
                        dataListGrid,
                        allDoneButton,
                    },
                },
            };
        }

        private void AllDoneButton_Clicked(object sender, EventArgs e)
        {
            if (SourceCore is MusicBrainzCore core)
            {
                core.ChangeCoreState(Sdk.Data.CoreState.Configured);
            }
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            var fileSystemService = SourceCore.GetServiceSafe<IFileSystemService>();
            Guard.IsNotNull(fileSystemService, nameof(fileSystemService));

            var folder = await fileSystemService.PickFolder();

            if (folder != null)
            {
                Debug.WriteLine(folder.Name);
                Debug.WriteLine(folder.Path);
            }
            else
            {
                Debug.WriteLine("Nothing picked");
            }
        }

        private void TextBlock_ValueChanged(object sender, string e)
        {
            if (e == "something useful")
            {
                ((MusicBrainzCore)SourceCore).ChangeCoreState(Sdk.Data.CoreState.Configured);
            }
        }

        /// <inheritdoc />
        public ICore SourceCore { get; }

        /// <inheritdoc/>
        public IServiceProvider? Services { get; private set; }

        /// <inheritdoc/>
        public IReadOnlyList<AbstractUIElementGroup> AbstractUIElements { get; }

        /// <inheritdoc/>
        public Uri LogoSvgUrl => new Uri("ms-appx:///Assets/MusicBrainz/logo.svg");

        /// <inheritdoc />
        public MediaPlayerType PreferredPlayerType => MediaPlayerType.None;

        /// <summary>
        /// Configures services for this instance of the core.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task ConfigureServices(IServiceCollection services)
        {
            var cacheService = new MusicBrainzCacheService();
            await cacheService.InitAsync();

            var musicBrainzClient = new MusicBrainzClient
            {
                Cache = new FileRequestCache(cacheService.RootFolder.Path),
            };

            var musicBrainzArtistHelper = new MusicBrainzArtistHelpersService(musicBrainzClient);

            services.Add(new ServiceDescriptor(typeof(MusicBrainzClient), musicBrainzClient));
            services.Add(new ServiceDescriptor(typeof(MusicBrainzArtistHelpersService), musicBrainzArtistHelper));

            Services = services.BuildServiceProvider();
        }
    }
}
