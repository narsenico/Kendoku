namespace Kendoku.Models;

public record Cell(int GroupIndex, int Row, int Col, int MatrixRow, int MatrixCol)
{
    public override string ToString()
    {
        return $"[{MatrixRow}:{MatrixCol}]{GroupIndex}:{Row}:{Col}";
    }
}