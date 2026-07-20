using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedKernel.Web.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class InternalApiKeyAttribute : Attribute, IAsyncActionFilter
    {
        private const string ApiKeyHeaderName = "X-Internal-Api-Key";
        private const string ApiKeyConfigName = "InternalApiKey";

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var config = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var expectedApiKey = config.GetValue<string>(ApiKeyConfigName);

            // Check if the configuration is missing on the server
            if (string.IsNullOrEmpty(expectedApiKey))
            {
                context.Result = new ObjectResult(new { Message = "Server configuration error: API Key is missing." })
                {
                    StatusCode = 500
                };
                return;
            }

            // Check if the client provided the key
            if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var receivedKey))
            {
                context.Result = new UnauthorizedObjectResult(new { Message = "API Key was not provided." });
                return;
            }

            // Check if the keys match
            if (!expectedApiKey.Equals(receivedKey))
            {
                context.Result = new UnauthorizedObjectResult(new { Message = "Unauthorized client." });
                return;
            }

            await next();
        }
    }

}

