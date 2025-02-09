using UnityEngine;

namespace SBaier.Expressions
{
    public abstract class Expression
    {
        public abstract ComputeBuffer Evaluate();
    }
}