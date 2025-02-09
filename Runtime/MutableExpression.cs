using System;
using UnityEngine;

namespace SBaier.Expressions
{
    public class MutableExpression : Expression
    {
        public string Name { get; }
        private ComputeBuffer _values;

        public MutableExpression(string name)
        {
            Name = name;
        }

        public void Set(ComputeBuffer values)
        {
            _values = values;
        }

        public override ComputeBuffer Evaluate()
        {
            if (_values == null)
            {
                throw new InvalidOperationException($"Please set the value before calling evaluate");
            }
            return _values;
        }
    }
}