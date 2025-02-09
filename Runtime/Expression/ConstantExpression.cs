using UnityEngine;

namespace SBaier.Expressions
{
    public class ConstantExpression : Expression
    {
        private float _value;

        public float Value => _value;
        
        public ConstantExpression(float value)
        {
            _value = value;
        }

        public override ComputeBuffer Evaluate()
        {
            ComputeBuffer buffer = new ComputeBuffer(1, sizeof(float));
            buffer.SetData(new float[] {_value});
            return buffer;
        }
    }
}
