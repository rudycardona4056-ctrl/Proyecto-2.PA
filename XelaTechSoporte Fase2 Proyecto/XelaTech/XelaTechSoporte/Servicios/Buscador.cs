using System;
using System.Collections.Generic;

namespace XelaTechSoporte
{
    public static class Buscador
    {
        
        public static List<Cliente> ClientesPorNombre(List<Cliente> clientes, string texto)
        {
            Validador.TextoObligatorio("Texto a buscar", texto);
            string buscado = texto.Trim();
            List<Cliente> coincidencias = new List<Cliente>();
            foreach (Cliente c in clientes)
            {
                if (c.NombreCompleto.IndexOf(buscado, StringComparison.OrdinalIgnoreCase) >= 0)
                    coincidencias.Add(c);
            }
            return coincidencias;
        }

        public static List<Solicitud> SolicitudesPorEstado(List<Solicitud> solicitudes, EstadoSolicitud estado)
        {
            List<Solicitud> coincidencias = new List<Solicitud>();
            foreach (Solicitud s in solicitudes)
            {
                if (s.Estado == estado)
                    coincidencias.Add(s);
            }
            return coincidencias;
        }

        public static List<Equipo> EquiposPorMarca(List<Equipo> equipos, string marca)
        {
            Validador.TextoObligatorio("Marca", marca);
            string buscada = marca.Trim();
            List<Equipo> coincidencias = new List<Equipo>();
            foreach (Equipo e in equipos)
            {
                if (e.Marca.IndexOf(buscada, StringComparison.OrdinalIgnoreCase) >= 0)
                    coincidencias.Add(e);
            }
            return coincidencias;
        }

        
        public static bool EstaOrdenadaPorCodigo(List<Solicitud> solicitudes)
        {
            for (int i = 1; i < solicitudes.Count; i++)
            {
                if (string.CompareOrdinal(solicitudes[i - 1].Codigo, solicitudes[i].Codigo) > 0)
                    return false;
            }
            return true;
        }

        
        public static Solicitud BinariaPorCodigo(List<Solicitud> solicitudes, string codigo)
        {
            Validador.TextoObligatorio("Código", codigo);
            if (solicitudes.Count == 0)
                throw new ReglaNegocioException("No hay solicitudes registradas.");
            if (!EstaOrdenadaPorCodigo(solicitudes))
                throw new ReglaNegocioException("Los datos no están ordenados por código; ordénelos primero.");

            string buscado = codigo.Trim().ToUpperInvariant();
            int izq = 0;
            int der = solicitudes.Count - 1;
            while (izq <= der)
            {
                int medio = izq + (der - izq) / 2;
                int cmp = string.CompareOrdinal(solicitudes[medio].Codigo, buscado);
                if (cmp == 0)
                    return solicitudes[medio];
                else if (cmp < 0)
                    izq = medio + 1;
                else
                    der = medio - 1;
            }
            return null;
        }
    }
}
