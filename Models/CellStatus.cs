namespace Kendoku.Models;

public sealed class CellStatus
{
    private readonly Cell _cell;
    private readonly HashSet<int> _possibilities;
    public int Value { get; private set; }

    public CellStatus(Cell cell,
                      int[] initialPossibilities)
    {
        _cell = cell;
        _possibilities = new HashSet<int>(initialPossibilities);
    }

    public IEnumerable<int> Possibilities => _possibilities;
    public Cell Cell => _cell;

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
        if (_possibilities.Count == 0)
        {
            throw new InvalidOperationException($"Zero possibilities for cell {Cell}!");
        }

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