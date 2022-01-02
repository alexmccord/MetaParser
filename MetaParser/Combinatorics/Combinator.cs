namespace MetaParser.Combinatorics;

internal static class Combinator
{
    internal static ParseResult<T> Parse<T>(this Parser<T> p, string input) => p(new Cursor(input));
    internal static T ParseUnwrap<T>(this Parser<T> p, string input) => p.Parse(input).Unwrap();

    public static Parser<B> Bind<A, B>(this Parser<A> p1, Func<A, Parser<B>> map)
        => cursor =>
        {
            var r1 = p1(cursor);
            if (r1.IsFailure)
                return Result.Failure<B>();

            var p2 = map(r1.Unwrap());
            return p2(cursor);
        };

    public static Parser<B> Transform<A, B>(this Parser<A> p, Func<A, B> transform)
        => cursor => p(cursor).AndThenUnwrap(a => Result.Success(cursor.State, transform(a))).OrElse(() => Result.Failure<B>());

    private static IEnumerable<T> ManyImpl<T>(this Parser<T> p, Cursor cursor)
    {
        List<T> acc = new();

        ParseResult<T> current;
        while ((current = p(cursor)).IsSuccess)
            acc.Add(current.Unwrap());

        return acc;
    }

    public static Parser<IEnumerable<T>> Many<T>(this Parser<T> p)
        => cursor =>
        {
            // Because ManyImpl is side-effecting, we need to perform ManyImpl first before we get the new State.
            var result = ManyImpl(p, cursor);
            return Result.Success(cursor.State, result);
        };

    public static Parser<string> Many(this Parser<char> p)
        => cursor =>
        {
            // Again, ManyImpl should be performed first before accessing the new State because it is side-effecting.
            var result = ManyImpl(p, cursor);
            return Result.Success(cursor.State, string.Concat(result));
        };

    public static Parser<C> Between<A, B, C>(this Parser<C> middle, Parser<A> open, Parser<B> close)
        => cursor => open(cursor).AndThen(_ => middle(cursor).AndThenUnwrap(c => close(cursor).AndThen(_ => Result.Success(cursor.State, c))));

    public static Parser<T> Satisfy<T>(this Parser<T> parser, Predicate<char> pred)
        => cursor => pred(cursor.Current) ? parser(cursor) : Result.Failure<T>();
}
