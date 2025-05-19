using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Interpreter.Scoping
{
    /// <summary>
    /// Represents a variable within a specific scope.
    /// It holds the variable's value, type information, and accessible members.
    /// </summary>
    public class Variable
    {
        public readonly Type? VariableType;

        /// <summary>
        /// Gets or sets the value of the variable.
        /// Throws an ArgumentException if the value being set is not of the correct type.
        /// </summary>
        public object? Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (value is not null && VariableType != value.GetType())
                {
                    throw new ArgumentException($"Type mismatch: Expected {VariableType}, but got {value.GetType()}");
                }
                _value = value;
            }
        }

        private object? _value;
        public readonly string FullTypeName;
        public readonly ObjectMemberMap MemberMap;

        public Variable(object value, ObjectMemberMap memberMap)
        {
            _value = value;
            if (value is null)
            {
                throw new ArgumentException("cannot be null", nameof(value));
            }
            VariableType = value.GetType();
            FullTypeName = VariableType?.FullName ?? "Object";
            MemberMap = memberMap;
        }

        public Variable(Type type, ObjectMemberMap memberMap)
        {
            VariableType = type;
            FullTypeName = type.FullName ?? "Object";
            MemberMap = memberMap;
        }

        public Variable(Type type) : this(type, new ObjectMemberMap(type))
        { }

        [ExcludeFromCodeCoverage]
        public override string ToString()
        {
            return $"[{FullTypeName}]: {Value}";
        }
    }
}
