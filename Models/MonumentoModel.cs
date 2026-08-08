using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.Firebase.Firestore;

namespace CRadventure.Models;

public class MonumentoModel
{
    [FirestoreDocumentId]
    public string Id { get; set; } = string.Empty;

    [FirestoreProperty("nombre")]
    public string Nombre { get; set; } = string.Empty;

    [FirestoreProperty("historia")]
    public string Historia { get; set; } = string.Empty;

    [FirestoreProperty("imagen_url")]
    public string ImagenUrl { get; set; } = string.Empty;

    [FirestoreProperty("zona")]
    public string Zona { get; set; } = string.Empty;

    [FirestoreProperty("activo")]
    public bool Activo { get; set; }

    [FirestoreProperty("ubicacion")]
    public GeoPoint Ubicacion { get; set; } = new GeoPoint(0, 0);

    public double Latitud => Ubicacion.Latitude;
    public double Longitud => Ubicacion.Longitude;
}