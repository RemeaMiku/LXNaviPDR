namespace LXNavi.Views;

public partial class OptionsPage : ContentPage
{
    #region Public Constructors

    public OptionsPage(OptionsViewModel optionsViewModel)
    {
        InitializeComponent();
        BindingContext = optionsViewModel;
    }

    #endregion Public Constructors
}