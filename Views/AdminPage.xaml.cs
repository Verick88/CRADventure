using System;
using CRadventure.Models;
using CRadventure.Services;
using Microsoft.Maui.Controls;

namespace CRadventure.Views;

public partial class AdminPage : ContentPage
{
    private readonly UsuarioService _usuarioService;

    // Aquí guardamos temporalmente en la memoria al usuario que acabamos de buscar
    private UsuarioModel _usuarioEncontrado;

    public AdminPage()
    {
        InitializeComponent();
        _usuarioService = new UsuarioService();
    }

    // 1. Botón Buscar
    private async void OnBuscarClicked(object sender, EventArgs e)
    {
        string email = txtEmailBusqueda.Text?.Trim();

        if (string.IsNullOrEmpty(email))
        {
            await DisplayAlert("Atención", "Por favor escribe un correo para buscar.", "OK");
            return;
        }

        // Ocultamos la tarjeta por si había una búsqueda anterior
        frmResultado.IsVisible = false;

        // Vamos a la base de datos
        _usuarioEncontrado = await _usuarioService.ObtenerUsuarioPorEmailAsync(email);

        if (_usuarioEncontrado != null)
        {
            // Llenamos la tarjeta con los datos de Firestore
            lblNombreUsuario.Text = $"{_usuarioEncontrado.Nombre} {_usuarioEncontrado.Apellidos}";
            lblCorreoUsuario.Text = _usuarioEncontrado.Email;

            // Si el rol viene vacío por alguna razón, asumimos que es cliente
            lblRolActual.Text = string.IsNullOrEmpty(_usuarioEncontrado.Rol) ? "cliente" : _usuarioEncontrado.Rol;

            // Mostramos la tarjeta
            frmResultado.IsVisible = true;
        }
        else
        {
            await DisplayAlert("No encontrado", "No existe ningún usuario con ese correo.", "OK");
        }
    }

    // 2. Botón Hacer Cliente (Rojo)
    private async void OnRevocarClicked(object sender, EventArgs e)
    {
        if (_usuarioEncontrado != null)
        {
            // Cambiamos el rol localmente
            _usuarioEncontrado.Rol = "cliente";

            // Lo guardamos en Firebase usando nuestro servicio
            await _usuarioService.GuardarUsuarioAsync(_usuarioEncontrado);

            // Actualizamos la pantallita para que diga "cliente"
            lblRolActual.Text = "cliente";

            await DisplayAlert("Éxito", "Permisos revocados. El usuario ahora es un Cliente normal.", "OK");
        }
    }

    // 3. Botón Hacer Guía (Verde)
    private async void OnAscenderClicked(object sender, EventArgs e)
    {
        if (_usuarioEncontrado != null)
        {
            // Cambiamos el rol localmente
            _usuarioEncontrado.Rol = "guia";

            // Lo guardamos en Firebase usando nuestro servicio
            await _usuarioService.GuardarUsuarioAsync(_usuarioEncontrado);

            // Actualizamos la pantallita para que diga "guia"
            lblRolActual.Text = "guia";

            await DisplayAlert("Éxito", "Permisos otorgados. El usuario ahora es un Guía.", "OK");
        }
    }
}