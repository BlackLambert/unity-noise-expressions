using Random = System.Random;

namespace SBaier.Expressions
{
    public class Expression1DParser : ExpressionParserBase
    {
        private const string _x = "x";
        private const string _xExpressionName = "X Input";

        public Expression1DParser(ExpressionSettings settings, ComputeShaderSettings shaderSettings, Random random) :
            base(settings, shaderSettings, random)
        {
        }

        public Expression1DEvaluator Parse()
        {
            MutableExpression xInput = AddVariable(_x, _xExpressionName);
            BaseExpression output = ParseOutput();
            return new Expression1DEvaluator(_operations, output, _variables, xInput);
        }
    }
}