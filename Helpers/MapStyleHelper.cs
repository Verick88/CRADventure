#if ANDROID
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps.Handlers;

namespace CRadventure;

public static class MapStyleHelper
{
    public static void ApplyStyle(MapHandler mapHandler)
    {
        if (mapHandler.PlatformView is Android.Gms.Maps.MapView mapView)
        {
            mapView.GetMapAsync(new MapStyleCallback());
        }
    }
}

public class MapStyleCallback : Java.Lang.Object, Android.Gms.Maps.IOnMapReadyCallback
{
    public void OnMapReady(Android.Gms.Maps.GoogleMap googleMap)
    {
        try
        {
            string styleJson = @"[
              { ""elementType"": ""geometry"", ""stylers"": [{ ""color"": ""#f5f3ef"" }] },
              { ""elementType"": ""labels.text.fill"", ""stylers"": [{ ""color"": ""#5e4b3c"" }] },
              { ""elementType"": ""labels.text.stroke"", ""stylers"": [{ ""color"": ""#f1eae1"" }] },
              { ""featureType"": ""administrative"", ""elementType"": ""geometry.stroke"", ""stylers"": [{ ""color"": ""#c9b9a6"" }] },
              { ""featureType"": ""landscape"", ""elementType"": ""geometry"", ""stylers"": [{ ""color"": ""#f9f6f0"" }] },
              { ""featureType"": ""landscape.natural"", ""elementType"": ""geometry"", ""stylers"": [{ ""color"": ""#dfd5c3"" }] },
              { ""featureType"": ""poi"", ""elementType"": ""geometry"", ""stylers"": [{ ""color"": ""#e8dfd1"" }] },
              { ""featureType"": ""poi.park"", ""elementType"": ""geometry"", ""stylers"": [{ ""color"": ""#d2d8c7"" }] },
              { ""featureType"": ""road"", ""elementType"": ""geometry"", ""stylers"": [{ ""color"": ""#faf8f5"" }] },
              { ""featureType"": ""road.highway"", ""elementType"": ""geometry"", ""stylers"": [{ ""color"": ""#e3d3bc"" }] },
              { ""featureType"": ""road.highway"", ""elementType"": ""geometry.stroke"", ""stylers"": [{ ""color"": ""#c5b299"" }] },
              { ""featureType"": ""water"", ""elementType"": ""geometry"", ""stylers"": [{ ""color"": ""#c8d7df"" }] }
            ]";

            var style = new Android.Gms.Maps.Model.MapStyleOptions(styleJson);
            googleMap.SetMapStyle(style);
        }
        catch (System.Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error al aplicar estilo al mapa: {ex.Message}");
        }
    }
}
#endif