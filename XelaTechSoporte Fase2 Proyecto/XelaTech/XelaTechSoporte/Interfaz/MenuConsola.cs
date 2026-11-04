using System;
using System.Collections.Generic;
using System.IO;

namespace XelaTechSoporte
{
   
    public class MenuConsola
    {
        private SistemaXelaTech sistema;
        private Reportes reportes;
        private GeneradorDatos generador;

        public MenuConsola(SistemaXelaTech sistema)
        {
            this.sistema = sistema;
            this.reportes = new Reportes(sistema);
            this.generador = new GeneradorDatos(sistema);
        }

        public void Iniciar()
        {
            bool salir = false;
            while (!salir)
            {
                Console.Clear();
                Console.WriteLine();
                Console.WriteLine(new string('=', 44));
                Console.WriteLine("              XELATECH SOPORTE");
                Console.WriteLine(new string('=', 44));
                Console.WriteLine(" 1. Gestión de clientes");
                Console.WriteLine(" 2. Gestión de técnicos");
                Console.WriteLine(" 3. Gestión de equipos");
                Console.WriteLine(" 4. Gestión de solicitudes");
                Console.WriteLine(" 5. Gestión de repuestos");
                Console.WriteLine(" 6. Atención de colas");
                Console.WriteLine(" 7. Ordenamientos");
                Console.WriteLine(" 8. Búsquedas");
                Console.WriteLine(" 9. Reportes");
                Console.WriteLine("10. Generar datos de prueba");
                Console.WriteLine("11. Salir");
                Console.WriteLine(new string('-', 44));

                int opcion = Consola.LeerEntero("Seleccione una opción (1-11): ", 1, 11);
                try
                {
                    switch (opcion)
                    {
                        case 1: MenuClientes(); break;
                        case 2: MenuTecnicos(); break;
                        case 3: MenuEquipos(); break;
                        case 4: MenuSolicitudes(); break;
                        case 5: MenuRepuestos(); break;
                        case 6: MenuColas(); break;
                        case 7: MenuOrdenamientos(); break;
                        case 8: MenuBusquedas(); break;
                        case 9: MenuReportes(); break;
                        case 10: MenuDatosPrueba(); break;
                        case 11: salir = Consola.Confirmar("¿Desea salir del programa?"); break;
                    }
                }
                catch (EndOfStreamException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error inesperado: " + ex.Message);
                    Consola.Pausa();
                }
            }
            Console.WriteLine("\nGracias por usar XelaTech Soporte. ¡Hasta luego!");
        }

        private void Ejecutar(Action operacion)
        {
            try
            {
                operacion();
            }
            catch (ReglaNegocioException ex)
            {
                Console.WriteLine("\n  [ERROR] " + ex.Message);
            }
            catch (EndOfStreamException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine("\n  [ERROR INESPERADO] " + ex.Message);
            }
            Consola.Pausa();
        }

        private void MenuClientes()
        {
            int opcion;
            do
            {
                Consola.Titulo("GESTIÓN DE CLIENTES");
                Console.WriteLine(" 1. Registrar cliente");
                Console.WriteLine(" 2. Consultar clientes");
                Console.WriteLine(" 3. Actualizar cliente");
                Console.WriteLine(" 4. Volver");
                opcion = Consola.LeerEntero("Seleccione una opción (1-4): ", 1, 4);
                switch (opcion)
                {
                    case 1: Ejecutar(RegistrarCliente); break;
                    case 2: Ejecutar(ConsultarClientes); break;
                    case 3: Ejecutar(ActualizarCliente); break;
                }
            } while (opcion != 4);
        }

        private void RegistrarCliente()
        {
            string nombre = Consola.LeerTexto("Nombre completo: ");
            string telefono = Consola.LeerTexto("Teléfono (8 dígitos): ");
            string correo = Consola.LeerTexto("Correo electrónico: ");
            int tipo = Consola.ElegirOpcion("Tipo de cliente:", Enum.GetNames(typeof(TipoCliente)));
            Cliente c = sistema.Clientes.Registrar(nombre, telefono, correo, (TipoCliente)tipo);
            Console.WriteLine("\n  Cliente registrado: " + c.ObtenerResumen());
        }

        private void ConsultarClientes()
        {
            List<Cliente> lista = sistema.Clientes.ObtenerTodos();
            if (lista.Count == 0)
            {
                Console.WriteLine("\n  No hay clientes registrados.");
                return;
            }
            Console.WriteLine();
            Console.WriteLine(Consola.Ajustar("Código", 10) + Consola.Ajustar("Nombre", 26) + Consola.Ajustar("Teléfono", 10)
                              + Consola.Ajustar("Tipo", 13) + "Solicitudes");
            Consola.Linea(70);
            foreach (Cliente c in lista)
            {
                Console.WriteLine(Consola.Ajustar(c.Codigo, 10) + Consola.Ajustar(c.NombreCompleto, 26) + Consola.Ajustar(c.Telefono, 10)
                                  + Consola.Ajustar(c.Tipo.ToString(), 13) + c.CantidadSolicitudes);
            }
            string codigo = Consola.LeerTexto("\nCódigo para ver el detalle (Enter para omitir): ");
            if (codigo.Length > 0)
            {
                Cliente detalle = sistema.Clientes.ObtenerPorCodigo(codigo);
                if (detalle == null)
                    throw new ReglaNegocioException("No existe un cliente con el código " + codigo + ".");
                Console.WriteLine("\n  " + detalle.ObtenerResumen());
            }
        }

