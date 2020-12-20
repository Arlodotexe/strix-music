using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        private IFileSystemService? _fileSystemService;

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
                IconCode = "\uE73E",
            };

            var richTextblock = new AbstractRichTextBlock(Guid.NewGuid().ToString(), "The initial value")
            {
                Title = "RichTextBlock Example",
                IconCode = "\uE2B1",
            };

            allDoneButton.Clicked += AllDoneButton_Clicked;

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

            var mutableDataListGrid = new AbstractMutableDataList(id: "mutableDataListGrid", dataListItems.ToList())
            {
                Title = "MutableDataList grid test",
                Subtitle = "Add or remove something",
                PreferredDisplayMode = AbstractDataListPreferredDisplayMode.Grid,
            };

            var mutableDataList = new AbstractMutableDataList(id: "mutableDataList", dataListItems.ToList())
            {
                Title = "MutableDataList test",
                Subtitle = "Add or remove something",
                PreferredDisplayMode = AbstractDataListPreferredDisplayMode.List,
            };

            mutableDataListGrid.AddRequested += MutableDataListGrid_AddRequested;
            mutableDataList.AddRequested += MutableDataListGrid_AddRequested;

            var multiChoiceItems = dataListItems.ToList();

            var comboBox = new AbstractMultiChoiceUIElement(id: "comboBoxTest", multiChoiceItems[0], multiChoiceItems)
            {
                Title = "ComboBox test",
            };

            var radioButtons = new AbstractMultiChoiceUIElement(id: "radioButtonsTest", multiChoiceItems[0], multiChoiceItems)
            {
                Title = "RadioButtons test",
                PreferredDisplayMode = AbstractMultiChoicePreferredDisplayMode.RadioButtons,
            };

            comboBox.ItemSelected += ComboBox_ItemSelected;
            radioButtons.ItemSelected += ComboBox_ItemSelected;

            var boolUi = new AbstractBooleanUIElement("booleanTest", "On")
            {
                State = true,
                Title = "AbstractBoolean test",
            };

            boolUi.StateChanged += BoolUi_StateChanged;

            AbstractUIElements = new List<AbstractUIElementGroup>
            {
                new AbstractUIElementGroup("about", PreferredOrientation.Horizontal)
                {
                    Title = "MusicBrainz Core",
                    Subtitle = "AbstractUI Demo",
                    Items =  new List<AbstractUIElement>()
                    {
                        textBlock,
                        boolUi,
                        comboBox,
                        radioButtons,
                        dataList,
                        dataListGrid,
                        mutableDataListGrid,
                        button,
                        mutableDataList,
                        allDoneButton,
                    },
                },
            };
        }

        private void BoolUi_StateChanged(object sender, bool e)
        {
            if (sender is AbstractBooleanUIElement boolUi)
            {
                boolUi.ChangeLabel(e ? "On" : "Off");
            }
        }

        private void ComboBox_ItemSelected(object sender, AbstractUIMetadata e)
        {
            if (e.Title == "Item 3")
            {
                ((MusicBrainzCore)SourceCore).ChangeCoreState(Sdk.Data.CoreState.Configured);
            }
        }

        private async void MutableDataListGrid_AddRequested(object sender, EventArgs e)
        {
            if (!(sender is AbstractMutableDataList dataList))
                return;

            Guard.IsNotNull(_fileSystemService, nameof(_fileSystemService));

            var folder = await _fileSystemService.PickFolder();

            if (folder is null)
                return;

            var newItem = new AbstractUIMetadata(Guid.NewGuid().ToString())
            {
                IconCode = "\uED25",
                Title = folder.Name,
                Subtitle = folder.Path,
                TooltipText = folder.Path,
            };

            dataList.AddItem(newItem);
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
            Guard.IsNotNull(_fileSystemService, nameof(_fileSystemService));

            var folder = await _fileSystemService.PickFolder();

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
            Services = null;

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

        /// <summary>
        /// Configures the minimum required services for core configuration in a safe manner and is guaranteed not to throw.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task SetupConfigurationServices(IServiceCollection services)
        {
            var provider = services.BuildServiceProvider();

            _fileSystemService = provider.GetRequiredService<IFileSystemService>();
            return _fileSystemService.InitAsync();
        }
    }
}
