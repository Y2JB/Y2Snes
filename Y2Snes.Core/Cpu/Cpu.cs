using System;
using System.Collections.Generic;
using System.Text;

namespace Y2Snes.Core
{
    // Snes uses a modified W65C816S 21.47Mhz
    public partial class Cpu
    {
        // Main registers
        public ushort A { get; set; }
        //public ushort B { get; set; }

        // Index registers
        public ushort X { get; set; }
        public ushort Y { get; set; }

        // Direct Page register
        public ushort DP { get; set; }

        // Data Bank register (8 bit)
        public byte DB { get; set; }

        // Flags register
        public byte P { get; set; }

        // Program Bank register (8 bit)
        public byte PB { get; set; }

        // Progrtam counter (16 bit)
        public ushort PC { get; set; }

        // Stack Pointer (16 bit)
        public ushort SP { get; set; }

        enum CpuFlag
        {
            Carry       = 1 << 0,       // C Carry(0=No Carry, 1=Carry)
            Zero        = 1 << 1,       // Z Zero(0=Nonzero, 1=Zero)
            IrqDisable  = 1 << 2,       // I IRQ Disable(0=IRQ Enable, 1=IRQ Disable)
            Decimal     = 1 << 3,       // D Decimal Mode(0=Normal, 1=BCD Mode for ADC/SBC opcodes)
            Break       = 1 << 4,       // X/B Break Flag(0=IRQ/NMI, 1=BRK/PHP opcode)  (0=16bit, 1=8bit)
            Unused      = 1 << 5,       // M/U Unused(Always 1)
            Overflow    = 1 << 6,       // V Overflow(0=No Overflow, 1=Overflow)
            Negative    = 1 << 7,       // N Negative/Sign(0=Positive, 1=Negative)
        }
        Instruction[] instructions = new Instruction[256];
        
        public Instruction GetInstruction(byte opcode) { return instructions[opcode]; }

        SuperFamicom system;


        public Cpu(SuperFamicom system)
        {
            this.system = system;
            RegisterInstructionHandlers();
        }


        public void Reset(ushort resetVector)
        {
            PC = system.rom.ResetVectorEM;
        }


        public void Step()
        {
            byte opCode = system.memory.ReadByte(0x00, PC++);

            var instruction = instructions[opCode];
            if (instruction == null || instruction.Handler == null)
            {
                throw new ArgumentException(String.Format("Unsupported instruction 0x{0:X2} {1}", opCode, instruction == null ? "-" : instruction.Name));
            }

            ushort operandValue = 0;
            if (instruction.OperandLength == 1)
            {
                operandValue = system.memory.ReadByte(0x00, PC);
            }
            else if (instruction.OperandLength == 2)
            {
                operandValue = system.memory.ReadShort(0x00, PC);
            }
            PC += instruction.OperandLength;

            instruction.Handler(operandValue);

        }


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
            P = (byte) CpuFlag.Unused;
        }

        public override String ToString()
        {
            return String.Format("A - 0x{0:X4}{1}X - 0x{2:X4}{3}Y - 0x{4:X4}{5}P - 0x{6:X2}{7}SP - 0x{8:X4}{9}PC - 0x({10:X2}){11:X4}{12}",
                A, Environment.NewLine, X, Environment.NewLine, Y, Environment.NewLine, P, Environment.NewLine, SP, Environment.NewLine, PB, PC, Environment.NewLine);
        }


        void RegisterInstructionHandlers()
        {
            instructions[0x78] = new Instruction("SEI", 0x78, 0, (v) => this.SEI());
            instructions[0x9C] = new Instruction("STZ 0x{0:X4}", 0x9C, 2, (v) => this.STZ(v));
            instructions[0xEA] = new Instruction("NOP", 0xEA, 0, (v) => this.NOP());
            

            // Check we don't have repeat id's (we made a type in the table above)
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
