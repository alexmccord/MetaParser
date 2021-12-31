namespace MetaParser.Combinatorics;

internal delegate ParseResult<T> ParserFn<T>(Cursor cursor);

internal static class Parser
{
    public static ParserFn<char> AnyChar { get; }
        = cursor =>
        {
            if (cursor.IsEof)
                return Result.Failure<char>();

            char c = cursor.Current;
            cursor.Next();
            return Result.Success(cursor.State, c);
        };
}
