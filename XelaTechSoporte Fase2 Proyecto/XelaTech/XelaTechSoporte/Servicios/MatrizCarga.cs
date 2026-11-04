using System;
using System.Collections.Generic;

namespace XelaTechSoporte
{
   
    public class MatrizCarga
    {
        public const int MAX_TECNICOS = 10;
        public const int DIAS = 7;

        private static readonly string[] NombresCortos = { "Lun", "Mar", "Mié", "Jue", "Vie", "Sáb", "Dom" };
        private static readonly string[] NombresDias = { "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado", "Domingo" };

        private int[,] carga = new int[MAX_TECNICOS, DIAS];

        public static int IndiceDia(DayOfWeek dia)
        {
            return ((int)dia + 6) % 7;
        }

        public static string NombreDia(int columna)
        {
            return NombresDias[columna];
        }

        public void RegistrarAsignacion(int fila, DayOfWeek dia)
        {
            if (fila < 0 || fila >= MAX_TECNICOS)
                throw new ReglaNegocioException("La fila del técnico en la matriz no es válida.");
            carga[fila, IndiceDia(dia)]++;
        }

        public int ObtenerValor(int fila, int columna)
        {
            return carga[fila, columna];
        }

        public int SumaFila(int fila)
        {
            int suma = 0;
            for (int d = 0; d < DIAS; d++)
                suma += carga[fila, d];
            return suma;
        }

        public int SumaDia(int columna, int filas)
        {
            int suma = 0;
            for (int f = 0; f < filas; f++)
                suma += carga[f, columna];
            return suma;
        }

        public int ObtenerFilaMayorCarga(int filas)
        {
            int mejor = -1;
            int mayor = 0;
            for (int f = 0; f < filas; f++)
            {
                int suma = SumaFila(f);
                if (suma > mayor)
                {
                    mayor = suma;
                    mejor = f;
                }
            }
            return mejor;
        }

        public int ObtenerDiaMayorCarga(int filas)
        {
            int mejor = -1;
            int mayor = 0;
            for (int d = 0; d < DIAS; d++)
            {
                int suma = SumaDia(d, filas);
                if (suma > mayor)
                {
                    mayor = suma;
                    mejor = d;
                }
            }
            return mejor;
        }

        public void Mostrar(List<Tecnico> tecnicos)
        {
            Consola.Titulo("MATRIZ SEMANAL DE CARGA DE TRABAJO");
            if (tecnicos.Count == 0)
            {
                Console.WriteLine("No hay técnicos registrados.");
                return;
            }

            int filas = tecnicos.Count;
            string encabezado = Consola.Ajustar("Técnico", 26);
            for (int d = 0; d < DIAS; d++)
                encabezado += NombresCortos[d].PadLeft(5);
            encabezado += "  | Total";
            Console.WriteLine(encabezado);
            Consola.Linea(encabezado.Length);

            for (int f = 0; f < filas; f++)
            {
                string linea = Consola.Ajustar(tecnicos[f].Codigo + " " + tecnicos[f].NombreCompleto, 26);
                for (int d = 0; d < DIAS; d++)
                    linea += carga[f, d].ToString().PadLeft(5);
                linea += "  | " + SumaFila(f).ToString().PadLeft(5);
                Console.WriteLine(linea);
            }

            Consola.Linea(encabezado.Length);
            string totales = Consola.Ajustar("Total por día", 26);
            int granTotal = 0;
            for (int d = 0; d < DIAS; d++)
            {
                int s = SumaDia(d, filas);
                granTotal += s;
                totales += s.ToString().PadLeft(5);
            }
            totales += "  | " + granTotal.ToString().PadLeft(5);
            Console.WriteLine(totales);
            Console.WriteLine();

            int filaMayor = ObtenerFilaMayorCarga(filas);
            int diaMayor = ObtenerDiaMayorCarga(filas);
            if (filaMayor < 0)
            {
                Console.WriteLine("Todavía no se han asignado solicitudes.");
            }
            else
            {
                Console.WriteLine("Técnico con mayor carga: " + tecnicos[filaMayor].Codigo + " " + tecnicos[filaMayor].NombreCompleto
                                  + " (" + SumaFila(filaMayor) + " solicitudes)");
                Console.WriteLine("Día con mayor carga:     " + NombresDias[diaMayor] + " (" + SumaDia(diaMayor, filas) + " solicitudes)");
            }
        }
    }
}
