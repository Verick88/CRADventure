using System;
using System.Threading.Tasks;
using CRadventure.Models;
using Plugin.Firebase.Firestore;

namespace CRadventure.Services
{
    public class UsuarioService
    {
        private const string ColeccionUsuarios = "usuarios";

        public async Task GuardarUsuarioAsync(UsuarioModel usuario)
        {
            try
            {
                await CrossFirebaseFirestore.Current
                    .GetCollection(ColeccionUsuarios)
                    .GetDocument(usuario.Uid)
                    .SetDataAsync(usuario);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al guardar usuario: {ex.Message}");
            }
        }

        public async Task<UsuarioModel?> ObtenerUsuarioPorUidAsync(string uid)
        {
            try
            {
                var docSnapshot = await CrossFirebaseFirestore.Current
                    .GetCollection(ColeccionUsuarios)
                    .GetDocument(uid)
                    .GetDocumentSnapshotAsync<UsuarioModel>();

                var usuario = docSnapshot?.Data;

                if (usuario == null)
                {
                    return null;
                }

                usuario.Uid = uid; // por si el campo Uid no viene guardado en el documento
                return usuario;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al obtener usuario: {ex.Message}");
                return null;
            }
        }

        public async Task ActualizarPerfilAsync(string uid, string nombre, string apellidos, string telefono)
        {
            var usuario = await ObtenerUsuarioPorUidAsync(uid);
            if (usuario == null) return;

            usuario.Nombre = nombre;
            usuario.Apellidos = apellidos;
            usuario.Telefono = telefono;

            await GuardarUsuarioAsync(usuario);
        }
    }
}