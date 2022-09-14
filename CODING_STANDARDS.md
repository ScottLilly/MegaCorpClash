When making a pull request, please follow these coding standards:  
**NOTE:** This list will changes as I notice things I forgot to include.

### General
- Use clear names, with minimal abbreviations
- Acronyms and initialisms should only capitalize first letter (e.g., use "DbRepository" instead of "DBRepository")
- Only have one statement per line
- Use parentheses to make expressions clearer, even if code hints say they aren't needed

### Classes
- PascalCase class names
- Use file-scoped namespaces
- Make classes internal, is not used outside the assembly
- Mark non-base classes as "sealed"
- Each class is defined in a separate .cs file
  - **EXCEPTION:** Classes used for deserialization

### Interfaces
- Start with capital "I"

### Members
- PasalCase fields, properties, events, methods, and local functions

### Functions
- camelCase parameter names
- Declare variables close to where they are used
- Surround all code blocks with curly braces on separate lines. This includes single-line "if" code blocks
- Convert simple functions to expression-bodied functions

### Variables
- Only declare one variable per line
- Naming style:
  - Class-level: _camelCasedWithUnderscorePrefix
  - Static class-level: s_camelCasedWithUnderscore
  - Static class-level thread: t_timeSpan
  - Inside functions: camelCased
- Only use "var" when datatype is obvious
  - For example, `var companies = new List<Company>();`
- Prefer `new()` when initially populating variables with values

### Constants
- Naming style: UPPER_CASE_WITH_UNDERSCORES

### Programming
- Use `using` syntax for IDisposable objects
- Write LINQ statements in method chaining style `companies.Where(c => c.IsActive)` instead of reverse SQL style
