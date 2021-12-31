using System.Text;

namespace MetaParser.Combinatorics;

internal static class Combinator
{
    internal static ParseResult<T> Parse<T>(this ParserFn<T> p, string input) => p(new Cursor(input));
    internal static T ParseUnwrap<T>(this ParserFn<T> p, string input) => p.Parse(input).Unwrap();

    public static ParserFn<B> Bind<A, B>(this ParserFn<A> p1, Func<A, ParserFn<B>> map)
        => cursor =>
        {
            var r1 = p1(cursor);
            if (r1.IsFailure)
                return Result.Failure<B>();

            var p2 = map(r1.Unwrap());
            var oldState = cursor.State;
            var r2 = p2(cursor);
            return r2;
        };

    public static ParserFn<B> Transform<A, B>(this ParserFn<A> p, Func<A, B> transform)
        => cursor => p(cursor).AndThenUnwrap(a => Result.Success(cursor.State, transform(a))).OrElse(() => Result.Failure<B>());

    private static IEnumerable<T> ManyImpl<T>(this ParserFn<T> p, Cursor cursor)
    {
        List<T> acc = new();

        ParseResult<T> current;
        while ((current = p(cursor)).IsSuccess)
            acc.Add(current.Unwrap());

        return acc;
    }

    public static ParserFn<IEnumerable<T>> Many<T>(this ParserFn<T> p)
        => cursor =>
        {
            // Because ManyImpl is side-effecting, we need to perform ManyImpl first before we get the new State.
            var result = ManyImpl(p, cursor);
            return Result.Success(cursor.State, result);
        };

    public static ParserFn<string> Many(this ParserFn<char> p)
        => cursor =>
        {
            // Again, ManyImpl should be performed first before accessing the new State because it is side-effecting.
            var result = ManyImpl(p, cursor);
            return Result.Success(cursor.State, string.Concat(result));
        };

    public static ParserFn<C> Between<A, B, C>(this ParserFn<C> middle, ParserFn<A> open, ParserFn<B> close)
        => cursor => open(cursor).AndThen(_ => middle(cursor).AndThenUnwrap(c => close(cursor).AndThen(_ => Result.Success(cursor.State, c))));

    public static ParserFn<T> Satisfy<T>(this ParserFn<T> parser, Predicate<char> pred)
        => cursor => pred(cursor.Current) ? parser(cursor) : Result.Failure<T>();
}
