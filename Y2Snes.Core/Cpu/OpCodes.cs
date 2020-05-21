using System;
using System.Collections.Generic;
using System.Text;

namespace Y2Snes.Core
{
    public partial class Cpu
    {
        // We use seperate handlers depending on the CPU M&X flags             
        Instruction[] opCodesM0X0 = new Instruction[256];           // 16 bit A, X, Y
        Instruction[] opCodesM1X1 = new Instruction[256];           // 8 bit A, X, Y

        public Instruction GetInstruction(byte opcode) 
        { 
            if(IsFlagSet(CpuFlag.M))
            {
                if(IsFlagSet(CpuFlag.X))
                {
                    return opCodesM1X1[opcode];
                }
                throw new NotImplementedException("M1X0 missing");
            }
            else if(IsFlagSet(CpuFlag.X))
            {
                throw new NotImplementedException("M0X1 missing");              
            }
            return opCodesM0X0[opcode]; 
        }


        void RegisterInstructionHandlers()
        {



            /////////////////////////////////////////////////////////////////////////////////////////////////////////
            // M0X0 - 8 bit A, X, Y
            /////////////////////////////////////////////////////////////////////////////////////////////////////////
            opCodesM1X1[0x18] = new Instruction("CLC", 0x18, 0, (v) => this.CLC());
            opCodesM1X1[0x1B] = new Instruction("TCS", 0x1B, 0, (v) => this.TCS());
            opCodesM1X1[0x5B] = new Instruction("TCD", 0x5B, 0, (v) => this.TCD());
            opCodesM1X1[0x78] = new Instruction("SEI", 0x78, 0, (v) => this.SEI());

            opCodesM1X1[0x8D] = new Instruction("STA 0x{0:X4}", 0x8D, 2, (nn) => this.STA_8((ushort)nn));
            opCodesM1X1[0x8F] = new Instruction("STA 0x{0:X6}", 0x8F, 3, (nnn) => this.STA_8((uint)nnn));

            opCodesM1X1[0x9C] = new Instruction("STZ 0x{0:X4}", 0x9C, 2, (nn) => this.STZ_8((ushort) nn));

            opCodesM1X1[0xA9] = new Instruction("LDA 0x{0:X2}", 0xA9, 1, (n) => this.LDA_8((byte) n));

            opCodesM1X1[0xC2] = new Instruction("REP", 0xC2, 1, (n) => this.REP((byte) n));
            opCodesM1X1[0xEA] = new Instruction("NOP", 0xEA, 0, (v) => this.NOP());
            opCodesM1X1[0xFB] = new Instruction("XCE", 0xFB, 0, (v) => this.XCE());


            /////////////////////////////////////////////////////////////////////////////////////////////////////////
            // M0X0 - 16 bit A, X, Y
            /////////////////////////////////////////////////////////////////////////////////////////////////////////
            opCodesM0X0[0x18] = new Instruction("CLC", 0x18, 0, (v) => this.CLC());
            opCodesM0X0[0x1B] = new Instruction("TCS", 0x1B, 0, (v) => this.TCS());            
            opCodesM0X0[0x5B] = new Instruction("TCD", 0x5B, 0, (v) => this.TCD());
            opCodesM0X0[0x78] = new Instruction("SEI", 0x78, 0, (v) => this.SEI());

            opCodesM0X0[0x8D] = new Instruction("STA 0x{0:X4}", 0x8D, 2, (nn) => this.STA_16((ushort)nn));
            opCodesM0X0[0x8F] = new Instruction("STA 0x{0:X6}", 0x8F, 3, (nnn) => this.STA_16((uint)nnn));

            opCodesM0X0[0x9C] = new Instruction("STZ 0x{0:X4}", 0x9C, 2, (nn) => this.STZ_16((ushort) nn));

            opCodesM0X0[0xA9] = new Instruction("LDA 0x{0:X4}", 0xA9, 2, (nn) => this.LDA_16((ushort) nn));

            opCodesM1X1[0xC2] = new Instruction("REP", 0xC2, 1, (n) => this.REP((byte)n));
            opCodesM0X0[0xEA] = new Instruction("NOP", 0xEA, 0, (v) => this.NOP());
            opCodesM0X0[0xFB] = new Instruction("XCE", 0xFB, 0, (v) => this.XCE());




            CheckInstructions(opCodesM0X0);
            CheckInstructions(opCodesM1X1);
        }

        void CheckInstructions(Instruction[] instructions)
        {
            // Check we don't have repeat id's (we made a type in the tables above)
            for (int i = 0; i < 255; i++)
            {
                var instruction = instructions[i];
                if (instruction == null) continue;

                if (instruction.OpCode != i)
                {
                    throw new ArgumentException("Bad opcode");
                }
                for (int j = 0; j < 255; j++)
                {
                    if (i == j) continue;
                    var rhs = instructions[j];
                    if (rhs == null) continue;

                    if (instruction.OpCode == rhs.OpCode)
                    {
                        throw new ArgumentException("Bad opcode");
                    }
                }
            }
        }
    }
}
