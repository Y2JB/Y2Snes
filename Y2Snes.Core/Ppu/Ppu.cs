using System;
using System.Collections.Generic;
using System.Text;

namespace Y2Snes.Core
{
    public partial class Ppu
    {
        public const int Screen_X_Resolution = 256;
        public const int Screen_Y_Resolution = 224;

        public const int First_Visible_Line = 1;

        UInt32 lastCpuTickCount;
        UInt32 elapsedTicks;

        SuperFamicom snes;

     
        public void Ppu(SuperFamicom snes)
        {
            this.snes = snes;
        }


        public void Reset()
        {

        }


        // The PPU takes 2 master clocks to output each half-pixel. (Hires and pseudo-hires change the RGB output every half-pixel.)
        public void Step()
        {
            UInt32 cpuTickCount = snes.cpu.Ticks;
            UInt32 tickCount = (cpuTickCount - lastCpuTickCount);
            // Track how many cycles the CPU has done since we last changed states
            elapsedTicks += tickCount;
            lastCpuTickCount = cpuTickCount;
        }


    }
}
