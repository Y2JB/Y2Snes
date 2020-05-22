using System;
using System.Collections.Generic;
using System.Text;

namespace Y2Snes.Core
{
    public partial class Ppu
    {

        byte Register4212 
        {
            get
            {/*
                byte value = 0;

                if ((CPU.V_Counter >= Screen_Y_Resolution + First_Visible_Line) && (CPU.V_Counter < Screen_Y_Resolution + First_Visible_Line + 3))
                    value = 1;
                if ((CPU.Cycles < Timings.HBlankEnd) || (CPU.Cycles >= Timings.HBlankStart))
                    value |= 0x40;
                if (CPU.V_Counter >= Screen_Y_Resolution + First_Visible_Line)
                    value |= 0x80;

                return (byte);*/
                return 1;
            }
        }
    }
}
