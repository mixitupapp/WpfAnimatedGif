using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WpfAnimatedGif.Decoding
{
    internal class GifFile
    {
        public GifHeader Header { get; private set; }
        public GifColor[] GlobalColorTable { get; set; }
        public IList<GifFrame> Frames { get; set; }
        public IList<GifExtension> Extensions { get; set; }
        public ushort RepeatCount { get; set; }

        private GifFile()
        {
        }

        internal static async Task<GifFile> ReadGifFileAsync(Stream stream, bool metadataOnly)
        {
            var file = new GifFile();
            await file.ReadAsync(stream, metadataOnly);
            return file;
        }

        private async Task ReadAsync(Stream stream, bool metadataOnly)
        {
            Header = await GifHeader.ReadHeaderAsync(stream);

            if (Header.LogicalScreenDescriptor.HasGlobalColorTable)
            {
                GlobalColorTable = await GifHelpers.ReadColorTableAsync(stream, Header.LogicalScreenDescriptor.GlobalColorTableSize);
            }
            await ReadFramesAsync(stream, metadataOnly);

            var netscapeExtension =
                            Extensions
                                .OfType<GifApplicationExtension>()
                                .FirstOrDefault(GifHelpers.IsNetscapeExtension);

            if (netscapeExtension != null)
                RepeatCount = GifHelpers.GetRepeatCount(netscapeExtension);
            else
                RepeatCount = 1;
        }

        private async Task ReadFramesAsync(Stream stream, bool metadataOnly)
        {
            List<GifFrame> frames = new List<GifFrame>();
            List<GifExtension> controlExtensions = new List<GifExtension>();
            List<GifExtension> specialExtensions = new List<GifExtension>();
            while (true)
            {
                var block = await GifBlock.ReadBlockAsync(stream, controlExtensions, metadataOnly);

                if (block.Kind == GifBlockKind.GraphicRendering)
                    controlExtensions = new List<GifExtension>();

                if (block is GifFrame)
                {
                    frames.Add((GifFrame)block);
                }
                else if (block is GifExtension)
                {
                    var extension = (GifExtension)block;
                    switch (extension.Kind)
                    {
                        case GifBlockKind.Control:
                            controlExtensions.Add(extension);
                            break;
                        case GifBlockKind.SpecialPurpose:
                            specialExtensions.Add(extension);
                            break;
                    }
                }
                else if (block is GifTrailer)
                {
                    break;
                }
            }

            this.Frames = frames.AsReadOnly();
            this.Extensions = specialExtensions.AsReadOnly();
        }
    }
}
