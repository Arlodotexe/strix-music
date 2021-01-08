using StrixMusic.Core.LocalFiles.Backing.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using TagLib;

namespace StrixMusic.Core.LocalFiles.MetadataScanner
{
    /// <summary>
    /// Class for pulling ID3 metadata out of media files.
    /// </summary>
    public class ID3MetadataScanner
    {
		/// <summary>
		/// Scans a media file for ID3 tags.
		/// </summary>
		/// <param name="filePath">The path to the file.</param>
        public TrackMetadata? ScanTrackMetadata(File.IFileAbstraction fileAbstraction)
        {
			try
			{
				using (var tagFile = File.Create(fileAbstraction))
				{
					// Read the raw tags
					var tags = tagFile.Tag;

					return new TrackMetadata()
					{
						Description = tags.Description,
						Title = tags.Title,
						DiscNumber = tags.Disc,
						Duration = tagFile.Properties.Duration,
						Genres = new List<string>(tags.Genres),
						TrackNumber = tags.Track,
					};
				}
			}
			catch (UnsupportedFormatException ex)
			{
				Debug.WriteLine("UNSUPPORTED FILE");
				Debug.WriteLine("---------------------------------------\r\n");
				return null;
			}
		}
    }
}
