using Microsoft.Maui.Maps;
using Microsoft.Maui.Maps.Handlers;
using IMap = Microsoft.Maui.Maps.IMap;
using Microsoft.Maui.Platform;
#pragma warning disable CS8632
namespace LXNavi;


#if ANDROID
using Android.Gms.Maps.Model;
using Android.Gms.Maps;
using Android.Graphics.Drawables;
public class CustomMapHandler : MapHandler
{
    public static readonly IPropertyMapper<IMap, IMapHandler> CustomMapper =
        new PropertyMapper<IMap, IMapHandler>(Mapper)
        {
            [nameof(IMap.Pins)] = MapPins,
        };

    public CustomMapHandler() : base(CustomMapper, CommandMapper)
    {
    }

    public CustomMapHandler(IPropertyMapper? mapper = null, CommandMapper? commandMapper = null) : base(
        mapper ?? CustomMapper, commandMapper ?? CommandMapper)
    {
    }

    public List<Marker> Markers { get; } = new();

    protected override void ConnectHandler(MapView platformView)
    {
        base.ConnectHandler(platformView);
        var mapReady = new MapCallbackHandler(this);
        PlatformView.GetMapAsync(mapReady);
    }

    private static new void MapPins(IMapHandler handler, IMap map)
    {
        if (handler is CustomMapHandler mapHandler)
        {
            foreach (var marker in mapHandler.Markers)
            {
                marker.Remove();
            }

            mapHandler.AddPins(map.Pins);
        }
    }

    private void AddPins(IEnumerable<IMapPin> mapPins)
    {
        if (Map is null || MauiContext is null)
        {
            return;
        }

        foreach (var pin in mapPins)
        {
            var pinHandler = pin.ToHandler(MauiContext);
            if (pinHandler is IMapPinHandler mapPinHandler)
            {
                var markerOption = mapPinHandler.PlatformView;
                if (pin is CustomPin cp)
                {
                    cp.ImageSource.LoadImage(MauiContext, result =>
                    {
                        if (result?.Value is BitmapDrawable bitmapDrawable)
                        {
                            markerOption.SetIcon(BitmapDescriptorFactory.FromBitmap(bitmapDrawable.Bitmap));
                        }

                        AddMarker(Map, pin, Markers, markerOption);
                    });
                }
                else
                {
                    AddMarker(Map, pin, Markers, markerOption);
                }
            }
        }
    }

    private static void AddMarker(GoogleMap map, IMapPin pin, List<Marker> markers, MarkerOptions markerOption)
    {
        var marker = map.AddMarker(markerOption);
        pin.MarkerId = marker.Id;
        markers.Add(marker);
    }
}
class MapCallbackHandler : Java.Lang.Object, IOnMapReadyCallback
{
    private readonly IMapHandler _mapHandler;

    public MapCallbackHandler(IMapHandler mapHandler)
    {
        this._mapHandler = mapHandler;
    }

    public void OnMapReady(GoogleMap googleMap)
    {
        _mapHandler.UpdateValue(nameof(IMap.Pins));
    }
}


#endif

