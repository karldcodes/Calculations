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
                    prop.PropertyType == typeof(float)) 
                {
                    metadata["step"] = "0.1";
                }

                if (prop.PropertyType == typeof(int))
                {
                    metadata["step"] = "1";
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

        if (type.IsEnum)
            return "enum";

        return Type.GetTypeCode(type) switch
        {
            TypeCode.String => "string",

            TypeCode.Int16 or // account for any .net number types rather then just int etc
            TypeCode.Int32 or
            TypeCode.Int64 or
            TypeCode.UInt16 or
            TypeCode.UInt32 or
            TypeCode.UInt64 or
            TypeCode.Decimal or
            TypeCode.Double or
            TypeCode.Single => "number",

            TypeCode.Boolean => "boolean",

            TypeCode.DateTime => "date",

            _ => "object"
        };
    }

    private static bool IsRequired(PropertyInfo property)
    {
        return property.GetCustomAttribute<RequiredAttribute>() is not null;
    }
}
