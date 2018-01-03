using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonitorPorts
{
    public enum SendDir :int
    {
        /// <summary>
        /// VOBC发送给ZC
        /// </summary>
        VOBCToZC = 1,
        /// <summary>
        /// ZC发送给VOBC
        /// </summary>
        ZCToVOBC = 2,
        /// <summary>
        /// VOBC发送给CI
        /// </summary>
        VOBCToCI = 3,
        /// <summary>
        /// CI发送给VOBC
        /// </summary>
        CIToVOBC = 4,
        /// <summary>
        /// VOBC发送给ATS
        /// </summary>
        VOBCToATS = 5,
        /// <summary>
        /// ATS发送给VOBC
        /// </summary>
        ATSToVOBC = 6

    }
}
