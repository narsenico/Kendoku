using Kendoku.Models;

namespace kendoku.Interfaces;

public interface IHashProvider
{
    public string GetHash(IEnumerable<CellStatus> cells);
}
