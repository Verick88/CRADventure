using Plugin.Firebase.Firestore;

namespace CRadventure.Models;

public class ReservaModel
{
    [FirestoreProperty("tour_id")]
    public string TourId { get; set; } = string.Empty;

    [FirestoreProperty("usuario_id")]
    public string UsuarioId { get; set; } = string.Empty;

    [FirestoreProperty("cantidad_entradas")]
    public int CantidadEntradas { get; set; }

    [FirestoreProperty("fecha_compra")]
    public string FechaCompra { get; set; } = string.Empty;

    [FirestoreProperty("precio_pagado")]
    public string PrecioPagado { get; set; } = string.Empty;

    [FirestoreProperty("estado")]
    public string Estado { get; set; } = "activa";
}