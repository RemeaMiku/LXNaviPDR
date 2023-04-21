using System.Numerics;

namespace LXNavi;

public class SensorData
{
    #region Public Constructors

    public SensorData(DateTime dateTime, Vector3 value)
    {
        DateTime = dateTime;
        Value = value;
    }

    #endregion Public Constructors

    #region Public Properties

    public static SensorData Empty { get; } = new(new(), new(float.NaN));
    public DateTime DateTime { get; init; }
    //public GpsTime GpsTime => new(DateTime);
    public Vector3 Value { get; init; }
    public float X => Value.X;
    public float Y => Value.Y;
    public float Z => Value.Z;

    #endregion Public Properties

    #region Public Methods

    public override string ToString()
    {
        return $"{DateTime:yyyy/MM/dd HH/mm/ss.ffff},{X},{Y},{Z}";
    }

    #endregion Public Methods
}
