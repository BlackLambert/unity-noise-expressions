using System.Collections.Generic;

namespace SBaier.Expressions
{
    public interface ExpressionSettings
    {
        public string Expression { get; }
        public IReadOnlyList<VariableSettings> Variables { get; }
    }
}
