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

## Step 3: Global storage
`git checkout step-3`

Globals may seem a strange thing to implement next, but they help round out the last bits of a functional virtual machine: variable storage and retrieval.

> **NOTE:** Despite the negative connotations that might be associated with the term "globals", global storage is present in every virtual machine of note, though it is often used for purposes not directly identified. In the JVM, for example, it is in "global storage" areas that static variables are stored (which is what allows them to survive indefinitely if they are objects). Take care to differentiate this "static storage" from general-purpose "heap" storage--that is typically a different sort of storage altogether, and is typically managed automatically (aka garbage-collected).

There's only two points of implementation required to give us global storage:

* Implement the storage mechanism itself. In the case of simplevm, we will use another fixed-size array of 32 integers to represent all the global storage we will want or need. Create this array as a field (`globals`) in the `VirtualMachine`, and create a method (`getGlobals`) to access the array by index to load/store values.

> **NOTE:** If this were a longer-lived, more-sophisticated system, we might create some conventions around specific use-cases for certain indices of these globals, effectively turning them into general-purpose registers. In fact, given the way we've implemented the stack pointer, it wouldn't be that hard to make the decision that `globals[0]` is the stack pointer, and refactor all of the `sp`-related code accordingly. That way, if there are any additional "registers" we feel like implementing, we need only set the convention accordingly.

* Implement the `GSTORE` and `GLOAD` opcodes. `GSTORE` takes an index (into the globals array) and stores the top of the stack into that global; `GLOAD` also takes an index, and pushes the value found at that index in the globals array onto the operations stack.

## Interlude: Write some bytecode!
The addition of globals to the system gives us pretty much all the basics we need to do some more-extensive tests/experiments. Consider some of the bytecode patterns displayed below, and explore what additional ones you could write. (Note: No guarantees that these all work without bugs--writing assembly is not easy!)

* Even or odd (if/then/else)

    ```
    /* 0*/ CONST, 5,  // Define the value to be tested
    /* 2*/ CONST, 2,  // Push 2 
    /* 4*/ MOD,       // Mod
    /* 5*/ DUMP,      // Let's see what's on the stack
    /* 6*/ JZ, 9,     // If it's 0, it divided evenly
    /* 8*/ FATAL,     // 5 % 2 shouldn't be 0!
    /* 9*/ NOP
    ```

* 3... 2... 1... blastoff (do/while): count down from 5 to 0

    ```
    /* 0*/ GSTORE, 0,   // move count into globals[0]
    /* 2*/ GLOAD, 0,    // print globals[0]
    /* 4*/ PRINT,       // 
    /* 5*/ GLOAD, 0,    // globals[0]
    /* 7*/ CONST, 0,    // 0
    /* 9*/ EQ,          // globals[0] == 0 ?
    /*10*/ JNZ, 20,     // jump to end
    /*12*/ GLOAD, 0,    // globals[0] = globals[0] - 1
    /*14*/ CONST, 1,    // 
    /*15*/ SUB,
    /*17*/ GSTORE, 0,
    /*19*/ JMP, 2,      // jump to top of loop
    /*20*/ NOP          // all done
    ```

* Factorial (non-recursive)

    ```
    // Calculate 5!
    // globals[0] will hold the "factor" (the parameter)
    // globals[1] will be the "accumulator" (the calculations and final result)
    /* 0*/ CONST, 5,
    /* 2*/ GSTORE, 0,   // globals[0] = 5
    /* 4*/ GLOAD, 0,
    /* 6*/ GSTORE, 1,   // globals[1] = globals[0]
    /* 8*/ GLOAD, 0,    // print globals[0]
    /* 9*/ PRINT,
    /*11*/ GLOAD, 0,    // globals[0] = globals[0] - 1
    /*13*/ CONST, 1,
    /*14*/ SUB,
    /*16*/ GSTORE, 0,
    /*18*/ GLOAD, 0,    // globals[0]
    /*20*/ JZ, 31,      //   ... == 0? if yes, break out of loop
    /*22*/ GLOAD, 1,    // globals[1] = globals[1] * globals[0]
    /*24*/ GLOAD, 0,
    /*25*/ MUL,
    /*27*/ GSTORE, 1,
    /*29*/ JMP, 11,     // jump to line 11, our "top of loop"
    /*31*/ GLOAD, 1,    // print globals[1]
    /*33*/ PRINT
    ```
