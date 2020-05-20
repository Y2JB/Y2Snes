using System;
using System.Runtime.CompilerServices;

namespace Y2Snes.Core
{
    public partial class Cpu
    {


        // ********************
        // Instruction Handlers 
        // ********************

        // 0x78
        void SEI()
        {
            // Set Interrupt Disable Flag
            SetFlag(CpuFlag.IrqDisable);
        }

        // 0x9C
        void STZ(ushort nn)
        {
            // TODO: Bank?!?
            system.memory.WriteShort(0, nn, 0);
        }

        // 0xEA
        void NOP()
        {
        }

      
    }
}
