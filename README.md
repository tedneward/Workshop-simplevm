# Workshop-simplevm
Files and templates to go along with my "Busy Dev's Workshop on Building a Virtual Machine".

Because this workshop is intended for developers of all stripes, there are several subdirectories here with implementation scaffolding for different language/platforms:

* C#
* Java

Developers are free to choose whichever of the subdirectories they wish. Alternatively, if comfortable with doing so, feel free to create a new subdirectory and implement it in your own language of choice. Virtual machines have been written in almost every language in the universe, including Smalltalk and JavaScript and Self, and the concepts we are exploring here are not so low-level that performance or overhead will be a problem.

## C#
There is a dotnet 6.0 proejct set up to use C# to implement the Workshop in the cs subdirectory from here. See that README for details.

## Java
There is a Gradle project set up to use Java to implement the Workshop in the java subdirectory from here. See that README for details.

# Getting Started
There are several steps involved in this workshop, each of them beginning with a particular Git branch. To begin each step, checkout the appropriate branch name; to see the solution I came up with, you can check out the subsequent branch name (so to see what I wrote for Step 0, for example, checkout Step 1's branch).

The steps are as follows:

* **Step 0:** Basic infrastructure; in here, you put the basic fetch-decode-execute functionality in place to be able to execute a bytecode instruction, and then write a few diagnostic options (and associated opcodes) into the VirtualMachine.
* **Step 1:** Maths; you flesh out the basic mathematical binary operations (add, subtract, multiply, divide, modulo), as well as some unary operations (absolute value, negation).
* **Step 2:** Comparisons and branching; opcodes to compare two operands, as well as opcodes to adjust the instruction pointer (i.e., what instruction gets executed next).
* **Step 3:** Global values; opcodes to load and store globals, along with the necessary VirtualMachine support to hold on to them.
* **Step 4:** Procedures and locals; opcodes to create a new call frame, jump to a new location, while preserving the return location for when that "procedure" is complete. Includes support for local (to the call frame) variables.

If you're ready for all this, take the first step and `git checkout step-0` to begin!
