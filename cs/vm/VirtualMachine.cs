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
        Console.WriteLine("IP: {0} / Trace: {1}", IP, trace);
        Console.WriteLine("Working stack (SP {0}): {1}", SP, String.Join(", ", Stack));
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

    // Stack management
    //
    int SP = -1; // points to the current top of stack
    int[] stack = new int[100];
    public int[] Stack { get { return stack.Take(SP+1).ToArray(); } }
    public void Push(int operand)
    {
        Trace("Push: " + operand);
        stack[++SP] = operand;
        Trace(" -->  Stack: " + String.Join(",", stack));
    }
    public int Pop()
    {
        Trace("Pop");
        int result = stack[SP--];
        Trace(" -->  Stack: " + String.Join(",", stack));
        return result;
    }



    public void Execute(Bytecode opcode, params int operands)
    {
        switch (opcode)
        {
            case Bytecode.NOP:
                // Do nothing!
                Trace("NOP");
                break;
            case Bytecode.DUMP:
                Trace("DUMP");
                Dump();
                break;
            case Bytecode.TRACE:
                trace = !trace;
                Trace("TRACE " + trace);
                break;
            case Bytecode.PRINT:
                Trace("PRINT");
                Console.WriteLine(Pop());
                break;
            case Bytecode.HALT:
                Trace("HALT");
                return;
            case Bytecode.FATAL:
                Trace("FATAL");
                throw new Exception(String.Format("FATAL exception thrown; IP {0}",IP));
            case Bytecode.CONST:
                int operand = operands[0];
                Trace("CONST " + operand);
                Push(operand);
                break;
            case Bytecode.POP:
                Trace("POP");
                // throw away returned value
                Pop();
                break;

            // Mathematical ops
            case Bytecode.ADD:
            {
                Trace("ADD");
                int rhs = Pop();
                int lhs = Pop();
                Push(lhs + rhs);
                break;
            }
            case Bytecode.SUB:
            {
                Trace("SUB");
                int rhs = Pop();
                int lhs = Pop();
                Push(lhs - rhs);
                break;
            }
            case Bytecode.MUL:
            {
                Trace("MUL");
                int rhs = Pop();
                int lhs = Pop();
                Push(lhs * rhs);
                break;
            }
            case Bytecode.DIV:
            {
                Trace("DIV");
                int rhs = Pop();
                int lhs = Pop();
                Push(lhs / rhs);
                break;
            }
            case Bytecode.MOD:
            {
                Trace("MOD");
                int rhs = Pop();
                int lhs = Pop();
                Push(lhs % rhs);
                break;
            }
            case Bytecode.ABS:
            {
                Trace("ABS");
                int val = Pop();
                Push(Math.Abs(val));
                break;
            }
            case Bytecode.NEG:
            {
                Trace("NEG");
                int val = Pop();
                Push(-val);
                break;
            }
            
        }
    }
    int IP = -1;
    public void Execute(Bytecode[] code)
    {
        for (IP = 0; IP < code.Length; )
        {
            Bytecode opcode = code[IP];
            switch (opcode)
            {
                // 0-operand opcodes
                case Bytecode.NOP:
                case Bytecode.DUMP:
                case Bytecode.TRACE:
                case Bytecode.PRINT:
                case Bytecode.FATAL:
                case Bytecode.POP:
                    Execute(opcode);
                    break;

                // 1-operand opcodes
                case Bytecode.CONST:
                    int operand = (int)code[++IP];
                    Execute(opcode, operand);
                    break;

                // 2-operand opcodes

                // Special handling to bail out early
                case Bytecode.HALT:
                    return;

                // Unrecognized opcode
                default:
                    throw new Exception("Unrecognized opcode: " + code[IP]);
            }
            IP++;
        }
    }
}
