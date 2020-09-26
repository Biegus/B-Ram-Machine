using System;
using System.Collections.Generic;
using System.Text;

namespace RamMachine
{
    public abstract partial class CommandBehavior
    {
        public abstract void Do(RamMachineCommand command, IRam ram);
        public virtual bool IsArgumentCorrect(string argument) => true;
        public virtual bool PreInvoke() => false;
    }
    public abstract partial class CommandBehavior
    {
        public class LoadCommand : CommandBehavior
        {
            public override bool IsArgumentCorrect(string argument)
            {
                return RamMachineHelper.CheckPreArgument(argument);
                    
            }
            public override void Do(RamMachineCommand command, IRam ram)
            {
                RamMachineHelper.DoWithDeepLv(command.Argument, (ramEl, val) => ramEl.Set(0, val), ram);
            }
        }
        public class JumpCommand : CommandBehavior
        {
            public override void Do(RamMachineCommand command, IRam ram)
            {
                ram.Jump(command.Argument);
            }
        }
        public class LabelCommand : CommandBehavior
        {
            public override bool IsArgumentCorrect(string argument)
            {
                return argument.Equals(string.Empty);
            }
            public override void Do(RamMachineCommand command, IRam ram)
            {
                ram.PushLabel(command.Type.Substring(0,command.Type.Length-1));
            }
            public override bool PreInvoke()
            {
                return true;
            }
        }
        public class WriteCommand : CommandBehavior
        {
            public override bool IsArgumentCorrect(string argument)
            {
                return RamMachineHelper.CheckPreArgument(argument);
            }
            public override void Do(RamMachineCommand command, IRam ram)
            {
                RamMachineHelper.DoWithDeepLv(command.Argument, (ramEl, val) => ramEl.Write(val),  ram);
            }
        }
        public class ReadCommand : CommandBehavior
        {
            public override bool IsArgumentCorrect(string argument)
            {
                return RamMachineHelper.CheckPreArgument(argument, new char?[] { '*',null});

            }
            public override void Do(RamMachineCommand command, IRam ram)
            {               
                var splited = RamMachineHelper.SplitToPreArgument(command.Argument);
                long val = long.Parse(splited.argument);
                switch (splited.pre)
                {
                    case null: ram.Set((int)val, ram.Read()); break;
                    case '^':
                    case '*': ram.Set((int) ram.Get((int)val), ram.Read()); break;
                }
            }
        }
        public class StoreCommand : CommandBehavior
        {
            public override bool IsArgumentCorrect(string argument)
            {
                return RamMachineHelper.CheckPreArgument(argument, new char?[] { '*', null });
            }
            public override void Do(RamMachineCommand command, IRam ram)
            {
                var splited = RamMachineHelper.SplitToPreArgument(command.Argument);
                int val = int.Parse(splited.argument);
                switch (splited.pre)
                {
                    case null: ram.Set(val, ram.Get(0)); break;
                    case '*': ram.Set((int)ram.Get((int)val), ram.Get(0)); break;            
                }
            }        
        }
        public class JumpIfCommand : CommandBehavior
        {
            private Func<long, bool> func;
            public JumpIfCommand(Func<long, bool> func)
            {
                this.func = func;
            }
            public override void Do(RamMachineCommand command, IRam ram)
            {
                if(func(ram.Get(0)))
                {
                    ram.Jump(command.Argument);
                }
            }
        }
        public class MathOperandCommand : CommandBehavior
        {
            public override bool IsArgumentCorrect(string argument)
            {
                return RamMachineHelper.CheckPreArgument(argument);
            }
            private Func<long, long, long> func;
            public MathOperandCommand(Func<long, long, long> func)
            {
                this.func = func;
            }
            public override void Do(RamMachineCommand command, IRam ram)
            {
                RamMachineHelper.DoWithDeepLv(command.Argument, (ramEl, val)=>
                {
                    ramEl.Set(0, func(ramEl.Get(0),val));
                },   ram);
            }
        }
        public class Halt : CommandBehavior
        {
            public override bool IsArgumentCorrect(string argument)
            {
                return argument.Equals(string.Empty);
            }
            public override void Do(RamMachineCommand command, IRam ram)
            {
                ram.Halt();
            }
        }
    }
}
