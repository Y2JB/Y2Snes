using System;

namespace Y2Snes.Core
{
    // TODO: Rename to immediate, including methods 
    public interface IDirectMemoryReader
    {
        byte ReadByte(uint address);
        ushort ReadShort(uint address);
    }


    public interface IBankedMemoryReader
    {
        byte ReadByte(byte bank, ushort address);
        ushort ReadShort(byte bank, ushort address);
    }

    public interface IMemoryWriter
    {
        void WriteByte(byte bank, ushort address, byte value);
        void WriteShort(byte bank, ushort address, ushort value);
    }


    public interface IMemoryReaderWriter : IBankedMemoryReader, IMemoryWriter
    {
    }


 
}
