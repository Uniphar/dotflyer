using DotFlyer.Common.EmailTemplates;
using Microsoft.AspNetCore.Components;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace DotFlyer.EmailTemplates.Preview;

public sealed record PreviewTemplate(string Name, Type ComponentType, Type ModelType);

public sealed class TemplateCatalog
{
    private const string ComponentsNamespace = "DotFlyer.EmailTemplates.Components";

    private readonly Dictionary<string, PreviewTemplate> _byName;

    private TemplateCatalog(IReadOnlyList<PreviewTemplate> templates)
    {
        Templates = templates;
        _byName = templates.ToDictionary(t => t.Name, StringComparer.Ordinal);
    }

    public IReadOnlyList<PreviewTemplate> Templates { get; }

    // Templates are compiled components in the Components namespace that take a [Parameter] Model.
    public static TemplateCatalog Discover(Assembly assembly) =>
        new(assembly.GetTypes()
            .Select(ToPreviewTemplate)
            .OfType<PreviewTemplate>()
            .OrderBy(t => t.Name, StringComparer.Ordinal)
            .ToList());

    public bool TryGet(string name, [NotNullWhen(true)] out PreviewTemplate? template) =>
        _byName.TryGetValue(name, out template);

    private static PreviewTemplate? ToPreviewTemplate(Type type)
    {
        var modelProperty = type.GetProperty("Model");
        return type is { IsClass: true, IsAbstract: false }
            && type.Namespace == ComponentsNamespace
            && typeof(IComponent).IsAssignableFrom(type)
            && modelProperty?.IsDefined(typeof(ParameterAttribute)) == true
                ? new PreviewTemplate(type.Name, type, modelProperty.PropertyType)
                : null;
    }

    public object CreateModel(Type modelType)
    {
        var model = Activator.CreateInstance(modelType)!;

        if (ModelCache.TryGetValue(modelType, out var cachedModel))
            return cachedModel;

        foreach (var property in modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (!property.CanWrite || property.GetValue(model) != null) 
                continue;

            if (property.PropertyType == typeof(string)) 
                property.SetValue(model, "VALUE");
            else if (property.PropertyType == typeof(IEnumerable<string>)) 
                property.SetValue(model, Array.Empty<string>());
        }

        return model;
    }

    private static readonly Dictionary<Type, object> ModelCache = new()
    {
        { typeof(NewEmployeeOnboardingModel), new NewEmployeeOnboardingModel { FirstName = "John", UserPrincipalName = "user@uniphar.ie", PwPushUrl = "https://eu.pwpush.com/ABC" } }
        // add other models here as needed
    };
    
}
