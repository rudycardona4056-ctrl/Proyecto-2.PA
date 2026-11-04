using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace XelaTechSoporte
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
            Console.Title = "XelaTech Soporte";

            try
            {
                SistemaXelaTech sistema = new SistemaXelaTech();
                MenuConsola menu = new MenuConsola(sistema);
                menu.Iniciar();
            }
            catch (EndOfStreamException)
            {
                Console.WriteLine("\nSe cerró la entrada de datos. Fin del programa.");
            }
        }
    }
}
