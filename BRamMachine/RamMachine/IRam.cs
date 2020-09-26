using System;
using System.Collections.Generic;
using System.Text;

namespace RamMachine
{
    public interface IRam
    {
         void Set(int place, long val);
        long Get(int place);
         void Write(long value);
        long Read();
         void Jump(string place);
         void Halt();
         void PushLabel(string place);
        uint GetPoint();
    }
}
