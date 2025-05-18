using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Interpreter.Demo.Examples.Models
{
    public class User
    {
        public required string Name { get; set; }
        public required int Age { get; set; }
        public required string City { get; set; }
        public string SayHi(string to)
        {
            return $"Hi there {to}!, I'm {Name}";
        }
        public override string ToString()
        {
            return $"{Name} (Age: {Age}, City: {City})";
        }
    }
}
