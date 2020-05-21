using System;

namespace Y2Snes.Core
{
    // Used to directly access 24 bit addresses. Addresses will be interpetted as 24 bit
    public class AbsoluteLongMemoryReaderWriter : IAbsoluteLongMemoryReaderWriter
    {
        IBankedMemoryReaderWriter memoryMap;

        public AbsoluteLongMemoryReaderWriter(IBankedMemoryReaderWriter memoryMap)
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
