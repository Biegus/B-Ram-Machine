using System;
using System.Collections.Generic;
using System.Text;

namespace RamMachine
{
    class RamMachineParserException : FormatException
    {
        public uint LineIndex { get; }
        public RamMachineParserException(uint line, string message) : base(message)
        {
            LineIndex = line;
        }
    }
}
