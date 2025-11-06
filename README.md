# dimpoc

POC project demonstrating annotation-driven sanitization and validation of string fields in C#.

## Quick Start

Targets: net9.0 (auto-detected from environment where this POC was created).

How to build and run:

```powershell
# from repository root
dotnet build
dotnet run --project .\dimpoc.csproj
```

How to run tests:

```powershell
dotnet test
```

## Current Implementation

What is included:
- Attributes for `TIN`, `PAN`, `Tel`, and `USNUM`.
- `AnnotationProcessor.Sanitize(object)` which sanitizes annotated string fields and returns `FieldResults`.
- `AnnotationProcessor.Validate(object)` which validates annotated fields (and may normalize phone numbers by adding leading `1` for US numbers).
- A `Program` with sample `DemoData` containing valid and invalid examples.

Notes:
- `Sanitize` will convert null strings to empty string and mark them as empty.
- `Sanitize` sets `FieldResultItem.Valid` to true when the field did not change, false otherwise.
- `Validate` performs lightweight checks (Luhn for PAN, 9 digits for TIN, US phone rules, numeric parse for USNUM).

Next steps (optional): add more international phone handling, add more robust TIN rules, and expand number formatting support.

## Original Project Specification

### Background and Problem Statement

Application Security requires that all data field values which come into an application be sanitized to eliminate attack data. Input filtering blocks SQL injection, XSS, and a number of other attacks.

Many application frameworks have validation functions but these apply only to http request parameters. They don't typically provide access to the input value so their logic can't be used to sanitize data for later processing. These methods cannot be applied to field values read from a database, from input files, or from other sources.

Regardless of source, parameters end up as field values of a C# object. This proof of concept demonstrates attaching C# annotations to C# string fields so that a sanitization method can be passed an object of a class whose fields have these annotations. Different sanitization logic would be applied depending on the meaning of the attribute as specified in the annotation.

### Core Requirements

1. **Data Processing Model**
   - String fields are easy to read from http requests, database rows, and other sources
   - After validation, the application can convert the fields in any manner desired
   - Processing methods take an object which may contain annotated string fields

2. **Required Methods**
   - **Sanitize**: removes all attack data
     - Phone number (Tel): remove all non-numeric characters
     - Credit card number (PAN): remove all non-numeric characters
     - Social security number (TIN): remove all non-numeric data except for - and a leading P
     - Number [USNUM]: remove all characters except valid numerical representation (+, -, ., E/e)

   - **Validate**: verify fields are potentially useful (called after sanitization)
     - Phone: must start with country code, proper number of digits for country
     - PAN: must have valid Luhn algorithm check digit
     - TIN: basic format validation
     - USNUM: valid numerical notation (including scientific notation)

3. **Field Processing Results**
   - Both methods return a FieldResults object containing:
     - List of FieldResultItem objects (one per annotated field)
     - Boolean indicating if all fields are OK
   - Each FieldResultItem includes:
     - Field name
     - Empty flag (true if value is empty/null)
     - Valid flag (depends on context)

### Validation Rules

- Sanitize sets empty=true for null strings, converting them to empty string
- Empty string after removing attack characters also sets empty=true
- Validate only checks if field COULD be legitimate
  - Does not verify TIN exists
  - Does not verify phone number is in service
  - Does not verify PAN has active account

### Demonstration Requirements

The POC should include a C# main program that:
1. Defines an object with several valid and invalid fields of each type
2. Calls sanitize(object) to remove attack data
3. Calls validate(object) to verify potentially valid values
4. Demonstrates the FieldResults reporting mechanism
