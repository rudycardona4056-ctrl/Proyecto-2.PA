using System;
using System.Collections.Generic;

namespace XelaTechSoporte
{
    
    public class Reportes
    {
        private SistemaXelaTech sistema;

        public Reportes(SistemaXelaTech sistema)
        {
            this.sistema = sistema;
        }

        private static void IndicarOrden(string algoritmo, string criterio, bool asc)
        {
            Console.WriteLine("Algoritmo: " + algoritmo + " | Criterio: " + criterio + " | Orden: "
                              + (asc ? "Ascendente" : "Descendente"));
            Consola.Linea(70);
        }

        private static void ExigirDatosParaOrdenar(int cantidad)
        {
            if (cantidad < 2)
                throw new ReglaNegocioException("No hay datos suficientes para ordenar (se requieren al menos 2 registros; hay " + cantidad + ").");
        }

        private static string NombreCliente(Solicitud s)
        {
            return s.Cliente.NombreCompleto;
        }

        public void EsperaPorPrioridad()
        {
            Consola.Titulo("REPORTE 1: SOLICITUDES EN ESPERA POR PRIORIDAD");
            MostrarCola("URGENTES", PrioridadSolicitud.Urgente);
            MostrarCola("NORMALES", PrioridadSolicitud.Normal);
        }

        private void MostrarCola(string nombre, PrioridadSolicitud prioridad)
        {
            List<Solicitud> lista = sistema.Solicitudes.ObtenerEnCola(prioridad);
            Console.WriteLine();
            Console.WriteLine("Cola " + nombre + " (" + lista.Count + "):");
            if (lista.Count == 0)
            {
                Console.WriteLine("  (sin solicitudes en espera)");
                return;
            }
            Console.WriteLine("  " + Consola.Ajustar("No.", 5) + Consola.Ajustar("Código", 11) + Consola.Ajustar("Cliente", 24)
                              + Consola.Ajustar("Equipo", 10) + "Recepción");
            int numero = 1;
            foreach (Solicitud s in lista)
            {
                Console.WriteLine("  " + Consola.Ajustar(numero.ToString(), 5) + Consola.Ajustar(s.Codigo, 11)
                                  + Consola.Ajustar(NombreCliente(s), 24) + Consola.Ajustar(s.Equipo.CodigoInterno, 10)
                                  + Consola.Fecha(s.FechaRecepcion));
                numero++;
            }
        }

        public void ActivasFinalizadasCanceladas()
        {
            Consola.Titulo("REPORTE 2: SOLICITUDES ACTIVAS, FINALIZADAS Y CANCELADAS");
            List<Solicitud> todas = sistema.Solicitudes.ObtenerTodas();
            if (todas.Count == 0)
            {
                Console.WriteLine("No hay solicitudes registradas.");
                return;
            }

            int activas = 0, finalizadas = 0, canceladas = 0;
            Console.WriteLine("\nACTIVAS:");
            foreach (Solicitud s in todas)
            {
                if (s.EstaActiva())
                {
                    activas++;
                    FilaSolicitudEstado(s);
                }
            }
            if (activas == 0) Console.WriteLine("  (ninguna)");

            Console.WriteLine("\nFINALIZADAS (entregadas):");
            foreach (Solicitud s in todas)
            {
                if (s.EstaFinalizada())
                {
                    finalizadas++;
                    FilaSolicitudEstado(s);
                }
            }
            if (finalizadas == 0) Console.WriteLine("  (ninguna)");

            Console.WriteLine("\nCANCELADAS:");
            foreach (Solicitud s in todas)
            {
                if (s.Estado == EstadoSolicitud.Cancelada)
                {
                    canceladas++;
                    FilaSolicitudEstado(s);
                }
            }
            if (canceladas == 0) Console.WriteLine("  (ninguna)");

            Consola.Linea(60);
            Console.WriteLine("Activas: " + activas + " | Finalizadas: " + finalizadas + " | Canceladas: " + canceladas);
        }

        private static void FilaSolicitudEstado(Solicitud s)
        {
            Console.WriteLine("  " + Consola.Ajustar(s.Codigo, 11) + Consola.Ajustar(NombreCliente(s), 24)
                              + Consola.Ajustar(s.Estado.ToString(), 19) + Consola.Dinero(s.CalcularTotal()));
        }

