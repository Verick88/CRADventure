using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls.Maps;

namespace CRadventure.Models
{
    internal class CustomPin : Pin //Crear Pin personalizado
    {
        public string ImageSource { get; set; } = string.Empty; //Asignar ruta de imagen
        public Action? OnClicked { get; set; } //Define una accion cuando el usuario de click a un pin
    }
}
