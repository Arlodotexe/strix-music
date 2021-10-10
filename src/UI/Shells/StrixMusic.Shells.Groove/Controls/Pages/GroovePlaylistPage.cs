﻿using StrixMusic.Shells.Groove.ViewModels.Pages;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.Groove.Controls.Pages
{
    public partial class GroovePlaylistPage : Control
    {
        /// <summary>
        /// The backing dependency property for <see cref="ViewModel"/>.
        /// </summary>
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(nameof(ViewModel), typeof(GroovePlaylistPageViewModel), typeof(GroovePlaylistPage), new PropertyMetadata(null));

        public GroovePlaylistPage()
        {
            this.DefaultStyleKey = typeof(GroovePlaylistPage);
        }

        /// <summary>
        /// The <see cref="GroovePlaylistViewModel"/> for the <see cref="GroovePlaylistPage"/> template.
        /// </summary>
        public GroovePlaylistPageViewModel? ViewModel
        {
            get { return (GroovePlaylistPageViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
    }
}
