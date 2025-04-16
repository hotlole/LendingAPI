using System.ComponentModel;
namespace Landing.Core.Enums
{
    public enum ImageSize
    {
        [Description("Оригинальный размер")]
        Original = 0,

        [Description("512 x 380 пикселей")]
        Size_512x380 = 1,

        [Description("256 x 190 пикселей")]
        Size_256x190 = 2
    }

}
