using System;
using System.Runtime.CompilerServices;

namespace Y2Snes.Core
{
    public partial class Cpu
    {


        // ********************
        // Instruction Handlers 
        // ********************


        // 0x18
        void CLC()
        {
            ClearFlag(CpuFlag.Carry);
        }

        // 0x1B
        void TCS()
        {
            // Transfer 16-bit Accumulator to Stack Pointer
            SP = A;
        }

        // 0x5B
        void TCD()
        {
            // Transfer 16-bit Accumulator to Direct Page Register
            D = A;
            SetZN(D);
        }

 
        // 0x78
        void SEI()
        {
            // Set Interrupt Disable Flag
            SetFlag(CpuFlag.IrqDisable);
        }


        // 0xC2
        void REP(byte flags)
        {
            // Clears the bits specified in the operands of the flag
            P &= (byte)(~flags);            
        }


        // 0xEA
        void NOP()
        {
        }


        // 0xFB
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
