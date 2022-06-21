# SimpleVM: Java Implementation
This directory contains the Java implementation of the SimpleVM stack-machine implementation.

Given that this is greenfield work, the project uses latest-and-greatest versions of Java at the time of its creation. Currently that means:

* Gradle 7.4.2
* Java 18

The root Gradle settings.gradle.kts contains references to the `vm` project, which is the virtual machine implementation project. Tests for `vm` are found in that project.

## Running tests
`gradle test` from the top-level directory, or drop into `vm` and `gradle test` from there (which is slightly faster).

## Step 4: Locals and procedures
`git checkout step-4`

Procedure calls, from an assembly-level perspective, are essentially a formalized convention around three things:

* the location of parameters to the procedure call,
* the location of the return address, and
* a scheme by which local variables can be declared and reclaimed at the termination of the procedure call

For the simplevm, we will create a collection of "stack frames" (also known as "activation records", or by the more colloquial "call stack") that exist as a separate entity to the operations stack. While a more close-to-x86 representation would have us store procedure parameters and local variables on the stack, it's much (*much*) easier to have a separate data structure to store the call stack--generate stack traces, for example, is much easier when there's a formal list of each call, along with the locals allocated within that stack frame.

> **NOTE:** This approach is the exact same approach as used by the JVM and CLR. Remember, everything we do here is an abstraction, and when (if) the simplevm reaches the point of doing JIT compilation to native CPU instructions, we would generation machine code to use the approach required by whatever the CPU's convention is.

The first thing we require is a `CallFrame` type (which we will drop into a `LinkedList` called `frames` and grow/shrink as calls are created). This `CallFrame` will contain only a few things:

* an int array called `locals`, which is where locals to the procedure will be allocated,
* an int called `returnAddress`, which will contain the IP location for where we wish to return when this procedure is finished
* a constructor to initialize `locals` to a fixed-size array of 32 integers. (**NOTE:** in some VMs, such as the JVM, the size of the locals storage space is determined at runtime by some metadata defined as part of the code being loaded--in the JVM, you can see this in `javap`-generated output as "locals: x". As it turns out, the JVM actually stores a great deal of information about locals in methods as part of its metadata, but only retains it if the "debug" flag is on during `javac` compilation.)

Thus:

* create the `CallFrame` type; if you choose to nest it inside of `VirtualMachine.java`, make sure it is declared as a "static" class, so that it doesn't drag along a pointer to the `VirtualMachine` in each and every instance of the `CallFrame`.
* create a field in the `VirtualMachine` called `frames` that is a `LinkedList` of `CallFrames`
* create a method in the `VirtualMachine` called `fp` that will return the topmost `CallFrame`. ("fp" is our "frame pointer", which should always point to the currently-active call frame.)

Once that's in place, we can implement the four opcodes that will give us procedure call and locals support:

* `CALL` will expect an address (integer offset into the code) as an operand. It will immediately create a new `CallFrame`, capture the return address (current IP plus 2), and add the `CallFrame` to `frames`.
* `RET` will look at the current `CallFrame`, extract the `returnAddress`, set `ip` to point to that value, and then remove the current `CallFrame` from `frames` entirely.
* `STORE` and `LOAD` will operate exactly as `GSTORE` and `GLOAD` do, with the caveat that they will modify the `locals` of the current `CallFrame` (instead of `globals`, as the `GSTORE`/`GLOAD` opcodes do).

At this point, if the tests pass, you have completed the simplevm workshop! You now have a working, highly-inefficient, overly-simplistic stack-based virtual machine, with a barely-documented bytecode set that is missing any interesting features whatsoever! Fame and fortune are sure to be yours.

It would be an interesting exercise to implement in the `VirtualMachine` the assumption that there is always at least one `CallFrame` active; that is to say, when we call `execute(int[])`, we create a `CallFrame` in which to execute that code. This would also give us locals support at the top-most level of code execution. (That in turn could spark some discussion as to whether we need "globals" when we have "locals" at this topmost level.) However, doing this requires that we always be keeping an eye on the `returnAddress` of any `CallFrame`, because if that points to -1 (or some other signal), it indicates we are at the topmost level of the call stack, and there's no place to return to.
