using System;
using System.Collections.Generic;

namespace XelaTechSoporte
{
    
    public class Solicitud
    {
        private string codigo;
        private Cliente cliente;
        private Equipo equipo;
        private string descripcion;
        private DateTime fechaRecepcion;
        private PrioridadSolicitud prioridad;
        private EstadoSolicitud estado;
        private Tecnico tecnico;
        private string diagnostico;
        private decimal costoManoObra;
        private double tiempoEstimadoHoras;
        private DateTime? fechaEntrega;
        private int calificacion;
        private List<RepuestoUtilizado> repuestosUtilizados;
        private Stack<RegistroEstado> historial;

        public Solicitud(string codigo, Cliente cliente, Equipo equipo, string descripcion, PrioridadSolicitud prioridad)
        {
            Validador.TextoObligatorio("Código de solicitud", codigo);
            Validador.TextoObligatorio("Descripción del problema", descripcion);
            if (cliente == null)
                throw new ReglaNegocioException("No se puede crear una solicitud para un cliente inexistente.");
            if (equipo == null)
                throw new ReglaNegocioException("No se puede crear una solicitud para un equipo inexistente.");

            this.codigo = codigo.Trim();
            this.cliente = cliente;
            this.equipo = equipo;
            this.descripcion = descripcion.Trim();
            this.prioridad = prioridad;
            this.fechaRecepcion = DateTime.Now;
            this.estado = EstadoSolicitud.Recibida;
            this.tecnico = null;
            this.diagnostico = "";
            this.costoManoObra = 0m;
            this.tiempoEstimadoHoras = equipo.EstimarTiempoHoras(); 
            this.fechaEntrega = null;
            this.calificacion = 0;
            this.repuestosUtilizados = new List<RepuestoUtilizado>();
            this.historial = new Stack<RegistroEstado>();
        }

        public Solicitud(string codigo, Cliente cliente, Equipo equipo, string descripcion)
            : this(codigo, cliente, equipo, descripcion, PrioridadSolicitud.Normal)
        {
        }

        public string Codigo { get { return codigo; } }
        public Cliente Cliente { get { return cliente; } }
        public Equipo Equipo { get { return equipo; } }
        public string Descripcion { get { return descripcion; } }
        public DateTime FechaRecepcion { get { return fechaRecepcion; } }
        public PrioridadSolicitud Prioridad { get { return prioridad; } }
        public EstadoSolicitud Estado { get { return estado; } }
        public Tecnico Tecnico { get { return tecnico; } }
        public string Diagnostico { get { return diagnostico; } }
        public decimal CostoManoObra { get { return costoManoObra; } }
        public double TiempoEstimadoHoras { get { return tiempoEstimadoHoras; } }
        public DateTime? FechaEntrega { get { return fechaEntrega; } }
        public int Calificacion { get { return calificacion; } }
        public List<RepuestoUtilizado> RepuestosUtilizados { get { return repuestosUtilizados; } }
        public int CantidadHistorial { get { return historial.Count; } }
        public decimal CostoRepuestos { get { return CalcularCostoRepuestos(); } }
        public decimal Total { get { return CalcularTotal(); } }

        
        public bool EstaActiva()
        {
            return estado != EstadoSolicitud.Entregada && estado != EstadoSolicitud.Cancelada;
        }

        public bool EstaFinalizada()
        {
            return estado == EstadoSolicitud.Entregada;
        }

        public bool TecnicoTieneCargaActiva()
        {
            return estado == EstadoSolicitud.Asignada
                   || estado == EstadoSolicitud.EnDiagnostico
                   || estado == EstadoSolicitud.EnReparacion;
        }

