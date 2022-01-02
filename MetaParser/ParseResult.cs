namespace MetaParser;

internal enum ParseStatus
{
    Success = 1,
    Failure = 0,
}

internal record ParseResult<T>(ParseStatus Status, CursorState? State = null, T? Value = default)
{
    public bool IsSuccess => Status == ParseStatus.Success;
    public bool IsFailure => Status == ParseStatus.Failure;
    
    // Success and Failure both can have values, but Failure will always be the default of the given T.
    public T Unwrap() => Value ?? throw new InvalidCastException("Cannot unwrap: Value is missing");

    public CursorState UnwrapState() => Status switch
    {
        ParseStatus.Success when State is not null => State,
        _ => throw new InvalidCastException("Cannot unwrap: this instance wasn't provided a CursorState"),
    };

    public ParseResult<U> AndThen<U>(Func<T?, ParseResult<U>> f) => IsSuccess ? f(Value) : Result.Failure<U>();
    public ParseResult<U> AndThenUnwrap<U>(Func<T, ParseResult<U>> f) => IsSuccess ? f(Unwrap()) : Result.Failure<U>();

    // Only use OrElse if you wish to transform the failure into another alternative.
    public ParseResult<T> OrElse(Func<ParseResult<T>> f) => IsFailure ? f() : this;
}

internal static class Result
{
    // Exists solely to force C# to infer type arguments.
    public static ParseResult<T> Success<T>(CursorState state, T value) => new(ParseStatus.Success, state, value);
    public static ParseResult<T> Failure<T>() => new(ParseStatus.Failure);
}
