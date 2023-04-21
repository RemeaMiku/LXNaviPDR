namespace LXNavi.Views;


public partial class SensorDetailPage : ContentPage
{
    #region Public Constructors

    public SensorDetailPage(SensorDetailViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    #endregion Public Constructors

    #region Protected Methods

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        _viewModel.SensorViewModel.IsOnDetailPage = true;
        base.OnNavigatedTo(args);
    }

    protected override void OnNavigatingFrom(NavigatingFromEventArgs args)
    {
        _viewModel.SensorViewModel.IsOnDetailPage = false;
        _viewModel.SensorViewModel.DisplayDatas.Clear();
        base.OnNavigatingFrom(args);
    }

    #endregion Protected Methods

    #region Private Fields

    private readonly SensorDetailViewModel _viewModel;

    #endregion Private Fields
}