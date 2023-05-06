using System;
using System.Collections.Generic;
using TagLib;

namespace StrixMusic.Cores.Storage.FileMetadata.Scanners;

/// <summary>
/// Miscellaneous helpers for TagLibSharp.
/// </summary>
internal static class TagLibHelper
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
        File.AddFileTypeResolver((abstr, mimeType, readStyle) => TagLibFileFactory.TryGetValue(mimeType, out var factory) 
                ? factory(abstr, readStyle)
                : throw new NotSupportedException($"{mimeType} not supported or mapped.")
        );
    }

    /// <summary>
    /// A key-value pair. Keys are supported mime types, values are a factory to create a TagLib file.
    /// </summary>
    public static Dictionary<string, Func<File.IFileAbstraction, ReadStyle, File>> TagLibFileFactory { get; } = new()
    {
        // Aac
        { "taglib/aac", (abstr, readStyle) => new TagLib.Aac.File(abstr, readStyle) },
        { "audio/aac", (abstr, readStyle) => new TagLib.Aac.File(abstr, readStyle) },

        // Aiff
        { "taglib/aif", (abstr, readStyle) => new TagLib.Aiff.File(abstr, readStyle) },
        { "taglib/aiff", (abstr, readStyle) => new TagLib.Aiff.File(abstr, readStyle) },
        { "audio/x-aiff", (abstr, readStyle) => new TagLib.Aiff.File(abstr, readStyle) },
        { "audio/aiff", (abstr, readStyle) => new TagLib.Aiff.File(abstr, readStyle) },
        { "sound/aiff", (abstr, readStyle) => new TagLib.Aiff.File(abstr, readStyle) },
        { "application/x-aiff", (abstr, readStyle) => new TagLib.Aiff.File(abstr, readStyle) },

        // Ape
        { "taglib/ape", (abstr, readStyle) => new TagLib.Ape.File(abstr, readStyle) },
        { "audio/x-ape", (abstr, readStyle) => new TagLib.Ape.File(abstr, readStyle) },
        { "audio/ape", (abstr, readStyle) => new TagLib.Ape.File(abstr, readStyle) },
        { "application/x-ape", (abstr, readStyle) => new TagLib.Ape.File(abstr, readStyle) },

        // Asf
        { "taglib/wma", (abstr, readStyle) => new TagLib.Asf.File(abstr, readStyle) },
        { "taglib/wmv", (abstr, readStyle) => new TagLib.Asf.File(abstr, readStyle) },
        { "taglib/asf", (abstr, readStyle) => new TagLib.Asf.File(abstr, readStyle) },
        { "audio/x-ms-wma", (abstr, readStyle) => new TagLib.Asf.File(abstr, readStyle) },
        { "audio/x-ms-asf", (abstr, readStyle) => new TagLib.Asf.File(abstr, readStyle) },
        { "video/x-ms-asf", (abstr, readStyle) => new TagLib.Asf.File(abstr, readStyle) },

        // Audible
        { "taglib/aa", (abstr, readStyle) => new TagLib.Audible.File(abstr, readStyle) },
        { "taglib/aax", (abstr, readStyle) => new TagLib.Audible.File(abstr, readStyle) },
        { "audio/x-audible", (abstr, readStyle) => new TagLib.Audible.File(abstr, readStyle) },

        // Dsf
        { "taglib/dsf", (abstr, readStyle) => new TagLib.Dsf.File(abstr, readStyle) },
        { "audio/x-dsf", (abstr, readStyle) => new TagLib.Dsf.File(abstr, readStyle) },
        { "audio/dsf", (abstr, readStyle) => new TagLib.Dsf.File(abstr, readStyle) },
        { "sound/dsf", (abstr, readStyle) => new TagLib.Dsf.File(abstr, readStyle) },
        { "application/x-dsf", (abstr, readStyle) => new TagLib.Dsf.File(abstr, readStyle) },

        // Flac
        { "taglib/flac", (abstr, readStyle) => new TagLib.Flac.File(abstr, readStyle) },
        { "audio/x-flac", (abstr, readStyle) => new TagLib.Flac.File(abstr, readStyle) },
        { "application/x-flac", (abstr, readStyle) => new TagLib.Flac.File(abstr, readStyle) },
        { "audio/flac", (abstr, readStyle) => new TagLib.Flac.File(abstr, readStyle) },

        // gif
        { "taglib/gif", (abstr, readStyle) => new TagLib.Gif.File(abstr, readStyle) },
        { "image/gif", (abstr, readStyle) => new TagLib.Gif.File(abstr, readStyle) },

        // Image.NoMetadata
        { "taglib/bmp", (abstr, readStyle) => new TagLib.Image.NoMetadata.File(abstr, readStyle) },
        { "image/x-MS-bmp", (abstr, readStyle) => new TagLib.Image.NoMetadata.File(abstr, readStyle) },
        { "image/x-bmp", (abstr, readStyle) => new TagLib.Image.NoMetadata.File(abstr, readStyle) },
        { "taglib/ppm", (abstr, readStyle) => new TagLib.Image.NoMetadata.File(abstr, readStyle) },
        { "taglib/pgm", (abstr, readStyle) => new TagLib.Image.NoMetadata.File(abstr, readStyle) },
        { "taglib/pbm", (abstr, readStyle) => new TagLib.Image.NoMetadata.File(abstr, readStyle) },
        { "taglib/pnm", (abstr, readStyle) => new TagLib.Image.NoMetadata.File(abstr, readStyle) },
        { "image/x-portable-pixmap", (abstr, readStyle) => new TagLib.Image.NoMetadata.File(abstr, readStyle) },
        { "image/x-portable-graymap", (abstr, readStyle) => new TagLib.Image.NoMetadata.File(abstr, readStyle) },
        { "image/x-portable-bitmap", (abstr, readStyle) => new TagLib.Image.NoMetadata.File(abstr, readStyle) },
        { "image/x-portable-anymap", (abstr, readStyle) => new TagLib.Image.NoMetadata.File(abstr, readStyle) },
        { "taglib/pcx", (abstr, readStyle) => new TagLib.Image.NoMetadata.File(abstr, readStyle) },
        { "taglib/svg", (abstr, readStyle) => new TagLib.Image.NoMetadata.File(abstr, readStyle) },
        { "taglib/svgz", (abstr, readStyle) => new TagLib.Image.NoMetadata.File(abstr, readStyle) },
        { "image/x-pcx", (abstr, readStyle) => new TagLib.Image.NoMetadata.File(abstr, readStyle) },
        { "image/svg+xml", (abstr, readStyle) => new TagLib.Image.NoMetadata.File(abstr, readStyle) },
/*        "taglib/kdc" => new TagLib.Image.NoMetadata.File(abstr, readStyle), // FIXME: Not supported yet!
        "taglib/orf" => new TagLib.Image.NoMetadata.File(abstr, readStyle), // FIXME: Not supported yet!
        "taglib/srf" => new TagLib.Image.NoMetadata.File(abstr, readStyle), // FIXME: Not supported yet!
        "taglib/crw" => new TagLib.Image.NoMetadata.File(abstr, readStyle), // FIXME: Not supported yet!
        "taglib/mrw" => new TagLib.Image.NoMetadata.File(abstr, readStyle), // FIXME: Not supported yet!
        "taglib/raf" => new TagLib.Image.NoMetadata.File(abstr, readStyle), // FIXME: Not supported yet!
        "taglib/x3f" => new TagLib.Image.NoMetadata.File(abstr, readStyle), // FIXME: Not supported yet!*/

        // Jpeg
        { "image/jpeg", (abstr, readStyle) => new TagLib.Jpeg.File(abstr, readStyle) },
        { "taglib/jpg", (abstr, readStyle) => new TagLib.Jpeg.File(abstr, readStyle) },
        { "taglib/jpeg", (abstr, readStyle) => new TagLib.Jpeg.File(abstr, readStyle) },
        { "taglib/jpe", (abstr, readStyle) => new TagLib.Jpeg.File(abstr, readStyle) },
        { "taglib/jif", (abstr, readStyle) => new TagLib.Jpeg.File(abstr, readStyle) },
        { "taglib/jfif", (abstr, readStyle) => new TagLib.Jpeg.File(abstr, readStyle) },
        { "taglib/jfi", (abstr, readStyle) => new TagLib.Jpeg.File(abstr, readStyle) },

        // Matroska
        { "taglib/mkv", (abstr, readStyle) => new TagLib.Matroska.File(abstr, readStyle) },
        { "taglib/mka", (abstr, readStyle) => new TagLib.Matroska.File(abstr, readStyle) },
        { "taglib/mks", (abstr, readStyle) => new TagLib.Matroska.File(abstr, readStyle) },
        { "taglib/webm", (abstr, readStyle) => new TagLib.Matroska.File(abstr, readStyle) },
        { "video/webm", (abstr, readStyle) => new TagLib.Matroska.File(abstr, readStyle) },
        { "video/x-matroska", (abstr, readStyle) => new TagLib.Matroska.File(abstr, readStyle) },

        // MusePack
        { "taglib/mpc", (abstr, readStyle) => new TagLib.MusePack.File(abstr, readStyle) },
        { "taglib/mp+", (abstr, readStyle) => new TagLib.MusePack.File(abstr, readStyle) },
        { "taglib/mpp", (abstr, readStyle) => new TagLib.MusePack.File(abstr, readStyle) },
        { "audio/x-musepack", (abstr, readStyle) => new TagLib.MusePack.File(abstr, readStyle) },

        // Mpeg
        { "taglib/mp3", (abstr, readStyle) => new TagLib.Mpeg.AudioFile(abstr, readStyle) },
        { "audio/x-mp3", (abstr, readStyle) => new TagLib.Mpeg.AudioFile(abstr, readStyle) },
        { "application/x-id3", (abstr, readStyle) => new TagLib.Mpeg.AudioFile(abstr, readStyle) },
        { "audio/mpeg", (abstr, readStyle) => new TagLib.Mpeg.AudioFile(abstr, readStyle) },
        { "audio/x-mpeg", (abstr, readStyle) => new TagLib.Mpeg.AudioFile(abstr, readStyle) },
        { "audio/x-mpeg-3", (abstr, readStyle) => new TagLib.Mpeg.AudioFile(abstr, readStyle) },
        { "audio/mpeg3", (abstr, readStyle) => new TagLib.Mpeg.AudioFile(abstr, readStyle) },
        { "audio/mp3", (abstr, readStyle) => new TagLib.Mpeg.AudioFile(abstr, readStyle) },
        { "taglib/m2a", (abstr, readStyle) => new TagLib.Mpeg.AudioFile(abstr, readStyle) },
        { "taglib/mp2", (abstr, readStyle) => new TagLib.Mpeg.AudioFile(abstr, readStyle) },
        { "taglib/mp1", (abstr, readStyle) => new TagLib.Mpeg.AudioFile(abstr, readStyle) },
        { "audio/x-mp2", (abstr, readStyle) => new TagLib.Mpeg.AudioFile(abstr, readStyle) },
        { "audio/x-mp1", (abstr, readStyle) => new TagLib.Mpeg.AudioFile(abstr, readStyle) },

        { "taglib/mpg", (abstr, readStyle) => new TagLib.Mpeg.File(abstr, readStyle) },
        { "taglib/mpeg", (abstr, readStyle) => new TagLib.Mpeg.File(abstr, readStyle) },
        { "taglib/mpe", (abstr, readStyle) => new TagLib.Mpeg.File(abstr, readStyle) },
        { "taglib/mpv2", (abstr, readStyle) => new TagLib.Mpeg.File(abstr, readStyle) },
        { "taglib/m2v", (abstr, readStyle) => new TagLib.Mpeg.File(abstr, readStyle) },
        { "video/x-mpg", (abstr, readStyle) => new TagLib.Mpeg.File(abstr, readStyle) },
        { "video/mpeg", (abstr, readStyle) => new TagLib.Mpeg.File(abstr, readStyle) },

        // Mpeg4
        { "taglib/m4a", (abstr, readStyle) => new TagLib.Mpeg4.File(abstr, readStyle) },
        { "taglib/m4b", (abstr, readStyle) => new TagLib.Mpeg4.File(abstr, readStyle) },
        { "taglib/m4v", (abstr, readStyle) => new TagLib.Mpeg4.File(abstr, readStyle) },
        { "taglib/m4p", (abstr, readStyle) => new TagLib.Mpeg4.File(abstr, readStyle) },
        { "audio/mp4", (abstr, readStyle) => new TagLib.Mpeg4.File(abstr, readStyle) },
        { "audio/x-m4a", (abstr, readStyle) => new TagLib.Mpeg4.File(abstr, readStyle) },
        { "video/mp4", (abstr, readStyle) => new TagLib.Mpeg4.File(abstr, readStyle) },
        { "video/x-m4v", (abstr, readStyle) => new TagLib.Mpeg4.File(abstr, readStyle) },

        // Ogg
        { "taglib/ogg", (abstr, readStyle) => new TagLib.Ogg.File(abstr, readStyle) },
        { "taglib/oga", (abstr, readStyle) => new TagLib.Ogg.File(abstr, readStyle) },
        { "taglib/ogv", (abstr, readStyle) => new TagLib.Ogg.File(abstr, readStyle) },
        { "taglib/opus", (abstr, readStyle) => new TagLib.Ogg.File(abstr, readStyle) },
        { "application/x-ogg", (abstr, readStyle) => new TagLib.Ogg.File(abstr, readStyle) },
        { "audio/vorbis", (abstr, readStyle) => new TagLib.Ogg.File(abstr, readStyle) },
        { "audio/x-vorbis", (abstr, readStyle) => new TagLib.Ogg.File(abstr, readStyle) },
        { "audio/x-vorbis+ogg", (abstr, readStyle) => new TagLib.Ogg.File(abstr, readStyle) },
        { "audio/ogg", (abstr, readStyle) => new TagLib.Ogg.File(abstr, readStyle) },
        { "audio/x-ogg", (abstr, readStyle) => new TagLib.Ogg.File(abstr, readStyle) },
        { "video/ogg", (abstr, readStyle) => new TagLib.Ogg.File(abstr, readStyle) },
        { "video/x-ogm+ogg", (abstr, readStyle) => new TagLib.Ogg.File(abstr, readStyle) },
        { "video/x-theora+ogg", (abstr, readStyle) => new TagLib.Ogg.File(abstr, readStyle) },
        { "video/x-theora", (abstr, readStyle) => new TagLib.Ogg.File(abstr, readStyle) },
        { "audio/opus", (abstr, readStyle) => new TagLib.Ogg.File(abstr, readStyle) },
        { "audio/x-opus", (abstr, readStyle) => new TagLib.Ogg.File(abstr, readStyle) },
        { "audio/x-opus+ogg", (abstr, readStyle) => new TagLib.Ogg.File(abstr, readStyle) },

        // Png
        { "taglib/png", (abstr, readStyle) => new TagLib.Png.File(abstr, readStyle) },
        { "image/png", (abstr, readStyle) => new TagLib.Png.File(abstr, readStyle) },

        // Riff
        { "taglib/avi", (abstr, readStyle) => new TagLib.Riff.File(abstr, readStyle) },
        { "taglib/wav", (abstr, readStyle) => new TagLib.Riff.File(abstr, readStyle) },
        { "taglib / divx", (abstr, readStyle) => new TagLib.Riff.File(abstr, readStyle) },
        { "video/avi", (abstr, readStyle) => new TagLib.Riff.File(abstr, readStyle) },
        { "video/msvideo", (abstr, readStyle) => new TagLib.Riff.File(abstr, readStyle) },
        { "video/x-msvideo", (abstr, readStyle) => new TagLib.Riff.File(abstr, readStyle) },
        { "image/avi", (abstr, readStyle) => new TagLib.Riff.File(abstr, readStyle) },
        { "application/x-troff-msvideo", (abstr, readStyle) => new TagLib.Riff.File(abstr, readStyle) },
        { "audio/avi", (abstr, readStyle) => new TagLib.Riff.File(abstr, readStyle) },
        { "audio/wav", (abstr, readStyle) => new TagLib.Riff.File(abstr, readStyle) },
        { "audio/wave", (abstr, readStyle) => new TagLib.Riff.File(abstr, readStyle) },
        { "audio/x-wav", (abstr, readStyle) => new TagLib.Riff.File(abstr, readStyle) },

        // Tiff.Arw
        { "taglib/arw", (abstr, readStyle) => new TagLib.Tiff.Arw.File(abstr, readStyle) },
        { "image/arw", (abstr, readStyle) => new TagLib.Tiff.Arw.File(abstr, readStyle) },
        { "image/x-sony-arw", (abstr, readStyle) => new TagLib.Tiff.Arw.File(abstr, readStyle) },

        // Tiff.Cr2
        { "taglib/cr2", (abstr, readStyle) => new TagLib.Tiff.Cr2.File(abstr, readStyle) },
        { "image/cr2", (abstr, readStyle) => new TagLib.Tiff.Cr2.File(abstr, readStyle) },
        { "image/x-cannon-cr2", (abstr, readStyle) => new TagLib.Tiff.Cr2.File(abstr, readStyle) },

        // Tiff.Dng
        { "taglib/dng", (abstr, readStyle) => new TagLib.Tiff.Dng.File(abstr, readStyle) },
        { "image/dng", (abstr, readStyle) => new TagLib.Tiff.Dng.File(abstr, readStyle) },
        { "image/x-adobe-dng", (abstr, readStyle) => new TagLib.Tiff.Dng.File(abstr, readStyle) },

        // Tiff.Nef
        { "taglib/nef", (abstr, readStyle) => new TagLib.Tiff.Nef.File(abstr, readStyle) },
        { "image/nef", (abstr, readStyle) => new TagLib.Tiff.Nef.File(abstr, readStyle) },
        { "image/x-nikon-nef", (abstr, readStyle) => new TagLib.Tiff.Nef.File(abstr, readStyle) },

        // Tiff.Pef
        { "taglib/pef", (abstr, readStyle) => new TagLib.Tiff.Pef.File(abstr, readStyle) },
        { "image/pef", (abstr, readStyle) => new TagLib.Tiff.Pef.File(abstr, readStyle) },
        { "image/x-pentax-pef", (abstr, readStyle) => new TagLib.Tiff.Pef.File(abstr, readStyle) },

        // Tiff.Rw2
        { "taglib/rw2", (abstr, readStyle) => new TagLib.Tiff.Rw2.File(abstr, readStyle) },
        { "image/rw2", (abstr, readStyle) => new TagLib.Tiff.Rw2.File(abstr, readStyle) },
        { "image/raw", (abstr, readStyle) => new TagLib.Tiff.Rw2.File(abstr, readStyle) },
        { "taglib/raw", (abstr, readStyle) => new TagLib.Tiff.Rw2.File(abstr, readStyle) },
        { "image/x-raw", (abstr, readStyle) => new TagLib.Tiff.Rw2.File(abstr, readStyle) },
        { "image/x-panasonic-raw", (abstr, readStyle) => new TagLib.Tiff.Rw2.File(abstr, readStyle) },

        // Tiff
        { "taglib/tiff", (abstr, readStyle) => new TagLib.Tiff.File(abstr, readStyle) },
        { "image/tiff", (abstr, readStyle) => new TagLib.Tiff.File(abstr, readStyle) },

        // WavPack
        { "taglib/wv", (abstr, readStyle) => new TagLib.WavPack.File(abstr, readStyle) },
        { "audio/x-wavpack", (abstr, readStyle) => new TagLib.WavPack.File(abstr, readStyle) },
    };
}
