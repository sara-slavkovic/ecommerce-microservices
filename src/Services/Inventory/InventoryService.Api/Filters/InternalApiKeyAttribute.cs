using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace InventoryService.Api.Filters
{
    public class InternalApiKeyAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var config = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var expectedApiKey = config.GetValue<string>("InternalApiKey");

            if (!context.HttpContext.Request.Headers.TryGetValue("X-Internal-Api-Key", out var receivedKey) || receivedKey != expectedApiKey)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            await next();
        }
    }
}
