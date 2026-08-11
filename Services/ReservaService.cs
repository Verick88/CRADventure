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
            var listaVisuales = new List<ReservaVisual>(); //Crea lista vacia para ir almacenando

            //Descarga de las reservas
            var snapshotReservas = await CrossFirebaseFirestore.Current
                .GetCollection("reservas")
                .GetDocumentsAsync<ReservaModel>();

            //Ciclo por cada una de las reservas encontradas
            foreach (var doc in snapshotReservas.Documents)
            {
                var reservaData = doc.Data; //Extrae datos de la reserva actual
                if (reservaData == null) continue; //Si es null, ignora y continua

                if (reservaData.UsuarioId?.Trim() == userId.Trim()) //Compara el ID del usuario con la sesion iniciada con el ID del user que realizo la reserva
                {
                    string tourId = reservaData.TourId;

                    var docTour = await CrossFirebaseFirestore.Current
                        .GetCollection("tours")
                        .GetDocument(tourId)
                        .GetDocumentSnapshotAsync<TourModel>();

                    var tourData = docTour?.Data;
                    if (tourData != null) //Valida que tour realmente exista en la base de datos
                    {
                        //Creacion del objeto visual
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

                            Provincia = tourData.Provincia,
                            DuracionTour = tourData.DuracionVisual,
                            GuiaAsociado = tourData.GuiaAsociado,
                            PuntoEncuentro = string.IsNullOrEmpty(tourData.PuntoEncuentro) ? "Por definir" : tourData.PuntoEncuentro
                        };

                        listaVisuales.Add(visual);
                    }
                }
            }

            return listaVisuales;
        }

        // Metodo para cancelar reserva y devolver tiquetes
        public async Task CancelarReservaAsync(ReservaVisual reserva)
        {
            //Actualiza el estado de la reserva a cancelada
            await CrossFirebaseFirestore.Current
                .GetCollection("reservas")
                .GetDocument(reserva.ReservaId)
                .UpdateDataAsync(new Dictionary<object, object> { { "estado", "cancelada" } });

            //Obtiene el tour para devolverle los cupos disponibles
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

        // Metodo crea nueva reserva y descuenta los tiquetes
        public async Task CrearReservaAsync(TourModel tour, string userId, int cantidadEntradas, string precioTotalVisual)
        {
            //Descuenta las plazas disponibles localmente en el objeto
            tour.PlazasDisponibles -= cantidadEntradas;

            //Actualiza los cupos del tour en Firestore
            await CrossFirebaseFirestore.Current
                .GetCollection("tours")
                .GetDocument(tour.Id)
                .UpdateDataAsync(new Dictionary<object, object>
                {
                    { "plazasDisponibles", tour.PlazasDisponibles }
                });

            //Construye el objeto ReservaModel
            var nuevaReserva = new ReservaModel
            {
                TourId = tour.Id,
                UsuarioId = userId,
                CantidadEntradas = cantidadEntradas,
                FechaCompra = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                PrecioPagado = precioTotalVisual,
                Estado = "activa"
            };

            //Guarda la reserva en Firestore
            await CrossFirebaseFirestore.Current
                .GetCollection("reservas")
                .AddDocumentAsync(nuevaReserva);
        }
    }
}
