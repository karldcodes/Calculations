public interface ICalculation
{
    string Name { get; }
    Type RequestType { get; }
    Type ResponseType { get; }

    Task<object?> ExecuteAsync(object request);
}
