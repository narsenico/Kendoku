using Kendoku.Interfaces;
using Kendoku.Models;

namespace Kendoku.Implementations;

public class SimpleResolver : IResolver
{
    private readonly IEventListener _listener;
    private readonly IHashProvider _hashProvider;

    public SimpleResolver(IEventListener listener,
                          IHashProvider hashProvider)
    {
        _listener = listener;
        _hashProvider = hashProvider;
    }

    public Result Resolve(CellStatus[] cells,
                          Constraint[] constraints,
                          Helper[] helpers)
    {
        // 1. applico gli aiuti
        // 2. inizio iterazione
        //  - per ogni cella applico regole sudoku
        //  - per ogni cella applico constraint
        //  - se non ci sono state modifiche nello stato della matrice esco (altrimenti loop infinito)
        //  - reitero fino a che tutte le celle non sono risolte
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

        foreach (var cell in cells.ExcludeResolved())
        {
            ApplyConstrainsts(cell, cells, constraints);
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

    private void ApplyConstrainsts(CellStatus cell,
                                   CellStatus[] cells,
                                   Constraint[] constraints)
    {
        foreach (var constraint in constraints.FilterBy(cell))
        {
            ApplyConstrainst(cell, cells, constraint);
        }
    }

    private void ApplyConstrainst(CellStatus cell,
                                  CellStatus[] cells,
                                  Constraint constraint)
    {
        // escludo quei valori che non possono risolvere il constraint
        //  quindi se un valore sommato a qualsiasi altro valore delle altre celle non risolve il constraint va scartato

        // cell 1,2,4,5,6
        // c1   3
        // c2   1, 2, 6
        // sum 13
        // scarto 1 perché sommato a 3 e a 1,2 o 6 non fa mai 13
        // scarto 2 perché sommato a 3 e a 1,2 o 6 non fa mai 13
        // mantengo 4 perché sommato a 3 e a 6 fa 13
        // scarto 5 perché sommato a 3 e a 1,2 o 6 non fa mai 13
        // scarto 6 perché sommato a 3 e a 1,2 o 6 non fa mai 13

        var constrainedCells = cells.OnConstarint(constraint)
            .Exclude(cell);

        if (constrainedCells.Any())
        {
            var otherValues = constrainedCells.Select(c => c.Possibilities.ToArray()).ToArray();

            foreach (var cellValue in cell.Possibilities.ToArray())
            {
                if (!CheckConstraint(otherValues, cellValue, constraint.Sum))
                {
                    cell.RemovePossibility(cellValue);
                }
            }
        }
        else
        {
            // se non ci sono altre celle nel constraint (per errore di configurazione si presume)
            // controllo solo se la cella può assumere il valore del constraint
            if (cell.Possibilities.Contains(constraint.Sum))
            {
                cell.Resolve(constraint.Sum);
            }
        }

        if (cell.IsResolved)
        {
            _listener.OnCellResolved(cell.Cell, constraint);
        }
    }

    private static bool CheckConstraint(int[][] values, int initialValue, int check)
    {
        // 1,2,4,5,6 <= value
        // 3         <= values[0]
        // 1, 2, 6   <= values[1]

        var maxIteration = values.Aggregate(1, (a, r) => a *= r.Length);
        var rowIndexes = new int[values.Length]; // 0 0 0

        for (int ii = 0; ii < maxIteration; ii++)
        {
            var acc = Accumulate(values, rowIndexes, 0, initialValue);
            if (acc == check)
            {
                return true;
            }
        }

        return false;
    }

    // TODO: questa funzione mi fa ribrezzo!
    private static int Accumulate(int[][] values, int[] indexes, int row, int value)
    {
        var col = indexes[row];
        value += values[row][col];

        if (row == values.Length - 1) // non ci sono più altre righe
        {
            if (col == values[row].Length - 1) // non ci sono più altre colonne su questa riga
            {
                if (row != 0)
                {
                    indexes[row - 1] = indexes[row - 1] + 1; // passo all'elemento successivo della riga prima
                    indexes[row] = 0; // mentre per questa riga torno alla prima posizione
                }
            }
            else
            {
                indexes[row] = indexes[row] + 1; // passo all'elemento successivo di questa riga
            }
            return value;
        }

        return Accumulate(values, indexes, row + 1, value);
    }
}
