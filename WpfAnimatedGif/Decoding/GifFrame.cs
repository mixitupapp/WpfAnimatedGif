using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WpfAnimatedGif.Decoding
{
    internal class GifFrame : GifBlock
    {
        internal const int ImageSeparator = 0x2C;

        public GifImageDescriptor Descriptor { get; private set; }
        public GifColor[] LocalColorTable { get; private set; }
        public IList<GifExtension> Extensions { get; private set; }
        public GifImageData ImageData { get; private set; }

        private GifFrame()
        {
        }

        internal override GifBlockKind Kind
        {
            get { return GifBlockKind.GraphicRendering; }
        }

        internal static async Task<GifFrame> ReadFrameAsync(Stream stream, IEnumerable<GifExtension> controlExtensions, bool metadataOnly)
        {
            var frame = new GifFrame();

            await frame.ReadAsync(stream, controlExtensions, metadataOnly);

            return frame;
        }

        private async Task ReadAsync(Stream stream, IEnumerable<GifExtension> controlExtensions, bool metadataOnly)
        {
            // Note: at this point, the Image Separator (0x2C) has already been read

            Descriptor = await GifImageDescriptor.ReadImageDescriptorAsync(stream);
            if (Descriptor.HasLocalColorTable)
            {
                LocalColorTable = await GifHelpers.ReadColorTableAsync(stream, Descriptor.LocalColorTableSize);
            }
            ImageData = await GifImageData.ReadImageDataAsync(stream, metadataOnly);
            Extensions = controlExtensions.ToList().AsReadOnly();
        }
    }
}
