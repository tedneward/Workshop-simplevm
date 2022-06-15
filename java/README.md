# SimpleVM: Java Implementation
This directory contains the Java implementation of the SimpleVM stack-machine implementation.

Given that this is greenfield work, the project uses latest-and-greatest versions of Java at the time of its creation. Currently that means:

* Gradle 7.4.2
* Java 18

The root Gradle settings.gradle.kts contains references to the `vm` project, which is the virtual machine implementation project. Tests for `vm` are found in that project.

## Running tests
`gradle test` from the top-level directory, or drop into `vm` and `gradle test` from there (which is slightly faster).

## Step 1: Mathematics and Calculations
`git checkout step-1`

With basic diagnostics and stack manipulation in place, we can implement some mathematical operations. We have two different kinds of mathematical operations: binary operations, which take two values to carry out, and unary operations, which require only one. `ADD`, `SUB`, `MUL`, `DIV`, and `MOD` are all binary operations, taking two values; for unary operations we will focus only on two: `ABS`, which will figure the absolute value, and `NEG`, which will convert a positive value to negative, and vice versa.

Unary operations will take one parameter off the stack, operate on it, and push the result back to the stack.

Binary operations will take two parameters off the stack, operate on them, and push the result to the stack. Because `SUB`, `DIV`, and `MOD` are position-sensitive (`5 - 2` is a different operation than `2 - 5`), we need a convention for passing parameters so we can know which is the left-hand-side and which is the right. Either we can pass the parameters "left-to-right" (which means left is bottommost on the stack, and right is the top), or the opposite, "right-to-left" (which means left is at the top and right on the bottom). There's strong arguments for either approach; this workshop assumes we will take a "left-to-right" approach (mostly because it's easier for us when writing the raw bytecode). So for binary operations like this, the top of the stack will be the right-hand operand, and the next one down will be the left-hand operand.

When these seven opcodes are all in place (and the tests pass), you're ready to move on to the next step.

> **NOTE**: Many of you will recognize that we have effectively implemented a Reverse-Polish Notation (RPN) calculator.

*Optional:* Take a shot at implementation a "summation" operator for addition (and another for multiplication) that takes a variable number of arguments. One way to implement this opcode (call summation-add `SIGADD` and summation-multiply `SIGMUL`, in honor of "sigma", the Greek letter used for "summations" in algebra) would be to pop the topmost value off the stack, containing the number of arguments in the operation, and then pop all of those values off the stack, do the math on each, and then push the result. Thus `CONST 5 CONST 10 CONST 2 SIGADD` would pop the "2", pop two more values (the "10" and the "5"), add all of them ("15"), and push the result to the top of the stack. Assume that `SIGADD` and `SIGMUL` with a 0 for the number of arguments pushes `0` as a result (and maybe generates a warning, because why would this be at all useful?).
