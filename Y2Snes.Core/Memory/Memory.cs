using System;
using System.Collections.Generic;
using System.Text;

namespace Y2Snes.Core
{

    public class Memory
    {
        public byte OpenBus { get; set; }

        public byte[] WRam { get; set; }
        public byte[] ApuPpu1Ram { get; set; }
        public byte[] DmaPpu2Ram { get; set; }

        public IRom Rom { get; set; }

        SuperFamicom system;

        public Memory(SuperFamicom system)
        {
            this.system = system;

            // 128K
            WRam = new byte[0x20000];

            // 256 bytes
            ApuPpu1Ram = new byte[0xFF];

            // 768 bytes
            DmaPpu2Ram = new byte[0x300];

            Rom = system.rom;
        }

    }
}
