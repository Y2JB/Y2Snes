using System;
using System.Collections.Generic;
using System.Text;

namespace Y2Snes.Core
{
    // Snes uses a modified W65C816S 3.58 MHz
    public partial class Cpu
    {
        // 1 Cpu cycle is 6 Master Clock cycles. The Master Clock syncs everything (PPU etc) so we measure everything in Master clocks 
        // The system divides the master clock to produce the CPU clock. The divider depends on the accessed address. RAM and slow ROM are 8 clocks per cycle; 
        // most everything else is 6 clocks per cycle.
        public const int One_Cpu_Cycle = 6;
        public const int One_Cpu_Cycle_Slow = 8;

        // Main registers
        public ushort A { get; set; }
        public byte AL { get { return (byte)(A & 0x00FF); } set { A = (ushort)((A & 0xFF00) | value); } }
        public byte AH { get { return (byte)((A & 0xFF00) >> 8); } }

        // Index registers
        public ushort X { get; set; }
        public byte XL { get { return (byte)(X & 0x00FF); } set { X = (ushort)((X & 0xFF00) | value); } }
        public byte XH { get { return (byte)((X & 0xFF00) >> 8); } set { X = (ushort)((X & 0x00FF) | (ushort)(value << 8)); } }

        public ushort Y { get; set; }
        public byte YL { get { return (byte)(Y & 0x00FF); } set { Y = (ushort)((Y & 0xFF00) | value); } }
        public byte YH { get { return (byte)((Y & 0xFF00) >> 8); } set { Y = (ushort)((Y & 0x00FF) | (ushort)(value << 8)); } }

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

        public IAbsoluteLongMemoryReaderWriter MemoryAbsolute { get; private set; }

        public UInt32 Ticks { get; private set; }

        SuperFamicom snes;
        IBankedMemoryReaderWriter memoryMap;
        

        public Cpu(SuperFamicom system)
        {
            this.snes = system;
            memoryMap = system.MemoryMap;

            MemoryAbsolute = new AbsoluteLongMemoryReaderWriter(memoryMap);

            RegisterInstructionHandlers();
        }


        public void Reset(ushort resetVector)
        {
            PC = snes.rom.ResetVectorEM;
            SP = 0x00;

            // Acc, X & Y all start in 8 bit mode
            SetFlag(CpuFlag.M);
            SetFlag(CpuFlag.X);
            SetFlag(CpuFlag.IrqDisable);
        }


        
        // The PPU outputs half pixels at 10.74 MHz, which is one-half the master clock.This is faster than the CPU clock, which is 3.58 MHz when accessing fast memory or 2.68 MHz 
        // when accessing slow memory.


        // $0000-$1FFF is slow memory, and all of banks $7E and $7F are slow memory
        // Fast memory is memory controller registers($4200-$43FF), the B bus($2100-$21FF), and ROM with bit 23 set($808000-$80FFFF, $818000-$81FFFF, $828000-$82FFFF, ..., $BF8000-$FFFFFF)
        // ROM with bit 23 clear($008000-$00FFFF, $018000-$01FFFF, $028000-$02FFFF, ..., $3F8000-$7DFFFF) is also slow memory
        public void Step()
        {
            byte opCode = memoryMap.ReadByte(PB, PC++);

            var instruction = GetInstruction(opCode);
            if (instruction == null || instruction.Handler == null)
            {
                throw new ArgumentException(String.Format("Unsupported instruction 0x{0:X2} {1}", opCode, instruction == null ? "-" : instruction.Name));
            }


            // I think this is when the PC no longer points to code within our PB block. Then we have to switch to addressing which takes and extra cycles 
            /* TODO: WHen PC goes over a 4K block boundary, we switch to 'slow' opcodes??? See cpuexec.cpp ln 160
            if ((Registers.PCw & MEMMAP_MASK) + ICPU.S9xOpLengths[Op] >= MEMMAP_BLOCK_SIZE)
		    {
				    Opcodes = S9xOpcodesSlow;
		    } 
              */

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

            // This is how we adjust memory addresses when the cpu uses different addressing modes such as AbsoluteIndexX
            if(instruction.OperandAdjuster != null)
            {
                operandValue = instruction.OperandAdjuster(operandValue);
            }

            instruction.Handler(operandValue);
        }


        public void StackPush16(ushort value)
        {
            memoryMap.WriteShort(0x00, (ushort)(SP - 1), value);
            SP -= 2;            
        }

        public void StackPush8(byte value)
        {
            memoryMap.WriteByte(0x00, (ushort)(SP), value);
            SP--;
        }

        public ushort StackPop16()
        {
            ushort value = memoryMap.ReadShort(0x00, (ushort)(SP + 1));
            SP += 2;
            return value;
        }

        public byte StackPop8()
        {
            byte value = memoryMap.ReadByte(0x00, (ushort)(SP + 1));
            SP ++;
            return value;
        }


        public override String ToString()
        {
            string flags = String.Format("{0}{1}{2}{3}{4}{5}{6}{7}",   CarryFlag ? "C" : "-", ZeroFlag ? "Z" : "-", IrqDisableFlag ? "I" : "-", DecimalFlag ? "D" : "-",
                                                                    XFlag ? "X" : "-", MFlag ? "M" : "-", OverflowFlag ? "V" : "-", NegativeFlag ? "N" : "-");


            return String.Format("PC: ({0:X2}){1:X4}{2}A: {3:X4}{4}X: {5:X4}{6}Y - {7:X4}{8}SP - {9:X4}{10}D - {11:X4}{12}DB - {13:X2}{14}P - {15:X2}{16}{17}{18}",
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
