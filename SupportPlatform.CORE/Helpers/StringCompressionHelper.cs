using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

namespace SupportPlatform.API.CORE.Helpers
{
    public static class StringCompressionHelper
    {
        public static async Task<string> ToBrotliAsync(this string value, CompressionLevel level = CompressionLevel.Optimal)
            => await ToCompressedStringAsync(value, s => new BrotliStream(s, level));

        private static async Task<string> ToCompressedStringAsync(
        string value,
        Func<Stream, Stream> createCompressionStream)
        {
            var bytes = Encoding.Unicode.GetBytes(value);
            await using var input = new MemoryStream(bytes);
            await using var output = new MemoryStream();
            await using var stream = createCompressionStream(output);

            await input.CopyToAsync(stream);
            await stream.FlushAsync();

            var result = output.ToArray();

            return Convert.ToBase64String(result);
        }

        private static async Task<string> FromCompressedStringAsync(string value, Func<Stream, Stream> createDecompressionStream)
        {
            var bytes = Convert.FromBase64String(value);
            await using var input = new MemoryStream(bytes);
            await using var output = new MemoryStream();
            await using var stream = createDecompressionStream(input);

            await stream.CopyToAsync(output);
            await output.FlushAsync();

            return Encoding.Unicode.GetString(output.ToArray());
        }

        public static async Task<string> FromBrotliAsync(this string value)
            => await FromCompressedStringAsync(value, s => new BrotliStream(s, CompressionMode.Decompress));
    }
}
