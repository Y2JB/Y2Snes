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
            opCodesM1X1[0x10] = new Instruction("BPL 0x{0:X4}", 0x10, 1, null, (n) => this.BPL((sbyte)n));
            opCodesM1X1[0x18] = new Instruction("CLC", 0x18, 0, null, (v) => this.CLC());
            opCodesM1X1[0x1B] = new Instruction("TCS", 0x1B, 0, null, (v) => this.TCS());
            opCodesM1X1[0x38] = new Instruction("SEC", 0x38, 0, null, (v) => this.SEC());
            opCodesM1X1[0x5B] = new Instruction("TCD", 0x5B, 0, null, (v) => this.TCD());
            opCodesM1X1[0x78] = new Instruction("SEI", 0x78, 0, null, (v) => this.SEI());

            opCodesM1X1[0x8D] = new Instruction("STA 0x{0:X4}", 0x8D, 2, null, (nn) => this.STA_8((ushort)nn));
            opCodesM1X1[0x8F] = new Instruction("STA 0x{0:X6}", 0x8F, 3, null, (nnn) => this.STA_8((uint)nnn));

            opCodesM1X1[0x98] = new Instruction("TYA", 0x98, 0, null, (v) => this.TYA_8());

            opCodesM1X1[0x9C] = new Instruction("STZ 0x{0:X4}", 0x9C, 2, null, (nn) => this.STZ_8((ushort) nn));

            opCodesM1X1[0x9F] = new Instruction("STA 0x{0:X6}, X", 0x9F, 3, AbsoluteLongIndexedX, (nnn) => this.STA_8((uint)nnn));

            opCodesM1X1[0xA0] = new Instruction("LDY 0x{0:X2}", 0xA0, 1, null, (n) => this.LDY_8((byte)n));

            opCodesM1X1[0xA2] = new Instruction("LDX 0x{0:X2}", 0xA2, 1, null, (n) => this.LDX_8((byte)n));

            opCodesM1X1[0xA8] = new Instruction("TAY", 0xA8, 0, null, (v) => this.TAY_8());

            opCodesM1X1[0xA9] = new Instruction("LDA 0x{0:X2}", 0xA9, 1, null, (n) => this.LDA_8((byte) n));

            opCodesM1X1[0xC2] = new Instruction("REP 0x{0:X2}", 0xC2, 1, null, (n) => this.REP((byte) n));

            opCodesM1X1[0xCA] = new Instruction("DEX", 0xCA, 0, null, (n) => this.DEX_8());

            opCodesM1X1[0xE2] = new Instruction("SEP 0x{0:X2}", 0xE2, 1, null, (n) => this.SEP((byte)n));
            opCodesM1X1[0xE9] = new Instruction("SBC 0x{0:X2}", 0xE9, 1, null, (n) => this.SBC_8((byte)n));

            opCodesM1X1[0xEA] = new Instruction("NOP", 0xEA, 0, null, (v) => this.NOP());
            opCodesM1X1[0xFB] = new Instruction("XCE", 0xFB, 0, null, (v) => this.XCE());


            /////////////////////////////////////////////////////////////////////////////////////////////////////////
            // M0X0 - 16 bit A, X, Y
            /////////////////////////////////////////////////////////////////////////////////////////////////////////
            opCodesM0X0[0x10] = new Instruction("BPL 0x{0:X4}", 0x10, 1, null, (n) => this.BPL((sbyte)n));
            opCodesM0X0[0x18] = new Instruction("CLC", 0x18, 0, null, (v) => this.CLC());
            opCodesM0X0[0x1B] = new Instruction("TCS", 0x1B, 0, null, (v) => this.TCS());
            opCodesM0X0[0x38] = new Instruction("SEC", 0x38, 0, null, (v) => this.SEC());
            opCodesM0X0[0x5B] = new Instruction("TCD", 0x5B, 0, null, (v) => this.TCD());
            opCodesM0X0[0x78] = new Instruction("SEI", 0x78, 0, null, (v) => this.SEI());

            opCodesM0X0[0x8D] = new Instruction("STA 0x{0:X4}", 0x8D, 2, null, (nn) => this.STA_16((ushort)nn));
            opCodesM0X0[0x8F] = new Instruction("STA 0x{0:X6}", 0x8F, 3, null, (nnn) => this.STA_16((uint)nnn));

            opCodesM0X0[0x98] = new Instruction("TYA", 0x98, 0, null, (v) => this.TYA_16());

            opCodesM0X0[0x9C] = new Instruction("STZ 0x{0:X4}", 0x9C, 2, null, (nn) => this.STZ_16((ushort) nn));

            opCodesM0X0[0x9F] = new Instruction("STA 0x{0:X6}, X", 0x9F, 3, AbsoluteLongIndexedX, (nnn) => this.STA_16((uint)nnn));

            opCodesM0X0[0xA0] = new Instruction("LDY 0x{0:X4}", 0xA0, 2, null, (nn) => this.LDY_16((ushort)nn));

            opCodesM0X0[0xA2] = new Instruction("LDX 0x{0:X4}", 0xA2, 2, null, (nn) => this.LDX_16((ushort)nn));

            opCodesM0X0[0xA8] = new Instruction("TAY", 0xA8, 0, null, (v) => this.TAY_16());

            opCodesM0X0[0xA9] = new Instruction("LDA 0x{0:X4}", 0xA9, 2, null, (nn) => this.LDA_16((ushort) nn));

            opCodesM0X0[0xC2] = new Instruction("REP 0x{0:X2}", 0xC2, 1, null, (n) => this.REP((byte)n));

            opCodesM0X0[0xCA] = new Instruction("DEX", 0xCA, 0, null, (n) => this.DEX_16());

            opCodesM0X0[0xE2] = new Instruction("SEP 0x{0:X2}", 0xE2, 1, null, (n) => this.SEP((byte)n));

            opCodesM0X0[0xE9] = new Instruction("SBC 0x{0:X4}", 0xE9, 2, null, (nn) => this.SBC_16((ushort)nn));

            opCodesM0X0[0xEA] = new Instruction("NOP", 0xEA, 0, null, (v) => this.NOP());
            opCodesM0X0[0xFB] = new Instruction("XCE", 0xFB, 0, null, (v) => this.XCE());




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
