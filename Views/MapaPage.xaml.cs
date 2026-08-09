using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using CRadventure.Models;
using CRadventure.Services;

namespace CRadventure.Views;

public partial class MapaPage : ContentPage
{
    private readonly MonumentoService _service = new();

    public MapaPage()
    {
        InitializeComponent();

        ConfigurarMapa();
        CargarMonumentos();
    }

    private void ConfigurarMapa()
    {
        var costaRica = new Location(9.9281, -84.0907);

        mapaCostaRica.MoveToRegion(
            MapSpan.FromCenterAndRadius(
                costaRica,
                Distance.FromKilometers(120)));
    }

    private async void CargarMonumentos()
    {
        try
        {
            var monumentos = await _service.ObtenerMonumentosAsync();

            mapaCostaRica.Pins.Clear();

            foreach (var m in monumentos)
            {
                var pin = new CustomPin
                {
                    Label = m.Nombre,
                    Address = m.Zona,
                    Type = PinType.Place,
                    Location = new Location(m.Latitud, m.Longitud),
                    ImageSource = "pin.png",
                    OnClicked = () => MostrarTarjetaMonumento(m) 
                };

                pin.MarkerClicked += (s, e) =>
                {
                    e.HideInfoWindow = true;
                    MostrarTarjetaMonumento(m);
                };

                mapaCostaRica.Pins.Add(pin);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Error",
                $"No se pudieron cargar los monumentos:\n{ex.Message}",
                "OK");
        }
    }

    private void MostrarTarjetaMonumento(MonumentoModel m)
    {
        lblNombre.Text = m.Nombre;
        lblHistoria.Text = m.Historia;

        // Se asigna la imagen desde la URL de firebase
        if (!string.IsNullOrEmpty(m.ImagenUrl))
        {
            imgMonumento.Source = ImageSource.FromUri(new Uri(m.ImagenUrl));
        }
        else
        {
            imgMonumento.Source = null;
        }

        panelMonumento.IsVisible = true;
    }

    private void CerrarPanel_Clicked(object sender, EventArgs e)
    {
        panelMonumento.IsVisible = false;
    }
}
