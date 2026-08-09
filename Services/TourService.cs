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
        //Constante para almacenar el nombre de la coleccion en la nube (tours)
        private const string ColeccionTours = "tours";

        //Servicio para actualizar los tours
        public async Task ActualizarTourAsync(TourModel tour)
        {
            await CrossFirebaseFirestore.Current //Accede a la instancia actual del servicio de Firestore
                .GetCollection(ColeccionTours) //Entra a la coleccion de tours
                .GetDocument(tour.Id) //Busca dentro de la coleccion el tour id (.Id exrtrae el id de toda la coleccion)
                .SetDataAsync(tour); //Toma el objeto tour con los nuevos cambios y los actualiza o sobreescribe
        }

        //Servicio para eliminar los tours
        public async Task EliminarTourAsync(string tourId)
        {
            await CrossFirebaseFirestore.Current
                .GetCollection(ColeccionTours)
                .GetDocument(tourId)
                .DeleteDocumentAsync(); //Elimina el tour completo del Firestore
        }

        //Servicio para agregar los tours
        public async Task AgregarTourAsync(TourModel tour)
        {
            await CrossFirebaseFirestore.Current
                .GetCollection(ColeccionTours)
                .AddDocumentAsync(tour); //Toma el objeto tour y lo envia a firestore. Crea un documento nuevo y le asigna su propio ID
        }

        // Método para obtener todos los tours
        public async Task<List<TourModel>> ObtenerTodosLosToursAsync() //Devuelve una lista de objetos de tipo TorModel
        {
            var documentos = await CrossFirebaseFirestore.Current
                .GetCollection(ColeccionTours) //ColeccionTours equivale a tours
                .GetDocumentsAsync<TourModel>(); //Pide a Firebase que descargue todos los documentos y que los intente convertir en un objeto tipo TourModel

            return documentos.Documents.Select(d => d.Data).ToList(); //Recorre cada documento para extraer solo su informacion util. ToList lo transforma en una lista limpia y lo devuelve
        }

        // Método para obtener un tour específico para la pagina de tour detalle
        public async Task<TourModel?> ObtenerTourPorIdAsync(string id) //Devuelve un objeto nuleable ToutModel?. Recibe como parametro el id del tour que se quiere consultar
        {
            var documento = await CrossFirebaseFirestore.Current
                .GetCollection(ColeccionTours) //Entra a la coleccion tours
                .GetDocument(id) //Apunta a un documento en especifico utilizando el id
                .GetDocumentSnapshotAsync<TourModel>(); //Descarga el estado actual del documento en la nube y lo convierte en un objeto tipo TourModel

            return documento?.Data; //Puede devolver documentos nulos
        }
    }
}
