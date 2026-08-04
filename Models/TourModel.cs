using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Plugin.Firebase.Firestore;

namespace CRadventure.Models
{
    class TourModel
    {
        [FirestoreDocumentId]
        public string Id { get; set; } = string.Empty;

        [FirestoreProperty("titulo")]
        public string NombreLugar { get; set; } = string.Empty;

        [FirestoreProperty("descripcionCorta")]
        public string DescripcionCorta { get; set; } = string.Empty;

        [FirestoreProperty("imagen_url")]
        public string ImagenUrl { get; set; } = string.Empty;

        [FirestoreProperty("precioNacional")]
        public double PrecioNacional { get; set; }

        [FirestoreProperty("precioExtranjero")]
        public double PrecioExtranjero { get; set; }

        [FirestoreProperty("duracionHoras")]
        public int DuracionHoras { get; set; }

        

        [FirestoreProperty("provincia")]
        public string Provincia { get; set; } = string.Empty;

        [FirestoreProperty("dificultad")]
        public string Dificultad { get; set; } = string.Empty;

        [FirestoreProperty("idiomas")]
        public string Idiomas { get; set; } = string.Empty;

        [FirestoreProperty("fechaTour")]
        public string FechaHoraVisual { get; set; } = "Fecha por definir";
        public string PrecioVisual { get; set; } = string.Empty;

        public void AplicarTarifa(bool esExtranjero)
        {
            double precioFinal = esExtranjero ? PrecioExtranjero : PrecioNacional;
            PrecioVisual = esExtranjero
                ? $"${precioFinal:F2} USD"
                : $"₡{precioFinal:N0} CRC";
        }

        public string IdiomasTexto { get; set; } = string.Empty;
        public double Calificacion { get; set; } = 4.5; // Valor estático PRUEBA

        public string DuracionVisual => $"{DuracionHoras} hrs";
        private DateTime _fechaTour;

        
        
    }
}
