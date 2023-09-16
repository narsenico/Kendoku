using Kendoku;

using System.Runtime.CompilerServices;

using Lines = System.Collections.Generic.List<string>;

namespace kendoku.Converters;

/// <summary>
/// ogni cella è un carattere
/// GroupRowSize: conto le celle tra i primi 2 * (*---* => 3)
/// GroupPerMatrixRow: conto quanti * ci sono in una sola riga -1 (*---*---* => 2)
/// GroupSize: conto le celle tra i primi 2 * in verticale e poi moltiplico per GroupRowSize
/// GroupCount: conto i * -1 nella prima riga e nella prima colonna e li moltipico
/// quando incontro un | iniziano le celle
/// un carattere [A-Za-z] indica un vincolo, lettera uguale corrisponde uno stesso vincolo (almeno 2 celle)
/// un carattere [a-z] indica un aiuto
/// </summary>
internal class FileToArgsConverter
{
    private record Point(int Row, int Col);
    private record Group(int Index, Point Start, Point End)
    {
        public int CellCount() => throw new NotImplementedException();
        public int RowSize() => End.Col - Start.Col + 1;
    }
    private record RelevantCell(char Key, int GroupIndex, int Row, int Col);

    public FileToArgsConverter()
    {

    }

    public string[] ConvertToArgs(string fileName)
    {
        var lines = ReadAllLines(fileName);

        var (matrixSettings, relevantCells, offset) = ParseMatrix(lines);

        // TODO: constraint e helpers partendo da relevantCells
        // TODO: comporre args

        throw new NotImplementedException();
    }

    private static Lines ReadAllLines(string fileName)
    {
        using var reader = File.OpenText(fileName);
        return File.ReadAllLines(fileName).ToList();
    }

    private static (MatrixSettings, List<RelevantCell>, int) ParseMatrix(Lines lines)
    {
        // TODO: devo ragionare per gruppi altrimenti non riesco a capire quali sono le coordiante relative al gruppo delle celle
        // 1. trovare i punti di partenza e fine della griglia (primo e ultimo *)
        // 2. trovare i punti di partenza e fine di ogni gruppo
        // 3. per ogni gruppo scorrere le celle e leggere constraint e helper

        var matrixStart = FindMatrixStart(lines);
        var matrixEnd = FindMatrixEnd(lines);
        var groups = GetGroups(lines, matrixStart, matrixEnd).ToList();

        if (groups.Count == 0)
        {
            throw new InvalidOperationException("Cannot find groups");
        }

        /// GroupRowSize: conto le celle tra i primi 2 * (*---* => 3)
        /// GroupPerMatrixRow: conto quanti * ci sono in una sola riga -1 (*---*---* => 2)
        /// GroupSize: conto le celle tra i primi 2 * in verticale e poi moltiplico per GroupRowSize
        /// GroupCount: conto i * -1 nella prima riga e nella prima colonna e li moltipico
        /// quando incontro un | iniziano le celle

        var groupPerMatrixRow = groups.Where(g => g.Start.Row == groups[0].Start.Row).Count();
        var groupSize = groups[0].CellCount();
        var groupCount = groups.Count;
        var groupRowSize = groups[0].RowSize();

        var relevantCells = groups.SelectMany(g => GetRelevantCells(lines, g)).ToList();
        var lastRow = groups.Last().End.Row;

        var matrixSettings = new MatrixSettings
        {
            GroupCount = groupCount,
            GroupRowSize = groupRowSize,
            GroupPerMatrixRow = groupPerMatrixRow,
            GroupSize = groupSize,
        };

        return (matrixSettings, relevantCells, lastRow);
    }

    private static Point FindMatrixStart(Lines lines)
    {
        for (int ii = 0; ii < lines.Count; ii++)
        {
            var line = lines[ii];

            if (line.Length == 0) continue;

            if (IsMatrixCorner(line[0]))
            {
                return new(ii, 0);
            }
        }

        throw new InvalidOperationException("Matrix starting point not found");
    }

    private static Point FindMatrixEnd(Lines lines)
    {
        Point? end = null;

        for (int ii = 0; ii < lines.Count; ii++)
        {
            var line = lines[ii];

            if (line.Length == 0) continue;

            if (IsMatrixCorner(line.Last()))
            {
                end = new(ii, line.Length - 1);
            }
        }

        if (end is null)
        {
            throw new InvalidOperationException("Matrix ending point not found");
        }

        return end;
    }

    private static IEnumerable<Group> GetGroups(Lines lines, Point matrixStart, Point matrixEnd)
    {
        var groups = new List<Group>();

        var groupRows = lines.GetRange(matrixStart.Row, matrixEnd.Row - matrixStart.Row)
            .Select((line, i) => (new { line, Row = matrixStart.Row + i }))
            .Where(l => IsMatrixCorner(l.line[0]))
            .Select(l => l.Row)
            .ToArray();

        if (groupRows.Length < 2 || groupRows.Length % 2 != 0)
        {
            throw new InvalidOperationException("Matrix not valid: cannot extract groups");
        }

        for (int ii = 0; ii < groupRows.Length - 1; ii++)
        {
            // TODO: no pirla! per ogni riga ci possono essere più gruppi

            var group = new Group(
                Index: ii,
                // non considero la riga della griglia *----*
                Start: new(groupRows[ii] + 1, matrixStart.Col + 1),
                // la riga precedente rappresenta la fine del gruppo corrente
                // non considero la riga della griglia *----*
                End: new(groupRows[ii + 1] - 1, matrixEnd.Col - 1)); 

            groups.Add(group);
        }

        return groups;
    }

    private static IEnumerable<RelevantCell> GetRelevantCells(Lines lines, Group group)
    {
        var cells = new List<RelevantCell>();

        for (int ii = group.Start.Row; ii <= group.End.Row; ii++)
        {
            var line = lines[ii];
            for (int jj = group.Start.Col; jj <= group.End.Col; jj++)
            {
                if (IsRelevantKey(line[jj]))
                {
                    cells.Add(new
                    (
                        Key: line[jj],
                        GroupIndex: group.Index,
                        Row: ii - group.Start.Row,
                        Col: jj - group.Start.Col
                    ));
                }
            }
        }

        return cells;
    }

    private static bool IsComment(char c) => c == '#';
    private static bool IsMatrixCorner(char c) => c == '*';
    private static bool IsMatrixWall(char c) => c == '|';
    private static bool IsRelevantKey(char c) => char.IsAsciiLetter(c);
    private static bool IsConstraintAssignment(char c) => char.IsAsciiLetter(c);
    private static bool IsHelperAssignment(char c) => char.IsAsciiLetterLower(c);
}
