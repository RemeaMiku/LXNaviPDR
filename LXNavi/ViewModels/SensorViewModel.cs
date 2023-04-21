using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace LXNavi;

public partial class SensorViewModel : ObservableObject
{
    private readonly string _sensorKey;
    #region Public Constructors

    public SensorViewModel(string sensorKey, string unit, Sensor sensor)
    {
        //Name = name;
        _sensorKey = sensorKey;
        Unit = unit;
        _sensor = sensor;
    }

    #endregion Public Constructors

    #region Public Properties

    public string Name => LocalizationService.Instance[_sensorKey].ToString();

    public string Unit { get; }

    public ObservableCollection<SensorData> DisplayDatas { get; } = new();

    public bool IsOnDetailPage { get; set; } = false;

    public SensorData LatestData
    {
        get => _sensor.Datas.Count == 0 ? SensorData.Empty : _sensor.Datas[^1];
        set
        {
            _sensor.Datas.Add(value);
            if (IsOnDetailPage)
            {
                DisplayDatas.Add(value);
                if (DisplayDatas.Count > 100)
                    DisplayDatas.RemoveAt(0);
                OnPropertyChanged(nameof(DisplayDatas));
                return;
            }
            OnPropertyChanged(nameof(LatestData));
            OnPropertyChanged(nameof(LatestDataText));
        }
    }

    public string LatestDataText => _sensor.Datas.Count == 0 ? LocalizationService.Instance["NoData"].ToString() : $"X:{LatestData.X} {Unit}{Environment.NewLine}Y:{LatestData.Y} {Unit}{Environment.NewLine}Z:{LatestData.Z} {Unit}";

    public bool IsMonitoring
    {
        get => _sensor.IsMonitoring;
        set
        {
            if (value == _sensor.IsMonitoring)
                return;
            _sensor.IsMonitoring = value;
            OnPropertyChanged(nameof(IsMonitoring));
        }
    }

    #endregion Public Properties

    #region Public Methods

    public void Clear()
    {
        if (_sensor.Datas.Count == 0)
            return;
        _sensor.Datas.Clear();
        OnPropertyChanged(nameof(LatestData));
        OnPropertyChanged(nameof(LatestDataText));
    }

    #endregion Public Methods

    #region Private Fields

    private readonly Sensor _sensor;

    #endregion Private Fields
}
