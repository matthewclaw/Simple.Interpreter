# Simple.Interpreter

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![NuGet](https://img.shields.io/nuget/v/Simple.Interpreter)](https://www.nuget.org/packages/Simple.Interpreter)
[![GitHub Stars](https://img.shields.io/github/stars/matthewclaw/Simple.Interpreter)](https://github.com/matthewclaw/Simple.Interpreter/stargazers)
[![GitHub Forks](https://img.shields.io/github/forks/matthewclaw/Simple.Interpreter)](https://github.com/matthewclaw/Simple.Interpreter/network/members)
[![GitHub Issues](https://img.shields.io/github/issues/matthewclaw/Simple.Interpreter)](https://github.com/matthewclaw/Simple.Interpreter/issues)
[![GitHub Last Commit](https://img.shields.io/github/last-commit/matthewclaw/Simple.Interpreter)](https://github.com/matthewclaw/Simple.Interpreter/commits/main)
[![Language: C#](https://img.shields.io/badge/Language-C%23-%23239120.svg)](https://docs.microsoft.com/en-us/dotnet/csharp/)

**Simple.Interpreter** is a lightweight **C#** library providing a straightforward interpreter for a simple, custom expression language. This NuGet package enables developers to empower their customers or clients to define dynamic expressions for various purposes, such as business rules, conditional logic, and data filtering, without requiring code recompilation. Notably, as demonstrated in the unit tests, the interpreter can also handle **complex objects as variables** within the expression context.

## Features

* **Simple Expression Syntax:** The interpreted language offers a minimal and intuitive syntax, easy for non-technical users to grasp.
* **Validation:** The interpreter validates the expression syntax and ensures that it is well-formed and can be evaluated.
* **Arithmetic Operations:** Includes standard arithmetic operations (+, -, \*, /).
* **Comparison Operators:** Supports a full suite of comparison operators (`==`, `!=`, `>`, `<`, `>=`, `<=`). Version `8.2.0` added support for more natural language comparison operators including:
    - `equal to`
    - `is equal to`
    - `not equal to`
    - `is not equal to`
    - `greater than`
    - `is greater than`
    - `less than`
    - `is less than`
    - `greater than or equal to`
    - `is greater than or equal to`
    - `less than or equal to`
    - `is less than or equal to`
* **Logical Operators:** Implements logical AND (`and`) and OR (`or`) operations for building complex conditions.
* **Array Operators:** `in` (array contains), `not in` (array does not contain) to check against arrays.
* **Variable Resolution:** Allows expressions to reference external variables provided at runtime, including **complex objects and their properties**.
* **Extensible Function Support:** Designed to allow developers to easily register custom functions that can be called within the expressions.
* **Lightweight and Embeddable:** The library has minimal dependencies and can be easily integrated into any .NET application.
* **Complex Object Support:** As showcased in the unit tests, the interpreter can access properties, fields and methods of complex objects passed within the evaluation context.

## Getting Started

### Installation via NuGet

You can install the **Simple.Interpreter** NuGet package using the NuGet Package Manager in Visual Studio or the .NET CLI:

```bash
dotnet add package Simple.Interpreter
```

### Basic Usage with Complex Objects
#### Example 1: Accessing Properties of Complex Objects

```csharp
using Simple.Interpreter;
using System;
using System.Collections.Generic;

public class User
{
    public string Name { get; set; }
    public int Age { get; set; }
    public string City { get; set; }
    public string SayHi(string to)
   {
       return $"Hi there {to}!, I'm {Name}";
   }

}

// Create an instance of the interpreter
var interpreter = new ExpressionInterpreter();

// Define the expression accessing properties of a complex object
string expressionString = "user.Age > 18 and user.City == 'Johannesburg'";

// Create a complex object to use as a variable
var user = new User
{
    Name = "Alice",
    Age = 25,
    City = "Pretoria"
};

// Provide the context (including the complex object) for the expression
var context = new Dictionary<string, object>
{
    {"user", user}
};
// Parse the expression
var expression = interpreter.GetExpression(expressionString);
// Set its scope
expression.SetScope(context);
// Evaluate the expression
object result = expression.Evaluate();

if (result is bool isAdultInJohannesburg && isAdultInJohannesburg)
{
    Console.WriteLine($"{user.Name} meets the criteria.");
}
else
{
    Console.WriteLine($"{user.Name} does not meet the criteria.");
}
```
#### Example 2: Using Conditional Logic and method call

```csharp
using Simple.Interpreter;
using System;
using System.Collections.Generic;

public class User
{
    public string Name { get; set; }
    public int Age { get; set; }
    public string City { get; set; }
    public string SayHi(string to)
   {
       return $"Hi there {to}!, I'm {Name}";
   }

}

// Create an instance of the interpreter
var interpreter = new ExpressionInterpreter();

// Define the expression accessing properties of a complex object
string expressionString = "alice.SayHi('Frank') if(alice.Age > 18+51) else bob.SayHi('Frank')";

var alice = new User
{
    Name = "Alice",
    Age = 25,
    City = "Pretoria"
};
var bob = new User
{
    Name = "Bob",
    Age = 19,
    City = "Cape Town"
};

// Provide the context (including the complex object) for the expression
var context = new Dictionary<string, object>
{
    {"alice", alice},
    {"bob", bob}
};
// Parse the expression
var expression = interpreter.GetExpression(expressionString);
// Set its scope
expression.SetScope(context);
// Evaluate the expression
object result = expression.Evaluate();
Console.WriteLine($"{result}"); //Outputs: Hi there Frank!, I'm Bob
```
*For more see the `Simple.Interpreter.Demo` project*

### Validation
```csharp
 // Create an instance of the interpreter
 var interpreter = new ExpressionInterpreter();

 // Define the expression
 string expressionString = "user.Age > 18 and user.City == 'Johannesburg'";

 // Define valid variable types allowed in expressions
 var validVariableTypes = new Dictionary<string, Type>
 {
    { "user", typeof(User) },
 };

 // validate the expression
 var isValid = interpreter.Validate(expressionString, validVariableTypes, out var errors); //Returns true

 if (isValid)
 {
     Console.WriteLine($"The expression is valid!");
 }
 else
 {
     Console.WriteLine($"There were issues with the expression: ");
     foreach (var err in errors)
     {
         Console.WriteLine(err.Message);
     }
 }
```
*For more see the `Simple.Interpreter.Demo` project*
### Defining Custom Functions

Custom functions can be defined and used by `Expressions` created with the `ExpressionInterpreter`.

*Custom functions can accept up to 4 arguments.*

```csharp
using Simple.Interpreter;
using System;
using System.Collections.Generic;

private static bool IsUserOlderThan(User user, int age)
{
    bool result = user?.Age > age;
    return result;
}

ExpressionInterpreter interpreter = new ExpressionInterpreter();

//Register custom Function
interpreter.RegisterFunction("isUserOlderThan", IsUserOlderThan);

var frank = new User
{
    Name = "Frank",
    Age = 40,
    City = "Cape Town"
};
var ageToTest = 30;

var scope = new Dictionary<string, object>()
{
    {"user", frank },
    {"age", ageToTest }
};
string expressionString = "isUserOlderThan(user, age)";
Expression expression = interpreter.GetExpression(expressionString);
// Set its scope
expression.SetScope(scope);

var result = expression.Evaluate(); //Returns true
if(result is bool isOldEnough && isOldEnough)
{
    Console.WriteLine($"{frank} is Older than {ageToTest}");
}
else
{
    Console.WriteLine($"{frank} is not older than {ageToTest}");
}
```
*For more see the `Simple.Interpreter.Demo` project*
### Logging
The `Expression` evaluation process offers detailed step-by-step logging to aid developers in understanding and debugging complex expressions. This logging is performed at the `DEBUG` level and provides insight into variable resolution, function calls, operator applications, and intermediate results. To enable this verbose logging, ensure your application's logging configuration is set to `DEBUG` for the relevant logger. The `ExpressionInterpreter` leverages `ILoggerFactory` to create its internal logger; you can either provide an `ILoggerFactory` instance upon `ExpressionInterpreter` initialization, or if using dependency injection (DI), it will be automatically injected. For fine-grained control, you can also explicitly toggle this debugging output for a specific Expression instance using the `.WithDebugging(false|true)` method.
#### Example
```csharp
ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Debug);
});
// Create an instance of the interpreter
var interpreter = new ExpressionInterpreter(loggerFactory);

// Define the expression
string expressionString = "user.Age > 18 and user.City == 'Johannesburg'";

var user = new User
{
    Name = "Alice",
    Age = 25,
    City = "Pretoria" 
};
var expression = interpreter.GetExpression(expressionString);
expression.SetScopedVariable("user", alice);
expression.Evaluate();
```
**Logging output:**
```
dbug: Simple.Interpreter.Ast.Expression[0]
      Evaluating Expression: user.Age > 18 and user.City == "Johannesburg"...
dbug: Simple.Interpreter.Ast.Expression[0]
      Evaluating Binary Node: user.Age > 18 and user.City == "Johannesburg"...
dbug: Simple.Interpreter.Ast.Expression[0]
      Evaluating Binary Node: user.Age > 18...
dbug: Simple.Interpreter.Ast.Expression[0]
      Evaluating Member Node: user.Age...
dbug: Simple.Interpreter.Ast.Expression[0]
      Member Evaluated: 25
dbug: Simple.Interpreter.Ast.Expression[0]
      Evaluating: 25 > 18...
dbug: Simple.Interpreter.Ast.Expression[0]
      Evaluated: True
dbug: Simple.Interpreter.Ast.Expression[0]
      Evaluating Binary Node: user.City == "Johannesburg"...
dbug: Simple.Interpreter.Ast.Expression[0]
      Evaluating Member Node: user.City...
dbug: Simple.Interpreter.Ast.Expression[0]
      Member Evaluated: Pretoria
dbug: Simple.Interpreter.Ast.Expression[0]
      Evaluating: Pretoria == Johannesburg...
dbug: Simple.Interpreter.Ast.Expression[0]
      Evaluated: False
dbug: Simple.Interpreter.Ast.Expression[0]
      Evaluating: True and False...
dbug: Simple.Interpreter.Ast.Expression[0]
      Evaluated: False
dbug: Simple.Interpreter.Ast.Expression[0]
      Expression Evaluated: False
```
## Language Syntax (Brief Overview for Expressions)

* **Variables:** Identifiers (e.g., `age`, `productName`, `user`). These will be resolved from the provided context. You can access properties, fields and methods of complex object variables using dot notation (e.g., `user.Age`, `order.Name`, `user.DoSomething()`). The expression can only access variables 1 level deep (e.g. `user.FullAddress.PostalCode` is 2 level deep and will fail validation)
* **Literals:**
    * **Numbers:** Integers and decimals (e.g., `10`, `3.14`).
    * **Strings:** Enclosed in single quotes (e.g., `'Hello'`).
    * **Booleans:** `true` or `false`.
    * **Arrays:** The `ExpressionInterpreter` can parse array literals of `string`, `double` and `int` (e.g. `[1, 2, 3, 4, 5]` or `['a', 'b', 'c']`).
* **Arithmetic Operators:** `+` (addition), `-` (subtraction), `*` (multiplication), `/` (division).
* **Comparison Operators:** `==` (equals), `!=` (not equals), `>` (greater than), `<` (less than), `>=` (greater than or equal to), `<=` (less than or equal to).
* **Logical Operators:** `and` (logical AND), `or` (logical OR).
* **Array Operators:** `in` (array contains), `not in` (array does not contain).
* **Parentheses:** Used to group expressions and control operator precedence (e.g., `(a + b) * c`).
* **Function Calls:** Registered custom functions can be called using their name followed by arguments in parentheses (e.g., `startsWith(name, 'Prefix')`, `isOlderThan(currentUser, 30)`). By default, the `ExpressionInterpreter` includes a set of built-in functions including `startsWith`, `endsWith`, `min` and `max`.
* **Ternary Expressions:** Python-style ternary support (e.g., `'Over 21' if (user.Age > 21) else 'Under 21'`).

## Contributing

Contributions to this project are welcome\! If you have ideas for improvements, bug fixes, or new features, especially those enhancing the expression language or the interpreter's ability to handle complex data structures, please feel free to:

1.  Fork the repository.
2.  Create a new branch for your changes (`git checkout -b feature/your-feature`).
3.  Commit your changes (`git commit -am 'Add some feature'`).
4.  Push to the branch (`git push origin feature/your-feature`).
5.  Open a pull request.

## License

This project is licensed under the [MIT License](https://opensource.org/licenses/MIT). See the `LICENSE` file for more details.

## Acknowledgements

  * This project aims to provide a simple and effective solution for dynamic expression evaluation in .NET applications, with the added capability of handling complex object variables.
  * Special thanks to the .NET community for its valuable resources and support.

## Contact

[Matthew Law](https://www.linkedin.com/in/matthew-l-87448694/)

## Donations

If you find this project useful, please consider making a donation to support its development. Your support will help me continue to maintain and improve this project.

[![PayPal](https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif)](https://www.paypal.com/donate/?hosted_button_id=ZSKSBJLT4TDG4)
