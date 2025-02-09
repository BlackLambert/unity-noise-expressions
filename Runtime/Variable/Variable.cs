using UnityEngine;

namespace SBaier.Expressions
{
    public class Variable
    {
        public string Name { get; }
        private readonly Expression _expression;
        private ComputeBuffer _expressionResult;

        public Expression Expression => _expression;

        public Variable(string name, Expression expression)
        {
            Name = name;
            _expression = expression;
        }

        public ComputeBuffer GetValue()
        {
            _expressionResult ??= _expression.Evaluate();
            return _expressionResult;
        }

        public void ClearValue()
        {
            _expressionResult?.Dispose();
            _expressionResult = null;
        }
    }
}