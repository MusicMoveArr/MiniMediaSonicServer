using Microsoft.AspNetCore.Mvc.ModelBinding;
using MiniMediaSonicServer.Application.Attributes;

namespace MiniMediaSonicServer.Api.Binders;

public class HybridBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        if (context.Metadata.ModelType.GetCustomAttributes(true)
            .OfType<HybridBindAttribute>().Any())
        {
            var binderType = typeof(HybridBinder<>)
                .MakeGenericType(context.Metadata.ModelType);
            return (IModelBinder)Activator.CreateInstance(binderType)!;
        }
        return null;
    }
}