using System;
using System.Collections.Generic;

namespace XelaTechSoporte
{
    public class GestorClientes
    {
        private List<Cliente> clientes = new List<Cliente>();
        private int contador = 0;

        private string GenerarCodigo()
        {
            contador++;
            return "CLI-" + contador.ToString("D4");
        }

        public bool ExisteCorreo(string correo)
        {
            foreach (Cliente c in clientes)
            {
                if (string.Equals(c.Correo, correo.Trim(), StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        public Cliente Registrar(string nombre, string telefono, string correo, TipoCliente tipo)
        {
            Validador.TextoObligatorio("Nombre completo", nombre);
            Validador.TelefonoValido(telefono);
            Validador.CorreoValido(correo);
            if (ExisteCorreo(correo))
                throw new ReglaNegocioException("Ya existe un cliente con el correo " + correo.Trim() + ".");

            Cliente nuevo = new Cliente(GenerarCodigo(), nombre, telefono, correo, tipo);
            clientes.Add(nuevo);
            return nuevo;
        }

        public Cliente ObtenerPorCodigo(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return null;
            foreach (Cliente c in clientes)
            {
                if (string.Equals(c.Codigo, codigo.Trim(), StringComparison.OrdinalIgnoreCase))
                    return c;
            }
            return null;
        }

        public void Actualizar(string codigo, string telefono, string correo)
        {
            Cliente c = ObtenerPorCodigo(codigo);
            if (c == null)
                throw new ReglaNegocioException("No existe un cliente con el código " + codigo + ".");
            Validador.TelefonoValido(telefono);
            Validador.CorreoValido(correo);

            foreach (Cliente otro in clientes)
            {
                if (otro != c && string.Equals(otro.Correo, correo.Trim(), StringComparison.OrdinalIgnoreCase))
                    throw new ReglaNegocioException("El correo " + correo.Trim() + " ya pertenece a otro cliente.");
            }
            c.ActualizarContacto(telefono, correo);
        }

        public List<Cliente> ObtenerTodos()
        {
            return clientes;
        }
    }
}
