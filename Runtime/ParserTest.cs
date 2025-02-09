using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = System.Random;

namespace SBaier.Expressions
{
    public class ParserTest : MonoBehaviour
    {
        [SerializeField] 
        private ScriptableExpressionSettings _settings;

        [SerializeField] 
        private ScriptableComputeShaderSettings _shaderSettings;

        [SerializeField] 
        private int _amount = 10_000_000;

        [SerializeField] 
        private Vector2 _vector = new Vector2(2f, 3f);
        

        private void Start()
        {
            ExpressionReader reader = new ExpressionReader(_shaderSettings, new Random(0));
            
            Stopwatch parserStopwatch = new Stopwatch();
            parserStopwatch.Start();
            Expression2DEvaluator result = reader.Read2D(_settings).Parse();
            parserStopwatch.Stop();
            Debug.Log($"Parsing took {parserStopwatch.ElapsedMilliseconds} milliseconds");

            float[] x = new float[_amount];
            float[] y = new float[_amount];

            float vecX = _vector.x;
            float vecY = _vector.y;
            
            for (int i = 0; i < _amount; i++)
            {
                x[i] = vecX;
                y[i] = vecY;
            }

            Stopwatch evalStopwatch = new Stopwatch();
            evalStopwatch.Start();
            ComputeBuffer buffer = result.Evaluate(x, y);
            evalStopwatch.Stop();
            Debug.Log($"Evaluation of {_amount} items took {evalStopwatch.ElapsedMilliseconds} milliseconds");
            
            Stopwatch resolveStopwatch = new Stopwatch();
            resolveStopwatch.Start();
            float[] values = buffer.Resolve<float>();
            resolveStopwatch.Stop();
            Debug.Log($"Resolving took {resolveStopwatch.ElapsedMilliseconds} milliseconds");
            
            Debug.Log(values.Last());
        }
    }
}
