using System;
using UnityEngine;
using Random = System.Random;

namespace SBaier.Expressions
{
    public class SimplexNoiseOperation : Operation
    {
        private const int _maxOffset = 10_000;
        
        private int _kernelIndex => 0;
        private readonly ComputeShader _shader;
        private readonly Expression _inputA;
        private readonly Expression _inputB;
        public Expression Output => _output;
        private readonly MutableExpression _output;
        private int[] _perm;
        private Vector2 _offset;
        
        public SimplexNoiseOperation(Expression inputA, Expression inputB, MutableExpression output, ComputeShader shader,
            Random random)
        {
            _inputA = inputA;
            _inputB = inputB;
            _output = output;
            _shader = shader;
            _perm = NoiseUtil.CreatePermutations(random);
            _offset = NoiseUtil.CreateOffset(random, _maxOffset);
        }
        
        public override void Apply()
        {
            // Potential Recursion!
            ComputeBuffer inputA = _inputA.Evaluate();
            ComputeBuffer inputB = _inputB.Evaluate();

            if (inputA.count != inputB.count && inputA.count > 1 && inputB.count> 1)
            {
                throw new ArgumentException();
            }
            
            int launchSize = Mathf.Max(inputA.count, inputB.count);
            ComputeBuffer outputBuffer = new ComputeBuffer(launchSize, sizeof(float));
            ComputeBuffer permBuffer = new ComputeBuffer(_perm.Length, sizeof(int));
            permBuffer.SetData(_perm);
            int permId = Shader.PropertyToID("_Perm");
            int inputAId = Shader.PropertyToID("_InputPointsX");
            int inputBId = Shader.PropertyToID("_InputPointsY");
            int outputId = Shader.PropertyToID("_OutputValues");
            int launchSizeId = Shader.PropertyToID("_LaunchSize");
            int inputDimensionId = Shader.PropertyToID("_InputDimensions");
            int offsetId = Shader.PropertyToID("_Offset");
            int[] inputDimensions = new int[]{inputA.count, inputB.count};
            _shader.SetInt(launchSizeId, launchSize);
            _shader.SetBuffer(_kernelIndex, permId, permBuffer);
            _shader.SetBuffer(_kernelIndex, inputAId, inputA);
            _shader.SetBuffer(_kernelIndex, inputBId, inputB);
            _shader.SetBuffer(_kernelIndex, outputId, outputBuffer);
            _shader.SetFloats(offsetId, _offset.x, _offset.y);
            _shader.SetInts(inputDimensionId, inputDimensions);
            _shader.Dispatch(launchSize, _kernelIndex);
            
            permBuffer.Release();
            inputA.Release();
            inputB.Release();
            _output.Set(outputBuffer);
        }

        private Gradients[] CombineGradients(float[] gradientValues)
        {
            Gradients[] result = new Gradients[gradientValues.Length / 6];
            for (int i = 0; i < result.Length; i++)
            {
                Vector2 first = new Vector2(gradientValues[i * 6], gradientValues[i * 6 + 1]);
                Vector2 second = new Vector2(gradientValues[i * 6 + 2], gradientValues[i * 6 + 3]);
                Vector2 third = new Vector2(gradientValues[i * 6 + 4], gradientValues[i * 6 + 5]);
                result[i] = new Gradients() { First = first, Second = second, Third = third };
            }

            return result;
        }

        private float[] Evaluate(float[] x, float[] y)
        {
            float[] result = new float [x.Length];
            for (int i = 0; i < x.Length; i++)
            {
                float currentX = x[i] + _offset.x;
                float currentY = y[i] + _offset.y;
                result[i] = Evaluate(currentX, currentY);
            }

            return result;
        }
                
        public float Evaluate(float x, float y)
        {
            float s = (x + y) * 0.36602540378f; // (sqrt(3) - 1) / 2
            int i = FastFloor(x + s);
            int j = FastFloor(y + s);

            float t = (i + j) * 0.2113248654f; // (3 - sqrt(3)) / 6
            float x0 = x - (i - t);
            float y0 = y - (j - t);

            int i1, j1;
            if (x0 > y0)
            {
                i1 = 1; j1 = 0;
            }
            else
            {
                i1 = 0; j1 = 1;
            }

            float x1 = x0 - i1 + 0.2113248654f;
            float y1 = y0 - j1 + 0.2113248654f;
            float x2 = x0 - 1.0f + 0.4226497308f;
            float y2 = y0 - 1.0f + 0.4226497308f;

            int ii = i & 255;
            int jj = j & 255;
            
            int gi0 = _perm[ii + _perm[jj]];
            int gi1 = _perm[ii + i1 + _perm[jj + j1]];
            int gi2 = _perm[ii + 1 + _perm[jj + 1]];
            
            // Noise contributions from the three corners
            double n0, n1, n2;

            // Calculate the contribution from the three corners
            double t0 = 0.5 - x0 * x0 - y0 * y0;
            if (t0 < 0)
                n0 = 0.0;
            else
            {
                t0 *= t0;
                // (x,y) of grad3 used for 2D gradient
                n0 = t0 * t0 * Dot(Grad(gi0), x0, y0);
            }

            double t1 = 0.5 - x1 * x1 - y1 * y1;
            if (t1 < 0)
                n1 = 0.0;
            else
            {
                t1 *= t1;
                n1 = t1 * t1 * Dot(Grad(gi1), x1, y1);
            }

            double t2 = 0.5 - x2 * x2 - y2 * y2;
            if (t2 < 0)
                n2 = 0.0;
            else
            {
                t2 *= t2;
                n2 = t2 * t2 * Dot(Grad(gi2), x2, y2);
            }

            // Add contributions from each corner to get the final noise value.
            // The result is scaled to return values in the interval [0,1].
            return (float)(70.0 * (n0 + n1 + n2));
        }

        private int FastFloor(float x)
        {
            return x > 0 ? (int)x : (int)x - 1;
        }

        private float Dot(Vector2 g, float x, float y)
        {
            return g.x * x + g.y * y;
        }

        private Vector2 Grad(int hash)
        {
            int h = hash & 7;
            Vector2 grad = new Vector2((h < 4) ? 1.0f : 0.0f, (h < 4) ? 0.0f : 1.0f);
            if ((h & 1) != 0) grad.x = -grad.x;
            if ((h & 2) != 0) grad.y = -grad.y;
            return grad;
        }

        private class Gradients
        {
            public Vector2 First;
            public Vector2 Second;
            public Vector2 Third;

            public override string ToString()
            {
                return $"{First} | {Second} | {Third}";
            }
        }
    }
}