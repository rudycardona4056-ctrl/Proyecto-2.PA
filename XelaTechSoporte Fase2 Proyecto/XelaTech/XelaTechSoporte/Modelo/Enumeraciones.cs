namespace XelaTechSoporte
{
    
    public enum PrioridadSolicitud { Normal, Urgente }

    
    public enum EstadoSolicitud
    {
        Recibida,
        EnEspera,
        Asignada,
        EnDiagnostico,
        EnReparacion,
        ListaParaEntregar,
        Entregada,
        Cancelada
    }

    
    public enum TipoCliente { Particular, Estudiante, Profesional, Negocio, Institucion }

    public enum Especialidad { Portatiles, Escritorio, Impresoras }

    public enum TipoEquipo { ComputadoraPortatil = 0, ComputadoraEscritorio = 1, Impresora = 2 }
}
