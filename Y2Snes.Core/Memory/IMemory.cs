using System;

namespace Y2Snes.Core
{
    public interface IAbsoluteLongMemoryReader
    {
        // Can be used to access 16 & 24 bit addresses
        byte ReadByte(uint address);
        ushort ReadShort(uint address);
        uint ReadLong(uint address);
    }

    public interface IAbsoluteLongMemoryWriter
    {
        // Can be used to access 16 & 24 bit addresses
        void WriteByte(uint address, byte value);
        void WriteShort(uint address, ushort value);
        void WriteLong(uint address, uint value);
    }

    public interface IAbsoluteLongMemoryReaderWriter : IAbsoluteLongMemoryReader, IAbsoluteLongMemoryWriter
    {
    }


    public interface IBankedMemoryReader
    {
        byte ReadByte(byte bank, ushort address);
        ushort ReadShort(byte bank, ushort address);
        UInt32 ReadLong(byte bank, ushort address);
    }

    public interface IBankedMemoryWriter
    {
        void WriteByte(byte bank, ushort address, byte value);
        void WriteShort(byte bank, ushort address, ushort value);
        void WriteLong(byte bank, ushort address, UInt32 value);
    }


    public interface IBankedMemoryReaderWriter : IBankedMemoryReader, IBankedMemoryWriter
    {
    }


 
}
