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
        #region Private Fields

        private readonly ObjectMemberMap _memberMap;

        #endregion Private Fields

        #region Public Constructors

        public ObjectMemberMapTests()
        {
            _memberMap = new ObjectMemberMap(typeof(Models.TestObject));
        }

        #endregion Public Constructors

        #region Public Methods

        [Fact]
        public void MapMembers_MapsPublicPropertiesFieldsAndMethods()
        {
            // Arrange
            var expectedMethodCount = 7;

            // Act
            var map = new ObjectMemberMap(typeof(TestObject));

            // Assert
            Assert.Single(map.Properties);
            Assert.Single(map.Fields);
            Assert.Equal(expectedMethodCount, map.Methods.Count);
        }

        [Fact]
        public void TryGetFieldValue_ReturnsFalseAndNull_WhenFieldDoesNotExist()
        {
            // Arrange
            var obj = new TestObject();
            var map = new ObjectMemberMap(typeof(TestObject));

            // Act
            bool result = map.TryGetFieldValue(obj, "NonExistentField", out var value);

            // Assert
            Assert.False(result);
            Assert.Null(value);
        }

        [Fact]
        public void TryGetFieldValue_ReturnsTrueAndValue_WhenFieldExists()
        {
            // Arrange
            var obj = new TestObject { PublicField = "test" };
            var map = new ObjectMemberMap(typeof(TestObject));
            var expectedValue = "test";

            bool result = map.TryGetFieldValue(obj, "PublicField", out var value);

            Assert.True(result);
            Assert.Equal(expectedValue, value);
        }

        [Fact]
        public void TryGetMemberValue_ReturnsTrueAndValue_WhenFieldOrPropertyExists()
        {
            // Arrange
            var obj = new TestObject { PublicProperty = 42, PublicField = "test" };
            var map = new ObjectMemberMap(typeof(TestObject));
            var expectedPropertyValue = 42;
            var expectedFieldValue = "test";

            // Act
            var couldGetProperty = map.TryGetMemberValue(obj, "PublicProperty", out var propertyValue);
            var couldGetField = map.TryGetMemberValue(obj, "PublicField", out var fieldValue);
            var couldGetNonMember = map.TryGetMemberValue(obj, "NonExistentMember", out var nonExistentValue);

            // Assert
            Assert.True(couldGetProperty);
            Assert.Equal(expectedPropertyValue, propertyValue);
            Assert.True(couldGetField);
            Assert.Equal(expectedFieldValue, fieldValue);
            Assert.False(couldGetNonMember);
            Assert.Null(nonExistentValue);
        }

        [Fact]
        public void TryGetPropertyValue_ReturnsFalseAndNull_WhenPropertyDoesNotExist()
        {
            // Arrange
            var obj = new TestObject();
            var map = new ObjectMemberMap(typeof(TestObject));

            // Act
            bool result = map.TryGetPropertyValue(obj, "NonExistentProperty", out var value);

            // Assert
            Assert.False(result);
            Assert.Null(value);
        }

        [Fact]
        public void TryGetPropertyValue_ReturnsTrueAndValue_WhenPropertyExists()
        {
            // Arrange
            var obj = new TestObject { PublicProperty = 42 };
            var map = new ObjectMemberMap(typeof(TestObject));
            var expectedValue = 42;

            //Act
            bool result = map.TryGetPropertyValue(obj, "PublicProperty", out var value);

            // Assert
            Assert.True(result);
            Assert.Equal(expectedValue, value);
        }

        [Fact]
        public void TryInvokeGenericMethod_ReturnsFalse_WhenMethodParametersAreLessThanArgs()
        {
            // Arrange
            var obj = new TestObject();
            var map = new ObjectMemberMap(typeof(TestObject));

            // Act
            bool actualTwoIntTryInvoke = map.TryInvokeMethod(obj, "GenericMethod", new object[] { 10, 12 }, out var actualTwoIntResult);
            bool actualStringIntTryInvoke = map.TryInvokeMethod(obj, "GenericMethod", new object[] { "hello", 42 }, out var actualStringIntResult);

            // Assert
            Assert.False(actualTwoIntTryInvoke);
            Assert.Null(actualTwoIntResult);

            Assert.False(actualStringIntTryInvoke);
            Assert.Null(actualStringIntResult);
        }

        [Fact]
        public void TryInvokeGenericMethod_ReturnsTrueAndValue_WhenMethodExistsAndIsInvokedWithCorrectType()
        {
            // Arrange
            var obj = new TestObject();
            var map = new ObjectMemberMap(typeof(TestObject));
            var expectedIntResult = 10;
            var expectedSecondIntResult = 12;
            var expectedSingleStringResult = "hello";
            var expectedTwoStringResult = "hello, world!";
            var expectedCacheKeys = new[]
            {
                "GenericMethod[System.Int32]",
                "GenericMethod[System.String,System.String]",
                "GenericMethod[System.String]"
            };
            var expectedCacheCount = expectedCacheKeys.Length;

            // Act
            bool intTryInkvokeResult = map.TryInvokeMethod(obj, "GenericMethod", new object[] { 10 }, out var actualIntResult); // This should generate a cached concrete implementation
            bool secondIntTryInkvokeResult = map.TryInvokeMethod(obj, "GenericMethod", new object[] { 12 }, out var actualSecondIntResult); // This should NOT generate a cached concrete implementation and should reuse the existing cached item
            bool twoStringTryInvokeResult = map.TryInvokeMethod(obj, "GenericMethod", new object[] { "hello, ", "world!" }, out var actualTwoStringResult); // This should generate a cached concrete implementation
            bool singleStringTyInvokeResult = map.TryInvokeMethod(obj, "GenericMethod", new object[] { "hello" }, out var actualSingleStringResult); // This should generate a cached concrete implementation

            // Assert
            Assert.True(intTryInkvokeResult);
            Assert.Equal(expectedIntResult, actualIntResult);

            Assert.True(secondIntTryInkvokeResult);
            Assert.Equal(expectedSecondIntResult, actualSecondIntResult);

            Assert.True(singleStringTyInvokeResult);
            Assert.Equal(expectedSingleStringResult, actualSingleStringResult);

            Assert.True(twoStringTryInvokeResult);
            Assert.Equal(expectedTwoStringResult, actualTwoStringResult);

            Assert.Equal(expectedCacheCount, map.ConcreteGenericMethodCache.Count);
            for (int i = 0; i < expectedCacheCount; i++)
            {
                Assert.True(map.ConcreteGenericMethodCache.TryGetValue(expectedCacheKeys[i], out var actualCachedItem));
                Assert.NotNull(actualCachedItem);
            }
        }

        [Fact]
        public void TryInvokeGenericMethodWithConstraint_ReturnsTrueAndValue_WhenMethodExistsAndIsInvokedWithCorrectType()
        {
            // Arrange
            var obj = new TestObject();
            var map = new ObjectMemberMap(typeof(TestObject));
            string input = "test";
            string suffix = "_suffix";
            var expectedNoOptionalResult = input;
            var expectedWithOptionalResult = input + suffix;

            // Act
            bool actualNoOptionalTryInvokeResult = map.TryInvokeMethod(obj, "GenericMethodWithConstraint", new object[] { input }, out var actualNoOptionalResult);
            bool actualWithOptionalTryInvokeResult = map.TryInvokeMethod(obj, "GenericMethodWithConstraint", new object[] { input, suffix }, out var actualWithOptionalResult);

            // Assert
            Assert.True(actualNoOptionalTryInvokeResult);
            Assert.Equal(expectedNoOptionalResult, actualNoOptionalResult);

            Assert.True(actualWithOptionalTryInvokeResult);
            Assert.Equal(expectedWithOptionalResult, actualWithOptionalResult);
        }

        [Fact]
        public void TryInvokeMethod_ReturnsCorrectSum()
        {
            // Arrange
            var obj = new TestObject();
            var map = new ObjectMemberMap(typeof(TestObject));
            object[] parameters = new object[] { 5, 3 };
            var expectedResult = 8;

            // Act
            bool actualTryInvokeResult = map.TryInvokeMethod(obj, "Add", parameters, out object? actualResult);

            // Assert
            Assert.True(actualTryInvokeResult);
            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public void TryInvokeMethod_ReturnsFalse_WhenMethodDoesNotExist()
        {
            // Arrange
            var obj = new TestObject();
            var map = new ObjectMemberMap(typeof(TestObject));

            // Act
            bool result = map.TryInvokeMethod(obj, "NonExistentMethod");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void TryInvokeMethod_ReturnsTrue_WhenMethodExistsAndIsInvoked()
        {
            // Arrange
            var obj = new TestObject();
            var map = new ObjectMemberMap(typeof(TestObject));

            // Act
            bool result = map.TryInvokeMethod(obj, "PublicMethod");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void TryInvokeMethodWithOptionalParams_ReturnsTrue_WhenMethodExistsAndIsInvokedWithFewerParams()
        {
            // Arrange
            var obj = new TestObject();
            var map = new ObjectMemberMap(typeof(TestObject));

            // Act
            bool result = map.TryInvokeMethod(obj, "PublicMethodWithOptionalParams", new object[] { 1 });

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void TryInvokeMethodWithParams_ReturnsTrue_WhenMethodExistsAndIsInvokedWithCorrectParams()
        {
            // Arrange
            var obj = new TestObject();
            var map = new ObjectMemberMap(typeof(TestObject));

            // Act
            bool result = map.TryInvokeMethod(obj, "PublicMethodWithParams", new object[] { 1, "test" });

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void TryInvokeMethodWithReturnValue_ReturnsTrueAndValue_WhenMethodExistsAndIsInvoked()
        {
            // Arrange
            var obj = new TestObject();
            var map = new ObjectMemberMap(typeof(TestObject));
            var expectedValue = 1;

            // Act
            bool result = map.TryInvokeMethod(obj, "PublicMethodWithReturn", out var value);

            // Assert
            Assert.True(result);
            Assert.Equal(expectedValue, value);
        }

        #endregion Public Methods
    }
}