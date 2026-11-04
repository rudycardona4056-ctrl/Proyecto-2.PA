using System;

namespace XelaTechSoporte
{
    public class RegistroEstado
    {
        private EstadoSolicitud estadoAnterior;
        private DateTime fechaHora;
        private string observacion;

        public RegistroEstado(EstadoSolicitud estadoAnterior, DateTime fechaHora, string observacion)
        {
            this.estadoAnterior = estadoAnterior;
            this.fechaHora = fechaHora;
            this.observacion = observacion;
        }

        public EstadoSolicitud EstadoAnterior { get { return estadoAnterior; } }
        public DateTime FechaHora { get { return fechaHora; } }
        public string Observacion { get { return observacion; } }
    }
}
