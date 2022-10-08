using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace WpfAnimatedGif.Decoding
{
    internal abstract class GifBlock
    {
        internal static async Task<GifBlock> ReadBlockAsync(Stream stream, IEnumerable<GifExtension> controlExtensions, bool metadataOnly)
        {
            int blockId = stream.ReadByte();
            if (blockId < 0)
                throw GifHelpers.UnexpectedEndOfStreamException();
            switch (blockId)
            {
                case GifExtension.ExtensionIntroducer:
                    return await GifExtension.ReadExtensionAsync(stream, controlExtensions, metadataOnly);
                case GifFrame.ImageSeparator:
                    return await GifFrame.ReadFrameAsync(stream, controlExtensions, metadataOnly);
                case GifTrailer.TrailerByte:
                    return await GifTrailer.ReadTrailerAsync();
                default:
                    throw GifHelpers.UnknownBlockTypeException(blockId);
            }
        }

        internal abstract GifBlockKind Kind { get; }
    }
}
