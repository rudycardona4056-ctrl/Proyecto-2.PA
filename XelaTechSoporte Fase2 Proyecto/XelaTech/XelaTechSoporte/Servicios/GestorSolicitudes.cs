using System;
using System.Collections.Generic;

namespace XelaTechSoporte
{

    public class GestorSolicitudes
    {
        private List<Solicitud> solicitudes = new List<Solicitud>();
        private Queue<Solicitud> colaUrgente = new Queue<Solicitud>();
        private Queue<Solicitud> colaNormal = new Queue<Solicitud>();
        private int contador = 0;

        private GestorClientes clientes;
        private GestorEquipos equipos;
        private GestorTecnicos tecnicos;
        private GestorRepuestos repuestos;
        private MatrizCarga matriz;

        public GestorSolicitudes(GestorClientes clientes, GestorEquipos equipos, GestorTecnicos tecnicos,
                                 GestorRepuestos repuestos, MatrizCarga matriz)
        {
            this.clientes = clientes;
            this.equipos = equipos;
            this.tecnicos = tecnicos;
            this.repuestos = repuestos;
            this.matriz = matriz;
        }

        public Solicitud ObtenerPorCodigo(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return null;
            foreach (Solicitud s in solicitudes)
            {
                if (string.Equals(s.Codigo, codigo.Trim(), StringComparison.OrdinalIgnoreCase))
                    return s;
            }
            return null;
        }

        private Solicitud ObtenerObligatoria(string codigo)
        {
            Solicitud s = ObtenerPorCodigo(codigo);
            if (s == null)
                throw new ReglaNegocioException("No existe una solicitud con el código " + codigo + ".");
            return s;
        }

        public List<Solicitud> ObtenerTodas()
        {
            return solicitudes;
        }

        public List<Solicitud> ObtenerEnCola(PrioridadSolicitud prioridad)
        {
            Queue<Solicitud> cola = prioridad == PrioridadSolicitud.Urgente ? colaUrgente : colaNormal;
            List<Solicitud> lista = new List<Solicitud>();
            foreach (Solicitud s in cola)              
            {
                if (s.Estado == EstadoSolicitud.EnEspera)
                    lista.Add(s);
            }
            return lista;
        }

        public bool TieneSolicitudActiva(string codigoEquipo)
        {
            foreach (Solicitud s in solicitudes)
            {
                if (s.Equipo.CodigoInterno == codigoEquipo && s.EstaActiva())
                    return true;
            }
            return false;
        }

        public Solicitud CrearSolicitud(string codigoCliente, string codigoEquipo, string descripcion,
                                        PrioridadSolicitud prioridad)
        {
            Cliente cliente = clientes.ObtenerPorCodigo(codigoCliente);
            if (cliente == null)
                throw new ReglaNegocioException("El cliente " + codigoCliente + " no existe.");
            Equipo equipo = equipos.ObtenerPorCodigo(codigoEquipo);
            if (equipo == null)
                throw new ReglaNegocioException("El equipo " + codigoEquipo + " no existe.");
            if (TieneSolicitudActiva(equipo.CodigoInterno))
                throw new ReglaNegocioException("El equipo " + equipo.CodigoInterno + " ya tiene una solicitud activa.");
            Validador.TextoObligatorio("Descripción del problema", descripcion);

            contador++;
            Solicitud nueva = new Solicitud("SOL-" + contador.ToString("D4"), cliente, equipo, descripcion, prioridad);
            solicitudes.Add(nueva);
            cliente.IncrementarSolicitudes();

            nueva.CambiarEstado(EstadoSolicitud.EnEspera, "Solicitud encolada");
            if (prioridad == PrioridadSolicitud.Urgente)
                colaUrgente.Enqueue(nueva);
            else
                colaNormal.Enqueue(nueva);
            return nueva;
        }

        private void DescartarCanceladas(Queue<Solicitud> cola)
        {
            while (cola.Count > 0 && cola.Peek().Estado == EstadoSolicitud.Cancelada)
                cola.Dequeue();
        }

        public Solicitud AtenderSiguiente()
        {
            return AtenderSiguiente(DateTime.Now.DayOfWeek);
        }

       
        public Solicitud AtenderSiguiente(DayOfWeek dia)
        {
            DescartarCanceladas(colaUrgente);
            DescartarCanceladas(colaNormal);

            Queue<Solicitud> cola;
            if (colaUrgente.Count > 0)
                cola = colaUrgente;
            else if (colaNormal.Count > 0)
                cola = colaNormal;
            else
                throw new ReglaNegocioException("No hay solicitudes en espera en ninguna cola.");

            Solicitud siguiente = cola.Peek();     
            Especialidad requerida = siguiente.Equipo.ObtenerEspecialidadRequerida();
            Tecnico tecnico = tecnicos.BuscarMejorDisponible(requerida);
            if (tecnico == null)
                throw new ReglaNegocioException("No hay técnico disponible con especialidad " + requerida
                                                + ". La solicitud " + siguiente.Codigo + " permanece en la cola.");

            cola.Dequeue();
            siguiente.AsignarTecnico(tecnico);
            tecnico.IniciarTrabajo();
            siguiente.CambiarEstado(EstadoSolicitud.Asignada, "Asignada a " + tecnico.NombreCompleto);
            matriz.RegistrarAsignacion(tecnico.IndiceMatriz, dia);
            return siguiente;
        }