        public bool EsTransicionValida(EstadoSolicitud nuevo)
        {
            switch (estado)
            {
                case EstadoSolicitud.Recibida:
                    return nuevo == EstadoSolicitud.EnEspera || nuevo == EstadoSolicitud.Cancelada;
                case EstadoSolicitud.EnEspera:
                    return nuevo == EstadoSolicitud.Asignada || nuevo == EstadoSolicitud.Cancelada;
                case EstadoSolicitud.Asignada:
                    return nuevo == EstadoSolicitud.EnDiagnostico || nuevo == EstadoSolicitud.Cancelada;
                case EstadoSolicitud.EnDiagnostico:
                    return nuevo == EstadoSolicitud.EnReparacion || nuevo == EstadoSolicitud.Cancelada;
                case EstadoSolicitud.EnReparacion:
                    return nuevo == EstadoSolicitud.ListaParaEntregar || nuevo == EstadoSolicitud.Cancelada;
                case EstadoSolicitud.ListaParaEntregar:
                    return nuevo == EstadoSolicitud.Entregada || nuevo == EstadoSolicitud.Cancelada;
                default:
                    return false;   
            }
        }

        
        public void CambiarEstado(EstadoSolicitud nuevo, string observacion)
        {
            if (!EstaActiva())
                throw new ReglaNegocioException("La solicitud " + codigo + " ya fue " + estado.ToString().ToLower()
                                                + " y no puede modificarse.");
            if (!EsTransicionValida(nuevo))
                throw new ReglaNegocioException("Cambio de estado no permitido: de " + estado + " a " + nuevo + ".");

            historial.Push(new RegistroEstado(estado, DateTime.Now, observacion));
            estado = nuevo;
            if (nuevo == EstadoSolicitud.Entregada)
                fechaEntrega = DateTime.Now;
        }

        
        public EstadoSolicitud DeshacerUltimoCambio()
        {
            if (!EstaActiva())
                throw new ReglaNegocioException("No se puede deshacer: la solicitud " + codigo + " ya fue entregada o cancelada.");
            if (estado != EstadoSolicitud.EnDiagnostico && estado != EstadoSolicitud.EnReparacion
                && estado != EstadoSolicitud.ListaParaEntregar)
                throw new ReglaNegocioException("Solo se pueden deshacer cambios cuando la solicitud está en diagnóstico, "
                                                + "reparación o lista para entregar.");
            if (historial.Count == 0)                       
                throw new ReglaNegocioException("El historial de la solicitud está vacío; no hay nada que deshacer.");

            EstadoSolicitud deshecho = estado;
            RegistroEstado registro = historial.Pop();
            estado = registro.EstadoAnterior;
            return deshecho;
        }

      
        public List<RegistroEstado> ConsultarHistorial()
        {
            List<RegistroEstado> lista = new List<RegistroEstado>();
            foreach (RegistroEstado r in historial)
                lista.Add(r);
            return lista;
        }

       
        public void AsignarTecnico(Tecnico t)
        {
            if (t == null)
                throw new ReglaNegocioException("Debe indicar un técnico.");
            if (estado != EstadoSolicitud.EnEspera)
                throw new ReglaNegocioException("Solo se puede asignar técnico a una solicitud en espera.");
            tecnico = t;
        }

        public void RegistrarDiagnostico(string texto, decimal costoManoObra)
        {
            Validador.TextoObligatorio("Diagnóstico", texto);
            Validador.NoNegativo("Costo de mano de obra", costoManoObra);
            if (tecnico == null)
                throw new ReglaNegocioException("No se puede registrar diagnóstico: la solicitud no tiene técnico asignado.");
            if (estado != EstadoSolicitud.EnDiagnostico && estado != EstadoSolicitud.EnReparacion)
                throw new ReglaNegocioException("El diagnóstico solo se registra con la solicitud en diagnóstico o reparación.");
            this.diagnostico = texto.Trim();
            this.costoManoObra = costoManoObra;
        }

        public void AgregarRepuesto(Repuesto repuesto, int cantidad)
        {
            if (repuesto == null)
                throw new ReglaNegocioException("El repuesto no existe.");
            Validador.EnteroPositivo("Cantidad", cantidad);
            if (estado != EstadoSolicitud.EnDiagnostico && estado != EstadoSolicitud.EnReparacion)
                throw new ReglaNegocioException("Los repuestos solo se registran con la solicitud en diagnóstico o reparación.");

            repuesto.Descontar(cantidad);   
            repuestosUtilizados.Add(new RepuestoUtilizado(repuesto, cantidad, repuesto.PrecioVenta));
        }

        public void AgregarRepuesto(Repuesto repuesto)
        {
            AgregarRepuesto(repuesto, 1);
        }

        public void EstablecerCalificacion(int valor)
        {
            Validador.CalificacionValida(valor);
            calificacion = valor;
        }

        public void EstablecerFechaRecepcion(DateTime fecha)
        {
            if (estado != EstadoSolicitud.Recibida && estado != EstadoSolicitud.EnEspera)
                throw new ReglaNegocioException("La fecha de recepción solo puede fijarse mientras la solicitud espera.");
            fechaRecepcion = fecha;
        }

        
        public decimal CalcularCostoRepuestos()
        {
            decimal suma = 0m;
            foreach (RepuestoUtilizado r in repuestosUtilizados)
                suma += r.CalcularSubtotal();
            return suma;
        }

        public decimal CalcularTotal()
        {
            return costoManoObra + CalcularCostoRepuestos();
        }

        public override string ToString()
        {
            return codigo + " | " + cliente.NombreCompleto + " | " + equipo.CodigoInterno + " | " + prioridad + " | " + estado;
        }
    }
}
