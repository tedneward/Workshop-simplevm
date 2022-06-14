package simplevm.vm;

import java.util.Arrays;
import java.util.LinkedList;
import java.util.List;

import static simplevm.vm.Bytecode.*;

public class VirtualMachine {

    public static class Exception extends java.lang.RuntimeException {
        public Exception(String message) {
            super(message);
        }
    }
    
    public void execute(int opcode, int... operands) {

    }
    public void execute(int[] code) {

    }
}
