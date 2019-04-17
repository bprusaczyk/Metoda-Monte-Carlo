using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MonteCarlo
{
    class Program
    {
        private static double promien;
        private static long liczbaLosowan;
        private static int liczbaWatkow;
        private static long liczbaWszystkichTrafien;
        private static readonly object blokada = new object();
        private static double poleKola;
        private static double poleKwadratu;

        static void Main(string[] args)
        {
            wczytajPromien:
            Console.WriteLine("Podaj promień koła:");
            try
            {
                promien = Convert.ToDouble(Console.ReadLine());
            }
            catch(FormatException e)
            {
                Console.WriteLine(e.Message);
                goto wczytajPromien;
            }
            wczytajN:
            Console.WriteLine("Podaj liczbę wylosowanych punktów:");
            try
            {
                liczbaLosowan = Convert.ToInt64(Console.ReadLine());
            }
            catch (FormatException e)
            {
                Console.WriteLine(e.Message);
                goto wczytajN;
            }
            wczytajWatki:
            Console.WriteLine("Podaj liczbę wątków:");
            try
            {
                liczbaWatkow = Convert.ToInt32(Console.ReadLine());
            }
            catch (FormatException e)
            {
                Console.WriteLine(e.Message);
                goto wczytajWatki;
            }
            Console.WriteLine("Czekaj...");
            long start = Environment.TickCount;
            Thread[] watki = new Thread[liczbaWatkow];
            EventWaitHandle[] semafory = new EventWaitHandle[liczbaWatkow];
            for (long i = 0; i < liczbaWatkow; i++)
            {
                semafory[i] = new AutoResetEvent(false);
            }
            long licznik = 0;
            for (long i = 0; i < liczbaWatkow; i++)
            {
                watki[i] = new Thread((ileLosowan) =>
                {
                    long liczbaTrafien = 0;
                    for (int j = 0; j < (long)ileLosowan; j++)
                    {
                        double x = new Random().NextDouble() * 2 * promien - promien;
                        double y = new Random().NextDouble() * 2 * promien - promien;
                        if (x * x + y * y <= promien * promien)
                        {
                            liczbaTrafien++;
                        }
                    }
                    lock(blokada)
                    {
                        liczbaWszystkichTrafien += liczbaTrafien;
                    }
                    long indeks = licznik++;
                    semafory[indeks].Set();
                });
            }
            bool pierwszy = true;
            foreach (Thread watek in watki)
            {
                if (pierwszy)
                {
                    pierwszy = false;
                    watek.Start((liczbaLosowan / liczbaWatkow) + (liczbaLosowan % liczbaWatkow));
                }
                else
                {
                    watek.Start(liczbaLosowan / liczbaWatkow);
                }
            }
            foreach (EventWaitHandle semafor in semafory)
            {
                semafor.WaitOne();
            }
            poleKwadratu = 4 * promien * promien;
            poleKola = poleKwadratu * ((double)liczbaWszystkichTrafien / (double)liczbaLosowan);
            Console.WriteLine("Pole koła o promieniu " + promien.ToString() + " wynosi " + poleKola + ".");
            long stop = Environment.TickCount;
            Console.WriteLine("Obliczenia trwały " + (stop - start) + " ms.");
            Console.Read();
        }
    }
}
