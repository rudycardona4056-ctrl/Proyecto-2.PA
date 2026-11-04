using System;
using System.Collections.Generic;

namespace XelaTechSoporte
{
    public class ResultadoBusqueda
    {
        
        public const string CRITERIO_CLIENTE = "Cliente por nombre parcial";
        public const string CRITERIO_ESTADO = "Solicitud por estado";
        public const string CRITERIO_MARCA = "Equipo por marca";
        public const string CRITERIO_CODIGO = "Solicitud por código";

        private string tipoBusqueda;
        private string criterio;
        private string valorBuscado;
        private List<string> coincidencias;
        private DateTime fechaHora;

        public ResultadoBusqueda(string tipoBusqueda, string criterio, string valorBuscado, List<string> coincidencias)
        {
            this.tipoBusqueda = tipoBusqueda;
            this.criterio = criterio;
            this.valorBuscado = valorBuscado;
            this.coincidencias = coincidencias;
            this.fechaHora = DateTime.Now;
        }

        public string TipoBusqueda { get { return tipoBusqueda; } }
        public string Criterio { get { return criterio; } }
        public string ValorBuscado { get { return valorBuscado; } }
        public List<string> Coincidencias { get { return coincidencias; } }
        public DateTime FechaHora { get { return fechaHora; } }
    }
}
