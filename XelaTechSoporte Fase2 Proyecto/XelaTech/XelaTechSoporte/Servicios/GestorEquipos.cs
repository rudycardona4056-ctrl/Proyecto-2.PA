using System;
using System.Collections.Generic;

namespace XelaTechSoporte
{
    public class GestorEquipos
    {
        private List<Equipo> equipos = new List<Equipo>();
        private int contador = 0;

        private string GenerarCodigo()
        {
            contador++;
            return "EQ-" + contador.ToString("D4");
        }

        public bool ExisteSerie(string serie)
        {
            if (string.IsNullOrWhiteSpace(serie))
                return false;
            foreach (Equipo e in equipos)
            {
                if (string.Equals(e.NumeroSerie, serie.Trim(), StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        private void ValidarAntesDeRegistrar(string serie, Cliente propietario)
        {
            Validador.TextoObligatorio("Número de serie", serie);
            if (propietario == null)
                throw new ReglaNegocioException("El propietario debe ser un cliente existente.");
            if (ExisteSerie(serie))
                throw new ReglaNegocioException("El número de serie " + serie.Trim() + " ya está registrado.");
        }

        public Equipo RegistrarPortatil(string serie, string marca, string modelo, Cliente propietario,
                                        string estadoFisico, string accesorios, bool requiereRevisionBateria)
        {
            ValidarAntesDeRegistrar(serie, propietario);
            Equipo nuevo = new ComputadoraPortatil(GenerarCodigo(), serie, marca, modelo, propietario,
                                                   estadoFisico, accesorios, requiereRevisionBateria);
            equipos.Add(nuevo);
            return nuevo;
        }

        public Equipo RegistrarEscritorio(string serie, string marca, string modelo, Cliente propietario,
                                          string estadoFisico, string accesorios, bool incluyeMonitor)
        {
            ValidarAntesDeRegistrar(serie, propietario);
            Equipo nuevo = new ComputadoraEscritorio(GenerarCodigo(), serie, marca, modelo, propietario,
                                                     estadoFisico, accesorios, incluyeMonitor);
            equipos.Add(nuevo);
            return nuevo;
        }

        public Equipo RegistrarImpresora(string serie, string marca, string modelo, Cliente propietario,
                                         string estadoFisico, string accesorios, string tipoImpresion)
        {
            ValidarAntesDeRegistrar(serie, propietario);
            Equipo nuevo = new Impresora(GenerarCodigo(), serie, marca, modelo, propietario,
                                         estadoFisico, accesorios, tipoImpresion);
            equipos.Add(nuevo);
            return nuevo;
        }

        public Equipo ObtenerPorCodigo(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return null;
            foreach (Equipo e in equipos)
            {
                if (string.Equals(e.CodigoInterno, codigo.Trim(), StringComparison.OrdinalIgnoreCase))
                    return e;
            }
            return null;
        }

        public List<Equipo> ObtenerTodos()
        {
            return equipos;
        }
    }
}
