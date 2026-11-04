using System;
using System.Collections.Generic;

namespace XelaTechSoporte
{
    
    public class GeneradorDatos
    {
        private Random aleatorio = new Random();
        private SistemaXelaTech sistema;

        private static readonly string[] Nombres = { "María", "José", "Ana", "Luis", "Carmen", "Pedro", "Rosa", "Jorge", "Sofía", "Carlos", "Lucía", "Diego" };
        private static readonly string[] Apellidos = { "López", "Pérez", "García", "Ramírez", "Méndez", "Tzoc", "Xicay", "Cifuentes", "Morales", "Hernández" };
        private static readonly string[] Marcas = { "HP", "Dell", "Lenovo", "Epson", "Canon", "Asus", "Acer", "Brother" };
        private static readonly string[] Modelos = { "Pro 400", "Inspiron 15", "ThinkPad E14", "L3250", "Pixma G3110", "VivoBook", "Aspire 5", "HL-L2350" };
        private static readonly string[] Problemas = { "No enciende", "Pantalla dañada", "Funciona muy lento", "No imprime", "Ruido extraño",
                                                        "Se sobrecalienta", "No reconoce el disco", "Atasco de papel frecuente" };
        private static readonly string[] NombresRepuesto = { "Batería", "Disco SSD", "Memoria RAM", "Fuente de poder", "Tóner", "Cabezal de impresión",
                                                              "Ventilador", "Pantalla", "Teclado", "Cargador" };
        private static readonly string[] CategoriasRepuesto = { "Energía", "Almacenamiento", "Memoria", "Energía", "Impresión", "Impresión",
                                                                 "Refrigeración", "Visualización", "Periféricos", "Energía" };

        public GeneradorDatos(SistemaXelaTech sistema)
        {
            this.sistema = sistema;
        }

        public void Generar(int cantidad)
        {
            Validador.EnteroPositivo("Cantidad", cantidad);
            int clientesAntes = sistema.Clientes.ObtenerTodos().Count;
            int tecnicosAntes = sistema.Tecnicos.ObtenerTodos().Count;
            int repuestosAntes = sistema.Repuestos.ObtenerTodos().Count;
            int equiposAntes = sistema.Equipos.ObtenerTodos().Count;
            int solicitudesAntes = sistema.Solicitudes.ObtenerTodas().Count;

            GenerarClientes(cantidad);
            GenerarTecnicos(cantidad);
            GenerarRepuestos(cantidad);
            GenerarEquiposYSolicitudes(cantidad);
            AvanzarSolicitudes(cantidad);

            Console.WriteLine();
            Console.WriteLine("Datos generados:");
            Console.WriteLine("  Clientes:    +" + (sistema.Clientes.ObtenerTodos().Count - clientesAntes));
            Console.WriteLine("  Técnicos:    +" + (sistema.Tecnicos.ObtenerTodos().Count - tecnicosAntes));
            Console.WriteLine("  Repuestos:   +" + (sistema.Repuestos.ObtenerTodos().Count - repuestosAntes));
            Console.WriteLine("  Equipos:     +" + (sistema.Equipos.ObtenerTodos().Count - equiposAntes));
            Console.WriteLine("  Solicitudes: +" + (sistema.Solicitudes.ObtenerTodas().Count - solicitudesAntes));
        }

        private void GenerarClientes(int n)
        {
            TipoCliente[] tipos = (TipoCliente[])Enum.GetValues(typeof(TipoCliente));
            int serie = sistema.Clientes.ObtenerTodos().Count;
            for (int i = 0; i < n; i++)
            {
                string nombre = Nombres[aleatorio.Next(Nombres.Length)] + " " + Apellidos[aleatorio.Next(Apellidos.Length)];
                string telefono = "5" + aleatorio.Next(1000000, 10000000).ToString();    
                string correo;
                do
                {
                    serie++;
                    correo = "cliente" + serie + "@correo.com";
                } while (sistema.Clientes.ExisteCorreo(correo));
                sistema.Clientes.Registrar(nombre, telefono, correo, tipos[aleatorio.Next(tipos.Length)]);
            }
        }

        private void GenerarTecnicos(int n)
        {
            Especialidad[] especialidades = (Especialidad[])Enum.GetValues(typeof(Especialidad));
            int cantidadNuevos = Math.Max(3, n / 3);
            for (int i = 0; i < cantidadNuevos; i++)
            {
                if (sistema.Tecnicos.ObtenerTodos().Count >= MatrizCarga.MAX_TECNICOS)
                    break;
                Especialidad esp = especialidades[i % especialidades.Length];
                string nombre = Nombres[aleatorio.Next(Nombres.Length)] + " " + Apellidos[aleatorio.Next(Apellidos.Length)];
                sistema.Tecnicos.Registrar(nombre, esp);
            }
        }

