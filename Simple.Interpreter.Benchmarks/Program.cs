using BenchmarkDotNet.Running;
using Simple.Interpreter.Benchmarks.InterpreterBenchmarks;

namespace Simple.Interpreter.Benchmarks
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting Benchmark in 5 seconds...");
            Thread.Sleep(5000);
            var interpreterSummary = BenchmarkRunner.Run<ExpressionInterpreterBenchmarks>();
            Console.WriteLine("Waiting for Profiler snapshot...");
            Thread.Sleep(5000);
            Console.WriteLine("Exiting...");
        }
    }
}
