namespace Kendoku;

public readonly struct MatrixSettings
{
    public readonly int GroupCount { get; init; }
    public readonly int GroupSize { get; init; }
    public readonly int GroupRowSize { get; init; }
    public readonly int GroupPerMatrixRow { get; init; }

    public int RowPerGroup => GroupSize / GroupRowSize;
    public IEnumerable<int> GetPossibilities() => Enumerable.Range(1, GroupSize);
}