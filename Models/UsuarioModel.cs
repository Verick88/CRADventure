using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.Firebase.Firestore;

namespace CRadventure.Models
{
    public class UsuarioModel : IFirestoreObject
    {
        [FirestoreDocumentId]
        public string Uid { get; set; } = string.Empty;

        [FirestoreProperty("email")]
        public string Email { get; set; } = string.Empty;

        [FirestoreProperty("rol")]
        public string Rol { get; set; } = string.Empty;

        [FirestoreProperty("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [FirestoreProperty("apellidos")]
        public string Apellidos { get; set; } = string.Empty;

        [FirestoreProperty("telefono")]
        public string Telefono { get; set; } = string.Empty;

        [FirestoreProperty("activo")]
        public bool Activo { get; set; }

        [FirestoreProperty("es_extranjero")]
        public bool EsExtranjero { get; set; }

        [FirestoreProperty("fecha_registro")]
        public DateTimeOffset? FechaRegistro { get; set; }

        // Campos exclusivos del guia
        [FirestoreProperty("biografia")]
        public string Biografia { get; set; } = string.Empty;

        [FirestoreProperty("credencial_turismo")]
        public string CredencialTurismo { get; set; } = string.Empty;

        [FirestoreProperty("verificado")]
        public bool Verificado { get; set; }

        // Campo para la foto de perfil
        [FirestoreProperty("foto_url")]
        public string FotoUrl { get; set; } = string.Empty;
    }
}