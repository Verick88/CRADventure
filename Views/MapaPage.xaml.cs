using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

namespace CRadventure.Views;

public partial class MapaPage : ContentPage
{
    public MapaPage()
    {
        InitializeComponent();

        ConfigurarMapa();
        AgregarMonumentosPrueba();
    }

    private void ConfigurarMapa()
    {
        var costaRica = new Location(9.9281, -84.0907);

        mapaCostaRica.MoveToRegion(
            MapSpan.FromCenterAndRadius(
                costaRica,
                Distance.FromKilometers(120)));
    }

    private void AgregarMonumentosPrueba()
    {
        mapaCostaRica.Pins.Add(new Pin
        {
            Label = "Fortin Nacional",
            Address = "construida en 1876 en el centro de la ciudad de Heredia, " +
            "famosa por haber sido diseñada por Fadrique Gutiérrez López y por tener troneras al revés",
            Type = PinType.Place,
            Location = new Location(9.999281722878715, -84.11707839947388)
        });

        mapaCostaRica.Pins.Add(new Pin
        {
            Label = "Basílica de Cartago",
            Address = "Cartago",
            Type = PinType.Place,
            Location = new Location(9.8644, -83.9194)
        });

        mapaCostaRica.Pins.Add(new Pin
        {
            Label = "Parque Nacional Manuel Antonio",
            Address = "Quepos",
            Type = PinType.Place,
            Location = new Location(9.4120, -84.1550)
        });
    }
}