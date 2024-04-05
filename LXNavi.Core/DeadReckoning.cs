using NaviSharp;
using static System.Math;
using static NaviSharp.Angle;
using System.Numerics;

namespace LXNavi.Core;

public class DeadReckoning
{
    #region Public Properties

    public Vector2 CurrentCoord { get; set; } = new(0, 0);
    public DateTimeOffset LastMeasurementTimeStamp { get; private set; }
    public DateTimeOffset LastUpdateTimeStamp { get; private set; }

    #endregion Public Properties

    #region Public Constructors

    public DeadReckoning(DateTimeOffset timeStamp)
    {
        LastMeasurementTimeStamp = timeStamp;
        LastUpdateTimeStamp = timeStamp;
    }

    #endregion Public Constructors

    #region Public Methods

    public Vector2 GetCurrentCoord(DeadReckoningMeasurement measurement)
    {
        if (measurement.TimeStamp < LastMeasurementTimeStamp)
            throw new ArgumentException($"{nameof(measurement.TimeStamp)} of {nameof(measurement)} is supposed to be greater than the last time ", nameof(measurement));
        if (measurement.TimeStamp - LastUpdateTimeStamp < _minimumInterval)
            return CurrentCoord;
        var acceleration = measurement.Acceleration;
        var yaw = measurement.EulerAngle.Yaw;
        var interval = measurement.TimeStamp - LastMeasurementTimeStamp;
        LastMeasurementTimeStamp = measurement.TimeStamp;
        if (interval > _maximumInterval)
        {
            _accelerationWindow.Clear();
            _yawWindow.Clear();
            _lastAcceleration = double.MaxValue;
        }
        _accelerationWindow.Enqueue(acceleration);
        _yawWindow.Enqueue(yaw);
        if (_accelerationWindow.Count > 3)
            _accelerationWindow.Dequeue();
        if (_yawWindow.Count > 5)
            _yawWindow.Dequeue();
        var isStep = _lastAcceleration < _stepThreshold && _accelerationWindow.Count == 3 && _lastAcceleration == _accelerationWindow.Min();
        _lastAcceleration = acceleration;
        if (!isStep)
            return CurrentCoord;
        LastUpdateTimeStamp = measurement.TimeStamp;
        if (_yawWindow.Count > 0)
            yaw = new(_yawWindow.Average(yaw => yaw.Radians));
        var delta = new Vector2((float)(StepLength * Sin(yaw)), (float)(StepLength * Cos(yaw)));
        return CurrentCoord += delta;
    }

    #endregion Public Methods

    #region Private Fields

    public static double StepLength { get; set; } = 0.4;

    private const double _stepThreshold = 9.2;
    private readonly static TimeSpan _minimumInterval = TimeSpan.FromSeconds(0.2);

    private readonly static TimeSpan _maximumInterval = TimeSpan.FromSeconds(1);

    private readonly Queue<Angle> _yawWindow = new();

    private readonly Queue<double> _accelerationWindow = new();

    private double _lastAcceleration = double.MaxValue;

    #endregion Private Fields
}
