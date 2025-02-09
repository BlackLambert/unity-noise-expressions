using UnityEngine;

namespace SBaier.Expressions
{
    public class UnaryOperation : Operation
    {
        private ComputeShader _shader;
        private Expression _input;
        private MutableExpression _output;

        private UnaryOperationType _type;
        public UnaryOperationType Type => _type;
        private int _kernelIndex => (int)_type;

        public UnaryOperation(Expression operand, MutableExpression output, UnaryOperationType type, ComputeShader shader)
        {
            _input = operand;
            _output = output;
            _type = type;
            _shader = shader;
        }

        public override void Apply()
        {
            ComputeBuffer input = _input.Evaluate();
            int launchSize = input.count;
            ComputeBuffer outputBuffer = new ComputeBuffer(launchSize, sizeof(float));
            int inputId = Shader.PropertyToID("_Input");
            int outId = Shader.PropertyToID("_Output");
            int launchSizeId = Shader.PropertyToID("_LaunchSize");

            _shader.SetInt(launchSizeId, launchSize);
            _shader.SetBuffer(_kernelIndex, inputId, input);
            _shader.SetBuffer(_kernelIndex, outId, outputBuffer);

            _shader.Dispatch(launchSize, _kernelIndex);

            input.Release();
            _output.Set(outputBuffer);
        }
    }
}