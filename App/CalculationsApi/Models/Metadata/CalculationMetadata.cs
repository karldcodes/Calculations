
public sealed record CalculationMetadata(
    Guid Id,
    string Name,
    IReadOnlyList<FieldMetadata> RequestFields,
    IReadOnlyList<FieldMetadata> ResponseFields
);
