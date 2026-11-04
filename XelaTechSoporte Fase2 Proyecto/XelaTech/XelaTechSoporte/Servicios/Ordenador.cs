using System;
using System.Collections.Generic;

namespace XelaTechSoporte
{
    
    public static class Ordenador
    {
        private static bool FueraDeOrden<T>(T a, T b, Func<T, IComparable> clave, bool asc)
        {
            int comparacion = clave(a).CompareTo(clave(b));
            return asc ? comparacion > 0 : comparacion < 0;
        }

        private static void Intercambiar<T>(List<T> lista, int i, int j)
        {
            T temp = lista[i];
            lista[i] = lista[j];
            lista[j] = temp;
        }

        public static void BubbleSort<T>(List<T> lista, Func<T, IComparable> clave, bool asc)
        {
            int n = lista.Count;
            for (int i = 0; i < n - 1; i++)
            {
                bool huboCambio = false;
                for (int j = 0; j < n - 1 - i; j++)
                {
                    if (FueraDeOrden(lista[j], lista[j + 1], clave, asc))
                    {
                        Intercambiar(lista, j, j + 1);
                        huboCambio = true;
                    }
                }
                if (!huboCambio)
                    break;
            }
        }

        public static void SelectionSort<T>(List<T> lista, Func<T, IComparable> clave, bool asc)
        {
            int n = lista.Count;
            for (int i = 0; i < n - 1; i++)
            {
                int indiceSel = i;
                for (int j = i + 1; j < n; j++)
                {
                    if (FueraDeOrden(lista[indiceSel], lista[j], clave, asc))
                        indiceSel = j;
                }
                if (indiceSel != i)
                    Intercambiar(lista, i, indiceSel);
            }
        }

        public static void ShellSort<T>(List<T> lista, Func<T, IComparable> clave, bool asc)
        {
            int n = lista.Count;
            int brecha = n / 2;
            while (brecha > 0)
            {
                for (int i = brecha; i < n; i++)
                {
                    T temp = lista[i];
                    int j = i;
                    while (j >= brecha && FueraDeOrden(lista[j - brecha], temp, clave, asc))
                    {
                        lista[j] = lista[j - brecha];
                        j -= brecha;
                    }
                    lista[j] = temp;
                }
                brecha /= 2;
            }
        }

        public static void QuickSort<T>(List<T> lista, Func<T, IComparable> clave, bool asc)
        {
            if (lista.Count > 1)
                QuickSort(lista, 0, lista.Count - 1, clave, asc);
        }

        public static void QuickSort<T>(List<T> lista, int bajo, int alto, Func<T, IComparable> clave, bool asc)
        {
            if (bajo < alto)
            {
                int p = Particionar(lista, bajo, alto, clave, asc);
                QuickSort(lista, bajo, p - 1, clave, asc);
                QuickSort(lista, p + 1, alto, clave, asc);
            }
        }

        private static int Particionar<T>(List<T> lista, int bajo, int alto, Func<T, IComparable> clave, bool asc)
        {
            T pivote = lista[alto];
            int i = bajo - 1;
            for (int j = bajo; j < alto; j++)
            {
                if (!FueraDeOrden(lista[j], pivote, clave, asc))
                {
                    i++;
                    Intercambiar(lista, i, j);
                }
            }
            Intercambiar(lista, i + 1, alto);
            return i + 1;
        }
    }
}
