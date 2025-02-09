using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Expressions
{
    public abstract class ExpressionEvaluatorBase
    {
        private List<Operation> _operations;
        private BaseExpression _output;
        protected Variables _variables;

        protected ExpressionEvaluatorBase(List<Operation> operations, BaseExpression output, Variables variables)
        {
            _output = output;
            _operations = operations;
            _variables = variables;
        }

        public ComputeBuffer Evaluate()
        {
            foreach (Operation operation in _operations)
            {
                operation.Apply();
            }
            
            return _output.Evaluate();
        }

        protected ComputeBuffer SetInput(string variableName, MutableExpression expression, float[] values)
        {
            ComputeBuffer inputBuffer = new ComputeBuffer(values.Length, sizeof(float));
            inputBuffer.SetData(values);
            expression.Set(inputBuffer);
            _variables.SetCleanValue(variableName, true);
            return inputBuffer;
        }

        protected void SetInput(string variableName, MutableExpression expression, ComputeBuffer values)
        {
            expression.Set(values);
            _variables.SetCleanValue(variableName, false);
        }
    }
}