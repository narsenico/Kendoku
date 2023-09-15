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
                        HelpCell[] helpCells)
    {
        // 1. applico gli aiuti
        // 2. inizio iterazione
        //  - per ogni cella applico constraint
        //  - per ogni cella applico regole sudoku
        //  - se non ci sono state modifiche nello stato della matrice esco (altrimenti loop infinito)
        //  - reitero fino a che tutte le celle non sono risolte

        ApplyHelps(cells, helpCells);

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

    private void ApplyHelps(CellStatus[] cells,
                            HelpCell[] helpCells)
    {
        foreach (var help in helpCells)
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
            // TODO: ApplyConstrainsts(cell, cells, constraints);
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
        throw new NotImplementedException();
    }
}