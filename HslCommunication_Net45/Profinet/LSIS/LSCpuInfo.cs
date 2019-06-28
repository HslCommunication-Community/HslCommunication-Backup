using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HslCommunication.Profinet.LSIS
{
    /// <summary>
    /// It is determined to be the XGK/I/R series through a reserved area
    /// </summary>
    public enum LSCpuInfo
    {
        XGK = 1,
        XGI,
        XGR,
        XGB_MK,
        XGB_IEC,
    }

    public enum LSCpuStatus
    {
        RUN = 1,
        STOP,
        ERROR,
        DEBUG
    }
    /// <summary>
    /// using FlagBit in Marker for Byte
    /// M0.0=1;M0.1=2;M0.2=4;M0.3=8;==========================>M0.7=128
    /// </summary>
    public enum FlagBit
    {
        Flag1 = 1,
        Flag2 = 2,
        Flag4 = 4,
        Flag8 = 8,
        Flag16 = 16,
        Flag32 = 32,
        Flag64 = 64,
        Flag128 = 128,
        Flag256 = 256,

    }
}
