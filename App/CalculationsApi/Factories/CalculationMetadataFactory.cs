using System.ComponentModel.DataAnnotations;
using System.Reflection;

public static class CalculationMetadataFactory
{
    public static CalculationMetadata Create(ICalculation calculation)
    {
        return new CalculationMetadata(
            calculation.Name,
            GetFields(calculation.RequestType),
            GetFields(calculation.ResponseType)
        );
    }

    // use reflection to find out the meta data on the calcuation request and response types
    private static IReadOnlyList<FieldMetadata> GetFields(Type type)
    {
        return type
            .GetProperties()
            .Select(property => new FieldMetadata(
                property.Name,
                MapType(property.PropertyType),
                IsRequired(property)
            ))
            .ToList();
    }

    // dont leak .net types to frontend. Keep them only related to JS types
    private static string MapType(Type type)
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
