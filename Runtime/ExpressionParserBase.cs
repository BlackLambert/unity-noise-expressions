using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace SBaier.Expressions
{
    public class ExpressionParserBase
    {
        private readonly ExpressionSettings _settings;
        private readonly ComputeShaderSettings _shaderSettings;
        private readonly ExpressionTokenizer _tokenizer;
        
        protected List<Operation> _operations;
        protected Variables _variables;
        private Random _random;
        
        public ExpressionParserBase(ExpressionSettings settings, ComputeShaderSettings shaderSettings,
            Random random)
        {
            _tokenizer = new ExpressionTokenizer();
            _settings = settings;
            _shaderSettings = shaderSettings;
            _random = new Random(random.Next(int.MaxValue));
            _operations = new List<Operation>();
            _variables = new Variables();
        }
        
        public MutableExpression AddVariable(string name, string expressionName, bool cleanValue = true)
        {
            MutableExpression variableOutput = new MutableExpression(expressionName);
            _variables.Set(new Variable(name.ToLower(), variableOutput), cleanValue);
            return variableOutput;
        }

        public void AddVariable(string name, string expressionName, ComputeBuffer values, bool cleanValue = true)
        {
            MutableExpression variableOutput = new MutableExpression(expressionName);
            variableOutput.Set(values);
            _variables.Set(new Variable(name.ToLower(), variableOutput), cleanValue);
        }

        public void AddVariable(string name, string expressionName, float[] values, bool cleanValue = true)
        {
            ComputeBuffer buffer = new ComputeBuffer(values.Length, sizeof(float));
            buffer.SetData(values);
            AddVariable(name, expressionName, buffer, cleanValue);
        }

        protected BaseExpression ParseOutput()
        {
            BaseExpression output;
            
            try
            {
                output = ParseBaseExpression();
            }
            catch (Exception exception)
            {
                Debug.LogError("Failed to parse noise expression. Please make sure the expression is correct.\n" +
                               $"Exception: {exception.Message}");
                throw;
            }

            return output;
        }

        private BaseExpression ParseBaseExpression()
        {
            ExpressionParser parser = new ExpressionParser(_tokenizer, _settings.Expression, _variables, _operations, 
                AddVariable, _shaderSettings, _random);
            return new BaseExpression(parser.Parse(), _variables);
        }

        private void AddVariable(string name)
        {
            VariableSettings variableSettings = _settings.Variables.FirstOrDefault(variable => string.Equals(variable.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (variableSettings == null)
            {
                throw new ArgumentException($"There is no variable with name '{name}' defined in the settings");
            }
            
            ExpressionParser parser = new ExpressionParser(_tokenizer, variableSettings.Expression, _variables,
                _operations, AddVariable, _shaderSettings, _random);
            Variable variable = new Variable(name, parser.Parse());
            _variables.Set(variable);
        }
    }
}