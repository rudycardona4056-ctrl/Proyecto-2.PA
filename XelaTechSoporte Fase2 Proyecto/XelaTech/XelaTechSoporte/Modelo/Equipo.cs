using System;

namespace XelaTechSoporte
{
    
    public abstract class Equipo
    {
        protected string codigoInterno;
        protected string numeroSerie;
        protected string marca;
        protected string modelo;
        protected Cliente propietario;
        protected string estadoFisico;
        protected string accesorios;

        protected Equipo(string codigoInterno, string numeroSerie, string marca, string modelo,
                         Cliente propietario, string estadoFisico, string accesorios)
        {
            Validador.TextoObligatorio("Código interno", codigoInterno);
            Validador.TextoObligatorio("Número de serie", numeroSerie);
            Validador.TextoObligatorio("Marca", marca);
            Validador.TextoObligatorio("Modelo", modelo);
            Validador.TextoObligatorio("Estado físico recibido", estadoFisico);
            Validador.TextoObligatorio("Accesorios entregados", accesorios);
            if (propietario == null)
                throw new ReglaNegocioException("El propietario debe ser un cliente existente.");

            this.codigoInterno = codigoInterno.Trim();
            this.numeroSerie = numeroSerie.Trim();
            this.marca = marca.Trim();
            this.modelo = modelo.Trim();
            this.propietario = propietario;
            this.estadoFisico = estadoFisico.Trim();
            this.accesorios = accesorios.Trim();
        }

        public string CodigoInterno { get { return codigoInterno; } }
        public string NumeroSerie { get { return numeroSerie; } }
        public string Marca { get { return marca; } }
        public string Modelo { get { return modelo; } }
        public Cliente Propietario { get { return propietario; } }
        public string EstadoFisico { get { return estadoFisico; } }
        public string Accesorios { get { return accesorios; } }
        public TipoEquipo Tipo { get { return ObtenerTipo(); } }

        
        public abstract TipoEquipo ObtenerTipo();
        public abstract Especialidad ObtenerEspecialidadRequerida();
        public abstract decimal CalcularCostoRevision();
        public abstract double EstimarTiempoHoras();

        
        public virtual string GenerarDiagnosticoSugerido()
        {
            return "Revisión general del equipo.";
        }

        
        public string NombreTipo()
        {
            switch (ObtenerTipo())
            {
                case TipoEquipo.ComputadoraPortatil: return "Computadora portátil";
                case TipoEquipo.ComputadoraEscritorio: return "Computadora de escritorio";
                default: return "Impresora";
            }
        }

        public override string ToString()
        {
            return codigoInterno + " | " + NombreTipo() + " | " + marca + " " + modelo + " | Serie: " + numeroSerie;
        }
    }

    
    public class ComputadoraPortatil : Equipo
    {
        private bool requiereRevisionBateria;

        public ComputadoraPortatil(string codigoInterno, string numeroSerie, string marca, string modelo,
                                   Cliente propietario, string estadoFisico, string accesorios,
                                   bool requiereRevisionBateria)
            : base(codigoInterno, numeroSerie, marca, modelo, propietario, estadoFisico, accesorios)
        {
            this.requiereRevisionBateria = requiereRevisionBateria;
        }

        public bool RequiereRevisionBateria { get { return requiereRevisionBateria; } }

        public override TipoEquipo ObtenerTipo() { return TipoEquipo.ComputadoraPortatil; }
        public override Especialidad ObtenerEspecialidadRequerida() { return Especialidad.Portatiles; }

        public override decimal CalcularCostoRevision()
        {
            return requiereRevisionBateria ? 150m : 120m;
        }

        public override double EstimarTiempoHoras()
        {
            return requiereRevisionBateria ? 4.0 : 3.0;
        }

        public override string GenerarDiagnosticoSugerido()
        {
            return "Revisar batería, disco, memoria, pantalla y ventilación.";
        }
    }

    public class ComputadoraEscritorio : Equipo
    {
        private bool incluyeMonitor;

        public ComputadoraEscritorio(string codigoInterno, string numeroSerie, string marca, string modelo,
                                     Cliente propietario, string estadoFisico, string accesorios,
                                     bool incluyeMonitor)
            : base(codigoInterno, numeroSerie, marca, modelo, propietario, estadoFisico, accesorios)
        {
            this.incluyeMonitor = incluyeMonitor;
        }

        public bool IncluyeMonitor { get { return incluyeMonitor; } }

        public override TipoEquipo ObtenerTipo() { return TipoEquipo.ComputadoraEscritorio; }
        public override Especialidad ObtenerEspecialidadRequerida() { return Especialidad.Escritorio; }

       
        public override decimal CalcularCostoRevision()
        {
            return incluyeMonitor ? 110m : 90m;
        }

        
        public override double EstimarTiempoHoras()
        {
            return incluyeMonitor ? 3.0 : 2.0;
        }

        public override string GenerarDiagnosticoSugerido()
        {
            return "Revisar fuente de poder, disco, memoria y tarjeta madre.";
        }
    }

    
    public class Impresora : Equipo
    {
        private string tipoImpresion;

        public Impresora(string codigoInterno, string numeroSerie, string marca, string modelo,
                         Cliente propietario, string estadoFisico, string accesorios,
                         string tipoImpresion)
            : base(codigoInterno, numeroSerie, marca, modelo, propietario, estadoFisico, accesorios)
        {
            Validador.TextoObligatorio("Tipo de impresión", tipoImpresion);
            this.tipoImpresion = tipoImpresion.Trim();
        }

        public string TipoImpresion { get { return tipoImpresion; } }

        private bool EsLaser()
        {
            return string.Equals(tipoImpresion, "Laser", StringComparison.OrdinalIgnoreCase)
                   || string.Equals(tipoImpresion, "Láser", StringComparison.OrdinalIgnoreCase);
        }

        public override TipoEquipo ObtenerTipo() { return TipoEquipo.Impresora; }
        public override Especialidad ObtenerEspecialidadRequerida() { return Especialidad.Impresoras; }

        public override decimal CalcularCostoRevision()
        {
            return EsLaser() ? 95m : 70m;
        }

        public override double EstimarTiempoHoras()
        {
            return EsLaser() ? 2.5 : 1.5;
        }

        public override string GenerarDiagnosticoSugerido()
        {
            return "Revisar cabezal o tóner, rodillos, alimentador y conexión.";
        }
    }
}
