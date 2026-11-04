using System;
using System.Globalization;
using System.IO;

namespace XelaTechSoporte
{
    public static class Consola
    {
        public static string LeerTexto(string mensaje)
        {
            Console.Write(mensaje);
            string linea = Console.ReadLine();
            if (linea == null)
                throw new EndOfStreamException("Fin de la entrada.");
            return linea.Trim();
        }

        public static int LeerEntero(string mensaje, int minimo, int maximo)
        {
            while (true)
            {
                string texto = LeerTexto(mensaje);
                int valor;
                if (int.TryParse(texto, NumberStyles.Integer, CultureInfo.InvariantCulture, out valor)
                    && valor >= minimo && valor <= maximo)
                    return valor;

                if (maximo == int.MaxValue)
                    Console.WriteLine("  Entrada inválida. Ingrese un número entero mayor o igual a " + minimo + ".");
                else
                    Console.WriteLine("  Entrada inválida. Ingrese un número entre " + minimo + " y " + maximo + ".");
            }
        }

        public static decimal LeerDecimal(string mensaje)
        {
            while (true)
            {
                string texto = LeerTexto(mensaje).Replace(',', '.');
                decimal valor;
                if (decimal.TryParse(texto, NumberStyles.Number, CultureInfo.InvariantCulture, out valor))
                    return valor;
                Console.WriteLine("  Entrada inválida. Ingrese un número (por ejemplo 125.50).");
            }
        }

        public static bool Confirmar(string mensaje)
        {
            while (true)
            {
                string r = LeerTexto(mensaje + " (s/n): ").ToLowerInvariant();
                if (r == "s" || r == "si" || r == "sí") return true;
                if (r == "n" || r == "no") return false;
                Console.WriteLine("  Responda con 's' o 'n'.");
            }
        }

        public static int ElegirOpcion(string titulo, string[] opciones)
        {
            Console.WriteLine(titulo);
            for (int i = 0; i < opciones.Length; i++)
                Console.WriteLine("  " + (i + 1) + ". " + opciones[i]);
            return LeerEntero("  Seleccione (1-" + opciones.Length + "): ", 1, opciones.Length) - 1;
        }

        public static bool PreguntarAscendente()
        {
            return ElegirOpcion("Sentido del ordenamiento:", new string[] { "Ascendente", "Descendente" }) == 0;
        }

        public static void Pausa()
        {
            Console.Write("\nPresione Enter para continuar...");
            Console.ReadLine();
        }

        public static void Linea(int largo)
        {
            Console.WriteLine(new string('-', largo));
        }

        public static void Titulo(string texto)
        {
            Console.WriteLine();
            Console.WriteLine(new string('=', 60));
            Console.WriteLine("  " + texto);
            Console.WriteLine(new string('=', 60));
        }

        public static string Ajustar(string texto, int ancho)
        {
            if (texto == null) texto = "";
            if (texto.Length > ancho)
                return texto.Substring(0, ancho - 1) + ".";
            return texto.PadRight(ancho);
        }

        public static string Dinero(decimal valor)
        {
            return "Q " + valor.ToString("N2", CultureInfo.InvariantCulture);
        }

        public static string Fecha(DateTime fecha)
        {
            return fecha.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
        }
    }
}
