using Kendoku.Interfaces;
using Kendoku.Models;


namespace Kendoku.Implementations;

public class SimpleResolverImpl : IResolver
{
    private readonly IEventListener _listener;
    private readonly IHashProvider _hashProvider;

    public SimpleResolverImpl(IEventListener listener,
                              IHashProvider hashProvider)
    {
        _listener = listener;
        _hashProvider = hashProvider;
    }

    public bool Resolve(CellStatus[] cells,
                        Constraint[] constraints,
                        Helper[] helpers)
    {
        // 1. applico gli aiuti
        // 2. inizio iterazione
        //  - per ogni cella applico regole sudoku
        //  - per ogni cella applico constraint
        //  - se non ci sono state modifiche nello stato della matrice esco (altrimenti loop infinito)
        //  - reitero fino a che tutte le celle non sono risolte

        ApplyHelpers(cells, helpers);
        _listener.OnEndIteration(0, cells.OnlyResolved().Count());

        // se dopo ogni iterazione l'hash non cambia significa che non ci sono stati cambiamenti nelle celle
        string hash = _hashProvider.GetHash(cells);

        for (var ii=1; !cells.IsResolved(); ii++)
        {
            ExecIteration(cells, constraints);

            var newHash = _hashProvider.GetHash(cells);
            if (newHash == hash) 
            {
                _listener.OnNothingChanged();
                break;
            }

            hash = newHash;
            _listener.OnEndIteration(ii, cells.OnlyResolved().Count());
        }
        
        return cells.IsResolved();
    }

    private static void ApplyHelpers(CellStatus[] cells,
                                     Helper[] helpers)
    {
        foreach (var help in helpers)
        {
            var cell = cells.Find(help);
            cell.Resolve(help.Value);

#if DEBUG
            if (cell.IsResolved)
            {
                Console.WriteLine($"Resolved with {help.ToHumanString()} => {cell.ToHumanString()}");
            }
#endif
        }
    }

    private static void ExecIteration(CellStatus[] cells,
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

    private static void ApplySudokuRules(CellStatus cell,
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

#if DEBUG
        if (cell.IsResolved)
        {
            Console.WriteLine($"Resolved with sudoku rules => {cell.ToHumanString()}");
        }
#endif
    }

    private static void ApplyConstrainsts(CellStatus cell,
                                          CellStatus[] cells,
                                          Constraint[] constraints)
    {
        foreach (var constraint in constraints.FilterBy(cell))
        {
            ApplyConstrainst(cell, cells, constraint);
        }
    }

    private static void ApplyConstrainst(CellStatus cell,
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

        var otherValues = constrainedCells.Select(c => c.Possibilities.ToArray()).ToArray();

        foreach (var cellValue in cell.Possibilities.ToArray())
        {
            if (!CheckConstraint(otherValues, cellValue, constraint.Sum))
            {
                cell.RemovePossibility(cellValue);
            }
        }

#if DEBUG
        if (cell.IsResolved)
        {
            Console.WriteLine($"Resolved with {constraint.ToHumanString()} => {cell.ToHumanString()}");
        }
#endif
    }

    private static bool CheckConstraint(int[][] values, int initialValue, int check)
    {
        // 1,2,4,5,6 <= value
        // 3         <= values[0]
        // 1, 2, 6   <= values[1]

        var maxIteration = values.Aggregate(1, (a, r) => a *= r.Length);
        var rowIndexes = new int[values.Length]; // 0 0 0

        for (int ii=0;ii<maxIteration;ii++)
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