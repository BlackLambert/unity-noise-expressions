namespace SBaier.Expressions
{
    public enum PrecedenceGroup : byte
    {
        Min = 0,
        Postfix = 1,
        Unary = 2,
        Multiplicative = 3,
        Additive = 4,
        Relational = 5,
        Equality = 6,
        LogicalAnd = 7,
        LogicalOr = 8,
        ConditionalSeparator = 9,
        Conditional = 10,
        Comma = 11,
        ClosingBracket = 12,
        Max = byte.MaxValue
    }
}