using Xunit;
using MetaParser.Combinatorics;

namespace MetaParser.InternalTests;

internal enum Letter
{
    Unknown,
    A,
    NotA,
}

internal static class LetterUtils
{
    public static Letter TransformIntoLetter(char c) => c == 'A' ? Letter.A : Letter.NotA;
}

public class CombinatoricTests
{
    [Fact]
    public void ParseOneChar()
    {
        var parser = Parser.AnyChar;

        Assert.Equal('5', parser.ParseUnwrap("5"));
    }

    [Theory]
    [InlineData("15")]
    [InlineData("30")]
    [InlineData("63245654tgfeaedfsw45w4eds")]
    public void ParseManyChars(string s)
    {
        Assert.Equal(s, Parser.AnyChar.Many().ParseUnwrap(s));
    }

    [Fact]
    public void MapCharToLetter()
    {
        var parser = Parser.AnyChar
            .Transform(LetterUtils.TransformIntoLetter);

        Assert.Equal(Letter.A, parser.ParseUnwrap("A"));
        Assert.Equal(Letter.NotA, parser.ParseUnwrap("B"));
    }

    [Fact]
    public void MapManyCharsToLetters()
    {
        var parser = Parser.AnyChar
            .Transform(LetterUtils.TransformIntoLetter)
            .Many();

        Assert.Equal(new[] { Letter.A, Letter.NotA, Letter.A }, parser.ParseUnwrap("ABA"));
        Assert.Equal(new[] { Letter.NotA, Letter.A, Letter.NotA }, parser.ParseUnwrap("BAB"));
    }

    [Fact]
    public void MapCharToLetterOrFail()
    {
        var parser = Parser.AnyChar
            .Satisfy(c => c == 'A')
            .Transform(LetterUtils.TransformIntoLetter);

        ParseResult<Letter> l1 = parser.Parse("A");
        Assert.True(l1.IsSuccess);
        Assert.Equal(Letter.A, l1.Value);

        ParseResult<Letter> l2 = parser.Parse("B");
        Assert.True(l2.IsFailure);
        Assert.Equal(Letter.Unknown, l2.Value);
    }

    [Fact]
    public void MapCharToLetterOrFail2()
    {
        var parser = Parser.AnyChar
            .Transform(LetterUtils.TransformIntoLetter)
            .Satisfy(c => c == 'A');

        ParseResult<Letter> l1 = parser.Parse("A");
        Assert.True(l1.IsSuccess);
        Assert.Equal(Letter.A, l1.Value);

        ParseResult<Letter> l2 = parser.Parse("B");
        Assert.True(l2.IsFailure);
        Assert.Equal(Letter.Unknown, l2.Value);
    }

    [Fact]
    public void MapManyCharsToLetters2()
    {
        var parser = Parser.AnyChar
            .Transform(LetterUtils.TransformIntoLetter)
            .Many();

        Assert.Equal(new[] { Letter.A, Letter.NotA, Letter.A }, parser.ParseUnwrap("ABA"));
        Assert.Equal(new[] { Letter.NotA, Letter.A, Letter.NotA }, parser.ParseUnwrap("BAB"));
    }

    [Fact]
    public void BetweenBraces()
    {
        var open = Parser.AnyChar.Satisfy(c => c == '{');
        var middle = Parser.AnyChar.Satisfy(c => c != '}').Many();
        var close = Parser.AnyChar.Satisfy(c => c == '}');

        Assert.Equal("test", middle.Between(open, close).ParseUnwrap("{test}"));
    }
}
