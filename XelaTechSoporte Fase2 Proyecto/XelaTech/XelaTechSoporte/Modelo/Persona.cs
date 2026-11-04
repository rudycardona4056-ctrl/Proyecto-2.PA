namespace XelaTechSoporte
{
    
    public abstract class Persona
    {
        protected string codigo;
        protected string nombreCompleto;

        protected Persona(string codigo, string nombre)
        {
            Validador.TextoObligatorio("Código", codigo);
            Validador.TextoObligatorio("Nombre completo", nombre);
            this.codigo = codigo.Trim();
            this.nombreCompleto = nombre.Trim();
        }

        public string Codigo { get { return codigo; } }
        public string NombreCompleto { get { return nombreCompleto; } }

        public abstract string ObtenerResumen();

        public override string ToString()
        {
            return codigo + " - " + nombreCompleto;
        }
    }
}
