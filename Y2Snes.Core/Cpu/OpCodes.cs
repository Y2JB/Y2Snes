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
            // M1X1 - 8 bit A, X, Y
            /////////////////////////////////////////////////////////////////////////////////////////////////////////
            opCodesM1X1[0x08] = new Instruction("PHP",                  null,                   (v) => this.PHP());
            opCodesM1X1[0x10] = new Instruction("BPL 0x{0:X4}",         null,                   (n) => this.BPL((sbyte)n));
            opCodesM1X1[0x18] = new Instruction("CLC",                  null,                   (v) => this.CLC());
            opCodesM1X1[0x1B] = new Instruction("TCS",                  null,                   (v) => this.TCS());
            opCodesM1X1[0x20] = new Instruction("JSR 0x{0:X4}",         null,                   (nn) => this.JSR((ushort) nn));
            opCodesM1X1[0x38] = new Instruction("SEC",                  null,                   (v) => this.SEC());
            opCodesM1X1[0x5B] = new Instruction("TCD",                  null,                   (v) => this.TCD());
            opCodesM1X1[0x60] = new Instruction("RTS",                  null,                   (v) => this.RTS());
            opCodesM1X1[0x78] = new Instruction("SEI",                  null,                   (v) => this.SEI());
            opCodesM1X1[0x8D] = new Instruction("STA 0x{0:X4}",         null,                   (nn) => this.STA_8((ushort)nn));
            opCodesM1X1[0x8F] = new Instruction("STA 0x{0:X6}",         null,                   (nnn) => this.STA_8((uint)nnn));
            opCodesM1X1[0x98] = new Instruction("TYA",                  null,                   (v) => this.TYA_8());
            opCodesM1X1[0x9C] = new Instruction("STZ 0x{0:X4}",         null,                   (nn) => this.STZ_8((ushort) nn));
            opCodesM1X1[0x9F] = new Instruction("STA 0x{0:X6}, X",      AbsoluteLongIndexedX,   (nnn) => this.STA_8((uint)nnn));
            opCodesM1X1[0xA0] = new Instruction("LDY 0x{0:X2}",         null,                   (n) => this.LDY_8((byte)n));
            opCodesM1X1[0xA2] = new Instruction("LDX 0x{0:X2}",         null,                   (n) => this.LDX_8((byte)n));
            opCodesM1X1[0xA8] = new Instruction("TAY",                  null,                   (v) => this.TAY_8());
            opCodesM1X1[0xA9] = new Instruction("LDA 0x{0:X2}",         null,                   (n) => this.LDA_8((byte) n));
            opCodesM1X1[0xC2] = new Instruction("REP 0x{0:X2}",         null,                   (n) => this.REP((byte) n));
            opCodesM1X1[0xCA] = new Instruction("DEX",                  null,                   (n) => this.DEX_8());
            opCodesM1X1[0xE2] = new Instruction("SEP 0x{0:X2}",         null,                   (n) => this.SEP((byte)n));
            opCodesM1X1[0xE9] = new Instruction("SBC 0x{0:X2}",         null,                   (n) => this.SBC_8((byte)n));
            opCodesM1X1[0xEA] = new Instruction("NOP",                  null,                   (v) => this.NOP());
            opCodesM1X1[0xFB] = new Instruction("XCE",                  null,                   (v) => this.XCE());

            for(int i=0; i < 256; i++)
            {
                if (opCodesM1X1[i] != null)
                {
                    opCodesM1X1[i].OpCode = (byte) i;
                    opCodesM1X1[i].OperandLength = (byte) (OpLengthsM1X1[i] - 1);
                }
            }

            /////////////////////////////////////////////////////////////////////////////////////////////////////////
            // M0X0 - 16 bit A, X, Y
            /////////////////////////////////////////////////////////////////////////////////////////////////////////
            opCodesM0X0[0x08] = new Instruction("PHP",                  null,                   (v) => this.PHP());
            opCodesM0X0[0x10] = new Instruction("BPL 0x{0:X4}",         null,                   (n) => this.BPL((sbyte)n));
            opCodesM0X0[0x18] = new Instruction("CLC",                  null,                   (v) => this.CLC());
            opCodesM0X0[0x1B] = new Instruction("TCS",                  null,                   (v) => this.TCS());
            opCodesM0X0[0x20] = new Instruction("JSR 0x{0:X4}",         null,                   (nn) => this.JSR((ushort)nn));
            opCodesM0X0[0x38] = new Instruction("SEC",                  null,                   (v) => this.SEC());
            opCodesM0X0[0x5B] = new Instruction("TCD",                  null,                   (v) => this.TCD());
            opCodesM0X0[0x60] = new Instruction("RTS",                  null,                   (v) => this.RTS());
            opCodesM0X0[0x78] = new Instruction("SEI",                  null,                   (v) => this.SEI());
            opCodesM0X0[0x8D] = new Instruction("STA 0x{0:X4}",         null,                   (nn) => this.STA_16((ushort)nn));
            opCodesM0X0[0x8F] = new Instruction("STA 0x{0:X6}",         null,                   (nnn) => this.STA_16((uint)nnn));
            opCodesM0X0[0x98] = new Instruction("TYA",                  null,                   (v) => this.TYA_16());
            opCodesM0X0[0x9C] = new Instruction("STZ 0x{0:X4}",         null,                   (nn) => this.STZ_16((ushort) nn));
            opCodesM0X0[0x9F] = new Instruction("STA 0x{0:X6}, X",      AbsoluteLongIndexedX,   (nnn) => this.STA_16((uint)nnn));
            opCodesM0X0[0xA0] = new Instruction("LDY 0x{0:X4}",         null,                   (nn) => this.LDY_16((ushort)nn));
            opCodesM0X0[0xA2] = new Instruction("LDX 0x{0:X4}",         null,                   (nn) => this.LDX_16((ushort)nn));
            opCodesM0X0[0xA8] = new Instruction("TAY",                  null,                   (v) => this.TAY_16());
            opCodesM0X0[0xA9] = new Instruction("LDA 0x{0:X4}",         null,                   (nn) => this.LDA_16((ushort) nn));
            opCodesM0X0[0xC2] = new Instruction("REP 0x{0:X2}",         null,                   (n) => this.REP((byte)n));
            opCodesM0X0[0xCA] = new Instruction("DEX",                  null,                   (n) => this.DEX_16());
            opCodesM0X0[0xE2] = new Instruction("SEP 0x{0:X2}",         null,                   (n) => this.SEP((byte)n));
            opCodesM0X0[0xE9] = new Instruction("SBC 0x{0:X4}",         null,                   (nn) => this.SBC_16((ushort)nn));
            opCodesM0X0[0xEA] = new Instruction("NOP",                  null,                   (v) => this.NOP());
            opCodesM0X0[0xFB] = new Instruction("XCE",                  null,                   (v) => this.XCE());

            for (int i = 0; i < 256; i++)
            {
                if (opCodesM0X0[i] != null)
                {
                    opCodesM0X0[i].OpCode = (byte)i;
                    opCodesM0X0[i].OperandLength = (byte)(OpLengthsM0X0[i] - 1);
                }
            }
        }


        // How many clock cycles for each instuction.
        // M0X0 - 16 bit A, X, Y
        byte[] M0X0Ticks = new byte[256] {
//                                                0x0 0x1 0x2 0x3 0x4 0x5 0x6 0x7   0x8 0x9 0xA 0xB 0xC 0xD 0xE 0xF
                                      /*0x0*/     0,  0,  0,  0,  0,  0,  0,  0,    0,  0,  0,  0,  0,  0,  0,  0, 
                                      /*0x1*/     0,  0,  0,  0,  0,  0,  0,  0,    0,  0,  0,  0,  0,  0,  0,  0, 
                                      /*0x2*/     0,  0,  0,  0,  0,  0,  0,  0,    0,  0,  0,  0,  0,  0,  0,  0, 
                                      /*0x3*/     0,  0,  0,  0,  0,  0,  0,  0,    0,  0,  0,  0,  0,  0,  0,  0, 
                                      /*0x4*/     0,  0,  0,  0,  0,  0,  0,  0,    0,  0,  0,  0,  0,  0,  0,  0, 
                                      /*0x5*/     0,  0,  0,  0,  0,  0,  0,  0,    0,  0,  0,  0,  0,  0,  0,  0, 
                                      /*0x6*/     0,  0,  0,  0,  0,  0,  0,  0,    0,  0,  0,  0,  0,  0,  0,  0, 
                                      /*0x7*/     0,  0,  0,  0,  0,  0,  0,  0,    2,  0,  0,  0,  0,  0,  0,  0, 
                                      /*0x8*/     0,  0,  0,  0,  0,  0,  0,  0,    0,  0,  0,  0,  0,  0,  0,  0, 
                                      /*0x9*/     0,  0,  0,  0,  0,  0,  0,  0,    0,  0,  0,  0,  0,  0,  0,  0, 
                                      /*0xA*/     0,  0,  0,  0,  0,  0,  0,  0,    0,  0,  0,  0,  0,  0,  0,  0, 
                                      /*0xB*/     0,  0,  0,  0,  0,  0,  0,  0,    0,  0,  0,  0,  0,  0,  0,  0, 
                                      /*0xC*/     0,  0,  0,  0,  0,  0,  0,  0,    0,  0,  0,  0,  0,  0,  0,  0, 
                                      /*0xD*/     0,  0,  0,  0,  0,  0,  0,  0,    0,  0,  0,  0,  0,  0,  0,  0, 
                                      /*0xE*/     0,  0,  0,  0,  0,  0,  0,  0,    0,  0,  0,  0,  0,  0,  0,  0,
                                      /*0xF*/     0,  0,  0,  0,  0,  0,  0,  0,    0,  0,  0,  0,  0,  0,  0,  0  
	                                       };


        // M1X1 - 8 bit A, X, Y
        byte[] M1X1Ticks = new byte[256] {
//                                                0x0 0x1 0x2 0x3 0x4 0x5 0x6 0x7   0x8 0x9 0xA 0xB 0xC 0xD 0xE 0xF
                                      /*0x0*/     0,  0,  0,  0,  0,  0,  0,  0,    0,  0,  0,  0,  0,  0,  0,  0, 
                                      /*0x1*/     0,  0,  0,  0,  0,  0,  0,  0,    0,  0,  0,  0,  0,  0,  0,  0, 
                                      /*0x2*/     0,  0,  0,  0,  0,  0,  0,  0,    0,  0,  0,  0,  0,  0,  0,  0, 
                                      /*0x3*/     0,  0,  0,  0,  0,  0,  0,  0,    0,  0,  0,  0,  0,  0,  0,  0, 
                                      /*0x4*/     0,  0,  0,  0,  0,  0,  0,  0,    0,  0,  0,  0,  0,  0,  0,  0, 
                                      /*0x5*/     0,  0,  0,  0,  0,  0,  0,  0,    0,  0,  0,  0,  0,  0,  0,  0, 
                                      /*0x6*/     0,  0,  0,  0,  0,  0,  0,  0,    0,  0,  0,  0,  0,  0,  0,  0, 
                                      /*0x7*/     0,  0,  0,  0,  0,  0,  0,  0,    2,  0,  0,  0,  0,  0,  0,  0, 
                                      /*0x8*/     0,  0,  0,  0,  0,  0,  0,  0,    0,  0,  0,  0,  0,  0,  0,  0, 
                                      /*0x9*/     0,  0,  0,  0,  0,  0,  0,  0,    0,  0,  0,  0,  0,  0,  0,  0, 
                                      /*0xA*/     0,  0,  0,  0,  0,  0,  0,  0,    0,  0,  0,  0,  0,  0,  0,  0, 
                                      /*0xB*/     0,  0,  0,  0,  0,  0,  0,  0,    0,  0,  0,  0,  0,  0,  0,  0, 
                                      /*0xC*/     0,  0,  0,  0,  0,  0,  0,  0,    0,  0,  0,  0,  0,  0,  0,  0, 
                                      /*0xD*/     0,  0,  0,  0,  0,  0,  0,  0,    0,  0,  0,  0,  0,  0,  0,  0, 
                                      /*0xE*/     0,  0,  0,  0,  0,  0,  0,  0,    0,  0,  0,  0,  0,  0,  0,  0,
                                      /*0xF*/     0,  0,  0,  0,  0,  0,  0,  0,    0,  0,  0,  0,  0,  0,  0,  0
                                           };

        byte[] OpLengthsM0X0 = new byte[256]
{
        //  0  1  2  3  4  5  6  7  8  9  A  B  C  D  E  F
	        2, 2, 2, 2, 2, 2, 2, 2, 1, 3, 1, 1, 3, 3, 3, 4, // 0
	        2, 2, 2, 2, 2, 2, 2, 2, 1, 3, 1, 1, 3, 3, 3, 4, // 1
	        3, 2, 4, 2, 2, 2, 2, 2, 1, 3, 1, 1, 3, 3, 3, 4, // 2
	        2, 2, 2, 2, 2, 2, 2, 2, 1, 3, 1, 1, 3, 3, 3, 4, // 3
	        1, 2, 2, 2, 3, 2, 2, 2, 1, 3, 1, 1, 3, 3, 3, 4, // 4
	        2, 2, 2, 2, 3, 2, 2, 2, 1, 3, 1, 1, 4, 3, 3, 4, // 5
	        1, 2, 3, 2, 2, 2, 2, 2, 1, 3, 1, 1, 3, 3, 3, 4, // 6
	        2, 2, 2, 2, 2, 2, 2, 2, 1, 3, 1, 1, 3, 3, 3, 4, // 7
	        2, 2, 3, 2, 2, 2, 2, 2, 1, 3, 1, 1, 3, 3, 3, 4, // 8
	        2, 2, 2, 2, 2, 2, 2, 2, 1, 3, 1, 1, 3, 3, 3, 4, // 9
	        3, 2, 3, 2, 2, 2, 2, 2, 1, 3, 1, 1, 3, 3, 3, 4, // A
	        2, 2, 2, 2, 2, 2, 2, 2, 1, 3, 1, 1, 3, 3, 3, 4, // B
	        3, 2, 2, 2, 2, 2, 2, 2, 1, 3, 1, 1, 3, 3, 3, 4, // C
	        2, 2, 2, 2, 2, 2, 2, 2, 1, 3, 1, 1, 3, 3, 3, 4, // D
	        3, 2, 2, 2, 2, 2, 2, 2, 1, 3, 1, 1, 3, 3, 3, 4, // E
	        2, 2, 2, 2, 3, 2, 2, 2, 1, 3, 1, 1, 3, 3, 3, 4  // F
};

        byte[] OpLengthsM0X1 = new byte[256]
        {
        //  0  1  2  3  4  5  6  7  8  9  A  B  C  D  E  F
	        2, 2, 2, 2, 2, 2, 2, 2, 1, 3, 1, 1, 3, 3, 3, 4, // 0
	        2, 2, 2, 2, 2, 2, 2, 2, 1, 3, 1, 1, 3, 3, 3, 4, // 1
	        3, 2, 4, 2, 2, 2, 2, 2, 1, 3, 1, 1, 3, 3, 3, 4, // 2
	        2, 2, 2, 2, 2, 2, 2, 2, 1, 3, 1, 1, 3, 3, 3, 4, // 3
	        1, 2, 2, 2, 3, 2, 2, 2, 1, 3, 1, 1, 3, 3, 3, 4, // 4
	        2, 2, 2, 2, 3, 2, 2, 2, 1, 3, 1, 1, 4, 3, 3, 4, // 5
	        1, 2, 3, 2, 2, 2, 2, 2, 1, 3, 1, 1, 3, 3, 3, 4, // 6
	        2, 2, 2, 2, 2, 2, 2, 2, 1, 3, 1, 1, 3, 3, 3, 4, // 7
	        2, 2, 3, 2, 2, 2, 2, 2, 1, 3, 1, 1, 3, 3, 3, 4, // 8
	        2, 2, 2, 2, 2, 2, 2, 2, 1, 3, 1, 1, 3, 3, 3, 4, // 9
	        2, 2, 2, 2, 2, 2, 2, 2, 1, 3, 1, 1, 3, 3, 3, 4, // A
	        2, 2, 2, 2, 2, 2, 2, 2, 1, 3, 1, 1, 3, 3, 3, 4, // B
	        2, 2, 2, 2, 2, 2, 2, 2, 1, 3, 1, 1, 3, 3, 3, 4, // C
	        2, 2, 2, 2, 2, 2, 2, 2, 1, 3, 1, 1, 3, 3, 3, 4, // D
	        2, 2, 2, 2, 2, 2, 2, 2, 1, 3, 1, 1, 3, 3, 3, 4, // E
	        2, 2, 2, 2, 3, 2, 2, 2, 1, 3, 1, 1, 3, 3, 3, 4  // F
        };

        byte[] OpLengthsM1X0 = new byte[256]
        {
        //  0  1  2  3  4  5  6  7  8  9  A  B  C  D  E  F
	        2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 1, 1, 3, 3, 3, 4, // 0
	        2, 2, 2, 2, 2, 2, 2, 2, 1, 3, 1, 1, 3, 3, 3, 4, // 1
	        3, 2, 4, 2, 2, 2, 2, 2, 1, 2, 1, 1, 3, 3, 3, 4, // 2
	        2, 2, 2, 2, 2, 2, 2, 2, 1, 3, 1, 1, 3, 3, 3, 4, // 3
	        1, 2, 2, 2, 3, 2, 2, 2, 1, 2, 1, 1, 3, 3, 3, 4, // 4
	        2, 2, 2, 2, 3, 2, 2, 2, 1, 3, 1, 1, 4, 3, 3, 4, // 5
	        1, 2, 3, 2, 2, 2, 2, 2, 1, 2, 1, 1, 3, 3, 3, 4, // 6
	        2, 2, 2, 2, 2, 2, 2, 2, 1, 3, 1, 1, 3, 3, 3, 4, // 7
	        2, 2, 3, 2, 2, 2, 2, 2, 1, 2, 1, 1, 3, 3, 3, 4, // 8
	        2, 2, 2, 2, 2, 2, 2, 2, 1, 3, 1, 1, 3, 3, 3, 4, // 9
	        3, 2, 3, 2, 2, 2, 2, 2, 1, 2, 1, 1, 3, 3, 3, 4, // A
	        2, 2, 2, 2, 2, 2, 2, 2, 1, 3, 1, 1, 3, 3, 3, 4, // B
	        3, 2, 2, 2, 2, 2, 2, 2, 1, 2, 1, 1, 3, 3, 3, 4, // C
	        2, 2, 2, 2, 2, 2, 2, 2, 1, 3, 1, 1, 3, 3, 3, 4, // D
	        3, 2, 2, 2, 2, 2, 2, 2, 1, 2, 1, 1, 3, 3, 3, 4, // E
	        2, 2, 2, 2, 3, 2, 2, 2, 1, 3, 1, 1, 3, 3, 3, 4  // F
        };

        byte[] OpLengthsM1X1 = new byte[256]
        {
        //  0  1  2  3  4  5  6  7  8  9  A  B  C  D  E  F
	        2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 1, 1, 3, 3, 3, 4, // 0
	        2, 2, 2, 2, 2, 2, 2, 2, 1, 3, 1, 1, 3, 3, 3, 4, // 1
	        3, 2, 4, 2, 2, 2, 2, 2, 1, 2, 1, 1, 3, 3, 3, 4, // 2
	        2, 2, 2, 2, 2, 2, 2, 2, 1, 3, 1, 1, 3, 3, 3, 4, // 3
	        1, 2, 2, 2, 3, 2, 2, 2, 1, 2, 1, 1, 3, 3, 3, 4, // 4
	        2, 2, 2, 2, 3, 2, 2, 2, 1, 3, 1, 1, 4, 3, 3, 4, // 5
	        1, 2, 3, 2, 2, 2, 2, 2, 1, 2, 1, 1, 3, 3, 3, 4, // 6
	        2, 2, 2, 2, 2, 2, 2, 2, 1, 3, 1, 1, 3, 3, 3, 4, // 7
	        2, 2, 3, 2, 2, 2, 2, 2, 1, 2, 1, 1, 3, 3, 3, 4, // 8
	        2, 2, 2, 2, 2, 2, 2, 2, 1, 3, 1, 1, 3, 3, 3, 4, // 9
	        2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 1, 1, 3, 3, 3, 4, // A
	        2, 2, 2, 2, 2, 2, 2, 2, 1, 3, 1, 1, 3, 3, 3, 4, // B
	        2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 1, 1, 3, 3, 3, 4, // C
	        2, 2, 2, 2, 2, 2, 2, 2, 1, 3, 1, 1, 3, 3, 3, 4, // D
	        2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 1, 1, 3, 3, 3, 4, // E
	        2, 2, 2, 2, 3, 2, 2, 2, 1, 3, 1, 1, 3, 3, 3, 4  // F
        };

 
    }
}
