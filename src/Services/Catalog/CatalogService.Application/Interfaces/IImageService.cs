using System;
using System.Collections.Generic;
using System.Text;

namespace CatalogService.Application.Interfaces
{
    public interface IImageService
    {
        Task<string> UploadProductImageAsync(Stream fileStream, string originalFileName, string productName);
    }
}
