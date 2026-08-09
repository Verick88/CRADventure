using System;
using System.Threading.Tasks;
using System.Linq;
using CRadventure.Models;
using Plugin.Firebase.Firestore;
using Plugin.Firebase.Auth;

namespace CRadventure.Services
{
    public class UsuarioService
    {
        //Constante para acceder a la coleccion en la nube de usuarios
        private const string ColeccionUsuarios = "usuarios";

        //Servicio para guardar un usuario
        public async Task GuardarUsuarioAsync(UsuarioModel usuario)
        {
            try
            {
                await CrossFirebaseFirestore.Current
                    .GetCollection(ColeccionUsuarios) //Accede a la coleccion de usuarios
                    .GetDocument(usuario.Uid) //Asegura que el documento del usuario tenga el mismo Uid tanto en Firestore como en autenticacion
                    .SetDataAsync(usuario); //Guarda al usuario, si el usuario existe actualiza sus datos, si no existe, lo crea
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al guardar usuario: {ex.Message}"); //ex Message imprime el error exacto
            }
        }

        //Servicio para obtener a un usuario por su uid
        public async Task<UsuarioModel?> ObtenerUsuarioPorUidAsync(string uid)
        {
            try
            {
                var docSnapshot = await CrossFirebaseFirestore.Current
                    .GetCollection(ColeccionUsuarios)
                    .GetDocument(uid) //Utiliza el uid que recibe como parametro para buscar exactamente el documento con ese id
                    .GetDocumentSnapshotAsync<UsuarioModel>(); //Descarga la snapshot del documento y la convierte en el objeto UsuarioModel

                var usuario = docSnapshot?.Data; //Saca los datos del documento descargado

                //If de seguridad, si el usuario no se encuentra, devuelve null
                if (usuario == null)
                {
                    return null;
                }

                usuario.Uid = uid; // Asigna manualmente el Uid al objeto antes de retornarlo para garantizar que el modelo llegue completo y funcional al resto de la app
                return usuario;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al obtener usuario: {ex.Message}");
                return null;
            }
        }


        // Servicio para obtener usuario por correo
        public async Task<UsuarioModel?> ObtenerUsuarioPorEmailAsync(string email)
        {
            try
            {
                var querySnapshot = await CrossFirebaseFirestore.Current
                    .GetCollection(ColeccionUsuarios)
                    .WhereEqualsTo("email", email.Trim()) //Le pide a firebase que filtre y busque solo el email que sea igual al parametro que se paso
                    .GetDocumentsAsync<UsuarioModel>(); //Descarga los documentos que cumplieron con el parametro dado

                if (querySnapshot != null && querySnapshot.Documents.Any())// verifica si firebase encontro al menos un documento que coincida, si esta vacio retorna null
                {
                    var documento = querySnapshot.Documents.First(); //Toma el primer documento encontrado
                    var usuario = documento.Data; //Extrae la informacion del documento

                    //Asigna el ID del documento(documento.Reference.Id) como el Uid del usuario para asegurar que el modelo no vaya vacío, y lo retorna.
                    if (usuario != null)
                    {
                        usuario.Uid = documento.Reference.Id;
                        return usuario;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al buscar por correo: {ex.Message}");
                return null;
            }
        }

        //Servicio para actualizar el perfil de un usuario
        public async Task ActualizarPerfilAsync(string uid, string nombre, string apellidos, string telefono)
        {
            var usuario = await ObtenerUsuarioPorUidAsync(uid); //Llama el servicio creado para pasarle el uid para obtener el objeto completo del usuario
            if (usuario == null) return; //Si el usaurio no existe, es nulo

            //Modifica los datos en memoria local
            usuario.Nombre = nombre;
            usuario.Apellidos = apellidos;
            usuario.Telefono = telefono;

            await GuardarUsuarioAsync(usuario); //Modifica los datos en la nube
        }

        // Método para la recuperación de contraseña
        public async Task<bool> RecuperarPasswordAsync(string email)
        {
            try
            {
                await CrossFirebaseAuth.Current.SendPasswordResetEmailAsync(email); //Se conecta a la autenticacion de Firebase y ordena que genere y envie un correo de recuperacion al email
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al enviar correo de recuperación: {ex.Message}");
                return false;
            }
        }

        //  Servicio para actualizar únicamente la foto de perfil en Firestore
        public async Task ActualizarFotoPerfilAsync(string uid, string fotoBase64) //fotoBase64 recibe la imagen convertida a texto 
        {
            try
            {
                var usuario = await ObtenerUsuarioPorUidAsync(uid); //Se obtiene el usuario por uid por medio del servicio

                //solo procede a actualizar si el usuario realmente existe en el sistema.
                if (usuario != null) 
                {
                    usuario.FotoUrl = fotoBase64; 
                    await GuardarUsuarioAsync(usuario);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al actualizar la foto: {ex.Message}");
                throw;
            }
        }
    }
}