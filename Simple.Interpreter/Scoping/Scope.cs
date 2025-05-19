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
    /// Represents a scope in the interpreter, holding variables and their values.
    /// Scopes allow for variable lookup, creation, and modification within a specific context.
    /// </summary>
    public class Scope
    {
        #region Private Fields

        /// <summary>
        /// Map of instance members for registered variable Types
        /// </summary>
        private readonly Dictionary<string, ObjectMemberMap> _internalMembersMap;

        /// <summary>
        /// Map of variables that have been registered in this scope
        /// </summary>
        private readonly Dictionary<string, Variable> _internalVariables;

        /// <summary>
        /// The parent scope of this scope.  Used for variable lookup in enclosing scopes.  Null if this is the global scope.
        /// </summary>
        private readonly Scope? _parent;

        #endregion Private Fields

        #region Public Constructors

        public Scope()
        {
            _internalVariables = new Dictionary<string, Variable>();
            _internalMembersMap = new Dictionary<string, ObjectMemberMap>();
        }

        public Scope(Scope parent) : this()
        {
            _parent = parent;
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Registers an ObjectMemberMap for a given type full name within the scope.
        /// If the type's members are already registered in this scope or a parent scope, this method registers in the parent.
        /// </summary>
        /// <param name="typeFullName">The full name of the type whose members are to be registered.</param>
        /// <param name="memberMap">A ObjectMemberMap representing the members of the type to register.</param>
        public void RegisterTypeMembers(string typeFullName, ObjectMemberMap memberMap)
        {
            if (_parent != null)
            {
                _parent.RegisterTypeMembers(typeFullName, memberMap);
                return;
            }
            _internalMembersMap.Add(typeFullName, memberMap);
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

        /// <summary>
        /// Registers a variable types without assigning values. This is useful for declaring variables before assigning values, or for expression validation
        /// </summary>
        /// <param name="variableTypes"></param>
        public void RegisterVariableTypes(Dictionary<string, Type> variableTypes)
        {
            foreach (var item in variableTypes)
            {
                RegisterVariableType(item.Key, item.Value);
            }
        }

        public void SetVariable(string name, object value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            var scopedValueType = value.GetType();
            SetVariable(name, scopedValueType, value);
        }

        /// <summary>
        /// Sets multiple variables in the scope from a dictionary of name-value pairs.
        /// </summary>
        /// <param name="scope">A dictionary where the key is the variable name and the value is the variable's value.</param>
        public void SetVariables(Dictionary<string, object> scope)
        {
            foreach (var item in scope)
            {
                SetVariable(item.Key, item.Value);
            }
        }

        [ExcludeFromCodeCoverage]
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (_parent is not null)
            {
                stringBuilder.AppendLine("Parent variable:");
                stringBuilder.AppendLine(_parent.ToString());
            }
            stringBuilder.AppendLine("Variables:");
            foreach (var variable in _internalVariables)
            {
                stringBuilder.AppendLine($"- {variable.Key} => {variable.Value}");
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Tries to retrieve reflected members for the specified type from the scope or its parent scopes.
        /// </summary>
        /// <param name="type">The type whose members are to be retrieved.</param>
        /// <param name="members">When this method returns, contains the dictionary of member information associated with the specified type, if found; otherwise, null. This parameter is passed uninitialized.</param>
        /// <returns>True if members for the specified type are found in the scope or a parent scope; otherwise, false.</returns>
        public bool TryGetMemberMap(Type type, out ObjectMemberMap members)
        {
            return TryGetMemberMap(type.FullName!, out members);
        }

        /// <summary>
        /// Tries to retrieve reflected members for the specified type from the scope or its parent scopes.
        /// </summary>
        /// <param name="type">The full name of the type whose members are to be retrieved.</param>
        /// <param name="members">When this method returns, contains the dictionary of member information associated with the specified type, if found; otherwise, null. This parameter is passed uninitialized.</param>
        /// <returns>True if members for the specified type are found in the scope or a parent scope; otherwise, false.</returns>
        public bool TryGetMemberMap(string type, out ObjectMemberMap members)
        {
            if (_internalMembersMap.TryGetValue(type, out members))
            {
                return true;
            }
            return _parent != null && _parent.TryGetMemberMap(type, out members);
        }

        /// <summary>
        /// Tries to retrieve a variable from the scope or its parent scopes.
        /// </summary>
        /// <param name="name">The name of the variable to retrieve.</param>
        /// <param name="value">When this method returns, contains the variable associated with the specified name, if found; otherwise, null. This parameter is passed uninitialized.</param>
        /// <returns>True if the variable is found in the scope or a parent scope; otherwise, false.</returns>
        public bool TryGetVariable(string name, out Variable value)
        {
            if (_internalVariables.TryGetValue(name, out value))
            {
                return true;
            }
            return _parent != null && _parent.TryGetVariable(name, out value);
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Sets the value of a variable in the scope. If the variable already exists, its value is updated.
        /// If the variable does not exist, a new variable is created and added to the scope.
        /// If the type's members are not already registered, it also registers the type members.
        /// </summary>
        /// <param name="name">The name of the variable to set.</param>
        /// <param name="scopedValueType">The type of the variable's value.</param>
        /// <param name="value">The value to assign to the variable.</param>
        private void SetVariable(string name, Type scopedValueType, object? value)
        {
            if (_internalVariables.ContainsKey(name))
            {
                _internalVariables[name].Value = value;
                return;
            }
            Variable newVariable;
            if (!TryGetMemberMap(scopedValueType, out var memberMap))
            {
                memberMap = new ObjectMemberMap(scopedValueType);
                RegisterTypeMembers(scopedValueType.FullName, memberMap);
            }
            if (value != null)
            {
                newVariable = new Variable(value, memberMap);
            }
            else
            {
                newVariable = new Variable(scopedValueType, memberMap);
            }
            _internalVariables.Add(name, newVariable);
        }

        #endregion Private Methods
    }
}