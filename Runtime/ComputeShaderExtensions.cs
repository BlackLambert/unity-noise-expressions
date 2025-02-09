using UnityEngine;

namespace SBaier.Expressions
{
    public static class ComputeShaderExtensions
    {
        private const int _maxThreadsPerDispatch = 65535 * 1024;

        public static void Dispatch(this ComputeShader shader, int amount, int kernelIndex)
        {
            shader.GetKernelThreadGroupSizes(kernelIndex, out uint threadGroupSizeX, out uint sizey, out uint sizez );
            
            int totalElements = amount;
            int currentStartIndex = 0;
            int startIndexId = Shader.PropertyToID("_StartIndex");

            while (totalElements > 0)
            {
                int elementsToProcess = Mathf.Min(totalElements, _maxThreadsPerDispatch);
                int numThreadBlocks = Mathf.CeilToInt((float)elementsToProcess / threadGroupSizeX);
                
                shader.SetInt(startIndexId, currentStartIndex);
                shader.Dispatch(kernelIndex, numThreadBlocks, 1, 1);
                currentStartIndex += elementsToProcess;
                totalElements -= elementsToProcess;
            }
        }
    }
}