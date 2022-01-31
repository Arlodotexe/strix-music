namespace StrixMusic.Sdk.FileMetadata.Scanners
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

            // Mirrors the [SupportedMimeType] attribute that TagLib puts on each type.
            TagLib.File.AddFileTypeResolver((abstr, mimeType, readStyle) => mimeType switch
            {
                // Aac
                "taglib/aac" => new TagLib.Aac.File(abstr, readStyle),
                "audio/aac" => new TagLib.Aac.File(abstr, readStyle),

                // Aiff
                "taglib/aif" => new TagLib.Aiff.File(abstr, readStyle),
                "taglib/aiff" => new TagLib.Aiff.File(abstr, readStyle),
                "audio/x-aiff" => new TagLib.Aiff.File(abstr, readStyle),
                "audio/aiff" => new TagLib.Aiff.File(abstr, readStyle),
                "sound/aiff" => new TagLib.Aiff.File(abstr, readStyle),
                "application/x-aiff" => new TagLib.Aiff.File(abstr, readStyle),

                // Ape
                "taglib/ape" => new TagLib.Ape.File(abstr, readStyle),
                "audio/x-ape" => new TagLib.Ape.File(abstr, readStyle),
                "audio/ape" => new TagLib.Ape.File(abstr, readStyle),
                "application/x-ape" => new TagLib.Ape.File(abstr, readStyle),

                // Asf
                "taglib/wma" => new TagLib.Asf.File(abstr, readStyle),
                "taglib/wmv" => new TagLib.Asf.File(abstr, readStyle),
                "taglib/asf" => new TagLib.Asf.File(abstr, readStyle),
                "audio/x-ms-wma" => new TagLib.Asf.File(abstr, readStyle),
                "audio/x-ms-asf" => new TagLib.Asf.File(abstr, readStyle),
                "video/x-ms-asf" => new TagLib.Asf.File(abstr, readStyle),

                // Audible
                "taglib/aa" => new TagLib.Audible.File(abstr, readStyle),
                "taglib/aax" => new TagLib.Audible.File(abstr, readStyle),
                "audio/x-audible" => new TagLib.Audible.File(abstr, readStyle),

                // Dsf
                "taglib/dsf" => new TagLib.Dsf.File(abstr, readStyle),
                "audio/x-dsf" => new TagLib.Dsf.File(abstr, readStyle),
                "audio/dsf" => new TagLib.Dsf.File(abstr, readStyle),
                "sound/dsf" => new TagLib.Dsf.File(abstr, readStyle),
                "application/x-dsf" => new TagLib.Dsf.File(abstr, readStyle),

                // Flac
                "taglib/flac" => new TagLib.Flac.File(abstr, readStyle),
                "audio/x-flac" => new TagLib.Flac.File(abstr, readStyle),
                "application/x-flac" => new TagLib.Flac.File(abstr, readStyle),
                "audio/flac" => new TagLib.Flac.File(abstr, readStyle),

                // gif
                "taglib/gif" => new TagLib.Gif.File(abstr, readStyle),
                "image/gif" => new TagLib.Gif.File(abstr, readStyle),

                // Image.NoMetadata
                "taglib/bmp" => new TagLib.Image.NoMetadata.File(abstr, readStyle),
                "image/x-MS-bmp" => new TagLib.Image.NoMetadata.File(abstr, readStyle),
                "image/x-bmp" => new TagLib.Image.NoMetadata.File(abstr, readStyle),
                "taglib/ppm" => new TagLib.Image.NoMetadata.File(abstr, readStyle),
                "taglib/pgm" => new TagLib.Image.NoMetadata.File(abstr, readStyle),
                "taglib/pbm" => new TagLib.Image.NoMetadata.File(abstr, readStyle),
                "taglib/pnm" => new TagLib.Image.NoMetadata.File(abstr, readStyle),
                "image/x-portable-pixmap" => new TagLib.Image.NoMetadata.File(abstr, readStyle),
                "image/x-portable-graymap" => new TagLib.Image.NoMetadata.File(abstr, readStyle),
                "image/x-portable-bitmap" => new TagLib.Image.NoMetadata.File(abstr, readStyle),
                "image/x-portable-anymap" => new TagLib.Image.NoMetadata.File(abstr, readStyle),
                "taglib/pcx" => new TagLib.Image.NoMetadata.File(abstr, readStyle),
                "taglib/svg" => new TagLib.Image.NoMetadata.File(abstr, readStyle),
                "taglib/svgz" => new TagLib.Image.NoMetadata.File(abstr, readStyle),
                "image/x-pcx" => new TagLib.Image.NoMetadata.File(abstr, readStyle),
                "image/svg+xml" => new TagLib.Image.NoMetadata.File(abstr, readStyle),
                "taglib/kdc" => new TagLib.Image.NoMetadata.File(abstr, readStyle),    // FIXME: Not supported yet!
                "taglib/orf" => new TagLib.Image.NoMetadata.File(abstr, readStyle),    // FIXME: Not supported yet!
                "taglib/srf" => new TagLib.Image.NoMetadata.File(abstr, readStyle),    // FIXME: Not supported yet!
                "taglib/crw" => new TagLib.Image.NoMetadata.File(abstr, readStyle),    // FIXME: Not supported yet!
                "taglib/mrw" => new TagLib.Image.NoMetadata.File(abstr, readStyle),    // FIXME: Not supported yet!
                "taglib/raf" => new TagLib.Image.NoMetadata.File(abstr, readStyle),    // FIXME: Not supported yet!
                "taglib/x3f" => new TagLib.Image.NoMetadata.File(abstr, readStyle),    // FIXME: Not supported yet!

                // Jpeg
                "image/jpeg" => new TagLib.Jpeg.File(abstr, readStyle),
                "taglib/jpg" => new TagLib.Jpeg.File(abstr, readStyle),
                "taglib/jpeg" => new TagLib.Jpeg.File(abstr, readStyle),
                "taglib/jpe" => new TagLib.Jpeg.File(abstr, readStyle),
                "taglib/jif" => new TagLib.Jpeg.File(abstr, readStyle),
                "taglib/jfif" => new TagLib.Jpeg.File(abstr, readStyle),
                "taglib/jfi" => new TagLib.Jpeg.File(abstr, readStyle),

                // Matroska
                "taglib/mkv" => new TagLib.Matroska.File(abstr, readStyle),
                "taglib/mka" => new TagLib.Matroska.File(abstr, readStyle),
                "taglib/mks" => new TagLib.Matroska.File(abstr, readStyle),
                "taglib/webm" => new TagLib.Matroska.File(abstr, readStyle),
                "video/webm" => new TagLib.Matroska.File(abstr, readStyle),
                "video/x-matroska" => new TagLib.Matroska.File(abstr, readStyle),

                // MusePack
                "taglib/mpc" => new TagLib.MusePack.File(abstr, readStyle),
                "taglib/mp+" => new TagLib.MusePack.File(abstr, readStyle),
                "taglib/mpp" => new TagLib.MusePack.File(abstr, readStyle),
                "audio/x-musepack" => new TagLib.MusePack.File(abstr, readStyle),

                // Mpeg
                "taglib/mp3" => new TagLib.Mpeg.AudioFile(abstr, readStyle),
                "audio/x-mp3" => new TagLib.Mpeg.AudioFile(abstr, readStyle),
                "application/x-id3" => new TagLib.Mpeg.AudioFile(abstr, readStyle),
                "audio/mpeg" => new TagLib.Mpeg.AudioFile(abstr, readStyle),
                "audio/x-mpeg" => new TagLib.Mpeg.AudioFile(abstr, readStyle),
                "audio/x-mpeg-3" => new TagLib.Mpeg.AudioFile(abstr, readStyle),
                "audio/mpeg3" => new TagLib.Mpeg.AudioFile(abstr, readStyle),
                "audio/mp3" => new TagLib.Mpeg.AudioFile(abstr, readStyle),
                "taglib/m2a" => new TagLib.Mpeg.AudioFile(abstr, readStyle),
                "taglib/mp2" => new TagLib.Mpeg.AudioFile(abstr, readStyle),
                "taglib/mp1" => new TagLib.Mpeg.AudioFile(abstr, readStyle),
                "audio/x-mp2" => new TagLib.Mpeg.AudioFile(abstr, readStyle),
                "audio/x-mp1" => new TagLib.Mpeg.AudioFile(abstr, readStyle),

                "taglib/mpg" => new TagLib.Mpeg.File(abstr, readStyle),
                "taglib/mpeg" => new TagLib.Mpeg.File(abstr, readStyle),
                "taglib/mpe" => new TagLib.Mpeg.File(abstr, readStyle),
                "taglib/mpv2" => new TagLib.Mpeg.File(abstr, readStyle),
                "taglib/m2v" => new TagLib.Mpeg.File(abstr, readStyle),
                "video/x-mpg" => new TagLib.Mpeg.File(abstr, readStyle),
                "video/mpeg" => new TagLib.Mpeg.File(abstr, readStyle),

                // Mpeg4
                "taglib/m4a" => new TagLib.Mpeg4.File(abstr, readStyle),
                "taglib/m4b" => new TagLib.Mpeg4.File(abstr, readStyle),
                "taglib/m4v" => new TagLib.Mpeg4.File(abstr, readStyle),
                "taglib/m4p" => new TagLib.Mpeg4.File(abstr, readStyle),
                "audio/mp4" => new TagLib.Mpeg4.File(abstr, readStyle),
                "audio/x-m4a" => new TagLib.Mpeg4.File(abstr, readStyle),
                "video/mp4" => new TagLib.Mpeg4.File(abstr, readStyle),
                "video/x-m4v" => new TagLib.Mpeg4.File(abstr, readStyle),

                // Ogg
                "taglib/ogg" => new TagLib.Ogg.File(abstr, readStyle),
                "taglib/oga" => new TagLib.Ogg.File(abstr, readStyle),
                "taglib/ogv" => new TagLib.Ogg.File(abstr, readStyle),
                "taglib/opus" => new TagLib.Ogg.File(abstr, readStyle),
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
                "taglib/png" => new TagLib.Png.File(abstr, readStyle),
                "image/png" => new TagLib.Png.File(abstr, readStyle),

                // Riff
                "taglib/avi" => new TagLib.Riff.File(abstr, readStyle),
                "taglib/wav" => new TagLib.Riff.File(abstr, readStyle),
                "taglib / divx" => new TagLib.Riff.File(abstr, readStyle),
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
                "taglib/arw" => new TagLib.Tiff.Arw.File(abstr, readStyle),
                "image/arw" => new TagLib.Tiff.Arw.File(abstr, readStyle),
                "image/x-sony-arw" => new TagLib.Tiff.Arw.File(abstr, readStyle),

                // Tiff.Cr2
                "taglib/cr2" => new TagLib.Tiff.Cr2.File(abstr, readStyle),
                "image/cr2" => new TagLib.Tiff.Cr2.File(abstr, readStyle),
                "image/x-cannon-cr2" => new TagLib.Tiff.Cr2.File(abstr, readStyle),

                // Tiff.Dng
                "taglib/dng" => new TagLib.Tiff.Dng.File(abstr, readStyle),
                "image/dng" => new TagLib.Tiff.Dng.File(abstr, readStyle),
                "image/x-adobe-dng" => new TagLib.Tiff.Dng.File(abstr, readStyle),

                // Tiff.Nef
                "taglib/nef" => new TagLib.Tiff.Nef.File(abstr, readStyle),
                "image/nef" => new TagLib.Tiff.Nef.File(abstr, readStyle),
                "image/x-nikon-nef" => new TagLib.Tiff.Nef.File(abstr, readStyle),

                // Tiff.Pef
                "taglib/pef" => new TagLib.Tiff.Pef.File(abstr, readStyle),
                "image/pef" => new TagLib.Tiff.Pef.File(abstr, readStyle),
                "image/x-pentax-pef" => new TagLib.Tiff.Pef.File(abstr, readStyle),

                // Tiff.Rw2
                "taglib/rw2" => new TagLib.Tiff.Rw2.File(abstr, readStyle),
                "image/rw2" => new TagLib.Tiff.Rw2.File(abstr, readStyle),
                "image/raw" => new TagLib.Tiff.Rw2.File(abstr, readStyle),
                "taglib/raw" => new TagLib.Tiff.Rw2.File(abstr, readStyle),
                "image/x-raw" => new TagLib.Tiff.Rw2.File(abstr, readStyle),
                "image/x-panasonic-raw" => new TagLib.Tiff.Rw2.File(abstr, readStyle),

                // Tiff
                "taglib/tiff" => new TagLib.Tiff.File(abstr, readStyle),
                "image/tiff" => new TagLib.Tiff.File(abstr, readStyle),

                // WavPack
                "taglib/wv" => new TagLib.WavPack.File(abstr, readStyle),
                "audio/x-wavpack" => new TagLib.WavPack.File(abstr, readStyle),

                _ => throw new System.NotSupportedException($"{mimeType} not supported or mapped."),
            });
        }
    }
}
