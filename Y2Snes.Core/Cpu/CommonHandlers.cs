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

        void LDY_16(ushort n)
        {
            Y = n;
            SetZN(Y);
        }

        void LDY_8(byte n)
        {
            YL = n;
            SetZN(YL);
        }

        void LDX_16(ushort n)
        {
            X = n;
            SetZN(X);
        }

        void LDX_8(byte n)
        {
            XL = n;
            SetZN(XL);
        }

        void STZ_16(uint address)
        {
            MemoryAbsolute.WriteShort(address, 0x0000);
        }

        void STZ_8(uint address)
        {
            MemoryAbsolute.WriteByte(address, 0x00);          
        }

        // Store Accumulator to Memory
        void STA_16(uint address)
        {
            MemoryAbsolute.WriteShort(address, A);
        }
        
        void STA_8(uint address)
        {
            MemoryAbsolute.WriteByte(address, AL);
        }


        // Subtract with Borrow from Accumulator
        void SBC_16(ushort Work16)
        {
            if (DecimalFlag)
            {
                int result;
                int carry = (CarryFlag ? 1 : 0);

                Work16 ^= 0xFFFF;

                result = (A & 0x000F) + (Work16 & 0x000F) + carry;
                if (result < 0x0010)
                    result -= 0x0006;
                carry = ((result > 0x000F) ? 1 : 0);

                result = (A & 0x00F0) + (Work16 & 0x00F0) + (result & 0x000F) + carry * 0x10;
                if (result < 0x0100)
                    result -= 0x0060;
                carry = ((result > 0x00FF) ? 1 : 0);

                result = (A & 0x0F00) + (Work16 & 0x0F00) + (result & 0x00FF) + carry * 0x100;
                if (result < 0x1000)
                    result -= 0x0600;
                carry = ((result > 0x0FFF) ? 1 : 0);

                result = (A & 0xF000) + (Work16 & 0xF000) + (result & 0x0FFF) + carry * 0x1000;

                if (((A ^ Work16) & 0x8000) == 0 && (((A ^ result) & 0x8000)!= 0))
                    SetFlag(CpuFlag.Overflow);
                else
                    ClearFlag(CpuFlag.Overflow);

                if (result < 0x10000)
                    result -= 0x6000;

                if (result > 0xFFFF)
                    SetFlag(CpuFlag.Carry);
                else
                    ClearFlag(CpuFlag.Carry);

                A = (ushort) (result & 0xFFFF);
                SetZN(A);
            }
            else
            {
                Int32 int32 = (Int32)A - (Int32)Work16 + (Int32)((CarryFlag ? 1 : 0) - 1);

                if (int32 >= 0)
                    SetFlag(CpuFlag.Carry);
                else
                    ClearFlag(CpuFlag.Carry);

                if (((A ^ Work16) & (A ^ (UInt16)int32) & 0x8000) != 0)
                    SetFlag(CpuFlag.Overflow);
                else
                    ClearFlag(CpuFlag.Overflow);

                A = (UInt16)int32;
                SetZN(A);
            }
        }

        void SBC_8(byte Work8)
        {
            if (DecimalFlag)
            {
                int result;
                int carry = (CarryFlag ? 1 : 0);

                Work8 ^= 0xFF;

                result = (AL & 0x0F) + (Work8 & 0x0F) + carry;
                if (result < 0x10)
                    result -= 0x06;
                carry = ((result > 0x0F) ? 1 : 0);

                result = (AL & 0xF0) + (Work8 & 0xF0) + (result & 0x0F) + carry * 0x10;

                if ((AL & 0x80) == (Work8 & 0x80) && (AL & 0x80) != (result & 0x80))
                    SetFlag(CpuFlag.Overflow);
                else
                    ClearFlag(CpuFlag.Overflow);

                if (result < 0x100)
                    result -= 0x60;

                if (result > 0xFF)
                    SetFlag(CpuFlag.Carry);
                else
                    ClearFlag(CpuFlag.Carry);

                AL = (byte) (result & 0xFF);
                SetZN(AL);
            }
            else
            {
                short int16 = (short)((short)AL - (short)Work8 + (short)((CarryFlag ? 1 : 0) - 1));

                if(int16 >= 0)
                    SetFlag(CpuFlag.Carry);
                else
                    ClearFlag(CpuFlag.Carry);

                if (((AL ^ Work8) & (AL ^ (byte)int16) & 0x80) != 0)
                    SetFlag(CpuFlag.Overflow);
                else
                    ClearFlag(CpuFlag.Overflow);

                AL = (byte)int16;
                SetZN(AL);
            }
        }
    }
}
