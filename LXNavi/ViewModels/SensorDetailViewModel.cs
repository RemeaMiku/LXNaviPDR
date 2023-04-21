using CommunityToolkit.Mvvm.ComponentModel;

namespace LXNavi;

[QueryProperty(nameof(SensorViewModel), "SensorViewModel")]
public partial class SensorDetailViewModel : ObservableObject
{
    #region Public Fields

    public LocalizationService LocalizationService => LocalizationService.Instance;

    [ObservableProperty]
    public SensorViewModel sensorViewModel;



    #endregion Public Fields
}
