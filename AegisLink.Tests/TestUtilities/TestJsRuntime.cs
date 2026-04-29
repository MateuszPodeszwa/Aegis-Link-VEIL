using Microsoft.JSInterop;

namespace AegisLink.Tests.TestUtilities;

public sealed class TestJsRuntime : IJSRuntime
{
    private readonly Func<string, object?[]?, object?> _handler;

    public TestJsRuntime(Func<string, object?[]?, object?> handler)
    {
        _handler = handler;
    }

    public List<(string Identifier, object?[]? Arguments)> Invocations { get; } = new();

    public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
    {
        return InvokeAsync<TValue>(identifier, CancellationToken.None, args);
    }

    public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
    {
        Invocations.Add((identifier, args));
        var result = _handler(identifier, args);

        if (result is null)
        {
            return new ValueTask<TValue>(default(TValue)!);
        }

        if (result is TValue typed)
        {
            return new ValueTask<TValue>(typed);
        }

        return new ValueTask<TValue>((TValue)result);
    }
}
