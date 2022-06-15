# SimpleVM: C# Implementation
This directory contains the C# implementation of the SimpleVM stack-machine implementation.

Given that this is greenfield work, the project uses latest-and-greatest versions of `dotnet` at the time of its creation. Currently that means:

* .NET SDK v6.0.300

The root SLN file contains references to the `vm` project, which is the virtual machine implementation project. Tests for `vm` are found in the peer `vmtest` project, and `app` doesn't do much besides provide a simple shell by which to run the VM, for those who prefer to see console output instead of test results.

## Running tests
`dotnet test` from the top-level directory will run the `vmtest` tests. There are no `app` tests.

If you use `Console.WriteLine` to display output, the standard test runner will not display it; to see the generated console messages, use `dotnet test -l "console;verbosity=detailed"` instead.

## Running the app
`dotnet run --project app` from the top-level directory will run the app, or drop into the `app` directory and do a `dotnet run` from there.

## Step 0: Infrastructure
`git checkout step-0`

Our `VirtualMachine` needs some basic infrastructure to get started. Our VM will be an extremely simple stack-based machine, so as to keep the infrastructure needs lightweight and straightforward. We will work solely wth integers (no floats, no strings, no booleans, etc), and we will start by handling the simplest opcodes: those that do nothing (NOP), or give us some diagnostic insight and/or control.

There are some tests already defined in the respective test projects; if you use different names than the ones described here, refactor the tests accordingly, and by all means, feel free to add a few more tests if you see opportunities to do so.

* We want to begin by implementing the world's simplest opcode: `NOP`, which literally does nothing. Examine the type `Bytecode`, wherein we've defined our bytecode set. It's a C# enum, which is convenient because it is backed by an `int` type.

    Within that enumeration, `NOP` is defined to be a value of 0. (Most bytecode and CPU instructions set `NOP` to 0 for a variety of reasons both historical and practical.) Note, however, that for most of this lab, the actual integer value of each opcode is irrelelvant--this is why we use the `Bytecode` symbolic-constants/enumerations, to hide the actual value of each. (During early stages of development, the values/encodings will often change.)

* The abstract virtual machine processor cycle (fetch-decode-execute) is easier to understand if we break it apart. We'll worry about fetch and decode later; let's focus on execute for now. Within the `Execute` method that takes a single `Bytecode` parameter (the `opcode`) and a variable-length array (for `operands`), write code to examine the opcode (and any operands that are required to execute it) and implement the below functionality:
    * `NOP` (no-operation): do nothing.
    * `DUMP` (diagnostic dump): dumpd the current state of the VM to console (or logfile, if you want to build one out). (The `Dump` method is already written for you in the `VirtualMachine` class, though parts of it are commented out--uncomment those as we get to them later in the workshop.)
    * `TRACE` (diagnostic trace): flip the status of the private `trace` boolean field (from true to false or false to true). This flag is used by the `Trace` method, which will print out step-by-step events as they occur in the SimpleVM (if tracing is turned on). Take a moment to put calls to `Trace` into all of the opcode-execution paths (and make sure to add them to future opcode paths, too) so that you can trace the path of the SimpleVM's execution of opcodes when you wish. 
    * `HALT`: halt all bytecode execution. (This opcode usually implies terminating all virtual machine execution.)
    * `FATAL`: throw an exception immediately. (This opcode would be used in places where we want to have an opcode that "should never be executed" as part of testing.)

    In many, if not most, simple virtual machines, this all is a giant `switch` statement, but you are free to use any other mechanism that works. Put some error-handling in here to throw an exception if the opcode is not recognized. (This, along with tracing, will help immensely when debugging branching/jump instructions.)
    
* Given that this is a stack machine, we need an execution stack on which to push and pop values that will be the input and output for our various opcodes. We will use a fixed-size stack of integers, and we will "push" elements ("grow the stack") towards 100; the first element will be pushed at array index 0, the second at array index 1, and so on. For simplicity's sake, we will not worry about growing the stack or shrinking it. We'll want a reference to the "top" of the stack, and for testing purposes we'll want to be able to see the stack from the outside of the VM.

    > **NOTE**: If we truly didn't care about performance, we could use a higher-level abstract data type class, such as Stack class, to represent our execution stack, but this approach is more accurate to what a virtual machine actually does.

    * Create an array of 100 integers and call it `stack`. This will be the raw stack. 
    * Create an integer that points to the current index of the top of the stack; this is our "stack pointer", so call it `SP`.
    * Create two public methods, `Push` (which takes an integer and returns nothing) and `pop` (which takes nothing and returns an integer) that push or pop the value at the top of the stack respectively. Make sure `SP` gets incremented or decremented correctly with each push or pop.
    * Create a public property (`Stack`) that returns a copy of the current contents of the stack. For easier diagnostics, make sure to only return the part of the stack that is in use--so if the current SP is 2, only return elements 0 through 2 of the total stack. If the current SP is -1, then return an empty array (the stack is empty). We'll use this in tests to verify everything works the way it should.

* Now let's put two of the core stack machine opcodes into place: `CONST`, which takes one operand containing a value to push onto the stack, and `POP`, which pops a value off the stack (and throws it away). Note that `CONST` has one quirk to it, in that the value to push will be embedded in the code itself, so it will be masquerading as a `Bytecode` value. This is, in many respects, the "decode" part of the fetch-decode-execute cycle. (You'll need to cast from `Bytecode` to `int` and back again to keep the compilers happy.)

    > **NOTE:** This means that any given collection of code will actually contain values that aren't entirely code, and this is true of all assembly languages. In many more optimized bytecode or CPU sets, these values can be "packed" together with the opcode value into a single 8-, 16- or 32-bit value, to allow for more efficient storage. Thus "unpacking" these values becomes a core part of the "decode" step. To keep things simple, we will not be exploring this packing/unpacking behavior. This is where much of the interesting parts of studying virtual machines comes, in understanding these sorts of design decisions. For example, the JVM (which encodes all of its instructions into 8-bit bytes, rather than our 32-bit words here) has an opcode specifically to push a "0" value (0, 0.0, false, null, etc) onto the stack. The Android RunTime encodes opcode and up to two operands into 32-bit values. Python, meanwhile, uses 2 bytes per instruction. This "packing" can create some space efficiencies, but at a (slight) performance cost "unpacking" the instructions at runtime.

    Make sure to write tests that ensure the stack works correctly. You shouldn't need to be too exhaustive--once you've verified that pushes advance the SP and pops reduce it, and that values appear on the stack and then are gone again, we can move on. (In a production implementation, )

* Implement the `PRINT` opcode that takes the top element off the operand stack, and prints it.

* Implement the method that takes an array of `Bytecode` and executes them in sequence, until we run out of them. Loop through the array of bytecode, extracting additional operands as necessary (only `CONST` will need to do this so far) and passing them to the single-opcode version of `Execute` you wrote earlier.

* For our own purposes (and to make the VM more interesting), implement a `PRINT` opcode that will print (and consume) the contents of the top of the stack. So a sequence of `CONST 5 PRINT` should print "5" to the console (and leave an empty stack behind).

* *(Optional)* Although we won't get to needing it in this workshop, most stack-based virtual machines have another stack-manipulation instruction called `DUP` that pops the current top of the stack, then pushes it twice. (It is frequently used when a local variable is used as part of a procedure call, since the values passed on the stack are consumed by the called procedure.) Implement it (and some tests for it) as a test for understanding.