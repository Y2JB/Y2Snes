using System;
using Y2Snes.Core;

namespace SnesDebugger
{
    // We bolt onto the instruction the state it had when it executed - pc, operand etc
    public class StoredInstruction : Instruction
    {
        // These methods are only used when peeking the instruction, not when exectuing as then the data needs to be fetched 
        public bool HasOperand { get { return OperandLength != 0; } }
        public uint Operand { get; set; }
        public ushort PC { get; set; }

        // NB: I'm not setting the handler as this is purely for debugging!
        public static StoredInstruction DeepCopy(Instruction instruction)
        {
            if (instruction == null) return new StoredInstruction("UNKNOWN INSTRUCTION", 0, 0, null);
            return new StoredInstruction(instruction.Name, instruction.OpCode, instruction.OperandLength, null)
            {
                //Operand = instruction.Operand,
                //extendedInstruction = instruction.extendedInstruction
            };       
        }

        public static StoredInstruction DeepCopy(StoredInstruction instruction)
        {
            return new StoredInstruction(instruction.Name, instruction.OpCode, instruction.OperandLength, null)
            {
                Operand = instruction.Operand,
                PC = instruction.PC,
            };
        }

        public StoredInstruction(string name, byte opCode, byte operandLength, Action<ushort> handler) : base(name, opCode, operandLength, null)
        {
        }

        public override String ToString()
        {
            if (HasOperand)
            {
                string instructionWithOperand = String.Format(Name, Operand);
                return String.Format("({0:X2})  ->  {1}", PC, instructionWithOperand);

            }
            else
            {
                return String.Format("({0:X2})  ->  {1}", PC, Name);
            }
        }
    }
}
