using static NaviSharp.EarthEllipsoid;
using LXNavi.Core;

namespace LXNavi;

public partial class DeadReckoningService
{
    #region Public Properties

    public Location InitialLocation { get; private set; }

    public Location CurrentLocation { get; private set; }

    #endregion Public Properties

    #region Public Methods

    public void Initialize(Location initialLocation)
    {
        InitialLocation = initialLocation;
        DeadReckoning = new(initialLocation.Timestamp);
    }

    public Location UpDate(DeadReckoningMeasurement measurement)
    {
        var currentCoord = DeadReckoning.GetCurrentCoord(measurement);
        var initialLatitude = FromDegrees(InitialLocation.Latitude);
        var deltaLon = UnitConverters.RadiansToDegrees(currentCoord.X / Wgs84.M(initialLatitude));
        var deltaLat = UnitConverters.RadiansToDegrees(currentCoord.Y / (Wgs84.N(initialLatitude) * Cos(initialLatitude)));
        CurrentLocation = new Location(InitialLocation.Latitude + deltaLat, InitialLocation.Longitude + deltaLon, DeadReckoning.LastMeasurementTimeStamp);
        return CurrentLocation;
    }


    #endregion Public Methods

    #region Private Fields

    public DeadReckoning DeadReckoning { get; set; }

    #endregion Private Fields
}
