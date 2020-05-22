using System;
using System.Collections.Generic;
using System.Text;

namespace Y2Snes.Core
{
    public class SuperFamicom
    {
        public Rom rom { get; private set; }
        public Cpu cpu { get; private set; }
        public Ppu ppu { get; private set; }
        public Memory memory { get; private set; }
        public LoRomMemoryMap MemoryMap { get; private set; }


        public void PowerOn()
        {
            rom = new Rom("../../../../roms/Super Mario World.smc");
            
            
            memory = new Memory(this);
            MemoryMap = new LoRomMemoryMap(memory);
            cpu = new Cpu(this);
            ppu = new Ppu(this);

            cpu.Reset(rom.ResetVectorEM);
            ppu.Reset();
        }


        public void Step()
        {
            cpu.Step();
            ppu.Step();
        }
    }
}
