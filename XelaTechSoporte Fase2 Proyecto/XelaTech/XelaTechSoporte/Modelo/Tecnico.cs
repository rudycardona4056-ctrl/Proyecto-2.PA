namespace XelaTechSoporte
{
   
    public class Tecnico : Persona
    {
        public const int LIMITE_TRABAJOS = 3;

        private Especialidad especialidad;
        private bool disponible;
        private int trabajosActivos;
        private int serviciosFinalizados;
        private int sumaCalificaciones;
        private int cantidadCalificaciones;
        private int indiceMatriz;

        public Tecnico(string codigo, string nombre, Especialidad especialidad, int indiceMatriz)
            : base(codigo, nombre)
        {
            this.especialidad = especialidad;
            this.disponible = true;
            this.trabajosActivos = 0;
            this.serviciosFinalizados = 0;
            this.sumaCalificaciones = 0;
            this.cantidadCalificaciones = 0;
            this.indiceMatriz = indiceMatriz;
        }

        public Especialidad Especialidad { get { return especialidad; } }
        public bool Disponible { get { return disponible; } }
        public int TrabajosActivos { get { return trabajosActivos; } }
        public int ServiciosFinalizados { get { return serviciosFinalizados; } }
        public int IndiceMatriz { get { return indiceMatriz; } }
        public double CalificacionPromedio { get { return CalcularPromedio(); } }

        public bool TieneCargaCompleta { get { return trabajosActivos >= LIMITE_TRABAJOS; } }

        
        public bool PuedeRecibir(Especialidad requerida)
        {
            return disponible && especialidad == requerida && trabajosActivos < LIMITE_TRABAJOS;
        }

        public void IniciarTrabajo()
        {
            if (trabajosActivos >= LIMITE_TRABAJOS)
                throw new ReglaNegocioException("El técnico " + nombreCompleto + " ya alcanzó el límite de "
                                                + LIMITE_TRABAJOS + " trabajos activos.");
            trabajosActivos++;
        }

        public void TerminarTrabajo()
        {
            if (trabajosActivos > 0)
                trabajosActivos--;
        }

        public void ReanudarTrabajo()
        {
            if (trabajosActivos >= LIMITE_TRABAJOS)
                throw new ReglaNegocioException("No se puede reanudar: el técnico " + nombreCompleto
                                                + " alcanzó el límite de trabajos activos.");
            trabajosActivos++;
        }

        public void RegistrarServicioFinalizado(int calificacion)
        {
            Validador.CalificacionValida(calificacion);
            serviciosFinalizados++;
            sumaCalificaciones += calificacion;
            cantidadCalificaciones++;
        }

        public double CalcularPromedio()
        {
            if (cantidadCalificaciones == 0)
                return 0;
            return (double)sumaCalificaciones / cantidadCalificaciones;
        }

        public void CambiarDisponibilidad(bool valor)
        {
            disponible = valor;
        }

        public override string ObtenerResumen()
        {
            return codigo + " | " + nombreCompleto + " | " + especialidad + " | "
                   + (disponible ? "Disponible" : "No disponible") + " | Activos: " + trabajosActivos + "/" + LIMITE_TRABAJOS
                   + " | Finalizados: " + serviciosFinalizados + " | Promedio: " + CalcularPromedio().ToString("0.0");
        }
    }
}