        public void RegistrarDiagnostico(string codigo, string texto, decimal costoAdicional)
        {
            Solicitud s = ObtenerObligatoria(codigo);
            Validador.NoNegativo("Costo adicional", costoAdicional);
            Validador.TextoObligatorio("Diagnóstico", texto);
            if (!s.EstaActiva())
                throw new ReglaNegocioException("La solicitud ya fue entregada o cancelada; no puede modificarse.");
            if (s.Tecnico == null)
                throw new ReglaNegocioException("La solicitud aún no tiene técnico asignado.");

            if (s.Estado == EstadoSolicitud.Asignada)
                s.CambiarEstado(EstadoSolicitud.EnDiagnostico, "Inicio de diagnóstico");

            decimal manoObra = s.Equipo.CalcularCostoRevision() + costoAdicional;
            s.RegistrarDiagnostico(texto, manoObra);
        }

        public Repuesto AgregarRepuesto(string codigo, string codigoRepuesto, int cantidad)
        {
            Solicitud s = ObtenerObligatoria(codigo);
            if (!s.EstaActiva())
                throw new ReglaNegocioException("La solicitud ya fue entregada o cancelada; no puede modificarse.");
            Repuesto r = repuestos.ObtenerPorCodigo(codigoRepuesto);
            if (r == null)
                throw new ReglaNegocioException("No existe un repuesto con el código " + codigoRepuesto + ".");
            s.AgregarRepuesto(r, cantidad);
            return r;
        }

      
        public void ActualizarEstado(string codigo, EstadoSolicitud nuevo, string observacion)
        {
            Solicitud s = ObtenerObligatoria(codigo);
            if (!s.EstaActiva())
                throw new ReglaNegocioException("La solicitud " + s.Codigo + " ya fue entregada o cancelada; no puede modificarse.");
            if (!s.EsTransicionValida(nuevo))
                throw new ReglaNegocioException("Cambio de estado no permitido: de " + s.Estado + " a " + nuevo + ".");
            if (nuevo == EstadoSolicitud.Asignada)
                throw new ReglaNegocioException("La asignación de técnico solo se realiza desde 'Atención de colas'.");
            if (nuevo == EstadoSolicitud.Entregada)
                throw new ReglaNegocioException("Use la opción 'Registrar entrega' (requiere la calificación del cliente).");

            bool tenia = s.TecnicoTieneCargaActiva();
            s.CambiarEstado(nuevo, observacion);
          
            if (tenia && !s.TecnicoTieneCargaActiva() && s.Tecnico != null)
                s.Tecnico.TerminarTrabajo();
        }

        public Solicitud RegistrarEntrega(string codigo, int calificacion)
        {
            Solicitud s = ObtenerObligatoria(codigo);
            Validador.CalificacionValida(calificacion);
            if (!s.EstaActiva())
                throw new ReglaNegocioException("La solicitud " + s.Codigo + " ya fue entregada o cancelada; no puede modificarse.");

            s.CambiarEstado(EstadoSolicitud.Entregada, "Equipo entregado al cliente");   
            s.EstablecerCalificacion(calificacion);
            if (s.Tecnico != null)
                s.Tecnico.RegistrarServicioFinalizado(calificacion);
            tecnicos.RegistrarCalificacion(calificacion);
            return s;
        }

        public Solicitud DeshacerUltimoCambio(string codigo)
        {
            Solicitud s = ObtenerObligatoria(codigo);

            if (s.Estado == EstadoSolicitud.ListaParaEntregar && s.Tecnico != null
                && s.Tecnico.TrabajosActivos >= Tecnico.LIMITE_TRABAJOS)
                throw new ReglaNegocioException("No se puede deshacer: el técnico " + s.Tecnico.NombreCompleto
                                                + " alcanzó el límite de trabajos activos.");

            EstadoSolicitud deshecho = s.DeshacerUltimoCambio();
            if (deshecho == EstadoSolicitud.ListaParaEntregar && s.Tecnico != null)
                s.Tecnico.ReanudarTrabajo();
            return s;
        }

        public List<RegistroEstado> ConsultarHistorial(string codigo)
        {
            return ObtenerObligatoria(codigo).ConsultarHistorial();
        }
    }
}
