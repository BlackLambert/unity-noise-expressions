using UnityEngine;

namespace SBaier.Expressions
{
    public class BasicComputeShaderSettings : ComputeShaderSettings
    {
        public ComputeShader TernaryOperations { get; }
        public ComputeShader BinaryOperations { get; }
        public ComputeShader UnaryOperations { get; }
        public ComputeShader SimplexNoise { get; }
        public ComputeShader CopyBuffer { get; }

        public BasicComputeShaderSettings(
            ComputeShader ternaryOperations,
            ComputeShader binaryOperations,
            ComputeShader unaryOperations,
            ComputeShader simplexNoise, 
            ComputeShader copyBuffer)
        {
            BinaryOperations = binaryOperations;
            UnaryOperations = unaryOperations;
            TernaryOperations = ternaryOperations;
            SimplexNoise = simplexNoise;
            CopyBuffer = copyBuffer;
        }
    }
}