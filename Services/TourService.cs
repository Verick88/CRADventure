using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Plugin.Firebase.Firestore;
using CRadventure.Models;

namespace CRadventure.Services
{
    class TourService
    {
        private const string ColeccionTours = "tours";

        // Método para obtener todos los tours
        public async Task<List<TourModel>> ObtenerTodosLosToursAsync()
        {
            var documentos = await CrossFirebaseFirestore.Current
                .GetCollection(ColeccionTours)
                .GetDocumentsAsync<TourModel>();

            return documentos.Documents.Select(d => d.Data).ToList();
        }

        // Método para obtener un tour específico para la pagina de tour detalle
        public async Task<TourModel?> ObtenerTourPorIdAsync(string id)
        {
            var documento = await CrossFirebaseFirestore.Current
                .GetCollection(ColeccionTours)
                .GetDocument(id)
                .GetDocumentSnapshotAsync<TourModel>();

            return documento?.Data;
        }
    }
}
