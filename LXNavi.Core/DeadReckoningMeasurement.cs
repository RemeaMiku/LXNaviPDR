using NaviSharp;

namespace LXNavi.Core;

public class DeadReckoningMeasurement
{
    #region Public Constructors

    public DeadReckoningMeasurement(DateTimeOffset timeStamp, double acceleration, EulerAngle eulerAngle)
    {
        TimeStamp = timeStamp;
        Acceleration = acceleration;
        EulerAngle = eulerAngle;
    }

    #endregion Public Constructors

    #region Public Properties

    public DateTimeOffset TimeStamp { get; init; }
    public double Acceleration { get; init; }
    public EulerAngle EulerAngle { get; init; }

    #endregion Public Properties
}
