using LXNavi.Views;

namespace LXNavi;

public partial class AppShell : Shell
{
    #region Public Constructors

    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(SensorDetailPage), typeof(SensorDetailPage));
        BindingContext = new AppShellViewModel();
    }

    #endregion Public Constructors
}
