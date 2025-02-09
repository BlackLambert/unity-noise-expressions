using System;
using UnityEngine;

namespace SBaier.Expressions
{
    public class BinaryOperation: Operation
    {
        private readonly BinaryOperationType _type;
        private int _kernelIndex => (int)_type;
        private readonly ComputeShader _shader;
        private readonly Expression _inputA;
        private readonly Expression _inputB;
        public Expression Output => _output;
        private readonly MutableExpression _output;

        

        public BinaryOperation(Expression inputA, Expression inputB, MutableExpression output, BinaryOperationType type, ComputeShader shader)
        {
            _inputA = inputA;
            _inputB = inputB;
            _output = output;
            _type = type;
            _shader = shader;
        }
        public override void Apply()
        {
            // Potential Recursion!
            ComputeBuffer inputA = _inputA.Evaluate();
            ComputeBuffer inputB = _inputB.Evaluate();
            int launchSize = Mathf.Max(inputA.count, inputB.count);
            ComputeBuffer outputBuffer = new ComputeBuffer(launchSize, sizeof(float));

            if (inputA.count != inputB.count && inputA.count > 1 && inputB.count> 1)
            {
                throw new ArgumentException();
            }
            
            int inputAId = Shader.PropertyToID("_InputA");
            int inputBId = Shader.PropertyToID("_InputB");
            int outputId = Shader.PropertyToID("_Output");
            int launchSizeId = Shader.PropertyToID("_LaunchSize");
            int inputDimensionId = Shader.PropertyToID("_InputDimensions");
            int[] inputDimensions = new int[]{inputA.count, inputB.count};
            _shader.SetInt(launchSizeId, launchSize);
            _shader.SetBuffer(_kernelIndex, inputAId, inputA);
            _shader.SetBuffer(_kernelIndex, inputBId, inputB);
            _shader.SetBuffer(_kernelIndex, outputId, outputBuffer);
            _shader.SetInts(inputDimensionId, inputDimensions);
            
            _shader.Dispatch(launchSize, _kernelIndex);
            
            inputA.Release();
            inputB.Release();
            _output.Set(outputBuffer);
        }
    }
}