        private void ActualizarCliente()
        {
            string codigo = Consola.LeerTexto("Código del cliente: ");
            Cliente c = sistema.Clientes.ObtenerPorCodigo(codigo);
            if (c == null)
                throw new ReglaNegocioException("No existe un cliente con el código " + codigo + ".");
            Console.WriteLine("  Actual: " + c.ObtenerResumen());
            string telefono = Consola.LeerTexto("Nuevo teléfono (8 dígitos): ");
            string correo = Consola.LeerTexto("Nuevo correo electrónico: ");
            sistema.Clientes.Actualizar(codigo, telefono, correo);
            Console.WriteLine("\n  Cliente actualizado: " + c.ObtenerResumen());
        }

        private void MenuTecnicos()
        {
            int opcion;
            do
            {
                Consola.Titulo("GESTIÓN DE TÉCNICOS");
                Console.WriteLine(" 1. Registrar técnico");
                Console.WriteLine(" 2. Consultar técnicos");
                Console.WriteLine(" 3. Consultar por especialidad");
                Console.WriteLine(" 4. Cambiar disponibilidad");
                Console.WriteLine(" 5. Distribución de calificaciones");
                Console.WriteLine(" 6. Volver");
                opcion = Consola.LeerEntero("Seleccione una opción (1-6): ", 1, 6);
                switch (opcion)
                {
                    case 1: Ejecutar(RegistrarTecnico); break;
                    case 2: Ejecutar(ConsultarTecnicos); break;
                    case 3: Ejecutar(ConsultarPorEspecialidad); break;
                    case 4: Ejecutar(CambiarDisponibilidad); break;
                    case 5: Ejecutar(MostrarDistribucionCalificaciones); break;
                }
            } while (opcion != 6);
        }

        private void RegistrarTecnico()
        {
            string nombre = Consola.LeerTexto("Nombre completo: ");
            int esp = Consola.ElegirOpcion("Especialidad:", Enum.GetNames(typeof(Especialidad)));
            Tecnico t = sistema.Tecnicos.Registrar(nombre, (Especialidad)esp);
            Console.WriteLine("\n  Técnico registrado: " + t.ObtenerResumen());
        }

        private void MostrarTablaTecnicos(List<Tecnico> lista)
        {
            Console.WriteLine();
            Console.WriteLine(Consola.Ajustar("Código", 10) + Consola.Ajustar("Nombre", 22) + Consola.Ajustar("Especialidad", 13)
                              + Consola.Ajustar("Disponible", 11) + Consola.Ajustar("Activos", 8) + Consola.Ajustar("Finaliz.", 9) + "Promedio");
            Consola.Linea(80);
            foreach (Tecnico t in lista)
            {
                Console.WriteLine(Consola.Ajustar(t.Codigo, 10) + Consola.Ajustar(t.NombreCompleto, 22) + Consola.Ajustar(t.Especialidad.ToString(), 13)
                                  + Consola.Ajustar(t.Disponible ? "Sí" : "No", 11)
                                  + Consola.Ajustar(t.TrabajosActivos + "/" + Tecnico.LIMITE_TRABAJOS, 8)
                                  + Consola.Ajustar(t.ServiciosFinalizados.ToString(), 9) + t.CalcularPromedio().ToString("0.0"));
            }
        }

        private void ConsultarTecnicos()
        {
            List<Tecnico> lista = sistema.Tecnicos.ObtenerTodos();
            if (lista.Count == 0)
            {
                Console.WriteLine("\n  No hay técnicos registrados.");
                return;
            }
            MostrarTablaTecnicos(lista);
        }

        private void ConsultarPorEspecialidad()
        {
            int esp = Consola.ElegirOpcion("Especialidad:", Enum.GetNames(typeof(Especialidad)));
            Especialidad elegida = (Especialidad)esp;
            List<Tecnico> coincidencias = new List<Tecnico>();
            foreach (Tecnico t in sistema.Tecnicos.ObtenerTodos())
            {
                if (t.Especialidad == elegida)
                    coincidencias.Add(t);
            }
            if (coincidencias.Count == 0)
            {
                Console.WriteLine("\n  No hay técnicos con la especialidad " + elegida + ".");
                return;
            }
            MostrarTablaTecnicos(coincidencias);
        }

        private void CambiarDisponibilidad()
        {
            string codigo = Consola.LeerTexto("Código del técnico: ");
            Tecnico t = sistema.Tecnicos.ObtenerPorCodigo(codigo);
            if (t == null)
                throw new ReglaNegocioException("No existe un técnico con el código " + codigo + ".");
            bool disponible = Consola.Confirmar("¿Marcar a " + t.NombreCompleto + " como DISPONIBLE?");
            t.CambiarDisponibilidad(disponible);
            Console.WriteLine("\n  " + t.ObtenerResumen());
        }

