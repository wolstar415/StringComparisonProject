using System;
using System.Runtime.CompilerServices;

public enum CompareOp
{
    Less,
    LessEqual,
    Greater,
    GreaterEqual,
    Equal,
    NotEqual
}

public readonly struct Comparison
{
    private readonly CompareOp _op;
    private readonly FlexibleValue _reference;

    private readonly bool _isRange;
    private readonly FlexibleValue _minValue;
    private readonly FlexibleValue _maxValue;
    private readonly CompareOp _minOp;
    private readonly CompareOp _maxOp;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Comparison(CompareOp op, FlexibleValue reference)
    {
        _op = op;
        _reference = reference;
        _isRange = false;
        _minValue = default;
        _maxValue = default;
        _minOp = default;
        _maxOp = default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Comparison(FlexibleValue minValue, CompareOp minOp, FlexibleValue maxValue, CompareOp maxOp)
    {
        _isRange = true;
        _minValue = minValue;
        _maxValue = maxValue;
        _minOp = minOp;
        _maxOp = maxOp;
        _op = default;
        _reference = default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Check(FlexibleValue input)
    {
        if (_isRange)
        {
            float v = input.AsFloat();
            float min = _minValue.AsFloat();
            float max = _maxValue.AsFloat();
            return FastCompare(_minOp, v, min) && FastCompare(_maxOp, v, max);
        }

        if (!_reference.IsValid || !input.IsValid)
            return false;

        if (_reference.IsString && input.IsString)
        {
            string a = input.ToString();
            string b = _reference.ToString();
            return FastCompare(_op, a, b);
        }

        if (_reference.IsBoolean && input.IsBoolean)
        {
            bool a = input.AsBool();
            bool b = _reference.AsBool();
            return FastCompare(_op, a, b);
        }

        if (_reference.IsInteger && input.IsInteger)
        {
            int a = input.AsInt();
            int b = _reference.AsInt();
            return FastCompare(_op, a, b);
        }

        float x = input.AsFloat();
        float y = _reference.AsFloat();
        return FastCompare(_op, x, y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool FastCompare(CompareOp op, int a, int b)
    {
        switch (op)
        {
            case CompareOp.Equal: return a == b;
            case CompareOp.NotEqual: return a != b;
            case CompareOp.Less: return a < b;
            case CompareOp.LessEqual: return a <= b;
            case CompareOp.Greater: return a > b;
            case CompareOp.GreaterEqual: return a >= b;
            default: return false;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool FastCompare(CompareOp op, float a, float b)
    {
        switch (op)
        {
            case CompareOp.Less: return a < b;
            case CompareOp.LessEqual: return a <= b;
            case CompareOp.Greater: return a > b;
            case CompareOp.GreaterEqual: return a >= b;
            case CompareOp.Equal: return AreFloatsEqual(a, b);
            case CompareOp.NotEqual: return !AreFloatsEqual(a, b);
            default: return false;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool FastCompare(CompareOp op, string a, string b)
    {
        switch (op)
        {
            case CompareOp.Equal: return string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
            case CompareOp.NotEqual: return !string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
            default: return false;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool FastCompare(CompareOp op, bool a, bool b)
    {
        switch (op)
        {
            case CompareOp.Equal: return a == b;
            case CompareOp.NotEqual: return a != b;
            default: return false;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool AreFloatsEqual(float a, float b)
    {
        return Math.Abs(a - b) < 0.00001f;
    }

    public static Comparison Parse(string expr)
    {
        if (string.IsNullOrWhiteSpace(expr))
            return new Comparison(CompareOp.Equal, FlexibleValue.None);

        expr = expr.Trim();

        int xIdx = expr.IndexOf('x');
        if (xIdx < 0) xIdx = expr.IndexOf('X');
        if (xIdx >= 0)
        {
            var leftStr = expr.Substring(0, xIdx).Trim();
            var rightStr = expr.Substring(xIdx + 1).Trim();
            var left = ParsePart(leftStr, true);
            var right = ParsePart(rightStr, false);
            return new Comparison(left.Item1, left.Item2, right.Item1, right.Item2);
        }

        foreach (var pair in _operatorsArr)
        {
            if (expr.StartsWith(pair.Item1, StringComparison.Ordinal))
            {
                var valueStr = expr.Substring(pair.Item1.Length).Trim();
                return new Comparison(pair.Item2, FlexibleValue.Parse(valueStr));
            }
        }

        if (expr.StartsWith("!"))
        {
            var valStr = expr.Substring(1).Trim();
            return new Comparison(CompareOp.NotEqual, FlexibleValue.Parse(valStr));
        }

        return new Comparison(CompareOp.Equal, FlexibleValue.Parse(expr));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static (FlexibleValue, CompareOp) ParsePart(string part, bool isLeft)
    {
        foreach (var pair in _operatorsArr)
        {
            if (isLeft && part.EndsWith(pair.Item1, StringComparison.Ordinal))
            {
                var num = part.Substring(0, part.Length - pair.Item1.Length).Trim();
                return (FlexibleValue.Parse(num), pair.Item2);
            }
            else if (!isLeft && part.StartsWith(pair.Item1, StringComparison.Ordinal))
            {
                var num = part.Substring(pair.Item1.Length).Trim();
                return (FlexibleValue.Parse(num), pair.Item2);
            }
        }
        return (FlexibleValue.Parse(part), CompareOp.Equal);
    }

    private static readonly (string, CompareOp)[] _operatorsArr = new[]
    {
        ("<=", CompareOp.LessEqual),
        (">=", CompareOp.GreaterEqual),
        ("<", CompareOp.Less),
        (">", CompareOp.Greater),
        ("==", CompareOp.Equal),
        ("!=", CompareOp.NotEqual)
    };
}