        public void TecnicosDisponiblesYCompletos()
        {
            Consola.Titulo("REPORTE 3: TÉCNICOS DISPONIBLES Y CON CARGA COMPLETA");
            List<Tecnico> lista = sistema.Tecnicos.ObtenerTodos();
            if (lista.Count == 0)
            {
                Console.WriteLine("No hay técnicos registrados.");
                return;
            }

            Console.WriteLine("\nDISPONIBLES (marcados como disponibles y con menos de " + Tecnico.LIMITE_TRABAJOS + " trabajos):");
            int disponibles = 0;
            foreach (Tecnico t in lista)
            {
                if (t.Disponible && !t.TieneCargaCompleta)
                {
                    disponibles++;
                    Console.WriteLine("  " + Consola.Ajustar(t.Codigo, 11) + Consola.Ajustar(t.NombreCompleto, 22)
                                      + Consola.Ajustar(t.Especialidad.ToString(), 13) + t.TrabajosActivos + "/" + Tecnico.LIMITE_TRABAJOS);
                }
            }
            if (disponibles == 0) Console.WriteLine("  (ninguno)");

            Console.WriteLine("\nCON CARGA COMPLETA (" + Tecnico.LIMITE_TRABAJOS + " trabajos activos):");
            int completos = 0;
            foreach (Tecnico t in lista)
            {
                if (t.TieneCargaCompleta)
                {
                    completos++;
                    Console.WriteLine("  " + Consola.Ajustar(t.Codigo, 11) + Consola.Ajustar(t.NombreCompleto, 22)
                                      + Consola.Ajustar(t.Especialidad.ToString(), 13) + t.TrabajosActivos + "/" + Tecnico.LIMITE_TRABAJOS);
                }
            }
            if (completos == 0) Console.WriteLine("  (ninguno)");
        }

        public void TecnicoConMasFinalizados()
        {
            Consola.Titulo("REPORTE 4: TÉCNICO CON MÁS SERVICIOS FINALIZADOS");
            List<Tecnico> lista = sistema.Tecnicos.ObtenerTodos();
            int mayor = 0;
            foreach (Tecnico t in lista)
            {
                if (t.ServiciosFinalizados > mayor)
                    mayor = t.ServiciosFinalizados;
            }
            if (mayor == 0)
            {
                Console.WriteLine("Todavía no hay servicios finalizados.");
                return;
            }
            foreach (Tecnico t in lista)           
            {
                if (t.ServiciosFinalizados == mayor)
                    Console.WriteLine(t.Codigo + " | " + t.NombreCompleto + " | Finalizados: " + t.ServiciosFinalizados
                                      + " | Promedio: " + t.CalcularPromedio().ToString("0.0"));
            }
        }

        public void EquiposPorTipo()
        {
            Consola.Titulo("REPORTE 5: EQUIPOS ATENDIDOS POR TIPO");
            int[] conteo = new int[3];                  
            string[] nombres = { "Computadora portátil", "Computadora de escritorio", "Impresora" };
            int total = 0;
            foreach (Solicitud s in sistema.Solicitudes.ObtenerTodas())
            {
                if (s.Estado != EstadoSolicitud.Recibida && s.Estado != EstadoSolicitud.EnEspera
                    && s.Estado != EstadoSolicitud.Cancelada)
                {
                    conteo[(int)s.Equipo.Tipo]++;
                    total++;
                }
            }
            if (total == 0)
            {
                Console.WriteLine("Todavía no se ha atendido ningún equipo.");
                return;
            }
            for (int i = 0; i < conteo.Length; i++)
                Console.WriteLine(Consola.Ajustar(nombres[i], 28) + conteo[i].ToString().PadLeft(3) + "  " + new string('#', conteo[i]));
            Consola.Linea(40);
            Console.WriteLine("Total de equipos atendidos: " + total);
        }

        public void SolicitudesPorCosto(bool asc)
        {
            Consola.Titulo("REPORTE 6: SOLICITUDES ORDENADAS POR COSTO TOTAL");
            ExigirDatosParaOrdenar(sistema.Solicitudes.ObtenerTodas().Count);
            List<Solicitud> copia = new List<Solicitud>(sistema.Solicitudes.ObtenerTodas());   
            Ordenador.BubbleSort(copia, s => s.CalcularTotal(), asc);

            IndicarOrden("Bubble Sort", "Costo total", asc);
            Console.WriteLine(Consola.Ajustar("No.", 5) + Consola.Ajustar("Código", 11) + Consola.Ajustar("Cliente", 22)
                              + Consola.Ajustar("Mano obra", 14) + Consola.Ajustar("Repuestos", 14) + "Total");
            int n = 1;
            foreach (Solicitud s in copia)
            {
                Console.WriteLine(Consola.Ajustar(n.ToString(), 5) + Consola.Ajustar(s.Codigo, 11) + Consola.Ajustar(NombreCliente(s), 22)
                                  + Consola.Ajustar(Consola.Dinero(s.CostoManoObra), 14) + Consola.Ajustar(Consola.Dinero(s.CalcularCostoRepuestos()), 14)
                                  + Consola.Dinero(s.CalcularTotal()));
                n++;
            }
            Consola.Linea(70);
            Console.WriteLine("Solicitudes ordenadas: " + copia.Count);
        }

