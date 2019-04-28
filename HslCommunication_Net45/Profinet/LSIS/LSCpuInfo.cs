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
}
