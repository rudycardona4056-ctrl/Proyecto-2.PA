using System;
using System.Collections.Generic;

namespace XelaTechSoporte
{
    public class GestorRepuestos
    {
        private List<Repuesto> repuestos = new List<Repuesto>();
        private int contador = 0;

        public Repuesto Registrar(string nombre, string categoria, int existencia, int existenciaMinima,
                                  decimal costoUnitario, decimal precioVenta)
        {
            Validador.TextoObligatorio("Nombre", nombre);
            Validador.TextoObligatorio("Categoría", categoria);
            Validador.NoNegativo("Existencia", existencia);
            Validador.NoNegativo("Existencia mínima", existenciaMinima);
            Validador.NoNegativo("Costo unitario", costoUnitario);
            Validador.NoNegativo("Precio de venta", precioVenta);
            if (precioVenta < costoUnitario)
                throw new ReglaNegocioException("El precio de venta no puede ser menor que el costo unitario.");

            contador++;
            Repuesto nuevo = new Repuesto("REP-" + contador.ToString("D4"), nombre, categoria, existencia,
                                          existenciaMinima, costoUnitario, precioVenta);
            repuestos.Add(nuevo);
            return nuevo;
        }

        public Repuesto ObtenerPorCodigo(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return null;
            foreach (Repuesto r in repuestos)
            {
                if (string.Equals(r.Codigo, codigo.Trim(), StringComparison.OrdinalIgnoreCase))
                    return r;
            }
            return null;
        }

        public void AgregarExistencia(string codigo, int cantidad)
        {
            Repuesto r = ObtenerPorCodigo(codigo);
            if (r == null)
                throw new ReglaNegocioException("No existe un repuesto con el código " + codigo + ".");
            r.AgregarExistencia(cantidad);
        }

        public List<Repuesto> ObtenerTodos()
        {
            return repuestos;
        }
    }
}
