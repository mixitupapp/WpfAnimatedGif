using System;
using System.IO;
using System.Threading.Tasks;

namespace WpfAnimatedGif.Decoding
{
    // label 0xF9
    internal class GifGraphicControlExtension : GifExtension
    {
        internal const int ExtensionLabel = 0xF9;

        public int BlockSize { get; private set; }
        public int DisposalMethod { get; private set; }
        public bool UserInput { get; private set; }
        public bool HasTransparency { get; private set; }
        public int Delay { get; private set; }
        public int TransparencyIndex { get; private set; }

        private GifGraphicControlExtension()
        {

        }

        internal override GifBlockKind Kind
        {
            get { return GifBlockKind.Control; }
        }

        internal static async Task<GifGraphicControlExtension> ReadGraphicsControlAsync(Stream stream)
        {
            var ext = new GifGraphicControlExtension();
            await ext.ReadAsync(stream);
            return ext;
        }

        private async Task ReadAsync(Stream stream)
        {
            // Note: at this point, the label (0xF9) has already been read

            byte[] bytes = new byte[6];
            await stream.ReadAsync(bytes, 0, bytes.Length);
            BlockSize = bytes[0]; // should always be 4
            if (BlockSize != 4)
                throw GifHelpers.InvalidBlockSizeException("Graphic Control Extension", 4, BlockSize);
            byte packedFields = bytes[1];
            DisposalMethod = (packedFields & 0x1C) >> 2;
            UserInput = (packedFields & 0x02) != 0;
            HasTransparency = (packedFields & 0x01) != 0;
            Delay = BitConverter.ToUInt16(bytes, 2) * 10; // milliseconds
            TransparencyIndex = bytes[4];
        }
    }
}
