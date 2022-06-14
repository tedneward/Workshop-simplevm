namespace vmtest;

[TestClass]
public class SimpleTests
{
    [TestMethod]
    public void TestTest()
    {
        Assert.AreEqual(2, 1 + 1);
        //Assert.AreEqual(2, 1 - 1); // just to make sure tests can fail, just in case
    }

    [TestMethod]
    public void TestInstantiation()
    {
        VirtualMachine vm = new VirtualMachine();

        Assert.IsTrue(vm != null);
    }

    [TestMethod]
    public void TestNOP()
    {
        VirtualMachine vm = new VirtualMachine();

        vm.Execute(Bytecode.NOP);

        // If we get here, we're good
        Assert.IsTrue(vm != null);
    }

    [TestMethod]
    public void TestNOPs()
    {
        VirtualMachine vm = new VirtualMachine();
        vm.Execute(new Bytecode[] {
            Bytecode.NOP,
            Bytecode.NOP
        });

        // If we get here, we're good
        Assert.IsTrue(vm != null);
    }

    [TestMethod]
    public void TestPRINT()
    {
        VirtualMachine vm = new VirtualMachine();
        
        vm.Execute(new Bytecode[] {
            Bytecode.CONST, (Bytecode)12,
            Bytecode.PRINT
        });
        
        // Check out the output window to see if it printed

        // If we get here, we're good
        Assert.IsTrue(vm != null);
    }

    [TestMethod]
    public void TestDUMP()
    {
        VirtualMachine vm = new VirtualMachine();

        vm.Execute(new Bytecode[] {
            Bytecode.NOP,
            Bytecode.DUMP
        });

        // If we get here, we're good
        Assert.IsTrue(vm != null);
    }
    
    [TestMethod]
    public void TestTRACE()
    {
        VirtualMachine vm = new VirtualMachine();

        vm.Execute(new Bytecode[] {
            Bytecode.TRACE,
            Bytecode.NOP,
            Bytecode.DUMP
        });

        // If we get here, we're good
        Assert.IsTrue(vm != null);
    }
}