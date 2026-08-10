using CRadventure.Models;
using CRadventure.Services;

namespace CRadventure.Views;

[QueryProperty(nameof(TourSeleccionado), "TourAMostrar")]
public partial class ReservaPage : ContentPage
{
    private readonly ReservaService _reservaService = new();
    private TourModel _tourSeleccionado;
    private readonly UsuarioModel _usuarioActual;
    private int _cantidadEntradas = 1;
    private double _precioUnitario = 0;

    public TourModel TourSeleccionado
    {
        get => _tourSeleccionado;
        set
        {
            _tourSeleccionado = value;
            OnPropertyChanged();
            BindingContext = _tourSeleccionado;

            if (_tourSeleccionado != null && _usuarioActual != null)
            {
                _precioUnitario = _usuarioActual.EsExtranjero ? _tourSeleccionado.PrecioExtranjero : _tourSeleccionado.PrecioNacional;
                ActualizarTotales();
            }
        }
    }

    public ReservaPage()
    {
        InitializeComponent();
        _usuarioActual = SesionService.UsuarioActual;
    }

    private void OnMenosClicked(object sender, EventArgs e)
    {
        if (_cantidadEntradas > 1)
        {
            _cantidadEntradas--;
            ActualizarTotales();
        }
    }

    private async void OnMasClicked(object sender, EventArgs e)
    {
        if (_tourSeleccionado != null && _cantidadEntradas < _tourSeleccionado.PlazasDisponibles)
        {
            _cantidadEntradas++;
            ActualizarTotales();
        }
        else
        {
            await DisplayAlert("Límite alcanzado", "No puedes reservar más entradas que los cupos disponibles.", "OK");
        }
    }

    private void ActualizarTotales()
    {
        if (_tourSeleccionado == null || _usuarioActual == null) return;

        lblCantidad.Text = _cantidadEntradas.ToString();
        double totalPagar = _precioUnitario * _cantidadEntradas;

        if (_usuarioActual.EsExtranjero)
        {
            lblTotalPagar.Text = $"${totalPagar:F2} USD";
        }
        else
        {
            lblTotalPagar.Text = $"₡{totalPagar:N0} CRC";
        }
    }

    private async void OnReservarClicked(object sender, EventArgs e)
    {
        var usuarioAuth = Plugin.Firebase.Auth.CrossFirebaseAuth.Current.CurrentUser;

        if (_tourSeleccionado == null || usuarioAuth == null) return;

        if (_tourSeleccionado.PlazasDisponibles < _cantidadEntradas)
        {
            await DisplayAlert("Agotado", "No hay suficientes tiquetes disponibles para esta reserva.", "OK");
            return;
        }

        bool confirmacion = await DisplayAlert("Confirmar Reserva",
            $"¿Deseas reservar {_cantidadEntradas} espacio(s) para {_tourSeleccionado.NombreLugar} por un total de {lblTotalPagar.Text}?",
            "Sí, Reservar", "Cancelar");

        if (!confirmacion) return;

        try
        {
            // Toda la lógica de Firestore ahora la maneja el servicio limpiamente
            await _reservaService.CrearReservaAsync(_tourSeleccionado, usuarioAuth.Uid, _cantidadEntradas, lblTotalPagar.Text);

            await DisplayAlert("¡Éxito!", "Tu reserva se ha guardado correctamente.", "Genial");
            OnPropertyChanged(nameof(TourSeleccionado));
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Hubo un problema al conectar con el servidor: {ex.Message}", "OK");
        }
    }
}