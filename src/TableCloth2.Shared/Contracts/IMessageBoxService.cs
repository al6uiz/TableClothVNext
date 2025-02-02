namespace TableCloth2.Contracts;

public interface IMessageBoxService
{
    void ShowInformation(string message, string title);

    void ShowError(string message, string title);

    void ShowWarning(string message, string title);

    bool? ShowYesNoCancel(string message, string title);

    Task ShowInformationAsync(string message, string title);

    Task ShowErrorAsync(string message, string title);

    Task ShowWarningAsync(string message, string title);

    Task<bool?> ShowYesNoCancelAsync(string message, string title);
}
