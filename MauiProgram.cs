using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using Microsoft.Maui.Maps.Handlers;
using Map = Microsoft.Maui.Controls.Maps.Map;

namespace CRadventure;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiMaps()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("KaushanScript-Regular.ttf", "Kaushan");
                fonts.AddFont("Montserrat-Regular.ttf", "MontserratRegular");
            });

#if ANDROID
        MapHandler.Mapper.AppendToMapping("CustomMapStyle", (handler, view) =>
        {
            if (handler is MapHandler mapHandler)
            {
                MapStyleHelper.ApplyStyle(mapHandler);
            }
        });

        MapHandler.Mapper.Add("Pins", (handler, view) => { });

        MapHandler.Mapper.AppendToMapping("CustomPinHandler", (handler, view) =>
        {
            handler.PlatformView.GetMapAsync(new MapReadyCallbackCustom(view as Map));
        });
#endif

        return builder.Build();
    }
}

#if ANDROID
public class MapReadyCallbackCustom : Java.Lang.Object, Android.Gms.Maps.IOnMapReadyCallback
{
    private readonly Map _mauiMap;
    private Android.Gms.Maps.GoogleMap _googleMap;

    public MapReadyCallbackCustom(Map mauiMap)
    {
        _mauiMap = mauiMap;
    }

    public void OnMapReady(Android.Gms.Maps.GoogleMap googleMap)
    {
        _googleMap = googleMap;
        if (_mauiMap == null) return;

        //Limitar a costa rica
        var costaRicaBounds = new Android.Gms.Maps.Model.LatLngBounds(
            new Android.Gms.Maps.Model.LatLng(7.8, -86.3),
            new Android.Gms.Maps.Model.LatLng(11.5, -82.5)
        );

        _googleMap.SetLatLngBoundsForCameraTarget(costaRicaBounds);
        _googleMap.SetMinZoomPreference(7.5f);
        _googleMap.UiSettings.RotateGesturesEnabled = false;

        // Dibujamos los pines iniciales
        DibujarPines();

        // Escuchamos si se agregan más pines dinámicamente en el código
        if (_mauiMap.Pins is System.Collections.Specialized.INotifyCollectionChanged collectionChanged)
        {
            collectionChanged.CollectionChanged += (s, e) =>
            {
                DibujarPines();
            };
        }

        _googleMap.MarkerClick += (sender, e) =>
        {
            e.Handled = true;
            var clickedPin = _mauiMap.Pins.FirstOrDefault(p => p.Label == e.Marker.Tag?.ToString());
            if (clickedPin is CRadventure.Models.CustomPin customPin)
            {
                customPin.OnClicked?.Invoke();
            }
        };
    }

    private void DibujarPines()
    {
        if (_googleMap == null) return;

        _googleMap.Clear();

        foreach (var pin in _mauiMap.Pins)
        {
            var markerOptions = new Android.Gms.Maps.Model.MarkerOptions();
            markerOptions.SetPosition(new Android.Gms.Maps.Model.LatLng(pin.Location.Latitude, pin.Location.Longitude));
            markerOptions.SetTitle(pin.Label);
            markerOptions.SetSnippet(pin.Address);

            if (pin is CRadventure.Models.CustomPin)
            {
                int resourceId = Android.App.Application.Context.Resources.GetIdentifier(
                    "pin",
                    "drawable",
                    Android.App.Application.Context.PackageName);

                if (resourceId != 0)
                {
                    var originalBitmap = Android.Graphics.BitmapFactory.DecodeResource(Android.App.Application.Context.Resources, resourceId);
                    if (originalBitmap != null)
                    {
                        var resizedBitmap = Android.Graphics.Bitmap.CreateScaledBitmap(originalBitmap, 96, 96, false);
                        var icon = Android.Gms.Maps.Model.BitmapDescriptorFactory.FromBitmap(resizedBitmap);
                        markerOptions.InvokeIcon(icon);
                    }
                }
            }

            var marker = _googleMap.AddMarker(markerOptions);
            marker.Tag = new Java.Lang.String(pin.Label);
        }
    }
}
#endif