        private void GenerarRepuestos(int n)
        {
            for (int i = 0; i < n; i++)
            {
                int indice = aleatorio.Next(NombresRepuesto.Length);
                decimal costo = aleatorio.Next(50, 501);
                decimal precio = costo + aleatorio.Next(20, 151);          
                int existencia = aleatorio.Next(0, 16);
                int minima = aleatorio.Next(1, 6);
                sistema.Repuestos.Registrar(NombresRepuesto[indice], CategoriasRepuesto[indice], existencia, minima, costo, precio);
            }
        }

        private void GenerarEquiposYSolicitudes(int n)
        {
            List<Cliente> clientes = sistema.Clientes.ObtenerTodos();
            for (int i = 0; i < n; i++)
            {
                Cliente propietario = clientes[aleatorio.Next(clientes.Count)];
                string serie;
                do
                {
                    serie = "SN" + aleatorio.Next(100000, 1000000).ToString();
                } while (sistema.Equipos.ExisteSerie(serie));

                string marca = Marcas[aleatorio.Next(Marcas.Length)];
                string modelo = Modelos[aleatorio.Next(Modelos.Length)];
                Equipo equipo;
                switch (aleatorio.Next(3))
                {
                    case 0:
                        equipo = sistema.Equipos.RegistrarPortatil(serie, marca, modelo, propietario, "Buen estado", "Cargador", aleatorio.Next(2) == 0);
                        break;
                    case 1:
                        equipo = sistema.Equipos.RegistrarEscritorio(serie, marca, modelo, propietario, "Con rayones", "Cable de poder", aleatorio.Next(2) == 0);
                        break;
                    default:
                        equipo = sistema.Equipos.RegistrarImpresora(serie, marca, modelo, propietario, "Buen estado", "Cable USB",
                                                                    aleatorio.Next(2) == 0 ? "Tinta" : "Láser");
                        break;
                }

                PrioridadSolicitud prioridad = aleatorio.Next(100) < 30 ? PrioridadSolicitud.Urgente : PrioridadSolicitud.Normal;
                Solicitud s = sistema.Solicitudes.CrearSolicitud(propietario.Codigo, equipo.CodigoInterno,
                                                                 Problemas[aleatorio.Next(Problemas.Length)], prioridad);
                s.EstablecerFechaRecepcion(DateTime.Now.AddMinutes(-(n - i) * 90));
            }
        }

        private void AvanzarSolicitudes(int n)
        {
            int objetivo = Math.Max(2, n * 2 / 3);
            for (int k = 0; k < objetivo; k++)
            {
                Solicitud s;
                try
                {
                    s = sistema.Solicitudes.AtenderSiguiente((DayOfWeek)aleatorio.Next(7));
                }
                catch (ReglaNegocioException)
                {
                    break;      
                }

                try
                {
                    int etapa = aleatorio.Next(6);     
                    if (etapa >= 1)
                    {
                        sistema.Solicitudes.RegistrarDiagnostico(s.Codigo, s.Equipo.GenerarDiagnosticoSugerido(), aleatorio.Next(0, 5) * 25);
                        AgregarRepuestosAleatorios(s);
                    }
                    if (etapa >= 2)
                        sistema.Solicitudes.ActualizarEstado(s.Codigo, EstadoSolicitud.EnReparacion, "Reparación iniciada");
                    if (etapa >= 3)
                        sistema.Solicitudes.ActualizarEstado(s.Codigo, EstadoSolicitud.ListaParaEntregar, "Reparación terminada");
                    if (etapa >= 5)
                        sistema.Solicitudes.RegistrarEntrega(s.Codigo, aleatorio.Next(3, 6));
                }
                catch (ReglaNegocioException)
                {
                    
                }
            }

            foreach (Solicitud s in new List<Solicitud>(sistema.Solicitudes.ObtenerTodas()))
            {
                if (s.Estado == EstadoSolicitud.EnEspera && aleatorio.Next(100) < 10)
                    sistema.Solicitudes.ActualizarEstado(s.Codigo, EstadoSolicitud.Cancelada, "Cancelada por el cliente");
            }
        }

        private void AgregarRepuestosAleatorios(Solicitud s)
        {
            List<Repuesto> inventario = sistema.Repuestos.ObtenerTodos();
            int usos = aleatorio.Next(0, 3);
            for (int i = 0; i < usos && inventario.Count > 0; i++)
            {
                Repuesto r = inventario[aleatorio.Next(inventario.Count)];
                int cantidad = aleatorio.Next(1, 3);
                if (r.HayExistencia(cantidad))
                    s.AgregarRepuesto(r, cantidad);
            }
        }
    }
}
