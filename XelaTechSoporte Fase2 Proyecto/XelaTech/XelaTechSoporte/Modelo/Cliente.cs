namespace XelaTechSoporte
{
   
    public class Cliente : Persona
    {
        private string telefono;
        private string correo;
        private TipoCliente tipoCliente;
        private int cantidadSolicitudes;

        
        public Cliente(string codigo, string nombre, string telefono, string correo, TipoCliente tipo)
            : base(codigo, nombre)
        {
            Validador.TelefonoValido(telefono);
            Validador.CorreoValido(correo);
            this.telefono = telefono.Trim();
            this.correo = correo.Trim();
            this.tipoCliente = tipo;
            this.cantidadSolicitudes = 0;
        }

        
        public Cliente(string codigo, string nombre, string telefono, string correo)
            : this(codigo, nombre, telefono, correo, TipoCliente.Particular)
        {
        }

        public string Telefono { get { return telefono; } }
        public string Correo { get { return correo; } }
        public TipoCliente Tipo { get { return tipoCliente; } }
        public int CantidadSolicitudes { get { return cantidadSolicitudes; } }

        
        public void ActualizarContacto(string nuevoTelefono, string nuevoCorreo)
        {
            Validador.TelefonoValido(nuevoTelefono);
            Validador.CorreoValido(nuevoCorreo);
            telefono = nuevoTelefono.Trim();
            correo = nuevoCorreo.Trim();
        }

        
        public void IncrementarSolicitudes()
        {
            cantidadSolicitudes++;
        }

        public override string ObtenerResumen()
        {
            return codigo + " | " + nombreCompleto + " | Tel: " + telefono + " | " + correo
                   + " | " + tipoCliente + " | Solicitudes: " + cantidadSolicitudes;
        }
    }
}
