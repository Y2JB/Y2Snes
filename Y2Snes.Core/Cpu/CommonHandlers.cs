using System;
using System.Collections.Generic;
using System.Text;

namespace Y2Snes.Core
{
    public partial class Cpu
    {
 
        void LDA_16(ushort nn)
        {
            A = nn;
            SetZN(A);
        }

 
        void LDA_8(byte n)
        {
            AL = n;
            SetZN(AL);
        }


        void STZ_8(uint address)
        {
            MemoryAbsolute.WriteByte(address, 0x00);          
        }

        void STZ_16(uint address)
        {
            MemoryAbsolute.WriteShort(address, 0x0000);
        }


        // Store Accumulator to Memory
        void STA_8(uint address)
        {
            MemoryAbsolute.WriteByte(address, AL);
        }

        void STA_16(uint address)
        {
            MemoryAbsolute.WriteShort(address, A);
        }

    }
}
