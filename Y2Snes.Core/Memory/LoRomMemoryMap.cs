using System;
using System.Collections.Generic;
using System.Text;

namespace Y2Snes.Core
{

    public class LoRomMemoryMap : IBankedMemoryReaderWriter
    {
        Memory memory;

        public LoRomMemoryMap(Memory memory)
        {
            this.memory = memory; 
        }


        public byte ReadByte(byte bank, ushort address)
        {
            if (bank >= 0x00 && bank <= 0x3F)
            {
                // Rom data is mapped in 32K chunks between 8000h-FFFFh
                // Changing the bank moves the ptr in 32K increments
                if (address >= 0x8000 && address <= 0xFFFF)
                {
                    uint romAddress = (uint)(address - 0x8000);
                    romAddress += (uint)(bank * 0x8000);
                    return memory.Rom.ReadByte(romAddress);
                }
                else if(address >= 0x0000 && address <= 0x1FFF)
                {
                    // Provides access to the first 8K of RAM, ignores the bank
                    return memory.WRam[address];
                }
                else if (address >= 0x2100 && address <= 0x21FF)
                {
                    // PPU1, APU, hardware registers
                    return memory.ApuPpu1Ram[address - 0x2100];
                }
                
                else if (address >= 0x4200 && address <= 0x44FF)
                {
                    // DMA, PPU2 hardware registers, always accesses the same data regardless of bank
                    return memory.DmaPpu2Ram[address - 0x4200];
                }
            }
            else if (bank == 0x7E)
            {
                // RAM access, first 64K 
                return memory.WRam[address];
            }
            else if (bank == 0x7F)
            {
                // RAM second 64K
                return memory.WRam[address + 0xFFFF];
            }
            throw new ArgumentException("bad memory read");
        }


        public ushort ReadShort(byte bank, ushort address)
        {
            // NB: Little Endian
            return (ushort)((ReadByte(bank, (ushort)(address + 1)) << 8) | ReadByte(bank, address));
        }


        public UInt32 ReadLong(byte bank, ushort address)
        {
            // NB: Little Endian
            return (UInt32)((ReadByte(bank, (ushort)(address + 2)) << 16) | (ReadByte(bank, (ushort)(address + 1)) << 8) | ReadByte(bank, address));
        }


        public void WriteByte(byte bank, ushort address, byte value)
        {
            if (bank >= 0x00 && bank <= 0x3F)
            {
                if (address >= 0x0000 && address <= 0x1FFF)
                {
                    // Provides access to the first 8K of RAM, ignores the bank
                    memory.WRam[address] = value;
                }
                else if (address >= 0x2100 && address <= 0x21FF)
                {
                    // PPU1, APU, hardware registers
                    memory.ApuPpu1Ram[address - 0x2100] = value; 
                }
                else if (address >= 0x4200 && address <= 0x44FF)
                {
                    // DMA, PPU2 hardware registers
                    memory.DmaPpu2Ram[address - 0x4200] = value;
                }
                else
                {
                    throw new ArgumentException("bad memory write");
                }
            }
            else if (bank == 0x7E)
            {
                // RAM access, first 64K 
                memory.WRam[address] = value;
            }
            else if (bank == 0x7F)
            {
                // RAM second 64K
                memory.WRam[address + 0xFFFF] = value;
            }
            else
            {
                throw new ArgumentException("bad memory write");
            }
        }


        public void WriteShort(byte bank, ushort address, ushort value)
        {
            WriteByte(bank, address, (byte)(value & 0x00ff));
            WriteByte(bank, (ushort)(address + 1), (byte)((value & 0xff00) >> 8));
        }

        public void WriteLong(byte bank, ushort address, UInt32 value)
        {
            WriteByte(bank, address, (byte)(value & 0x000000ff));
            WriteByte(bank, (ushort)(address + 1), (byte)((value & 0x0000ff00) >> 8));
            WriteByte(bank, (ushort)(address + 2), (byte)((value & 0x00ff0000) >> 16));
        }
    }
}
