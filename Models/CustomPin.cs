using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls.Maps;

namespace CRadventure.Models
{
    internal class CustomPin : Pin
    {
        public string ImageSource { get; set; } = string.Empty;
        public Action? OnClicked { get; set; }
    }
}