        private void MostrarDistribucionCalificaciones()
        {
            int[] distribucion = sistema.Tecnicos.ObtenerDistribucion();
            int total = 0;
            for (int i = 0; i < distribucion.Length; i++)
                total += distribucion[i];
            Console.WriteLine("\n  Distribución de calificaciones recibidas:");
            for (int i = 0; i < distribucion.Length; i++)
                Console.WriteLine("  " + (i + 1) + " estrella(s): " + distribucion[i].ToString().PadLeft(3) + "  " + new string('#', distribucion[i]));
            Console.WriteLine("  Total de calificaciones: " + total);
        }

        private void MenuEquipos()
        {
            int opcion;
            do
            {
                Consola.Titulo("GESTIÓN DE EQUIPOS");
                Console.WriteLine(" 1. Registrar computadora portátil");
                Console.WriteLine(" 2. Registrar computadora de escritorio");
                Console.WriteLine(" 3. Registrar impresora");
                Console.WriteLine(" 4. Consultar equipos");
                Console.WriteLine(" 5. Volver");
                opcion = Consola.LeerEntero("Seleccione una opción (1-5): ", 1, 5);
                switch (opcion)
                {
                    case 1: Ejecutar(RegistrarPortatil); break;
                    case 2: Ejecutar(RegistrarEscritorio); break;
                    case 3: Ejecutar(RegistrarImpresora); break;
                    case 4: Ejecutar(ConsultarEquipos); break;
                }
            } while (opcion != 5);
        }

        private Cliente PedirPropietario()
        {
            string codigo = Consola.LeerTexto("Código del cliente propietario: ");
            Cliente propietario = sistema.Clientes.ObtenerPorCodigo(codigo);
            if (propietario == null)
                throw new ReglaNegocioException("El propietario debe ser un cliente existente (código " + codigo + " no encontrado).");
            return propietario;
        }

        private void RegistrarPortatil()
        {
            Cliente propietario = PedirPropietario();
            string serie = Consola.LeerTexto("Número de serie: ");
            string marca = Consola.LeerTexto("Marca: ");
            string modelo = Consola.LeerTexto("Modelo: ");
            string estado = Consola.LeerTexto("Estado físico recibido: ");
            string accesorios = Consola.LeerTexto("Accesorios entregados (escriba 'Ninguno' si no hay): ");
            bool bateria = Consola.Confirmar("¿Requiere revisión de batería?");
            Equipo e = sistema.Equipos.RegistrarPortatil(serie, marca, modelo, propietario, estado, accesorios, bateria);
            Console.WriteLine("\n  Equipo registrado: " + e);
        }

        private void RegistrarEscritorio()
        {
            Cliente propietario = PedirPropietario();
            string serie = Consola.LeerTexto("Número de serie: ");
            string marca = Consola.LeerTexto("Marca: ");
            string modelo = Consola.LeerTexto("Modelo: ");
            string estado = Consola.LeerTexto("Estado físico recibido: ");
            string accesorios = Consola.LeerTexto("Accesorios entregados (escriba 'Ninguno' si no hay): ");
            bool monitor = Consola.Confirmar("¿Incluye monitor?");
            Equipo e = sistema.Equipos.RegistrarEscritorio(serie, marca, modelo, propietario, estado, accesorios, monitor);
            Console.WriteLine("\n  Equipo registrado: " + e);
        }

        private void RegistrarImpresora()
        {
            Cliente propietario = PedirPropietario();
            string serie = Consola.LeerTexto("Número de serie: ");
            string marca = Consola.LeerTexto("Marca: ");
            string modelo = Consola.LeerTexto("Modelo: ");
            string estado = Consola.LeerTexto("Estado físico recibido: ");
            string accesorios = Consola.LeerTexto("Accesorios entregados (escriba 'Ninguno' si no hay): ");
            int tipo = Consola.ElegirOpcion("Tipo de impresión:", new string[] { "Tinta", "Láser" });
            Equipo e = sistema.Equipos.RegistrarImpresora(serie, marca, modelo, propietario, estado, accesorios, tipo == 0 ? "Tinta" : "Láser");
            Console.WriteLine("\n  Equipo registrado: " + e);
        }

        private void ConsultarEquipos()
        {
            List<Equipo> lista = sistema.Equipos.ObtenerTodos();
            if (lista.Count == 0)
            {
                Console.WriteLine("\n  No hay equipos registrados.");
                return;
            }
            MostrarTablaEquipos(lista);
        }

        private void MostrarTablaEquipos(List<Equipo> lista)
        {
            Console.WriteLine();
            Console.WriteLine(Consola.Ajustar("Código", 9) + Consola.Ajustar("Tipo", 27) + Consola.Ajustar("Marca", 10)
                              + Consola.Ajustar("Modelo", 14) + Consola.Ajustar("Serie", 10) + "Propietario");
            Consola.Linea(85);
            foreach (Equipo e in lista)
            {
                Console.WriteLine(Consola.Ajustar(e.CodigoInterno, 9) + Consola.Ajustar(e.NombreTipo(), 27) + Consola.Ajustar(e.Marca, 10)
                                  + Consola.Ajustar(e.Modelo, 14) + Consola.Ajustar(e.NumeroSerie, 10) + e.Propietario.NombreCompleto);
            }
        }

