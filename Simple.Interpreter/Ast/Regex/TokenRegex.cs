using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Interpreter.Ast
{
    public static class TokenRegex
    {
        public const string ARITHMETIC_OPERATORS_PART = @"[\+\-\*\/]";
        public const string COMPARISON_OPERATORS_PART = @"([=!]=)|([><]=?)|((not\s)?in)|(and)|(or)";
        public const string VALUE_LITERAL_PART = @"""[^""]*""|'[^']*'|\d+\.?\d*";
        public const string VARIABLE_AND_MEMBER_PART = @"\w+(\.\w+)*";
        public const string MISC_PART = @"[\[\]\(\)\,]";
        private const string REGEX_OR = "|";
        public const string COMPILED = COMPARISON_OPERATORS_PART + REGEX_OR + VALUE_LITERAL_PART + REGEX_OR + VARIABLE_AND_MEMBER_PART + REGEX_OR + ARITHMETIC_OPERATORS_PART + REGEX_OR +  MISC_PART;
    }
}
