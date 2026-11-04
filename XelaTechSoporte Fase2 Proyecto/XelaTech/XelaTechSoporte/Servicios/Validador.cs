namespace XelaTechSoporte
{
    public static class Validador
    {
        public static void TextoObligatorio(string campo, string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                throw new ReglaNegocioException("El campo '" + campo + "' es obligatorio y no puede estar vacío.");
        }

        public static void NoNegativo(string campo, decimal valor)
        {
            if (valor < 0)
                throw new ReglaNegocioException("El campo '" + campo + "' no puede ser negativo.");
        }

        public static void EnteroPositivo(string campo, int valor)
        {
            if (valor <= 0)
                throw new ReglaNegocioException("El campo '" + campo + "' debe ser mayor que cero.");
        }

        public static void CalificacionValida(int valor)
        {
            if (valor < 1 || valor > 5)
                throw new ReglaNegocioException("La calificación debe estar entre 1 y 5.");
        }

        public static void TelefonoValido(string telefono)
        {
            if (telefono == null || telefono.Trim().Length != 8)
                throw new ReglaNegocioException("El teléfono debe tener exactamente 8 dígitos.");
            foreach (char c in telefono.Trim())
            {
                if (!char.IsDigit(c))
                    throw new ReglaNegocioException("El teléfono solo puede contener números.");
            }
        }

        public static void CorreoValido(string correo)
        {
            if (string.IsNullOrWhiteSpace(correo))
                throw new ReglaNegocioException("El correo electrónico es obligatorio.");
            string c = correo.Trim();
            int arroba = c.IndexOf('@');
            bool valido = arroba > 0
                          && c.IndexOf('@', arroba + 1) < 0
                          && c.IndexOf('.', arroba) >= arroba + 2
                          && !c.EndsWith(".")
                          && !c.Contains(" ");
            if (!valido)
                throw new ReglaNegocioException("El correo electrónico no tiene un formato válido (ejemplo: nombre@correo.com).");
        }
    }
}
