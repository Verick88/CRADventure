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

        return resultado.Documents
            .Select(d => d.Data)
            .Where(m => m.Activo)
            .ToList();
    }
}