using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace SBaier.Expressions
{
    public class OctaveNoiseOperation : Operation
    {
        private readonly ComputeShader _noiseShader;
        private readonly ComputeShader _binaryShader;
        private readonly Expression _inputA;
        private readonly Expression _inputB;
        private readonly int _octavesAmount;
        private readonly Expression _inputValueFactor;
        private readonly Expression _outputValueFactor;
        private readonly List<Operation> _operations;
        
        private readonly MutableExpression _output;
        
        public OctaveNoiseOperation(Expression inputA, Expression inputB, int octavesAmount, 
            Expression inputValueFactor, Expression outputValueFactor, MutableExpression output, ComputeShader noiseShader,
            ComputeShader binaryShader, Random random)
        {
            _inputA = inputA;
            _inputB = inputB;
            _octavesAmount = octavesAmount;
            _inputValueFactor = inputValueFactor;
            _outputValueFactor = outputValueFactor;
            _output = output;
            _noiseShader = noiseShader;
            _binaryShader = binaryShader;
            
            _operations = CreateOperations(random);
        }

        public override void Apply()
        {
            foreach (Operation operation in _operations)
            {
                operation.Apply();
            }
        }
        
        private List<Operation> CreateOperations(Random random)
        {
            List<Operation> operations = new List<Operation>();
            float inputFactor = 1;
            float outputFactor = 1;

            Expression formerExpression = new ConstantExpression(0);
            
            for (int i = 0; i < _octavesAmount; i++)
            {
                MutableExpression inputFactorExpressionX = new MutableExpression("Input Factor X");
                operations.Add(new BinaryOperation(_inputValueFactor, new ConstantExpression(inputFactor), inputFactorExpressionX, BinaryOperationType.Multiply, _binaryShader));
                
                MutableExpression xInput = new MutableExpression("Input X");
                operations.Add(new BinaryOperation(_inputA, inputFactorExpressionX, xInput, BinaryOperationType.Multiply, _binaryShader));
                
                MutableExpression inputFactorExpressionY = new MutableExpression("Input Factor Y");
                operations.Add(new BinaryOperation(_inputValueFactor, new ConstantExpression(inputFactor), inputFactorExpressionY, BinaryOperationType.Multiply, _binaryShader));
                
                MutableExpression yInput = new MutableExpression("Input Y");
                operations.Add(new BinaryOperation(_inputB, inputFactorExpressionY, yInput, BinaryOperationType.Multiply, _binaryShader));

                MutableExpression simplex = new MutableExpression("Simplex");
                operations.Add(new SimplexNoiseOperation(xInput, yInput, simplex, _noiseShader, new Random(random.Next(int.MaxValue))));
                
                MutableExpression outputFactorExpression = new MutableExpression("Output Factor");
                operations.Add(new BinaryOperation(_outputValueFactor, new ConstantExpression(outputFactor), outputFactorExpression, BinaryOperationType.Multiply, _binaryShader));
                
                MutableExpression output = new MutableExpression("Output");
                operations.Add(new BinaryOperation(simplex, outputFactorExpression, output, BinaryOperationType.Multiply, _binaryShader));

                MutableExpression addition = new MutableExpression("Add to values");
                operations.Add(new BinaryOperation(formerExpression, output, addition, BinaryOperationType.Add, _binaryShader));
                
                inputFactor *= 2;
                outputFactor *= 0.5f;
                formerExpression = addition;
            }

            operations.Add(new BinaryOperation(formerExpression, new ConstantExpression(0), _output, BinaryOperationType.Add, _binaryShader));
            
            return operations;
        }
    }
}