using Microsoft.AspNetCore.Hosting;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;


namespace Landing.Infrastructure.Services
{
    public class ImageCompressionService
    {
        private readonly IWebHostEnvironment _env;

        public ImageCompressionService(IWebHostEnvironment env)
        {
            _env = env;
        }

        private static readonly (string folder, int width, int height)[] Sizes =
        {
            ("large", 512, 380),
            ("medium", 256, 190),
            ("small", 128, 95)
        };

        public async Task<Dictionary<string, string>> SaveCompressedVersionsAsync(Stream imageStream, string fileName)
        {
            var uploadsRoot = Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadsRoot);

            // Уникальное имя, чтобы избежать коллизий
            var baseFileName = $"{Guid.NewGuid()}{Path.GetExtension(fileName)}";
            var savedPaths = new Dictionary<string, string>();

            imageStream.Position = 0;

            using var image = await Image.LoadAsync(imageStream);

            foreach (var (folder, width, height) in Sizes)
            {
                var outputDir = Path.Combine(uploadsRoot, folder);
                Directory.CreateDirectory(outputDir);
                var outputPath = Path.Combine(outputDir, baseFileName);

                imageStream.Position = 0; // сбрасываем поток
                using var resizedImage = await Image.LoadAsync(imageStream);
                resizedImage.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(width, height),
                    Mode = ResizeMode.Max
                }));

                await resizedImage.SaveAsJpegAsync(outputPath, new JpegEncoder { Quality = 85 });
                savedPaths[folder] = $"/uploads/{folder}/{baseFileName}";
            }


            // Можно также сохранить оригинал
            var originalDir = Path.Combine(uploadsRoot, "original");
            Directory.CreateDirectory(originalDir);
            var originalPath = Path.Combine(originalDir, baseFileName);
            imageStream.Position = 0;
            using var originalImage = await Image.LoadAsync(imageStream);
            await originalImage.SaveAsJpegAsync(originalPath, new JpegEncoder { Quality = 90 });

            savedPaths["original"] = $"/uploads/original/{baseFileName}";

            return savedPaths;
        }
    }
}
