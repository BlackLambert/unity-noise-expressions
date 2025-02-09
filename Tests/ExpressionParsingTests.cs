using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using Random = System.Random;

namespace SBaier.Expressions.Tests
{
    public class ExpressionParsingTests
    {
        private static readonly Vector2[] input2D = new[]
        {
            new Vector2(0, 0),
            new Vector2(-2, 3),
            new Vector2(5, 5),
            new Vector2(-3, -6)
        };

        private static readonly float[] input1D = new[]
        {
            0,
            -2f,
            5f,
            -3f
        };

        private static readonly TestData2D[] data2D = new TestData2D[]
        {
            new TestData2D()
            {
                ExpressionSettings = new BasicExpressionSettings("x * (y - 2) / (s + 1)",
                    new List<VariableSettings>() { new VariableSettings("s", "2") }),
                Input = input2D,
                ExpectedOutput = new[] { 0, -2f / 3, 5f, 24f/3 }
            },
            new TestData2D()
            {
                ExpressionSettings = new BasicExpressionSettings("Min(-1, x) * Max(y + 1, s)",
                    new List<VariableSettings>() { new VariableSettings("s", "2") }),
                Input = input2D,
                ExpectedOutput = new[] { -2f, -8f, -6f, -6f }
            },
            new TestData2D()
            {
                ExpressionSettings = new BasicExpressionSettings("Clamp(x, -1, 1) * s",
                    new List<VariableSettings>() { 
                        new VariableSettings("s", "Clamp(y, -1, MaxY)"), 
                        new VariableSettings("MaxY", "4")
                    }),
                Input = input2D,
                ExpectedOutput = new[] { 0, -3f, 4f, 1f }
            },
            new TestData2D()
            {
                ExpressionSettings = new BasicExpressionSettings("x * ((2 + (3 - 1)) - ((y + 0.5) / 2) * abs(-4)) * 2",
                    new List<VariableSettings>() { }),
                Input = input2D,
                ExpectedOutput = new[] { 0, 12, -70f, -90f }
            }
        };

        private static readonly TestData1D[] data1D = new TestData1D[]
        {
            new TestData1D()
            {
                ExpressionSettings = new BasicExpressionSettings("x + 2", new List<VariableSettings>()),
                Input = input1D,
                ExpectedOutput = new[] { 2f, 0, 7f, -1f }
            },
            new TestData1D()
            {
                ExpressionSettings = new BasicExpressionSettings("Min(x, 2) * t", new List<VariableSettings>(){new VariableSettings("t", "-4")}),
                Input = input1D,
                ExpectedOutput = new[] { 0, 8f, -8f, 12f }
            },
            new TestData1D()
            {
                ExpressionSettings = new BasicExpressionSettings("xMin ? -x : !xMin && x <= 0", 
                    new List<VariableSettings>(){new VariableSettings("xMin", "x >= 2")}),
                Input = input1D,
                ExpectedOutput = new[] { 1f, 1f, -5f, 1f}
            },
        };

        private static readonly TestData0D[] data0D = new TestData0D[]
        {
            new TestData0D()
            {
                ExpressionSettings = new BasicExpressionSettings("3 + 2 * 4", new List<VariableSettings>()),
                ExpectedOutput = 11f
            },
            new TestData0D()
            {
                ExpressionSettings = new BasicExpressionSettings("!1", new List<VariableSettings>()),
                ExpectedOutput = 0
            },
            new TestData0D()
            {
                ExpressionSettings = new BasicExpressionSettings("!0", new List<VariableSettings>()),
                ExpectedOutput = 1
            },
            new TestData0D()
            {
                ExpressionSettings = new BasicExpressionSettings("0 ? 1 : -1", new List<VariableSettings>()),
                ExpectedOutput = -1
            },
            new TestData0D()
            {
                ExpressionSettings = new BasicExpressionSettings("4 / 2 + abs(-6) -(2 * 3)", new List<VariableSettings>()),
                ExpectedOutput = 2
            },
            new TestData0D()
            {
                ExpressionSettings = new BasicExpressionSettings("2 > 3 && 5 >= 5 || 1 < 0 ? 30 : -20", new List<VariableSettings>()),
                ExpectedOutput = -20
            },
        };

        private ComputeShaderSettings _computeShaderSettings;

        [SetUp]
        public void Setup()
        {
            ComputeShader unary = Resources.Load("ComputeShader/UnaryOperations") as ComputeShader;
            ComputeShader binary = Resources.Load("ComputeShader/BinaryOperations") as ComputeShader;
            ComputeShader ternary = Resources.Load("ComputeShader/TernaryOperations") as ComputeShader;
            ComputeShader simplex = Resources.Load("ComputeShader/EvaluateNoise2D") as ComputeShader;
            ComputeShader copyBuffer = Resources.Load("ComputeShader/CopyBuffer") as ComputeShader;
            _computeShaderSettings = new BasicComputeShaderSettings(ternary, binary, unary, simplex, copyBuffer);
        }

        [Test]
        public void Parse_2D_CreatesTheExpectedResult([ValueSource(nameof(data2D))] TestData2D testData)
        {
            Expression2DEvaluator evaluator = new ExpressionReader(_computeShaderSettings, new Random(0))
                .Read2D(testData.ExpressionSettings).Parse();
            float[] x = testData.Input.Select(i => i.x).ToArray();
            float[] y = testData.Input.Select(i => i.y).ToArray();
            ComputeBuffer resultBuffer = evaluator.Evaluate(x, y);
            float[] result = resultBuffer.Resolve<float>();
            bool isIdentical = testData.ExpectedOutput.SequenceEqual(result);
            Assert.True(isIdentical, "The expression parser and the evaluator did not produce the expected output");
        }

        [Test]
        public void Parse_1D_CreatesTheExpectedResult([ValueSource(nameof(data1D))] TestData1D testData)
        {
            Expression1DEvaluator evaluator = new ExpressionReader(_computeShaderSettings, new Random(0))
                .Read1D(testData.ExpressionSettings).Parse();
            ComputeBuffer resultBuffer = evaluator.Evaluate(testData.Input);
            float[] result = resultBuffer.Resolve<float>();
            bool isIdentical = testData.ExpectedOutput.SequenceEqual(result);
            Assert.True(isIdentical, "The expression parser and the evaluator did not produce the expected output");
        }

        [Test]
        public void Parse_0D_CreatesTheExpectedResult([ValueSource(nameof(data0D))] TestData0D testData)
        {
            Expression0DEvaluator evaluator = new ExpressionReader(_computeShaderSettings, new Random(0))
                .Read(testData.ExpressionSettings).Parse();
            ComputeBuffer resultBuffer = evaluator.Evaluate();
            float result = resultBuffer.ResolveSingle<float>();
            bool isIdentical = Math.Abs(testData.ExpectedOutput - result) < float.Epsilon;
            Assert.True(isIdentical, "The expression parser and the evaluator did not produce the expected output");
        }

        public class TestData2D
        {
            public ExpressionSettings ExpressionSettings;
            public Vector2[] Input;
            public float[] ExpectedOutput;
        }

        public class TestData1D
        {
            public ExpressionSettings ExpressionSettings;
            public float[] Input;
            public float[] ExpectedOutput;
        }

        public class TestData0D
        {
            public ExpressionSettings ExpressionSettings;
            public float ExpectedOutput;
        }
    }
}