using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using TableCloth2.Shared;
using TableCloth2.Shared.Models.Catalog;
using TableCloth2.Spork.ViewModels;

namespace TableCloth2.Spork;

public partial class SporkForm : Form
{
    internal SporkForm()
    {
        InitializeComponent();
    }

    public SporkForm(
        SporkViewModel viewModel,
        IMessenger messenger)
        : this()
    {
        _viewModel = viewModel;
        _messenger = messenger;

        SuspendLayout();

        _messenger.Register<AsyncRequestMessage<bool>, int>(
            this, (int)Messages.LoadImageList, OnLoadImageList);

        launchButton.Bind(c => c.Command, _viewModel, v => v.LaunchCommand);

        Load += viewModel.InitializeCommand.ToEventHandler();
        listView.ItemSelectionChanged += ListView_ItemSelectionChanged;
        listView.ItemActivate += viewModel.LaunchCommand.ToEventHandler();

        ResumeLayout();
    }

    private readonly SporkViewModel _viewModel = default!;
    private readonly IMessenger _messenger = default!;

    private void ListView_ItemSelectionChanged(object? sender, ListViewItemSelectionChangedEventArgs e)
    {
        if (e.Item?.Tag is not CatalogInternetService service)
            return;

        if (e.IsSelected)
        {
            if (!_viewModel.SelectedServices.Contains(service))
                _viewModel.SelectedServices.Add(service);
        }
        else
        {
            if (_viewModel.SelectedServices.Contains(service))
                _viewModel.SelectedServices.Remove(service);
        }
    }

    private void OnLoadImageList(object recipient, AsyncRequestMessage<bool> message)
    {
        foreach (var eachImage in _viewModel.Images)
        {
            using var image = Image.FromFile(eachImage.Value);
            largeImageList.Images.Add(eachImage.Key, image);
            smallImageList.Images.Add(eachImage.Key, image);
        }

        foreach (var eachCategory in Enum.GetNames<CatalogInternetServiceCategory>())
        {
            var displayName = CommonStrings.ResourceManager.GetString(
                $"DisplayName_{eachCategory.ToString()}");

            if (string.IsNullOrWhiteSpace(displayName))
                displayName = eachCategory.ToString();

            var group = new ListViewGroup(eachCategory.ToString(), displayName)
            {
                Tag = eachCategory,
            };

            listView.Groups.Add(group);
        }

        listView.Columns.Add(
            nameof(CatalogInternetService.DisplayName),
            "Name");
        listView.Columns.Add(
            nameof(CatalogInternetService.Category),
            "Category");
        listView.Columns.Add(
            nameof(CatalogInternetService.Url),
            "URL");

        foreach (var eachService in _viewModel.Catalog.Services)
        {
            var item = new ListViewItem(eachService.DisplayName, eachService.Id)
            {
                Tag = eachService,
                Group = listView.Groups[eachService.Category.ToString()],
                Name = nameof(eachService.DisplayName),
            };

            item.SubItems.Add(new ListViewItem.ListViewSubItem(
                item, eachService.Category.ToString())
            { Name = nameof(eachService.Category), });

            item.SubItems.Add(new ListViewItem.ListViewSubItem(
                item, eachService.Url)
            { Name = nameof(eachService.Url), });

            listView.Items.Add(item);
        }

        listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        message.Reply(true);
    }
}
