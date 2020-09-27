using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RamMachine
{
    public class RamMachineHelper
    {
        public static void DoWithDeepLv(string argument,Action<IRamMachine,long> action,IRamMachine ram)
        {
            var splited = SplitToPreArgument(argument);
            int value = int.Parse(splited.argument);
            switch(splited.pre)
            {
                case '=': action(ram, value);break;
                case null: action(ram, ram.Get(value));break;
                case '^':
                case '*': action(ram, ram.Get((int)ram.Get((int)value))); break;
            }
        } 
        public static bool CheckPreArgument(string argument,char?[] acceptable=null)
        {
            if(argument.Equals(string.Empty)||(argument.Length==1&& !char.IsNumber(argument[0])||(argument.Length>1&&(!char.IsNumber(argument[1])&&argument[1]!='-'&&argument[1]!='+'))))
            {
                return false;
            }
            acceptable =acceptable?? new char?[] { null, '=', '*','^' };
            return acceptable.Contains(SplitToPreArgument(argument).pre);
        }
        public static uint GetLineNumber(string full, string line)
        {
            return (uint)full.Substring(0, full.IndexOf(line)).Count(item => item == '\n') + 1;
        }
        public static (char? pre, string argument) SplitToPreArgument(string original)
        {
            char? pre=null;
            string argument;
            if (original.Length==0)
                return default;
            if (!char.IsNumber(original[0]) && (char.IsNumber(original[1]) || original[1] == '-' || original[1] == '+'))
            {
                pre = original[0];
                argument = original.Substring(1);
            }
            else
            {
                argument = original;
            }
            return (pre, argument);
        }
    }
}
