using UnityEngine;

namespace SBaier.Expressions
{
    public class VariableExpression : Expression
    {
        private readonly Variables _variables;
        private readonly string _variableName;
        private readonly ComputeShader _copyShader;

        public VariableExpression(Variables variables, string variableName, ComputeShader copyShader)
        {
            _variables = variables;
            _variableName = variableName;
            _copyShader = copyShader;
        }

        public Variable GetVariable()
        {
            return _variables.Get(_variableName);
        }

        public override ComputeBuffer Evaluate()
        {
            ComputeBuffer variableBuffer = GetVariable().GetValue();
            int launchSize = variableBuffer.count;
            ComputeBuffer result = new ComputeBuffer(launchSize, sizeof(float));
            
            int sourceId = Shader.PropertyToID("_SourceBuffer");
            int destinationId = Shader.PropertyToID("_DestinationBuffer");
            int launchSizeId = Shader.PropertyToID("_LaunchSize");
            
            _copyShader.SetInt(launchSizeId, launchSize);
            _copyShader.SetBuffer(0, sourceId, variableBuffer);
            _copyShader.SetBuffer(0, destinationId, result);

            _copyShader.Dispatch(launchSize, 0);

            return result;
        }
    }
}
