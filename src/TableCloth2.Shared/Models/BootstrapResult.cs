namespace TableCloth2.Shared.Models;

public sealed class BootstrapResult
{
    private bool _isSuccessful;
    private string? _errorMessage;
    private List<string> _warnings = new List<string>();

    public bool IsSuccessful
    {
        get => _isSuccessful;
        set => _isSuccessful = value;
    }

    public string? ErrorMessage
    {
        get => _errorMessage;
        set => _errorMessage = value;
    }

    public IList<string> Warnings
    {
        get => _warnings;
    }
}
