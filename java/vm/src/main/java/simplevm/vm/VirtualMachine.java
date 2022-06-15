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

    // Tracing
    //
    boolean trace = false;
    private void trace(String message) {
        if (trace) {
            System.out.println("(IP " + ip + "):" + message);
        }
    }

    // Dump
    //
    private void dump() {
        System.out.println("SimpleVM DUMP");
        System.out.println("=============");
        /*
        System.out.println("IP: " + ip);
        System.out.println("Working stack (SP " + sp + "): " + Arrays.toString(Arrays.copyOfRange(stack, 0, sp+1)));
        System.out.println("Globals: " + Arrays.toString(globals));
        System.out.println("Call stack: ");
        for (int f = frames.size(); f != 0; f--) {
            CallFrame cf = frames.get(f - 1);
            System.out.println("  Call Frame " + (f - 1) + ":");
            System.out.println("  +-- Return Address: " + cf.returnAddress);
            System.out.println("  +-- Locals: " + Arrays.toString(cf.locals));
        }
        */
    }

    public void execute(int opcode, int... operands) {

    }
    public void execute(int[] code) {

    }
}
