using CRadventure.Models;
using CRadventure.Services;
using Plugin.Firebase.Firestore;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CRadventure.Views;

public partial class MisReservasPage : ContentPage
{
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

            var snapshotReservas = await CrossFirebaseFirestore.Current
                .GetCollection("reservas")
                .GetDocumentsAsync<ReservaModel>();

            string miUid = usuarioAuth.Uid.Trim();

            foreach (var doc in snapshotReservas.Documents)
            {
                var reservaData = doc.Data;
                if (reservaData == null) continue;

                if (reservaData.UsuarioId?.Trim() == miUid)
                {
                    string tourId = reservaData.TourId;

                    var docTour = await CrossFirebaseFirestore.Current
                        .GetCollection("tours")
                        .GetDocument(tourId)
                        .GetDocumentSnapshotAsync<TourModel>();

                    var tourData = docTour?.Data;
                    if (tourData != null)
                    {
                        var visual = new ReservaVisual
                        {
                            ReservaId = doc.Reference.Id,
                            TourId = tourId,
                            CantidadEntradas = reservaData.CantidadEntradas,
                            PrecioPagado = string.IsNullOrEmpty(reservaData.PrecioPagado) ? "$0" : reservaData.PrecioPagado,
                            Estado = string.IsNullOrEmpty(reservaData.Estado) ? "activa" : reservaData.Estado,

                            NombreLugar = string.IsNullOrEmpty(tourData.NombreLugar) ? "Tour" : tourData.NombreLugar,
                            ImagenUrl = tourData.ImagenUrl,
                            FechaTour = string.IsNullOrEmpty(tourData.FechaHoraVisual) ? "Por definir" : tourData.FechaHoraVisual
                        };

                        MisReservas.Add(visual);
                    }
                }
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
            await CrossFirebaseFirestore.Current
                .GetCollection("reservas")
                .GetDocument(reserva.ReservaId)
                .UpdateDataAsync(new Dictionary<object, object> { { "estado", "cancelada" } });

            var docTour = await CrossFirebaseFirestore.Current
                .GetCollection("tours")
                .GetDocument(reserva.TourId)
                .GetDocumentSnapshotAsync<TourModel>();

            if (docTour?.Data != null)
            {
                int cuposActuales = docTour.Data.PlazasDisponibles;

                await CrossFirebaseFirestore.Current
                    .GetCollection("tours")
                    .GetDocument(reserva.TourId)
                    .UpdateDataAsync(new Dictionary<object, object> { { "plazasDisponibles", cuposActuales + reserva.CantidadEntradas } });
            }

            await DisplayAlert("Cancelada", "Reserva cancelada y cupos devueltos.", "OK");
            await CargarMisReservas();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al cancelar: {ex.Message}", "OK");
        }
    }
}

public class ReservaVisual
{
    public string ReservaId { get; set; } = string.Empty;
    public string TourId { get; set; } = string.Empty;
    public int CantidadEntradas { get; set; }
    public string PrecioPagado { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string NombreLugar { get; set; } = string.Empty;
    public string ImagenUrl { get; set; } = string.Empty;
    public string FechaTour { get; set; } = string.Empty;

    public bool EsActiva => Estado == "activa";
    public string ColorEstado => EsActiva ? "#93C94A" : "#757575";
    public string TextoEstado => EsActiva ? "RESERVA ACTIVA" : "CANCELADA";
}