        public void SolicitudesPorFechaOTiempo(bool porFecha, bool asc)
        {
            Consola.Titulo("REPORTE 7: SOLICITUDES ORDENADAS POR FECHA O TIEMPO ESTIMADO");
            ExigirDatosParaOrdenar(sistema.Solicitudes.ObtenerTodas().Count);
            List<Solicitud> copia = new List<Solicitud>(sistema.Solicitudes.ObtenerTodas());
            if (porFecha)
                Ordenador.QuickSort(copia, s => s.FechaRecepcion, asc);
            else
                Ordenador.QuickSort(copia, s => s.TiempoEstimadoHoras, asc);

            IndicarOrden("Quick Sort", porFecha ? "Fecha de recepción" : "Tiempo estimado", asc);
            Console.WriteLine(Consola.Ajustar("No.", 5) + Consola.Ajustar("Código", 11) + Consola.Ajustar("Recepción", 18)
                              + Consola.Ajustar("Tiempo est.", 13) + Consola.Ajustar("Prioridad", 11) + "Estado");
            int n = 1;
            foreach (Solicitud s in copia)
            {
                Console.WriteLine(Consola.Ajustar(n.ToString(), 5) + Consola.Ajustar(s.Codigo, 11) + Consola.Ajustar(Consola.Fecha(s.FechaRecepcion), 18)
                                  + Consola.Ajustar(s.TiempoEstimadoHoras.ToString("0.0") + " h", 13) + Consola.Ajustar(s.Prioridad.ToString(), 11)
                                  + s.Estado);
                n++;
            }
            Consola.Linea(70);
            Console.WriteLine("Solicitudes ordenadas: " + copia.Count);
        }

        public void RepuestosPorExistencia(bool asc)
        {
            Consola.Titulo("REPORTE 8: REPUESTOS ORDENADOS POR EXISTENCIA");
            ExigirDatosParaOrdenar(sistema.Repuestos.ObtenerTodos().Count);
            List<Repuesto> copia = new List<Repuesto>(sistema.Repuestos.ObtenerTodos());
            Ordenador.ShellSort(copia, r => r.Existencia, asc);

            IndicarOrden("Shell Sort", "Existencia disponible", asc);
            Console.WriteLine(Consola.Ajustar("No.", 5) + Consola.Ajustar("Código", 11) + Consola.Ajustar("Repuesto", 26)
                              + Consola.Ajustar("Categoría", 16) + Consola.Ajustar("Existencia", 12) + "Mínima");
            int n = 1;
            foreach (Repuesto r in copia)
            {
                Console.WriteLine(Consola.Ajustar(n.ToString(), 5) + Consola.Ajustar(r.Codigo, 11) + Consola.Ajustar(r.Nombre, 26)
                                  + Consola.Ajustar(r.Categoria, 16) + Consola.Ajustar(r.Existencia.ToString(), 12) + r.ExistenciaMinima);
                n++;
            }
            Consola.Linea(70);
            Console.WriteLine("Repuestos ordenados: " + copia.Count);
        }

        public void TecnicosPorFinalizados(bool asc)
        {
            Consola.Titulo("TÉCNICOS ORDENADOS POR SERVICIOS FINALIZADOS");
            ExigirDatosParaOrdenar(sistema.Tecnicos.ObtenerTodos().Count);
            List<Tecnico> copia = new List<Tecnico>(sistema.Tecnicos.ObtenerTodos());
            Ordenador.SelectionSort(copia, t => t.ServiciosFinalizados, asc);

            IndicarOrden("Selection Sort", "Servicios finalizados", asc);
            Console.WriteLine(Consola.Ajustar("No.", 5) + Consola.Ajustar("Código", 11) + Consola.Ajustar("Técnico", 22)
                              + Consola.Ajustar("Especialidad", 14) + Consola.Ajustar("Finalizados", 13) + "Promedio");
            int n = 1;
            foreach (Tecnico t in copia)
            {
                Console.WriteLine(Consola.Ajustar(n.ToString(), 5) + Consola.Ajustar(t.Codigo, 11) + Consola.Ajustar(t.NombreCompleto, 22)
                                  + Consola.Ajustar(t.Especialidad.ToString(), 14) + Consola.Ajustar(t.ServiciosFinalizados.ToString(), 13)
                                  + t.CalcularPromedio().ToString("0.0"));
                n++;
            }
            Consola.Linea(70);
            Console.WriteLine("Técnicos ordenados: " + copia.Count);
        }

