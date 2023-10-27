# BrainSharp
### A set of simple Brainfuck development tools for .NET

Have you ever wanted to develop your apps in Brainfuck?

No?

<sup>Fuck.</sup>

In case you change your mind, I've made BrainSharp. It includes a code executor (with built-in debugger) and a minifer, with more tools on the way.

## Executor

`BrainSharp.exe -e <path> [options]`
- Execute a Brainfuck source file.
- Options:
    - `-o <path>` Specify output file path
    - `-d` Debug mode
    - `-m` Minify first (faster if source contains many comments)
    - `-a <size>` Specify data array size (default 30000)

## Minifier

`BrainSharp.exe -m <path> [options]`
- Minify a Brainfuck source file.
- Options:
    - `-o <path>` Specify output file path
