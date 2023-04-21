using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LXNavi.Views;

namespace LXNavi;

public partial class SensorsViewModel : ObservableObject
{
    #region Public Constructors


    public SensorsViewModel(SensorsService sensorsService, TrackDisplayViewModel trackDisplayViewModel)
    {
        _sensorService = sensorsService;
        _trackDisplayViewModel = trackDisplayViewModel;
        LocalizationService.PropertyChanged += LocalizationService_PropertyChanged;
    }

    private void LocalizationService_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (IsMonitoring)
            ToggleButtonText = LocalizationService.Instance["Stop"].ToString();
        else
            ToggleButtonText = LocalizationService.Instance["StartMonitoring"].ToString();
    }

    #endregion Public Constructors

    #region Public Properties

    public LocalizationService LocalizationService => LocalizationService.Instance;
    public SensorViewModel AccelerometerViewModel => _sensorService.AccelerometerViewModel;

    public SensorViewModel GyroscopeViewModel => _sensorService.GyroscopeViewModel;

    public SensorViewModel MagnetometerViewModel => _sensorService.MagnetometerViewModel;

    #endregion Public Properties

    #region Private Fields

    private readonly SensorsService _sensorService;
    private readonly TrackDisplayViewModel _trackDisplayViewModel;
    [ObservableProperty]
    private string _toggleButtonText = LocalizationService.Instance["StartMonitoring"].ToString();
    [ObservableProperty]
    bool _isMonitoring = false;

    #endregion Private Fields

    #region Private Methods

    [RelayCommand]
    private async void Toggle()
    {
        if (IsMonitoring && _trackDisplayViewModel.IsBusy)
        {
            await Shell.Current.DisplayAlert(Emoji.GetEmoji(EmojiType.Warning), StringResource.TrackPlottingNotStopMessage, StringResource.Ok);
            return;
        }
        var stringBuilder = new StringBuilder(Environment.NewLine);
        if (!Accelerometer.Default.IsSupported)
            stringBuilder.AppendLine("Accelerometer");
        if (!Gyroscope.Default.IsSupported)
            stringBuilder.AppendLine("Gyroscope");
        if (!Magnetometer.Default.IsSupported)
            stringBuilder.AppendLine("Magnetometer");
        if (stringBuilder.ToString() != Environment.NewLine)
            await Shell.Current.DisplayAlert("Attention", string.Format(StringResource.SensorsNotSupportedMessage, stringBuilder), StringResource.Ok);
        _sensorService.ToggleAccelerometer();
        _sensorService.ToggleGyroscope();
        _sensorService.ToggleMagnetometer();
        _sensorService.ToggleOrientation();
        IsMonitoring = AccelerometerViewModel.IsMonitoring || GyroscopeViewModel.IsMonitoring || MagnetometerViewModel.IsMonitoring;
        if (IsMonitoring)
            ToggleButtonText = LocalizationService.Instance["Stop"].ToString();
        else
            ToggleButtonText = LocalizationService.Instance["StartMonitoring"].ToString();
    }
    [RelayCommand]
    private async Task GoToDetailAsync(SensorViewModel sensorViewModel)
    {
        await Shell.Current.GoToAsync(nameof(SensorDetailPage), true, new Dictionary<string, object>
        {
            {"SensorViewModel",sensorViewModel }
        });
    }
    [RelayCommand]
    private async void Clear()
    {
        var isYes = await Shell.Current.DisplayAlert(Emoji.GetEmoji(EmojiType.Question), StringResource.ClearingDataMessage, StringResource.Yes, StringResource.No);
        if (!isYes)
            return;
        if (IsMonitoring)
            Toggle();
        AccelerometerViewModel.Clear();
        GyroscopeViewModel.Clear();
        MagnetometerViewModel.Clear();
        _sensorService.AttitudeDatas.Clear();
        _sensorService.IntegratedDatas.Clear();
        await Shell.Current.DisplayAlert(Emoji.GetEmoji(EmojiType.Infomation), StringResource.DataClearedMessage, StringResource.Done);
    }

    [RelayCommand]
    private void SaveAndShare()
    {
        _sensorService.SaveAndShareSensorDatas();
    }

    #endregion Private Methods
}
