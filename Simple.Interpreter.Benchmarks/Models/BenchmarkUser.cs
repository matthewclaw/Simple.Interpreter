using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Interpreter.Benchmarks.Models
{
    public class BenchmarkUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string SayHello(string to)
        {
            return $"Hi there {to}, I'm {Name}";
        }
        public string GetDomain()
        {
            if (string.IsNullOrEmpty(Email)) return string.Empty;
            return Email.Split('@').Last();
        }
    }
}
