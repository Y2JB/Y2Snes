using System;
using System.Runtime.CompilerServices;

namespace Y2Snes.Core
{
    public partial class Cpu
    {

        //10
        void BPL(sbyte n)
        {
            // Branch if Plus
            if (NegativeFlag == false)
            {
                int jmp = (int)PC + (int)n;
                PC = (ushort) jmp;
            }
            else
            {

            }
        }


        // 18
        void CLC()
        {
            ClearFlag(CpuFlag.Carry);
        }

        // 1B
        void TCS()
        {
            // Transfer 16-bit Accumulator to Stack Pointer
            SP = A;
        }

        // 38
        void SEC()
        {
            SetFlag(CpuFlag.Carry);
        }

        // 5B
        void TCD()
        {
            // Transfer 16-bit Accumulator to Direct Page Register
            D = A;
            SetZN(D);
        }

 
        // 78
        void SEI()
        {
            // Set Interrupt Disable Flag
            SetFlag(CpuFlag.IrqDisable);
        }

        
        // 98
        void TYA_8()
        {
            // Transfer Index Register Y to Accumulator
            AL = YL;
            SetZN(AL);
        }

        // 98
        void TYA_16()
        {
            A = Y;
            SetZN(A);
        }

        // A8
        void TAY_8()
        {
            // Transfer Accumulator to Index Register Y
            YL = AL;
            SetZN(YL);
        }

        // A8
        void TAY_16()
        {
            Y = A;
            SetZN(Y);
        }

        // 0xC2
        void REP(byte flags)
        {
            // Clears the bits specified in the operands of the flag
            P &= (byte)(~flags);

            if (XFlag)
            {
                XH = 0;
                YH = 0;
            }
        }

        // CA
        void DEX_8()
        {
            XL--;
            SetZN(XL);
        }

        // CA
        void DEX_16()
        {
            X--;
            SetZN(X);
        }

        // 0xE2
        void SEP(byte flags)
        {
            // Sets the bits specified in the operands of the flag
            P |= flags;

            if (XFlag)
            {
                XH = 0;
                YH = 0;
            }
        }

        // EA
        void NOP()
        {
        }


        // FB
        bool emulationModeChange = false;
        void XCE()
        {
            if(emulationModeChange)
            {
                throw new NotImplementedException("Changing back to emulation mdoe?");
            }
            emulationModeChange = true;

            // We take a shortcut here...Snes never re-enters emulation mode so we don't model the emulation flag and this is just a one way street
            SetFlag(CpuFlag.Carry);
        }
    }
}
