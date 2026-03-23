using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MiniMediaSonicServer.Api.Binders;

public class HybridBinder<T> : IModelBinder where T : class, new()
{
    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var request = bindingContext.HttpContext.Request;
        var result = new T();

        if (request.ContentLength > 0 || request.Headers.TransferEncoding == "chunked")
        {
            request.EnableBuffering();
            var body = await JsonSerializer.DeserializeAsync<T>(
                request.Body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            if (body != null)
            {
                result = body;
            }
            request.Body.Position = 0;
        }

        if (bindingContext.HttpContext.Request.Query.Any())
        {
            foreach (var prop in typeof(T).GetProperties().Where(p => p.CanWrite))
            {
                var currentValue = prop.GetValue(result);

                if (currentValue != null)
                {
                    continue;
                }
                
                var jsonPropertyName = prop.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name;
                var queryKey = jsonPropertyName ?? prop.Name;
                var valueResult = bindingContext.ValueProvider.GetValue(queryKey);

                if (valueResult == ValueProviderResult.None && jsonPropertyName != null)
                    valueResult = bindingContext.ValueProvider.GetValue(prop.Name);

                if (valueResult != ValueProviderResult.None)
                {
                    try
                    {
                        var converted = Convert.ChangeType(valueResult.FirstValue, prop.PropertyType);
                        prop.SetValue(result, converted);
                    }
                    catch { /* skip unparseable values */ }
                }
            }
        }

        bindingContext.Result = ModelBindingResult.Success(result);
    }
}