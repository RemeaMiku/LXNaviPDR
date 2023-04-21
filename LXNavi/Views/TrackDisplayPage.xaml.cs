using System.Diagnostics;
using Microsoft.Maui.Controls.Maps;
using Syncfusion.Maui.Backdrop;

namespace LXNavi.Views;

public partial class TrackDisplayPage : SfBackdropPage
{
    #region Public Constructors

    public TrackDisplayPage(TrackDisplayViewModel trackDisplayViewModel, SensorsViewModel sensorsViewModel, SensorsService sensorsService, DeadReckoningService deadReckoningService, IGeolocation geolocation, IConnectivity connectivity)
    {
        InitializeComponent();
        BindingContext = trackDisplayViewModel;
        _trackDisplayViewModel = trackDisplayViewModel;
        _sensorsViewModel = sensorsViewModel;
        _sensorsService = sensorsService;
        _deadReckoningService = deadReckoningService;
        _geolocation = geolocation;
        _connectivity = connectivity;
        MapView.MoveToRegion(new(new(30.6, 114.3), 0.05, 0.05));
    }

    #endregion Public Constructors

    #region Private Fields

    private readonly SensorsService _sensorsService;
    private readonly DeadReckoningService _deadReckoningService;
    private readonly IGeolocation _geolocation;
    private readonly IConnectivity _connectivity;
    private readonly TimeSpan _updateInterval = TimeSpan.FromSeconds(1);

    private readonly CancellationTokenSource _cancellationTokenSource = new();

    private readonly List<Location> _trackPoints = new(500);

    private readonly SensorsViewModel _sensorsViewModel;

    private readonly TrackDisplayViewModel _trackDisplayViewModel;

    private DateTimeOffset _lastUpdateTime;
    private CustomPin _pin;
    private Polyline _track;

    #endregion Private Fields

    #region Private Methods

    private void UpdateMapView(Location location)
    {
        _trackPoints.Add(location);
        if (MapView.MapElements.Count > 0)
            MapView.MapElements.Clear();
        _track = new Polyline
        {
            StrokeWidth = 10,
            StrokeColor = Color.FromArgb(_trackDisplayViewModel.StrokeColor),
        };
        foreach (var point in _trackPoints)
        {
            _track.Add(point);
        }
        MapView.MapElements.Add(_track);
        if (MapView.Pins.Count > 0)
            MapView.Pins.Clear();
        _pin = new()
        {
            Type = PinType.Place,
            Label = StringResource.CurrentLocation,
            Address = $"Lat:{location.Latitude:F4}°,Lon:{location.Longitude:F4}°",
            Location = location,
            ImageSource = ImageSource.FromFile("Resources/Images/pin.png")
        };
        MapView.Pins.Add(_pin);
    }

    private void UpdateTrackPoints(object sender, SensorsService.IntegratedDatasChangedEventArgs e)
    {
        var lastLocation = _trackPoints[^1];
        var currentLocation = _deadReckoningService.UpDate(e.Measurement);
        var currentTime = DateTimeOffset.Now;
        if (currentTime - _lastUpdateTime <= _updateInterval)
            return;
        if (Location.CalculateDistance(lastLocation, currentLocation, DistanceUnits.Kilometers) < 0.001)
            return;
        Debug.WriteLine($"Add Track:{currentLocation}, Track.Count={_trackPoints.Count}");
        _lastUpdateTime = currentTime;
        UpdateMapView(currentLocation);
        _trackDisplayViewModel.CurrentLocation = currentLocation;
        if (!_trackDisplayViewModel.IsKeepingCentered)
            return;
        var region = MapView.VisibleRegion;
        MapView.MoveToRegion(new(currentLocation, region.LatitudeDegrees, region.LongitudeDegrees));
    }

    private async void ToggleButton_Clicked(object sender, EventArgs e)
    {
        if (!_trackDisplayViewModel.IsBusy)
        {
            if (!_sensorsViewModel.IsMonitoring)
            {
                await Shell.Current.DisplayAlert(Emoji.GetEmoji(EmojiType.Warning), StringResource.MonitoringNotStartedMessage, StringResource.Ok);
                return;
            }
            _trackDisplayViewModel.IsBusy = true;
            if (_connectivity.NetworkAccess != NetworkAccess.Internet)
                await Shell.Current.DisplayAlert(Emoji.GetEmoji(EmojiType.Warning), StringResource.NetworkDisabledMessage, StringResource.Ok);
            var request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(3));
            try
            {
                var location = await _geolocation.GetLocationAsync(request);
                if (location is null)
                {
                    await Shell.Current.DisplayAlert(Emoji.GetEmoji(EmojiType.Error), StringResource.NullLocationMessage, StringResource.Ok);
                    _cancellationTokenSource.Cancel();
                    _trackDisplayViewModel.IsBusy = false;
                    return;
                }
                _lastUpdateTime = DateTimeOffset.Now;
                _trackDisplayViewModel.CurrentLocation = location;
                location = CoordTransformation.Wgs84ToGcj02(location);
                _deadReckoningService.Initialize(location);
                UpdateMapView(location);
                MapView.MoveToRegion(new(location, 0.005, 0.005));
            }
            catch (Exception)
            {
                await Shell.Current.DisplayAlert(Emoji.GetEmoji(EmojiType.Error), StringResource.LocationDisabledMessage, StringResource.Ok);
                _trackDisplayViewModel.IsBusy = false;
                return;
            }
            _sensorsService.IntegratedDatasChanged += UpdateTrackPoints;
        }
        else
        {
            _sensorsService.IntegratedDatasChanged -= UpdateTrackPoints;
            _cancellationTokenSource.Cancel();
            _trackPoints.Clear();
            MapView.Pins.Clear();
            _trackDisplayViewModel.IsBusy = false;
            _trackDisplayViewModel.CurrentLocation = null;
        }
    }

    #endregion Private Methods
}