using CatalogService.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace CatalogService.Infrastructure.Services
{
    public class ImageService : IImageService
    {
        public async Task<string> UploadProductImageAsync(Stream fileStream, string originalFileName, string productName)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(originalFileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                throw new ArgumentException("Only .jpg .jpeg .png and .webp files are allowed.");

            var imagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

            if (!Directory.Exists(imagesFolder))
                Directory.CreateDirectory(imagesFolder);

            var slug = GenerateSlug(productName);
            var uniquePart = Guid.NewGuid().ToString("N").Substring(0, 8);
            var fileName = $"{slug}-{uniquePart}{extension}";
            var filePath = Path.Combine(imagesFolder, fileName);

            using (var output = new FileStream(filePath, FileMode.Create))
            {
                await fileStream.CopyToAsync(output);
            }

            return $"images/{fileName}";
        }

        private static string GenerateSlug(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return "product-image";

            text = text.ToLowerInvariant().Trim();
            text = Regex.Replace(text, @"\s+", "-");
            text = Regex.Replace(text, @"[^a-z0-9\-]", string.Empty);
            text = Regex.Replace(text, @"-+", "-");

            return text.Trim('-');
        }
    }
}
