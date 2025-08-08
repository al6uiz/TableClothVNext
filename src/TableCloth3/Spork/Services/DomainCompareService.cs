using Nager.PublicSuffix;
using Nager.PublicSuffix.RuleProviders;
using Nager.PublicSuffix.RuleProviders.CacheProviders;
using System.Net;

namespace TableCloth3.Spork.Services;

public sealed class DomainCompareService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private IDomainParser? _parser;

    public DomainCompareService(
        IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    private async Task<IDomainParser> InitializeParserAsync(CancellationToken cancellationToken = default)
    {
        if (_parser != null)
            return _parser;

        using var httpClient = _httpClientFactory.CreateClient();
        var cacheProvider = new LocalFileSystemCacheProvider();
        var ruleProvider = new CachedHttpRuleProvider(cacheProvider, httpClient);
        await ruleProvider.BuildAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

        return _parser = new DomainParser(ruleProvider);
    }

    public async Task<bool> IsSameDomainAsync(string url1, string url2, CancellationToken cancellationToken = default)
    {
        var parser = await InitializeParserAsync(cancellationToken).ConfigureAwait(false);

        if (parser == null)
            throw new Exception("Cannot initialize domain parser.");

        if (!TryGetUri(url1, out var u1) || !TryGetUri(url2, out var u2))
            return false;

        if (u1 == null || u2 == null)
            return false;

        var h1 = GetIdnHost(u1);
        var h2 = GetIdnHost(u2);

        if (IsIpLike(h1) || IsIpLike(h2))
            return string.Equals(h1, h2, StringComparison.OrdinalIgnoreCase);

        var d1 = parser.Parse(h1);
        var d2 = parser.Parse(h2);

        var r1 = d1?.RegistrableDomain;
        var r2 = d2?.RegistrableDomain;

        if (!string.IsNullOrEmpty(r1) && !string.IsNullOrEmpty(r2))
            return string.Equals(r1, r2, StringComparison.OrdinalIgnoreCase);

        return string.Equals(h1, h2, StringComparison.OrdinalIgnoreCase);
    }

    private bool TryGetUri(string? input, out Uri? uri)
        => Uri.TryCreate(input, UriKind.Absolute, out uri) && !string.IsNullOrEmpty(uri?.Host);

    private string GetIdnHost(Uri uri)
        => !string.IsNullOrEmpty(uri.IdnHost) ? uri.IdnHost : uri.Host;

    private bool IsIpLike(string host)
        => IPAddress.TryParse(host, out _) || host.Contains(":");
}
