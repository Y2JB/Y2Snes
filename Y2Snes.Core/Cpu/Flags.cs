using System;
using System.Collections.Generic;
using System.Text;

namespace Y2Snes.Core
{
    public partial class Cpu
    {     
        enum CpuFlag
        {
            Carry       = 1 << 0,       // C Carry(0=No Carry, 1=Carry)
            Zero        = 1 << 1,       // Z Zero(0=Nonzero, 1=Zero)
            IrqDisable  = 1 << 2,       // I IRQ Disable(0=IRQ Enable, 1=IRQ Disable)
            Decimal     = 1 << 3,       // D Decimal Mode(0=Normal, 1=BCD Mode for ADC/SBC opcodes)
            X           = 1 << 4,       // Index register size (native mode only) (0 = 16-bit, 1 = 8-bit)
            M           = 1 << 5,       // Accumulator register size (native mode only) (0 = 16-bit, 1 = 8-bit)
            Overflow    = 1 << 6,       // V Overflow(0=No Overflow, 1=Overflow)
            Negative    = 1 << 7,       // N Negative/Sign(0=Positive, 1=Negative)
        }

        bool CarryFlag { get { return IsFlagSet(CpuFlag.Carry);  } }
        bool ZeroFlag { get  { return IsFlagSet(CpuFlag.Zero); } }
        bool IrqDisableFlag { get  { return IsFlagSet(CpuFlag.IrqDisable); } }
        bool DecimalFlag { get  { return IsFlagSet(CpuFlag.Decimal); } }
        bool XFlag { get  { return IsFlagSet(CpuFlag.X); } }
        bool MFlag { get  { return IsFlagSet(CpuFlag.M); } }
        bool OverflowFlag { get  { return IsFlagSet(CpuFlag.Overflow); } }
        bool NegativeFlag { get  { return IsFlagSet(CpuFlag.Negative); } }



        void SetFlag(CpuFlag flag)
        {
            P |= (byte)flag;
        }


        void ClearFlag(CpuFlag flag)
        {
            P &= (byte)~((byte)flag);
        }

        void ClearAllFlags()
        {
            P = 0;
        }
        
        bool IsFlagSet(CpuFlag flag)
        {
            return ((P & ((byte)flag)) != 0);
        }

        void SetZN(byte n)
        {
            ClearFlag(CpuFlag.Zero);
            if (n == 0) SetFlag(CpuFlag.Zero);

            ClearFlag(CpuFlag.Negative);
            if ((n & 0x80) != 0) SetFlag(CpuFlag.Negative);
        }

        void SetZN(ushort nn)
        {
            ClearFlag(CpuFlag.Zero);
            if (nn == 0) SetFlag(CpuFlag.Zero);

            ClearFlag(CpuFlag.Negative);
            if(((nn >> 8) & 0x80) != 0) SetFlag(CpuFlag.Negative);
        }


    }
}
