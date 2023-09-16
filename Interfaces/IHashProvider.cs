using Kendoku.Models;

namespace Kendoku.Interfaces;

public interface IHashProvider
{
    public string GetHash(IEnumerable<CellStatus> cells);
}
