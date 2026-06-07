public abstract class Calculation<TRequest, TResponse> : ICalculation
{
    public Type RequestType => typeof(TRequest);
    public Type ResponseType => typeof(TResponse);

    public virtual string Name { get; } = "";

    public async Task<object?> ExecuteAsync(object request)
    {
        var typedRequest = (TRequest)request;

        Validate(typedRequest);

        return await CalculateAsync(typedRequest);
    }

    protected virtual void Validate(TRequest request)
    {
        // Default run no logic
    }

    protected abstract Task<TResponse> CalculateAsync(TRequest request);
}
