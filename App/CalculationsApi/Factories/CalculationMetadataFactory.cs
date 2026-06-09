using System.ComponentModel.DataAnnotations;
using System.Reflection;

public static class CalculationMetadataFactory
{
    public static CalculationMetadata Create(ICalculation calculation)
    {
        return new CalculationMetadata(
            Guid.NewGuid(), // used for react FE to have a key for loops
            calculation.Name,
            GetFields(calculation.RequestType),
            GetFields(calculation.ResponseType) // Not used in FE at the moment but there is a comment in the form submission logic to explain its use
        );
    }

    // use reflection to find out the meta data on the calcuation request and response types this will be used in FE to dynamically create the form fields
    private static IReadOnlyList<FieldMetadata> GetFields(Type type)
    {
        return type
            .GetProperties()
            .Select(prop =>
            {
                var metadata = new Dictionary<string, object>();

                if(IsRequired(prop))
                    metadata["required"] = true;

                if (prop.PropertyType == typeof(decimal) ||
                prop.PropertyType == typeof(double) ||
                prop.PropertyType == typeof(float) ||
                prop.PropertyType == typeof(int))
                {
                    metadata["step"] = "any";
                }

                if (prop.GetCustomAttribute<RangeAttribute>() is { } range)
                {
                    metadata["min"] = range.Minimum?.ToString() ?? "";
                    metadata["max"] = range.Maximum?.ToString() ?? "";
                }

                // Get field label
                var display = prop.GetCustomAttribute<DisplayAttribute>();
                var label = display?.GetName() ?? prop.Name;

                return new FieldMetadata
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = prop.Name,
                    Label = label,
                    Type = GetInputType(prop.PropertyType),
                    Metadata = metadata
                };

            })
            .ToList();
    }

    // dont leak .net types to frontend. Keep them only related to JS types
    private static string GetInputType(Type type)
    {
        type = Nullable.GetUnderlyingType(type) ?? type;

        if (type == typeof(string)) return "string";
        if (type == typeof(int)) return "number";
        if (type == typeof(decimal)) return "number";
        if (type == typeof(double)) return "number";
        if (type == typeof(float)) return "number";
        if (type == typeof(bool)) return "boolean";
        if (type == typeof(DateTime)) return "date";
        if (type.IsEnum) return "enum";

        return "object";
    }

    private static bool IsRequired(PropertyInfo property)
    {
        return property.GetCustomAttribute<RequiredAttribute>() is not null;
    }
}
