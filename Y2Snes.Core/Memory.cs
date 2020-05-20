using System;
using System.Collections.Generic;
using System.Text;

namespace Y2Snes.Core
{

    // TODO: This is LoROM Memory Map, this should be written in sucha way that it can be swapped out for HiROM

    public class Memory : IMemoryReaderWriter
    {
        public byte OpenBus { get; set; }

        public byte[] WRam { get; set; }
        public byte[] DmaPpuRam { get; set; }

        SuperFamicom system;

        public Memory(SuperFamicom system)
        {
            this.system = system;

            // 128K
            WRam = new byte[0x20000];

            // 768 bytes
            DmaPpuRam = new byte[0x300];
        }


        // Memory Map


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
                    return system.rom.ReadByte(romAddress);
                }
                else if(address >= 0x0000 && address <= 0x1FFF)
                {
                    // Provides access to the first 8K of RAM, ignores the bank
                    return WRam[address];
                }
                // DMA, PPU2 hardware registers, always accesses the same data regardless of bank
                else if (address >= 0x4200 && address <= 0x44FF)
                {
                    return DmaPpuRam[address - 0x4200];
                }
            }
            else if (bank == 0x7E)
            {
                // RAM access, first 64K 
                return WRam[address];
            }
            else if (bank == 0x7F)
            {
                // RAM second 64K
                return WRam[address + 0xFFFF];
            }
            throw new ArgumentException("bad memory read");
        }


        public ushort ReadShort(byte bank, ushort address)
        {
            // NB: Little Endian
            return (ushort)((ReadByte(bank, (ushort)(address + 1)) << 8) | ReadByte(bank, address));
        }


        public void WriteByte(byte bank, ushort address, byte value)
        {
            if (bank >= 0x00 && bank <= 0x3F)
            {
                if (address >= 0x0000 && address <= 0x1FFF)
                {
                    // Provides access to the first 8K of RAM, ignores the bank
                    WRam[address] = value;
                }
                else if (bank >= 0x00 && bank <= 0x3F)
                {
                    // DMA, PPU2 hardware registers
                    if (address >= 0x4200 && address <= 0x44FF)
                    {
                        DmaPpuRam[address - 0x4200] = value;
                    }
                }
            }
            else if (bank == 0x7E)
            {
                // RAM access, first 64K 
                WRam[address] = value;
            }
            else if (bank == 0x7F)
            {
                // RAM second 64K
                WRam[address + 0xFFFF] = value;
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
    }
}
