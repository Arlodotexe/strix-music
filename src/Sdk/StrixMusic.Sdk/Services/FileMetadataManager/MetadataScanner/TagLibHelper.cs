using OwlCore.Validation.Mime;
using System.IO;

namespace StrixMusic.Sdk.Services.FileMetadataManager.MetadataScanner
{
    /// <summary>
    /// Miscellaneous helpers for <see cref="TagLib"/>.
    /// </summary>
    public static class TagLibHelper
    {
        private static bool _manualFileTypeResolversAdded;

        /// <summary>
        /// Adds a manual file type resolver to TagLib. Prevents it from using reflection to resolve file types.
        /// </summary>
        public static void TryAddManualFileTypeResolver()
        {
            if (_manualFileTypeResolversAdded)
                return;

            _manualFileTypeResolversAdded = true;

            // The last added resolver gets run first.
            // Resolve by file extension as fallback.
            TagLib.File.AddFileTypeResolver(ResolveByFileExtension);
            TagLib.File.AddFileTypeResolver(ResolveByMimeType);

            TagLib.File? ResolveByFileExtension(TagLib.File.IFileAbstraction abstr, string mimeType, TagLib.ReadStyle readStyle)
            {
                var extension = Path.GetExtension(abstr.Name);
                if (string.IsNullOrWhiteSpace(extension))
                    return null;

                // TODO: Use TagLib extension-to-file mapping to guaruntee compatability.
                // Not all platforms supply the actual mime type.
                // Fall back to getting mime type from the file extension.
                if (MimeTypeMap.TryGetMimeType(extension, out var detectedMimeType))
                    return ResolveByMimeType(abstr, detectedMimeType, readStyle);

                return null;
            }

            TagLib.File? ResolveByMimeType(TagLib.File.IFileAbstraction abstr, string mimeType, TagLib.ReadStyle readStyle) => mimeType switch
            {
                // Aac
                "audio/aac" => new TagLib.Aac.File(abstr, readStyle),

                // Aiff
                "audio/x-aiff" => new TagLib.Aiff.File(abstr, readStyle),
                "audio/aiff" => new TagLib.Aiff.File(abstr, readStyle),
                "sound/aiff" => new TagLib.Aiff.File(abstr, readStyle),
                "application/x-aiff" => new TagLib.Aiff.File(abstr, readStyle),

                // Ape
                "audio/x-ape" => new TagLib.Ape.File(abstr, readStyle),
                "audio/ape" => new TagLib.Ape.File(abstr, readStyle),
                "application/x-ape" => new TagLib.Ape.File(abstr, readStyle),

                // Asf
                "audio/x-ms-wma" => new TagLib.Asf.File(abstr, readStyle),
                "audio/x-ms-asf" => new TagLib.Asf.File(abstr, readStyle),
                "video/x-ms-asf" => new TagLib.Asf.File(abstr, readStyle),

                // Audible
                "audio/x-audible" => new TagLib.Audible.File(abstr, readStyle),

                // Dsf
                "audio/x-dsf" => new TagLib.Dsf.File(abstr, readStyle),
                "audio/dsf" => new TagLib.Dsf.File(abstr, readStyle),
                "sound/dsf" => new TagLib.Dsf.File(abstr, readStyle),
                "application/x-dsf" => new TagLib.Dsf.File(abstr, readStyle),

                // Flac
                "audio/x-flac" => new TagLib.Flac.File(abstr, readStyle),
                "application/x-flac" => new TagLib.Flac.File(abstr, readStyle),
                "audio/flac" => new TagLib.Flac.File(abstr, readStyle),

                // gif
                "image/gif" => new TagLib.Gif.File(abstr, readStyle),

                // Image.NoMetadata
                "image/x-MS-bmp" => new TagLib.Image.NoMetadata.File(abstr, readStyle),
                "image/x-bmp" => new TagLib.Image.NoMetadata.File(abstr, readStyle),
                "image/x-portable-pixmap" => new TagLib.Image.NoMetadata.File(abstr, readStyle),
                "image/x-portable-graymap" => new TagLib.Image.NoMetadata.File(abstr, readStyle),
                "image/x-portable-bitmap" => new TagLib.Image.NoMetadata.File(abstr, readStyle),
                "image/x-portable-anymap" => new TagLib.Image.NoMetadata.File(abstr, readStyle),
                "image/x-pcx" => new TagLib.Image.NoMetadata.File(abstr, readStyle),
                "image/svg+xml" => new TagLib.Image.NoMetadata.File(abstr, readStyle),

                // Jpeg
                "image/jpeg" => new TagLib.Jpeg.File(abstr, readStyle),

                // Matroska
                "video/webm" => new TagLib.Matroska.File(abstr, readStyle),
                "video/x-matroska" => new TagLib.Matroska.File(abstr, readStyle),

                // MusePack
                "audio/x-musepack" => new TagLib.MusePack.File(abstr, readStyle),

                // Mpeg
                "audio/x-mp3" => new TagLib.Mpeg.AudioFile(abstr, readStyle),
                "application/x-id3" => new TagLib.Mpeg.AudioFile(abstr, readStyle),
                "audio/mpeg" => new TagLib.Mpeg.AudioFile(abstr, readStyle),
                "audio/x-mpeg" => new TagLib.Mpeg.AudioFile(abstr, readStyle),
                "audio/x-mpeg-3" => new TagLib.Mpeg.AudioFile(abstr, readStyle),
                "audio/mpeg3" => new TagLib.Mpeg.AudioFile(abstr, readStyle),
                "audio/mp3" => new TagLib.Mpeg.AudioFile(abstr, readStyle),
                "audio/x-mp2" => new TagLib.Mpeg.AudioFile(abstr, readStyle),
                "audio/x-mp1" => new TagLib.Mpeg.AudioFile(abstr, readStyle),

                "video/x-mpg" => new TagLib.Mpeg.File(abstr, readStyle),
                "video/mpeg" => new TagLib.Mpeg.File(abstr, readStyle),

                // Mpeg4
                "audio/mp4" => new TagLib.Mpeg4.File(abstr, readStyle),
                "audio/x-m4a" => new TagLib.Mpeg4.File(abstr, readStyle),
                "video/mp4" => new TagLib.Mpeg4.File(abstr, readStyle),
                "video/x-m4v" => new TagLib.Mpeg4.File(abstr, readStyle),

                // Ogg
                "application/ogg" => new TagLib.Ogg.File(abstr, readStyle),
                "application/x-ogg" => new TagLib.Ogg.File(abstr, readStyle),
                "audio/vorbis" => new TagLib.Ogg.File(abstr, readStyle),
                "audio/x-vorbis" => new TagLib.Ogg.File(abstr, readStyle),
                "audio/x-vorbis+ogg" => new TagLib.Ogg.File(abstr, readStyle),
                "audio/ogg" => new TagLib.Ogg.File(abstr, readStyle),
                "audio/x-ogg" => new TagLib.Ogg.File(abstr, readStyle),
                "video/ogg" => new TagLib.Ogg.File(abstr, readStyle),
                "video/x-ogm+ogg" => new TagLib.Ogg.File(abstr, readStyle),
                "video/x-theora+ogg" => new TagLib.Ogg.File(abstr, readStyle),
                "video/x-theora" => new TagLib.Ogg.File(abstr, readStyle),
                "audio/opus" => new TagLib.Ogg.File(abstr, readStyle),
                "audio/x-opus" => new TagLib.Ogg.File(abstr, readStyle),
                "audio/x-opus+ogg" => new TagLib.Ogg.File(abstr, readStyle),

                // Png
                "image/png" => new TagLib.Png.File(abstr, readStyle),

                // Riff
                "video/avi" => new TagLib.Riff.File(abstr, readStyle),
                "video/msvideo" => new TagLib.Riff.File(abstr, readStyle),
                "video/x-msvideo" => new TagLib.Riff.File(abstr, readStyle),
                "image/avi" => new TagLib.Riff.File(abstr, readStyle),
                "application/x-troff-msvideo" => new TagLib.Riff.File(abstr, readStyle),
                "audio/avi" => new TagLib.Riff.File(abstr, readStyle),
                "audio/wav" => new TagLib.Riff.File(abstr, readStyle),
                "audio/wave" => new TagLib.Riff.File(abstr, readStyle),
                "audio/x-wav" => new TagLib.Riff.File(abstr, readStyle),

                // Tiff.Arw
                "image/arw" => new TagLib.Tiff.Arw.File(abstr, readStyle),
                "image/x-sony-arw" => new TagLib.Tiff.Arw.File(abstr, readStyle),

                // Tiff.Cr2
                "image/cr2" => new TagLib.Tiff.Cr2.File(abstr, readStyle),
                "image/x-cannon-cr2" => new TagLib.Tiff.Cr2.File(abstr, readStyle),

                // Tiff.Dng
                "image/dng" => new TagLib.Tiff.Dng.File(abstr, readStyle),
                "image/x-adobe-dng" => new TagLib.Tiff.Dng.File(abstr, readStyle),

                // Tiff.Nef
                "image/nef" => new TagLib.Tiff.Nef.File(abstr, readStyle),
                "image/x-nikon-nef" => new TagLib.Tiff.Nef.File(abstr, readStyle),

                // Tiff.Pef
                "image/pef" => new TagLib.Tiff.Pef.File(abstr, readStyle),
                "image/x-pentax-pef" => new TagLib.Tiff.Pef.File(abstr, readStyle),

                // Tiff.Rw2
                "image/rw2" => new TagLib.Tiff.Rw2.File(abstr, readStyle),
                "image/raw" => new TagLib.Tiff.Rw2.File(abstr, readStyle),
                "image/x-raw" => new TagLib.Tiff.Rw2.File(abstr, readStyle),
                "image/x-panasonic-raw" => new TagLib.Tiff.Rw2.File(abstr, readStyle),

                // Tiff
                "image/tiff" => new TagLib.Tiff.File(abstr, readStyle),

                // WavPack
                "audio/x-wavpack" => new TagLib.WavPack.File(abstr, readStyle),

                _ => throw new System.NotSupportedException($"{mimeType} not supported or mapped."),
            };
        }
    }
}
