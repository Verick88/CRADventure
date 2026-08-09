using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.Firebase.Firestore;
namespace CRadventure.Models

{

    public class TourModel

    {
        [FirestoreDocumentId]
        public string Id { get; set; } = string.Empty;

        [FirestoreProperty("titulo")]
        public string NombreLugar { get; set; } = string.Empty;

        [FirestoreProperty("descripcionCorta")]
        public string DescripcionCorta { get; set; } = string.Empty;

        [FirestoreProperty("descripcionLarga")]
        public string DescripcionLarga { get; set; } = string.Empty;

        [FirestoreProperty("plazasDisponibles")]
        public int PlazasDisponibles { get; set; }

        [FirestoreProperty("guiaAsociado")]
        public string GuiaAsociado { get; set; } = string.Empty;

        [FirestoreProperty("guiasAdicionales")]
        public string GuiasAdicionales { get; set; } = string.Empty;

        [FirestoreProperty("puntoEncuentro")]
        public string PuntoEncuentro { get; set; } = string.Empty;

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
            //condicion    Si es verdadero  : Si es falso
            double precioFinal = esExtranjero ? PrecioExtranjero : PrecioNacional;
            PrecioVisual = esExtranjero
            //            F2 fuerza a mostrar 2 decimales
            ? $"${precioFinal:F2} USD"
            //             N0 formato con separadores de miles y cero decimales
            : $"₡{precioFinal:N0} CRC";
        }

        public string IdiomasTexto { get; set; } = string.Empty;

        public string DuracionVisual => $"{DuracionHoras} hrs";

    }

}

