using CartService.Application.DTOs;
using CartService.Application.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace CartService.Infrastructure.Clients
{
    public class CachedCatalogServiceClient : ICatalogServiceClient
    {
        private readonly ICatalogServiceClient _innerClient;
        private readonly IMemoryCache _cache; 
        private readonly ILogger<CachedCatalogServiceClient> _logger;

        // We inject the real HTTP client and the Memory Cache
        public CachedCatalogServiceClient(ICatalogServiceClient innerClient, IMemoryCache cache, ILogger<CachedCatalogServiceClient> logger)
        {
            _innerClient = innerClient;
            _cache = cache;
            _logger = logger;
        }

        public async Task<ProductSnapshotDto?> GetProductSnapshotByIdAsync(Guid productId)
        {
            string cacheKey = $"catalog-product-{productId}";

            try
            {
                // Try the real HTTP call first
                var product = await _innerClient.GetProductSnapshotByIdAsync(productId);

                if (product != null)
                {
                    // Set cache options: evict after 24 hours max, or after 2 hours if nobody accesses it
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromHours(24))
                        .SetSlidingExpiration(TimeSpan.FromHours(2));

                    // Even if it is in the cache, overwrite it, so  fallback always has the most up to date price
                    _cache.Set(cacheKey, product, cacheEntryOptions);
                }

                return product;
            }
            catch (Exception)
            {
                // Fallback: The HTTP call failed or Shared Kernel EnsureSuccessOrThrowAsync threw an error
                // We catch the exception and look for the data in the cache
                if (_cache.TryGetValue(cacheKey, out ProductSnapshotDto? cachedProduct))
                {
                    // We found a fallback - the frontend will never know the Catalog service was down
                    _logger.LogWarning("CatalogService unavailable. Serving fallback snapshot for Product {ProductId} from memory cache.", productId);
                    return cachedProduct;
                }

                // If the Catalog is down and we don't have it in cache, we must rethrow Shared Kernel exception
                _logger.LogError("CatalogService unavailable and no cached fallback found for Product {ProductId}.", productId);
                throw;
            }
        }
    }
}
