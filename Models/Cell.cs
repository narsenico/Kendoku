namespace Kendoku.Models;

public sealed class Cell
{
    public int Row { get; init; }
    public int Col { get; init; }
    public int Group { get; init; }
    public int Index { get; init; }
    public int Value { get; private set; }
    private readonly HashSet<int> _possibilities;

    public Cell(int row,
                int col,
                int group,
                int index,
                int[] possibilities)
    {
        Row = row;
        Col = col;
        Group = group;
        Index = index;

        _possibilities = new HashSet<int>(possibilities);
    }

    public IEnumerable<int> Possibilities => _possibilities;

    public bool IsResolved => Value != 0;

    public void AddPossibility(int value)
    {
        if (_possibilities.Add(value))
        {
            OnPossibilitiesChanged();
        }
    }

    public void RemovePossibility(int value)
    {
        if (_possibilities.Remove(value))
        {
            OnPossibilitiesChanged();
        }
    }

    public void Resolve(int value)
    {
        _possibilities.Clear();
        _possibilities.Add(value);
        Value = value;
    }

    private void OnPossibilitiesChanged()
    {
        if (_possibilities.Count == 1)
        {
            Value = _possibilities.First();
        }
        else
        {
            Value = 0;
        }
    }
}