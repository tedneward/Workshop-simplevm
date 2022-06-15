# SimpleVM: Java Implementation
This directory contains the Java implementation of the SimpleVM stack-machine implementation.

Given that this is greenfield work, the project uses latest-and-greatest versions of Java at the time of its creation. Currently that means:

* Gradle 7.4.2
* Java 18

The root Gradle settings.gradle.kts contains references to the `vm` project, which is the virtual machine implementation project. Tests for `vm` are found in that project.

## Running tests
`gradle test` from the top-level directory, or drop into `vm` and `gradle test` from there (which is slightly faster).

## Step 0: Infrastructure
`git checkout step-0`

Our `VirtualMachine` needs some basic infrastructure to get started. Our VM will be an extremely simple stack-based machine, so as to keep everything lightweight and straightforward. We will work solely wth integers (no floats, no strings, no booleans, etc), and we start by handling the simplest opcodes, which either do nothing (NOP), or give us some diagnostic insight and/or control.

There are some tests already defined in the respective test projects; if you use different names than the ones described here, refactor the tests accordingly, and by all means, feel free to add a few more tests if you see opportunities to do so.

There are two classes defined for you (in the `vm/src/main/java/simplevm/vm` directory): one is `VirtualMachine`, which is (not surprisingly) where most of the work you will do will be, and the other is a `Bytecode` class that contains the symbolic values for all of our opcodes. Our goal for this workshop will be to implement all of these opcodes in the SimpleVM.

* The abstract virtual machine processor cycle (fetch-decode-execute) is easier to understand if we break it apart. We'll worry about fetch and decode later; let's focus on execute for now. Within the `execute` method that takes a single `int` parameter (the `opcode`) and a variable-length array of integers (for `operands`), write code to examine the opcode (and any operands that are required to execute it) and implement the below functionality.
    * `NOP` (no-operation): do nothing.
    * `DUMP` (diagnostic dump): dumpd the current state of the VM to console (or logfile, if you want to build one out). (The `dump` method is already written for you in the `VirtualMachine` class, though parts of it are commented out--uncomment those as we get to them in the workshop.)
    * `TRACE` (diagnostic trace): flip the status of the private `trace` boolean field (from true to false or false to true). This flag is used by the `trace` method, which will print out step-by-step events as they occur in the SimpleVM (if tracing is turned on). Take a moment to put calls to `trace` into all of the opcode-execution paths (and make sure to add them to future opcode paths, too) so that you can trace the path of the SimpleVM's execution of opcodes when you wish. 
    * `HALT`: halt all bytecode execution. (This opcode usually implies terminating all virtual machine execution.)
    * `FATAL`: throw an exception immediately. (This opcode would be used in places where we want to have an opcode that "should never be executed" as part of testing.)

    In many, if not most, simple virtual machines, this all is a giant `switch` statement, but you are free to use any other mechanism that works. Put some error-handling in here to throw an exception if the opcode is not recognized. (This, along with tracing, will help immensely when debugging branching/jump instructions.)

* Given that this is a stack machine, we need an execution stack on which to push and pop values that will be the input and output for our various opcodes. We will use a fixed-size stack of 100 integers to represent our stack. (It's a simple VM.) We will "push" elements ("grow the stack") towards 100; that is to say, the first element will be pushed at array index 0, the second at array index 1, and so on. For simplicity's sake, we will not worry about growing the stack or shrinking it. We'll want a reference to the "top" of the stack, which we will call the "stack pointer", and for testing purposes we'll want to be able to see the stack from the outside of the VM:

    * Create an array of 100 integers and call it `stack`. This will be the raw "operations" stack, and be where we provide operands for many/most of the opcodes in the SimpleVM.
    * Create an integer that points to the current index of the top of the stack; this is our "stack pointer", so call it `sp`. It should always point to the index of the topmost stack element. (If we push three items onto the stack, it should hold `2`, which is the topmost stack element.) If the stack is entirely empty, have it hold `-1`.
    * Create two public methods, `push` (which takes an integer and returns nothing) and `pop` (which takes nothing and returns an integer) that push or pop the value at the top of the stack respectively. Make sure `sp` gets incremented or decremented correctly with each push or pop.
    * Create a public method/property (`getStack()`) that returns a copy of the current contents of the stack. For easier diagnostics, make sure to only return the part of the stack that is in use--so if the current SP is 2, only return elements 0 through 2 of the operations stack. If the current SP is -1, then return an empty array (the stack is empty).
    * Go through the tests and uncomment the code that references these fields and methods.

    > **NOTE:** If we truly didn't care about performance, we could use a higher-level abstract data type class, such as Stack class, to represent our execution stack. Although performance isn't really a concern for the SimpleVM, learning how VMs work is, and this approach is more accurate to what a virtual machine actually does.

* Now let's put two of the core stack machine opcodes into place: `CONST`, which takes one operand containing a value to push onto the stack, and `POP`, which pops a value off the stack (and throws it away). Note that `CONST` has one quirk to it, in that the value to push will be embedded in the code itself, masquerading as a `int`/bytecode value. This is, in many respects, the "decode" part of the fetch-decode-execute cycle.

    > **NOTE:** This means that any given collection of code will actually contain values that aren't entirely code, and this is true of all assembly languages. In many more optimized bytecode or CPU sets, these values can be "packed" together with the opcode value into a single 8-, 16- or 32-bit value, to allow for more efficient storage. Thus "unpacking" these values becomes a core part of the "decode" step. To keep things simple, we will not be exploring this packing/unpacking behavior. This is where much of the interesting parts of studying virtual machines comes, in understanding these sorts of design decisions. For example, the JVM (which encodes all of its instructions into 8-bit bytes, rather than our 32-bit words here) has an opcode specifically to push a "0" value (0, 0.0, false, null, etc) onto the stack. The Android RunTime encodes opcode and up to two operands into 32-bit values. Python, meanwhile, uses 2 bytes per instruction. This "packing" can create some space efficiencies, but at a (slight) performance cost "unpacking" the instructions at runtime.

    Make sure to write tests that ensure the stack works correctly. You shouldn't need to be too exhaustive--once you've verified that pushes advance the SP and pops reduce it, and that values appear on the stack and then are gone again, we can move on. (In a production implementation, )

* Implement the overloaded `execute` method that takes an array of `int` and executes them in sequence, until we run out of them. Loop through the array of bytecode, extracting additional operands as necessary (only `CONST` will need to do this so far) and passing them to the single-opcode version of `execute` you wrote earlier.

* For our own purposes (and to make the VM more interesting), implement the `PRINT` opcode that will print (and consume) the contents of the top of the stack. So a sequence of `CONST 5 PRINT` should print "5" to the console (and leave an empty stack behind, since `PRINT` consumed the top of the stack when it printed it).

* *(Optional)* Although we won't get to needing it in this workshop, most stack-based virtual machines have another stack-manipulation instruction called `DUP` that pops the current top of the stack, then pushes it twice. (It is frequently used when a local variable is used as part of a procedure call, since the values passed on the stack are consumed by the called procedure.) If you have the time, implement it (and some tests for it) as a test for understanding.
