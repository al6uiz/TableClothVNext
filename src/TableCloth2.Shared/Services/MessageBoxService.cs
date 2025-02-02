using TableCloth2.Contracts;
using WindowsFormsLifetime;

namespace TableCloth2.Services;

internal class MessageBoxService : IMessageBoxService
{
    public MessageBoxService(IFormProvider formProvider)
    {
        _formProvider = formProvider;
    }

    private readonly IFormProvider _formProvider;

    public void ShowInformation(string message, string title) =>
        MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);

    public void ShowError(string message, string title) =>
        MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);

    public void ShowWarning(string message, string title) =>
        MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);

    public bool? ShowYesNoCancel(string message, string title) =>
        MessageBox.Show(message, title, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) switch
        {
            DialogResult.Yes => true,
            DialogResult.No => false,
            _ => null,
        };

    public async Task ShowErrorAsync(string message, string title)
    {
        var mainForm = await _formProvider.GetMainFormAsync().ConfigureAwait(false);
        mainForm.Invoke(() => MessageBox.Show(mainForm, message, title, MessageBoxButtons.OK, MessageBoxIcon.Error));
    }

    public async Task ShowInformationAsync(string message, string title)
    {
        var mainForm = await _formProvider.GetMainFormAsync().ConfigureAwait(false);
        mainForm.Invoke(() => MessageBox.Show(mainForm, message, title, MessageBoxButtons.OK, MessageBoxIcon.Information));
    }

    public async Task ShowWarningAsync(string message, string title)
    {
        var mainForm = await _formProvider.GetMainFormAsync().ConfigureAwait(false);
        mainForm.Invoke(() => MessageBox.Show(mainForm, message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning));
    }

    public async Task<bool?> ShowYesNoCancelAsync(string message, string title)
    {
        var mainForm = await _formProvider.GetMainFormAsync().ConfigureAwait(false);

        return mainForm.Invoke(() => MessageBox.Show(mainForm, message, title, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question)) switch
        {
            DialogResult.Yes => true,
            DialogResult.No => false,
            _ => null,
        };
    }
}
