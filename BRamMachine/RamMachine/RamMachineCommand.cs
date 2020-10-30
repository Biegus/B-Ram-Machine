using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RamMachine
{
    public class RamMachineCommand
    {
        public readonly string Type; 
        public readonly string Argument;
        public readonly uint Line;
        public RamMachineCommand(string type,string argument, uint curLine)
        {
            Line = curLine;
            if (!RamMachineController.Exist(type))
            {
                throw new RamMachineParserException(0,$"Command {type} doesn't exist");
            }
          
            (Type, Argument) = (type, argument);
        }
        public override string ToString()
        {
            return $"{Type} {Argument}";
        }
        public void Invoke(IRamMachine ram)
        {
            RamMachineController.Invoke(this, ram);
        }
        public bool IsCorrectArgument(string argument)
        {
            return RamMachineController.GetCommandBehavior(this.Argument).IsArgumentCorrect(argument);
        }
        public static IEnumerable<RamMachineCommand> Parse(string text)
        {
            Regex regex = new Regex(@"#.*?$",RegexOptions.Multiline);//getting rid of comments
            text = regex.Replace(text, "");
            int index = 0;
            uint curLine = 0;
          
            foreach(string comLine in text.Split('\n'))
            {

                curLine++;
                foreach(string line in Regex.Split(comLine, "$|(?<=:)"))
                {
                    var trimed = line.Trim();
                    if (trimed == string.Empty)
                        continue;
                    string[] splited = Regex.Split(trimed, " ").Where(item => item != string.Empty).Select(item => item.Trim()).ToArray();
                    string type = splited[0];
                    string argument = "";
                   
                    if (splited.Length > 1)
                    {
                        argument = splited[1];
                        if ((RamMachineController.GetCommandBehavior(type)?.IsArgumentCorrect(argument) == false))
                        {

                            throw new RamMachineParserException(curLine, $"\"{argument}\" is not correct argument for \"{type}\" at line {curLine} ");
                        }
                    }

                    if (RamMachine.RamMachineController.Exist((type)) == false)
                    {
                        throw new RamMachineParserException(curLine, $"Unknown command ({type}) at {curLine} line");
                    }
                    yield return new RamMachineCommand(type, argument,curLine);
                    index++;
                   
                }
                
            }
        }
    }

}
