
public sealed record CalculationMetadata(
    string Name,
    IReadOnlyList<FieldMetadata> RequestFields,
    IReadOnlyList<FieldMetadata> ResponseFields
);
