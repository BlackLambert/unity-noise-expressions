using UnityEngine;

namespace SBaier.Expressions
{
    public class BaseExpression : Expression
    {
        private Variables _variables;
        private Expression _innerExpression;

        public BaseExpression(Expression innerExpression, Variables variables)
        {
            _innerExpression = innerExpression;
            _variables = variables;
        }

        public override ComputeBuffer Evaluate()
        {
            ComputeBuffer result = _innerExpression.Evaluate();
            _variables.ClearValues();
            return result;
        }
    }
}