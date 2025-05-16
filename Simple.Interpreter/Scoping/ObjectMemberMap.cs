using Simple.Interpreter.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Simple.Interpreter.Scoping
{
    public class ObjectMemberMap
    {
        #region Public Fields

        public readonly Dictionary<string, FieldInfo> Fields;
        public readonly Dictionary<string, List<MethodInfo>> Methods;
        public readonly string ObjectTypeName;
        public readonly Dictionary<string, PropertyInfo> Properties;

        #endregion Public Fields

        #region Public Constructors

        public ObjectMemberMap(string objectTypeName, Dictionary<string, PropertyInfo> properties, Dictionary<string, FieldInfo> fields, Dictionary<string, List<MethodInfo>> methods)
        {
            ObjectTypeName = objectTypeName;
            Properties = properties;
            Fields = fields;
            Methods = methods;
        }

        public ObjectMemberMap(string objectTypeName, List<MemberInfo> members)
        {
            ObjectTypeName = objectTypeName;
            Properties = new Dictionary<string, PropertyInfo>();
            Fields = new Dictionary<string, FieldInfo>();
            Methods = new Dictionary<string, List<MethodInfo>>();
            MapMembers(members);
        }

        public ObjectMemberMap(Type type)
        {
            ObjectTypeName = type.Name;
            Properties = new Dictionary<string, PropertyInfo>();
            Fields = new Dictionary<string, FieldInfo>();
            Methods = new Dictionary<string, List<MethodInfo>>();
            MapMembers(type);
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Attempts to retrieve the value of a field from the given instance.
        /// </summary>
        /// <param name="instance">The object instance from which to retrieve the field value.</param>
        /// <param name="field">The name of the field to retrieve.</param>
        /// <param name="value">When this method returns, contains the value of the field if it exists; otherwise, null.
        /// This parameter is passed uninitialized.</param>
        /// <returns>true if the field exists and the value was successfully retrieved; otherwise, false.</returns>
        public bool TryGetFieldValue(object instance, string field, out object? value)
        {
            if (Fields.TryGetValue(field, out var fieldInfo))
            {
                value = fieldInfo.GetValue(instance);
                return true;
            }

            value = null;
            return false;
        }

        /// <summary>
        /// Tries to retrieve a member (field or property) by its name.
        /// </summary>
        /// <param name="fieldOrProperty">The name of the field or property to retrieve.</param>
        /// <param name="memberInfo">When this method returns, contains the MemberInfo if found; otherwise, null.</param>
        /// <returns>True if the member was found; otherwise, false.</returns>
        public bool TryGetMember(string fieldOrProperty, out MemberInfo? memberInfo)
        {
            memberInfo = null;
            if (Fields.TryGetValue(fieldOrProperty, out var fieldInfo))
            {
                memberInfo = fieldInfo;
                return true;
            }
            if (Properties.TryGetValue(fieldOrProperty, out var propertyInfo))
            {
                memberInfo = propertyInfo;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Attempts to retrieve the value of a member (field or property) from the given instance.
        /// </summary>
        /// <param name="instance">The object instance from which to retrieve the member value.</param>
        /// <param name="fieldOrProperty">The name of the field or property to retrieve.</param>
        /// <param name="value">When this method returns, contains the value of the member if it exists; otherwise, null.
        /// This parameter is passed uninitialized.</param>
        /// <returns>true if the member exists and the value was successfully retrieved; otherwise, false.</returns>
        public bool TryGetMemberValue(object instance, string fieldOrProperty, out object? value)
        {
            return TryGetFieldValue(instance, fieldOrProperty, out value) || TryGetPropertyValue(instance, fieldOrProperty, out value);
        }

        /// <summary>
        /// Attempts to retrieve a method member by its signature (name and arguments).
        /// </summary>
        /// <param name="name">The name of the method to retrieve.</param>
        /// <param name="args">The arguments to the method.  Used to select the correct overload.</param>
        /// <param name="method">When this method returns, contains the MethodInfo if found; otherwise, null.</param>
        /// <returns>True if the method was found; otherwise, false.</returns>
        public bool TryGetMethodMember(string name, object[]? args, out MethodInfo? method)
        {
            if (!Methods.TryGetValue(name, out var methods))
            {
                method = null;
                return false;
            }
            method = TryGetMethodImplementation(methods, args);
            return method is not null;
        }

        /// <summary>
        /// Attempts to retrieve the value of a property from the given instance.
        /// </summary>
        /// <param name="instance">The object instance from which to retrieve the property value.</param>
        /// <param name="property">The name of the property to retrieve.</param>
        /// <param name="value">When this method returns, contains the value of the property if it exists; otherwise, null.
        /// This parameter is passed uninitialized.</param>
        /// <returns>true if the property exists and the value was successfully retrieved; otherwise, false.</returns>
        public bool TryGetPropertyValue(object instance, string property, out object? value)
        {
            if (Properties.TryGetValue(property, out var propertyInfo))
            {
                value = propertyInfo.GetValue(instance);
                return true;
            }

            value = null;
            return false;
        }

        /// <summary>
        /// Attempts to invoke a method with no parameters on the given instance.
        /// </summary>
        /// <param name="instance">The object instance on which to invoke the method.</param>
        /// <param name="method">The name of the method to invoke.</param>
        /// <returns>true if the method was found and successfully invoked; otherwise, false.</returns>
        public bool TryInvokeMethod(object instance, string method)
        {
            return TryInvokeMethod(instance, method, Array.Empty<object>());
        }

        /// <summary>
        /// Attempts to invoke a method with no parameters on the given instance and retrieve the return value.
        /// </summary>
        /// <param name="instance">The object instance on which to invoke the method.</param>
        /// <param name="method">The name of the method to invoke.</param>
        /// <param name="value">When this method returns, contains the return value of the method if it was successfully invoked; otherwise, null.</param>
        /// <returns>true if the method was found and successfully invoked; otherwise, false.</returns>
        public bool TryInvokeMethod(object instance, string method, out object? value)
        {
            return TryInvokeMethod(instance, method, Array.Empty<object>(), out value);
        }

        /// <summary>
        /// Attempts to invoke a method with the given name and arguments on the given instance.
        /// </summary>
        /// <param name="instance">The object instance on which to invoke the method.</param>
        /// <param name="method">The name of the method to invoke.</param>
        /// <param name="args">The arguments to pass to the method.</param>
        /// <returns>true if the method was found and successfully invoked; otherwise, false.</returns>
        public bool TryInvokeMethod(object instance, string method, object[] args)
        {
            return TryInvokeMethod(instance, method, args, out _);
        }

        /// <summary>
        /// Attempts to invoke a method with the given name and arguments on the given instance and retrieve the return value.
        /// </summary>
        /// <param name="instance">The object instance on which to invoke the method.</param>
        /// <param name="method">The name of the method to invoke.</param>
        /// <param name="args">The arguments to pass to the method.</param>
        /// <param name="value">When this method returns, contains the return value of the method if it was successfully invoked; otherwise, null.</param>
        /// <returns>true if the method was found and successfully invoked; otherwise, false.</returns>
        public bool TryInvokeMethod(object instance, string method, object[] args, out object? value)
        {
            value = null;
            if (!Methods.TryGetValue(method, out var implementations))
            {
                return false;
            }

            var methodInfo = TryGetMethodImplementation(implementations, args);
            if (methodInfo == null)
            {
                return false;
            }

            return methodInfo.TryInvoke(instance, out value, args);
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Checks if the signature of a method matches the provided arguments.
        /// </summary>
        /// <param name="method">The method to check.</param>
        /// <param name="parameters">The parameter information for the method.</param>
        /// <param name="args">The arguments to be passed to the method.</param>
        /// <returns>True if the method signature matches the provided arguments, false otherwise.</returns>
        private bool IsSignatureMatch(MethodInfo method, ParameterInfo[] parameters, object[] args)
        {
            if (parameters.Length == 0 && (args == null || args.Length == 0))
            {
                return true;
            }

            if (parameters.Length < (args?.Length ?? 0))
            {
                return false;
            }

            for (int i = 0; i < (args?.Length ?? 0); i++)
            {
                var parameter = parameters[i];
                var arg = args[i];

                if (arg == null && parameter.ParameterType.IsClass)
                {
                    continue; // Null is acceptable for class types
                }

                if (arg == null && parameter.ParameterType.IsValueType && Nullable.GetUnderlyingType(parameter.ParameterType) != null)
                {
                    continue; // Null is acceptable for nullable value types
                }

                if (arg != null && !parameter.ParameterType.IsInstanceOfType(arg))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Maps the members of a given type to the internal dictionaries (Properties, Fields, and Methods).
        /// </summary>
        /// <param name="type">The type whose members should be mapped.</param>
        private void MapMembers(Type type)
        {
            var allInstanceMembers = type?.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly) ?? Array.Empty<MemberInfo>();
            MapMembers(allInstanceMembers);
        }

        /// <summary>
        /// Maps the members of a given collection to the internal dictionaries (Properties, Fields, and Methods).
        /// </summary>
        /// <param name="members">The collection of MemberInfo objects whose members should be mapped.</param>
        private void MapMembers(IEnumerable<MemberInfo> members)
        {
            foreach (var member in members)
            {
                switch (member)
                {
                    case PropertyInfo propertyInfo:
                        Properties[propertyInfo.Name] = propertyInfo;
                        break;

                    case FieldInfo fieldInfo:
                        Fields[fieldInfo.Name] = fieldInfo;
                        break;

                    case MethodInfo methodInfo:
                        if (methodInfo.IsSpecialName)
                        {
                            // Skip special methods (e.g., property accessors).
                            break;
                        }
                        if (!Methods.TryGetValue(methodInfo.Name, out var methodList))
                        {
                            methodList = new List<MethodInfo>();
                            Methods[methodInfo.Name] = methodList;
                        }
                        methodList.Add(methodInfo);
                        break;
                }
            }
        }

        /// <summary>
        /// Attempts to find a matching method implementation from a list of methods based on the provided arguments.
        /// It first tries to find an exact match. If that fails, it looks for a method with optional parameters that can accommodate the given arguments.
        /// </summary>
        /// <param name="methods">The list of MethodInfo objects to search through.</param>
        /// <param name="args">The arguments to be passed to the method.</param>
        /// <returns>The MethodInfo object that matches the provided arguments, or null if no match is found.</returns>
        private MethodInfo? TryGetMethodImplementation(List<MethodInfo> methods, object[]? args)
        {
            if (methods.Count == 0)
            {
                return null;
            }

            var argLength = args?.Length ?? 0;

            // Try to find an exact match first
            foreach (var method in methods)
            {
                MethodInfo? concreteMethod = null;
                var parameters = method.GetParameters();
                if (method.IsGenericMethodDefinition)
                {
                    // Check if we can construct a concrete method from the arguments
                    try
                    {
                        concreteMethod = method.MakeGenericMethod(args.Select(a => a.GetType()).ToArray());
                        parameters = concreteMethod.GetParameters();
                    }
                    catch (ArgumentException)
                    {
                        continue; //  The provided arguments did not satisfy the constraints of the generic method definition.
                    }
                }
                else if (parameters.Length != argLength)
                {
                    continue;
                }

                if (concreteMethod == null)
                {
                    concreteMethod = method;
                }

                if (IsSignatureMatch(concreteMethod, parameters, args))
                {
                    return concreteMethod;
                }
            }

            // If no exact match is found, look for a method with optional parameters
            foreach (var method in methods)
            {
                MethodInfo? concreteMethod = null;
                var parameters = method.GetParameters();
                if (method.IsGenericMethodDefinition)
                {
                    // Check if we can construct a concrete method from the arguments
                    try
                    {
                        concreteMethod = method.MakeGenericMethod(args.Select(a => a.GetType()).ToArray());
                        parameters = concreteMethod.GetParameters();
                    }
                    catch (ArgumentException)
                    {
                        continue; //  The provided arguments did not satisfy the constraints of the generic method definition.
                    }
                }
                else if (parameters.Length < argLength)
                {
                    continue;
                }

                var requiredParams = parameters.Count(p => !p.IsOptional);
                if (requiredParams > argLength)
                {
                    continue;
                }

                if (concreteMethod == null)
                {
                    concreteMethod = method;
                }

                if (IsSignatureMatch(concreteMethod, parameters, args))
                {
                    return concreteMethod;
                }
            }

            return null;
        }

        #endregion Private Methods
    }
}