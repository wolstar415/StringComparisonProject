using System;

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

    public Comparison(CompareOp op, FlexibleValue reference)
    {
        _op = op;
        _reference = reference;
    }

    public bool Check(FlexibleValue input)
    {
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
                _ => false
            };
        }

        if (_reference.IsNumber && input.IsNumber)
        {
            float a = input.AsFloat();
            float b = _reference.AsFloat();

            return _op switch
            {
                CompareOp.Equal => AreFloatsEqual(a, b),
                CompareOp.NotEqual => !AreFloatsEqual(a, b),
                CompareOp.Less => a < b,
                CompareOp.LessEqual => a <= b || AreFloatsEqual(a, b),
                CompareOp.Greater => a > b,
                CompareOp.GreaterEqual => a >= b || AreFloatsEqual(a, b),
                _ => false
            };
        }

        if (_reference.IsString && input.IsString)
        {
            string a = input.Text;
            string b = _reference.Text;

            return _op switch
            {
                CompareOp.Equal => string.Equals(a, b, StringComparison.OrdinalIgnoreCase),
                CompareOp.NotEqual => !string.Equals(a, b, StringComparison.OrdinalIgnoreCase),
                _ => false
            };
        }

        return input.EqualsTo(in _reference);
    }

    public override string ToString()
    {
        string opStr = _op switch
        {
            CompareOp.Less => "<",
            CompareOp.LessEqual => "<=",
            CompareOp.Greater => ">",
            CompareOp.GreaterEqual => ">=",
            CompareOp.Equal => "==",
            CompareOp.NotEqual => "!=",
            _ => "?"
        };

        return $"{opStr}{_reference}";
    }

    public static Comparison Parse(string expr)
    {
        if (string.IsNullOrWhiteSpace(expr))
            return new Comparison(CompareOp.Equal, FlexibleValue.None);

        expr = expr.Trim();

        if (expr.Length >= 2 && expr[0] == '!' && expr[1] != '=')
        {
            var raw = expr[1..].Trim();
            return new Comparison(CompareOp.NotEqual, FlexibleValue.Parse(raw));
        }

        foreach (var (symbol, op) in _operators)
        {
            if (expr.StartsWith(symbol, StringComparison.Ordinal))
            {
                var valueStr = expr[symbol.Length..].Trim();
                return new Comparison(op, FlexibleValue.Parse(valueStr));
            }
        }

        return new Comparison(CompareOp.Equal, FlexibleValue.Parse(expr));
    }

    private static bool AreFloatsEqual(float a, float b)
    {
        return Math.Abs(a - b) < 0.00001f;
    }

    private static readonly (string, CompareOp)[] _operators = new[]
    {
        ("<=", CompareOp.LessEqual),
        (">=", CompareOp.GreaterEqual),
        ("<", CompareOp.Less),
        (">", CompareOp.Greater),
        ("==", CompareOp.Equal),
        ("!=", CompareOp.NotEqual),
    };
}
