using System.Collections.Generic;

namespace SBaier.Expressions
{
    public class BasicExpressionSettings : ExpressionSettings
    {
        public string Expression { get; private set; }
        public IReadOnlyList<VariableSettings> Variables => _variables;

        private List<VariableSettings> _variables;

        public BasicExpressionSettings(string expression, List<VariableSettings> variables)
        {
            Expression = expression;
            _variables = variables;
        }
    }
}