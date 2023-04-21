namespace LXNavi;

public class Sensor
{
    #region Public Constructors

    #endregion Public Constructors

    #region Public Properties
    public bool IsMonitoring { get; set; } = false;
    public List<SensorData> Datas { get; init; } = new();

    #endregion Public Properties
}