        private void MenuSolicitudes()
        {
            int opcion;
            do
            {
                Consola.Titulo("GESTIÓN DE SOLICITUDES");
                Console.WriteLine(" 1. Crear solicitud (normal o urgente)");
                Console.WriteLine(" 2. Registrar diagnóstico");
                Console.WriteLine(" 3. Registrar repuestos");
                Console.WriteLine(" 4. Actualizar estado (incluye cancelar)");
                Console.WriteLine(" 5. Registrar entrega");
                Console.WriteLine(" 6. Consultar historial de estados");
                Console.WriteLine(" 7. Deshacer último cambio");
                Console.WriteLine(" 8. Consultar solicitudes");
                Console.WriteLine(" 9. Volver");
                opcion = Consola.LeerEntero("Seleccione una opción (1-9): ", 1, 9);
                switch (opcion)
                {
                    case 1: Ejecutar(CrearSolicitud); break;
                    case 2: Ejecutar(RegistrarDiagnostico); break;
                    case 3: Ejecutar(RegistrarRepuestos); break;
                    case 4: Ejecutar(ActualizarEstado); break;
                    case 5: Ejecutar(RegistrarEntrega); break;
                    case 6: Ejecutar(ConsultarHistorial); break;
                    case 7: Ejecutar(DeshacerCambio); break;
                    case 8: Ejecutar(ConsultarSolicitudes); break;
                }
            } while (opcion != 9);
        }

        private void CrearSolicitud()
        {
            string codCliente = Consola.LeerTexto("Código del cliente: ");
            string codEquipo = Consola.LeerTexto("Código del equipo: ");
            string descripcion = Consola.LeerTexto("Descripción del problema: ");
            int prio = Consola.ElegirOpcion("Prioridad:", new string[] { "Normal", "Urgente" });
            Solicitud s = sistema.Solicitudes.CrearSolicitud(codCliente, codEquipo, descripcion, (PrioridadSolicitud)prio);
            Console.WriteLine("\n  Solicitud creada: " + s.Codigo + " | Prioridad: " + s.Prioridad + " | Estado: " + s.Estado);
            Console.WriteLine("  Tiempo estimado de atención: " + s.TiempoEstimadoHoras.ToString("0.0") + " h");
            Console.WriteLine("  Colocada en la cola " + (s.Prioridad == PrioridadSolicitud.Urgente ? "URGENTE" : "NORMAL") + ".");
        }

        private void RegistrarDiagnostico()
        {
            string codigo = Consola.LeerTexto("Código de la solicitud: ");
            Solicitud s = sistema.Solicitudes.ObtenerPorCodigo(codigo);
            if (s == null)
                throw new ReglaNegocioException("No existe una solicitud con el código " + codigo + ".");
            string sugerido = s.Equipo.GenerarDiagnosticoSugerido();
            Console.WriteLine("  Diagnóstico sugerido para este tipo de equipo: " + sugerido);
            string texto = Consola.LeerTexto("Diagnóstico (Enter para usar el sugerido): ");
            if (texto.Length == 0)
                texto = sugerido;
            decimal adicional = Consola.LeerDecimal("Costo adicional de mano de obra (0 si no hay): Q ");
            sistema.Solicitudes.RegistrarDiagnostico(codigo, texto, adicional);
            Console.WriteLine("\n  Diagnóstico registrado. Mano de obra: " + Consola.Dinero(s.CostoManoObra)
                              + " (revisión " + Consola.Dinero(s.Equipo.CalcularCostoRevision()) + " + adicional " + Consola.Dinero(adicional) + ")");
        }

        private void RegistrarRepuestos()
        {
            string codigo = Consola.LeerTexto("Código de la solicitud: ");
            string codRepuesto = Consola.LeerTexto("Código del repuesto: ");
            int cantidad = Consola.LeerEntero("Cantidad: ", 1, int.MaxValue);
            Repuesto r = sistema.Solicitudes.AgregarRepuesto(codigo, codRepuesto, cantidad);
            Console.WriteLine("\n  Repuesto registrado: " + cantidad + " x " + r.Nombre + " a " + Consola.Dinero(r.PrecioVenta) + " c/u.");
            if (r.EstaAgotado())
                Console.WriteLine("  [ALERTA] El repuesto '" + r.Nombre + "' quedó AGOTADO.");
            else if (r.EstaEnMinimo())
                Console.WriteLine("  [ALERTA] El repuesto '" + r.Nombre + "' alcanzó la existencia mínima (" + r.Existencia + ").");
        }

