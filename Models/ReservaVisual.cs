using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRadventure.Models
{
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
        public string Provincia { get; set; } = string.Empty;
        public string DuracionTour { get; set; } = string.Empty;
        public string GuiaAsociado { get; set; } = string.Empty;
        public string PuntoEncuentro { get; set; } = string.Empty;

        public bool EsActiva => Estado == "activa";
        public string ColorEstado => EsActiva ? "#93C94A" : "#757575";
        public string TextoEstado => EsActiva ? "RESERVA ACTIVA" : "CANCELADA";
    }
}
