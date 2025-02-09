using System;
using UnityEngine;

namespace SBaier.Expressions
{
    [Serializable]
    public class VariableSettings
    {
        [field: SerializeField]
        public string Name { get; private set; }
        
        [field: SerializeField]
        public string Expression { get; private set; }

        public VariableSettings(string name, string expression)
        {
            Name = name;
            Expression = expression;
        }
    }
}