        private void ActualizarEstado()
        {
            string codigo = Consola.LeerTexto("Código de la solicitud: ");
            Solicitud s = sistema.Solicitudes.ObtenerPorCodigo(codigo);
            if (s == null)
                throw new ReglaNegocioException("No existe una solicitud con el código " + codigo + ".");
            Console.WriteLine("  Estado actual: " + s.Estado);
            int nuevo = Consola.ElegirOpcion("Nuevo estado:", Enum.GetNames(typeof(EstadoSolicitud)));
            string obs = Consola.LeerTexto("Observación (opcional): ");
            sistema.Solicitudes.ActualizarEstado(codigo, (EstadoSolicitud)nuevo, obs);
            Console.WriteLine("\n  Estado actualizado: " + s.Codigo + " ahora está " + s.Estado + ".");
        }

        private void RegistrarEntrega()
        {
            string codigo = Consola.LeerTexto("Código de la solicitud: ");
            int calificacion = Consola.LeerEntero("Calificación del cliente (1 a 5): ", 1, 5);
            Solicitud s = sistema.Solicitudes.RegistrarEntrega(codigo, calificacion);
            Console.WriteLine("\n  Equipo entregado. Resumen de la solicitud " + s.Codigo + ":");
            Console.WriteLine("    Mano de obra: " + Consola.Dinero(s.CostoManoObra));
            Console.WriteLine("    Repuestos:    " + Consola.Dinero(s.CalcularCostoRepuestos()));
            Console.WriteLine("    TOTAL:        " + Consola.Dinero(s.CalcularTotal()));
        }

        private void ConsultarHistorial()
        {
            string codigo = Consola.LeerTexto("Código de la solicitud: ");
            List<RegistroEstado> historial = sistema.Solicitudes.ConsultarHistorial(codigo);
            Solicitud s = sistema.Solicitudes.ObtenerPorCodigo(codigo);
            Console.WriteLine("\n  Estado actual de " + s.Codigo + ": " + s.Estado);
            if (historial.Count == 0)
            {
                Console.WriteLine("  El historial está vacío.");
                return;
            }
            Console.WriteLine("  Historial (del más reciente al más antiguo):");
            foreach (RegistroEstado r in historial)
                Console.WriteLine("    " + Consola.Fecha(r.FechaHora) + " | estado anterior: " + Consola.Ajustar(r.EstadoAnterior.ToString(), 18) + r.Observacion);
        }

        private void DeshacerCambio()
        {
            string codigo = Consola.LeerTexto("Código de la solicitud: ");
            if (!Consola.Confirmar("¿Deshacer el último cambio de estado de " + codigo + "?"))
                return;
            Solicitud s = sistema.Solicitudes.DeshacerUltimoCambio(codigo);
            Console.WriteLine("\n  Cambio deshecho. La solicitud " + s.Codigo + " volvió al estado " + s.Estado + ".");
        }

        private void ConsultarSolicitudes()
        {
            List<Solicitud> lista = sistema.Solicitudes.ObtenerTodas();
            if (lista.Count == 0)
            {
                Console.WriteLine("\n  No hay solicitudes registradas.");
                return;
            }
            Console.WriteLine();
            Console.WriteLine(Consola.Ajustar("Código", 10) + Consola.Ajustar("Cliente", 20) + Consola.Ajustar("Equipo", 9)
                              + Consola.Ajustar("Prioridad", 10) + Consola.Ajustar("Estado", 19) + Consola.Ajustar("Técnico", 10) + "Total");
            Consola.Linea(90);
            foreach (Solicitud s in lista)
            {
                string tecnico = s.Tecnico == null ? "-" : s.Tecnico.Codigo;
                Console.WriteLine(Consola.Ajustar(s.Codigo, 10) + Consola.Ajustar(s.Cliente.NombreCompleto, 20) + Consola.Ajustar(s.Equipo.CodigoInterno, 9)
                                  + Consola.Ajustar(s.Prioridad.ToString(), 10) + Consola.Ajustar(s.Estado.ToString(), 19)
                                  + Consola.Ajustar(tecnico, 10) + Consola.Dinero(s.CalcularTotal()));
            }
            string codigo = Consola.LeerTexto("\nCódigo para ver el detalle (Enter para omitir): ");
            if (codigo.Length > 0)
            {
                Solicitud detalle = sistema.Solicitudes.ObtenerPorCodigo(codigo);
                if (detalle == null)
                    throw new ReglaNegocioException("No existe una solicitud con el código " + codigo + ".");
                MostrarDetalleSolicitud(detalle);
            }
        }

