# Tiny Compiler

A data-driven compiler implementation leveraging rigorous parsing techniques and formal language theory to transform source code into structured abstract syntax trees. This project applies advanced compiler construction principles—from lexical analysis to syntax parsing—to extract actionable program semantics from complex code structures.

## Overview

**Tiny Compiler** is a complete compiler frontend written in C# that processes source code through a traditional two-stage pipeline:
1. **Lexical Analysis (Scanner)** - Tokenization of input source code
2. **Syntax Analysis (Parser)** - Derivation of parse trees from token streams

The compiler transforms textual programs into hierarchical parse trees that represent the complete syntactic structure of the input, enabling subsequent semantic analysis and code generation phases.

## Key Features

- **Complete Lexical Scanner** - Recognizes reserved words, identifiers, operators, and literals
- **Recursive Descent Parser** - Implements context-free grammar through predictive parsing
- **Parse Tree Construction** - Builds detailed abstract syntax trees for program analysis
- **Comprehensive Error Handling** - Collects and reports syntax errors with context information
- **GUI Integration** - Windows Forms interface for interactive code compilation and visualization
- **Type System Support** - Handles multiple data types (int, float, string)

## Architecture

### Core Components

```
TinyCompiler/
├── Scanner.cs          - Lexical analyzer (tokenization)
├── Parser.cs           - Syntax analyzer (parse tree generation)
├── Tiny_Compiler.cs    - Main orchestrator (pipeline coordination)
├── Errors.cs           - Error collection and reporting
├── Form1.cs            - GUI interface for user interaction
└── Program.cs          - Application entry point
```

### Grammar Definition

The compiler implements a formal context-free grammar with the following structure:

**Non-Terminals (40):**
Program, Function_Statement_List, Main_Function, Function_Statement, Function_Declaration, Function_Body, Data_Type, Parameter_List, Statement_List, Statement, Expression, Term, Equation, Condition_Statement, If_Statement, Repeat_Statement, Assignment_Statement, Declaration_Statement, Write_Statement, Read_Statement, Function_Call, and more...

**Terminals (37):**
- Reserved Words: `main`, `int`, `float`, `string`, `return`, `write`, `read`, `endl`, `if`, `then`, `elseif`, `else`, `end`, `repeat`, `until`
- Operators: `+`, `-`, `*`, `/`, `<`, `>`, `=`, `<>`, `&`, `|`
- Symbols: `(`, `)`, `{`, `}`, `;`, `:=`, `,`, `"`
- Other: identifiers, numbers, comments

**37 Production Rules** implementing:
- Program structure with main function and helper functions
- Type system (int, float, string)
- Control flow (if/elseif/else, repeat/until)
- I/O operations (read, write)
- Arithmetic and boolean expressions
- Function declarations and calls

## Compilation Pipeline

```
Source Code
    ↓
[Scanner] → Tokenization
    ↓
Token Stream
    ↓
[Parser] → Parse Tree Construction
    ↓
Abstract Syntax Tree (AST)
    ↓
[Error Reporting]
```

### Execution Flow

1. **Start_Compiling(sourceCode)** - Entry point that orchestrates the pipeline
2. **Scanner Phase** - Tokenizes character-by-character input into a token stream
3. **Parser Phase** - Applies production rules to derive parse trees through recursive descent
4. **Error Collection** - Gathers syntax errors encountered during parsing
5. **Tree Output** - Returns the complete parse tree for downstream processing

## Usage

### Programmatic API

```csharp
// Initialize and compile source code
string sourceCode = @"
    int main()
    {
        int x := 5;
        write x;
        return 0;
    }
";

Tiny_Compiler.Start_Compiling(sourceCode);

// Access results
Node parseTree = Tiny_Compiler.treeroot;
List<string> errors = Errors.Error_List;
```

### GUI Application

1. Launch the application through Form1
2. Enter source code in the editor
3. Execute the compilation process
4. View the generated parse tree and any errors reported

## Supported Language Features

### Data Types
- `int` - Integer values
- `float` - Floating-point values
- `string` - String literals

### Control Structures
- **If Statements**: `if condition then ... elseif ... else ... end`
- **Repeat Loops**: `repeat ... until condition`

### I/O Operations
- **Write**: `write expression;` or `write endl;`
- **Read**: `read identifier;`

### Function Operations
- Function declarations with parameters
- Function calls with arguments
- Return statements

### Operators
- **Arithmetic**: `+`, `-`, `*`, `/`
- **Comparison**: `<`, `>`, `=`, `<>`
- **Boolean**: `&` (AND), `|` (OR)
- **Assignment**: `:=`

## Technical Highlights

### Advanced Parsing Techniques
- **LL(1) Predictive Parsing** - Single token lookahead for decision-making
- **Left-Recursive Elimination** - Grammar transformed to right-recursive form
- **Error Recovery** - Graceful handling of syntax errors with context preservation

### Data Structure Optimization
- Token caching for efficient stream processing
- Tree node hierarchies for semantic representation
- Error list aggregation for batch reporting

### Software Engineering Patterns
- **Pipeline Architecture** - Modular, composable compilation stages
- **Separation of Concerns** - Distinct scanner, parser, and error handling
- **State Management** - Clean initialization and context tracking

## Implementation Status

- ✅ Complete lexical analyzer
- ✅ Full recursive descent parser
- ✅ Parse tree visualization
- ✅ Comprehensive error reporting
- ✅ GUI interface
- 🔄 Semantic analysis (future)
- 🔄 Code generation (future)

## Project Metadata

- **Language**: C#
- **Platform**: .NET / Windows Forms
- **Repository**: AAFouad/Tiny-Compiler
- **License**: MIT
- **Status**: Active Development

## Contributing

Contributions are welcome! Areas for enhancement include:
- Semantic analyzer implementation
- Intermediate code generation
- Code optimization passes
- Extended language features

## References

- Dragon Book: Compilers - Principles, Techniques, and Tools
- Context-Free Grammars and LL(1) Parsing Theory
- Recursive Descent Parser Implementation Patterns
