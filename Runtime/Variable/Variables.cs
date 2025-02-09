using System;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Expressions
{
    public class Variables
    {
        private Dictionary<string, Variable> _variables = new Dictionary<string, Variable>();
        private Dictionary<string, bool> _cleanValue = new Dictionary<string, bool>();

        public void Set(Variable variable, bool cleanValue = true)
        {
            _variables[variable.Name] = variable;
            _cleanValue[variable.Name] = cleanValue;
        }

        public void SetCleanValue(string name, bool cleanValue)
        {
            _cleanValue[name] = cleanValue;
        }

        public bool Has(string name)
        {
            return _variables.ContainsKey(name);
        }

        public Variable Get(string name)
        {
            return _variables[name];
        }

        public void ClearValues()
        {
            foreach (Variable variable in _variables.Values)
            {
                if (_cleanValue[variable.Name])
                {
                    variable.ClearValue();
                }
            }
        }
    }
}