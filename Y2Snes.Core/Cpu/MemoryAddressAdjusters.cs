using System;

namespace Y2Snes.Core
{
    public partial class Cpu
    {
        uint AbsoluteLongIndexedX(uint addressIn)
        {
            return addressIn + X;
        }
    }





 
}
