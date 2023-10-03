namespace Kendoku;

public readonly struct MatrixSettings
{
    public readonly int GroupCount { get; init; }
    public readonly int GroupSize { get; init; }
    public readonly int GroupRowSize { get; init; }
    public readonly int MatrixRowSize { get; init; }

    public int RowPerMatrix => GroupCount / MatrixRowSize;
    public int RowPerGroup => GroupSize / GroupRowSize;
    public IEnumerable<int> GetPossibilities() => Enumerable.Range(1, GroupSize);
}
