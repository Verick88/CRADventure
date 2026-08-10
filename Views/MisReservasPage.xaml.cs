using CRadventure.Models;
using CRadventure.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CRadventure.Views;

public partial class MisReservasPage : ContentPage
{
    private readonly ReservaService _reservaService = new();
    public ObservableCollection<ReservaVisual> MisReservas { get; set; } = new();
    public ICommand CancelarReservaCommand { get; }

    public MisReservasPage()
    {
        InitializeComponent();
        CancelarReservaCommand = new Command<ReservaVisual>(async (reserva) => await CancelarReserva(reserva));
        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CargarMisReservas();
    }

    private async Task CargarMisReservas()
    {
        var usuarioAuth = Plugin.Firebase.Auth.CrossFirebaseAuth.Current.CurrentUser;
        if (usuarioAuth == null) return;

        try
        {
            MisReservas.Clear();
            var reservasObtenidas = await _reservaService.ObtenerReservasUsuarioAsync(usuarioAuth.Uid);

            foreach (var reserva in reservasObtenidas)
            {
                MisReservas.Add(reserva);
            }

            if (MisReservas.Count == 0)
            {
                layoutVacio.IsVisible = true;
                listaReservas.IsVisible = false;
            }
            else
            {
                layoutVacio.IsVisible = false;
                listaReservas.IsVisible = true;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al cargar reservas: {ex.Message}", "OK");
        }
    }

    private async Task CancelarReserva(ReservaVisual reserva)
    {
        bool confirmar = await DisplayAlert("Cancelar Reserva",
            $"¿Seguro que quieres cancelar {reserva.CantidadEntradas} tiquetes para {reserva.NombreLugar}?",
            "Sí", "No");

        if (!confirmar) return;

        try
        {
            await _reservaService.CancelarReservaAsync(reserva);

            await DisplayAlert("Cancelada", "Reserva cancelada y cupos devueltos.", "OK");
            await CargarMisReservas();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al cancelar: {ex.Message}", "OK");
        }
    }
}
