using Random = System.Random;

namespace SBaier.Expressions
{
    public class Expression0DParser : ExpressionParserBase
    {
        public Expression0DParser(ExpressionSettings settings, ComputeShaderSettings shaderSettings, Random random) : 
            base(settings, shaderSettings, random)
        {
        }
        
        public Expression0DEvaluator Parse()
        {
            BaseExpression output = ParseOutput();
            return new Expression0DEvaluator(_operations, output, _variables);
        }
    }
}