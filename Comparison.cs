using System;
using System.Runtime.CompilerServices;

public enum CompareOp
{
    Less,
    LessEqual,
    Greater,
    GreaterEqual,
    Equal,
    NotEqual,
    ModuloEqual,
    RemainderEqual
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

    public bool Check(FlexibleValue input)
    {
        if (_isRange)
        {
            var v = input.AsFloat();
            return FastCompare(_minOp, v, _minValue.AsFloat()) && FastCompare(_maxOp, v, _maxValue.AsFloat());
        }

        if (!_reference.IsValid || !input.IsValid)
            return false;

        if (_reference.IsInteger && input.IsInteger)
        {
            int a = input.AsInt();
            int b = _reference.AsInt();

            return _op switch
            {
                CompareOp.Equal => a == b,
                CompareOp.NotEqual => a != b,
                CompareOp.Less => a < b,
                CompareOp.LessEqual => a <= b,
                CompareOp.Greater => a > b,
                CompareOp.GreaterEqual => a >= b,
                CompareOp.ModuloEqual => (a / b) == b,
                CompareOp.RemainderEqual => (a % b) == b,
                _ => false
            };
        }

        var x = input.AsFloat();
        var y = _reference.AsFloat();

        return _op switch
        {
            CompareOp.Less => x < y,
            CompareOp.LessEqual => x <= y,
            CompareOp.Greater => x > y,
            CompareOp.GreaterEqual => x >= y,
            CompareOp.Equal => AreFloatsEqual(x, y),
            CompareOp.NotEqual => !AreFloatsEqual(x, y),
            CompareOp.ModuloEqual => AreFloatsEqual(MathF.Floor(x / y), y),
            CompareOp.RemainderEqual => AreFloatsEqual(x % y, y),
            _ => false
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool FastCompare(CompareOp op, float a, float b)
    {
        return op switch
        {
            CompareOp.Less => a < b,
            CompareOp.LessEqual => a <= b,
            CompareOp.Greater => a > b,
            CompareOp.GreaterEqual => a >= b,
            CompareOp.Equal => AreFloatsEqual(a, b),
            CompareOp.NotEqual => !AreFloatsEqual(a, b),
            _ => false
        };
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

        if (expr.Contains("x", StringComparison.OrdinalIgnoreCase))
        {
            var parts = expr.Split(new[] { 'x', 'X' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 2)
            {
                var left = ParsePart(parts[0].Trim(), true);
                var right = ParsePart(parts[1].Trim(), false);
                return new Comparison(left.Item1, left.Item2, right.Item1, right.Item2);
            }
        }

        foreach (var (symbol, op) in _operators)
        {
            if (expr.StartsWith(symbol))
            {
                var valueStr = expr[symbol.Length..].Trim();
                return new Comparison(op, FlexibleValue.Parse(valueStr));
            }
        }

        if (expr.StartsWith("!"))
        {
            var valStr = expr[1..].Trim();
            return new Comparison(CompareOp.NotEqual, FlexibleValue.Parse(valStr));
        }

        return new Comparison(CompareOp.Equal, FlexibleValue.Parse(expr));
    }

    private static (FlexibleValue, CompareOp) ParsePart(string part, bool isLeft)
    {
        foreach (var (symbol, op) in _operators)
        {
            if (isLeft && part.EndsWith(symbol))
            {
                var num = part[..^symbol.Length].Trim();
                return (FlexibleValue.Parse(num), op);
            }
            else if (!isLeft && part.StartsWith(symbol))
            {
                var num = part[symbol.Length..].Trim();
                return (FlexibleValue.Parse(num), op);
            }
        }

        return (FlexibleValue.Parse(part), CompareOp.Equal);
    }

    private static readonly (string, CompareOp)[] _operators = new[]
    {
        ("<=", CompareOp.LessEqual),
        (">=", CompareOp.GreaterEqual),
        ("<", CompareOp.Less),
        (">", CompareOp.Greater),
        ("==", CompareOp.Equal),
        ("!=", CompareOp.NotEqual),
        ("%=", CompareOp.ModuloEqual),
        ("/=" , CompareOp.RemainderEqual),
    };
}
