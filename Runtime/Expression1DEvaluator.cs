using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Expressions
{
    public class Expression1DEvaluator : ExpressionEvaluatorBase
    {
        private MutableExpression _input;
        
        public Expression1DEvaluator(
            List<Operation> operations, 
            BaseExpression output, 
            Variables variables,
            MutableExpression input) : base(operations, output, variables)
        {
            _input = input;
        }
        
        public ComputeBuffer Evaluate(float[] input)
        {
            ComputeBuffer xBuffer = SetInput("x", _input, input);
            ComputeBuffer result = Evaluate();
            xBuffer.Release();
            return result;
        }

        public ComputeBuffer Evaluate(ComputeBuffer input)
        {
            SetInput("x", _input, input);
            return Evaluate();
        }
    }
}