        private void MostrarDetalleSolicitud(Solicitud s)
        {
            Console.WriteLine("\n  Solicitud:     " + s.Codigo);
            Console.WriteLine("  Cliente:       " + s.Cliente);
            Console.WriteLine("  Equipo:        " + s.Equipo);
            Console.WriteLine("  Problema:      " + s.Descripcion);
            Console.WriteLine("  Recepción:     " + Consola.Fecha(s.FechaRecepcion));
            Console.WriteLine("  Prioridad:     " + s.Prioridad + " | Estado: " + s.Estado);
            Console.WriteLine("  Técnico:       " + (s.Tecnico == null ? "(sin asignar)" : s.Tecnico.ToString()));
            Console.WriteLine("  Diagnóstico:   " + (s.Diagnostico.Length == 0 ? "(pendiente)" : s.Diagnostico));
            Console.WriteLine("  Tiempo est.:   " + s.TiempoEstimadoHoras.ToString("0.0") + " h");
            if (s.RepuestosUtilizados.Count > 0)
            {
                Console.WriteLine("  Repuestos:");
                foreach (RepuestoUtilizado r in s.RepuestosUtilizados)
                    Console.WriteLine("    " + r.Cantidad + " x " + r.Repuesto.Nombre + " a " + Consola.Dinero(r.PrecioAplicado)
                                      + " = " + Consola.Dinero(r.CalcularSubtotal()));
            }
            Console.WriteLine("  Mano de obra:  " + Consola.Dinero(s.CostoManoObra));
            Console.WriteLine("  Repuestos:     " + Consola.Dinero(s.CalcularCostoRepuestos()));
            Console.WriteLine("  TOTAL:         " + Consola.Dinero(s.CalcularTotal()));
            if (s.FechaEntrega.HasValue)
                Console.WriteLine("  Entrega:       " + Consola.Fecha(s.FechaEntrega.Value) + " | Calificación: " + s.Calificacion);
        }

        private void MenuRepuestos()
        {
            int opcion;
            do
            {
                Consola.Titulo("GESTIÓN DE REPUESTOS");
                Console.WriteLine(" 1. Registrar repuesto");
                Console.WriteLine(" 2. Consultar repuestos");
                Console.WriteLine(" 3. Agregar existencia");
                Console.WriteLine(" 4. Volver");
                opcion = Consola.LeerEntero("Seleccione una opción (1-4): ", 1, 4);
                switch (opcion)
                {
                    case 1: Ejecutar(RegistrarRepuesto); break;
                    case 2: Ejecutar(ConsultarRepuestos); break;
                    case 3: Ejecutar(AgregarExistencia); break;
                }
            } while (opcion != 4);
        }

        private void RegistrarRepuesto()
        {
            string nombre = Consola.LeerTexto("Nombre del repuesto: ");
            string categoria = Consola.LeerTexto("Categoría: ");
            int existencia = Consola.LeerEntero("Existencia inicial: ", 0, int.MaxValue);
            int minima = Consola.LeerEntero("Existencia mínima: ", 0, int.MaxValue);
            decimal costo = Consola.LeerDecimal("Costo unitario: Q ");
            decimal precio = Consola.LeerDecimal("Precio de venta: Q ");
            Repuesto r = sistema.Repuestos.Registrar(nombre, categoria, existencia, minima, costo, precio);
            Console.WriteLine("\n  Repuesto registrado: " + r);
        }

        private void ConsultarRepuestos()
        {
            List<Repuesto> lista = sistema.Repuestos.ObtenerTodos();
            if (lista.Count == 0)
            {
                Console.WriteLine("\n  No hay repuestos registrados.");
                return;
            }
            Console.WriteLine();
            Console.WriteLine(Consola.Ajustar("Código", 10) + Consola.Ajustar("Repuesto", 24) + Consola.Ajustar("Categoría", 15)
                              + Consola.Ajustar("Exist.", 7) + Consola.Ajustar("Mín.", 6) + Consola.Ajustar("Costo", 11)
                              + Consola.Ajustar("Precio", 11) + Consola.Ajustar("Usados", 7) + "Situación");
            Consola.Linea(105);
            foreach (Repuesto r in lista)
            {
                Console.WriteLine(Consola.Ajustar(r.Codigo, 10) + Consola.Ajustar(r.Nombre, 24) + Consola.Ajustar(r.Categoria, 15)
                                  + Consola.Ajustar(r.Existencia.ToString(), 7) + Consola.Ajustar(r.ExistenciaMinima.ToString(), 6)
                                  + Consola.Ajustar(Consola.Dinero(r.CostoUnitario), 11) + Consola.Ajustar(Consola.Dinero(r.PrecioVenta), 11)
                                  + Consola.Ajustar(r.CantidadUtilizada.ToString(), 7) + r.ObtenerSituacion());
            }
        }

        private void AgregarExistencia()
        {
            string codigo = Consola.LeerTexto("Código del repuesto: ");
            int cantidad = Consola.LeerEntero("Cantidad a agregar: ", 1, int.MaxValue);
            sistema.Repuestos.AgregarExistencia(codigo, cantidad);
            Console.WriteLine("\n  Existencia actualizada: " + sistema.Repuestos.ObtenerPorCodigo(codigo));
        }

        private void MenuColas()
        {
            int opcion;
            do
            {
                Consola.Titulo("ATENCIÓN DE COLAS");
                Console.WriteLine(" 1. Ver colas");
                Console.WriteLine(" 2. Atender siguiente solicitud");
                Console.WriteLine(" 3. Mostrar matriz de carga semanal");
                Console.WriteLine(" 4. Volver");
                opcion = Consola.LeerEntero("Seleccione una opción (1-4): ", 1, 4);
                switch (opcion)
                {
                    case 1: Ejecutar(reportes.EsperaPorPrioridad); break;
                    case 2: Ejecutar(AtenderSiguiente); break;
                    case 3: Ejecutar(reportes.MatrizSemanal); break;
                }
            } while (opcion != 4);
        }

