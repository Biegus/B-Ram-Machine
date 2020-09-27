using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
namespace RamMachine
{
    public class ParsedRamCode
    {
        private RamMachineCommand[] commands;
        public  ReadOnlyCollection<RamMachineCommand> Commands { get; }
        public string Raw { get; }
        public ParsedRamCode(string code)
        {
            commands = RamMachineCommand.Parse(code).ToArray();
            Raw = code;
            Commands = new ReadOnlyCollection<RamMachineCommand>(commands);
        }
        public override string ToString()
        {
            return Raw;
        }
    }
}
