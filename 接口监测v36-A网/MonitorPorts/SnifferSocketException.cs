using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonitorPorts
{
    class SnifferSocketException : Exception
    {
        public SnifferSocketException()
            : base()
        {

        }
        public SnifferSocketException(string message)
            : base(message)
        {

        }
        public SnifferSocketException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
