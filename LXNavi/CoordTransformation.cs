using NaviSharp.SpatialReference;
using static System.Math;

namespace LXNavi;

public static class CoordTransformation
{
    #region Public Methods

    /// <summary>
    /// https://github.com/wandergis/coordTransform_py/blob/master/coordTransform_utils.py
    /// </summary>
    /// <param name="wgs84Location"></param>
    /// <returns></returns>
    public static Location Wgs84ToGcj02(Location wgs84Location)
    {
        if (!IsInChina(wgs84Location))
            return wgs84Location;
        var lon = wgs84Location.Longitude;
        var lat = wgs84Location.Latitude;
        double dLat = TransformLat(lon - 105.0, lat - 35.0);
        double dLon = TransformLon(lon - 105.0, lat - 35.0);
        double radLat = UnitConverters.DegreesToRadians(lat);
        double magic = Sin(radLat);
        var ee = Pow(EarthEllipsoid.Wgs84.E1, 2);
        magic = 1 - ee * magic * magic;
        double sqrtMagic = Sqrt(magic);
        dLat = (dLat * 180.0) / ((EarthEllipsoid.Wgs84.A * (1 - ee)) / (magic * sqrtMagic) * PI);
        dLon = (dLon * 180.0) / (EarthEllipsoid.Wgs84.A / sqrtMagic * Cos(radLat) * PI);
        double mgLat = lat + dLat;
        double mgLon = lon + dLon;
        return new(mgLat, mgLon);
    }

    #endregion Public Methods

    #region Private Methods

    private static bool IsInChina(Location location)
        => location.Longitude > 73.66 && location.Longitude < 135.05 && location.Latitude > 3.86 && location.Latitude < 53.55;

    private static double TransformLat(double x, double y)
    {
        var delta = -100.0 + 2.0 * x + 3.0 * y + 0.2 * y * y + 0.1 * x * y + 0.2 * Sqrt(Abs(x));
        delta += (20.0 * Sin(6.0 * x * PI) + 20.0 * Sin(2.0 * x * PI)) * 2.0 / 3.0;
        delta += (20.0 * Sin(y * PI) + 40.0 * Sin(y / 3.0 * PI)) * 2.0 / 3.0;
        delta += (160.0 * Sin(y / 12.0 * PI) + 320 * Sin(y * PI / 30.0)) * 2.0 / 3.0;
        return delta;
    }

    private static double TransformLon(double x, double y)
    {
        var delta = 300.0 + x + 2.0 * y + 0.1 * x * x + 0.1 * x * y + 0.1 * Sqrt(Abs(x));
        delta += (20.0 * Sin(6.0 * x * PI) + 20.0 * Sin(2.0 * x * PI)) * 2.0 / 3.0;
        delta += (20.0 * Sin(x * PI) + 40.0 * Sin(x / 3.0 * PI)) * 2.0 / 3.0;
        delta += (150.0 * Sin(x / 12.0 * PI) + 300.0 * Sin(x / 30.0 * PI)) * 2.0 / 3.0;
        return delta;
    }

    #endregion Private Methods
}