        private void AtenderSiguiente()
        {
            Solicitud s = sistema.Solicitudes.AtenderSiguiente();
            Console.WriteLine("\n  Solicitud atendida: " + s.Codigo + " (" + s.Prioridad + ")");
            Console.WriteLine("  Cliente: " + s.Cliente.NombreCompleto + " | Equipo: " + s.Equipo);
            Console.WriteLine("  Técnico asignado: " + s.Tecnico.NombreCompleto + " (" + s.Tecnico.TrabajosActivos + "/" + Tecnico.LIMITE_TRABAJOS + " trabajos activos)");
            Console.WriteLine("  Estado: " + s.Estado);
        }

        private void MenuOrdenamientos()
        {
            int opcion;
            do
            {
                Consola.Titulo("ORDENAMIENTOS");
                Console.WriteLine(" 1. Solicitudes por costo total (Bubble Sort)");
                Console.WriteLine(" 2. Técnicos por servicios finalizados (Selection Sort)");
                Console.WriteLine(" 3. Repuestos por existencia (Shell Sort)");
                Console.WriteLine(" 4. Solicitudes por fecha o tiempo estimado (Quick Sort)");
                Console.WriteLine(" 5. Volver");
                opcion = Consola.LeerEntero("Seleccione una opción (1-5): ", 1, 5);
                switch (opcion)
                {
                    case 1: Ejecutar(OrdenarSolicitudesPorCosto); break;
                    case 2: Ejecutar(OrdenarTecnicos); break;
                    case 3: Ejecutar(OrdenarRepuestos); break;
                    case 4: Ejecutar(OrdenarSolicitudesPorFechaOTiempo); break;
                }
            } while (opcion != 5);
        }

        private void OrdenarSolicitudesPorCosto()
        {
            reportes.SolicitudesPorCosto(Consola.PreguntarAscendente());
        }

        private void OrdenarTecnicos()
        {
            reportes.TecnicosPorFinalizados(Consola.PreguntarAscendente());
        }

        private void OrdenarRepuestos()
        {
            reportes.RepuestosPorExistencia(Consola.PreguntarAscendente());
        }

        private void OrdenarSolicitudesPorFechaOTiempo()
        {
            int criterio = Consola.ElegirOpcion("Criterio de ordenamiento:", new string[] { "Fecha de recepción", "Tiempo estimado de atención" });
            bool asc = Consola.PreguntarAscendente();
            reportes.SolicitudesPorFechaOTiempo(criterio == 0, asc);
        }

        private void MenuBusquedas()
        {
            int opcion;
            do
            {
                Consola.Titulo("BÚSQUEDAS");
                Console.WriteLine(" 1. Clientes por nombre parcial (secuencial)");
                Console.WriteLine(" 2. Solicitudes por estado (secuencial)");
                Console.WriteLine(" 3. Equipos por marca (secuencial)");
                Console.WriteLine(" 4. Solicitud por código (binaria)");
                Console.WriteLine(" 5. Volver");
                opcion = Consola.LeerEntero("Seleccione una opción (1-5): ", 1, 5);
                switch (opcion)
                {
                    case 1: Ejecutar(BuscarClientes); break;
                    case 2: Ejecutar(BuscarSolicitudesPorEstado); break;
                    case 3: Ejecutar(BuscarEquipos); break;
                    case 4: Ejecutar(BuscarSolicitudPorCodigo); break;
                }
            } while (opcion != 5);
        }

        private void RegistrarYMostrar(string tipo, string criterio, string valor, List<string> coincidencias)
        {
            sistema.Busquedas.Add(new ResultadoBusqueda(tipo, criterio, valor, coincidencias));
            Console.WriteLine();
            if (coincidencias.Count == 0)
            {
                Console.WriteLine("  Sin coincidencias.");
                return;
            }
            Console.WriteLine("  Coincidencias encontradas: " + coincidencias.Count);
            foreach (string linea in coincidencias)
                Console.WriteLine("   - " + linea);
        }

        private void BuscarClientes()
        {
            string texto = Consola.LeerTexto("Nombre (o parte del nombre) a buscar: ");
            List<Cliente> encontrados = Buscador.ClientesPorNombre(sistema.Clientes.ObtenerTodos(), texto);
            List<string> lineas = new List<string>();
            foreach (Cliente c in encontrados)
                lineas.Add(c.ObtenerResumen());
            RegistrarYMostrar("Secuencial", ResultadoBusqueda.CRITERIO_CLIENTE, texto, lineas);
        }

