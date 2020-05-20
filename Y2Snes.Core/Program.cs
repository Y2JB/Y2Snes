using System;

namespace Y2Snes.Core
{
    class Program
    {
        
        static void Main(string[] args)
        {
            SuperFamicom snes = new SuperFamicom();

            snes.PowerOn();

            Console.WriteLine("Hello World!");
        }
    }
}
