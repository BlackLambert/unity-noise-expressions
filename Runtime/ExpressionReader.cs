using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace SBaier.Expressions
{
    public class ExpressionReader
    {
        private readonly Regex _xPattern = new Regex(@"\bx\b");
        private readonly Regex _yPattern = new Regex(@"\by\b");

        private readonly ComputeShaderSettings _shaderSettings;
        private readonly Random _random;

        public ExpressionReader(ComputeShaderSettings shaderSettings, Random random)
        {
            _shaderSettings = shaderSettings;
            _random = random;
        }

        public Expression0DParser Read(ExpressionSettings expressionSettings)
        {
            ValidateExpression(0, expressionSettings);
            return new Expression0DParser(expressionSettings, _shaderSettings, _random);
        }

        public Expression1DParser Read1D(ExpressionSettings expressionSettings)
        {
            ValidateExpression(1, expressionSettings);
            return new Expression1DParser(expressionSettings, _shaderSettings, _random);
        }

        public Expression2DParser Read2D(ExpressionSettings expressionSettings)
        {
            ValidateExpression(2, expressionSettings);
            return new Expression2DParser(expressionSettings, _shaderSettings, _random);
        }

        private void ValidateExpression(int expectedParameterAmount, ExpressionSettings expressionSettings)
        {
            bool hasX = Has(_xPattern, expressionSettings);
            bool hasY = Has(_yPattern, expressionSettings);

            bool isValid = expectedParameterAmount switch
            {
                2 => hasX && hasY,
                1 => hasX && !hasY,
                0 => !hasX && !hasY,
                _ => throw new IndexOutOfRangeException()
            };

            if (!isValid)
            {
                throw new InvalidOperationException("Failed to read the provided expression. " +
                                                    "Please make sure to select the Read method matching your the expressions dimensions");
            }
        }

        private bool Has(Regex parameterRegex, ExpressionSettings expressionSettings)
        {
            return Has(parameterRegex, expressionSettings.Expression) ||
                   expressionSettings.Variables.Any(e => Has(parameterRegex, e.Expression));
        }

        private bool Has(Regex parameterRegex, string expression)
        {
            MatchCollection matches = parameterRegex.Matches(expression);
            return matches.Count > 0;
        }
    }
}