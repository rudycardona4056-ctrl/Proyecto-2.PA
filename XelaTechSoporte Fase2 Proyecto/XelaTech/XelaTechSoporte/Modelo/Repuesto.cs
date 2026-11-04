namespace XelaTechSoporte
{
    public class Repuesto
    {
        private string codigo;
        private string nombre;
        private string categoria;
        private int existencia;
        private int existenciaMinima;
        private decimal costoUnitario;
        private decimal precioVenta;
        private int cantidadUtilizada;

        public Repuesto(string codigo, string nombre, string categoria, int existencia,
                        int existenciaMinima, decimal costoUnitario, decimal precioVenta)
        {
            Validador.TextoObligatorio("Código", codigo);
            Validador.TextoObligatorio("Nombre", nombre);
            Validador.TextoObligatorio("Categoría", categoria);
            Validador.NoNegativo("Existencia", existencia);
            Validador.NoNegativo("Existencia mínima", existenciaMinima);
            Validador.NoNegativo("Costo unitario", costoUnitario);
            Validador.NoNegativo("Precio de venta", precioVenta);
            if (precioVenta < costoUnitario)
                throw new ReglaNegocioException("El precio de venta no puede ser menor que el costo unitario.");

            this.codigo = codigo.Trim();
            this.nombre = nombre.Trim();
            this.categoria = categoria.Trim();
            this.existencia = existencia;
            this.existenciaMinima = existenciaMinima;
            this.costoUnitario = costoUnitario;
            this.precioVenta = precioVenta;
            this.cantidadUtilizada = 0;
        }

        public string Codigo { get { return codigo; } }
        public string Nombre { get { return nombre; } }
        public string Categoria { get { return categoria; } }
        public int Existencia { get { return existencia; } }
        public int ExistenciaMinima { get { return existenciaMinima; } }
        public decimal CostoUnitario { get { return costoUnitario; } }
        public decimal PrecioVenta { get { return precioVenta; } }
        public int CantidadUtilizada { get { return cantidadUtilizada; } }

        public bool HayExistencia(int cantidad)
        {
            return cantidad > 0 && cantidad <= existencia;
        }

        public void Descontar(int cantidad)
        {
            Validador.EnteroPositivo("Cantidad", cantidad);
            if (!HayExistencia(cantidad))
                throw new ReglaNegocioException("Existencia insuficiente de '" + nombre + "': hay "
                                                + existencia + " y se solicitaron " + cantidad + ".");
            existencia -= cantidad;
            cantidadUtilizada += cantidad;
        }

        public void AgregarExistencia(int cantidad)
        {
            Validador.EnteroPositivo("Cantidad", cantidad);
            existencia += cantidad;
        }

        public bool EstaAgotado()
        {
            return existencia == 0;
        }

        public bool EstaEnMinimo()
        {
            return existencia <= existenciaMinima;
        }

        public string ObtenerSituacion()
        {
            if (EstaAgotado()) return "AGOTADO";
            if (EstaEnMinimo()) return "MÍNIMO";
            return "Normal";
        }

        public override string ToString()
        {
            return codigo + " | " + nombre + " | Existencia: " + existencia + " (mín. " + existenciaMinima + ")";
        }
    }
}
