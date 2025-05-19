using Simple.Interpreter.Scoping;
using Simple.Interpreter.Tests.Scoping.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Interpreter.Tests.Scoping
{
    public class ObjectMemberMapTests
    {
        private readonly ObjectMemberMap _memberMap;
        public ObjectMemberMapTests()
        {
            _memberMap = new ObjectMemberMap(typeof(Models.TestObject));
        }

        [Fact]
        public void MapMembers_MapsPublicPropertiesFieldsAndMethods()
        {
            var map = new ObjectMemberMap(typeof(TestObject));

            Assert.Single(map.Properties);
            Assert.Single(map.Fields);
            Assert.Equal(7, map.Methods.Count); //Includes the inherited methods, like Equals, GetHashCode etc.
        }

        [Fact]
        public void TryGetPropertyValue_ReturnsTrueAndValue_WhenPropertyExists()
        {
            var obj = new TestObject { PublicProperty = 42 };
            var map = new ObjectMemberMap(typeof(TestObject));

            bool result = map.TryGetPropertyValue(obj, "PublicProperty", out var value);

            Assert.True(result);
            Assert.Equal(42, value);
        }

        [Fact]
        public void TryGetPropertyValue_ReturnsFalseAndNull_WhenPropertyDoesNotExist()
        {
            var obj = new TestObject();
            var map = new ObjectMemberMap(typeof(TestObject));

            bool result = map.TryGetPropertyValue(obj, "NonExistentProperty", out var value);

            Assert.False(result);
            Assert.Null(value);
        }

        [Fact]
        public void TryGetFieldValue_ReturnsTrueAndValue_WhenFieldExists()
        {
            var obj = new TestObject { PublicField = "test" };
            var map = new ObjectMemberMap(typeof(TestObject));

            bool result = map.TryGetFieldValue(obj, "PublicField", out var value);

            Assert.True(result);
            Assert.Equal("test", value);
        }

        [Fact]
        public void TryGetFieldValue_ReturnsFalseAndNull_WhenFieldDoesNotExist()
        {
            var obj = new TestObject();
            var map = new ObjectMemberMap(typeof(TestObject));

            bool result = map.TryGetFieldValue(obj, "NonExistentField", out var value);

            Assert.False(result);
            Assert.Null(value);
        }

        [Fact]
        public void TryGetMemberValue_ReturnsTrueAndValue_WhenFieldOrPropertyExists()
        {
            var obj = new TestObject { PublicProperty = 42, PublicField = "test" };
            var map = new ObjectMemberMap(typeof(TestObject));

            Assert.True(map.TryGetMemberValue(obj, "PublicProperty", out var propertyValue));
            Assert.Equal(42, propertyValue);

            Assert.True(map.TryGetMemberValue(obj, "PublicField", out var fieldValue));
            Assert.Equal("test", fieldValue);

            Assert.False(map.TryGetMemberValue(obj, "NonExistentMember", out var nonExistentValue));
            Assert.Null(nonExistentValue);
        }

        [Fact]
        public void TryInvokeMethod_ReturnsTrue_WhenMethodExistsAndIsInvoked()
        {
            var obj = new TestObject();
            var map = new ObjectMemberMap(typeof(TestObject));

            bool result = map.TryInvokeMethod(obj, "PublicMethod");

            Assert.True(result);
        }

        [Fact]
        public void TryInvokeMethodWithReturnValue_ReturnsTrueAndValue_WhenMethodExistsAndIsInvoked()
        {
            var obj = new TestObject();
            var map = new ObjectMemberMap(typeof(TestObject));

            bool result = map.TryInvokeMethod(obj, "PublicMethodWithReturn", out var value);

            Assert.True(result);
            Assert.Equal(1, value);
        }

        [Fact]
        public void TryInvokeMethodWithParams_ReturnsTrue_WhenMethodExistsAndIsInvokedWithCorrectParams()
        {
            var obj = new TestObject();
            var map = new ObjectMemberMap(typeof(TestObject));

            bool result = map.TryInvokeMethod(obj, "PublicMethodWithParams", new object[] { 1, "test" });

            Assert.True(result);
        }

        [Fact]
        public void TryInvokeMethodWithOptionalParams_ReturnsTrue_WhenMethodExistsAndIsInvokedWithFewerParams()
        {
            var obj = new TestObject();
            var map = new ObjectMemberMap(typeof(TestObject));

            bool result = map.TryInvokeMethod(obj, "PublicMethodWithOptionalParams", new object[] { 1 });

            Assert.True(result);
        }

        [Fact]
        public void TryInvokeGenericMethod_ReturnsTrueAndValue_WhenMethodExistsAndIsInvokedWithCorrectType()
        {
            var obj = new TestObject();
            var map = new ObjectMemberMap(typeof(TestObject));

            bool result = map.TryInvokeMethod(obj, "GenericMethod", new object[] { 10 }, out var value);

            Assert.True(result);
            Assert.Equal(10, value);

            result = map.TryInvokeMethod(obj, "GenericMethod", new object[] { "hello" }, out value);

            Assert.True(result);
            Assert.Equal("hello", value);


            result = map.TryInvokeMethod(obj, "GenericMethod", new object[] { "hello, ", "world!" }, out value);

            Assert.True(result);
            Assert.Equal("hello, world!", value);
        }
        [Fact]
        public void TryInvokeGenericMethod_ReturnsFalse_WhenMethodParametersAreLessThanArgs()
        {
            var obj = new TestObject();
            var map = new ObjectMemberMap(typeof(TestObject));

            bool result = map.TryInvokeMethod(obj, "GenericMethod", new object[] { 10, 12 }, out var value);

            Assert.False(result);
            Assert.Null(value);

            result = map.TryInvokeMethod(obj, "GenericMethod", new object[] { "hello", 42 }, out value);

            Assert.False(result);
            Assert.Null(value);
        }

        [Fact]
        public void TryInvokeMethod_ReturnsCorrectSum()
        {
            var obj = new TestObject();
            var map = new ObjectMemberMap(typeof(TestObject));
            object[] parameters = new object[] { 5, 3 };

            bool result = map.TryInvokeMethod(obj, "Add", parameters, out object? value);

            Assert.True(result);
            Assert.Equal(8, value);
        }

        [Fact]
        public void TryInvokeGenericMethodWithConstraint_ReturnsTrueAndValue_WhenMethodExistsAndIsInvokedWithCorrectType()
        {
            var obj = new TestObject();
            var map = new ObjectMemberMap(typeof(TestObject));

            string input = "test";
            bool result = map.TryInvokeMethod(obj, "GenericMethodWithConstraint", new object[] { input }, out var value);

            Assert.True(result);
            Assert.Equal("test", value);
        }

        [Fact]
        public void TryInvokeMethod_ReturnsFalse_WhenMethodDoesNotExist()
        {
            var obj = new TestObject();
            var map = new ObjectMemberMap(typeof(TestObject));

            bool result = map.TryInvokeMethod(obj, "NonExistentMethod");

            Assert.False(result);
        }
    }
}
