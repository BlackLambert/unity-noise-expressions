using UnityEngine;

namespace SBaier.Expressions
{
    [CreateAssetMenu(fileName = "ComputeShaderSettings", menuName = "Expression/ComputeShaderSettings", order = 1)]
    public class ScriptableComputeShaderSettings : ScriptableObject, ComputeShaderSettings
    {
        [field: SerializeField]
        public ComputeShader TernaryOperations { get; private set; }
        [field: SerializeField]
        public ComputeShader BinaryOperations { get; private set; }
        [field: SerializeField]
        public ComputeShader UnaryOperations { get; private set; }
        [field: SerializeField]
        public ComputeShader SimplexNoise { get; private set; }
        [field: SerializeField]
        public ComputeShader CopyBuffer { get; private set; }
    }
}
