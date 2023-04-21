namespace LXNavi;

public partial class MainPage : ContentPage
{
    #region Public Constructors

    public MainPage(SensorsViewModel sensorsViewModel)
    {
        InitializeComponent();
        BindingContext = sensorsViewModel;
    }

    #endregion Public Constructors


}

