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
* **Arithmetic Operations:** Includes standard arithmetic operations (+, -, \*, /).
* **Comparison Operators:** Supports a full suite of comparison operators (==, !=, >, <, >=, <=).
* **Logical Operators:** Implements logical AND (`and`), OR (`or`), and NOT (`not`) operations for building complex conditions.
* **Variable Resolution:** Allows expressions to reference external variables provided at runtime, including **complex objects and their properties**.
* **Extensible Function Support:** Designed to allow developers to easily register custom functions that can be called within the expressions.
* **Lightweight and Embeddable:** The library has minimal dependencies and can be easily integrated into any .NET application.
* **Complex Object Support:** As showcased in the unit tests, the interpreter can access properties of complex objects passed within the evaluation context.

## Getting Started

### Installation via NuGet

You can install the **Simple.Interpreter** NuGet package using the NuGet Package Manager in Visual Studio or the .NET CLI:

```bash
dotnet add package Simple.Interpreter
```

### Basic Usage with Complex Objects

```csharp
using Simple.Interpreter;
using System;
using System.Collections.Generic;

public class User
{
    public string Name { get; set; }
    public int Age { get; set; }
    public string City { get; set; }
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
*For more see the `Simple.Interpreter.Demo` project*
### Defining Custom Functions (Including with Complex Objects)

Custom functions can be defined and used by `Expressions` created with the `ExpressionInterpreter`.

*PS: Unfortunately, due to limitations, custom functions must accept either an `object` or an `object[]` and return an `object`.*

```csharp
using Simple.Interpreter;
using System;
using System.Collections.Generic;

private static object IsUserOlderThan(object[] args)
{
    if (args.Length != 2)
    {
        throw new ArgumentException($"{nameof(IsUserOlderThan)} expects 2 arguments");
    }
    User? user = args[0] as User;
    int? age = args[1] as int?;
    bool result = user?.Age > age;
    return result;
}

ExpressionInterpreter interpreter = new ExpressionInterpreter();
//Register custom Function
// PS: Due to limitations, custom functions must accept either an `object` or an `object[]` and return an `object`. Sorry :/
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

Expression expression = interpreter.GetExpression(EXPRESSION);
// Set its scope
expression.SetScope(scope);

var result = expression.Evaluate();
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
## Language Syntax (Brief Overview for Expressions)

* **Variables:** Identifiers (e.g., `age`, `productName`, `user`). These will be resolved from the provided context. You can access properties of complex object variables using dot notation (e.g., `user.Age`, `order.Name`). The expression can only access variables 1 level deep (e.g. `user.FullAddress.PostalCode` is 2 level deep and will fail validation)
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

[Your Name/Username] - [Your Email Address (Optional)] - [Your Website/Social Media (Optional)]