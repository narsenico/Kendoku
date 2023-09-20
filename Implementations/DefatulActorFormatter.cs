using Kendoku.Interfaces;
using Kendoku.Models;

namespace Kendoku.Implementations;

public class DefaultActorFormatter : IFormatter
{
    public string Format(object value) => value switch {
        Constraint c => c.ToHumanString(),
        Helper h => h.ToHumanString(),
        null => "nothing",
        _ => value.ToString()!
    };
}
