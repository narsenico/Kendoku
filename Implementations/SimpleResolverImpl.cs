using kendoku.Implementations;
using kendoku.Interfaces;

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
        //  - per ogni cella applico constraint
        //  - per ogni cella applico regole sudoku
        //  - se non ci sono state modifiche nello stato della matrice esco (altrimenti loop infinito)
        //  - reitero fino a che tutte le celle non sono risolte

        ApplyHelpers(cells, helpers);

        // se dopo ogni iterazione l'hash non cambia significa che non ci sono stati cambiamenti nelle celle
        string hash = _hashProvider.GetHash(cells);

        // TODO: l'ordine conta?
        for (var ii=0; !cells.IsResolved(); ii++)
        {
            ExecIteration(cells, constraints);

            var newHash = _hashProvider.GetHash(cells);
            if (newHash == hash) 
            {
                _listener.OnNothingChanged();
                break;
            }

            hash = newHash;
            _listener.OnEndIteration(ii);
        }
        
        return false;
    }

    private void ApplyHelpers(CellStatus[] cells,
                            Helper[] helpers)
    {
        foreach (var help in helpers)
        {
            var cell = cells.Find(help);
            cell.Resolve(help.Value);
            _listener.OnCellResolved(cell);
        }
    }

    private static void ExecIteration(CellStatus[] cells,
                                      Constraint[] constraints)
    {
        foreach (var cell in cells.ExcludeResolved())
        {
            RemoveResolved(cell, cells);
            ApplyConstrainsts(cell, cells, constraints);
        }
    }

    private static void RemoveResolved(CellStatus cell,
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
        // TODO: devo escludere quei valori che non possono risolvere il constraint
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

        Console.WriteLine($"Apply {constraint.ToHumanString()} to cell {cell.ToHumanString()}");
        //throw new NotImplementedException();
    }
}