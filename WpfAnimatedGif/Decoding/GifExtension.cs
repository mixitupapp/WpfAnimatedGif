using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace WpfAnimatedGif.Decoding
{
    internal abstract class GifExtension : GifBlock
    {
        internal const int ExtensionIntroducer = 0x21;

        internal static async Task<GifExtension> ReadExtensionAsync(Stream stream, IEnumerable<GifExtension> controlExtensions, bool metadataOnly)
        {
            // Note: at this point, the Extension Introducer (0x21) has already been read

            int label = stream.ReadByte();
            if (label < 0)
                throw GifHelpers.UnexpectedEndOfStreamException();
            switch (label)
            {
                case GifGraphicControlExtension.ExtensionLabel:
                    return await GifGraphicControlExtension.ReadGraphicsControlAsync(stream);
                case GifCommentExtension.ExtensionLabel:
                    return await GifCommentExtension.ReadCommentAsync(stream);
                case GifPlainTextExtension.ExtensionLabel:
                    return await GifPlainTextExtension.ReadPlainTextAsync(stream, controlExtensions, metadataOnly);
                case GifApplicationExtension.ExtensionLabel:
                    return await GifApplicationExtension.ReadApplicationAsync(stream);
                default:
                    throw GifHelpers.UnknownExtensionTypeException(label);
            }
        }
    }
}
