using System;
using System.Collections.Generic;
using System.Text;

namespace Y2Snes.Core
{
    public class SuperFamicom
    {
        public Rom rom { get; private set; }
        public Cpu cpu { get; private set; }
        public Memory memory { get; private set; }

        public void PowerOn()
        {
            rom = new Rom("../../../../roms/Super Mario World.smc");

            cpu = new Cpu(this);
            memory = new Memory(this);

            cpu.Reset(rom.ResetVectorEM);
        }

        public void Step()
        {
            cpu.Step();
        }
    }
}
