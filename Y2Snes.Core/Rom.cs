using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Y2Snes.Core
{
    public class Rom : IRom
    {
        private byte[] romData;
        private readonly int RomNameOffset = 0x7FC0;


        public string RomName { get; private set; }

        // Native Mode Vectors

        // Emulation Mode Vectors
        private readonly ushort ResetVectorEMOffset = 0x7FFC;


        // Snes starts in emulation mode
        public ushort ResetVectorEM { get; private set; }

        //#define ROM_OFFSET_ROM_SIZE 0x148
        //#define ROM_OFFSET_RAM_SIZE 0x149

        string romFileName;

        public Rom(string fn)
        {
            romFileName = fn;

            romData = new MemoryStream(File.ReadAllBytes(fn)).ToArray();

            // Trim the smc header
            romData = romData.Skip(512).ToArray();

            RomName = Encoding.UTF8.GetString(romData, RomNameOffset, 21);

            // TODO: There are multpiple vecotrs here for IRQ's etc. You'll need to read them out!
            ResetVectorEM = ReadShort(ResetVectorEMOffset);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte ReadByte(uint address)
        {
            return romData[address];
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort ReadShort(uint address)
        {
            // NB: Little Endian
            return (ushort)((ReadByte((uint)(address+1)) << 8) | ReadByte(address));
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint ReadLong(uint address)
        {
            // NB: Little Endian
            return (uint)((ReadByte((uint)(address + 2)) << 16) | (ReadByte((uint)(address + 1)) << 8) | ReadByte(address));
        }
    }
}
