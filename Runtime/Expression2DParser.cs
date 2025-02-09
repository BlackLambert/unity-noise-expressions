using Random = System.Random;

namespace SBaier.Expressions
{
    public class Expression2DParser : ExpressionParserBase
    {
        private const string _x = "x";
        private const string _xExpressionName = "X Input";
        private const string _y = "y";
        private const string _yExpressionName = "Y Input";
        
        public Expression2DParser(ExpressionSettings settings, ComputeShaderSettings shaderSettings,
            Random random) : base(settings, shaderSettings, new Random(random.Next(int.MaxValue)))
        {

        }

        public Expression2DEvaluator Parse()
        {
            MutableExpression xInput = AddVariable(_x, _xExpressionName);
            MutableExpression yInput = AddVariable(_y, _yExpressionName);
            BaseExpression output = ParseOutput();
            return new Expression2DEvaluator(_operations, output, _variables, xInput, yInput);
        }
    }
}