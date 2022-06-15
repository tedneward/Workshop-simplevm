namespace vm;

public enum Bytecode 
{
    NOP,
    DUMP,
    TRACE,
    PRINT,
    HALT,
    FATAL,

    // Stack opcodes
    CONST,
    POP,

    // Math opcodes (binary)
    ADD,
    SUB,
    MUL,
    DIV,
    MOD,

    // Math opcodes (unary)
    ABS,
    NEG,

    // Comparison
    EQ,
    NEQ,
    GT,
    LT,
    GTE,
    LTE,

    // Branching opcodes
    JMP,
    JMPI,
    RJMP,
    RJMPI,
    JZ,
    JNZ,

    // Globals
    GSTORE,
    GLOAD,

    // Procedures/locals
    CALL,
    RET,
    LOAD,
    STORE
}


public class VirtualMachine
{
    // Tracing
    //
    bool trace = false;
    private void Trace(string message)
    {
        if (trace)
            Console.WriteLine("TRACE: {0}", message);
    }

    // Diagnostics
    //
    private void Dump()
    {
        Console.WriteLine("SimpleVM - DUMP");
        Console.WriteLine("===============");
        // Uncomment when you implement Execute(Bytecode[])
        /*
        Console.WriteLine("IP: {0} / Trace: {1}", IP, trace);
        Console.WriteLine("Working stack (SP {0}): {1}", SP, String.Join(", ", Stack));
        */
        // Uncomment when you implement global values
        /*
        Console.WriteLine("Globals: {0}", Globals);
        */
        // Uncomment when you implement CallFrames (procedures)
        /*
        Console.WriteLine("Call stack: ");
        for (int f = Frames.Count - 1; f > -1; f--) {
            CallFrame cf = Frames[f];
            Console.WriteLine("  Call Frame {0}:", f);
            Console.WriteLine("  +- ReturnAddr: {0}", cf.ReturnAddress);
            Console.WriteLine("  +- Locals: {0}", cf.Locals);
        }
        */
    }

    public void Execute(Bytecode opcode, params int operands)
    {

    }
    public void Execute(Bytecode[] code)
    {
        
    }
}
