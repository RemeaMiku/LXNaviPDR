namespace LXNavi;

public partial class App : Application
{
    #region Public Constructors

    public App()
    {
        InitializeComponent();
        MainPage = new AppShell();
    }

    #endregion Public Constructors
}
