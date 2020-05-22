using System;

namespace Y2Snes.Core
{
    public class Instruction
    {
        public string Name { get; }
        public byte OpCode { get; set; }
        public byte OperandLength { get; set;  }
        public Func<uint, uint> OperandAdjuster { get; }
        public Action<uint> Handler { get; }

        public Instruction(string name, Func<uint, uint> opAdjuster, Action<uint> handler)
        {
            Name = name;
            // must be set manually (now storing op lengths in static tabls)
            //OpCode = opCode;
            //OperandLength = operandLength;
            OperandAdjuster = opAdjuster;
            Handler = handler;
        }
        
    }
}
