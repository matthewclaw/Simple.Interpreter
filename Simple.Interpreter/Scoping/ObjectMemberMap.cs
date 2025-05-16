using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Interpreter.Scoping
{
    public class ObjectMemberMap
    {
        public readonly string ObjectTypeName;
        public readonly Dictionary<string, PropertyInfo> Properties;
        public readonly Dictionary<string, FieldInfo> Fields;
        public readonly Dictionary<string, List<MethodInfo>> Methods;

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
        private void MapMembers(Type type)
        {
            var allInstanceMembers = type?.GetMembers(BindingFlags.Public | BindingFlags.Instance) ?? Array.Empty<MemberInfo>();
            MapMembers(allInstanceMembers);
        }
        private void MapMembers(IEnumerable<MemberInfo> members)
        {
            foreach (var member in members)
            {
                if (member is PropertyInfo propertyInfo)
                {
                    Properties[propertyInfo.Name] = propertyInfo;
                    continue;
                }
                if (member is FieldInfo fieldInfo)
                {
                    Fields[fieldInfo.Name] = fieldInfo;
                    continue;
                }
                if ((member is MethodInfo methodInfo))
                {
                    if (!Methods.ContainsKey(methodInfo.Name))
                    {
                        Methods[methodInfo.Name] = new List<MethodInfo>();
                    }
                    Methods[methodInfo.Name].Add(methodInfo);
                }
            }
        }
        public bool TryGetPropertyValue(object instance, string property, out object? value)
        {
            if (!Properties.TryGetValue(property, out var propertyInfo))
            {
                value = null;
                return false;
            }
            value = propertyInfo.GetValue(instance);
            return true;
        }
        public bool TryGetFieldValue(object instance, string field, out object? value)
        {
            if (!Fields.TryGetValue(field, out var fieldInfo))
            {
                value = null;
                return false;
            }
            value = fieldInfo.GetValue(instance);
            return true;
        }
        public bool TryGetMemberValue(object instance, string fieldOrProperty, out object? value)
        {
            return TryGetFieldValue(instance, fieldOrProperty, out value) || TryGetPropertyValue(instance, fieldOrProperty, out value);
        }

        public bool TryInvokeMethod(object instance, string method)
        {
            throw new NotImplementedException();
        }
        public bool TryInvokeMethod(object instance, string method, out object? value)
        {
            return TryInvokeMethod(instance, method, Array.Empty<object>(), out value);
        }

        public bool TryInvokeMethod(object instance, string method, object[] args)
        {
            return TryInvokeMethod(instance, method, args, out _);
        }
        public bool TryInvokeMethod(object instance, string method, object[] args, out object? value)
        {
            value = null;
            if (!Methods.TryGetValue(method, out var implementations))
            {
                return false;
            }
            if (!TryGetMethodImplementation(implementations, args, out var methodInfo))
            {
                return false;
            }
            try
            {
                var optionalMissingCount = methodInfo!.GetParameters().Length - args.Length;
                if (optionalMissingCount > 0)
                {
                    var oldSize = args.Length;
                    var newSize = oldSize + optionalMissingCount;
                    Array.Resize(ref args, newSize);
                    for (int i = oldSize; i < newSize; i++)
                    {
                        args[i] = Type.Missing;
                    }
                }
                value = methodInfo!.Invoke(instance, args);
                return true;
            }
            catch
            {
                return false;
            }
        }
        private bool IsArgLengthMatch(ParameterInfo[] parameters, int argLenth)
        {
            if (parameters.Length == argLenth)
            {
                return true;
            }
            var requiredCount = parameters.Where(p => !p.IsOptional).Count();
            return requiredCount == argLenth;
        }

        private bool IsSignareMatch(MethodInfo methodInfo, object[]? args)
        {
            var parameters = methodInfo.GetParameters();
            if (parameters.Length == 0 && args?.Length == 0)
            {
                return true;
            }
            if (!IsArgLengthMatch(parameters, args?.Length ?? 0))
            {
                return false;
            }
            return CheckParameterTypes(methodInfo, parameters, args!);
        }

        private bool CheckParameterTypes(MethodInfo methodInfo, ParameterInfo[] parameters, object[] args)
        {
            var generics = methodInfo.GetGenericArguments();
            var genericArgCount = 0;
            for (int i = 0; i < args!.Length; i++)
            {
                if (parameters[i].ParameterType == args![i].GetType())
                {
                    continue;
                }
                if (generics.Length == 0)
                {
                    return false;
                }
                if (genericArgCount >= generics.Length)
                {
                    return false;
                }
                if (generics[genericArgCount].GenericParameterAttributes == GenericParameterAttributes.ReferenceTypeConstraint && args[i].GetType().IsValueType)
                {
                    return false;
                }
                var constraints = generics[genericArgCount].GetGenericParameterConstraints();
                if (constraints is null || constraints.Length == 0)
                {
                    genericArgCount++;
                    continue;
                }
                if (constraints.Any(c => !c.IsAssignableFrom(args![i].GetType())))
                {
                    return false;
                }
                genericArgCount++;
                continue;

            }
            return true;
        }
        private MethodInfo GetConcreteMethodInfo(MethodInfo methodInfo, object[]? args)
        {
            if (args is null)
            {
                return methodInfo;
            }
            return methodInfo.MakeGenericMethod(args.Select(p => p.GetType()).ToArray());
        }
        private bool TryGetMethodImplementation(List<MethodInfo> methods, object[]? args, out MethodInfo? methodInfo)
        {
            var argLenth = args?.Length ?? 0;
            if (methods.Count == 0)
            {
                methodInfo = null;
                return false;
            }
            if (methods.Count == 1)
            {
                methodInfo = methods[0];
                if (!IsArgLengthMatch(methodInfo.GetParameters(), argLenth))
                {
                    methodInfo = null;
                    return false;
                }
            }
            else
            {
                methodInfo = methods.FirstOrDefault(m => IsSignareMatch(m, args));
            }
            if (methodInfo == null)
            {
                return false;
            }
            if (methodInfo.IsGenericMethod)
            {
                methodInfo = GetConcreteMethodInfo(methodInfo, args);
            }
            return true;
        }
    }
}
