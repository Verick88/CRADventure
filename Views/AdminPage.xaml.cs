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

    //Botón Buscar
    private async void OnBuscarClicked(object sender, EventArgs e)
    {
        string email = txtEmailBusqueda.Text?.Trim();

        if (string.IsNullOrEmpty(email))
        {
            await DisplayAlert("Atención", "Por favor escribe un correo para buscar.", "OK");
            return;
        }

        //Ocultar la tarjeta si habia una busqueda anterior
        frmResultado.IsVisible = false;

        //Se obtiene al user por email
        _usuarioEncontrado = await _usuarioService.ObtenerUsuarioPorEmailAsync(email);

        if (_usuarioEncontrado != null)
        {
            //Se llena la tarjeta con datos de firestore
            lblNombreUsuario.Text = $"{_usuarioEncontrado.Nombre} {_usuarioEncontrado.Apellidos}";
            lblCorreoUsuario.Text = _usuarioEncontrado.Email;

            //Si el rol viene vacio, se asume que es cliente
            lblRolActual.Text = string.IsNullOrEmpty(_usuarioEncontrado.Rol) ? "cliente" : _usuarioEncontrado.Rol;

            //Se muestra la tarjeta
            frmResultado.IsVisible = true;
        }
        else
        {
            await DisplayAlert("No encontrado", "No existe ningún usuario con ese correo.", "OK");
        }
    }

    //Botón Hacer Cliente
    private async void OnRevocarClicked(object sender, EventArgs e)
    {
        if (_usuarioEncontrado != null)
        {
            //Se cambia el rol localmente
            _usuarioEncontrado.Rol = "cliente";

            //Se cambia en Firebase usando el servicio
            await _usuarioService.GuardarUsuarioAsync(_usuarioEncontrado);

            // Se actualiza en la pantalla para que diga cliente
            lblRolActual.Text = "cliente";

            await DisplayAlert("Éxito", "Permisos revocados. El usuario ahora es un Cliente normal.", "OK");
        }
    }

    //Boton hacer guia
    private async void OnAscenderClicked(object sender, EventArgs e)
    {
        if (_usuarioEncontrado != null)
        {
            //Se cambia el rol localmente
            _usuarioEncontrado.Rol = "guia";

            //Se guarda en Firebase usando el servicio
            await _usuarioService.GuardarUsuarioAsync(_usuarioEncontrado);

            //Se actualiza en la pantalla para que diga guia
            lblRolActual.Text = "guia";

            await DisplayAlert("Éxito", "Permisos otorgados. El usuario ahora es un Guía.", "OK");
        }
    }

    private async void OnVolverClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync(); 
    }
}