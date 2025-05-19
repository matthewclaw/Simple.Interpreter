using Simple.Interpreter.Scoping;
using Simple.Interpreter.Tests.Scoping.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Interpreter.Tests.Scoping
{
    public class VariableTests
    {
        #region Public Methods

        [Fact]
        public void Constructer_SettingIncorrectType_ThrowsArgumentException()
        {
            // Arrange
            var variable = new Variable(typeof(TestObject));
            var wasExceptionThrown = false;

            // Act
            try
            {
                variable.Value = "foo";
            }
            catch
            {
                wasExceptionThrown = true;
            }

            // Assert
            Assert.True(wasExceptionThrown);
        }

        [Fact]
        public void Constructer_WithType_MapsCorrectly()
        {
            // Arrange
            var expectedMethodCount = 8;

            // Act
            var variable = new Variable(typeof(TestObject));

            // Assert
            Assert.Single(variable.MemberMap.Properties);
            Assert.Single(variable.MemberMap.Fields);
            Assert.Equal(expectedMethodCount, variable.MemberMap.Methods.Count);
        }

        #endregion Public Methods
    }
}