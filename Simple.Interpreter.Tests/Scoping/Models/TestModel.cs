using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Interpreter.Tests.Scoping.Models
{
    [ExcludeFromCodeCoverage]
    internal class TestModel
    {
        public string Id { get; set; }
        public string Name;
        public void SetName(string name)
        {
            Name = name;
        }
        public void SetId(string id)
        {
            Id = id;
        }
        public void SetName<T>(T model, string suffix = "") where T : class
        {
            Name = (model?.ToString() ?? nameof(model)) + suffix;
        }
        public string GetString<T>(T value, string suffix = "")
        {
            return value.ToString()+suffix;
        }
        public string GetName()
        {
            return Name;
        }
    }
}
