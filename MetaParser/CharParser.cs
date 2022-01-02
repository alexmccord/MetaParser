namespace MetaParser;

internal delegate ParseResult<T> Parser<T>(Cursor cursor);

internal static class CharParser
{
    public static Parser<char> AnyChar { get; }
        = cursor =>
        {
            if (cursor.IsEof)
                return Result.Failure<char>();

            char c = cursor.Current;
            cursor.Next();
            return Result.Success(cursor.State, c);
        };
}
