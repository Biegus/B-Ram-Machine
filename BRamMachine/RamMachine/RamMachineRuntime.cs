﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace RamMachine
{
    public class RamMachineRuntime : IRamMachine
    {
        public class RamMachineOutputArgs : EventArgs
        {
          
            public RamMachineRuntime Runtime { get; }
            public long NewestValue { get; }
            public RamMachineOutputArgs(RamMachineRuntime runtime, long newestValue)
            {
                Runtime = runtime;
                NewestValue = newestValue;
            }

        }

        private readonly Dictionary<string, uint> labels = new Dictionary<string, uint>();
       
        private readonly List<long?> ram = new List<long?>();
        
        private readonly Queue<long> output = new Queue<long>();
        private readonly Queue<long> input = new Queue<long>();
        private readonly RamMachineCommand[] commands;
        private bool registerEnd = false;
        public uint Point { get; set; } = 0;
        public  ReadOnlyCollection<RamMachineCommand> Commands { get; }
        public uint LineInvoked { get; set; } = 0;

        public event EventHandler<RamMachineOutputArgs> OnOutput = delegate { };
        public event EventHandler OnEnded = delegate { };

        public ReadOnlyCollection<long?> Memory => ram.AsReadOnly();
        public long[] GetOutput() => output.ToArray();
        
        public bool HasEnded => Point >= Commands.Count;
        public uint InvokedLimit { get; set; } = 5000000;

        public RamMachineRuntime(IEnumerable<RamMachineCommand> commands)
        {
            this.commands = commands.ToArray();
            Commands = new ReadOnlyCollection<RamMachineCommand>(this.commands);
            PrepareInvoke();
        }
        public static RamMachineRuntime Run(IEnumerable<RamMachineCommand> code,params long[] input)
        {
            var runtime = new RamMachineRuntime(code);
            runtime.DoUntillEnd();
            runtime.Input(((IEnumerable<long>)input)??Enumerable.Empty<long>());
            return runtime;

        }
        private void PrepareInvoke()
        {
            while(Point<Commands.Count)
            {
                if (RamMachineController.GetCommandBehavior(Commands[(int)Point].Type).PreInvoke())
                    RamMachineController.Invoke(Commands[(int)Point], this);
                Point++;
            }
            Point=0;
        }
        public void Input(long value)
        {
            this.input.Enqueue(value);
           
        }
        public void Input(IEnumerable<long> values)
        {
            foreach (var item in values)
                Input(item);
        }
        public void ClearInput()
        {
            this.input.Clear();
        }
        public bool DoNext(uint count)
        {
            for(uint x=0;x<count;x++)
            {
                if (!DoNext())
                    return false;
            }
            return true;          
        }      
        public bool DoNext()
        {
            if(Point>=Commands.Count)
            {
                if(!registerEnd)
                {
                    OnEnded(this,EventArgs.Empty);
                    registerEnd = true;
                }
                return false;
                
            }
            registerEnd = false;
            if (RamMachineController.GetCommandBehavior(Commands[(int)Point].Type).PreInvoke() == false)
                RamMachineController.Invoke(Commands[(int)Point], this);
            Point++;
            LineInvoked++;
            if(InvokedLimit>0&&LineInvoked>InvokedLimit)
            {
                throw new RamMachineRuntimeException(Point, $"Limit of {InvokedLimit} commands has been approched, the ram machine code can have infinity loop at {Point+1} line", null);
            }
            return true;
        }
        public void DoUntillEnd()
        {
            while (DoNext()) { }  
        }
        long IRamMachine.Get(int place)
        {
            if(place<ram.Count&&ram[(int)place]!=null)
            {
                return (long)ram[(int)place];
            }
            else
            {
                throw new RamMachineRuntimeException(commands[(int)Point].Line,$"Trying to get unready value ({Commands[(int)Point]})",null);
            }
        }

        void IRamMachine.Set(int place, long val)
        {
           if(place>=ram.Count)
            {
                ram.AddRange(Enumerable.Range(1, (int)((place - ram.Count) + 1)).Select(item => ((long?)null)));
            }
            ram[(int)place] = val;

        }
        long IRamMachine.Read()
        {
            if (input.Count == 0)
                throw new RamMachineRuntimeException(0,"Input is empty", null);
            return input.Dequeue();
        }
        void IRamMachine.Write(long value)
        {
            output.Enqueue(value);
            OnOutput(this, new RamMachineOutputArgs(this, value));
        }
        void IRamMachine.Jump(string label)
        {
            if (labels.ContainsKey(label) == false)
                throw new RamMachineRuntimeException(Point,$"Undefined label ({label}) at {Point+1}",null);
            Point = labels[label];
        }
        void IRamMachine.PushLabel(string label)
        {
            labels[label] = Point;
        }
        void IRamMachine.Halt()
        {
            Point = (uint)(Commands.Count - 1);
        }

        uint IRamMachine.GetPoint()
        {
            return Point;
        }

      
    }
}
