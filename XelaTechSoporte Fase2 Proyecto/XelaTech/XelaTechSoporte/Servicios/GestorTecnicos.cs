using System;
using System.Collections.Generic;

namespace XelaTechSoporte
{
    
    public class GestorTecnicos
    {
        private List<Tecnico> tecnicos = new List<Tecnico>();
        private int[] calificaciones = new int[5];
        private int contador = 0;

        public Tecnico Registrar(string nombre, Especialidad especialidad)
        {
            Validador.TextoObligatorio("Nombre completo", nombre);
            if (tecnicos.Count >= MatrizCarga.MAX_TECNICOS)
                throw new ReglaNegocioException("No se pueden registrar más de " + MatrizCarga.MAX_TECNICOS
                                                + " técnicos (filas de la matriz de carga).");

            contador++;
            string codigo = "TEC-" + contador.ToString("D4");
            
            Tecnico nuevo = new Tecnico(codigo, nombre, especialidad, tecnicos.Count);
            tecnicos.Add(nuevo);
            return nuevo;
        }

        public Tecnico ObtenerPorCodigo(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return null;
            foreach (Tecnico t in tecnicos)
            {
                if (string.Equals(t.Codigo, codigo.Trim(), StringComparison.OrdinalIgnoreCase))
                    return t;
            }
            return null;
        }

        
        public Tecnico BuscarMejorDisponible(Especialidad especialidad)
        {
            Tecnico mejor = null;
            foreach (Tecnico t in tecnicos)
            {
                if (t.PuedeRecibir(especialidad))
                {
                    if (mejor == null || t.TrabajosActivos < mejor.TrabajosActivos)
                        mejor = t;
                }
            }
            return mejor;
        }

        public int CantidadPorEspecialidad(Especialidad especialidad)
        {
            int cantidad = 0;
            foreach (Tecnico t in tecnicos)
            {
                if (t.Especialidad == especialidad)
                    cantidad++;
            }
            return cantidad;
        }

        public void RegistrarCalificacion(int valor)
        {
            Validador.CalificacionValida(valor);
            calificaciones[valor - 1]++;
        }

        public int[] ObtenerDistribucion()
        {
            return calificaciones;
        }

        public List<Tecnico> ObtenerTodos()
        {
            return tecnicos;
        }
    }
}
