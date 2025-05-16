using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Interpreter.Scoping
{
    /// <summary>
    /// Represents a scope in the interpreter, holding variables and their values.
    /// Scopes allow for variable lookup, creation, and modification within a specific context.
    /// </summary>
    public class Scope
    {
        /// <summary>
        /// Map of variables that have been registered in this scope
        /// </summary>
        private readonly Dictionary<string, Variable> _internalVariables;
        /// <summary>
        /// Map of instance members for registered variable Types
        /// </summary>
        private readonly Dictionary<string, Dictionary<string, MemberInfo>> _internalMembersMap;
        /// <summary>
        /// Map of instance members for registered variable Types
        /// </summary>
        private readonly Dictionary<string, Dictionary<string, List<MethodInfo>>> _internalOverloadedMethodsMap;
        private readonly Scope _parent;

        public Scope()
        {
            _internalVariables = new Dictionary<string, Variable>();
            _internalMembersMap = new Dictionary<string, Dictionary<string, MemberInfo>>();
            _internalOverloadedMethodsMap = new Dictionary<string, Dictionary<string, List<MethodInfo>>>();
        }

        public Scope(Scope parent) : this()
        {
            _parent = parent;
        }

        public Scope(Dictionary<string, object> variables) : this()
        {
            SetVariables(variables);
        }

        /// <summary>
        /// Checks if the scope or any of its parent scopes contain a variable with the specified name.
        /// </summary>
        /// <param name="name">The name of the variable to check for.</param>
        /// <returns>True if the scope or a parent scope contains the variable; otherwise, false.</returns>
        public bool ContainsVariable(string name)
        {
            if (_internalVariables.ContainsKey(name))
            {
                return true;
            }
            return _parent != null && _parent.ContainsVariable(name);
        }

        /// <summary>
        /// Checks if the scope or any of its parent scopes contain reflected members for the specified type.
        /// </summary>
        /// <param name="type">The full name of the type to check for.</param>
        /// <returns>True if the scope or a parent scope contains members for the type; otherwise, false.</returns>
        public bool ContainsMembersFor(string type)
        {
            if (_internalMembersMap.ContainsKey(type))
            {
                return true;
            }
            return _parent != null && _parent.ContainsMembersFor(type);
        }

        public bool TryGetVariable(string name, out Variable value)
        {
            if (_internalVariables.TryGetValue(name, out value))
            {
                return true;
            }
            return _parent != null && _parent.TryGetVariable(name, out value);
        }

        /// <summary>
        /// Tries to retrieve reflected members for the specified type from the scope or its parent scopes.
        /// </summary>
        /// <param name="type">The full name of the type whose members are to be retrieved.</param>
        /// <param name="members">When this method returns, contains the dictionary of member information associated with the specified type, if found; otherwise, null. This parameter is passed uninitialized.</param>
        /// <returns>True if members for the specified type are found in the scope or a parent scope; otherwise, false.</returns>
        public bool TryGetMembers(string type, out Dictionary<string, MemberInfo> members)
        {
            if (_internalMembersMap.TryGetValue(type, out members))
            {
                return true;
            }
            return _parent != null && _parent.TryGetMembers(type, out members);
        }

        /// <summary>
        /// Tries to retrieve reflected members for the specified type from the scope or its parent scopes.
        /// </summary>
        /// <param name="type">The type whose members are to be retrieved.</param>
        /// <param name="members">When this method returns, contains the dictionary of member information associated with the specified type, if found; otherwise, null. This parameter is passed uninitialized.</param>
        /// <returns>True if members for the specified type are found in the scope or a parent scope; otherwise, false.</returns>
        public bool TryGetMembers(Type type, out Dictionary<string, MemberInfo> members)
        {
            if (_internalMembersMap.TryGetValue(type.FullName, out members))
            {
                return true;
            }
            return _parent != null && _parent.TryGetMembers(type.FullName, out members);
        }

        public void SetVariables(Dictionary<string, object> scope)
        {
            foreach (var item in scope)
            {
                SetVariable(item.Key, item.Value);
            }
        }

        /// <summary>
        /// Registers the public members of a type within the scope, allowing for member access during interpretation.
        /// If the type's members are already registered in this scope or a parent scope, this method does nothing.
        /// </summary>
        /// <param name="type">The type whose members are to be registered.</param>
        public void RegisterTypeMembers(Type type)
        {
            var fullName = type.FullName;
            var memberMap = GetPublicMembersIfObject(type);
            RegisterTypeMembers(fullName, memberMap);
        }

        /// <summary>
        /// Registers a list of member information for a given type full name within the scope.
        /// If the type's members are already registered in this scope or a parent scope, this method does nothing.
        /// </summary>
        /// <param name="typeFullName">The full name of the type whose members are to be registered.</param>
        /// <param name="members">A list of MemberInfo representing the members of the type to register.</param>
        public void RegisterTypeMembers(string typeFullName, List<MemberInfo> members)
        {
            var memberMap = members.ToDictionary(x => x.Name);
            RegisterTypeMembers(typeFullName, memberMap);
        }

        /// <summary>
        /// Registers a dictionary of member information for a given type full name within the scope.
        /// If the type's members are already registered in this scope or a parent scope, this method does nothing.
        /// </summary>
        /// <param name="typeFullName">The full name of the type whose members are to be registered.</param>
        /// <param name="members">A dictionary of MemberInfo representing the members of the type to register, keyed by member name.</param>
        public void RegisterTypeMembers(string typeFullName, Dictionary<string, MemberInfo> members)
        {
            if (ContainsMembersFor(typeFullName))
            {
                return;
            }
            if (_parent != null)
            {
                _parent.RegisterTypeMembers(typeFullName, members);
                return;
            }
            _internalMembersMap.Add(typeFullName, members);
        }

        /// <summary>
        /// Registers a variable's type without assigning a value. This is useful for declaring variables before assigning values, or for expression validation
        /// </summary>
        /// <param name="name">The name of the variable to register.</param>
        /// <param name="type">The type of the variable.</param>
        public void RegisterVariableType(string name, Type type)
        {
            SetVariable(name, type, null);
        }

        public void SetVariable(string name, object value)
        {
            if (value?.Equals(null) ?? true)
            {
                throw new ArgumentNullException(nameof(value));
            }
            var scopedValueType = value.GetType();
            SetVariable(name, scopedValueType, value);
        }

        private void SetVariable(string name, Type scopedValueType, object value)
        {
            if (_internalVariables.ContainsKey(name))
            {
                _internalVariables[name].Value = value;
                return;
            }
            Variable newVariable;
            if (!TryGetMembers(scopedValueType, out var members))
            {
                members = GetPublicMembersIfObject(scopedValueType);
                RegisterTypeMembers(scopedValueType.FullName, members);
            }
            if (value != null)
            {
                newVariable = new Variable(value, members);
            }
            else
            {
                newVariable = new Variable(scopedValueType, members);
            }
            _internalVariables.Add(name, newVariable);
        }

        /// <summary>
        /// Retrieves a dictionary of public instance members (excluding constructors) for a given type.
        /// If the type is null, primitive, or a string, an empty dictionary is returned.
        /// </summary>
        /// <param name="type">The type whose public instance members are to be retrieved.</param>
        /// <returns>A dictionary where the key is the member name and the value is the MemberInfo. Returns an empty dictionary if the type is null, primitive, or a string.</returns>
        private static Dictionary<string, MemberInfo> GetPublicMembersIfObject(Type type)
        {
            if (type == null || type.IsPrimitive || type == typeof(string))
            {
                return new Dictionary<string, MemberInfo>();
            }
            var result = new Dictionary<string, MemberInfo>();
            var allInstanceMembers = type?.GetMembers(BindingFlags.Public | BindingFlags.Instance) ?? new MemberInfo[0];
            foreach (var member in allInstanceMembers)
            {
                if (!result.ContainsKey(member.Name))
                {
                    result.Add(member.Name, member);
                }
                else
                {
                    var props = (member as MethodInfo).GetParameters();
                }
            }
            return result;
        }
    }
}
