using Kendoku.Interfaces;
using Kendoku.Models;

namespace Kendoku.Implementations;

public class SudokuResolver : IResolver
{
    private readonly IEventListener _listener;
    private readonly IHashProvider _hashProvider;

    public SudokuResolver(IEventListener listener,
                          IHashProvider hashProvider)
    {
        _listener = listener;
        _hashProvider = hashProvider;
    }

    public Result Resolve(CellStatus[] cells,
                          Constraint[] constraints,
                          Helper[] helpers)
    {
       var ii = 0;

        ApplyHelpers(cells, helpers);
        _listener.OnEndIteration(ii, cells.OnlyResolved().Count());

        // se dopo ogni iterazione l'hash non cambia significa che non ci sono stati cambiamenti nelle celle
        string hash = _hashProvider.GetHash(cells);

        while (!cells.IsResolved())
        {
            ++ii;
            ExecIteration(cells, constraints);

            var newHash = _hashProvider.GetHash(cells);
            if (newHash == hash)
            {
                _listener.OnNothingChanged(ii);
                break;
            }

            hash = newHash;
            _listener.OnEndIteration(ii, cells.OnlyResolved().Count());
        }

        return new Result(
                Success: cells.IsResolved(),
                Cells: cells,
                IterationCount: ii + 1);
    }

    private void ApplyHelpers(CellStatus[] cells,
                              Helper[] helpers)
    {
        foreach (var help in helpers)
        {
            var cell = cells.Find(help);
            cell.Resolve(help.Value);

            _listener.OnCellResolved(cell.Cell, help);
        }
    }

    private void ExecIteration(CellStatus[] cells,
                                      Constraint[] constraints)
    {
        foreach (var cell in cells.ExcludeResolved())
        {
            ApplySudokuRules(cell, cells);
        }
    }

    private void ApplySudokuRules(CellStatus cell,
                                  CellStatus[] cells)
    {
        // 1. rimuovo i numeri già usati sulla stessa riga della matrice
        cells.OnSameMatrixRowOf(cell)
            .Exclude(cell)
            .OnlyResolved()
            .PurgePossibilitiesOf(cell);

        // 2. rimuovo i numeri già usati sulla stessa colonna della matrice
        cells.OnSameMatrixColOf(cell)
            .Exclude(cell)
            .OnlyResolved()
            .PurgePossibilitiesOf(cell);

        // 3. rimuovo i numeri già usati all'interno del gruppo
        cells.OnGroupOf(cell)
            .Exclude(cell)
            .OnlyResolved()
            .PurgePossibilitiesOf(cell);

        // 4. se la cella ha un numero che ha solo lei rispetto alla riga, 
        // colonna o gruppo, allora è quello risolutivo
        cells.OnSameMatrixRowOf(cell)
            .Exclude(cell)
            .SelectMany(c => c.Possibilities)
            .MantainUniqueValueIn(cell);

        cells.OnSameMatrixColOf(cell)
            .Exclude(cell)
            .SelectMany(c => c.Possibilities)
            .MantainUniqueValueIn(cell);

        cells.OnGroupOf(cell)
            .Exclude(cell)
            .SelectMany(c => c.Possibilities)
            .MantainUniqueValueIn(cell);

        if (cell.IsResolved)
        {
            _listener.OnCellResolved(cell.Cell, "sudoku ruels");
        }
    }
}
