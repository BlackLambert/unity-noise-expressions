using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Expressions
{
    [CreateAssetMenu(fileName = "ExpressionSettings", menuName = "Expression/ExpressionSettings", order = 1)]
    public class ScriptableExpressionSettings : ScriptableObject, ExpressionSettings
    {
        [field: SerializeField]
        public string Expression { get; private set; }
        
        public IReadOnlyList<VariableSettings> Variables => _variables;
        [SerializeField] 
        private List<VariableSettings> _variables = new List<VariableSettings>();
    }
}
