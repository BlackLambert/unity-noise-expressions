using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SBaier.Expressions
{
    public class ExpressionParser
    {
        private const string _minus = "-";
        private const string _plus = "+";
        private const string _multiplicator = "*";
        private const string _divide = "/";
        private const string _abs = "abs";
        private const string _openBracket = "(";
        private const string _closeBracket = ")";
        private const string _min = "min";
        private const string _max = "max";
        private const string _comma = ",";
        private const string _clamp = "clamp";
        private const string _simplex = "simplex";
        private const string _easeInOut = "easeinout";
        private const string _ovtave = "octave";
        private const string _floor = "floor";
        private const string _equals = "==";
        private const string _notEquals = "!=";
        private const string _largerEquals = ">=";
        private const string _lowerEquals = "<=";
        private const string _lower = "<";
        private const string _larger = ">";
        private const string _not = "!";
        private const string _or = "||";
        private const string _and = "&&";
        private const string _conditional = "?";
        private const string _conditionalSeparator = ":";

        private readonly Dictionary<string, PrecedenceGroup> _tokenToGroup = new Dictionary<string, PrecedenceGroup>
        {
            {_minus, PrecedenceGroup.Additive},
            {_plus, PrecedenceGroup.Additive},
            {_multiplicator, PrecedenceGroup.Multiplicative},
            {_divide, PrecedenceGroup.Multiplicative},
            {_abs, PrecedenceGroup.Min},
            {_openBracket, PrecedenceGroup.Min},
            {_closeBracket, PrecedenceGroup.ClosingBracket},
            {_min, PrecedenceGroup.Min},
            {_max, PrecedenceGroup.Min},
            {_comma, PrecedenceGroup.Comma},
            {_clamp, PrecedenceGroup.Min},
            {_simplex, PrecedenceGroup.Min},
            {_easeInOut, PrecedenceGroup.Min},
            {_ovtave, PrecedenceGroup.Min},
            {_floor, PrecedenceGroup.Min},
            {_equals, PrecedenceGroup.Equality},
            {_notEquals, PrecedenceGroup.Equality},
            {_largerEquals, PrecedenceGroup.Relational},
            {_lowerEquals, PrecedenceGroup.Relational},
            {_lower, PrecedenceGroup.Relational},
            {_larger, PrecedenceGroup.Relational},
            {_not, PrecedenceGroup.Unary},
            {_and, PrecedenceGroup.LogicalAnd},
            {_or, PrecedenceGroup.LogicalOr},
            {_conditional, PrecedenceGroup.Conditional},
            {_conditionalSeparator, PrecedenceGroup.ConditionalSeparator},
        };
        
        private readonly string[] _preNegationTokens = { _plus, _multiplicator, _divide, _openBracket, _comma, 
            _conditionalSeparator, _conditional, _equals, _notEquals, _largerEquals, _lowerEquals, _lower,
            _larger, _not, _and, _or
        };
        
        private readonly ExpressionTokenizer _tokenizer;
        private readonly string _expressionString;
        private readonly Variables _variables;
        private readonly AddVariableAction _addVariableAction;
        private readonly ComputeShaderSettings _shaderSettings;
        private readonly Random _random;

        private Stack<Expression> _expressions = new Stack<Expression>();
        private List<string> _tokens;
        private List<Operation> _operations;
        private int _position = -1;

        public ExpressionParser(ExpressionTokenizer tokenizer, string expressionString, Variables variables,
            List<Operation> operations, AddVariableAction addVariableAction, ComputeShaderSettings shaderSettings,
            Random random)
        {
            _tokenizer = tokenizer;
            _expressionString = expressionString;
            _variables = variables;
            _operations = operations;
            _addVariableAction = addVariableAction;
            _shaderSettings = shaderSettings;
            _random = random;
        }

        public Expression Parse()
        {
            _tokens = _tokenizer.Tokenize(_expressionString);
            ParseExpressionRecursive(PrecedenceGroup.Max);

            if (_expressions.Count is > 1 or 0)
            {
                throw new ArgumentException("The amount of resulting expressions is invalid");
            }
            
            return _expressions.Pop();
        }

        private void ParseExpressionRecursive(PrecedenceGroup precedenceLevel)
        {
            if (_position + 1 >= _tokens.Count)
            {
                return;
            }

            string token = GetNextToken();
            ParseExpression(token);

            string nextToken = _position + 1 < _tokens.Count ? _tokens[_position + 1] : string.Empty;
            
            // Stop recursion if the next token is not a number or variable and is within a higher precedence group
            if (string.IsNullOrEmpty(nextToken) || 
                _tokenToGroup.TryGetValue(nextToken, out PrecedenceGroup group) && group >= precedenceLevel)
            {
                if (nextToken == _closeBracket && precedenceLevel == PrecedenceGroup.ClosingBracket)
                {
                    GetNextToken();
                }
                
                return;
            }

            ParseExpressionRecursive(precedenceLevel);
        }

        private void ParseExpression(string token)
        {
            switch (token)
            {
                case _minus:
                    HandleMinus();
                    break;
                case _plus:
                    AddBinaryOperation(BinaryOperationType.Add, "Addition Output", _tokenToGroup[token]);
                    break;
                case _multiplicator:
                    AddBinaryOperation(BinaryOperationType.Multiply, "Multiplication Output", _tokenToGroup[token]);
                    break;
                case _divide:
                    AddBinaryOperation(BinaryOperationType.Divide, "Division Output", _tokenToGroup[token]);
                    break;
                case _conditional:
                    AddTernaryOperation(TernaryOperationType.Conditional, "Conditional Output", _tokenToGroup[token]);
                    break;
                case _equals:
                    AddBinaryOperation(BinaryOperationType.Equals, "Equals Output", _tokenToGroup[token]);
                    break;
                case _notEquals:
                    AddBinaryOperation(BinaryOperationType.NotEquals, "Not Equals Output", _tokenToGroup[token]);
                    break;
                case _largerEquals:
                    AddBinaryOperation(BinaryOperationType.LargerEquals, "Larger Equals Output", _tokenToGroup[token]);
                    break;
                case _lowerEquals:
                    AddBinaryOperation(BinaryOperationType.LowerEquals, "Lower Equals Output", _tokenToGroup[token]);
                    break;
                case _lower:
                    AddBinaryOperation(BinaryOperationType.Lower, "Lower Output", _tokenToGroup[token]);
                    break;
                case _larger:
                    AddBinaryOperation(BinaryOperationType.Larger, "Larger Output", _tokenToGroup[token]);
                    break;
                case _or:
                    AddBinaryOperation(BinaryOperationType.Or, "Or Output", _tokenToGroup[token]);
                    break;
                case _and:
                    AddBinaryOperation(BinaryOperationType.And, "And Output", _tokenToGroup[token]);
                    break;
                case _not:
                    AddUnaryOperation(UnaryOperationType.Not, "Not Output", _tokenToGroup[token]);
                    break;
                case _openBracket:
                    ParseExpressionRecursive(PrecedenceGroup.ClosingBracket);
                    break;
                case _closeBracket:
                case _comma:
                case _conditionalSeparator:
                    break;
                case _abs:
                    AddUnaryBracketOperation(UnaryOperationType.Absolute, "Abs Output");
                    break;
                case _simplex:
                    HandleSimplex();
                    break;
                case _min:
                    AddBinaryBracketOperation(BinaryOperationType.Min, "Min Output");
                    break;
                case _max:
                    AddBinaryBracketOperation(BinaryOperationType.Max, "Max Output");
                    break;
                case _clamp:
                    HandleClamp();
                    break;
                case _easeInOut:
                    AddUnaryBracketOperation(UnaryOperationType.EaseInOut, "EaseInOut Output");
                    break;
                case _ovtave:
                    AddOctavesOperation();
                    break;
                case _floor:
                    AddUnaryBracketOperation(UnaryOperationType.Floor, "Floor Output");
                    break;
                default:
                {
                    if (float.TryParse(token, NumberStyles.Any, CultureInfo.InvariantCulture, out float number))
                    {
                        AddConstant(number);
                    }
                    else
                    {
                        AddVariable(token);
                    }

                    break;
                }
            }
        }

        private void HandleMinus()
        {
            if (IsNegation())
            {
                AddUnaryOperation(UnaryOperationType.Negation, "Negation Output", PrecedenceGroup.Unary);
            }
            else
            {
                AddBinaryOperation(BinaryOperationType.Subtract, "Subtraction Output", PrecedenceGroup.Additive);
            }
        }

        private bool IsNegation()
        {
            string previousToken = _position != 0 ? _tokens[_position - 1] : string.Empty;
            return _position == 0 ||
                   _preNegationTokens.Any(t => t == previousToken);
        }

        private void HandleClamp()
        {
            ValidateOpenBracket();
            AddTernaryOperation(TernaryOperationType.Clamp, "Clamp Output", PrecedenceGroup.ClosingBracket);
        }

        private void AddTernaryOperation(TernaryOperationType operationType, string name, PrecedenceGroup group)
        {
            ParseExpressionRecursive(group);
            Expression input2 = _expressions.Pop();
            Expression input1 = _expressions.Pop();
            Expression input0 = _expressions.Pop();
            MutableExpression output = new MutableExpression(name);
            _operations.Add(new TernaryOperation(input0, input1, input2, output, operationType,
                _shaderSettings.TernaryOperations));
            _expressions.Push(output);
        }

        private void HandleSimplex()
        {
            ValidateOpenBracket();
            ParseExpressionRecursive(PrecedenceGroup.ClosingBracket);
            Expression input2 = _expressions.Pop();
            Expression input1 = _expressions.Pop();
            MutableExpression output = new MutableExpression("Simplex Output");
            _operations.Add(new SimplexNoiseOperation(input1, input2, output,
                _shaderSettings.SimplexNoise, new Random(_random.Next(int.MaxValue))));
            _expressions.Push(output);
        }

        private void AddOctavesOperation()
        {
            ValidateOpenBracket();
            ParseExpressionRecursive(PrecedenceGroup.ClosingBracket);
            Expression outputFactor = _expressions.Pop();
            Expression inputFactor = _expressions.Pop();
            int octaves = GetOctaves(_expressions.Pop());
            Expression input2 = _expressions.Pop();
            Expression input1 = _expressions.Pop();
            MutableExpression output = new MutableExpression("Octave Output");
            _operations.Add(new OctaveNoiseOperation(input1, input2, octaves, inputFactor, outputFactor, output, 
                _shaderSettings.SimplexNoise, _shaderSettings.BinaryOperations, new Random(_random.Next(int.MaxValue))));
            _expressions.Push(output);
        }

        private int GetOctaves(Expression expression)
        {
            ConstantExpression constant;
            
            if (expression is ConstantExpression constantExpression)
            {
                constant = constantExpression;
            }
            else if (expression is VariableExpression variableExpression && 
                     variableExpression.GetVariable().Expression is ConstantExpression constantVariableExpression)
            {
                constant = constantVariableExpression;
            }
            else
            {
                throw new ArgumentException("The amount of octaves need to be a constant expression.");
            }

            return (int)constant.Value;
        }

        private void AddConstant(float number)
        {
            Expression result = new ConstantExpression(number);
            _expressions.Push(result);
        }

        private void AddVariable(string token)
        {
            if (!_variables.Has(token))
            {
                _addVariableAction(token);
            }

            Expression variable = new VariableExpression(_variables, token, _shaderSettings.CopyBuffer);
            _expressions.Push(variable);
        }

        private void AddUnaryBracketOperation(UnaryOperationType type, string name)
        {
            ValidateOpenBracket();
            AddUnaryOperation(type, name, PrecedenceGroup.ClosingBracket);
        }

        private void AddUnaryOperation(UnaryOperationType type, string name, PrecedenceGroup group)
        {
            ParseExpressionRecursive(group);
            Expression input = _expressions.Pop();
            MutableExpression output = new MutableExpression(name);
            _operations.Add(new UnaryOperation(input, output, type,
                _shaderSettings.UnaryOperations));
            _expressions.Push(output);
        }

        private void AddBinaryBracketOperation(BinaryOperationType type, string name)
        {
            ValidateOpenBracket();
            AddBinaryOperation(type, name, PrecedenceGroup.ClosingBracket);
        }

        private void AddBinaryOperation(BinaryOperationType type, string name, PrecedenceGroup group)
        {
            ParseExpressionRecursive(group);
            Expression input2 = _expressions.Pop();
            Expression input1 = _expressions.Pop();
            MutableExpression output = new MutableExpression(name);
            _operations.Add(new BinaryOperation(input1, input2, output, type,
                _shaderSettings.BinaryOperations));
            _expressions.Push(output);
        }

        private void ValidateOpenBracket()
        {
            string nextToken = GetNextToken();

            if (nextToken != _openBracket)
            {
                ThrowProvideArgumentsInBracketsException();
            }
        }

        private void ThrowProvideArgumentsInBracketsException()
        {
            throw new ArgumentException(
                "Failed to parse expression. Please provide arguments separated by comma within round brackets after a function expression like 'Min', 'Max' or 'Simplex'. Example: Min(2, x)");
        }

        private string GetNextToken()
        {
            return _tokens[++_position];
        }

        public delegate void AddVariableAction(string variableName);
    }
}