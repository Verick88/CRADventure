using CRadventure.Models;
using Plugin.Firebase.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRadventure.Services
{
    public class ReservaService
    {
        // Método para obtener y combinar las reservas del usuario con los datos de sus tours
        public async Task<List<ReservaVisual>> ObtenerReservasUsuarioAsync(string userId)
        {
            var listaVisuales = new List<ReservaVisual>();

            var snapshotReservas = await CrossFirebaseFirestore.Current
                .GetCollection("reservas")
                .GetDocumentsAsync<ReservaModel>();

            foreach (var doc in snapshotReservas.Documents)
            {
                var reservaData = doc.Data;
                if (reservaData == null) continue;

                if (reservaData.UsuarioId?.Trim() == userId.Trim())
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
                            FechaTour = string.IsNullOrEmpty(tourData.FechaHoraVisual) ? "Por definir" : tourData.FechaHoraVisual,

                            // Nuevas propiedades mapeadas desde el TourModel
                            Provincia = tourData.Provincia,
                            DuracionTour = tourData.DuracionVisual,
                            GuiaAsociado = tourData.GuiaAsociado
                        };

                        listaVisuales.Add(visual);
                    }
                }
            }

            return listaVisuales;
        }

        // Método para cancelar la reserva y devolver los cupos al tour correspondiente
        public async Task CancelarReservaAsync(ReservaVisual reserva)
        {
            // 1. Actualizar el estado de la reserva a cancelada
            await CrossFirebaseFirestore.Current
                .GetCollection("reservas")
                .GetDocument(reserva.ReservaId)
                .UpdateDataAsync(new Dictionary<object, object> { { "estado", "cancelada" } });

            // 2. Obtener el tour para devolverle los cupos disponibles
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
        }

        // Método para crear una nueva reserva y descontar los cupos del tour
        public async Task CrearReservaAsync(TourModel tour, string userId, int cantidadEntradas, string precioTotalVisual)
        {
            // 1. Descontar las plazas disponibles localmente en el objeto
            tour.PlazasDisponibles -= cantidadEntradas;

            // 2. Actualizar los cupos del tour en Firestore
            await CrossFirebaseFirestore.Current
                .GetCollection("tours")
                .GetDocument(tour.Id)
                .UpdateDataAsync(new Dictionary<object, object>
                {
                    { "plazasDisponibles", tour.PlazasDisponibles }
                });

            // 3. Construir el objeto ReservaModel
            var nuevaReserva = new ReservaModel
            {
                TourId = tour.Id,
                UsuarioId = userId,
                CantidadEntradas = cantidadEntradas,
                FechaCompra = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                PrecioPagado = precioTotalVisual,
                Estado = "activa"
            };

            // 4. Guardar la reserva en Firestore
            await CrossFirebaseFirestore.Current
                .GetCollection("reservas")
                .AddDocumentAsync(nuevaReserva);
        }
    }
}
