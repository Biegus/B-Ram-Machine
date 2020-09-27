using System;
using System.Collections.Generic;
using System.Text;

namespace RamMachine
{
    public class RamMachineController
    {
        private static CommandBehavior.LabelCommand labelCommand = new CommandBehavior.LabelCommand();
        private static Dictionary<string, CommandBehavior> commands = new Dictionary<string, CommandBehavior>()
        {
            ["load"] = new CommandBehavior.LoadCommand(),
            ["jump"] = new CommandBehavior.JumpCommand(),
            ["write"] = new CommandBehavior.WriteCommand(),
            ["read"] = new CommandBehavior.ReadCommand(),
            ["store"] = new CommandBehavior.StoreCommand(),
            ["mult"] = new CommandBehavior.MathOperandCommand((a, b) => a * b),
            ["div"] = new CommandBehavior.MathOperandCommand((a, b) => a / b),
            ["add"] = new CommandBehavior.MathOperandCommand((a, b) => a + b),
            ["sub"] = new CommandBehavior.MathOperandCommand((a, b) => a - b),
            ["jgtz"] = new CommandBehavior.JumpIfCommand((a) => a > 0),
            ["jzero"] = new CommandBehavior.JumpIfCommand((a) => a == 0),
            ["halt"] = new CommandBehavior.Halt(),


        };
        public static bool Exist(string type)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return commands.ContainsKey(type) || type.EndsWith(":");
        }
        public static CommandBehavior GetCommand(string type)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if(commands.TryGetValue(type,out CommandBehavior result))
            {
                return result;
            }
            else
            {
                return (type.EndsWith(":") ? labelCommand : null);
            }
        }
        public static void Invoke(RamMachineCommand command, IRamMachine ram)
        {
            if (ram is null)
            {
                throw new ArgumentNullException(nameof(ram));
            }

            try
            {
                if (command.Type.EndsWith(":"))
                    labelCommand.Do(command, ram);
                else
                    commands[command.Type].Do(command, ram);
            }
            
            catch(Exception exception) when (!(exception is RamMachineRuntimeException))
            {
                throw new RamMachineRuntimeException(RamMachineHelper.GetLineNumber(ram.GetRaw(), commands.ToString()), $"Unexpected exception had occured during runtime ({command})", exception);
            }
            
        }
      
    }
}
