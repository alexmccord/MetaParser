namespace MetaParser;

internal record CursorState(int Line, int Column, int Offset);

internal class Cursor
{
    private int line = 0;
    private int column = 0;
    private int offset = 0;

    public string Input { get; init; }
    public char Current => Input[offset];
    public bool IsEof => Input.Length <= offset;

    public CursorState State => new(line, column, offset);

    public Cursor(string input) => Input = input;

    public void Next() => ++offset;
}
