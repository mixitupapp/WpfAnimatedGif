using System.IO;
using System.Threading.Tasks;

namespace WpfAnimatedGif.Decoding
{
    internal class GifImageData
    {
        public byte LzwMinimumCodeSize { get; set; }
        public byte[] CompressedData { get; set; }

        private GifImageData()
        {
        }

        internal static async Task<GifImageData> ReadImageDataAsync(Stream stream, bool metadataOnly)
        {
            var imgData = new GifImageData();
            await imgData.ReadAsync(stream, metadataOnly);
            return imgData;
        }

        private async Task ReadAsync(Stream stream, bool metadataOnly)
        {
            LzwMinimumCodeSize = (byte)stream.ReadByte();
            CompressedData = await GifHelpers.ReadDataBlocksAsync(stream, metadataOnly);
        }
    }
}
