using UnityEngine;

namespace SBaier.Expressions
{
    public interface ComputeShaderSettings
    {
        public ComputeShader TernaryOperations { get; }
        public ComputeShader BinaryOperations { get; }
        public ComputeShader UnaryOperations { get; }
        public ComputeShader SimplexNoise { get; }
        public ComputeShader CopyBuffer { get; }
    }
}