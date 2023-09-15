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
        while (!cells.IsResolved())
        {
            ExecIteration(cells, constraints);

            var newHash = _hashProvider.GetHash(cells);
            if (newHash == hash) 
            {
                _listener.OnNothingChanged();
                break;
            }

            hash = newHash;
        }
        
        return false;
    }

    private void ApplyHelps(CellStatus[] cells,
                            HelpCell[] helpCells)
    {
        foreach (var help in helpCells)
        {
            var cell = cells.Find(help.Cell);
            cell.Resolve(help.Value);
            _listener.OnCellResolved(cell);
        }
    }

    private void ExecIteration(CellStatus[] cells,
                               Constraint[] constraints)
    {
        foreach (var cell in cells)
        {
            ExecOnCell(cell, constraints);
        }
    }

    private void ExecOnCell(CellStatus cell,
                            Constraint[] constraints)
    {
        // TODO
    }
}