When making a pull request, please follow these coding standards:  
**NOTE:** This list will changes as I notice things I forgot to include.

### General
- Use clear names, with minimal abbreviations
- Acronyms and initialisms should only capitalize first letter (e.g., use "XmlDocument" instead of "XMLDocument")

### Classes
- PascalCase class names
- Mark non-base classes as "sealed"
- Each class is defined in a separate .cs file
  - **EXCEPTION:** Classes used for deserialization

### Functions
- Declare variables close to where they are used
- Surround all code blocks with curly braces on separate lines. This includes single-line "if" code blocks

### Variables
- Class-level: _camelCasedWithUnderscorePrefix
- Static class-level: s_camelCasedWithUnderscore
- Inside functions: camelCased

### Constants
- Naming style: UPPER_CASE_WITH_UNDERSCORES
