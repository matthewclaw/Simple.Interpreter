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

        #endregion Private Fields

        #region Public Methods

        [Benchmark]
        public ExpressionInterpreter Creation()
        {
            return new ExpressionInterpreter();
        }

        [IterationSetup(Targets = new[] { nameof(RegisterVariableType), nameof(SetScopedVariable_ImplicitTypeRegistration) })]
        public void ColdSetup()
        {
            _interpreter = new ExpressionInterpreter();
        }
        [IterationSetup(Target = nameof(SetScopedVariable_AfterRegisterVariableType))]
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
        public void SetScopedVariable_AfterRegisterVariableType()
        {
            _interpreter.GlobalScope.SetVariable("user", _user);
        }

        [Benchmark]
        public void SetScopedVariable_ImplicitTypeRegistration()
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
    }
}