public sealed class FieldMetadata
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Label { get; set; } = "";
    public string Type { get; set; } = "text";

    public Dictionary<string, object> Metadata { get; set; } = [];
}