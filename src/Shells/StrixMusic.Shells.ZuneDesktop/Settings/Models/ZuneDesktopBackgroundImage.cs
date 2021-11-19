using System;
using Windows.UI.Xaml.Media;

namespace StrixMusic.Shells.ZuneDesktop.Settings.Models
{
    /// <summary>
    /// Represents a Background image
    /// </summary>
    public class ZuneDesktopBackgroundImage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ZuneDesktopBackgroundImage"/> class.
        /// </summary>
        public ZuneDesktopBackgroundImage()
        {
            IsNone = true;
            Name = "None";
            Path = null;
            YAlignment = AlignmentY.Bottom;
            Stretch = Stretch.Uniform;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZuneDesktopBackgroundImage"/> class.
        /// </summary>
        /// <param name="name">The name of the image.</param>
        /// <param name="alignment">The Y alignment of the image.</param>
        /// <param name="stretch">The stretch of the image.</param>
        public ZuneDesktopBackgroundImage(string name, AlignmentY alignment = AlignmentY.Bottom, Stretch stretch = Stretch.UniformToFill)
        {
            Path = new Uri("ms-appx:///Assets/Shells/Zune.Desktop.4.8/Backgrounds/" + name + ".png");
            Name = name;
            YAlignment = alignment;
            Stretch = stretch;
            IsNone = false;
        }

        /// <summary>
        /// The file path of the image.
        /// </summary>
        public Uri? Path { get; set; }

        /// <summary>
        /// The name of the image.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The YAlignment of the image.
        /// </summary>
        public AlignmentY YAlignment { get; set; }

        /// <summary>
        /// The Stretch of the image.
        /// </summary>
        public Stretch Stretch { get; set; }

        /// <summary>
        /// Indicates if the ZuneBackgroundImage represents none.
        /// </summary>
        public bool IsNone { get; set; }
    }
}
