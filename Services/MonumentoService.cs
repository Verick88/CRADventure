using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.Firebase.Firestore;
using CRadventure.Models;

namespace CRadventure.Services;

public class MonumentoService
{
    private const string Coleccion = "monumentos";

    public async Task<List<MonumentoModel>> ObtenerMonumentosAsync()
    {
        var resultado = await CrossFirebaseFirestore.Current
            .GetCollection(Coleccion)
            .GetDocumentsAsync<MonumentoModel>();

        return resultado.Documents //Accede a la lista de documentos
            .Select(d => d.Data)//Recorre la lista extrayendo los datos
            .Where(m => m.Activo) //Filtra que el activo sea true
            .ToList(); //Convierte a lista
    }
}