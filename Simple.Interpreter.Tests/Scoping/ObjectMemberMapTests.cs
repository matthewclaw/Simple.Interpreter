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
        [Fact]
        public void Create_MapsMembersCorrectly()
        {
            // Arrange
            var testObjectType = typeof(TestModel);
            var testObject = new TestModel();
            // Act
            var actualMap = new ObjectMemberMap(testObjectType);

            // Assert
            var test = actualMap.TryInvokeMethod(testObject, nameof(TestModel.GetString), new object[] { testObject }, out var value); 
        }
    }
}