        public void RepuestosMinimosOAgotados()
        {
            Consola.Titulo("REPORTE 9: REPUESTOS CON EXISTENCIA MÍNIMA O AGOTADOS");
            int encontrados = 0;
            foreach (Repuesto r in sistema.Repuestos.ObtenerTodos())
            {
                if (r.EstaEnMinimo())                 
                {
                    if (encontrados == 0)
                        Console.WriteLine(Consola.Ajustar("Código", 11) + Consola.Ajustar("Repuesto", 26) + Consola.Ajustar("Existencia", 12)
                                          + Consola.Ajustar("Mínima", 9) + "Situación");
                    encontrados++;
                    Console.WriteLine(Consola.Ajustar(r.Codigo, 11) + Consola.Ajustar(r.Nombre, 26) + Consola.Ajustar(r.Existencia.ToString(), 12)
                                      + Consola.Ajustar(r.ExistenciaMinima.ToString(), 9) + r.ObtenerSituacion());
                }
            }
            if (encontrados == 0)
                Console.WriteLine("Ningún repuesto está en existencia mínima o agotado.");
            else
                Console.WriteLine("\nTotal de repuestos en alerta: " + encontrados);
        }

        public void IngresosTotales()
        {
            Consola.Titulo("REPORTE 10: INGRESOS TOTALES POR SERVICIOS FINALIZADOS");
            decimal suma = 0m;
            int cantidad = 0;
            foreach (Solicitud s in sistema.Solicitudes.ObtenerTodas())
            {
                if (s.EstaFinalizada())
                {
                    if (cantidad == 0)
                        Console.WriteLine(Consola.Ajustar("Código", 11) + Consola.Ajustar("Cliente", 24) + "Total");
                    cantidad++;
                    suma += s.CalcularTotal();
                    Console.WriteLine(Consola.Ajustar(s.Codigo, 11) + Consola.Ajustar(NombreCliente(s), 24) + Consola.Dinero(s.CalcularTotal()));
                }
            }
            if (cantidad == 0)
            {
                Console.WriteLine("Todavía no hay servicios finalizados.");
                return;
            }
            Consola.Linea(50);
            Console.WriteLine("Servicios finalizados: " + cantidad);
            Console.WriteLine("INGRESOS TOTALES:      " + Consola.Dinero(suma));
            Console.WriteLine("Promedio por servicio: " + Consola.Dinero(suma / cantidad));
        }

        public void ResultadosBusquedas()
        {
            Consola.Titulo("REPORTE 11: RESULTADOS DE LAS BÚSQUEDAS");
            string[] criterios = { ResultadoBusqueda.CRITERIO_CLIENTE, ResultadoBusqueda.CRITERIO_ESTADO,
                                   ResultadoBusqueda.CRITERIO_MARCA, ResultadoBusqueda.CRITERIO_CODIGO };
            foreach (string criterio in criterios)
            {
                ResultadoBusqueda ultimo = null;
                foreach (ResultadoBusqueda b in sistema.Busquedas)      
                {
                    if (b.Criterio == criterio)
                        ultimo = b;
                }
                Console.WriteLine();
                if (ultimo == null)
                {
                    Console.WriteLine(criterio + ": (aún no se ha ejecutado)");
                    continue;
                }
                Console.WriteLine(ultimo.TipoBusqueda + " - " + criterio + " | Valor: '" + ultimo.ValorBuscado + "' | "
                                  + Consola.Fecha(ultimo.FechaHora));
                if (ultimo.Coincidencias.Count == 0)
                    Console.WriteLine("  Sin coincidencias.");
                else
                    foreach (string linea in ultimo.Coincidencias)
                        Console.WriteLine("  - " + linea);
            }
        }

        public void MatrizSemanal()
        {
            sistema.Carga.Mostrar(sistema.Tecnicos.ObtenerTodos());
        }
    }
}
