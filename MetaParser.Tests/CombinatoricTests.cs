using Xunit;
using MetaParser.Combinatorics;

namespace MetaParser.Tests;

public class CombinatoricTests
{
    [Fact]
    public void ParseOneChar()
    {
        ParserFn<char> parser = new(cursor => Result.Success(cursor));

        Assert.Equal('5', parser.ParseAndUnwrap("5"));
    }

    [Fact]
    public void ParseTwoChars()
    {
        ParserFn<string> parser = new ParserFn<char>(cursor => Result.Success(cursor)).Many();

        Assert.Equal("15", parser.ParseAndUnwrap("15"));
    }
}
