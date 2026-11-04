namespace XelaTechSoporte
{
    public class RepuestoUtilizado
    {
        private Repuesto repuesto;
        private int cantidad;
        private decimal precioAplicado;

        public RepuestoUtilizado(Repuesto repuesto, int cantidad, decimal precioAplicado)
        {
            this.repuesto = repuesto;
            this.cantidad = cantidad;
            this.precioAplicado = precioAplicado;
        }

        public Repuesto Repuesto { get { return repuesto; } }
        public int Cantidad { get { return cantidad; } }
        public decimal PrecioAplicado { get { return precioAplicado; } }

        public decimal CalcularSubtotal()
        {
            return cantidad * precioAplicado;
        }
    }
}
