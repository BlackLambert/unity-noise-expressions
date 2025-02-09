using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace SBaier.Expressions
{
    public class TokenizerTests
    {
        private static TestData[] data = new TestData[]
        {
            new(){ Input = "1 + 2", ExpectedOutput = new []{"1", "+", "2"}},
            new(){ Input = "y / x * s - |s|", ExpectedOutput = new []{"y", "/", "x", "*", "s", "-", "|", "s", "|"}},
            new(){ Input = "clamp(x, 0, 100)", ExpectedOutput = new []{"clamp", "(", "x", ",", "0", ",", "100", ")"}},
            new(){ Input = "Simplex(x, y) * 0.5", ExpectedOutput = new []{"simplex", "(", "x", ",", "y", ")", "*", "0.5"}},
            new(){ Input = "Max(size, radius)", ExpectedOutput = new []{"max", "(", "size", ",", "radius", ")"}},
            new(){ Input = "x * 32 + Min(2, y)", ExpectedOutput = new []{"x", "*", "32", "+", "min", "(", "2", ",", "y", ")"}},
            new(){ Input = "maxY", ExpectedOutput = new []{"maxy"}},
            new(){ Input = "max1", ExpectedOutput = new []{"max1"}},
            new(){ Input = "1max", ExpectedOutput = new []{"1", "max"}},
            new(){ Input = "easeInOut(t)", ExpectedOutput = new []{"easeinout", "(", "t", ")"}},
            new(){ Input = "1 > 2 < 3 == 4 >= 5 <= 6", ExpectedOutput = new []{"1", ">", "2", "<", "3", "==", "4", ">=", "5", "<=", "6"}},
            new(){ Input = "x > 3 ? x : 3", ExpectedOutput = new []{"x", ">", "3", "?", "x", ":", "3"}},
            new(){ Input = "x != 3 && y > 2 || s <= 4", ExpectedOutput = new []{"x", "!=", "3", "&&", "y", ">", "2", "||", "s", "<=", "4"}},
        };
        
        [Test]
        public void Tokenize_CreatesExpectedTokens([ValueSource(nameof(data))] TestData testData)
        {
            ExpressionTokenizer tokenizer = new ExpressionTokenizer();
            List<string> tokens = tokenizer.Tokenize(testData.Input);
            bool isIdentical = tokens.SequenceEqual(testData.ExpectedOutput);
            Assert.True(isIdentical, "The tokenizer did not produce the expected output");
        }

        public class TestData
        {
            public string Input;
            public string[] ExpectedOutput;
        }
    }
}
