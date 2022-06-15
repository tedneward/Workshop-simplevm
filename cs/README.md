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

## Step 2: Comparisons and Branching
`git checkout step-2`

Any programmatic environment needs to have the ability to evaluate values and take differing actions depending on the result of that comparison. This means we have two need categories of bytecode we need to implement: comparison operations, which will take values off the stack, evaluate them, and push a result (1 for true, 0 for false) back onto the stack, and branching operations, will will change the instruction pointer to tell the virtual machine to execute a different bytecode instruction next.

As we will see, it is often convenient (and common) to combine comparison and branching operations together into single opcodes, and/or to have multiple varieties of comparison operations. We will start with fundamentals, and then build out some convenience opcodes from there.

* The simplest comparison operation is equality--take two values off the stack, and if they are equal, push a "1" onto the stack, otherwise push "0". Call the opcode `EQ`. While you're at it, implement the inverse operation, `NEQ`, to test for not-equals.
* The next-simplest comparisons are the relative ones: greater-than (`GT`) and less-than (`LT`). Again, like `SUB`, these require positional sensitivity; `1 < 2` is different from `1 > 2`.
* Similar sorts of comparison operations are combinations of these: greater-than-or-equals (`GTE`), less-than-or-equals (`LTE`), and so on.

This is a pretty good collection of comparison opcodes.

Next let's do some branching opcodes, the easiest of which is `JMP` (an unconditional goto), which will take one operand: the address to jump to. In our simplified bytecode set, that "address" will be the 0-indexed index of the instruction in the array of bytecode, and will be passed in as an operand to the opcode. Thus, `JMP 6` means to jump to the 7th element (`code[6]`) in the bytecode array and execute it next.

> **NOTE**: The absolute nature of the jump target allows us (either deliberately or accidentally) to jump to an operand rather than an opcode--for example, consider the sequence "CONST 5 JMP 1", which would push a 0 onto the stack, then jump to the second element in the array (which is the operand `5` to the `CONST` instruction), and execute it as if it were bytecode (which is the `PRINT` instruction) before continuing on and JMPing again... This is, in fact, doable under a number of assembly languages, and for a time being able to jump into the middle of an instruction and have it execute correctly was considered a hallmark of the master programmer. (Most of the rest of us considered it insane in the extreme.)

We can also implement "indirect" jumps, which jumps to a location as specified at runtime rather than at the time the bytecode was written ("compile-time").

* Implement `JMPI`, an "indirect jump", which jumps to the location specified by the value at the top of the stack. So `CONST 0 JMPI` jumps back to the very start of the bytecode (location 0).

Sometimes calculating the absolute positions of where to jump is hard, so it's easier to have the bytecode take jump target locations that are relative to the current position.)

* Implement `RJMP`, which is a "relative jump", which jumps a number of bytecode positions (positive or negative) from its current location. So `RJMP -1` would jump back to the instruction previous to the RJMP, and `RJMP 1` would jump to the next instruction (which would really accomplish the exact same thing as doing nothing). Keep in mind that `RJMP 0` would effectively be an infinite loop--we just keep jumping to the same instruction. (What you do with that knowledge is up to you--is that an error? A fatal error? A desirable outcome?)
* Additionally, it can be helpful to jump "indirectly" to a "relative" location, so implement `RJMPI`, which jumps a number of bytecode positions relative to the current location by the amount specified at the top of the stack. So `CONST 5 RJMPI` means to pop the stack and examine the value; since it's 5, jump 5 positions ahead of the current location. Similarly, `CONST -5 RJMPI` would jump -5 spots.

Once that's done...

* Implement `JZ`, a jump-if-zero bytecode, which combines a `EQ 0` with a `JMP`. It should take one operand (the index to jump to if the top value on the stack is zero). (This is sometimes called "JF"/jump-if-false in some bytecode sets.)
* Implement `JNZ`, a jump-if-not-zero bytecode, which jumps if the top-of-the-stack is NOT zero. (This is sometimes called "JT"/jump-if-true in some bytecode sets.)

*Optional:* Some bytecode sets don't care for the word "jump", preferring "branch" instead. Since the only real value we care about is the integer value behind an opcode, you can create "aliases" in the `Bytecode` type by giving two mnemonics to the same value. So, for example, you can easily add "branch-if-true" as `BRT` in the Bytecode by giving it the exact same integer value as `JNZ` (such as writing `BRT = JNZ,` inside the `Bytecode` enumeration). Whether "branch" should take absolute addresses or relative ones is, of course, up to you.

*Optional:* Implement the following "shortcut" compare-and-branch instructions:

* `JEQ` "jump-if-equal": combine the `EQ` and `JT` opcodes. The top of stack (`SP`) holds the absolute address to jump to, and the next two stack operands (`SP - 1` and `SP - 2`) are the two values to compare for equality, pushed in left-to-right order (so left is at `SP - 2` and right is at `SP - 1`).
* `JNEQ` "jump-if-not-equal": combine the `NEQ` and `JT` opcodes. The top of stack (`SP`) holds the absolute address to jump to, and the next two stack operands (`SP - 1` and `SP - 2`) are the two values to compare for equality, pushed in left-to-right order (so left is at `SP - 2` and right is at `SP - 1`).
* `JGT` "jump-if-greater-than": combine the `GT` and `JT` opcodes. The top of stack (`SP`) holds the absolute address to jump to, and the next two stack operands (`SP - 1` and `SP - 2`) are the two values to compare, pushed in left-to-right order (so left is at `SP - 2` and right is at `SP - 1`). Jump only if left (`SP - 2`) is greater-than right (`SP - 1`).
* `JGTE` "jump-if-greater-than-or-equal": combine the `GTE` and `JT` opcodes. The top of stack (`SP`) holds the absolute address to jump to, and the next two stack operands (`SP - 1` and `SP - 2`) are the two values to compare, pushed in left-to-right order (so left is at `SP - 2` and right is at `SP - 1`). Jump only if left (`SP - 2`) is greater-than/equal-to right (`SP - 1`).
* `JLT` "jump-if-less-than": combine the `LT` and `JT` opcodes. The top of stack (`SP`) holds the absolute address to jump to, and the next two stack operands (`SP - 1` and `SP - 2`) are the two values to compare, pushed in left-to-right order (so left is at `SP - 2` and right is at `SP - 1`). Jump only if left (`SP - 2`) is less-than right (`SP - 1`).
* `JLTE` "jump-if-less-than-or-equal": combine the `LTE` and `JT` opcodes. The top of stack (`SP`) holds the absolute address to jump to, and the next two stack operands (`SP - 1` and `SP - 2`) are the two values to compare, pushed in left-to-right order (so left is at `SP - 2` and right is at `SP - 1`). Jump only if left (`SP - 2`) is less-than/equal-to right (`SP - 1`).
* There's dozens more combinations: relative-jump versions of all of the above, for example, or the negative-versions of them ("JNGT", jump if not greater than), to the point where it could get absurd. Choosing just the "right" number of these is a fine art and often requires some trial-and-error. The only way to really know is to write some bytecode programs.
