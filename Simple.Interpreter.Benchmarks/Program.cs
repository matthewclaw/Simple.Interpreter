using BenchmarkDotNet.Running;
using Simple.Interpreter.Benchmarks.InterpreterBenchmarks;

namespace Simple.Interpreter.Benchmarks
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var interpreterSummary = BenchmarkRunner.Run<ExpressionInterpreterBenchmarks>();
        }
    }
}
