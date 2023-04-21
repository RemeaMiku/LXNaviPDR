using CommunityToolkit.Mvvm.ComponentModel;
using LXNavi.Core;

namespace LXNavi;

public partial class TrackDisplayViewModel : ObservableObject
{
    #region Public Constructors

    public TrackDisplayViewModel()
    {
        LocalizationService.PropertyChanged += LocalizationService_PropertyChanged;
    }

    #endregion Public Constructors

    #region Private Methods

    private void LocalizationService_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (CurrentLocationText.Contains(':'))
            return;
        CurrentLocationText = StringResource.NoData;
        OnPropertyChanged(nameof(CurrentLocationText));
    }

    #endregion Private Methods


    #region Private Fields

    [ObservableProperty]
    private bool _isBusy = false;
    [ObservableProperty]
    private string _strokeColor = "#39C5BB";
    [ObservableProperty]
    private bool _isKeepingCentered = true;
    [ObservableProperty]
    private string _currentLocationText = StringResource.NoData;

    public double StepLength
    {
        get => DeadReckoning.StepLength;
        set
        {
            DeadReckoning.StepLength = value;
        }
    }

    public LocalizationService LocalizationService => LocalizationService.Instance;
    public Location CurrentLocation
    {
        set
        {
            CurrentLocationText = value is null ? StringResource.NoData : $"UTC:{value.Timestamp:yyyy/MM/dd HH:mm:ss}, Lat:{value.Latitude:F4}°, Lon:{value.Longitude:F4}°";
        }
    }
    #endregion Private Fields
}
