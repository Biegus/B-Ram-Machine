using Microsoft.VisualStudio.TestTools.UnitTesting;
using RamMachine;
namespace UnitTests
{
    [TestClass]
    public class GeneralTests
    {
        [TestMethod]
        public void ReadWriteTest()
        {
            string code = @"
read 0
write 0
";
            RamMachineRuntime runtime = new RamMachineRuntime(RamMachineCommand.Parse(code));
            runtime.Input(5);
            runtime.DoUntillEnd();
            Assert.AreEqual(5, runtime.GetOutput()[0]);
        }
        [TestMethod]
        public void MathOperationTest()
        {
            string code = @"

load =1
add =2
store 2
load =1
mult =4
store 3
load =3
div =3
store 4
load =10
sub =5
store 5

";
            var runtime = RamMachineRuntime.Run(RamMachineCommand.Parse(code));
            Assert.AreEqual(3, runtime.Memory[2]);
            Assert.AreEqual(4, runtime.Memory[3]);
            Assert.AreEqual(1, runtime.Memory[4]);
            Assert.AreEqual(5, runtime.Memory[5]);
        }
        [TestMethod]
        public void JumpTest()
        {
            string code = @"
       load =10

start:  sub =1
       jgtz start
       jump end
       halt
end:   write 0

";
            var runtime = RamMachineRuntime.Run(RamMachineCommand.Parse(code));
            Assert.AreEqual( 1, runtime.GetOutput().Length);
            Assert.AreEqual( 0, runtime.GetOutput()[0]);
        }
        [TestMethod]
        public void CommentTest()
        {
            string code = @"
load =10 #comment
#another one
write 0
";
            var runtime = RamMachineRuntime.Run(RamMachineCommand.Parse(code));
            Assert.AreEqual(10, runtime.GetOutput()[0]);
          
        }
        [TestMethod]
        public void RealExample()
        {
            string code = @"
          load  =0
          store 1        
reading:  read  2        
          load  2
evenLoop: jzero end
          load  2
          div   =2
          mult  =2
          store 3 # it's just comment to test
          load  2
          sub   3
          jzero next
          jump  reading
next:     load  2
          div   =2
          store 2
          load  1
          add   =1
          store 1
          jump  evenLoop
end:      write 1
";
            RamMachineRuntime runtime = new RamMachineRuntime(RamMachineCommand.Parse(code));
            runtime.Input(3);
            runtime.Input(8);
            runtime.Input(0);
            runtime.DoUntillEnd();
            Assert.AreEqual(3, runtime.GetOutput()[0]);
            
        }
    }
}
