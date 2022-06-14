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

Our `VirtualMachine` needs some basic infrastructure to get started. Our VM will be an extremely simple stack-based machine, so as to keep the infrastructure needs lightweight and straightforward. We will work solely wth integers (no floats, no strings, no booleans, etc) to keep things simple, and we will work to start by handling the simplest opcodes, which either do nothing (NOP), or give us some diagnostic insight and/or control.

There are some tests already defined in the respective test projects; if you use different names than the ones described here, refactor the tests accordingly, and by all means, feel free to add a few more tests if you see opportunities to do so.

* We want to begin by implementing the world's simplest opcode: `NOP`, which literally does nothing. Examine the type `Bytecode`, wherein we've defined our bytecode set.

    Within that class, `NOP` is defined to be a value of 0. (Most bytecode and CPU instructions set `NOP` to 0 for a variety of reasons both historical and practical.) Note, however, that for most of this lab, the actual integer value of each opcode is irrelelvant--this is why we use the `Bytecode` symbolic-constants/enumerations, to hide the actual value of each. (During early stages of development, the values/encodings will often change.)

* The processor cycle (fetch-decode-execute) is easier to understand if we break it apart. We'll worry about fetch and decode later; create a method, `execute` (depending on your language's naming conventions) that takes a single `int` parameter (the `opcode`) and a variable-length array of integers for `operands`. Within this method, you will examine the opcode and (at some point) any operands that are required to execute it. Put some error-handling in here to throw an exception if the opcode is not recognized. Make this method publicly accessible so we can call it from tests.
    * For a `NOP` value, do nothing.
    * Implement the `DUMP` opcode that dumps the current state of the VM to console (or logfile, if you want to build one out). (This method is already written for you in the `VirtualMachine` class.)
    * Implement the `TRACE` opcode that flips the status of a private `trace` boolean field (from true to false or false to true). (The "trace" method, a private method that takes a message and prints it to console or log file if `trace` is set to true, is already written for you. Use this method to help trace execution within the VM while writing/testing new opcodes through this workshop.)
    * Implement the `HALT` opcode, which terminates the fetch-decode-execute loop.
    * Implement the `FATAL` opcode, which throws an exception immediately. (This opcode would be used in places where we want to have an opcode that "should never be executed" as part of testing.)

* Given that this is a stack machine, we need an execution stack on which to push and pop values that will be the input and output for our various opcodes. We will use a fixed-size stack of integers, and we will "push" elements ("grow the stack") towards 100; the first element will be pushed at array index 0, the second at array index 1, and so on. For simplicity's sake, we will not worry about growing the stack or shrinking it. We'll want a reference to the "top" of the stack, and for testing purposes we'll want to be able to see the stack from the outside of the VM.

    > **NOTE**: If we truly didn't care about performance, we could use a higher-level abstract data type class, such as Stack class, to represent our execution stack, but this approach is more accurate to what a virtual machine actually does.

    * Create an array of 100 integers and call it `stack`. This will be the raw stack. 
    * Create an integer that points to the current index of the top of the stack; this is our "stack pointer", so call it `SP`.
    * Create two public methods, `push`/`Push` (which takes an integer and returns nothing) and `pop`/`Pop` (which takes nothing and returns an integer) that push or pop the value at the top of the stack respectively. Make sure `SP` gets incremented or decremented correctly with each push or pop.
    * Create a public method/property (`getStack()`/`Stack`) that returns a copy of the current contents of the stack. For easier diagnostics, make sure to only return the part of the stack that is in use--so if the current SP is 2, only return elements 0 through 2 of the total stack. If the current SP is -1, then return an empty array (the stack is empty).

* Now let's put two of the core stack machine opcodes into place: `CONST`, which takes one operand containing a value to push onto the stack, and `POP`, which pops a value off the stack (and throws it away). Note that `CONST` has one quirk to it, in that the value to push will be embedded in the code itself, so it will be masquerading as a `Bytecode` value. This is, in many respects, the "decode" part of the fetch-decode-execute cycle. (For those languages in which enumeration types are backed by an integer, you'll need to cast from `Bytecode` to `int` and back again to keep the compilers happy.)

    > **NOTE**: This means that any given collection of code will actually contain values that aren't entirely code, and this is true of all assembly languages. In many more optimized bytecode or CPU sets, these values can be "packed" together with the opcode value to allow for more efficient storage, and thus "unpacking" these values becomes a core part of the "decode" step. To keep things simple, we will not be exploring this packing/unpacking behavior.

    Make sure to write tests that ensure the stack works correctly. You shouldn't need to be too exhaustive--once you've verified that pushes advance the SP and pops reduce it, and that values appear on the stack and then are gone again, we can move on. (In a production implementation, )

* Implement the overloaded `execute` method that takes an array of `Bytecode` and executes them in sequence, until we run out of them. Loop through the array of bytecode, extracting additional operands as necessary (only `CONST` will need to do this so far) and passing them to the single-opcode version of `execute` you wrote earlier.

* For our own purposes (and to make the VM more interesting), implement a `PRINT` opcode that will print (and consume) the contents of the top of the stack. So a sequence of `CONST 5 PRINT` should print "5" to the console (and leave an empty stack behind).

* *(Optional)* Although we won't get to needing it in this workshop, most stack-based virtual machines have another stack-manipulation instruction called `DUP` that pops the current top of the stack, then pushes it twice. (It is frequently used when a local variable is used as part of a procedure call, since the values passed on the stack are consumed by the called procedure.) Implement it (and some tests for it) as a test for understanding.
