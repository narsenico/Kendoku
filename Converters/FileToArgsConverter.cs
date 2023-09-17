using Kendoku;

using System.Text.RegularExpressions;

using KeyValuePairs = System.Collections.Generic.Dictionary<char, int>;
using Lines = System.Collections.Generic.List<string>;

namespace kendoku.Converters;

/// <summary>
/// 
/// Esempio file:
/// 
/// # griglia 2x3, con blocchi 3x2
/// # a lettere uguali (case insensitive) corrisponde un vincolo, il cui valore è riportato in coda
/// # a una lettera minuscola corrisponde un aiuto, il cui valore è riportato in coda
/// # a un numero corrisponde un aiuto
/// 
/// *---*---*
/// |AaB|   |
/// |CAB|   |
/// *---*---*
/// |C  |   |
/// |   |   |
/// *---*---*
/// | 5 |   |
/// |   |   |
/// *---*---*
/// 
/// A = 13
/// B=6
/// C=3
/// a=3
/// 
/// </summary>
internal partial class FileToArgsConverter
{
    private record Point(int Row, int Col)
    {
        public Point ShiftBy(int row, int col) => new(Row + row, Col + col);
    }

    private record Group(int Index, Point Start, Point End)
    {
        public int RowSize() => End.Col - Start.Col + 1;
        public int ColSize() => End.Row - Start.Row + 1;
        public int CellCount() => RowSize() * ColSize();
    }

    private record RelevantCell(char Key, int GroupIndex, int Row, int Col)
    {
        public override string ToString()
        {
            return $"{GroupIndex}:{Row}:{Col},{Key}";
        }
    }

    public FileToArgsConverter()
    {

    }

    public string[] ConvertToArgs(string fileName)
    {
        var lines = ReadAllLines(fileName);
        var (matrixSettings, relevantCells, offset) = ParseMatrix(lines);
        var keyValuePairs = ParseKeyValuePairs(lines, offset);

        var args = new Lines()
        {
            "-c", matrixSettings.GroupCount.ToString(),
            "-s", matrixSettings.GroupSize.ToString(),
            "-r", matrixSettings.GroupRowSize.ToString(),
            "-m", matrixSettings.GroupPerMatrixRow.ToString(),
        };

        args.AddRange(relevantCells
            .Where(c => IsHelperKey(c.Key))
            .Select(c => ConvertRelevantCellToHelperArg(c, keyValuePairs))
            .PrefixWith("-l"));

        args.AddRange(relevantCells
            .Where(c => IsConstraintKey(c.Key))
            .GroupBy(c => char.ToUpper(c.Key))
            .Select(g => ConvertRelevantCellsToConstraintArg(g.Key, g, keyValuePairs))
            .PrefixWith("-t"));

        return args.ToArray();
    }

    private static Lines ReadAllLines(string fileName)
    {
        using var reader = File.OpenText(fileName);
        return File.ReadAllLines(fileName).ToList();
    }

    private static (MatrixSettings, List<RelevantCell>, int) ParseMatrix(Lines lines)
    {
        var matrixStart = FindMatrixStart(lines);
        var matrixEnd = FindMatrixEnd(lines);
        var groups = GetGroups(lines, matrixStart, matrixEnd).ToArray();

        if (groups.Length == 0)
        {
            throw new ConvertionFailedException("Cannot find matrix");
        }

        var groupPerMatrixRow = groups.GroupBy(g => g.Start.Row).Select(g => g.Count()).First();
        var groupSize = groups[0].CellCount();
        var groupCount = groups.Length;
        var groupRowSize = groups[0].RowSize();

        var relevantCells = groups.SelectMany(g => GetRelevantCells(lines, g)).ToList();
        var lastRow = matrixEnd.Row + 1;

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

        throw new ConvertionFailedException("Matrix starting point not found");
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
            throw new ConvertionFailedException("Matrix ending point not found");
        }

        return end;
    }

    private static IEnumerable<Group> GetGroups(Lines lines, Point matrixStart, Point matrixEnd)
    {
        var cornerPoints = lines.GetRange(matrixStart.Row, matrixEnd.Row - matrixStart.Row + 1)
            .SelectMany((line, row) => GetMatrixCornerPoints(row, line))
            .Select(p => p.ShiftBy(matrixStart.Row, matrixStart.Col)) // le posizioni sono relative al range, devo renderle assolute aggiungendo il numero di righe e colonne date dal punto di partenza della matrice
            .ToArray();

        var groups = new List<Group>();
        var index = 0; // mi fido del fatto che le posizioni siano tutte belle ordinate e quindi creo i gruppi nell'ordine corretto semplicemente incrementando di 1 l'indice ogni volta
        foreach (var startPoint in cornerPoints)
        {
            var endPoint = cornerPoints.FirstOrDefault(p => p.Row > startPoint.Row && p.Col > startPoint.Col);
            if (endPoint is not null)
            {
                // escludo la riga e la colonna dei marcatori
                var group = new Group(
                    Index: index++,
                    Start: startPoint.ShiftBy(1, 1),
                    End: endPoint.ShiftBy(-1, -1)
                );

                groups.Add(group);
            }
        }

        return groups;
    }

    private static IEnumerable<Point> GetMatrixCornerPoints(int row, string line)
    {
        for (int ii = 0; ii < line.Length; ii++)
        {
            if (IsMatrixCorner(line[ii]))
            {
                yield return new Point(row, ii);
            }
        }
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

    private static KeyValuePairs ParseKeyValuePairs(Lines lines, int offset)
    {
        return lines.Skip(offset)
            .Select(line => KeyValuePairRegex().Match(line))
            .Where(m => m.Success)
            .Select(m => (m.Groups[1].Value, m.Groups[2].Value))
            .ToDictionary(o => o.Item1[0], o => o.Item2.ToInt()!.Value);
    }

    private static string ConvertRelevantCellToHelperArg(RelevantCell cell, KeyValuePairs keyValuePairs)
    {
        if (!keyValuePairs.TryGetValue(cell.Key, out var value))
        {
            value = $"{cell.Key}".ToInt() ?? throw new ConvertionFailedException($"Cannot create helper for cell {cell}");
        }

        // g:r:c,v
        return $"{cell.GroupIndex}:{cell.Row}:{cell.Col},{value}";
    }

    private static string ConvertRelevantCellsToConstraintArg(char key,
                                                              IEnumerable<RelevantCell> cells,
                                                              KeyValuePairs keyValuePairs)
    {
        if (!keyValuePairs.TryGetValue(key, out var value))
        {
            throw new ConvertionFailedException($"Cannot create constraint with value {key}");
        }

        // g:r:c[,...g:r:c],v
        return $"{string.Join(',', cells.Select(cell => $"{cell.GroupIndex}:{cell.Row}:{cell.Col}"))},{value}";
    }

    private static bool IsMatrixCorner(char c) => c == '*';
    private static bool IsHelperKey(char c) => char.IsAsciiLetterLower(c) || char.IsNumber(c);
    private static bool IsConstraintKey(char c) => char.IsAsciiLetter(c);
    private static bool IsRelevantKey(char c) => char.IsAsciiLetter(c) || char.IsNumber(c);
    
    [GeneratedRegex("([A-Za-z])=(\\d+)")]
    private static partial Regex KeyValuePairRegex();
}
