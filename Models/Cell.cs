namespace Kendoku.Models;

public record Cell(int GroupIndex, int Row, int Col);

public record HelpCell(Cell Cell, int Value)
{
    public HelpCell(int GroupIndex, int Row, int Col, int Value) : this(new(GroupIndex, Row, Col), Value) { }
}