using System;

namespace Y2Snes.Core
{
    // Can be used to access 16 & 24 bit addresses
    public class AbsoluteMemoryReaderWriter : IAbsoluteMemoryReaderWriter
    {
        IBankedMemoryReaderWriter memoryMap;

        public AbsoluteMemoryReaderWriter(IBankedMemoryReaderWriter memoryMap)
        {
            this.memoryMap = memoryMap;
        }

      
        public byte ReadByte(uint address)
        {
            return memoryMap.ReadByte((byte)((address & 0x00FF0000) >> 16), (ushort)(address & 0x0000FFFF));
        }

        public ushort ReadShort(uint address)
        {
            return memoryMap.ReadShort((byte)((address & 0x00FF0000) >> 16), (ushort)(address & 0x0000FFFF));
        }

        public uint ReadLong(uint address)
        {
            return memoryMap.ReadLong((byte)((address & 0x00FF0000) >> 16), (ushort)(address & 0x0000FFFF));
        }


        public void WriteByte(uint address, byte value)
        {
            memoryMap.WriteByte((byte)((address & 0x00FF0000) >> 16), (ushort)(address & 0x0000FFFF), value);
        }


        public void WriteShort(uint address, ushort value)
        {
            memoryMap.WriteShort((byte)((address & 0x00FF0000) >> 16), (ushort)(address & 0x0000FFFF), value);
        }


        public void WriteLong(uint address, uint value)
        {
            memoryMap.WriteLong((byte)((address & 0x00FF0000) >> 16), (ushort)(address & 0x0000FFFF), value);
        }
    }





 
}
