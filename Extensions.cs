using Kendoku.Models;

namespace Kendoku;

public static class Extensions
{

    public static string ToHumanString(this Cell cell)
    {
        return $"[{cell.MatrixRow}/{cell.MatrixCol}]{cell.GroupIndex}/{cell.Row}/{cell.Col}";
    }
    
    public static string ToHumanString(this Helper helper)
    {
        return $"Helper {{ cell={helper.Cell} value={helper.Value} }}";
    }

    public static string ToHumanString(this IEnumerable<Helper> helpers)
    {
        return string.Join('\n', helpers.Select(c => c.ToHumanString()));
    }

    public static string ToHumanString(this CellStatus cell)
    {
        return $"{cell.Cell.ToHumanString()} => {string.Join(',', cell.Possibilities)}";
    }

    public static string ToHumanString(this IEnumerable<CellStatus> cells)
    {
        return string.Join('\n', cells.Select(c => c.ToHumanString()));
    }

    public static string ToHumanString(this Constraint constraint)
    {
        var cells = constraint.Cells.Select(cell => cell.ToHumanString());

        return $"Constraint {{ cells={string.Join(',', cells)} sum={constraint.Sum} }}";
    }

    public static string ToHumanString(this Constraint[] constraints)
    {
        return string.Join('\n', constraints.Select(c => c.ToHumanString()));
    }

    public static void Dump(this object @object)
    {
        Console.WriteLine(@object);
    }

    public static void Dump(this object[] @objects, char sep = '\n')
    {
        Console.WriteLine(string.Join(sep, @objects));
    }

    public static int? ToInt(this string text)
    {
        if (int.TryParse(text, out var value))
        {
            return value;
        }
        return null;
    }

    public static string? FirstArgOrDefault(this string[] args, string name)
    {
        for (var ii = 0; ii < args.Length; ii += 2)
        {
            if (args[ii] == name)
            {
                if (args.Length > ii + 1)
                {
                    return args[ii + 1];
                }
                else
                {
                    break;
                }
            }
        }

        return null;
    }

    public static IEnumerable<string> FindArgs(this string[] args, string name)
    {
        for (var ii = 0; ii < args.Length; ii += 2)
        {
            if (args[ii] == name)
            {
                if (args.Length > ii + 1)
                {
                    yield return args[ii + 1];
                }
                else
                {
                    break;
                }
            }
        }
    }
}