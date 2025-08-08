using Microsoft.Extensions.Configuration;
using TableCloth3.Shared.Models;

namespace TableCloth3.Shared.Services;

public sealed class ScenarioRouter
{
    public ScenarioRouter(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private readonly IConfiguration _configuration = default!;

    public Scenario GetScenario()
    {
        var modeString = _configuration["Mode"];

        if (!Enum.TryParse<Scenario>(modeString, true, out var scenarioValue))
            return default;

        return scenarioValue;
    }

    public SporkScenario GetSporkScenario()
    {
        var modeString = _configuration["SporkMode"];

        if (!Enum.TryParse<SporkScenario>(modeString, true, out var scenarioValue))
            return default;

        return scenarioValue;
    }

    public Uri? GetSporkTargetUri()
    {
        if (GetScenario() != Scenario.Lite)
            return default;
        var uriString = _configuration["LiteTargetUri"];
        if (string.IsNullOrWhiteSpace(uriString))
            return default;
        if (!Uri.TryCreate(uriString, UriKind.Absolute, out var uri))
            return default;
        return uri;
    }
}
