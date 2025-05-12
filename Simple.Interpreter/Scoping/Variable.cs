using System;
using System.Collections.Generic;
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
        public readonly Type VariableType;

        /// <summary>
        /// Gets or sets the value of the variable.
        /// Throws an ArgumentException if the value being set is not of the correct type.
        /// </summary>
        public object Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (VariableType != value.GetType())
                {
                    throw new ArgumentException($"Type mismatch: Expected {VariableType}, but got {value.GetType()}");
                }
                _value = value;
            }
        }

        private object _value;
        public readonly string FullTypeName;
        public readonly Dictionary<string, MemberInfo> Members;

        public Variable(object value, Dictionary<string, MemberInfo> members)
        {
            _value = value;
            VariableType = value?.GetType() ?? throw new ArgumentException("cannot be null", nameof(value));
            FullTypeName = VariableType?.FullName ?? "Object";
            Members = members;
        }

        public Variable(Type type, Dictionary<string, MemberInfo> members)
        {
            VariableType = type;
            FullTypeName = type.FullName ?? "Object";
            Members = members;
        }

        public Variable(Type type)
        {
            VariableType = type;
            FullTypeName = type.FullName ?? "Object";
            Members = type.GetMembers(BindingFlags.Instance | BindingFlags.Public).ToDictionary(x => x.Name);
        }

        public override string ToString()
        {
            return $"[{FullTypeName}]: {Value}";
        }
    }
}
