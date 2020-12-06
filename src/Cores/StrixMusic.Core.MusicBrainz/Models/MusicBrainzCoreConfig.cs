using System;
using System.Collections.Generic;
using Hqub.MusicBrainz.API;
using Microsoft.Extensions.DependencyInjection;
using OwlCore.AbstractUI.Models;
using StrixMusic.Core.MusicBrainz.Services;
using StrixMusic.Core.MusicBrainz.Utils;
using StrixMusic.Sdk.Data.Core;
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

            var textBlock =
                new AbstractTextBox("testBox", "The initial value")
                {
                    Title = "Test text box.",
                    Subtitle = "Enter something useful.",
                };

            textBlock.ValueChanged += TextBlock_ValueChanged;

            AbstractUIElements = new List<AbstractUIElementGroup>()
            {
                new AbstractUIElementGroup("about", PreferredOrientation.Horizontal)
                {
                    Title = "About",
                    Items =  new List<AbstractUIElement>()
                    {
                        textBlock,
                    },
                },
            };
        }

        private void TextBlock_ValueChanged(object sender, string e)
        {
            if (e == "All done!")
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
        public void ConfigureServices(IServiceCollection services)
        {
            var cacheService = new MusicBrainzCacheService();

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
