using System;

namespace Y2Snes.Core
{
    public class Instruction
    {
        public string Name { get; }
        public byte OpCode { get; }
        public byte OperandLength { get; }
        public Action<uint> Handler { get; }

        public Instruction(string name, byte opCode, byte operandLength, Action<uint> handler)
        {
            Name = name;
            OpCode = opCode;
            OperandLength = operandLength;
            Handler = handler;
        }
        
    }
}
