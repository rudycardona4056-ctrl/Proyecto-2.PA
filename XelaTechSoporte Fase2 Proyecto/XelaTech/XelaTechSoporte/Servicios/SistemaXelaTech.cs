using System.Collections.Generic;

namespace XelaTechSoporte
{
    public class SistemaXelaTech
    {
        public GestorClientes Clientes { get; private set; }
        public GestorTecnicos Tecnicos { get; private set; }
        public GestorEquipos Equipos { get; private set; }
        public GestorRepuestos Repuestos { get; private set; }
        public GestorSolicitudes Solicitudes { get; private set; }
        public MatrizCarga Carga { get; private set; }
        public List<ResultadoBusqueda> Busquedas { get; private set; }

        public SistemaXelaTech()
        {
            Clientes = new GestorClientes();
            Tecnicos = new GestorTecnicos();
            Equipos = new GestorEquipos();
            Repuestos = new GestorRepuestos();
            Carga = new MatrizCarga();
            Solicitudes = new GestorSolicitudes(Clientes, Equipos, Tecnicos, Repuestos, Carga);
            Busquedas = new List<ResultadoBusqueda>();
        }
    }
}
