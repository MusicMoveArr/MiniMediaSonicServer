using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Primitives;

namespace MiniMediaSonicServer.Api.Binders;

public class HybridBinder<T> : IModelBinder where T : class, new()
{
    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var request = bindingContext.HttpContext.Request;
        var result = new T();

        var queries = bindingContext.HttpContext.Request.Query
            .GroupBy(pair => pair.Key.ToLower())
            .Select(pair => pair.First())
            .ToDictionary(StringComparer.OrdinalIgnoreCase);
        
        if (request.HasFormContentType && request.Body?.Length > 0)
        {
            var form = await request.ReadFormAsync();
            foreach (var key in form.Keys)
            {
                if (!queries.ContainsKey(key))
                {
                    queries[key] = form[key];
                }
            }
        }
        else if (request.Body?.Length > 0 || request.Headers.TransferEncoding == "chunked")
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
        
        if (queries.Any())
        {
            foreach (var prop in typeof(T).GetProperties().Where(p => p.CanWrite))
            {
                var jsonPropertyName = prop.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name;
                var queryKey = jsonPropertyName ?? prop.Name;
                var valueResult = bindingContext.ValueProvider.GetValue(queryKey);

                if (valueResult == ValueProviderResult.None && jsonPropertyName != null)
                    valueResult = bindingContext.ValueProvider.GetValue(prop.Name);

                StringValues stringValues = valueResult.Values;
                if (valueResult == ValueProviderResult.None)
                {
                    queries.TryGetValue(queryKey, out stringValues);
                }
                
                if (stringValues.Count > 0)
                {
                    try
                    {
                        var converted = ConvertValue(stringValues, prop.PropertyType);
                        prop.SetValue(result, converted);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"HybridBinding failed on property '{prop.Name}' '{queryKey}', '{stringValues}', '{bindingContext.HttpContext.Request.Path}', {ex.Message}, {ex.StackTrace}");
                    }
                }
            }
        }

        bindingContext.Result = ModelBindingResult.Success(result);
    }
    
    private object ConvertValue(string raw, Type targetType)
    {
        if (string.IsNullOrEmpty(raw))
        {
            return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
        }

        var underlying = Nullable.GetUnderlyingType(targetType);
        if (underlying != null)
        {
            return ConvertValue(raw, underlying);
        }

        if (targetType.IsEnum)
        {
            return Enum.Parse(targetType, raw, ignoreCase: true);
        }

        if (targetType == typeof(Guid))
        {
            return Guid.Parse(raw);
        }

        if (targetType != typeof(string) && 
            (targetType.IsClass || targetType.IsGenericType))
        {
            if (raw.TrimStart().StartsWith("[") || raw.TrimStart().StartsWith("{"))
            {
                return JsonSerializer.Deserialize(raw, targetType);
            }

            if (targetType == typeof(List<string>) || targetType == typeof(IList<string>))
            {
                return raw.Split([',', ';'])
                    .Select(s => s.Trim())
                    .ToList();
            }

            if (targetType == typeof(List<Guid>) || targetType == typeof(IList<Guid>))
            {
                return raw.Split([',', ';'])
                    .Select(id => Guid.TryParse(id, out Guid guid) ? guid : Guid.Empty)
                    .Where(id => id != Guid.Empty)
                    .ToList();
            }
        }

        return Convert.ChangeType(raw, targetType);
    }
}