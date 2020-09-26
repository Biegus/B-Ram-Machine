using System;
using System.Collections.Generic;
using System.Text;

namespace RamMachine
{
    class RamMachineParserException : FormatException
    {
        public int LineIndex { get; }
        public RamMachineParserException(int line, string message) : base(message)
        {
            LineIndex = line;
        }
    }
}
