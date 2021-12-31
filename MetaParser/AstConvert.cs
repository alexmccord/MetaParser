namespace MetaParser;

/// <summary>
/// The parsing frontend that converts your input into the requested AST.
/// </summary>
/// <typeparam name="A"></typeparam>
public static class AstConvert<A>
{
    public static bool TryParse(string input, out A? output)
    {
        // TODO: implement the lexing/parsing logic here, probably through dynamic generation of parser combinatorics.
        output = default;
        return false;
    }

    public static A? Parse(string input)
    {
        if (TryParse(input, out var output))
            return output;

        return default;
    }
}
