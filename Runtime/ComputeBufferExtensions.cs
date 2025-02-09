using UnityEngine;

namespace SBaier.Expressions
{
    public static class ComputeBufferExtensions
    {
        public static T[] GetValue<T>(this ComputeBuffer buffer)
        {
            T[] result = new T[buffer.count];
            buffer.GetData(result);
            return result;
        }
        
        public static T GetSingleValue<T>(this ComputeBuffer buffer)
        {
            T[] result = new T[buffer.count];
            buffer.GetData(result);
            return result[0];
        }
        
        public static T[] Resolve<T>(this ComputeBuffer buffer)
        {
            T[] values = buffer.GetValue<T>();
            buffer.Release();
            return values;
        }
        
        public static T ResolveSingle<T>(this ComputeBuffer buffer)
        {
            T value = buffer.GetSingleValue<T>();
            buffer.Release();
            return value;
        }
    }
}