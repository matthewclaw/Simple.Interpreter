using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Simple.Interpreter.Ast;
using Simple.Interpreter.Benchmarks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Interpreter.Benchmarks.InterpreterBenchmarks
{
    [MemoryDiagnoser(true)]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    public class ExpressionInterpreterBenchmarks
    {
        #region Private Fields

        private ExpressionInterpreter _interpreter;
        private BenchmarkUser _user;

        private List<string> _testExpressions;

        #endregion Private Fields

        #region Public Methods

        [Benchmark]
        public ExpressionInterpreter Creation()
        {
            return new ExpressionInterpreter();
        }

        [IterationSetup(Targets = new[] { nameof(RegisterVariableType), nameof(SetVariable_ImplicitTypeRegistration) })]
        public void ColdSetup()
        {
            _interpreter = new ExpressionInterpreter();
        }
        [IterationSetup(Target = nameof(SetVariable_AfterRegisterVariableType))]
        public void IterationSetupWithRegistration()
        {
            _interpreter = new ExpressionInterpreter();
            _interpreter.GlobalScope.RegisterVariableType("user", typeof(BenchmarkUser));
        }

        [Benchmark]
        public void RegisterVariableType()
        {
            _interpreter.GlobalScope.RegisterVariableType("user", typeof(BenchmarkUser));
        }

        [Benchmark]
        public void SetVariable_AfterRegisterVariableType()
        {
            _interpreter.GlobalScope.SetVariable("user", _user);
        }

        [Benchmark]
        public void SetVariable_ImplicitTypeRegistration()
        {
            _interpreter.GlobalScope.SetVariable("user", _user);
        }


        [GlobalSetup]
        public void Setup()
        {
            _user = new BenchmarkUser()
            {
                Email = "alice.foo@benchmark.fake",
                Id = 123,
                LastName = "Foo",
                Name = "Alice"
            };
            _interpreter = new ExpressionInterpreter();

        }


        #endregion Public Methods
        [Benchmark]
        [ArgumentsSource(nameof(TestExpressions))]
        public Expression Interpret(string expression)
        {
            return _interpreter.GetExpression(expression);
        }

        public IEnumerable<string> TestExpressions()
        {
            foreach (var testExpression in _testExpressions)
            {
                yield return testExpression;
            }
        }
        public ExpressionInterpreterBenchmarks()
        {
            _testExpressions = new List<string>
            {
                "1+4 if(5==2) else 4",
                "1+4 if(5 equals 2) else 4",
                "2+3",
                "10/2.5",
                "10+2+3",
                "100 - (10*8)",
                "2>3",
                "2 greater than 3",
                "3>=3",
                "3 is greater or equal to 3",
                "2<=3",
                "2 less or equal to 3",
                "2<1",
                "2 is less than 1",
                "'foo'=='foo'",
                "'foo' is equal to 'foo'",
                "4==5",
                "4 equals 5",
                "4 is 5",
                "4 equal to 5",
                "6!=4",
                "6 not equal to 4",
                "6 is not equal to 4",
                "true and false",
                "true or false"
            };
        }
    }
}