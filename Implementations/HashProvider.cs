using Kendoku.Interfaces;

using Kendoku.Models;

using System.Security.Cryptography;

namespace Kendoku.Implementations;

public class HashProvider : IHashProvider
{
    public string GetHash(IEnumerable<CellStatus> cells)
    {        
        using var stream = new MemoryStream();
        foreach (var cell in cells)
        {
            Add(stream, cell);
        }
        stream.Position = 0;

        using var sha256Hash = SHA256.Create();
        var hash = sha256Hash.ComputeHash(stream);

        return BitConverter.ToString(hash);
    }

    private static void Add(MemoryStream stream, CellStatus cell)
    {
        stream.Write(BitConverter.GetBytes(cell.Cell.GroupIndex));
        stream.Write(BitConverter.GetBytes(cell.Cell.Row));
        stream.Write(BitConverter.GetBytes(cell.Cell.Col));

        foreach (int v in cell.Possibilities)
        {
            stream.Write(BitConverter.GetBytes(v));
        }
    }
}
