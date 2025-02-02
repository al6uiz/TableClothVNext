using Microsoft.Extensions.Configuration;
using TableCloth2.Shared.Models;

namespace TableCloth2;

public sealed class Configurations(IConfiguration Configuration)
{
    public ApplicationMode? Mode =>
        Enum.TryParse<ApplicationMode>(Configuration[nameof(Mode)], true, out var result) ? result : null;

    public bool Privileged =>
        string.Equals(Configuration[nameof(Privileged)], bool.TrueString, StringComparison.OrdinalIgnoreCase);
}
