using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Maps.Handlers;

namespace CRadventure.Platforms.Android;

public static class MapStyleHelper
{
    public static void ApplyStyle(MapHandler handler)
    {
        if (handler.PlatformView is not MapView mapView)
            return;

        mapView.GetMapAsync(new MapReadyCallback(googleMap =>
        {
            googleMap.SetMapStyle(
                MapStyleOptions.LoadRawResourceStyle(
                    handler.Context,
                    Resource.Raw.map_style));

            // Oculta edificios
            googleMap.BuildingsEnabled = false;
        }));
    }

    private class MapReadyCallback : Java.Lang.Object, IOnMapReadyCallback
    {
        private readonly Action<GoogleMap> _onReady;

        public MapReadyCallback(Action<GoogleMap> onReady)
        {
            _onReady = onReady;
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            _onReady?.Invoke(googleMap);
        }
    }
}