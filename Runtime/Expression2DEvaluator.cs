using System;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Expressions
{
    public class Expression2DEvaluator : ExpressionEvaluatorBase
    {
        private MutableExpression _xInput;
        private MutableExpression _yInput;

        public Expression2DEvaluator(
            List<Operation> operations, 
            BaseExpression output, 
            Variables variables,
            MutableExpression xInput, 
            MutableExpression yInput) : base(operations, output, variables)
        {
            _xInput = xInput;
            _yInput = yInput;
        }
        
        public ComputeBuffer Evaluate(float[] x, float[] y)
        {
            if (x.Length != y.Length)
            {
                throw new ArgumentException("The length of x and y need to be equal");
            }

            ComputeBuffer xBuffer = SetInput("x", _xInput, x);
            ComputeBuffer yBuffer = SetInput("y", _yInput, y);
            ComputeBuffer output = Evaluate(xBuffer, yBuffer);
            xBuffer.Release();
            yBuffer.Release();
            
            return output;
        }

        public ComputeBuffer Evaluate(ComputeBuffer xInput, ComputeBuffer yInput)
        {
            SetInput("x", _xInput, xInput);
            SetInput("y", _yInput, yInput);

            return Evaluate();
        }
    }
}