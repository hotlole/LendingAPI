using Landing.Core.Enums;

namespace Landing.Core.Models
{
    public static class ImageSizeExtensions
    {
        public static (int width, int height)? GetDimensions(this ImageSize size)
        {
            return size switch
            {
                ImageSize.Size_512x380 => (512, 380),
                ImageSize.Size_256x190 => (256, 190),
                _ => null
            };
        }

        public static string ToFileSuffix(this ImageSize size)
        {
            return size switch
            {
                ImageSize.Size_512x380 => "512x380",
                ImageSize.Size_256x190 => "256x190",
                _ => "original"
            };
        }
    }
}