        private void BuscarSolicitudesPorEstado()
        {
            int indice = Consola.ElegirOpcion("Estado a buscar:", Enum.GetNames(typeof(EstadoSolicitud)));
            EstadoSolicitud estado = (EstadoSolicitud)indice;
            List<Solicitud> encontradas = Buscador.SolicitudesPorEstado(sistema.Solicitudes.ObtenerTodas(), estado);
            List<string> lineas = new List<string>();
            foreach (Solicitud s in encontradas)
                lineas.Add(s.ToString());
            RegistrarYMostrar("Secuencial", ResultadoBusqueda.CRITERIO_ESTADO, estado.ToString(), lineas);
        }

        private void BuscarEquipos()
        {
            string marca = Consola.LeerTexto("Marca a buscar: ");
            List<Equipo> encontrados = Buscador.EquiposPorMarca(sistema.Equipos.ObtenerTodos(), marca);
            List<string> lineas = new List<string>();
            foreach (Equipo e in encontrados)
                lineas.Add(e.ToString());
            RegistrarYMostrar("Secuencial", ResultadoBusqueda.CRITERIO_MARCA, marca, lineas);
        }

        private void BuscarSolicitudPorCodigo()
        {
            string codigo = Consola.LeerTexto("Código de solicitud a buscar (ejemplo SOL-0001): ");
            List<Solicitud> datos = sistema.Solicitudes.ObtenerTodas();
            if (datos.Count == 0)
                throw new ReglaNegocioException("No hay solicitudes registradas.");

            if (!Buscador.EstaOrdenadaPorCodigo(datos))
            {
                Console.WriteLine("  Los datos NO están ordenados por código.");
                if (!Consola.Confirmar("¿Ordenar una copia con Quick Sort por código para poder buscar?"))
                    throw new ReglaNegocioException("La búsqueda binaria requiere datos ordenados por código.");
                datos = new List<Solicitud>(datos);
                Ordenador.QuickSort(datos, s => s.Codigo, true);
            }
            else
            {
                Console.WriteLine("  Datos verificados: ordenados por código.");
            }

            Solicitud encontrada = Buscador.BinariaPorCodigo(datos, codigo);
            List<string> lineas = new List<string>();
            if (encontrada != null)
                lineas.Add(encontrada.ToString());
            RegistrarYMostrar("Binaria", ResultadoBusqueda.CRITERIO_CODIGO, codigo, lineas);
        }

        private void MenuReportes()
        {
            int opcion;
            do
            {
                Consola.Titulo("REPORTES");
                Console.WriteLine(" 1. Solicitudes en espera por prioridad");
                Console.WriteLine(" 2. Solicitudes activas, finalizadas y canceladas");
                Console.WriteLine(" 3. Técnicos disponibles y con carga completa");
                Console.WriteLine(" 4. Técnico con más servicios finalizados");
                Console.WriteLine(" 5. Equipos atendidos por tipo");
                Console.WriteLine(" 6. Solicitudes ordenadas por costo total");
                Console.WriteLine(" 7. Solicitudes ordenadas por fecha o tiempo estimado");
                Console.WriteLine(" 8. Repuestos ordenados por existencia");
                Console.WriteLine(" 9. Repuestos con existencia mínima o agotados");
                Console.WriteLine("10. Ingresos totales por servicios finalizados");
                Console.WriteLine("11. Resultados de las búsquedas");
                Console.WriteLine("12. Matriz semanal de carga de trabajo");
                Console.WriteLine("13. Volver");
                opcion = Consola.LeerEntero("Seleccione una opción (1-13): ", 1, 13);
                switch (opcion)
                {
                    case 1: Ejecutar(reportes.EsperaPorPrioridad); break;
                    case 2: Ejecutar(reportes.ActivasFinalizadasCanceladas); break;
                    case 3: Ejecutar(reportes.TecnicosDisponiblesYCompletos); break;
                    case 4: Ejecutar(reportes.TecnicoConMasFinalizados); break;
                    case 5: Ejecutar(reportes.EquiposPorTipo); break;
                    case 6: Ejecutar(OrdenarSolicitudesPorCosto); break;
                    case 7: Ejecutar(OrdenarSolicitudesPorFechaOTiempo); break;
                    case 8: Ejecutar(OrdenarRepuestos); break;
                    case 9: Ejecutar(reportes.RepuestosMinimosOAgotados); break;
                    case 10: Ejecutar(reportes.IngresosTotales); break;
                    case 11: Ejecutar(reportes.ResultadosBusquedas); break;
                    case 12: Ejecutar(reportes.MatrizSemanal); break;
                }
            } while (opcion != 13);
        }

        private void MenuDatosPrueba()
        {
            Consola.Titulo("GENERAR DATOS DE PRUEBA");
            Console.WriteLine("Se generarán clientes, técnicos, repuestos, equipos y solicitudes con valores aleatorios controlados.");
            Ejecutar(GenerarDatos);
        }

        private void GenerarDatos()
        {
            int cantidad = Consola.LeerEntero("¿Cuántos registros base desea generar? (1-50): ", 1, 50);
            if (!Consola.Confirmar("¿Generar " + cantidad + " clientes, equipos y solicitudes con sus datos relacionados?"))
            {
                Console.WriteLine("  Operación cancelada.");
                return;
            }
            generador.Generar(cantidad);
        }
    }
}
