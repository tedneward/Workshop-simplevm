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
    public void Execute(Bytecode opcode, params int operands)
    {

    }
    public void Execute(Bytecode[] code)
    {
        
    }
}
