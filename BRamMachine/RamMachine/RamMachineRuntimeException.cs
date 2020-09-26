using System;
using System.Collections.Generic;
using System.Text;

namespace RamMachine
{
    class RamMachineRuntimeException : Exception
    {
        public uint Line { get; }
        public RamMachineRuntimeException(uint line,string message, Exception innerException) : base(message, innerException)
        {
            Line = line;
        }
    }
}
