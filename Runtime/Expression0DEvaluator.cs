using System.Collections.Generic;

namespace SBaier.Expressions
{
    public class Expression0DEvaluator : ExpressionEvaluatorBase
    {
        public Expression0DEvaluator(
            List<Operation> operations, 
            BaseExpression output,
            Variables variables) : base(operations, output, variables)
        {
 
        }
    }
}