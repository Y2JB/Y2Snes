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
        public byte AL { get { return (byte)(A & 0x00FF); } set { A = (ushort)((A & 0xFF00) | value); } }
        public byte AH { get { return (byte)((A & 0xFF00) >> 8); } }

        // Index registers
        public ushort X { get; set; }
        public ushort Y { get; set; }

        // Direct Page (Zero Page) register
        public ushort D { get; set; }

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

        public IAbsoluteMemoryReaderWriter MemoryAbsolute { get; private set; }

        SuperFamicom system;
        IBankedMemoryReaderWriter memoryMap;
        

        public Cpu(SuperFamicom system)
        {
            this.system = system;
            memoryMap = system.MemoryMap;

            MemoryAbsolute = new AbsoluteMemoryReaderWriter(memoryMap);

            RegisterInstructionHandlers();
        }


        public void Reset(ushort resetVector)
        {
            PC = system.rom.ResetVectorEM;
            SP = 0x00;

            // Acc, X & Y all start in 8 bit mode
            SetFlag(CpuFlag.M);
            SetFlag(CpuFlag.X);
            SetFlag(CpuFlag.IrqDisable);
        }


        public void Step()
        {
            byte opCode = memoryMap.ReadByte(PB, PC++);

            var instruction = GetInstruction(opCode);
            if (instruction == null || instruction.Handler == null)
            {
                throw new ArgumentException(String.Format("Unsupported instruction 0x{0:X2} {1}", opCode, instruction == null ? "-" : instruction.Name));
            }

            uint operandValue = 0;
            if (instruction.OperandLength == 1)
            {
                operandValue = memoryMap.ReadByte(PB, PC);
            }
            else if (instruction.OperandLength == 2)
            {
                operandValue = memoryMap.ReadShort(PB, PC);
            }
            else if (instruction.OperandLength == 3)
            {
                operandValue = memoryMap.ReadLong(PB, PC);
            }
            PC += instruction.OperandLength;

            // TODO: or can openbus just be set in memory.read?
            // OpenBus = foo

            instruction.Handler(operandValue);
        }


        public override String ToString()
        {
            string flags = String.Format("{0}{1}{2}{3}{4}{5}{6}{7}",   CarryFlag ? "C" : "-", ZeroFlag ? "Z" : "-", IrqDisableFlag ? "I" : "-", DecimalFlag ? "D" : "-",
                                                                    XFlag ? "X" : "-", MFlag ? "M" : "-", OverflowFlag ? "V" : "-", NegativeFlag ? "N" : "-");


            return String.Format("PC: ({0:X2}){1}{2}A: {3:X4}{4}X: {5:X4}{6}Y - {7:X4}{8}SP - {9:X4}{10}D - {11:X4}{12}DB - {13:X2}{14}P - {15:X2}{16}{17}{18}",
                                    PB, PC, Environment.NewLine, 
                                    A, Environment.NewLine, 
                                    X, Environment.NewLine, 
                                    Y, Environment.NewLine, 
                                    SP, Environment.NewLine,
                                    D, Environment.NewLine,
                                    DB, Environment.NewLine, 
                                    P, Environment.NewLine,
                                    flags, Environment.NewLine);

     
        }

    }
}
